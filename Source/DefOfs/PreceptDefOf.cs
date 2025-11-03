using RimWorld;

namespace MadagascarVanilla.DefOfs
{
    [DefOf]
    public static class PreceptDefOf
    {
        [MayRequireIdeology] 
        public static PreceptDef IdeoRole_ProductionSpecialist;
        
        static PreceptDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PreceptDefOf));
        }
    }
}