using RimWorld;
using Verse;

namespace MadagascarVanilla.DefOfs
{
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef ElectricTailoringBench;
        
        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }
    }
}