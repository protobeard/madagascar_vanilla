using System.Collections.Generic;
using System.Linq;
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
            
            if (MadagascarVanillaMod.Instance.CompatibilityManager.Check("EnableCompMilkableDisplayProperItem", out List<string> packageIds))
                listingStandard.Label("MV_SettingWillBeIgnored".Translate(packageIds.First()));
            listingStandard.CheckboxLabeled("MV_EnableCompMilkableDisplayProperItem".Translate(), ref EnableCompMilkableDisplayProperItem, "MV_EnableCompMilkableDisplayProperItemTooltip".Translate());
            
            //listingStandard.End();
        }
    }
}