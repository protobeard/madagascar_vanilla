using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using UnityEngine.Windows;
using Verse;
using Verse.Sound;

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
                DoRow(rect, ref y2, medicalCareCategory, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        // Draw a medical default settings row
        private static void DoRow(Rect rect, ref float y, MedicalCareCategory category, string medicalDefaultKey, string labelKey, string tipKey)
        {
            Rect rect1 = new Rect(rect.x, y, rect.width, 28f);
            Rect rect2 = new Rect(rect.x, y, 230f, 28f);
            Rect rect3 = new Rect(230f, y, 140f, 28f);
            if (Mouse.IsOver(rect1))
                Widgets.DrawLightHighlight(rect1);
            TooltipHandler.TipRegionByKey(rect1, tipKey);
            string label = (string)labelKey.Translate();
            Widgets.LabelFit(rect2, label);
            MedicalCareSetter(rect3, category, medicalDefaultKey);
            y += 34f;
        }
        
        // FIXME: painting doesn't work. Either remove the code or fix it.
        // TODO: what is the uniqueId number below? Where does it come from? Looks like a constant from somewhere...
        // Create our own copy of MedicalCareUtility.MedicalCareSetter so that we can
        // ensure that our Mod Settings window write to all the places we need.
        private static void MedicalCareSetter(Rect rect, MedicalCareCategory currentlySelectedMedicalCareCategory, string medicalDefaultKey)
        {
            bool medicalCarePainting = false;
            Texture2D[] careTextures = new Texture2D[5];
            careTextures[0] = ContentFinder<Texture2D>.Get("UI/Icons/Medical/NoCare");
            careTextures[1] = ContentFinder<Texture2D>.Get("UI/Icons/Medical/NoMeds");
            careTextures[2] = ThingDefOf.MedicineHerbal.uiIcon;
            careTextures[3] = ThingDefOf.MedicineIndustrial.uiIcon;
            careTextures[4] = ThingDefOf.MedicineUltratech.uiIcon;
            
            Rect rect1 = new Rect(rect.x, rect.y, rect.width / 5f, rect.height);
            for (int index = 0; index < 5; ++index)
            {
                MedicalCareCategory newMedicalCareCategory = (MedicalCareCategory) index;
                Widgets.DrawHighlightIfMouseover(rect1);
                MouseoverSounds.DoRegion(rect1);
                GUI.DrawTexture(rect1, (Texture) careTextures[index]);
                Widgets.DraggableResult result = Widgets.ButtonInvisibleDraggable(rect1);
                if (result == Widgets.DraggableResult.Dragged)
                    medicalCarePainting = true;
                
                if (medicalCarePainting && Mouse.IsOver(rect1) && currentlySelectedMedicalCareCategory != newMedicalCareCategory || (result == Widgets.DraggableResult.Pressed || result == Widgets.DraggableResult.DraggedThenPressed))
                {
                    currentlySelectedMedicalCareCategory = newMedicalCareCategory;
                    SettingsManager.SetSetting(MadagascarVanillaMod.ModId, medicalDefaultKey, newMedicalCareCategory.ToString());
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }
                if (currentlySelectedMedicalCareCategory == newMedicalCareCategory)
                    Widgets.DrawBox(rect1, 2);
                if (Mouse.IsOver(rect1))
                    TooltipHandler.TipRegion(rect1, (Func<string>) (() => newMedicalCareCategory.GetLabel().CapitalizeFirst()), 632165 + index * 17);
                rect1.x += rect1.width;
            }
            if (UnityEngine.Input.GetMouseButton(0))
                return;
            medicalCarePainting = false;
        }
    }
}