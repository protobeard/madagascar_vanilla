using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using PreceptDefOf = MadagascarVanilla.DefOfs.PreceptDefOf;
using ThoughtDefOf = MadagascarVanilla.DefOfs.ThoughtDefOf;

namespace MadagascarVanilla.ClassExtensions
{
    public static class PawnExtensions
    {
        public static bool IsProductionSpecialist(this Pawn pawn)
        {
            Precept_Role role = pawn.Ideo.GetRole(pawn);
            return role != null && role.def == PreceptDefOf.IdeoRole_ProductionSpecialist;
        }
        
        public static bool IsSleepingAlone(this Pawn p)
        {
            ThoughtWorker_WantToSleepWithSpouseOrLover worker = new ThoughtWorker_WantToSleepWithSpouseOrLover();
            worker.def = ThoughtDefOf.WantToSleepWithSpouseOrLover;
            return worker.CurrentState(p).Active;
        }
    }
}