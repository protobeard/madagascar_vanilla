using System;
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
        public const string StoreModeSetting = "storeMode";
        public const string IngredientSearchRadiusSetting = "ingredientSearchRadius";
        
        private const string DropOnFloor = "DropOnFloor";
        private const string BestStockpile = "BestStockpile";
        
        public static void Postfix(Bill_Production __instance)
        {
            string storeMode = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, StoreModeSetting));
            
            if (storeMode == DropOnFloor)
                __instance.SetStoreMode(BillStoreModeDefOf.DropOnFloor);
            else if (storeMode == BestStockpile)
                __instance.SetStoreMode(BillStoreModeDefOf.BestStockpile);
            
            float ingredientSearchRadius = float.Parse((SettingsManager.GetSetting(MadagascarVanillaMod.ModId, IngredientSearchRadiusSetting)));
            __instance.ingredientSearchRadius = ingredientSearchRadius;
        }
    }
}