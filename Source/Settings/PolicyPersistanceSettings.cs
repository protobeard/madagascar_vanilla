using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MadagascarVanilla.Settings
{
    public partial class MadagascarVanillaPersistables
    {
        private const string ColonistMedicalDefaultKey = "colonistMedicalDefault";
        private const string PrisonerMedicalDefaultKey = "prisonerMedicalDefault";
        private const string SlaveMedicalDefaultKey = "slaveMedicalDefault";
        private const string GhoulMedicalDefaultKey = "ghoulMedicalDefault";
        private const string TamedAnimalMedicalDefaultKey = "tamedAnimalMedicalDefault";
        private const string FriendlyMedicalDefaultKey = "friendlyMedicalDefault";
        private const string NeutralMedicalDefaultKey = "neutralMedicalDefault";
        private const string HostileMedicalDefaultKey = "hostileMedicalDefault";
        private const string NoFactionMedicalDefaultKey = "noFactionMedicalDefault";
        private const string WildlifeMedicalDefaultKey = "wildlifeMedicalDefault";
        private const string EntityMedicalDefaultKey = "entityMedicalDefault";

        
        // Mod setting name to Playsettings field name
        public static readonly Dictionary<string, string> MedicalDefaultsDict = new Dictionary<string, string>()
        {
            { ColonistMedicalDefaultKey, "defaultCareForColonist" },
            { PrisonerMedicalDefaultKey, "defaultCareForPrisoner" },
            { SlaveMedicalDefaultKey, "defaultCareForSlave" },
            { GhoulMedicalDefaultKey, "defaultCareForGhouls" },
            { TamedAnimalMedicalDefaultKey, "defaultCareForTamedAnimal" },
            { FriendlyMedicalDefaultKey, "defaultCareForFriendlyFaction" },
            { NeutralMedicalDefaultKey, "defaultCareForNeutralFaction" },
            { HostileMedicalDefaultKey, "defaultCareForHostileFaction" },
            { NoFactionMedicalDefaultKey, "defaultCareForNoFaction" },
            { WildlifeMedicalDefaultKey, "defaultCareForWildlife" },
            { EntityMedicalDefaultKey, "defaultCareForEntities" },
        };
        
        // Mod setting name to Playsettings default MedicalCareCategory value
        public static readonly Dictionary<string, MedicalCareCategory> MedicalDefaultsCareDict = new Dictionary<string, MedicalCareCategory>()
        {
            { ColonistMedicalDefaultKey, MedicalCareCategory.Best },
            { PrisonerMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { SlaveMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { GhoulMedicalDefaultKey, MedicalCareCategory.NoMeds },
            { TamedAnimalMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { FriendlyMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { NeutralMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { HostileMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { NoFactionMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { WildlifeMedicalDefaultKey, MedicalCareCategory.HerbalOrWorse },
            { EntityMedicalDefaultKey, MedicalCareCategory.NoMeds },
        };
        
        // Mod setting name to Dialog_MedicalDefaults label and tooltip/description
        public static readonly Dictionary<string, (string, string)> MedicalDefaultsSettingToHelpDict = new Dictionary<string, (string label, string tip)>()
        {
            { ColonistMedicalDefaultKey, ("MedGroupColonists", "MedGroupColonistsDesc") },
            { PrisonerMedicalDefaultKey, ("MedGroupPrisoners", "MedGroupColonistsDesc") },
            { SlaveMedicalDefaultKey, ("MedGroupSlaves", "MedGroupSlavesDesc") },
            { GhoulMedicalDefaultKey, ("MedGroupGhouls", "MedGroupGhoulsDesc") },
            { TamedAnimalMedicalDefaultKey, ("MedGroupTamedAnimals", "MedGroupTamedAnimalsDesc") },
            { FriendlyMedicalDefaultKey, ("MedGroupFriendlyFaction", "MedGroupFriendlyFactionDesc") },
            { NeutralMedicalDefaultKey, ("MedGroupNeutralFaction", "MedGroupNeutralFactionDesc") },
            { HostileMedicalDefaultKey, ("MedGroupHostileFaction", "MedGroupHostileFactionDesc") },
            { NoFactionMedicalDefaultKey, ("MedGroupNoFaction", "MedGroupNoFactionDesc") },
            { WildlifeMedicalDefaultKey, ("MedGroupWildlife", "MedGroupWildlifeDesc") },
            { EntityMedicalDefaultKey, ("MedGroupEntities", "MedGroupEntitiesDesc") },
        };

        private const float VerticalElementSpacing = 10f;
        private const float MedicalRowSpacing = 6f;
        private const float MedicalRowHeight = MedicalCareUtility.CareSetterHeight + MedicalRowSpacing;
        
        private void DoPolicyPersistenceSettingsContent(Rect rect, Listing_Standard listingStandard)
        {
            // Listing_Standard listingStandard = new Listing_Standard();
            // listingStandard.Begin(rect);
            
            listingStandard.Label("MV_PolicyPersistenceSettingsTitle".Translate());

            // Policies
            listingStandard.CheckboxLabeled("MV_EnablePersistingApparelPolicies".Translate(), ref EnablePersistingApparelPolicies, "MV_EnablePersistingApparelPoliciesTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnablePersistingDrugPolicies".Translate(), ref EnablePersistingDrugPolicies, "MV_EnablePersistingDrugPoliciesTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnablePersistingFoodPolicies".Translate(), ref EnablePersistingFoodPolicies, "MV_EnablePersistingFoodPoliciesTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnablePersistingReadingPolicies".Translate(), ref EnablePersistingReadingPolicies, "MV_EnablePersistingReadingPoliciesTooltip".Translate());
            listingStandard.CheckboxLabeled("MV_EnablePersistingMedicalSettings".Translate(), ref EnablePersistingMedicalSettings, "MV_EnablePersistingMedicalSettingsTooltip".Translate());
            
            float y = listingStandard.CurHeight + VerticalElementSpacing;
            Text.Font = GameFont.Small;
            Widgets.Label(rect, ref y, "DefaultMedicineSettingsDesc".Translate());
            float y2 = y + VerticalElementSpacing;
            Text.Anchor = TextAnchor.MiddleLeft;
            
            foreach (var (medicalDefaultKey, medicalDefaultHelp) in MedicalDefaultsSettingToHelpDict)
            {
                // Bail if we don't have the right DLCs for the setting
                if (!ModsConfig.IdeologyActive && medicalDefaultKey == SlaveMedicalDefaultKey ||
                    !ModsConfig.AnomalyActive && medicalDefaultKey == GhoulMedicalDefaultKey ||
                    !ModsConfig.AnomalyActive && medicalDefaultKey == EntityMedicalDefaultKey)
                    continue;

                switch (medicalDefaultKey)
                {
                    case ColonistMedicalDefaultKey:
                        DoRow(rect, ref y2, ColonistMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case PrisonerMedicalDefaultKey:
                        DoRow(rect, ref y2, PrisonerMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case SlaveMedicalDefaultKey:
                        DoRow(rect, ref y2, SlaveMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case TamedAnimalMedicalDefaultKey:
                        DoRow(rect, ref y2, TamedAnimalMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case WildlifeMedicalDefaultKey:
                        DoRow(rect, ref y2, WildlifeMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case FriendlyMedicalDefaultKey:
                        DoRow(rect, ref y2, FriendlyMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case NeutralMedicalDefaultKey:
                        DoRow(rect, ref y2, NeutralMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case HostileMedicalDefaultKey:
                        DoRow(rect, ref y2, HostileMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case NoFactionMedicalDefaultKey:
                        DoRow(rect, ref y2, NoFactionMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case GhoulMedicalDefaultKey:
                        DoRow(rect, ref y2, GhoulMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    case EntityMedicalDefaultKey:
                        DoRow(rect, ref y2, EntityMedicalDefault, medicalDefaultKey, medicalDefaultHelp.Item1, medicalDefaultHelp.Item2);
                        break;
                    default:
                        Log.Error($"Unknown medical default key: {medicalDefaultKey}");
                        break;
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
            
            listingStandard.GetRect(y2);
            
            //listingStandard.End();
        }
        
        // Draw a medical default settings row
        private void DoRow(Rect rect, ref float y, MedicalCareCategory category, string medicalDefaultKey, string labelKey, string tipKey)
        {
            float labelWidth = 230f;
            Rect rect1 = new Rect(rect.x, y, rect.width, MedicalCareUtility.CareSetterHeight);
            Rect rect2 = new Rect(rect.x, y, labelWidth, MedicalCareUtility.CareSetterHeight);
            Rect rect3 = new Rect(labelWidth, y, MedicalCareUtility.CareSetterWidth, MedicalCareUtility.CareSetterHeight);
            if (Mouse.IsOver(rect1))
                Widgets.DrawLightHighlight(rect1);
            TooltipHandler.TipRegionByKey(rect1, tipKey);
            string label = labelKey.Translate();
            Widgets.LabelFit(rect2, label);
            MedicalCareSetter(rect3, category, medicalDefaultKey);
            y += MedicalRowHeight;
        }
        
        // FIXME: painting doesn't work. Either remove the code or fix it.
        // TODO: what is the uniqueId number below? Where does it come from? Looks like a constant from somewhere...
        // Create our own copy of MedicalCareUtility.MedicalCareSetter so that we can
        // ensure that our Mod Settings window write to all the places we need.
        private void MedicalCareSetter(Rect rect, MedicalCareCategory currentlySelectedMedicalCareCategory, string medicalDefaultKey)
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
                GUI.DrawTexture(rect1, careTextures[index]);
                Widgets.DraggableResult result = Widgets.ButtonInvisibleDraggable(rect1);
                if (result == Widgets.DraggableResult.Dragged)
                    medicalCarePainting = true;
                
                if (medicalCarePainting && Mouse.IsOver(rect1) && currentlySelectedMedicalCareCategory != newMedicalCareCategory || (result == Widgets.DraggableResult.Pressed || result == Widgets.DraggableResult.DraggedThenPressed))
                {
                    currentlySelectedMedicalCareCategory = newMedicalCareCategory;
                    
                    
                    switch (medicalDefaultKey)
                    {
                        case ColonistMedicalDefaultKey:
                            ColonistMedicalDefault = newMedicalCareCategory;
                            break;
                        case PrisonerMedicalDefaultKey:
                            PrisonerMedicalDefault = newMedicalCareCategory;
                            break;
                        case SlaveMedicalDefaultKey:
                            SlaveMedicalDefault = newMedicalCareCategory;
                            break;
                        case TamedAnimalMedicalDefaultKey:
                            TamedAnimalMedicalDefault = newMedicalCareCategory;
                            break;
                        case WildlifeMedicalDefaultKey:
                            WildlifeMedicalDefault = newMedicalCareCategory;
                            break;
                        case FriendlyMedicalDefaultKey:
                            FriendlyMedicalDefault = newMedicalCareCategory;
                            break;
                        case NeutralMedicalDefaultKey:
                            NeutralMedicalDefault = newMedicalCareCategory;
                            break;
                        case HostileMedicalDefaultKey:
                            HostileMedicalDefault = newMedicalCareCategory;
                            break;
                        case NoFactionMedicalDefaultKey:
                            NoFactionMedicalDefault = newMedicalCareCategory;
                            break;
                        case GhoulMedicalDefaultKey:
                            GhoulMedicalDefault = newMedicalCareCategory;
                            break;
                        case EntityMedicalDefaultKey:
                            EntityMedicalDefault = newMedicalCareCategory;
                            break;
                        default:
                            Log.Error($"Unknown medical default key: {medicalDefaultKey}");
                            break;
                    }
                    
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }
                if (currentlySelectedMedicalCareCategory == newMedicalCareCategory)
                    Widgets.DrawBox(rect1, 2);
                if (Mouse.IsOver(rect1))
                    TooltipHandler.TipRegion(rect1, (Func<string>) (() => newMedicalCareCategory.GetLabel().CapitalizeFirst()), 632165 + index * 17);
                rect1.x += rect1.width;
            }
            if (Input.GetMouseButton(0))
                return;
            medicalCarePainting = false;
        }
        
        // Pull the mod medical default settings out and assign them to the playsettings in game.
        public void LoadMedicalSettingsIntoPlaySettings(PlaySettings playSettings)
        {
            playSettings.defaultCareForColonist = ColonistMedicalDefault;
            playSettings.defaultCareForPrisoner = PrisonerMedicalDefault;
            if (ModsConfig.IdeologyActive)
                playSettings.defaultCareForSlave = SlaveMedicalDefault;
                
            playSettings.defaultCareForTamedAnimal = TamedAnimalMedicalDefault;
            playSettings.defaultCareForWildlife = WildlifeMedicalDefault;
            
            playSettings.defaultCareForFriendlyFaction = FriendlyMedicalDefault;
            playSettings.defaultCareForNeutralFaction = NeutralMedicalDefault;
            playSettings.defaultCareForHostileFaction = HostileMedicalDefault;
            playSettings.defaultCareForNoFaction = NoFactionMedicalDefault;
        
            if (ModsConfig.AnomalyActive)
            {
                playSettings.defaultCareForGhouls = GhoulMedicalDefault;
                playSettings.defaultCareForEntities = EntityMedicalDefault;
            }
        }
        
        // Save the medical default settings from PlaySettings into our mod settings.
        // Ideology and Anomaly add pawn types, and if not active we don't want to overwrite our mod settings
        // with the game defaults.
        public void PersistMedicalSettings(PlaySettings playSettings)
        {
            ColonistMedicalDefault = playSettings.defaultCareForColonist;
            PrisonerMedicalDefault = playSettings.defaultCareForPrisoner;
            if (ModsConfig.IdeologyActive)
                SlaveMedicalDefault = playSettings.defaultCareForSlave;
                
            TamedAnimalMedicalDefault = playSettings.defaultCareForTamedAnimal;
            WildlifeMedicalDefault = playSettings.defaultCareForWildlife;
            
            FriendlyMedicalDefault = playSettings.defaultCareForFriendlyFaction;
            NeutralMedicalDefault = playSettings.defaultCareForNeutralFaction;
            HostileMedicalDefault = playSettings.defaultCareForHostileFaction;
            NoFactionMedicalDefault = playSettings.defaultCareForNoFaction;
        
            if (ModsConfig.AnomalyActive)
            {
                GhoulMedicalDefault = playSettings.defaultCareForGhouls;
                EntityMedicalDefault = playSettings.defaultCareForEntities;
            }
            
            this.Write();
        }

        // Reset all medical defaults mod settings to RimWorld defaults
        // public static void ResetMedicalDefaults()
        // {
        //     foreach ((string medicalDefaultSettingKey, MedicalCareCategory category) in MedicalDefaultsCareDict)
        //     {
        //         SettingsManager.SetSetting(MadagascarVanillaMod.ModId, medicalDefaultSettingKey, category.ToString());
        //     }
        //     
        //     // Save settings to disk, just like the XML Extensions Settings does when the mod settings
        //     // window closes. Necessary here so that when a player changes medical default settings in
        //     // game (and not in the mod options window) they will be persisted across game restarts.
        //     LoadedModManager.GetMod(typeof (XmlMod)).WriteSettings();
        // }
    }
    
    // FIXME: medical settings reset button
    // public class ResetMedicalDefaultsAction : ActionContainer
    // {
    //     protected override bool ApplyAction()
    //     {
    //         MedicalDefaults.ResetMedicalDefaults();
    //         return true;
    //     }
    // }
}