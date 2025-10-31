using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using XmlExtensions.Action;
using XmlExtensions.Setting;
using PawnTableDefOf = MadagascarVanilla.DefOfs.PawnTableDefOf;

namespace MadagascarVanilla.Settings
{
    public class ScheduleDefaults : SettingContainer
    {
        private const float HeaderHeight = 35;
        private const float RowSpacing = 5f;
        private const float RowHeight = 20 + RowSpacing;
        private const float BottomBufferSpacing = 5f;
        
        private const int TimeAssignmentSelectorWidth = 191;
        private const int TimeAssignmentSelectorHeight = 65;

        protected override float CalculateHeight(float width)
        {
            // One header row, 4 schedule type rows
            int rows = 5;
            return TimeAssignmentSelectorHeight + (rows * RowHeight) + BottomBufferSpacing + GetDefaultSpacing();
        }
        
        protected override void DrawSettingContents(Rect rect)
        {
            float yOffset = 0;
            List<Pawn> schedulePawns = SchedulePawns();
            float scheduleTypeLabelWidth = 100f;
            
            // lets us select what kind of schedule restriction to paint
            TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(new Rect(rect.x, rect.y, TimeAssignmentSelectorWidth, TimeAssignmentSelectorHeight));
            yOffset += TimeAssignmentSelectorHeight/2f;
            
            PawnTable table = new PawnTable(PawnTableDefOf.DefaultSchedules, (Func<IEnumerable<Pawn>>)(() => schedulePawns), (int)rect.width, (int)(rect.height - yOffset));
            
            PawnColumnWorker_DefaultTimetable pcwdt = new PawnColumnWorker_DefaultTimetable();
            
            pcwdt.DoHeader(new Rect(rect.x + scheduleTypeLabelWidth, rect.y + yOffset, rect.width - scheduleTypeLabelWidth, HeaderHeight), table);
            yOffset += HeaderHeight;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            foreach (Pawn pawn in schedulePawns)
            {
                Widgets.Label(new Rect(rect.x, rect.y + yOffset, scheduleTypeLabelWidth, RowHeight), pawn.Name.ToString().Translate());
                pcwdt.DoCell(new Rect(rect.x + scheduleTypeLabelWidth, rect.y + yOffset, rect.width - scheduleTypeLabelWidth, RowHeight), pawn, table);
                yOffset += RowHeight;
            }
        }
        
        // Set pawn schedules:
        //         NeverSleep           |        Night Shift      |          Biphasic    |         Day Shift   |     Default
        // BodyMastery/Never Sleep Gene > NightOwl > UV Sensitive > Very Sleepy > Sleepy > Low Sleep > Initial > RimWorld Default
        //
        // The constructor in Pawn_TimetableTracker:
        // for (int index = 0; index < 24; ++index)
        //    this.times.Add(index <= 5 || index > 21 ? TimeAssignmentDefOf.Sleep : TimeAssignmentDefOf.Anything);
        public static void SetSchedule(Pawn pawn, MadagascarVanillaPersistables.ScheduleType type = MadagascarVanillaPersistables.ScheduleType.DayShift)
        {
            List<TimeAssignmentDef> timeAssigments = new List<TimeAssignmentDef>();

            switch (type)
            {
                case MadagascarVanillaPersistables.ScheduleType.DayShift:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.DayShift];
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NightShift: 
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.NightShift];
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NeverSleep:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.NeverSleep];
                    break;
                case MadagascarVanillaPersistables.ScheduleType.Biphasic:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[MadagascarVanillaPersistables.ScheduleType.Biphasic];
                    break;
                default:
                    Log.Error("Unknown schedule type: " + type);
                    break;
            }

            // FIXME: don't assign slaves recreation
            pawn.timetable.times.Clear();
            for (int i = 0; i < 24; i++)
            {
                pawn.timetable.times.Add(timeAssigments[i]);
            }
        }
        
        public static void SetDefaultSchedule(Pawn pawn, MadagascarVanillaPersistables.ScheduleType type = MadagascarVanillaPersistables.ScheduleType.DayShift)
        {
            SetDefaultSchedule(pawn.timetable, type);
        }

        public static void SetDefaultSchedule(Pawn_TimetableTracker timetable, MadagascarVanillaPersistables.ScheduleType type = MadagascarVanillaPersistables.ScheduleType.DayShift)
        {
            switch (type)
            {
                case MadagascarVanillaPersistables.ScheduleType.DayShift:
                    timetable.times.Clear();
            
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 4)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 4 && index < 20)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 20 && index < 22)
                            timetable.times.Add(TimeAssignmentDefOf.Joy);
                        else
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                    }
                    break;
                case MadagascarVanillaPersistables.ScheduleType.NightShift:
                    timetable.times.Clear();
                        
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 11)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 11 && index < 19)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 19 && index < 22)
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
                    
                    for (int index = 0; index < 24; ++index)
                    {
                        if (index >= 0 && index < 2)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 2 && index < 10)
                            timetable.times.Add(TimeAssignmentDefOf.Anything);
                        else if (index >= 10 && index < 14)
                            timetable.times.Add(TimeAssignmentDefOf.Sleep);
                        else if (index >= 14 && index < 20)
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
    }
    
    public class PawnColumnWorker_DefaultTimetable : PawnColumnWorker_Timetable
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            // confirm that pawn name matches a ScheduleType -- settings file hasn't gotten weird.
            bool parsed = Enum.TryParse(pawn.Name.ToString(), false, out MadagascarVanillaPersistables.ScheduleType scheduleType);
            if (!parsed)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Trying to set {scheduleType} to {pawn.Name}, an unknown schedule type.");
                return;
            }
            
            MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[scheduleType] = pawn.timetable.times;
            
            base.DoCell(rect, pawn, table);
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
    }
    
    // Clear the schedulesDictionary, then regenerate the defaults by accessing it.
    public class ResetSchedulesAction : ActionContainer
    {
        protected override bool ApplyAction()
        {
            MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary = null;
            MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary.GetHashCode();
            MadagascarVanillaMod.Instance.WriteSettings();
            return true;
        }
    }
}