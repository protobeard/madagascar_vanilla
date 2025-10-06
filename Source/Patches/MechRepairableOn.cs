using System;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompMechRepairable))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class MechRepairableOnPatch
    {
        private const string EnableMechRepair = "enableMechRepair";
        
        public static void Postfix(CompMechRepairable __instance)
        {
            bool enableMechRepair = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableMechRepair));
            
            __instance.autoRepair = enableMechRepair;
        }
    }
}