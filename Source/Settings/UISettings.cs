using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoUISettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_UISettingsTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_DisableLearningHelperButton".Translate(), ref DisableLearningHelperButton, "MV_DisableLearningHelperButtonTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableTraitsInOutFitAssignmentRow".Translate(), ref EnableTraitsInOutFitAssignmentRow, "MV_EnableTraitsInOutFitAssignmentRowTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableSleepingAloneAlert".Translate(), ref EnableSleepingAloneAlert, "MV_EnableSleepingAloneAlertTooltip".Translate());
            
            // listingStandard.End();
        }
    }
}