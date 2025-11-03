using RimWorld;
using Verse;

namespace MadagascarVanilla.DefOfs
{
    [DefOf]
    public static class ThoughtDefOf
    {
        public static ThoughtDef WantToSleepWithSpouseOrLover;
        
        static ThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThoughtDefOf));
        }
    }
}