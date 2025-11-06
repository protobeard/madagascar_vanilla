using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private const int AllowXGravshipSubquestsMin = 3;

        private const int OdysseyQuestExtensionMultiplierMin = 1;

        private const int OdysseyQuestRangeExtenderMin = 1;
        private const int OdysseyQuestRangeExtenderMax = 100;
        
        private void DoOdysseyQuestSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_OdysseyQuestSettingsTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_EnableShowExpandingLandmarks".Translate(), ref EnableShowExpandingLandmarks, "MV_EnableShowExpandingLandmarksTooltip".Translate());
            
            listingStandard.Label("MV_AllowXGravshipSubquests".Translate(), tooltip: "MV_AllowXGravshipSubquestsTooltip".Translate());
            listingStandard.Label(AllowXGravshipSubquests.ToString());
            listingStandard.IntAdjuster(ref AllowXGravshipSubquests, 1, AllowXGravshipSubquestsMin);
            
            listingStandard.Label("MV_OdysseyQuestExtensionMultiplier".Translate(), tooltip:"MV_OdysseyQuestExtensionMultiplierTooltip".Translate());
            listingStandard.Label(OdysseyQuestExtensionMultiplier.ToString());
            listingStandard.IntAdjuster(ref OdysseyQuestExtensionMultiplier, 1, OdysseyQuestExtensionMultiplierMin);
            
            listingStandard.Label("MV_OdysseyQuestRangeExtender".Translate());
            listingStandard.IntRange(ref OdysseyQuestRangeExtender, OdysseyQuestRangeExtenderMin, OdysseyQuestRangeExtenderMax);
            
            //listingStandard.End();
        }
    }
}