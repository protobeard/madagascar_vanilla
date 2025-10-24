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
        private const string DisableDeadmansStockpileStorageKey = "disableDeadmansStockpileStorage";
        private const string DisableBiocodedStockpileStorageKey = "disableBiocodedStockpileStorage";
        
        [HarmonyPatch(typeof(Zone_Stockpile))]
        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(StorageSettingsPreset), typeof(ZoneManager) })]
        public static void Postfix(Zone_Stockpile __instance, StorageSettingsPreset preset)
        {
            bool disableRottenStockpileStorage = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableRottenStockpileStorageKey));
            bool disableRottenDumpingStockpileStorage = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableRottenDumpingStockpileStorageKey));

            bool disableDeadmansStockpileStorage = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableDeadmansStockpileStorageKey));
            bool disableBiocodedStockpileStorage = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableBiocodedStockpileStorageKey));
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Zone_StockpileConstructor.Postfix");
            
            SpecialThingFilterDef rottenSpecialThingFilterDef = DefDatabase<SpecialThingFilterDef>.GetNamed("AllowRotten");
            SpecialThingFilterDef deadmansSpecialThingFilterDef = DefDatabase<SpecialThingFilterDef>.GetNamed("AllowDeadmansApparel");
            SpecialThingFilterDef biocodedWeaponsSpecialThingFilterDef = DefDatabase<SpecialThingFilterDef>.GetNamed("AllowBiocodedWeapons");
            SpecialThingFilterDef biocodedApparelSpecialThingFilterDef = DefDatabase<SpecialThingFilterDef>.GetNamed("AllowBiocodedApparel");
            
            if (preset == StorageSettingsPreset.DefaultStockpile)
            {
                if (disableRottenStockpileStorage)
                    __instance.settings.filter.SetAllow(rottenSpecialThingFilterDef, false);
                
                if (disableDeadmansStockpileStorage)
                     __instance.settings.filter.SetAllow(deadmansSpecialThingFilterDef, false);

                if (disableBiocodedStockpileStorage)
                {
                    __instance.settings.filter.SetAllow(biocodedWeaponsSpecialThingFilterDef, false);
                    __instance.settings.filter.SetAllow(biocodedApparelSpecialThingFilterDef, false);
                }
            }
            else if (preset == StorageSettingsPreset.DumpingStockpile)
            {
                if (disableRottenDumpingStockpileStorage)
                {
                    __instance.settings.filter.SetAllow(rottenSpecialThingFilterDef, false);
                }
            }
        }
    }
}