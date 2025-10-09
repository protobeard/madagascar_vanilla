using System;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class MedicalDefaultsPatch
    {
        private const string PersistMedicalSettingsKey = "persistMedicalSettings";
        
        private const string ColonistMedicalDefault = "colonistMedicalDefault";
        private const string PrisonerMedicalDefault = "prisonerMedicalDefault";
        private const string SlaveMedicalDefault = "slaveMedicalDefault";
        
        private const string TamedAnimalMedicalDefault = "tameAnimalMedicalDefault";
        private const string WildlifeMedicalDefault = "wildlifeMedicalDefault";
        
        private const string FriendlyMedicalDefault = "friendlyMedicalDefault";
        private const string NeutralMedicalDefault = "neutralMedicalDefault";
        private const string HostileMedicalDefault = "hostileMedicalDefault";
        private const string NoFactionMedicalDefault = "noFactionMedicalDefault";

        private const string GhoulMedicalDefault = "ghoulMedicalDefault";
        private const string EntityMedicalDefault = "entityMedicalDefault";
        
        // When the vanilla UI for setting medical defaults closes, update our mod settings
        // I'd love to patch something more specific than Window, but since Close is a virtual
        // method and not overriden in Dialog_MedicalDefaults Harmony won't patch it directly
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch("PostClose")]
        public static void DialogMedicalDefaultsClosePatchPostfix(Window __instance)
        {
            if (!(__instance is Dialog_MedicalDefaults))
                return;

            Log.Message("Dialog_MedicalDefaultsClosePatch " + PersistMedicalSettingsKey);
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistMedicalSettingsKey));
            PlaySettings playSettings = Find.PlaySettings;
            if (persistMedicalSettings && playSettings != null)
                PersistMedicalSettings(playSettings);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("LoadGame")]
        public static void LoadGamePostfix(Game __instance)
        {
            Log.Message("Loading game postfix");
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistMedicalSettingsKey));
            if (persistMedicalSettings)
                LoadMedicalSettingsIntoPlaySettings(__instance.playSettings);
        }
        
        // On PlaySetting instantiation pull the medical default settings from our mod settings
        // and configure the PlaySettings. InitNewGame postfix is too late (starting pawns already have medical settings set).
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            Log.Message("PlaySettingsConstructorPostfix");
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistMedicalSettingsKey));
            if (persistMedicalSettings)
                LoadMedicalSettingsIntoPlaySettings(__instance);
        }

        // Pull the mod medical default settings out and assign them to the game.
        private static void LoadMedicalSettingsIntoPlaySettings(PlaySettings playSettings)
        {
            Log.Message("Loading medical default settings");
            // foreach (string key in SettingsManager.GetKeys(MadagascarVanillaMod.ModId))
            // {
            //     Log.Message("setting key: " + key);
            // }
            
            // TODO: turn field names into constants -- just make the keys match?
            Dictionary<string, string> medicalDefaultsDict = new Dictionary<string, string>()
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
            
            Traverse traverse = Traverse.Create(playSettings);
            string medicalCareCategoryName;
            MedicalCareCategory medicalCareCategory;
            bool medicalCategorySettingExists = false;
            
            foreach (var (medicalDefaultKey, medicalDefaultField) in medicalDefaultsDict)
            {
                Log.Message($"medical default key: {medicalDefaultKey}");
                Log.Message("medicalDefaultField " + medicalDefaultField);
                
                medicalCategorySettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, medicalDefaultKey, out medicalCareCategoryName);
                if (!medicalCategorySettingExists)
                {
                    Log.Message("medical default not found: " + medicalDefaultKey);
                    continue;
                }
                Log.Message("medicalCareCategoryName " + medicalCareCategoryName);
                Log.Message(" ");
                
                if (medicalCareCategoryName != null)
                {
                    bool parsed = Enum.TryParse(medicalCareCategoryName, false, out medicalCareCategory);
                    if (!parsed)
                    {
                        Log.Message("parsing enum failed for: " + medicalCareCategoryName);
                    }
                    
                    // Sanity check - set everything to Best. Confirms that Traverse setting is working.
                    // medicalCareCategory = MedicalCareCategory.Best;
                    traverse.Field(medicalDefaultField).SetValue(medicalCareCategory);
                        
                    Log.Message("medicalCareCategory for " + medicalDefaultField + ": " + medicalCareCategory);
                }
                
                Log.Message(" ");
            }
        }
        
        // Save the medical default settings from PlaySettings into our mod settings.
        // Ideology and Anomaly add pawn types, and if not active we don't want to overwrite our mod settings
        // with the game defaults.
        private static void PersistMedicalSettings(PlaySettings playSettings)
        {
            Log.Message("PersistMedicalSettings");
            
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, ColonistMedicalDefault, playSettings.defaultCareForColonist.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, PrisonerMedicalDefault, playSettings.defaultCareForPrisoner.ToString());
            if (ModsConfig.IdeologyActive)
                SettingsManager.SetSetting(MadagascarVanillaMod.ModId, SlaveMedicalDefault, playSettings.defaultCareForSlave.ToString());
                
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, TamedAnimalMedicalDefault, playSettings.defaultCareForTamedAnimal.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, WildlifeMedicalDefault, playSettings.defaultCareForWildlife.ToString());
                
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, FriendlyMedicalDefault, playSettings.defaultCareForFriendlyFaction.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, NeutralMedicalDefault, playSettings.defaultCareForNeutralFaction.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, HostileMedicalDefault, playSettings.defaultCareForHostileFaction.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, NoFactionMedicalDefault, playSettings.defaultCareForNoFaction.ToString());

            if (ModsConfig.AnomalyActive)
            {
                SettingsManager.SetSetting(MadagascarVanillaMod.ModId, GhoulMedicalDefault, playSettings.defaultCareForGhouls.ToString());
                SettingsManager.SetSetting(MadagascarVanillaMod.ModId, EntityMedicalDefault, playSettings.defaultCareForEntities.ToString());
            }
            
            // foreach (string key in SettingsManager.GetKeys(MadagascarVanillaMod.ModId))
            // {
            //     Log.Message("setting key (after persist): " + key);
            // }
            
            // Save settings to disk, just like the XML Extensions Settings does when the mod settings
            // window closes. Necessary here so that when a player changes medical default settings in
            // game (and not in the mod options window) they will be persisted across game restarts.
            LoadedModManager.GetMod(typeof (XmlMod)).WriteSettings();
        }
    }
}