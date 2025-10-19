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
        
        // Track whether we've loaded our settings since the user can go forward and back through
        // the new game setup UI, and we don't want to overwrite any changes they may have made
        // on each page.
        private static bool StorytellerSettingsLoaded;
        private static bool WorldGeneratorSettingsLoaded;
        private static bool IdeoligionSettingsLoaded;

        [HarmonyPatch(typeof(Page))]
        [HarmonyPatch("DoBack")]
        public static void Postfix(Page __instance)
        {
            if (__instance is Page_SelectStoryteller)
                StorytellerSettingsLoaded = false;
            else if (__instance is Page_CreateWorldParams)
                WorldGeneratorSettingsLoaded = false;
            else if (__instance is Page_ChooseIdeoPreset)
                IdeoligionSettingsLoaded = false;
            else if (__instance is Page_ConfigureIdeo)
            {
                // FIXME: is this what we actually want? IN this case I've choosen to configure a custom Ideo, then
                // hit the back button. Structure and style should get reloaded from mod Config, but should the 
                // ideoligion type selection? Maybe this is a bad idea, b/c when we pressed "Next" on config custom,
                // it calls ResetSelection and we then save that out, with no styles/structure/preset ideo. So we'd
                // have to not do that... and then clear our settings after the custom ideo is created? Keep them for
                // the next game?
                // IdeoligionSettingsLoaded = false;
            }
                
        }
        
        // Set defaults from mod settings for:
        //  - Storyteller
        //  - DifficultyDef/Difficulty
        //  - Commitment Mode
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch(nameof(Page_SelectStoryteller.PreOpen))]
        public static void Postfix(Page_SelectStoryteller __instance)
        {
            if (__instance == null)
                return;
            
            if (StorytellerSettingsLoaded)
                return;

            if (MadagascarVanillaMod.Verbose())
                Log.Message("Page_SelectStoryteller patch");
            
            Traverse traverse = Traverse.Create(__instance);
            
            // Set storyteller
            StorytellerDef storytellerDef = MadagascarVanillaMod.Persistables.StorytellerDef;
            if (storytellerDef != null && DefDatabase<StorytellerDef>.AllDefsListForReading.Contains(storytellerDef))
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_SelectStoryteller patch set storyteller: {storytellerDef.defName}");
                traverse.Field("storyteller").SetValue(storytellerDef);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: Unknown storyteller ({storytellerDef}), skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            // Set difficulty/custom difficulty/anomaly difficulty
            DifficultyDef difficultyDef = MadagascarVanillaMod.Persistables.DifficultyDef;
            if (difficultyDef != null && DefDatabase<DifficultyDef>.AllDefsListForReading.Contains(difficultyDef))
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_SelectStoryteller patch set difficulty: {difficultyDef.defName}");
                
                traverse.Field("difficulty").SetValue(difficultyDef);
                traverse.Field("difficultyValues").SetValue(MadagascarVanillaMod.Persistables.Difficulty);
            }
            else
            { 
                Log.Message("Madagascar Vanilla: Unknown difficulty, skip trying to set a default.  This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            // Set commitment mode
            bool permadeath = MadagascarVanillaMod.Persistables.Permadeath;
            if (MadagascarVanillaMod.Verbose())
                Log.Message($"GameInitData set commitment mode to: {permadeath}");
            
            Find.GameInitData.permadeath = permadeath;
            Find.GameInitData.permadeathChosen = true;

            StorytellerSettingsLoaded = true;
        }

        // Persist the bits of the Storyteller that we need for setup:
        // - storytellerDef
        // - difficultyDef/difficulty
        // - commitment mode
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch("CanDoNext")]
        public static void Postfix(Page_SelectStoryteller __instance, bool __result)
        {
            bool persistNewGameSetup = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistNewGameSetupKey));
            if (persistNewGameSetup && __result)
            { 
                //Log.Message("persistNewGameSetup true, __result true: persist storyteller settings");
                
                Traverse traverse = Traverse.Create(__instance);
                
                // Persist new game settings
                MadagascarVanillaMod.Persistables.StorytellerDef = traverse.Field("storyteller").GetValue<StorytellerDef>();
                MadagascarVanillaMod.Persistables.DifficultyDef = traverse.Field("difficulty").GetValue<DifficultyDef>();
                MadagascarVanillaMod.Persistables.Difficulty = traverse.Field("difficultyValues").GetValue<Difficulty>();
                MadagascarVanillaMod.Persistables.Permadeath = Find.GameInitData.permadeath;
                
                MadagascarVanillaMod.Instance.WriteSettings();
            }
        }
        
        // Set defaults from mod settings for:
        //  - planet coverage
        //  - rainfall
        //  - temperature
        //  - population density
        //  - landmark density
        //  - factions
        //  - pollution
        [HarmonyPatch(typeof(Page_CreateWorldParams))]
        [HarmonyPatch(nameof(Page_CreateWorldParams.PreOpen))]
        public static void Postfix(Page_CreateWorldParams __instance)
        {
            if (__instance == null)
                return;
            
            if (WorldGeneratorSettingsLoaded)
                return;
            
            Traverse traverse = Traverse.Create(__instance);
            
            List<FactionDef> factions = MadagascarVanillaMod.Persistables.Factions;
            List<FactionDef> validFactions = new List<FactionDef>();
            if (factions != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set factions:");
                foreach (FactionDef faction in factions)
                {
                    if (MadagascarVanillaMod.Verbose())
                        Log.Message($"{faction.defName}");
                    if (!FactionGenerator.ConfigurableFactions.Contains(faction))
                    {
                        Log.Message($"Madagascar Vanilla: Unknown faction ({faction.defName})), skipping.");
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
            
            // Set PlanetCoverage
            float? planetCoverage = MadagascarVanillaMod.Persistables.PlanetCoverage;
            if (planetCoverage != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set planetCoverage: {planetCoverage}");
                traverse.Field("planetCoverage").SetValue(planetCoverage);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no planetCoverage value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            OverallRainfall? rainfall = MadagascarVanillaMod.Persistables.Rainfall;
            if (rainfall != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set rainfall: {rainfall}");
                traverse.Field("rainfall").SetValue(rainfall);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no rainfall value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            OverallTemperature? temperature = MadagascarVanillaMod.Persistables.Temperature;
            if (temperature != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set temperature: {temperature}");
                traverse.Field("temperature").SetValue(temperature);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no temperature value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            OverallPopulation? population = MadagascarVanillaMod.Persistables.Population;
            if (population != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set population: {population}");
                traverse.Field("population").SetValue(population);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no population value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            LandmarkDensity? landmarkDensity = MadagascarVanillaMod.Persistables.LandmarkDensity;
            if (landmarkDensity != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set landmarkDensity: {landmarkDensity}");
                traverse.Field("landmarkDensity").SetValue(landmarkDensity);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no landmarkDensity value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            float? pollution = MadagascarVanillaMod.Persistables.Pollution;
            if (pollution != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set pollution: {pollution}");
                traverse.Field("pollution").SetValue(pollution);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no pollution value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            int? mapSize = MadagascarVanillaMod.Persistables.MapSize;
            if (mapSize != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set mapSize: {mapSize}");
                Find.GameInitData.mapSize = (int) mapSize;
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no mapSize value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            Season? season = MadagascarVanillaMod.Persistables.StartingSeason;
            if (season != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_CreateWorldParams patch set season: {season}");
                Find.GameInitData.startingSeason = (Season) season;
            }
            else
            {
                Log.Message($"Madagascar Vanilla: no season value, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }

            WorldGeneratorSettingsLoaded = true;
        }
        
        [HarmonyPatch(typeof(Page_CreateWorldParams))]
        [HarmonyPatch("CanDoNext")]
        public static void Postfix(Page_CreateWorldParams __instance, bool __result)
        {
            Log.Message("Page_CreateWorldParams.CanDoNext");
            bool persistNewGameSetup = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistNewGameSetupKey));
            if (persistNewGameSetup) // && __result) // seems like this method always returns false, after queing up the world gen.
            { 
                Log.Message("persistNewGameSetup true, __result true: persist world settings");
                
                Traverse traverse = Traverse.Create(__instance);
                
                MadagascarVanillaMod.Persistables.Factions = traverse.Field("factions").GetValue<List<FactionDef>>();
                
                MadagascarVanillaMod.Persistables.PlanetCoverage = traverse.Field("planetCoverage").GetValue<float>();
                MadagascarVanillaMod.Persistables.Rainfall = traverse.Field("rainfall").GetValue<OverallRainfall>();
                MadagascarVanillaMod.Persistables.Temperature = traverse.Field("temperature").GetValue<OverallTemperature>();
                MadagascarVanillaMod.Persistables.Population = traverse.Field("population").GetValue<OverallPopulation>();
                MadagascarVanillaMod.Persistables.LandmarkDensity = traverse.Field("landmarkDensity").GetValue<LandmarkDensity>();
                MadagascarVanillaMod.Persistables.Pollution = traverse.Field("pollution").GetValue<float>();
                
                MadagascarVanillaMod.Persistables.MapSize = Find.GameInitData.mapSize;
                MadagascarVanillaMod.Persistables.StartingSeason = Find.GameInitData.startingSeason;
                
                
                Log.Message("Write settings");
                MadagascarVanillaMod.Instance.WriteSettings();
            }
        }
        
        // Set defaults from mod settings for:
        // - Preset Selection (type of ideoligion)
        // - Ideoligion (if preset is "Preset", otherwise null)
        // - Structure
        // - Styles
        // [HarmonyPatch(typeof(Page_ChooseIdeoPreset))]
        // [HarmonyPatch(nameof(Page_ChooseIdeoPreset.PostOpen))]
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch(nameof(Window.PreOpen))]
        public static void Postfix(Window __instance)
        {
            if (!(__instance is Page_ChooseIdeoPreset))
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Page_ChooseIdeoPreset patch");
            
            if (IdeoligionSettingsLoaded)
                return;

            Traverse traverse = Traverse.Create((Page_ChooseIdeoPreset) __instance);
            
            // Set presetSelection and selectedIdeo
            PresetSelectionType? presetSelection = MadagascarVanillaMod.Persistables.PresetSelection;
            if (presetSelection != null)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_ChooseIdeoPreset patch set preset selection: {presetSelection}");

                // Grab the "real" PresetSelection private enum
                Type presetSelectionEnum = typeof(Page_ChooseIdeoPreset).GetNestedType("PresetSelection", BindingFlags.NonPublic);

                Log.Message($"Found nested type {presetSelectionEnum}");
                Log.Message($"Get PresetSelectionType with name {presetSelection.ToString()}");
                
                object presetSelectionPrivateEnumValue = null;
                var enumValueArray = Enum.GetValues(presetSelectionEnum);
                foreach (var enumValue in enumValueArray)
                {
                    Log.Message($"Checking {enumValue.GetType()} {enumValue} against {presetSelection}");
                    if (enumValue.ToString() == presetSelection.ToString())
                    {
                        presetSelectionPrivateEnumValue = enumValue;
                        break;
                    }
                }
                Log.Message($"Got PresetSelection: {presetSelectionPrivateEnumValue}");
                
                if (presetSelectionPrivateEnumValue != null)
                {
                    // If a Preset Ideoligion is selected, assign that to selectedIdeo. Otherwise, set the selectedIdeo to null.
                    if (presetSelection == PresetSelectionType.Preset)
                    {
                        Log.Message("Page_ChooseIdeoPreset: we have a Preset ideoligion, should set it.");
                        // Set the selected preset ideoligion
                        IdeoPresetDef ideoligion = MadagascarVanillaMod.Persistables.Ideoligion;
                        if (ideoligion != null && DefDatabase<IdeoPresetDef>.AllDefsListForReading.Contains(ideoligion))
                        {
                            if (MadagascarVanillaMod.Verbose())
                                Log.Message($"Page_ChooseIdeoPreset patch set selectedIdeo: {ideoligion.defName}");
                            traverse.Field("selectedIdeo").SetValue(ideoligion);
                        }
                        else
                        {
                            Log.Message($"Madagascar Vanilla: Unknown ideoligion ({ideoligion}), skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
                        }
                    }
                    else
                    {
                        // Set the preset ideoligion to null
                        Log.Message("Set the selectedIdeo to null as we don't have a Preset.");
                        traverse.Field("selectedIdeo").SetValue(null);
                    }
                    
                    // Assign the PresetSelection into the presetSelection private field
                    Log.Message($"setting newPresetSelection: {presetSelectionPrivateEnumValue}");
                    traverse.Field("presetSelection").SetValue(presetSelectionPrivateEnumValue);
                }
            }
            else
            {
                Log.Message($"Madagascar Vanilla: Unknown preset selection ({presetSelection}), skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            // Set Structure
            MemeDef structure = MadagascarVanillaMod.Persistables.Structure;
            if (structure != null && DefDatabase<MemeDef>.AllDefsListForReading.Contains(structure))
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"Page_ChooseIdeoPreset patch set structure: {structure.defName}");
                traverse.Field("selectedStructure").SetValue(structure);
            }
            else
            {
                Log.Message($"Madagascar Vanilla: Unknown structure ({structure}), skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }
            
            // Set Styles
            List<StyleCategoryDef> styleCategories = MadagascarVanillaMod.Persistables.StyleCategories;
            if (styleCategories != null && styleCategories.Count > 0)
            {
                Log.Message($"stylecategories not null and count {styleCategories.Count}");
                
                List<StyleCategoryDef> validStyleCategories = new List<StyleCategoryDef>();
                foreach (StyleCategoryDef styleCategoryDef in styleCategories)
                {
                    if (styleCategoryDef == null)
                    {
                        Log.Message("Null style category detected");
                        continue;
                    }

                    Log.Message($"Processing style category def: {styleCategoryDef}");
                    if (DefDatabase<StyleCategoryDef>.AllDefsListForReading.Contains(styleCategoryDef))
                    {
                         if (MadagascarVanillaMod.Verbose())
                            Log.Message($"Page_ChooseIdeoPreset patch set style category: {styleCategoryDef.defName}");
                         validStyleCategories.Add(styleCategoryDef);
                    }
                    else
                    { 
                        Log.Message($"Madagascar Vanilla: Unknown style category ({styleCategoryDef.defName}), skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
                    }
                }
                
                Log.Message("Setting selected styles to validStyleCategories.");
                traverse.Field("selectedStyles").SetValue(validStyleCategories);
                Log.Message("Recaching style categories.");
                RecacheStyleCategoriesWithPriority((Page_ChooseIdeoPreset) __instance);
            }
            else
            { 
                Log.Message($"Madagascar Vanilla: Style categories is null, skip trying to set a default. This is expected on first new game after enabling persistant storyteller settings.");
            }

            IdeoligionSettingsLoaded = true;
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
        

        // Persist the bits of the Ideoligion that we need for setup:
        // - Preset Selection (type of ideoligion)
        // - Ideoligion (if preset is "Preset", otherwise null)
        // - Structure
        // - Styles
        // TODO: check if ideoligion is loaded.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Page_ChooseIdeoPreset))]
        [HarmonyPatch("DoNext")]
        public static void DoNextPostfix(Page_ChooseIdeoPreset __instance)
        {
            bool persistNewGameSetup = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistNewGameSetupKey));
            if (persistNewGameSetup)
            { 
                //Log.Message("persistNewGameSetup true, __result true: persist ideoligion settings");
                
                Traverse traverse = Traverse.Create(__instance);
                
                //Log.Message($"Persist Preset selection: {traverse.Field("presetSelection").GetValue<PresetSelectionType>()}");
                MadagascarVanillaMod.Persistables.PresetSelection = traverse.Field("presetSelection").GetValue<PresetSelectionType>();
                 
                //Log.Message($"Persist ideoligion selection: {traverse.Field("selectedIdeo").GetValue<IdeoPresetDef>()}");
                MadagascarVanillaMod.Persistables.Ideoligion = traverse.Field("selectedIdeo").GetValue<IdeoPresetDef>();
                
                //Log.Message($"Persist structure: {traverse.Field("selectedStructure").GetValue<MemeDef>()}");
                MadagascarVanillaMod.Persistables.Structure = traverse.Field("selectedStructure").GetValue<MemeDef>();

                MadagascarVanillaMod.Persistables.StyleCategories = traverse.Field("selectedStyles").GetValue<List<StyleCategoryDef>>();

                //Log.Message("Write settings");
                MadagascarVanillaMod.Instance.WriteSettings();
            }
        }
    }
}