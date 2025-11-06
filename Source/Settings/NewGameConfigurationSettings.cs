using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private void DoNewGameConfigurationSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_NewGameConfigurationSettingsTitle".Translate());

            // New Game Setup
            listingStandard.CheckboxLabeled("MV_EnablePersistingNewGameSetup".Translate(), ref EnablePersistingNewGameSetup, "MV_EnablePersistingNewGameSetupTooltip".Translate());

            // New Game Setup (Misc)
            listingStandard.CheckboxLabeled("MV_EnableWorkPriorities".Translate(), ref EnableWorkPriorities, "MV_EnableWorkPrioritiesTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableGoodwillRewards".Translate(), ref DisableGoodwillRewards, "MV_DisableGoodwillRewards".Translate());
            listingStandard.CheckboxLabeled("MV_DisableRoyalFavorRewards".Translate(), ref DisableRoyalFavorRewards, "MV_DisableRoyalFavorRewards".Translate());
            
            listingStandard.Label("MV_DefaultHostilityResponse".Translate(), tooltip:"MV_DefaultHostilityResponseTooltip".Translate());
            foreach (HostilityResponseMode responseMode in Enum.GetValues(typeof(HostilityResponseMode)))
            {   
                bool active = DefaultHostilityResponse == responseMode;
                if (listingStandard.RadioButton(responseMode.ToString(), active))
                    DefaultHostilityResponse = responseMode;
            }
            
            //listingStandard.End();
        }
    }
}