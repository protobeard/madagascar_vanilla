using System;
using System.Collections.Generic;
using System.Linq;
using MadagascarVanilla.Patches;
using RimWorld;
using UnityEngine;
using UnityEngine.Windows;
using Verse;
using Verse.Sound;

using XmlExtensions;
using XmlExtensions.Setting;

namespace MadagascarVanilla.Settings
{
    public class ScheduleDefaults : SettingContainer
    {
        public const string PersistMedicalSettingsKey = "persistMedicalSettings";

        private const float VerticalElementSpacing = 10f;
        private const float RowSpacing = 6f;
        private const float RowHeight = 25 + RowSpacing;
        private const float BottomBufferSpacing = 5f;
        
        private const int TimeAssignmentSelectorWidth = 191;
        private const int TimeAssignmentSelectorHeight = 65;

        // FIXME: real implementation
        protected override float CalculateHeight(float width)
        {
            int rows = 4;

            GameFont currFont = Verse.Text.Font;
            Verse.Text.Font = GameFont.Small;
            float defaultMedicineDescriptionLabelHeight = (float)Math.Ceiling(Verse.Text.CalcHeight((string)"DefaultMedicineSettingsDesc".Translate(), width));
            Verse.Text.Font = currFont;
            
            return rows * RowHeight + defaultMedicineDescriptionLabelHeight + BottomBufferSpacing + GetDefaultSpacing();
        }
        
        protected override void DrawSettingContents(Rect rect)
        {
            // lets us select what kind of schedule restriction to paint
            TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(new Rect(rect.x, rect.y, TimeAssignmentSelectorWidth , TimeAssignmentSelectorHeight));

            List<Pawn> schedulePawns = SchedulePawns();
            
            PawnTableDef tableDef = DefDatabase<PawnTableDef>.GetNamed("DefaultSchedules");
            PawnTable table = new PawnTable(tableDef, (Func<IEnumerable<Pawn>>)(() => schedulePawns), (int)(rect.width - 20f), (int)(rect.height - 20f));
            
            PawnColumnWorker_DefaultTimetable pcwdt = new PawnColumnWorker_DefaultTimetable();
            
            pcwdt.DoHeader(new Rect(rect.x + 100, rect.y, rect.width - 100, rect.height), table);
            
            float y = 35;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            foreach (Pawn pawn in schedulePawns)
            {
                Widgets.Label(new Rect(rect.x, rect.y + y, rect.width, 25f), pawn.Name.ToString());
                pcwdt.DoCell(new Rect(rect.x + 100, rect.y + y, rect.width - 100, 25f), pawn, table);
                y += 25;
            }
        }

