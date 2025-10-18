using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class NewGameSetupPatches
    {
        private const string PersistNewGameSetupKey = "persistNewGameSetup";
        
        private const string CommitmentModeKey = "commitmentMode";
        private const string SelectedStorytellerKey = "selectedStoryteller";
        private const string SelectedDifficultyKey = "selectedDifficulty";
        
        private const string CommitmentMode = "CommitmentMode";
        
        // Track whether we've loaded our settings since the user can go forward and back through
        // the new game setup UI, and we don't want to overwrite any changes they may have made
        // on each page.
        private static bool StorytellerSettingsLoaded;
        private static bool WorldGeneratorSettingsLoaded;

        [HarmonyPatch(typeof(Page))]
        [HarmonyPatch("DoBack")]
        public static void Postfix(Page __instance)
        {
            if (__instance is Page_SelectStoryteller)
                StorytellerSettingsLoaded = false;
            else if (__instance is Page_CreateWorldParams)
                WorldGeneratorSettingsLoaded = false;
        }
        
        // Set defaults from mod settings for:
        //  - Storyteller
        //  - DifficultyDef/Difficulty
        //  - Commitment Mode
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch(nameof(Page_SelectStoryteller.PreOpen))]
        public static void Postfix(Page_SelectStoryteller __instance)
        {
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Page_SelectStoryteller patch");
            
            if (StorytellerSettingsLoaded)
                return;

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
                //Log.Message("Persist storyteller");
                MadagascarVanillaMod.Persistables.StorytellerDef = (StorytellerDef) traverse.Field("storyteller").GetValue<StorytellerDef>();
                
                //Log.Message("Persist DifficultyDef");
                MadagascarVanillaMod.Persistables.DifficultyDef = traverse.Field("difficulty").GetValue<DifficultyDef>();
                
                //Log.Message("Persist difficulty:");
                //Log.Message(traverse.Field("difficultyValues").GetValue<Difficulty>().DebugString());
                MadagascarVanillaMod.Persistables.Difficulty = (Difficulty) traverse.Field("difficultyValues").GetValue<Difficulty>();

                //Log.Message("Persist permadeath");
                MadagascarVanillaMod.Persistables.Permadeath = Find.GameInitData.permadeath;
                
               //Log.Message("Write settings");
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
    }
}
