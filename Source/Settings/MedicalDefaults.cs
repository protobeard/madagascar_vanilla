using RimWorld;
using UnityEngine;
using Verse;
using XmlExtensions;
using XmlExtensions.Setting;

namespace MadagascarVanilla.Settings
{
    public class MedicalDefaults : SettingContainer
    {
        // All parameters will appear as a node you can define in the XML
        public string text;

        // Calculate height of the text with Small GameFont
        protected override float CalculateHeight(float width)
        {
            int rows = 11;
            float spacePerRow = 10f;
            // Adding some spacing so it looks nicer when the settings are stacked on top of each other
            return rows * spacePerRow + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect rect)
        {
            
            GameFont currFont = Verse.Text.Font;
            Verse.Text.Font = GameFont.Small;
            Widgets.Label(rect, text);          
            Verse.Text.Font = currFont;
            
            // MedicalCareCategory defaultCareForColonist = MedicalCareCategory.Best;
            
            
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForColonist, "defaultCareForColonist", MedicalCareCategory.Best);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForTamedAnimal, "defaultCareForTamedAnimal", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForPrisoner, "defaultCareForPrisoner", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForSlave, "defaultCareForSlave", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForNeutralFaction, "defaultCareForNeutralFaction", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForWildlife, "defaultCareForWildlife", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForHostileFaction, "defaultCareForHumanlikeEnemies", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForFriendlyFaction, "defaultCareForFriendlyFaction", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForNoFaction, "defaultCareForNoFaction", MedicalCareCategory.HerbalOrWorse);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForEntities, "defaultCareForEntities", MedicalCareCategory.NoMeds);
                // Scribe_Values.Look<MedicalCareCategory>(ref this.defaultCareForGhouls, "defaultCareForGhouls", MedicalCareCategory.NoMeds);
            
            // float y1 = 0.0f;
            // using (new TextBlock(GameFont.Medium))
            //     Widgets.Label(rect, ref y1, (string)"DefaultMedicineSettings".Translate());
            // Text.Font = GameFont.Small;
            // Widgets.Label(rect, ref y1, (string)"DefaultMedicineSettingsDesc".Translate());
            // float y2 = y1 + 10f;
            // Text.Anchor = TextAnchor.MiddleLeft;
            // this.DoRow(rect, ref y2, ref Find.PlaySettings.defaultCareForColonist, "MedGroupColonists", "MedGroupColonistsDesc");
            // this.DoRow(rect, ref y2, ref Find.PlaySettings.defaultCareForPrisoner, "MedGroupPrisoners", "MedGroupPrisonersDesc");
            // if (ModsConfig.IdeologyActive)
            //     this.DoRow(rect, ref y2, ref Find.PlaySettings.defaultCareForSlave, "MedGroupSlaves", "MedGroupSlavesDesc");
            // if (ModsConfig.AnomalyActive)
            //     this.DoRow(rect, ref y2, ref Find.PlaySettings.defaultCareForGhouls, "MedGroupGhouls", "MedGroupGhoulsDesc");
            // this.DoRow(rect, ref y2, ref Find.PlaySettings.defaultCareForTamedAnimal, "MedGroupTamedAnimals", "MedGroupTamedAnimalsDesc");
            // float y3 = y2 + 17f;
            // this.DoRow(rect, ref y3, ref Find.PlaySettings.defaultCareForFriendlyFaction, "MedGroupFriendlyFaction", "MedGroupFriendlyFactionDesc");
            // this.DoRow(rect, ref y3, ref Find.PlaySettings.defaultCareForNeutralFaction, "MedGroupNeutralFaction", "MedGroupNeutralFactionDesc");
            // this.DoRow(rect, ref y3, ref Find.PlaySettings.defaultCareForHostileFaction, "MedGroupHostileFaction", "MedGroupHostileFactionDesc");
            // y3 += 17f;
            // this.DoRow(rect, ref y3, ref Find.PlaySettings.defaultCareForNoFaction, "MedGroupNoFaction", "MedGroupNoFactionDesc");
            // this.DoRow(rect, ref y3, ref Find.PlaySettings.defaultCareForWildlife, "MedGroupWildlife", "MedGroupWildlifeDesc");
            // if (ModsConfig.AnomalyActive)
            //     this.DoRow(rect, ref y3, ref Find.PlaySettings.defaultCareForEntities, "MedGroupEntities", "MedGroupEntitiesDesc");
            // Text.Anchor = TextAnchor.UpperLeft;
            
            
            
        }

        private void DoRow(
            Rect rect,
            ref float y,
            ref MedicalCareCategory category,
            string labelKey,
            string tipKey)
        {
            Rect rect1 = new Rect(rect.x, y, rect.width, 28f);
            Rect rect2 = new Rect(rect.x, y, 230f, 28f);
            Rect rect3 = new Rect(230f, y, 140f, 28f);
            if (Mouse.IsOver(rect1))
                Widgets.DrawLightHighlight(rect1);
            TooltipHandler.TipRegionByKey(rect1, tipKey);
            string label = (string)labelKey.Translate();
            Widgets.LabelFit(rect2, label);
            MedicalCareUtility.MedicalCareSetter(rect3, ref category);
            y += 34f;
        }
    }
}