using RimWorld;
using HarmonyLib;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompTreeConnection))]
    [HarmonyPatch(nameof(CompTreeConnection.ConnectionStrengthGainPerHourOfPruning))]
    [HarmonyPatch(MethodType.Getter)]
    public static class GauranlenTreePatch
    {
        // Multiply pruning speed by our setting
        public static void Postfix(CompTreeConnection __instance, ref float __result)
        {
            __result *= MadagascarVanillaMod.Persistables.GauralenPruningSpeedMultiplier;
        }
    }
}