using RimWorld;
using HarmonyLib;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompAutoCut))]
    [HarmonyPatch(nameof(CompAutoCut.PostSpawnSetup))]
    public static class AutoCutPatch
    {
        
        // Enable auto-cut on all things with CompAutoCuts:
        // Wind Turbine and Animal Pens
        public static void Postfix(CompAutoCut __instance)
        {
            __instance.autoCut = MadagascarVanillaMod.Persistables.EnableAutoCut;

            // If we're looking at a Wind Turbine we can force an immediate cut rather than waiting
            // for the next long tick.
            if (__instance.autoCut && __instance is CompAutoCutWindTurbine)
                __instance.DesignatePlantsToCut();
        }
    }
}