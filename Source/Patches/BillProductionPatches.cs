using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using HarmonyLib;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    // Set the default bill store mode and ingredient radius
    [HarmonyPatch(typeof(Bill_Production))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] {typeof(RecipeDef), typeof(Precept_ThingStyle)})]
    public static class BillProductionPatches
    {
        private const string StoreModeKey = "storeMode";
        private const string RepeatModeKey = "repeatMode";
        private const string HitpointRangeToCountKey = "hitpointRangeToCount";
        private const string MinQualityToCountKey = "minQualityToCount";
        private const string MaxQualityToCountKey = "maxQualityToCount";
        private const string IngredientSearchRadiusKey = "ingredientSearchRadius";
        
        private const char RangeSplitter = '~';
        
        public static void Postfix(Bill_Production __instance)
        {
            BillStoreModeDef storeMode = DefDatabase<BillStoreModeDef>.GetNamed((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, StoreModeKey)));
            if (storeMode != null)
                __instance.SetStoreMode(storeMode);
            
            BillRepeatModeDef repeatMode = DefDatabase<BillRepeatModeDef>.GetNamed((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, RepeatModeKey)));
            if (repeatMode != null)
                __instance.repeatMode = repeatMode;

            // TODO: there's gotta be a better way in XML Extension to do this...
            string hitpointRangeToCount = SettingsManager.GetSetting(MadagascarVanillaMod.ModId, HitpointRangeToCountKey);
            List<string> rangeBoundaries = hitpointRangeToCount.Split(RangeSplitter).ToList();

            if (rangeBoundaries.Count == 2)
            {
                __instance.hpRange.min = float.Parse(rangeBoundaries[0]);
                __instance.hpRange.max = float.Parse(rangeBoundaries[1]);
            }

            string minQualityName = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MinQualityToCountKey));
            string maxQualityName = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MaxQualityToCountKey));
            bool parsedMinQuality = Enum.TryParse(minQualityName, false, out QualityCategory minQualityCategory);
            bool parsedMaxQuality = Enum.TryParse(maxQualityName, false, out QualityCategory maxQualityCategory);

            if (parsedMinQuality && parsedMaxQuality)
            {
                // TODO: is there a way to ensure this in the XML Extension settings instead?
                if (minQualityCategory > maxQualityCategory)
                {
                    maxQualityCategory = minQualityCategory;
                }
                
                __instance.qualityRange.min = minQualityCategory;
                __instance.qualityRange.max = maxQualityCategory;
            }
            
            float ingredientSearchRadius = float.Parse((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, IngredientSearchRadiusKey)));
            __instance.ingredientSearchRadius = ingredientSearchRadius;
        }
    }
}