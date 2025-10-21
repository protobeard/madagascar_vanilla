using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using XmlExtensions;
using static MadagascarVanilla.MadagascarVanillaPersistables;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class NewGameSetupPatches
    {
        private const string PersistNewGameSetupKey = "persistNewGameSetup";
        private static bool PersistNewGameSetup;
        
        // Track whether we've loaded our settings since the user can go forward and back through
        // the new game setup UI, and we don't want to overwrite any changes they may have made
        // on each page.
        private static bool ScenarioSettingsLoaded;
        private static bool StorytellerSettingsLoaded;
        private static bool WorldGeneratorSettingsLoaded;
        private static bool IdeoligionSettingsLoaded;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page))]
        [HarmonyPatch("DoBack")]
        public static void PageDoBackPostfix(Page __instance)
        {
            if (!PersistNewGameSetup)
                return;

            switch (__instance)
            {
                case Page_SelectScenario scenarioWindow:
                    PersistScenarioSettings(scenarioWindow);
                    ScenarioSettingsLoaded = false;
                    break;
                case Page_SelectStoryteller storytellerWindow:
                    PersistStorytellerSettings(storytellerWindow);
                    StorytellerSettingsLoaded = false;
                    break;
                case Page_CreateWorldParams worldParamsWindow:
                    PersistWorldParams(worldParamsWindow);
                    WorldGeneratorSettingsLoaded = false;
                    break;
                case Page_ChooseIdeoPreset ideoligionWindow:
                    PersistIdeoligionSettings(ideoligionWindow);
                    IdeoligionSettingsLoaded = false;
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_SelectScenario))]
        [HarmonyPatch(nameof(Page_SelectScenario.PreOpen))]
        public static void SelectScenarioPreOpenPostfix(Page_SelectScenario __instance)
        {
            // Since this is the first part of new game setup cache the setting for whether we are persisting
            // settings. The rest of the time we can just check this value.
            PersistNewGameSetup = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistNewGameSetupKey));
            if (!PersistNewGameSetup)
                return;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_SelectScenario.PreOpen");
            
            Traverse traverse = Traverse.Create(__instance);
            
            SetValueOfField(traverse, "curScen", MadagascarVanillaMod.Persistables.SelectedScenario);
        }
        
        // Persist the selected scenario:
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_SelectScenario))]
        [HarmonyPatch("CanDoNext")]
        public static void SelectScenarioCanDoNextPostfix(Page_SelectScenario __instance, bool __result)
        {
            if (!PersistNewGameSetup || ScenarioSettingsLoaded)
                return;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_SelectStoryteller.CanDoNext");
            
            if (__result)
                PersistScenarioSettings(__instance);

            ScenarioSettingsLoaded = true;
        }

        private static void PersistScenarioSettings(Page_SelectScenario window)
        {
            Traverse traverse = Traverse.Create(window);
                
            // Persist scenario
            MadagascarVanillaMod.Persistables.SelectedScenario = traverse.Field("curScen").GetValue<Scenario>();
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        // Set defaults from mod settings for:
        //  - Storyteller
        //  - DifficultyDef/Difficulty
        //  - Commitment Mode
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch(nameof(Page_SelectStoryteller.PreOpen))]
        public static void SelectStorytellerPreOpenPostfix(Page_SelectStoryteller __instance)
        { 
            if (!PersistNewGameSetup || StorytellerSettingsLoaded || __instance == null)
                return;

            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_SelectStoryteller.PreOpen");
            
            Traverse traverse = Traverse.Create(__instance);
            
            SetValueOfField<StorytellerDef>(traverse, "storyteller", MadagascarVanillaMod.Persistables.StorytellerDef);

            DifficultyDef difficultyDef = MadagascarVanillaMod.Persistables.DifficultyDef;
            if (difficultyDef != null && DefDatabase<DifficultyDef>.AllDefsListForReading.Contains(difficultyDef))
            {
                SetValueOfField<DifficultyDef>(traverse, "difficulty", difficultyDef);
                SetValueOfField(traverse, "difficultyValues", MadagascarVanillaMod.Persistables.Difficulty);
            }
            
            // Set commitment mode
            bool permadeath = MadagascarVanillaMod.Persistables.Permadeath;
            if (MadagascarVanillaMod.Verbose()) Log.Message($"GameInitData set commitment mode to: {permadeath}");
            
            // permadeathChoosen tracks not whether "permadeath" itself was picked, but that *something* was picked
            // for the permadeath mode. So we always set it to true.
            Find.GameInitData.permadeath = permadeath;
            Find.GameInitData.permadeathChosen = true;

            StorytellerSettingsLoaded = true;
        }

        // Persist the bits of the Storyteller that we need for setup:
        // - storytellerDef
        // - difficultyDef/difficulty
        // - commitment mode
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch("CanDoNext")]
        public static void SelectStorytellerCanDoNextPostfix(Page_SelectStoryteller __instance, bool __result)
        {
            if (!PersistNewGameSetup)
                return;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_SelectStoryteller.CanDoNext");
            
            if (__result)
                PersistStorytellerSettings(__instance);
        }
        
        // Save storyteller settings if modified in game
        // - Storyteller
        // - DifficultDef
        // - Difficulty
        // note: commitment mode can't be changed in game, so don't do anything with it here.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_SelectStorytellerInGame))]
        [HarmonyPatch(nameof(Page_SelectStorytellerInGame.PreClose))]
        private static void SelectStorytellerInGamePreClosePostfix()
        {
            PersistNewGameSetup = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistNewGameSetupKey));
            if (!PersistNewGameSetup)
                return;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_SelectStorytellerInGame.PreClose");
            
            PersistStorytellerInGameSettings();
        }

        private static void PersistStorytellerInGameSettings()
        {
            Storyteller storyteller = Current.Game.storyteller;
                
            MadagascarVanillaMod.Persistables.StorytellerDef = storyteller.def;
            MadagascarVanillaMod.Persistables.DifficultyDef = storyteller.difficultyDef;
            MadagascarVanillaMod.Persistables.Difficulty = storyteller.difficulty;
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }

        private static void PersistStorytellerSettings(Page_SelectStoryteller window)
        {
            Traverse traverse = Traverse.Create(window);
                
            MadagascarVanillaMod.Persistables.StorytellerDef = traverse.Field("storyteller").GetValue<StorytellerDef>();
            MadagascarVanillaMod.Persistables.DifficultyDef = traverse.Field("difficulty").GetValue<DifficultyDef>();
            MadagascarVanillaMod.Persistables.Difficulty = traverse.Field("difficultyValues").GetValue<Difficulty>();
            MadagascarVanillaMod.Persistables.Permadeath = Find.GameInitData.permadeath;
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        // Set defaults from mod settings for:
        //  - planet coverage
        //  - rainfall
        //  - temperature
        //  - population density
        //  - landmark density
        //  - factions
        //  - pollution
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_CreateWorldParams))]
        [HarmonyPatch(nameof(Page_CreateWorldParams.PreOpen))]
        public static void CreateWorldParamsPreOpenPostfix(Page_CreateWorldParams __instance)
        {
            if (!PersistNewGameSetup || WorldGeneratorSettingsLoaded || __instance == null)
                return;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_CreateWorldParams.PreOpen");
            
            Traverse traverse = Traverse.Create(__instance);
            
            List<FactionDef> factions = MadagascarVanillaMod.Persistables.Factions;
            List<FactionDef> validFactions = new List<FactionDef>();
            if (factions != null)
            {
                foreach (FactionDef faction in factions)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"Page_CreateWorldParams processing: {faction.defName}");
                    
                    if (!FactionGenerator.ConfigurableFactions.Contains(faction))
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"Madagascar Vanilla: Unknown faction ({faction.defName})), skipping.");
                        continue;
                    }
                    validFactions.Add(faction);
                }
                traverse.Field("factions").SetValue(validFactions);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: Faction list null, skip trying to set defaults. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            SetValueOfField(traverse, "planetCoverage", MadagascarVanillaMod.Persistables.PlanetCoverage);
            SetValueOfField(traverse, "rainfall", MadagascarVanillaMod.Persistables.Rainfall);
            SetValueOfField(traverse, "temperature", MadagascarVanillaMod.Persistables.Temperature);
            SetValueOfField(traverse, "population", MadagascarVanillaMod.Persistables.Population);
            SetValueOfField(traverse, "landmarkDensity", MadagascarVanillaMod.Persistables.LandmarkDensity);
            SetValueOfField(traverse, "pollution", MadagascarVanillaMod.Persistables.Pollution);
            
            int? mapSize = MadagascarVanillaMod.Persistables.MapSize;
            if (mapSize != null)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Page_CreateWorldParams patch set mapSize: {mapSize}");
                Find.GameInitData.mapSize = (int) mapSize;
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no mapSize value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            Season? season = MadagascarVanillaMod.Persistables.StartingSeason;
            if (season != null)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Page_CreateWorldParams patch set season: {season}");
                Find.GameInitData.startingSeason = (Season) season;
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no season value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }

            WorldGeneratorSettingsLoaded = true;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_CreateWorldParams))]
        [HarmonyPatch("CanDoNext")]
        public static void CreateWorldParamsCanDoNextPostfix(Page_CreateWorldParams __instance, bool __result)
        {
            if (!PersistNewGameSetup)
                return;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_CreateWorldParams.CanDoNext");
            
            PersistWorldParams(__instance);
        }

        private static void PersistWorldParams(Page_CreateWorldParams window)
        {
            Traverse traverse = Traverse.Create(window);
            
            MadagascarVanillaMod.Persistables.Factions = traverse.Field("factions").GetValue<List<FactionDef>>();
            
            MadagascarVanillaMod.Persistables.PlanetCoverage = traverse.Field("planetCoverage").GetValue<float>();
            MadagascarVanillaMod.Persistables.Rainfall = traverse.Field("rainfall").GetValue<OverallRainfall>();
            MadagascarVanillaMod.Persistables.Temperature = traverse.Field("temperature").GetValue<OverallTemperature>();
            MadagascarVanillaMod.Persistables.Population = traverse.Field("population").GetValue<OverallPopulation>();
            MadagascarVanillaMod.Persistables.LandmarkDensity = traverse.Field("landmarkDensity").GetValue<LandmarkDensity>();
            MadagascarVanillaMod.Persistables.Pollution = traverse.Field("pollution").GetValue<float>();
            
            MadagascarVanillaMod.Persistables.MapSize = Find.GameInitData.mapSize;
            MadagascarVanillaMod.Persistables.StartingSeason = Find.GameInitData.startingSeason;
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        // Set defaults from mod settings for:
        // - Preset Selection (type of ideoligion)
        // - Ideoligion (if preset is "Preset", otherwise null)
        // - Structure
        // - Styles
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch(nameof(Window.PreOpen))]
        public static void WindowPreOpenPostfix(Window __instance)
        {
            if (!PersistNewGameSetup || IdeoligionSettingsLoaded || !(__instance is Page_ChooseIdeoPreset page))
                return;
                        
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_ChooseIdeoPreset.PreOpen");

            Traverse traverse = Traverse.Create(page);
            
            // Set presetSelection and selectedIdeo -- this is all a bit awkward because the PresetSeletion enum
            // is private, so we've stored it as a new enum: PresetSelectionType. So we grab our PresetSelectionType,
            // confirm it matches one of the PresetSelection values and assign that (using reflection).
            PresetSelectionType? presetSelectionType = MadagascarVanillaMod.Persistables.PresetSelection;
            if (presetSelectionType != null)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Page_ChooseIdeoPreset patch set preset selection: {presetSelectionType}");

                // Grab the "real" PresetSelection private enum
                Type presetSelectionEnum = typeof(Page_ChooseIdeoPreset).GetNestedType("PresetSelection", BindingFlags.NonPublic);
                object presetSelection = null;
                var enumValueArray = Enum.GetValues(presetSelectionEnum);
                foreach (var enumValue in enumValueArray)
                {
                    Log.Message($"Checking {enumValue.GetType()} {enumValue} against {presetSelectionType}");
                    if (enumValue.ToString() == presetSelectionType.ToString())
                    {
                        presetSelection = enumValue;
                        break;
                    }
                }
                
                if (presetSelection != null)
                {
                    // If a Preset Ideoligion is selected, assign that to selectedIdeo. Otherwise, set the selectedIdeo to null.
                    if (presetSelectionType == PresetSelectionType.Preset)
                    {
                        if (MadagascarVanillaMod.Verbose()) Log.Message("Page_ChooseIdeoPreset: we have a Preset ideoligion, should set it.");
                        SetValueOfField<IdeoPresetDef>(traverse, "selectedIdeo", MadagascarVanillaMod.Persistables.Ideoligion);
                    }
                    else
                    {
                        // Set the preset ideoligion to null
                        if (MadagascarVanillaMod.Verbose()) Log.Message("Set the selectedIdeo to null as we don't have a Preset.");
                        traverse.Field("selectedIdeo").SetValue(null);
                    }
                    
                    // Assign the PresetSelection into the presetSelection private field
                    //Log.Message($"setting newPresetSelection: {presetSelection}");
                    traverse.Field("presetSelection").SetValue(presetSelection);
                }
            }
            else
            {
                Log.Message($"Madagascar Vanilla: Preset selection missing, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            // Set Structure
            SetValueOfField<MemeDef>(traverse, "structure", MadagascarVanillaMod.Persistables.Structure);
            
            // Set Styles
            List<StyleCategoryDef> styleCategories = MadagascarVanillaMod.Persistables.StyleCategories;
            if (styleCategories != null && styleCategories.Count > 0)
            {
                List<StyleCategoryDef> validStyleCategories = new List<StyleCategoryDef>();
                foreach (StyleCategoryDef styleCategoryDef in styleCategories)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"Processing style category def: {styleCategoryDef}");
                    
                    if (styleCategoryDef == null)
                        continue;
                    
                    if (DefDatabase<StyleCategoryDef>.AllDefsListForReading.Contains(styleCategoryDef))
                    {
                         if (MadagascarVanillaMod.Verbose()) Log.Message($"Page_ChooseIdeoPreset patch set style category: {styleCategoryDef.defName}");
                         validStyleCategories.Add(styleCategoryDef);
                    }
                    else
                    { 
                        Log.Message($"Madagascar Vanilla: Unknown style category ({styleCategoryDef.defName}), skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
                    }
                }
                
                traverse.Field("selectedStyles").SetValue(validStyleCategories);
                RecacheStyleCategoriesWithPriority(page);
            }
            else
            { 
                Log.Message($"Madagascar Vanilla: Style categories is null, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }

            IdeoligionSettingsLoaded = true;
        }

        // Persist the bits of the Ideoligion that we need for setup:
        // - Preset Selection (type of ideoligion)
        // - Ideoligion (if preset is "Preset", otherwise null)
        // - Structure
        // - Styles
        // TODO: check if ideoligion is loaded.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_ChooseIdeoPreset))]
        [HarmonyPatch("DoNext")]
        public static void ChooseIdeoPresetDoNextPostfix(Page_ChooseIdeoPreset __instance)
        {
            if (!PersistNewGameSetup)
                return;
                        
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_ChooseIdeoPreset.DoNext");

            PersistIdeoligionSettings(__instance);
        }

        private static void PersistIdeoligionSettings(Page_ChooseIdeoPreset window)
        {
            Traverse traverse = Traverse.Create(window);
            
            MadagascarVanillaMod.Persistables.PresetSelection = traverse.Field("presetSelection").GetValue<PresetSelectionType>();
            MadagascarVanillaMod.Persistables.Ideoligion = traverse.Field("selectedIdeo").GetValue<IdeoPresetDef>();
            MadagascarVanillaMod.Persistables.Structure = traverse.Field("selectedStructure").GetValue<MemeDef>();
            MadagascarVanillaMod.Persistables.StyleCategories = traverse.Field("selectedStyles").GetValue<List<StyleCategoryDef>>();
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        // On new game start, reset the "settings loaded" state trackers so that
        // if we make another new game settings will still get loaded.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_ConfigureStartingPawns))]
        [HarmonyPatch("DoNext")]
        public static void ConfigureStartingPawnsDoNextPostfix(Page_ConfigureStartingPawns __instance)
        {
            if (!PersistNewGameSetup)
                return;
                        
            if (MadagascarVanillaMod.Verbose()) Log.Message($"MadagascarVanilla.Page_ConfigureStartingPawns.DoNext");

            StorytellerSettingsLoaded = false;
            WorldGeneratorSettingsLoaded = false;
            IdeoligionSettingsLoaded = false;
            ScenarioSettingsLoaded = false;
        }
        
        // Need to recache the selectedStylesWithPriority private field so that the tooltips get instantiated properly.
        // TODO: remove this and just call the private method in Page_ChooseIdeoPreset instead.
        private static void RecacheStyleCategoriesWithPriority(Page_ChooseIdeoPreset __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            List<StyleCategoryDef> selectedStyles = traverse.Field("selectedStyles").GetValue<List<StyleCategoryDef>>();
            List<ThingStyleCategoryWithPriority> selectedStylesWithPriority = traverse.Field("selectedStylesWithPriority").GetValue<List<ThingStyleCategoryWithPriority>>();
            
            selectedStylesWithPriority.Clear();
            for (int i = 0; i < selectedStyles.Count; i++)
            {
                StyleCategoryDef styleCategoryDef = selectedStyles[i];
                if (styleCategoryDef != null)
                {
                    selectedStylesWithPriority.Add(new ThingStyleCategoryWithPriority(styleCategoryDef, 3 - i));
                }
            }
            traverse.Field("selectedStylesWithPriority").SetValue(selectedStylesWithPriority);
        }
        
        // Defs: Set the field using the Traverse, with some extra logging
        private static void SetValueOfField<T>(Traverse traverse, string fieldName, object value) where T : Def
        {
            if (value != null && DefDatabase<T>.AllDefsListForReading.Contains(value))
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Madagascar Vanilla: set value of {fieldName}: {value}");
                traverse.Field(fieldName).SetValue(value);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no {fieldName} value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
        }
        
        // Non-Defs: Set the field using the Traverse, with some extra logging
        private static void SetValueOfField(Traverse traverse, string fieldName, object value)
        {
            if (value != null)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Madagascar Vanilla: patch set value of {fieldName}: {value}");
                traverse.Field(fieldName).SetValue(value);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no {fieldName} value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
        }
    }
}