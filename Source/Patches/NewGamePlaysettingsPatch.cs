using RimWorld;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    // Settings that change the defaults in RimWorld
    [HarmonyPatch]
    public static class NewGamePlaysettingsPatches
    {
        public const string LandmarkVisibilitySetting = "showExpandingLandmarks";
        public const string AutoHomeAreaSetting = "autoHomeArea";
        public const string AutoRebuildSetting = "autoRebuild";
        public const string UseWorkPrioritiesSetting = "useWorkPriorities";
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            bool landmarkVisibilitySetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, LandmarkVisibilitySetting));
            bool autoHomeAreaSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AutoHomeAreaSetting));
            bool autoRebuildSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AutoRebuildSetting));
            
            bool useWorkPrioritiesSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, UseWorkPrioritiesSetting));
            
            __instance.showExpandingLandmarks = landmarkVisibilitySetting;
            __instance.autoHomeArea = autoHomeAreaSetting;
            __instance.autoRebuild = autoRebuildSetting;
            
            __instance.useWorkPriorities = useWorkPrioritiesSetting;
        }
    }
}