using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using PreceptDefOf = MadagascarVanilla.DefOfs.PreceptDefOf;

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
            List<DirectPawnRelation> list = LovePartnerRelationUtility.ExistingLovePartners(p, allowDead: false);
            bool isSlaveOfColony = p.IsSlaveOfColony;
            if (list.NullOrEmpty())
            {
                return false;
            }
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].otherPawn.IsColonist && !list[i].otherPawn.IsWorldPawn() && list[i].otherPawn.relations.everSeenByPlayer && isSlaveOfColony == list[i].otherPawn.IsSlaveOfColony)
                {
                    if (p.ownership.OwnedBed != null && p.ownership.OwnedBed == list[i].otherPawn.ownership.OwnedBed)
                    {
                        return false;
                    }
                    if (p.ownership.OwnedRoom != null && p.ownership.OwnedRoom == list[i].otherPawn.ownership.OwnedRoom)
                    {
                        return false;
                    }
                    HistoryEventDef historyEventDef = ((list[i].def == PawnRelationDefOf.Spouse) ? HistoryEventDefOf.SharedBed_Spouse : HistoryEventDefOf.SharedBed_NonSpouse);
                    if (new HistoryEvent(historyEventDef, p.Named(HistoryEventArgsNames.Doer)).DoerWillingToDo())
                    {
                        num++;
                    }
                }
            }
            return num > 0;
        }
    }
}