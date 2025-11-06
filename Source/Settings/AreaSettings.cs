using System;
using System.Linq;
using MadagascarVanilla.ClassExtensions;
using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoAreaSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            //Listing_Standard listingStandard = rect.BeginListingStandard();
                
            listingStandard.Label("MV_AreaSettingsTitle".Translate());
            
            listingStandard.CheckboxLabeled("MV_DisableAutoHomeArea".Translate(), ref DisableAutoHomeArea, "MV_DisableAutoHomeAreaTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableAutoRebuildInHomeArea".Translate(), ref EnableAutoRebuildInHomeArea, "MV_EnableAutoRebuildInHomeAreaTooltip".Translate());

            // FIXME: text area
            // "MV_StartingAreasListTooltip".Translate()
            
            string areaList = listingStandard.TextEntryLabeled("MV_StartingAreasList".Translate(), string.Join(", ", StartingAreasList));
            StartingAreasList = areaList.Split(',').Select(s => s.Trim()).ToList();
            //listingStandard.End();
        }
    }
}