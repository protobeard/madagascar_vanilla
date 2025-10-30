using RimWorld;
using HarmonyLib;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    // Settings that change the defaults in RimWorld
    [HarmonyPatch]
    public static class NewGamePlaysettingsPatches
    {
        public const string LandmarkVisibilityKey = "showExpandingLandmarks";
        public const string AutoHomeAreaKey = "autoHomeArea";
        public const string AutoRebuildKey = "autoRebuild";
        public const string UseWorkPrioritiesKey = "useWorkPriorities";
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            bool landmarkVisibilitySetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, LandmarkVisibilityKey));
            bool autoHomeAreaSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AutoHomeAreaKey));
            bool autoRebuildSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AutoRebuildKey));
            
            bool useWorkPrioritiesSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, UseWorkPrioritiesKey));
            
            if (ModsConfig.OdysseyActive)
                __instance.showExpandingLandmarks = landmarkVisibilitySetting;
            
            __instance.autoHomeArea = autoHomeAreaSetting;
            __instance.autoRebuild = autoRebuildSetting;
            
            __instance.useWorkPriorities = useWorkPrioritiesSetting;
        }
    }
}