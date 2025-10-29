using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using HarmonyLib;
using MadagascarVanilla.ModExtensions;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    // Set bill defaults:
    // - repeatMode
    //  - If TargetCount, hitpointRange
    //  - If TargetCount, qualityRange
    // - storeMode
    // - ingredientSearchRadius
    //
    // For tailoring bills, disable ingredients:
    // - cloth
    // - devilstrand, hyperweave, synthread, thrumbofur, thrumbomane
    // - human leather, dread leather
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
        
        private const string DisableClothTextileKey = "disableClothTextile";
        private const string DisableValuableTextilesKey = "disableValuableTextiles";
        private const string DisableMoodImpactingTextilesKey = "disableMoodImpactingTextiles";
        
        private const char RangeSplitter = '~';
        
        public static void Postfix(Bill_Production __instance)
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting Store Mode for {__instance.Label}");
            BillStoreModeDef storeMode = DefDatabase<BillStoreModeDef>.GetNamed((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, StoreModeKey)));
            if (storeMode != null)
                __instance.SetStoreMode(storeMode);
            
            BillRepeatModeDef repeatMode = DefDatabase<BillRepeatModeDef>.GetNamed((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, RepeatModeKey)));
            if (repeatMode != null)
            {
                // Only assign settings for TargetCount RepeatMode if we're in a recipe which can count its products.
                // Recipes like "shred mechanoid," for example, don't work with TargetCount.
                if (repeatMode != BillRepeatModeDefOf.TargetCount)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting {repeatMode} Repeat Mode for {__instance.Label}");
                    __instance.repeatMode = repeatMode;
                } 
                else if (__instance.recipe.WorkerCounter.CanCountProducts(__instance))
                {
                    ConfigureTargetCountMode(__instance);
                }
            }
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting search radius for {__instance.Label}");
            float ingredientSearchRadius = float.Parse((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, IngredientSearchRadiusKey)));
            __instance.ingredientSearchRadius = ingredientSearchRadius;

            // If we're a tailoring bill we need to check the ingredient disabling settings
            if (__instance.recipe != null && __instance.recipe.recipeUsers != null && __instance.recipe.recipeUsers.Contains(DefOfs.ThingDefOf.ElectricTailoringBench))
            {
                DisableTailoringIngredients(__instance);
            }
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Leaving method for {__instance.Label}");
        }

        private static void ConfigureTargetCountMode(Bill_Production bill)
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Set TargetCount Repeat Mode {bill.Label}");
                
            bill.repeatMode = BillRepeatModeDefOf.TargetCount;
            
            // TODO: there's gotta be a better way in XML Extension to do this...
            //if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting hitpoint range for {__instance.Label}");
            string hitpointRangeToCount = SettingsManager.GetSetting(MadagascarVanillaMod.ModId, HitpointRangeToCountKey);
            List<string> rangeBoundaries = hitpointRangeToCount.Split(RangeSplitter).ToList();

            if (rangeBoundaries.Count == 2)
            {
                bill.hpRange.min = float.Parse(rangeBoundaries[0]);
                bill.hpRange.max = float.Parse(rangeBoundaries[1]);
            }

            //if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting quality range for {__instance.Label}");
            string minQualityName = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MinQualityToCountKey));
            string maxQualityName = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MaxQualityToCountKey));
            bool parsedMinQuality = Enum.TryParse(minQualityName, false, out QualityCategory minQualityCategory);
            bool parsedMaxQuality = Enum.TryParse(maxQualityName, false, out QualityCategory maxQualityCategory);

            if (parsedMinQuality && parsedMaxQuality)
            {
                // TODO: is there a way to ensure this in the XML Extension settings instead?
                if (minQualityCategory > maxQualityCategory)
                    maxQualityCategory = minQualityCategory;
                
                bill.qualityRange.min = minQualityCategory;
                bill.qualityRange.max = maxQualityCategory;
            }
        }

        private static void DisableTailoringIngredients(Bill_Production bill)
        {
            bool disableClothTextile = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableClothTextileKey));
            bool disableValuableTextiles = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableValuableTextilesKey));
            bool disableMoodImpactingTextiles = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableMoodImpactingTextilesKey));
            
            if (disableClothTextile)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Disabling textiles for {bill.Label}");
                
                bill.ingredientFilter.SetAllow(ThingDefOf.Cloth, false);
            }

            if (disableValuableTextiles)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Disabling textiles for {bill.Label}");

                foreach (ThingDef textile in DefDatabase<ThingDef>.AllDefs.Where(td => td.HasModExtension<ValuableTextileExtension>() && td.GetModExtension<ValuableTextileExtension>().ValuableTextile))
                {
                    bill.ingredientFilter.SetAllow(textile, false);
                }
            }

            if (disableMoodImpactingTextiles)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Disabling textiles for {bill.Label}");
                
                foreach (ThingDef textile in DefDatabase<ThingDef>.AllDefs.Where(td => td.HasModExtension<MoodAlteringTextileExtension>() && td.GetModExtension<MoodAlteringTextileExtension>().MoodAlteringTextile))
                {
                    bill.ingredientFilter.SetAllow(textile, false);
                }
            }
        }
    }
}