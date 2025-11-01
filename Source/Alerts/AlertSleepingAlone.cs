using System.Collections.Generic;
using System.Text;
using MadagascarVanilla.ClassExtensions;
using RimWorld;
using Verse;

namespace MadagascarVanilla.Alerts
{
 
    public class AlertSleepingAlone : Alert
    {
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
                        //if (MadagascarVanillaMod.Verbose()) Log.Message($"Evaluating pawn thoughts: {pawn.Name}");

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
            return AlertReport.CulpritsAre(SleepingAloneColonists);
        }
    }   
}