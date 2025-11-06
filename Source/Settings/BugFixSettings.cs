using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoBugFixSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            //Listing_Standard listingStandard = new Listing_Standard();
            //listingStandard.Begin(rect);
            
            listingStandard.Label("MV_BugFixesTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_DisableRemovingApparelWhileBleeding".Translate(), ref DisableRemovingApparelWhileBleeding, "MV_DisableRemovingApparelWhileBleedingTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableCompMilkableDisplayProperItem".Translate(), ref EnableCompMilkableDisplayProperItem, "MV_EnableCompMilkableDisplayProperItemTooltip".Translate());
            
            //listingStandard.End();
        }
    }
}