using RimWorld;
using Verse;
using HarmonyLib;
using MadagascarVanilla.Settings;
using XmlExtensions;
using GeneDefOf = MadagascarVanilla.DefOfs.GeneDefOf;
using TraitDefOf = MadagascarVanilla.DefOfs.TraitDefOf;


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
        private const string InitialScheduleKey = "initialSchedule";
        private const string InitialNightOwlScheduleKey = "initialNightOwlSchedule";
        private const string InitialBodyMasteryScheduleKey = "initialBodyMasterySchedule";
        private const string InitialUVSensitiveScheduleKey = "initialUVSensitiveSchedule";
        private const string InitialSleepyGeneScheduleKey = "initialSleepyGeneSchedule";
        private const string InitialNeverSleepGeneScheduleKey = "initialNeverSleepGeneSchedule";
        
        // Give pawns initial schedules that better reflect their traits and genes as well as
        // ensuring that all pawns recreate at the same time.
        public static void Postfix(ref Pawn pawn)
        {
            // Bail if pawn does not exist or doesn't have a story yet (won't have traits).
            if (pawn?.story == null)
                return;
            
            bool initialSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialScheduleKey));
            bool initialNightOwlSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialNightOwlScheduleKey));
            bool initialBodyMasterySchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialBodyMasteryScheduleKey));
            bool initialUVSensitiveSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialUVSensitiveScheduleKey));
            bool initialSleepyGeneSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialSleepyGeneScheduleKey));
            bool initialNeverSleepGeneSchedule = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, InitialNeverSleepGeneScheduleKey));
            
            // If the pawn has already had their schedule set, is not of the players faction, not humanlike, or does not have a timetable, no-op.
            Faction faction = pawn.Faction;
            ThingDef def = pawn.def;
            WorldComponentCache cache = Find.World.GetComponent<WorldComponentCache>();

            if (cache.HaveSetSchedules.Contains(pawn))
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Pawn {pawn.Name} is in the cache. Skipping.");
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
                (initialNeverSleepGeneSchedule && pawn.genes.HasActiveGene(GeneDefOf.Neversleep)))
            {
                ScheduleDefaults.SetSchedule(pawn, MadagascarVanillaPersistables.ScheduleType.NeverSleep);
            }
            else if ((initialNightOwlSchedule && pawn.story.traits.HasTrait(TraitDefOf.NightOwl)) ||
                (initialUVSensitiveSchedule && (pawn.genes.HasActiveGene(GeneDefOf.UVSensitivity_Mild) || pawn.genes.HasActiveGene(GeneDefOf.UVSensitivity_Intense))))
            {
                ScheduleDefaults.SetSchedule(pawn, MadagascarVanillaPersistables.ScheduleType.NightShift);
            }
            else if (initialSleepyGeneSchedule && (pawn.genes.HasActiveGene(GeneDefOf.VerySleepy) || pawn.genes.HasActiveGene(GeneDefOf.Sleepy)))
            {
                ScheduleDefaults.SetSchedule(pawn, MadagascarVanillaPersistables.ScheduleType.Biphasic);
            }
            else if ((initialSleepyGeneSchedule && (pawn.genes.HasActiveGene(GeneDefOf.LowSleep))) || 
                      initialSchedule)
            {
                ScheduleDefaults.SetSchedule(pawn, MadagascarVanillaPersistables.ScheduleType.DayShift);
            }
            
            // Add pawn to the list we've already looked at so that we don't reset their schedule
            // the next time AddAndRemoveDynamicComponents is called
            cache.HaveSetSchedules.Add(pawn);
        }
    }
}