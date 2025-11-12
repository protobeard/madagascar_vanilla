using System.Collections.Generic;
using System.Linq;
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

            if (MadagascarVanillaMod.Instance.CompatibilityManager.Check("EnableMechRepair", out List<string> packageIds))
                listingStandard.Label("MV_SettingWillBeIgnored".Translate(packageIds.First()));
            listingStandard.CheckboxLabeled("MV_EnableMechRepair".Translate(), ref EnableMechRepair, "MV_EnableMechRepairTooltip".Translate());
            
            //listingStandard.End();
        }
    }
}