        // TODO: clean this up -- put in a better place, give better name
        // Create temp pawns to use as timetable placeholders for the UI.
        private List<Pawn> SchedulePawns()
        {
            List<Pawn> pawns = new List<Pawn>();
            foreach ((MadagascarVanillaPersistables.ScheduleType type, List<TimeAssignmentDef> timeAssignments) in MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary)
            {
                Pawn pawn = new Pawn();
                pawn.Name = new NameSingle(type.ToString());
                pawn.timetable = new Pawn_TimetableTracker(pawn);
                pawn.timetable.times = timeAssignments;
                pawns.Add(pawn);
            }

            return pawns;
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
        public static void SetSchedule(Pawn pawn, MadagascarVanillaPersistables.ScheduleType type = MadagascarVanillaPersistables.ScheduleType.DayShift, bool reduceSleepForQuickSleepers = false, bool avoidScheduledMoodDebuffs = false)
        {
            bool quickSleeper = reduceSleepForQuickSleepers && pawn.story.traits.HasTrait(TraitDefOf.QuickSleeper);
            int quickSleeperOffset;
            List<TimeAssignmentDef> timeAssigments = new List<TimeAssignmentDef>();

            switch (type)
            {
                case MadagascarVanillaPersistables.ScheduleType.DayShift:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.DayShift];
                    quickSleeperOffset = quickSleeper ? 2 : 0;
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NightShift: 
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.NightShift];
                    quickSleeperOffset = avoidScheduledMoodDebuffs ? 0 : (quickSleeper ? 2 : 0);
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NeverSleep:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.NeverSleep];
                    break;
                case MadagascarVanillaPersistables.ScheduleType.Biphasic:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.Biphasic];
                    quickSleeperOffset = quickSleeper ? 1 : 0;
                    break;
                default:
                    Log.Error("Unknown schedule type: " + type);
                    break;
            }

            // FIXME: don't assign slaves recreation
            // FIXME: support quicksleeper offset. Not sure this is actually possible, as a user could create schedules where all sleep would get removed. The offset
            // only really worked b/c I knew the existing schedule.
            pawn.timetable.times.Clear();
            for (int i = 0; i < 24; i++)
            {
                pawn.timetable.times.Add(timeAssigments[i]);
            }
        }


        public static void SetDefaultSchedule(Pawn pawn, MadagascarVanillaPersistables.ScheduleType type = MadagascarVanillaPersistables.ScheduleType.DayShift, bool reduceSleepForQuickSleepers = false, bool avoidScheduledMoodDebuffs = false)
        {
            bool quickSleeper = reduceSleepForQuickSleepers && pawn.story.traits.HasTrait(TraitDefOf.QuickSleeper);
            SetDefaultSchedule(pawn.timetable, type, quickSleeper, reduceSleepForQuickSleepers, avoidScheduledMoodDebuffs);
        }

        public static void SetDefaultSchedule(Pawn_TimetableTracker timetable, MadagascarVanillaPersistables.ScheduleType type = MadagascarVanillaPersistables.ScheduleType.DayShift, bool quickSleeper = false, bool reduceSleepForQuickSleepers = false, bool avoidScheduledMoodDebuffs = false)
        {
            int quickSleeperOffset;
            
            switch (type)
            {
                case MadagascarVanillaPersistables.ScheduleType.DayShift:
                    timetable.times.Clear();
                    quickSleeperOffset = quickSleeper ? 2 : 0;
            
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 4 - quickSleeperOffset)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 4 - quickSleeperOffset && index < 20)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            timetable.times.Add(TimeAssignmentDefOf.Joy);
                        else
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                    }
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NightShift:
                    timetable.times.Clear();
                    quickSleeperOffset = avoidScheduledMoodDebuffs ? 0 : (quickSleeper ? 2 : 0);
                        
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 11)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 11 && index < 19 - quickSleeperOffset)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 19 - quickSleeperOffset && index < 22)
                            timetable.times.Add(TimeAssignmentDefOf.Joy);
                        else
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                    }
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NeverSleep:
                    timetable.times.Clear();
                    
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 20)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            timetable.times.Add(TimeAssignmentDefOf.Joy);
                        else
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                    }
                    break;
                case MadagascarVanillaPersistables.ScheduleType.Biphasic:
                    timetable.times.Clear();
                    quickSleeperOffset = quickSleeper ? 1 : 0;
                    
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 2 - quickSleeperOffset)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 2 - quickSleeperOffset && index < 10)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 10 && index < 14 - quickSleeperOffset)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 14 - quickSleeperOffset && index < 20)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            timetable.times.Add(TimeAssignmentDefOf.Joy);
                        else
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
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
        // private static TimeAssignmentDef JoyOrAnything(Pawn pawn)
        // {
        //     //Log.Message(pawn.Name + " is Slave of colony? " + pawn.IsSlaveOfColony);
        //     //Log.Message(pawn.Name + " is Slave: " + pawn.IsSlave);
        //     return pawn.IsSlaveOfColony ? TimeAssignmentDefOf.Anything : TimeAssignmentDefOf.Joy;
        // }
        
        
        
        
        

        public class PawnColumnWorker_DefaultTimetable : PawnColumnWorker_Timetable
        {
            public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
            {
                // confirm that pawn name matches a ScheduleType -- settings file hasn't gotten weird.
                bool parsed = Enum.TryParse(pawn.Name.ToString(), false, out MadagascarVanillaPersistables.ScheduleType scheduleType);
                if (!parsed)
                {
                    Log.Message($"Trying to set {scheduleType} to {pawn.Name}, an unknown schedule type.");
                    return;
                }

                Log.Message($"Setting DefaultScheduleDictionary[{scheduleType}] to {pawn.Name}");
                
                MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[scheduleType] = pawn.timetable.times;
                
                base.DoCell(rect, pawn, table);
                
                MadagascarVanillaMod.Instance.WriteSettings();
            }
        }
        

    }
}