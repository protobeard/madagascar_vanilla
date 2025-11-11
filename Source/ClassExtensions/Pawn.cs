using System.Collections.Generic;
using System.Linq;
using RimWorld;
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

        public static bool HasTrait(this Pawn pawn, TraitDef trait)
        {
            return pawn.story.traits.HasTrait(trait);
        }
        
        public static ThingFilter ThingFilterOfWornApparel(this Pawn pawn)
        {
            ThingFilter filter = pawn.outfits.CurrentApparelPolicy.filter;
            IEnumerable<ThingDef> apparelDefs = pawn.apparel.WornApparel.Select(apparel => apparel.def);
            
            IEnumerable<SpecialThingFilterDef> specialThingFilterDefs = DefDatabase<SpecialThingFilterDef>.AllDefs
                .Where(filterDef => !pawn.outfits.CurrentApparelPolicy.filter.Allows(filterDef));
            
            ThingFilter newFilter = new ThingFilter();
            newFilter.CopyAllowancesFrom(filter);
            newFilter.SetDisallowAll(apparelDefs, specialThingFilterDefs);
            
            return newFilter;
        }
    }
}