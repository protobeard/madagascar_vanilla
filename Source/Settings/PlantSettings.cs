using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private const int GauralenPruningSpeedMultiplierMin = 1;
        
        private void DoPlantSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_PlantSettingsTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_EnableHydroponicDevilstrand".Translate(), ref EnableHydroponicDevilstrand, "MV_EnableHydroponicDevilstrandTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableFalloutImmuneDevilstrand".Translate(), ref EnableFalloutImmuneDevilstrand, "MV_EnableFalloutImmuneDevilstrandTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableFalloutImmuneToxipotatos".Translate(), ref EnableFalloutImmuneToxipotatos, "MV_EnableFalloutImmuneToxipotatosTooltip".Translate());
            
            listingStandard.Label("MV_GauralenPruningSpeedMultiplier".Translate(), tooltip:"MV_GauralenPruningSpeedMultiplierTooltip".Translate());
            listingStandard.Label(GauralenPruningSpeedMultiplier.ToString());
            listingStandard.IntAdjuster(ref GauralenPruningSpeedMultiplier, 1, GauralenPruningSpeedMultiplierMin);

            listingStandard.CheckboxLabeled("MV_EnableAutoCut".Translate(), ref EnableAutoCut, "MV_EnableAutoCutTooltip".Translate());
            
            //listingStandard.End();
        }
    }
}