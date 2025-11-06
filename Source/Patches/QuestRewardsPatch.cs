using RimWorld;
using Verse;
using HarmonyLib;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class QuestRewardsPatch
    {
        // When factions are created, set allowGoodwillRewards/allowRoyalFavorRewards to our preference, rather than vanilla's always true.
        public static void Postfix(Faction __instance)
        {
            __instance.allowGoodwillRewards = !MadagascarVanillaMod.Persistables.DisableGoodwillRewards;
            
            if (ModsConfig.RoyaltyActive)
                __instance.allowRoyalFavorRewards = !MadagascarVanillaMod.Persistables.DisableRoyalFavorRewards;
        }
    }
}