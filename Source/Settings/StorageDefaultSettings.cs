using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoStorageDefaultSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_StorageDefaultSettingsTitle".Translate());
            
            // Storage (Shelf-like)
            listingStandard.CheckboxLabeled("MV_EnableClearShelfStorage".Translate(), ref EnableClearShelfStorage, "MV_EnableClearShelfStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableClearBookcaseStorage".Translate(), ref EnableClearBookcaseStorage, "MV_EnableClearBookcaseStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableClearOutfitStandStorage".Translate(), ref EnableClearOutfitStandStorage, "MV_EnableClearOutfitStandStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableClearHopperStorage".Translate(), ref EnableClearHopperStorage, "MV_EnableClearHopperStorageTooltip".Translate());

            // Storage Special (Shelf-like)
            listingStandard.CheckboxLabeled("MV_DisableRottenShelfStorage".Translate(), ref DisableRottenShelfStorage, "MV_DisableRottenShelfStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableDeadmansShelfStorage".Translate(), ref DisableDeadmansShelfStorage, "MV_DisableDeadmansShelfStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableBiocodedShelfStorage".Translate(), ref DisableBiocodedShelfStorage, "MV_DisableBiocodedShelfStorageTooltip".Translate());

            // Storage Special (Stockpile)
            listingStandard.CheckboxLabeled("MV_DisableRottenStockpileStorage".Translate(), ref DisableRottenStockpileStorage, "MV_DisableRottenStockpileStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableDeadmansStockpileStorage".Translate(), ref DisableDeadmansStockpileStorage, "MV_DisableDeadmansStockpileStorageTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableBiocodedStockpileStorage".Translate(), ref DisableBiocodedStockpileStorage, "MV_DisableBiocodedStockpileStorageTooltip".Translate());

            // Storage Special (Dumping Stockpile)
            listingStandard.CheckboxLabeled("MV_DisableRottenDumpingStockpileStorage".Translate(), ref DisableRottenDumpingStockpileStorage, "MV_DisableRottenDumpingStockpileStorageTooltip".Translate());
            
            // listingStandard.End();
        }
    }
}