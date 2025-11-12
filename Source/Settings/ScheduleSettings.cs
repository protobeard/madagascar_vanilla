using System;
using System.Collections.Generic;
using System.Linq;
using MadagascarVanilla;
using MadagascarVanilla.Settings;
using RimWorld;
using UnityEngine;
using Verse;
using PawnTableDefOf = MadagascarVanilla.DefOfs.PawnTableDefOf;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private const float ScheduleHeaderHeight = 35;
        private const float ScheduleRowSpacing = 5f;
        private const float ScheduleRowHeight = 20 + ScheduleRowSpacing;
        
        private const int TimeAssignmentSelectorWidth = 191;
        private const int TimeAssignmentSelectorHeight = 65;
        
        private void DoScheduleSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_ScheduleSettingsTitle".Translate());
            listingStandard.Label("MV_ScheduleSettingsDescription".Translate());
            
            listingStandard.CheckboxLabeled("MV_EnableBodyMasterySchedule".Translate(), ref EnableBodyMasterySchedule, "MV_EnableBodyMasteryScheduleTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableNeverSleepGeneSchedule".Translate(), ref EnableNeverSleepGeneSchedule, "MV_EnableNeverSleepGeneScheduleTooltip".Translate());

            if (MadagascarVanillaMod.Instance.CompatibilityManager.Check("EnableNightOwlSchedule", out List<string> packageIds))
            {
                listingStandard.Label("MV_SettingWillTakePrecedence".Translate(packageIds.First()));
            }
            listingStandard.CheckboxLabeled("MV_EnableNightOwlSchedule".Translate(), ref EnableNightOwlSchedule, "MV_EnableNightOwlScheduleTooltip".Translate());
            
            listingStandard.CheckboxLabeled("MV_EnableUVSensitiveSchedule".Translate(), ref EnableUVSensitiveSchedule, "MV_EnableUVSensitiveScheduleTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableSleepyGeneSchedule".Translate(), ref EnableSleepyGeneSchedule, "MV_EnableSleepyGeneScheduleTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableInitialSchedule".Translate(), ref EnableInitialSchedule, "MV_EnableInitialScheduleTooltip".Translate());

            // FIXME: reset schedule button
            
            float yOffset = 0;
            List<Pawn> schedulePawns = SchedulePawns();
            float scheduleTypeLabelWidth = 100f;
            
            // lets us select what kind of schedule restriction to paint
            TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(new Rect(rect.x, listingStandard.CurHeight, TimeAssignmentSelectorWidth, TimeAssignmentSelectorHeight));
            yOffset += TimeAssignmentSelectorHeight/2f;
            
            PawnTable table = new PawnTable(PawnTableDefOf.DefaultSchedules, (Func<IEnumerable<Pawn>>)(() => schedulePawns), (int)rect.width, (int)(listingStandard.CurHeight - yOffset));
            
            PawnColumnWorkerDefaultTimetable pcwdt = new PawnColumnWorkerDefaultTimetable();
            
            pcwdt.DoHeader(new Rect(rect.x + scheduleTypeLabelWidth, listingStandard.CurHeight + yOffset, rect.width - scheduleTypeLabelWidth, ScheduleHeaderHeight), table);
            yOffset += ScheduleHeaderHeight;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            foreach (Pawn pawn in schedulePawns)
            {
                Widgets.Label(new Rect(rect.x, listingStandard.CurHeight + yOffset, scheduleTypeLabelWidth, ScheduleRowHeight), pawn.Name.ToString().Translate());
                pcwdt.DoCell(new Rect(rect.x + scheduleTypeLabelWidth, listingStandard.CurHeight + yOffset, rect.width - scheduleTypeLabelWidth, ScheduleRowHeight), pawn, table);
                yOffset += ScheduleRowHeight;
            }
            
            // FIXME: this is a hack.
            listingStandard.GetRect(yOffset);
            
            // listingStandard.End();
        }
        
        // Set pawn schedules:
        //         NeverSleep           |        Night Shift      |          Biphasic    |         Day Shift   |     Default
        // BodyMastery/Never Sleep Gene > NightOwl > UV Sensitive > Very Sleepy > Sleepy > Low Sleep > Initial > RimWorld Default
        //
        // The constructor in Pawn_TimetableTracker:
        // for (int index = 0; index < 24; ++index)
        //    this.times.Add(index <= 5 || index > 21 ? TimeAssignmentDefOf.Sleep : TimeAssignmentDefOf.Anything);
        public static void SetSchedule(Pawn pawn, ScheduleType type = ScheduleType.DayShift)
        {
            List<TimeAssignmentDef> timeAssigments = new List<TimeAssignmentDef>();

            switch (type)
            {
                case ScheduleType.DayShift:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[ScheduleType.DayShift];
                    break;
                case ScheduleType.NightShift: 
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[ScheduleType.NightShift];
                    break;
                case ScheduleType.NeverSleep:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[ScheduleType.NeverSleep];
                    break;
                case ScheduleType.Biphasic:
                    timeAssigments = MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary[ScheduleType.Biphasic];
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
        
        public static void SetDefaultSchedule(Pawn pawn, ScheduleType type = ScheduleType.DayShift)
        {
            SetDefaultSchedule(pawn.timetable, type);
        }

        public static void SetDefaultSchedule(Pawn_TimetableTracker timetable, ScheduleType type = ScheduleType.DayShift)
        {
            switch (type)
            {
                case ScheduleType.DayShift:
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
                case ScheduleType.NightShift:
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
                case ScheduleType.NeverSleep:
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
                case ScheduleType.Biphasic:
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
            foreach ((ScheduleType type, List<TimeAssignmentDef> timeAssignments) in MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary)
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
    
    public class PawnColumnWorkerDefaultTimetable : PawnColumnWorker_Timetable
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
        }
    }
    
    // FIXME: reset schedules button
    // Clear the schedulesDictionary, then regenerate the defaults by accessing it.
    // public class ResetSchedulesAction : ActionContainer
    // {
    //     protected override bool ApplyAction()
    //     {
    //         MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary = null;
    //         MadagascarVanillaMod.Persistables.DefaultSchedulesDictionary.GetHashCode();
    //         MadagascarVanillaMod.Instance.WriteSettings();
    //         return true;
    //     }
    // }
}

