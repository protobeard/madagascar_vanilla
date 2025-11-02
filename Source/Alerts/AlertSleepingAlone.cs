using System.Collections.Generic;
using System.Text;
using MadagascarVanilla.ClassExtensions;
using RimWorld;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Alerts
{
 
    public class AlertSleepingAlone : Alert
    {
        private const string enableSleepingAloneAlertKey = "enableSleepingAloneAlert";
                
        private List<Pawn> sleepingAloneColonistsResult = new List<Pawn>();

        private StringBuilder sb = new StringBuilder();

        private List<Pawn> SleepingAloneColonists
        {
            get
            {
                sleepingAloneColonistsResult.Clear();
                List<Map> maps = Find.Maps;
                foreach (Map map in maps)
                {
                    if (!map.IsPlayerHome)
                        continue;

                    foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
                    {
                        if (pawn.IsSleepingAlone())
                            sleepingAloneColonistsResult.Add(pawn);
                    }
                }
                return sleepingAloneColonistsResult;
            }
        }

        public override string GetLabel()
        {
            return "ColonistsSleepingAlone".Translate(sleepingAloneColonistsResult.Count.ToStringCached());
        }

        public override TaggedString GetExplanation()
        {
            sb.Length = 0;
            foreach (Pawn pawn in sleepingAloneColonistsResult)
            {
                sb.AppendLine("  - " + pawn.NameShortColored.Resolve());
            }
            return "ColonistsSleepingAloneDesc".Translate(sb.ToString().TrimEndNewlines());
        }

        public override AlertReport GetReport()
        {
            bool enableSleepingAloneAlert = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, enableSleepingAloneAlertKey));

            if (enableSleepingAloneAlert)
                return AlertReport.CulpritsAre(SleepingAloneColonists);
            
            return AlertReport.Inactive;
        }
    }   
}