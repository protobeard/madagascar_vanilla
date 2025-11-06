using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoMechanitorSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_MechanitorSettingsTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_EnableMechRepair".Translate(), ref EnableMechRepair, "MV_EnableMechRepairTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableNonflammableMechResourceChips".Translate(), ref EnableNonflammableMechResourceChips, "MV_EnableNonflammableMechResourceChipsTooltip".Translate());
            
            //listingStandard.End();
        }
    }
}