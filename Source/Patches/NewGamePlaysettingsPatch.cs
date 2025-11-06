using RimWorld;
using HarmonyLib;
using Verse;

namespace MadagascarVanilla.Patches
{
    // Settings that change the defaults in RimWorld
    [HarmonyPatch]
    public static class NewGamePlaysettingsPatches
    {
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            if (ModsConfig.OdysseyActive)
                __instance.showExpandingLandmarks = MadagascarVanillaMod.Persistables.EnableShowExpandingLandmarks;
            
            __instance.autoHomeArea = MadagascarVanillaMod.Persistables.DisableAutoHomeArea;
            __instance.autoRebuild = MadagascarVanillaMod.Persistables.EnableAutoRebuildInHomeArea;
            
            __instance.useWorkPriorities = MadagascarVanillaMod.Persistables.EnableWorkPriorities;
        }
    }
}