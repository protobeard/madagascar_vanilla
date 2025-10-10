using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using XmlExtensions;
using XmlExtensions.Setting;

namespace MadagascarVanilla.Settings
{
    public class MedicalDefaults : SettingContainer
    {
        public const string PersistMedicalSettingsKey = "persistMedicalSettings";
        
        public const string ColonistMedicalDefault = "colonistMedicalDefault";
        public const string PrisonerMedicalDefault = "prisonerMedicalDefault";
        public const string SlaveMedicalDefault = "slaveMedicalDefault";
        
        public const string TamedAnimalMedicalDefault = "tameAnimalMedicalDefault";
        public const string WildlifeMedicalDefault = "wildlifeMedicalDefault";
        
        public const string FriendlyMedicalDefault = "friendlyMedicalDefault";
        public const string NeutralMedicalDefault = "neutralMedicalDefault";
        public const string HostileMedicalDefault = "hostileMedicalDefault";
        public const string NoFactionMedicalDefault = "noFactionMedicalDefault";

        public const string GhoulMedicalDefault = "ghoulMedicalDefault";
        public const string EntityMedicalDefault = "entityMedicalDefault";
        
        // All parameters will appear as a node you can define in the XML
        public string text;
        
        // TODO: turn field names into constants -- just make the keys match?
        // Mod setting name to Playsettings field name
        public static readonly Dictionary<string, string> MedicalDefaultsDict = new Dictionary<string, string>()
        {
            { ColonistMedicalDefault, "defaultCareForColonist" },
            { PrisonerMedicalDefault, "defaultCareForPrisoner" },
            { SlaveMedicalDefault, "defaultCareForSlave" },
            { TamedAnimalMedicalDefault, "defaultCareForTamedAnimal" },
            { WildlifeMedicalDefault, "defaultCareForWildlife" },
            { FriendlyMedicalDefault, "defaultCareForFriendlyFaction" },
            { NeutralMedicalDefault, "defaultCareForNeutralFaction" },
            { HostileMedicalDefault, "defaultCareForHostileFaction" },
            { NoFactionMedicalDefault, "defaultCareForNoFaction" },
            { GhoulMedicalDefault, "defaultCareForGhouls" },
            { EntityMedicalDefault, "defaultCareForEntities" },
        };
        
        public static readonly Dictionary<string, (string, string)> MedicalDefaultsSettingToHelpDict = new Dictionary<string, (string label, string tip)>()
        {
            { ColonistMedicalDefault, ("MedGroupColonists", "MedGroupColonistsDesc") },
            { PrisonerMedicalDefault, ("MedGroupPrisoners", "MedGroupColonistsDesc") },
            { SlaveMedicalDefault, ("MedGroupSlaves", "MedGroupSlavesDesc") },
            { TamedAnimalMedicalDefault, ("MedGroupTamedAnimals", "MedGroupTamedAnimalsDesc") },
            { WildlifeMedicalDefault, ("MedGroupWildlife", "MedGroupWildlifeDesc") },
            { FriendlyMedicalDefault, ("MedGroupFriendlyFaction", "MedGroupFriendlyFactionDesc") },
            { NeutralMedicalDefault, ("MedGroupNeutralFaction", "MedGroupNeutralFactionDesc") },
            { HostileMedicalDefault, ("MedGroupHostileFaction", "MedGroupHostileFactionDesc") },
            { NoFactionMedicalDefault, ("MedGroupNoFaction", "MedGroupNoFactionDesc") },
            { GhoulMedicalDefault, ("MedGroupGhouls", "MedGroupGhoulsDesc") },
            { EntityMedicalDefault, ("MedGroupEntities", "MedGroupEntitiesDesc") },
        };

        // FIXME: Calculate height of the settings for real.
        protected override float CalculateHeight(float width)
        {
            int rows = 11;
            float spacePerRow = 10f;
            // Adding some spacing so it looks nicer when the settings are stacked on top of each other
            return rows * spacePerRow + 400f + GetDefaultSpacing();
        }

        // So the issue here is that if the game is loaded, we want to pass the PlaySettings.default* through DoRow to MedicalCareSetter
        // but if we do that then only the game settings are changed, not ours. Can we patch MedicalCareUtility.MedicalCareSetter to call PersistSettings?
        // If we patch MedicalCareSetter do we even need to patch the Medical default dialog PostClose() anymore? 1.c. above is the problematic case.
        //
        // AND, if the game is not loaded, then we have no PlaySettings.default* to pass through, so we want to pass through our
        // settings to get modified. Then, when a new game is created or an old one loaded, our existing patches should load
        // our settings into the created PlaySettings object.
        //
        // Since we're in the mod settings window, we shouldn't have to worry about manually persisting our settings to disk.
        protected override void DrawSettingContents(Rect rect)
        {
            float y1 = rect.y + 10f;
            using (new TextBlock(GameFont.Medium))
                Widgets.Label(rect, ref y1, (string)"DefaultMedicineSettings".Translate());
            Text.Font = GameFont.Small;
            Widgets.Label(rect, ref y1, (string)"DefaultMedicineSettingsDesc".Translate());
            float y2 = y1 + 10f;
            Text.Anchor = TextAnchor.MiddleLeft;
            
            foreach (var (medicalDefaultKey, medicalDefaultHelp) in MedicalDefaultsSettingToHelpDict)
            {
                // Bail if we don't have the setting for some reason.
                bool medicalCategorySettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, medicalDefaultKey, out string medicalCareCategoryName);
                if (!medicalCategorySettingExists)
                    return;
            
                // Bail if the setting can't be parsed.
                bool parsed = Enum.TryParse(medicalCareCategoryName, false, out MedicalCareCategory medicalCareCategory);
                if (!parsed)
                    return;

                // Bail if we don't have the right DLCs for the setting
                if (!ModsConfig.IdeologyActive && medicalDefaultKey == SlaveMedicalDefault ||
                    !ModsConfig.AnomalyActive && medicalDefaultKey == GhoulMedicalDefault ||
                    !ModsConfig.AnomalyActive && medicalDefaultKey == EntityMedicalDefault)
                    return;
                
                // Add a row for the setting
                this.DoRow(rect, ref y2, ref medicalCareCategory, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DoRow(Rect rect, ref float y, ref MedicalCareCategory category, string labelKey, string tipKey)
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