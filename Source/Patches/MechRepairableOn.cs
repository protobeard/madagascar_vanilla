using RimWorld;
using HarmonyLib;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompMechRepairable))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class MechRepairableOnPatch
    {
        public static void Postfix(CompMechRepairable __instance)
        {
            __instance.autoRepair = MadagascarVanillaMod.Persistables.EnableMechRepair;
        }
    }
}