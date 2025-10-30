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
        public const string GhoulMedicalDefault = "ghoulMedicalDefault";
        public const string TamedAnimalMedicalDefault = "tameAnimalMedicalDefault";
        public const string FriendlyMedicalDefault = "friendlyMedicalDefault";
        public const string NeutralMedicalDefault = "neutralMedicalDefault";
        public const string HostileMedicalDefault = "hostileMedicalDefault";
        public const string NoFactionMedicalDefault = "noFactionMedicalDefault";
        public const string WildlifeMedicalDefault = "wildlifeMedicalDefault";
        public const string EntityMedicalDefault = "entityMedicalDefault";
        
        // Mod setting name to Playsettings field name
        public static readonly Dictionary<string, string> MedicalDefaultsDict = new Dictionary<string, string>()
        {
            { ColonistMedicalDefault, "defaultCareForColonist" },
            { PrisonerMedicalDefault, "defaultCareForPrisoner" },
            { SlaveMedicalDefault, "defaultCareForSlave" },
            { GhoulMedicalDefault, "defaultCareForGhouls" },
            { TamedAnimalMedicalDefault, "defaultCareForTamedAnimal" },
            { FriendlyMedicalDefault, "defaultCareForFriendlyFaction" },
            { NeutralMedicalDefault, "defaultCareForNeutralFaction" },
            { HostileMedicalDefault, "defaultCareForHostileFaction" },
            { NoFactionMedicalDefault, "defaultCareForNoFaction" },
            { WildlifeMedicalDefault, "defaultCareForWildlife" },
            { EntityMedicalDefault, "defaultCareForEntities" },
        };
        
        public static readonly Dictionary<string, (string, string)> MedicalDefaultsSettingToHelpDict = new Dictionary<string, (string label, string tip)>()
        {
            { ColonistMedicalDefault, ("MedGroupColonists", "MedGroupColonistsDesc") },
            { PrisonerMedicalDefault, ("MedGroupPrisoners", "MedGroupColonistsDesc") },
            { SlaveMedicalDefault, ("MedGroupSlaves", "MedGroupSlavesDesc") },
            { GhoulMedicalDefault, ("MedGroupGhouls", "MedGroupGhoulsDesc") },
            { TamedAnimalMedicalDefault, ("MedGroupTamedAnimals", "MedGroupTamedAnimalsDesc") },
            { FriendlyMedicalDefault, ("MedGroupFriendlyFaction", "MedGroupFriendlyFactionDesc") },
            { NeutralMedicalDefault, ("MedGroupNeutralFaction", "MedGroupNeutralFactionDesc") },
            { HostileMedicalDefault, ("MedGroupHostileFaction", "MedGroupHostileFactionDesc") },
            { NoFactionMedicalDefault, ("MedGroupNoFaction", "MedGroupNoFactionDesc") },
            
            { WildlifeMedicalDefault, ("MedGroupWildlife", "MedGroupWildlifeDesc") },
            { EntityMedicalDefault, ("MedGroupEntities", "MedGroupEntitiesDesc") },
        };

        private const float VerticalElementSpacing = 10f;
        private const float RowSpacing = 6f;
        private const float RowHeight = MedicalCareUtility.CareSetterHeight + RowSpacing;
        private const float BottomBufferSpacing = 10f;

        protected override float CalculateHeight(float width)
        {
            int rows = 8;

            if (ModsConfig.IdeologyActive)
                rows++;
            if (ModsConfig.AnomalyActive)
                rows += 2;
            
            GameFont currFont = Verse.Text.Font;
            Verse.Text.Font = GameFont.Small;
            float defaultMedicineDescriptionLabelHeight = (float)Math.Ceiling(Verse.Text.CalcHeight((string)"DefaultMedicineSettingsDesc".Translate(), width));
            Verse.Text.Font = currFont;
            
            return rows * RowHeight + defaultMedicineDescriptionLabelHeight + BottomBufferSpacing + GetDefaultSpacing();
        }
        
        protected override void DrawSettingContents(Rect rect)
        {
            float y = rect.y + VerticalElementSpacing;
            Text.Font = GameFont.Small;
            Widgets.Label(rect, ref y, (string)"DefaultMedicineSettingsDesc".Translate());
            float y2 = y + VerticalElementSpacing;
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
                    continue;
                
                // Add a row for the setting
                DoRow(rect, ref y2, medicalCareCategory, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        // Draw a medical default settings row
        private static void DoRow(Rect rect, ref float y, MedicalCareCategory category, string medicalDefaultKey, string labelKey, string tipKey)
        {
            float labelWidth = 230f;
            Rect rect1 = new Rect(rect.x, y, rect.width, MedicalCareUtility.CareSetterHeight);
            Rect rect2 = new Rect(rect.x, y, labelWidth, MedicalCareUtility.CareSetterHeight);
            Rect rect3 = new Rect(labelWidth, y, MedicalCareUtility.CareSetterWidth, MedicalCareUtility.CareSetterHeight);
            if (Mouse.IsOver(rect1))
                Widgets.DrawLightHighlight(rect1);
            TooltipHandler.TipRegionByKey(rect1, tipKey);
            string label = (string)labelKey.Translate();
            Widgets.LabelFit(rect2, label);
            MedicalCareSetter(rect3, category, medicalDefaultKey);
            y += RowHeight;
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
            
            int medicalCareCategoryCount = Enum.GetNames(typeof(MedicalCareCategory)).Length;
            
            Rect rect1 = new Rect(rect.x, rect.y, rect.width / medicalCareCategoryCount, rect.height);
            for (int index = 0; index < medicalCareCategoryCount; ++index)
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