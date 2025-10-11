using System;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using MadagascarVanilla.Settings;
using Verse;
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
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            bool landmarkVisibilitySetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, LandmarkVisibilitySetting));
            bool autoHomeAreaSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AutoHomeAreaSetting));
            bool autoRebuildSetting = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AutoRebuildSetting));
            
            __instance.showExpandingLandmarks = landmarkVisibilitySetting;
            __instance.autoHomeArea = autoHomeAreaSetting;
            __instance.autoRebuild = autoRebuildSetting;
        }
    }
}