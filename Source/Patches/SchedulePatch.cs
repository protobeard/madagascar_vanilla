using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;


namespace MadagascarVanilla.Patches
{
    // Other relevant code locations. Pawn_TimetableTracker is where RimWorld creates the default schedule, but
    // that happens before Pawns are given a story/traits, so it's too early to create a "good" schedule.
    //
    // SetFaction happens later than AddAndRemoveDynamicComponents but might be a fine place to patch, as
    // we really don't need a schedule for pawns that aren't in the Players faction.
    //
    // InteractionWorker Recruit/Enslave attempt seems more like whack-a-mole patching, though it might
    // be necessary to ensure that slaves aren't assigned recreation, since apparently AddAndRemoveDynamicComponents
    // happens before a pawn is considered a slave when they are enslaved.
    //
    // Thing.SpawnSetup just seems too high up the inheritance chain to me (too many Things are not pawns),
    // but may actually just be fine.
    //
    // [HarmonyPatch(typeof(Pawn_TimetableTracker))]
    // [HarmonyPatch(MethodType.Constructor)]
    // [HarmonyPatch(new Type[] { typeof(Pawn) })]
    //
    // [HarmonyPatch(typeof (Pawn), "SetFaction", new System.Type[] {typeof (Faction), typeof (Pawn)})]
    //
    // [HarmonyPatch(typeof (InteractionWorker_RecruitAttempt))]
    // [HarmonyPatch(typeof (InteractionWorker_EnslaveAttempt), "Interacted")]
    //
    // [HarmonyPatch(typeof (Thing), "SpawnSetup")]
    
