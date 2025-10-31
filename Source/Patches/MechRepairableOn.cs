using RimWorld;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompMechRepairable))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class MechRepairableOnPatch
    {
        private const string EnableMechRepairKey = "enableMechRepair";
        
        public static void Postfix(CompMechRepairable __instance)
        {
            bool enableMechRepair = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableMechRepairKey));
            
            __instance.autoRepair = enableMechRepair;
        }
    }
}