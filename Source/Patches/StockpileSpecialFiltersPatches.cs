using System;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class StockpileSpecialFiltersPatches
    {
        private const string DisableRottenStockpileStorageKey = "disableRottenStockpileStorage";
        private const string DisableRottenDumpingStockpileStorageKey = "disableRottenDumpingStockpileStorage";
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Zone_Stockpile))]
        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(StorageSettingsPreset), typeof(ZoneManager) })]
        public static void Postfix(Zone_Stockpile __instance, StorageSettingsPreset preset)
        {
            bool disableRottenStockpileStorage = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableRottenStockpileStorageKey));
            bool disableRottenDumpingStockpileStorage = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableRottenDumpingStockpileStorageKey));

            if (MadagascarVanillaMod.Verbose()) Log.Message($"Zone_StockpileConstructor.Postfix");
            
            SpecialThingFilterDef specialThingFilterDef = DefDatabase<SpecialThingFilterDef>.GetNamed("AllowRotten");

            if ((preset == StorageSettingsPreset.DefaultStockpile && !disableRottenStockpileStorage) ||
                (preset == StorageSettingsPreset.DumpingStockpile && !disableRottenDumpingStockpileStorage))
                return;
            
            __instance.settings.filter.SetAllow(specialThingFilterDef, false);
        }
    }
}