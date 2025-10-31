using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(MethodType.Constructor)]
    public static class QuestRewardsPatch
    {
        private const string GoodwillRewardsKey = "goodwillRewards";
        private const string RoyalFavorRewardsKey = "royalFavorRewards";
        
        // When factions are created, set allowGoodwillRewards/allowRoyalFavorRewards to our preference, rather than vanilla's always true.
        public static void Postfix(Faction __instance)
        {
            bool goodwillRewards = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, GoodwillRewardsKey));
            bool royalFavorRewards = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, RoyalFavorRewardsKey));
            
            __instance.allowGoodwillRewards = goodwillRewards;
            
            if (ModsConfig.RoyaltyActive)
                __instance.allowRoyalFavorRewards = royalFavorRewards;
        }
    }
}