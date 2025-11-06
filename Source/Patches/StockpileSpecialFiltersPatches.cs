using System;
using RimWorld;
using Verse;
using HarmonyLib;
using SpecialThingFilterDefOf = MadagascarVanilla.DefOfs.SpecialThingFilterDefOf;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class StockpileSpecialFiltersPatches
    {
        [HarmonyPatch(typeof(Zone_Stockpile))]
        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(StorageSettingsPreset), typeof(ZoneManager) })]
        public static void Postfix(Zone_Stockpile __instance, StorageSettingsPreset preset)
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Zone_StockpileConstructor.Postfix");
            
            if (preset == StorageSettingsPreset.DefaultStockpile)
            {
                if (MadagascarVanillaMod.Persistables.DisableRottenStockpileStorage)
                    __instance.settings.filter.SetAllow(SpecialThingFilterDefOf.AllowRotten, false);
                
                if (MadagascarVanillaMod.Persistables.DisableDeadmansStockpileStorage)
                     __instance.settings.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);

                if (MadagascarVanillaMod.Persistables.DisableBiocodedStockpileStorage && ModsConfig.RoyaltyActive)
                {
                    __instance.settings.filter.SetAllow(SpecialThingFilterDefOf.AllowBiocodedWeapons, false);
                    __instance.settings.filter.SetAllow(SpecialThingFilterDefOf.AllowBiocodedApparel, false);
                }
            }
            else if (preset == StorageSettingsPreset.DumpingStockpile)
            {
                if (MadagascarVanillaMod.Persistables.DisableRottenDumpingStockpileStorage)
                {
                    __instance.settings.filter.SetAllow(SpecialThingFilterDefOf.AllowRotten, false);
                }
            }
        }
    }
}