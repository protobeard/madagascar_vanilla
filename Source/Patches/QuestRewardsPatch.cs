using System;
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
        private const string GoodwillRewardsPreference = "goodwillRewards";
        private const string RoyalFavorRewardsPreference = "royalFavorRewards";
        
        // When factions are created, set allowGoodwillRewards/allowRoyalFavorRewards to our preference, rather than vanilla's always true.
        public static void Postfix(Faction __instance)
        {
            bool goodwillRewards = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.modId, GoodwillRewardsPreference));
            bool royalFavorRewards = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.modId, RoyalFavorRewardsPreference));
            
            __instance.allowGoodwillRewards = goodwillRewards;
            __instance.allowRoyalFavorRewards = royalFavorRewards;
        }
    }
}