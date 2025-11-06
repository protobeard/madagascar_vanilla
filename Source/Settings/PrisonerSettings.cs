using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoPrisonerSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_PrisonerSettingsTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_EnableAutoStrip".Translate(), ref EnableAutoStrip, "MV_EnableAutoStripTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableAutoStripArrestedColonist".Translate(), ref EnableAutoStripArrestedColonist, "MV_EnableAutoStripArrestedColonistTooltip".Translate());
            
            //listingStandard.End();
        }
    }
}