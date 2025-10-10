using System;
using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using MadagascarVanilla.Settings;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    // 1. Game is loaded, at mod settings
    //      a. load game
    //      b. quit, then make new game
    //      c. close mod settings, open medical settings dialog
    // 2. Game is loaded, at medical dialog
    //      a. load game
    //      b. quit, then make new game
    //      c. close medical dialog, open mod settings
    // 3. Game is not loaded, at mod settings
    //      a. load game
    //      b. new game
    [HarmonyPatch]
    public static class MedicalDefaultsPatch
    {
        
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

            Log.Message("Dialog_MedicalDefaultsClosePatch " + MedicalDefaults.PersistMedicalSettingsKey);
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            PlaySettings playSettings = Find.PlaySettings;
            if (persistMedicalSettings && playSettings != null)
                PersistMedicalSettings(playSettings);
        }
        
        // Load mod medical settings into Playsettings before opening the medical defaults dialog window.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch("PreOpen")]
        public static void DialogMedicalDefaultsOpenPatchPostfix(Window __instance)
        {
            if (!(__instance is Dialog_MedicalDefaults))
                return;

            Log.Message("Dialog_MedicalDefaultsOpenPatch " + MedicalDefaults.PersistMedicalSettingsKey);
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            PlaySettings playSettings = Find.PlaySettings;
            if (persistMedicalSettings && playSettings != null)
                LoadMedicalSettingsIntoPlaySettings(playSettings);
        }
        
        // Load mod medical settings into Playsettings on game load (in case they have been modified outside the game).
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("LoadGame")]
        public static void LoadGamePostfix(Game __instance)
        {
            Log.Message("Loading game postfix");
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            if (persistMedicalSettings)
                LoadMedicalSettingsIntoPlaySettings(__instance.playSettings);
        }
        
        // On New Game after PlaySetting instantiation pull the medical default settings from our mod settings
        // and configure the PlaySettings. InitNewGame postfix is too late (starting pawns already have medical settings set).
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            Log.Message("PlaySettingsConstructorPostfix");
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
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
            
            Traverse traverse = Traverse.Create(playSettings);
            string medicalCareCategoryName;
            MedicalCareCategory medicalCareCategory;
            bool medicalCategorySettingExists = false;
            
            foreach (var (medicalDefaultKey, medicalDefaultField) in MedicalDefaults.MedicalDefaultsDict)
            {
                // Log.Message($"medical default key: {medicalDefaultKey}");
                // Log.Message("medicalDefaultField " + medicalDefaultField);
                
                medicalCategorySettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, medicalDefaultKey, out medicalCareCategoryName);
                if (!medicalCategorySettingExists)
                {
                    Log.Message("medical default not found: " + medicalDefaultKey);
                    continue;
                }
                // Log.Message("medicalCareCategoryName " + medicalCareCategoryName);
                // Log.Message(" ");
                
                if (medicalCareCategoryName != null)
                {
                    bool parsed = Enum.TryParse(medicalCareCategoryName, false, out medicalCareCategory);
                    if (!parsed)
                    {
                        Log.Message("parsing enum failed for: " + medicalCareCategoryName);
                    }
                    
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
            
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.ColonistMedicalDefault, playSettings.defaultCareForColonist.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PrisonerMedicalDefault, playSettings.defaultCareForPrisoner.ToString());
            if (ModsConfig.IdeologyActive)
                SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.SlaveMedicalDefault, playSettings.defaultCareForSlave.ToString());
                
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.TamedAnimalMedicalDefault, playSettings.defaultCareForTamedAnimal.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.WildlifeMedicalDefault, playSettings.defaultCareForWildlife.ToString());
                
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.FriendlyMedicalDefault, playSettings.defaultCareForFriendlyFaction.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.NeutralMedicalDefault, playSettings.defaultCareForNeutralFaction.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.HostileMedicalDefault, playSettings.defaultCareForHostileFaction.ToString());
            SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.NoFactionMedicalDefault, playSettings.defaultCareForNoFaction.ToString());

            if (ModsConfig.AnomalyActive)
            {
                SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.GhoulMedicalDefault, playSettings.defaultCareForGhouls.ToString());
                SettingsManager.SetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.EntityMedicalDefault, playSettings.defaultCareForEntities.ToString());
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