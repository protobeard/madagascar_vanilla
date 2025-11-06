using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private const int HitpointRangeToCountMin = 1;
        private const int HitpointRangeToCountMax = 100;
        
        private void DoProductionBillSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_ProductionBillSettingsTitle".Translate());
            
            // Production Bills (General)
            listingStandard.CheckboxLabeled("MV_EnableProductionSpecialistOnlyBillAssignment".Translate(), ref EnableProductionSpecialistOnlyBillAssignment, "MV_EnableProductionSpecialistOnlyBillAssignmentTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnableInspiredOnlyBillAssignment".Translate(), ref EnableInspiredOnlyBillAssignment, "MV_EnableInspiredOnlyBillAssignmentTooltip".Translate());

            listingStandard.Label("MV_BillStoreMode".Translate(), tooltip:"MV_BillStoreModeTooltip".Translate());
            foreach (BillStoreModeDef storeMode in DefDatabase<BillStoreModeDef>.AllDefs)
            {   
                bool active = BillStoreMode == storeMode;
                if (listingStandard.RadioButton(storeMode.ToString(), active))
                    BillStoreMode = storeMode;
            }
            
            listingStandard.Label("MV_BillRepeatMode".Translate(), tooltip:"MV_BillRepeatModeTooltip".Translate());
            foreach (BillRepeatModeDef repeatMode in DefDatabase<BillRepeatModeDef>.AllDefs)
            {   
                bool active = BillRepeatMode == repeatMode;
                if (listingStandard.RadioButton(repeatMode.ToString(), active))
                    BillRepeatMode = repeatMode;
            }
            
            listingStandard.Label("MV_HitpointRangeToCount".Translate(), tooltip:"MV_HitpointRangeToCountTooltip".Translate());
            listingStandard.IntRange(ref HitpointRangeToCount, HitpointRangeToCountMin, HitpointRangeToCountMax);
            
            listingStandard.Label("MV_QualityToCountMin".Translate(), tooltip:"MV_QualityToCountMinTooltip".Translate());
            foreach (QualityCategory minQuality in Enum.GetValues(typeof(QualityCategory)))
            {   
                bool active = MinQualityToCount == minQuality;
                if (listingStandard.RadioButton(minQuality.ToString(), active))
                    MinQualityToCount = minQuality;
            }
            
            listingStandard.Label("MV_QualityToCountMax".Translate(), tooltip:"MV_QualityToCountMaxTooltip".Translate());
            foreach (QualityCategory maxQuality in Enum.GetValues(typeof(QualityCategory)))
            {   
                bool active = MaxQualityToCount == maxQuality;
                if (listingStandard.RadioButton(maxQuality.ToString(), active))
                    MaxQualityToCount = maxQuality;
            }
            
            string editBuffer = IngredientSearchRadius.ToString();
            listingStandard.Label("MV_IngredientSearchRadius".Translate(), tooltip:"MV_IngredientSearchRadiusTooltip".Translate());
            listingStandard.IntEntry(ref IngredientSearchRadius, ref editBuffer);
            
            // Production Bills (Tailor)
            listingStandard.CheckboxLabeled("MV_DisableClothTextile".Translate(), ref DisableClothTextile, "MV_DisableClothTextileTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableValuableTextiles".Translate(), ref DisableValuableTextiles, "MV_DisableValuableTextilesTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_DisableMoodImpactingTextiles".Translate(), ref DisableMoodImpactingTextiles, "MV_DisableMoodImpactingTextilesTooltip".Translate());

            // Production Bills (Crematorium)
            listingStandard.CheckboxLabeled("MV_DisableColonistCremation".Translate(), ref DisableColonistCremation, "MV_DisableColonistCremationTooltip".Translate());
            
            // listingStandard.End();
        }
    }
}