    [HarmonyPatch(typeof(PawnComponentsUtility))]
    [HarmonyPatch(nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents))]
    public static class SchedulePatch
    {
        private const string InitialSchedule = "initialSchedule";
        private const string InitialNightOwlSchedule = "initialNightOwlSchedule";
        private const string InitialBodyMasterySchedule = "initialBodyMasterySchedule";
        private const string InitialUVSensitiveSchedule = "initialUVSensitiveSchedule";
        private const string InitialSleepyGeneSchedule = "initialSleepyGeneSchedule";
        
        private const string ReduceSleepForQuickSleepers = "reduceSleepForQuickSleepers";
        private const string AvoidScheduledMoodDebuffs = "avoidScheduledMoodDebuffs";
        
        // Give pawns initial schedules that better reflect their traits and genes as well as
        // ensuring that all pawns recreate at the same time.
        public static void Postfix(ref Pawn pawn)
        {
            // Bail if pawn does not exist or doesn't have a story yet (won't have traits).
            if (pawn?.story == null)
                return;
            
            bool initialSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialSchedule));
            bool initialNightOwlSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialNightOwlSchedule));
            bool initialBodyMasterySchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialBodyMasterySchedule));
            bool initialUVSensitiveSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialUVSensitiveSchedule));
            bool initialSleepyGeneSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialSleepyGeneSchedule));
            
            bool reduceSleepForQuickSleepers = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, ReduceSleepForQuickSleepers));
            bool avoidScheduledMoodDebuffs = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AvoidScheduledMoodDebuffs));
            
            // If the pawn has already had their schedule set, is not of the players faction, not humanlike, or does not have a timetable, no-op.
            Faction faction = pawn.Faction;
            ThingDef def = pawn.def;
            WorldComponentCache cache = Find.World.GetComponent<WorldComponentCache>();

            if (cache.HaveSetSchedules.Contains(pawn))
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Pawn {pawn.Name} is in the cache. Skipping.");
                return;
            }
            
            if (faction == null || !faction.IsPlayer)
                return;
            
            if (def == null || (def.race != null && !def.race.Humanlike))
                return;
            
            if (pawn.timetable == null)
                return;

            if (MadagascarVanillaMod.Verbose())
            {
                Log.Message($"Setting schedule for {pawn.Name}");
                foreach(Trait trait in pawn.story.traits.allTraits) 
                    Log.Message("trait: " + trait);
            }
            
            if ((initialBodyMasterySchedule && pawn.story.traits.HasTrait(TraitDefOf.BodyMastery)) || 
                (initialSleepyGeneSchedule && pawn.genes.HasActiveGene(GeneDefOf.Neversleep)))
            {
                SetSchedule(pawn, ScheduleType.NeverSleep, reduceSleepForQuickSleepers);
            }
            else if ((initialNightOwlSchedule && pawn.story.traits.HasTrait(TraitDefOf.NightOwl)) ||
                (initialUVSensitiveSchedule && (pawn.genes.HasActiveGene(GeneDefOf.UVSensitivity_Mild) || pawn.genes.HasActiveGene(GeneDefOf.UVSensitivity_Intense))))
            {
                SetSchedule(pawn, ScheduleType.NightShift, reduceSleepForQuickSleepers, avoidScheduledMoodDebuffs);
            }
            else if (initialSleepyGeneSchedule && (pawn.genes.HasActiveGene(GeneDefOf.VerySleepy) || pawn.genes.HasActiveGene(GeneDefOf.Sleepy)))
            {
                SetSchedule(pawn, ScheduleType.Biphasic, reduceSleepForQuickSleepers);
            }
            else if ((initialSleepyGeneSchedule && (pawn.genes.HasActiveGene(GeneDefOf.LowSleep))) || 
                      initialSchedule)
            {
                SetSchedule(pawn, ScheduleType.DayShift, reduceSleepForQuickSleepers);
            }
            
            // Add pawn to the list we've already looked at so that we don't reset their schedule
            // the next time AddAndRemoveDynamicComponents is called
            cache.HaveSetSchedules.Add(pawn);
        }
        
        // Set pawn schedules:
        //         NeverSleep           |        Night Shift      |          Biphasic    |         Day Shift   |     Default
        // BodyMastery/Never Sleep Gene > NightOwl > UV Sensitive > Very Sleepy > Sleepy > Low Sleep > Initial > RimWorld Default
        // 
        // Modifiers: Quick Sleeper, reduce sleep time by 1/3
        //
        // If avoidScheduledMoodDebuffs then leave sleep schedule for Night Owl/UV Sensitive as sleeping through the day even
        // if the pawn has Quick Sleeper.
        //
        // The constructor in Pawn_TimetableTracker:
        // for (int index = 0; index < 24; ++index)
        //    this.times.Add(index <= 5 || index > 21 ? TimeAssignmentDefOf.Sleep : TimeAssignmentDefOf.Anything);
        private static void SetSchedule(Pawn pawn, ScheduleType type = ScheduleType.DayShift, bool reduceSleepForQuickSleepers = false, bool avoidScheduledMoodDebuffs = false)
        {
            bool quickSleeper = reduceSleepForQuickSleepers && pawn.story.traits.HasTrait(TraitDefOf.QuickSleeper);
            int quickSleeperOffset;
            
            switch (type)
            {
                case ScheduleType.DayShift:
                    pawn.timetable.times.Clear();
                    quickSleeperOffset = quickSleeper ? 2 : 0;
                    
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 4 - quickSleeperOffset)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 4 - quickSleeperOffset && index < 20)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            pawn.timetable.times.Add(JoyOrAnything(pawn));
                        else
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Sleep);
                    }
                    break;
                case ScheduleType.NightShift:
                    pawn.timetable.times.Clear();
                    quickSleeperOffset = avoidScheduledMoodDebuffs ? 0 : (quickSleeper ? 2 : 0);
                        
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 11)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 11 && index < 19 - quickSleeperOffset)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 19 - quickSleeperOffset && index < 22)
                            pawn.timetable.times.Add(JoyOrAnything(pawn));
                        else
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                    }
                    break;
                case ScheduleType.NeverSleep:
                    pawn.timetable.times.Clear();
                    
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 20)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            pawn.timetable.times.Add(JoyOrAnything(pawn));
                        else
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                    }
                    break;
                case ScheduleType.Biphasic:
                    pawn.timetable.times.Clear();
                    quickSleeperOffset = quickSleeper ? 1 : 0;
                    
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 2 - quickSleeperOffset)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 2 - quickSleeperOffset && index < 10)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 10 && index < 14 - quickSleeperOffset)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 14 - quickSleeperOffset && index < 20)
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            pawn.timetable.times.Add(JoyOrAnything(pawn));
                        else
                            pawn.timetable.times.Add(TimeAssignmentDefOf.Sleep);
                    }
                    break;
                default:
                    Log.Error("Unknown schedule type: " + type);
                    break;
            }
        }

        // FIXME: looks like pawns aren't considered slaves at this point in execution, even if enslaved from prison.
        // Need to either patch the enslave interaction or somehow make this trigger later.
        // Might just remove this method, since as of right now it always returns Joy.
        // Since slaves don't need Joy, we don't want to assign it to them in their schedule. Give them Anything instead.
        private static TimeAssignmentDef JoyOrAnything(Pawn pawn)
        {
            //Log.Message(pawn.Name + " is Slave of colony? " + pawn.IsSlaveOfColony);
            //Log.Message(pawn.Name + " is Slave: " + pawn.IsSlave);
            return pawn.IsSlaveOfColony ? TimeAssignmentDefOf.Anything : TimeAssignmentDefOf.Joy;
        }
    }
    
    public enum ScheduleType
    {
        DayShift,
        NightShift,
        Biphasic,
        NeverSleep
    }
}