using RimWorld;
using Verse;

namespace MadagascarVanilla.DefOfs
{
    [DefOf]
    public static class SpecialThingFilterDefOf
    {
        public static SpecialThingFilterDef AllowRotten;
        public static SpecialThingFilterDef AllowDeadmansApparel;
        
        [MayRequireRoyalty]
        public static SpecialThingFilterDef AllowBiocodedWeapons;
        
        [MayRequireRoyalty]
        public static SpecialThingFilterDef AllowBiocodedApparel;
    }
}