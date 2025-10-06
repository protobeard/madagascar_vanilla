using Verse;
using RimWorld;

namespace MadagascarVanilla
{
    [DefOf]
    public static class GeneDefOf
    {
        [MayRequireBiotech] 
        public static GeneDef Neversleep;
        [MayRequireBiotech] 
        public static GeneDef LowSleep;
        [MayRequireBiotech] 
        public static GeneDef Sleepy;
        [MayRequireBiotech] 
        public static GeneDef VerySleepy;
        
        [MayRequireBiotech] 
        public static GeneDef UVSensitivity_Mild;
        [MayRequireBiotech] 
        public static GeneDef UVSensitivity_Intense;
    }
}