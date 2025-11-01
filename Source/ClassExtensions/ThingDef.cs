using Verse;

namespace MadagascarVanilla.ClassExtensions
{
    public static class ThingDefExtensions
    {
        public static bool IsMilk(this ThingDef def)
        {
            return def.label.Contains("Milk");
        }
    }
}