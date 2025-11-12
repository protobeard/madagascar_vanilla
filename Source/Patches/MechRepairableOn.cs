using RimWorld;
using HarmonyLib;
using Verse;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatchCategory("AutoRepairMechs")]
    [HarmonyPatch(typeof(CompMechRepairable))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class MechRepairableOnPatch
    {
        public static void Postfix(CompMechRepairable __instance)
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message("MechRepairableOn Postfix");
            __instance.autoRepair = MadagascarVanillaMod.Persistables.EnableMechRepair;
        }
    }
}