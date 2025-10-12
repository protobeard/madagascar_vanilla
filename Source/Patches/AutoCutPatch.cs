using RimWorld;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompAutoCut))]
    [HarmonyPatch(nameof(CompAutoCut.PostSpawnSetup))]
    public static class AutoCutPatch
    {
        private const string EnableAutoCut = "enableAutoCut";
        
        // Enable auto-cut on all things with CompAutoCuts:
        // Wind Turbine and Animal Pens
        public static void Postfix(CompAutoCut __instance)
        {
            bool enableAutoCut = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableAutoCut));
            __instance.autoCut = enableAutoCut;

            // If we're looking at a Wind Turbine we can force an immediate cut rather than waiting
            // for the next long tick.
            if (__instance.autoCut && __instance is CompAutoCutWindTurbine)
                __instance.DesignatePlantsToCut();
        }
    }
}