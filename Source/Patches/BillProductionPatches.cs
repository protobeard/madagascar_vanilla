using System;
using System.Linq;
using RimWorld;
using HarmonyLib;
using MadagascarVanilla.ModExtensions;
using Verse;

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
        
        public static void Postfix(Bill_Production __instance)
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting Store Mode for {__instance.Label}");
            if (MadagascarVanillaMod.Persistables.BillStoreMode != null)
                __instance.SetStoreMode(MadagascarVanillaMod.Persistables.BillStoreMode);
            
            if (MadagascarVanillaMod.Persistables.BillRepeatMode != null)
            {
                // Only assign settings for TargetCount RepeatMode if we're in a recipe which can count its products.
                // Recipes like "shred mechanoid," for example, don't work with TargetCount.
                if (MadagascarVanillaMod.Persistables.BillRepeatMode != BillRepeatModeDefOf.TargetCount)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting {MadagascarVanillaMod.Persistables.BillRepeatMode} Repeat Mode for {__instance.Label}");
                    __instance.repeatMode = MadagascarVanillaMod.Persistables.BillRepeatMode;
                } 
                else if (__instance.recipe.WorkerCounter.CanCountProducts(__instance))
                {
                    ConfigureTargetCountMode(__instance);
                }
            }
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Setting search radius for {__instance.Label}");
            __instance.ingredientSearchRadius = MadagascarVanillaMod.Persistables.IngredientSearchRadius;

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
            
            bill.hpRange.min = MadagascarVanillaMod.Persistables.HitpointRangeToCount.min;
            bill.hpRange.max = MadagascarVanillaMod.Persistables.HitpointRangeToCount.max;

            // FIXME: is there a way to ensure this in the setter instead
            if (MadagascarVanillaMod.Persistables.MinQualityToCount > MadagascarVanillaMod.Persistables.MaxQualityToCount)
                MadagascarVanillaMod.Persistables.MaxQualityToCount = MadagascarVanillaMod.Persistables.MinQualityToCount;
            
            bill.qualityRange.min = MadagascarVanillaMod.Persistables.MinQualityToCount;
            bill.qualityRange.max = MadagascarVanillaMod.Persistables.MaxQualityToCount;
        }

        private static void DisableTailoringIngredients(Bill_Production bill)
        {
            if (MadagascarVanillaMod.Persistables.DisableClothTextile)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Disabling textiles for {bill.Label}");
                
                bill.ingredientFilter.SetAllow(ThingDefOf.Cloth, false);
            }

            if (MadagascarVanillaMod.Persistables.DisableValuableTextiles)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"BillProductionPatches.Postfix: Disabling textiles for {bill.Label}");

                foreach (ThingDef textile in DefDatabase<ThingDef>.AllDefs.Where(td => td.HasModExtension<ValuableTextileExtension>() && td.GetModExtension<ValuableTextileExtension>().ValuableTextile))
                {
                    bill.ingredientFilter.SetAllow(textile, false);
                }
            }

            if (MadagascarVanillaMod.Persistables.DisableMoodImpactingTextiles)
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