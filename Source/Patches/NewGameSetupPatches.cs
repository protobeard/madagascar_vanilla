using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
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
        //  - Difficulty
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
        // - difficultyDef
        // - difficulty (if custom)
        // - Anomaly settings (if Anomaly active and difficulty isn't custom (b/c if it is custom this gets saved there instead))
        // - commitment mode
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch("CanDoNext")]
        public static void Postfix(Page_SelectStoryteller __instance, bool __result)
        {
            bool persistNewGameSetup = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistNewGameSetupKey));
            if (persistNewGameSetup && __result)
            { 
                Log.Message("persistNewGameSetup true, __result true: persist storyteller settings");
                
                Traverse traverse = Traverse.Create(__instance);
                
                // Persist new game settings
                Log.Message("Persist storyteller");
                MadagascarVanillaMod.Persistables.StorytellerDef = (StorytellerDef) traverse.Field("storyteller").GetValue<StorytellerDef>();
                
                Log.Message("Persist DifficultyDef");
                MadagascarVanillaMod.Persistables.DifficultyDef = traverse.Field("difficulty").GetValue<DifficultyDef>();
                
                Log.Message("Persist difficulty:");
                //Log.Message(traverse.Field("difficultyValues").GetValue<Difficulty>().DebugString());
                MadagascarVanillaMod.Persistables.Difficulty = (Difficulty) traverse.Field("difficultyValues").GetValue<Difficulty>();

                Log.Message("Persist permadeath");
                MadagascarVanillaMod.Persistables.Permadeath = Find.GameInitData.permadeath;
                
                Log.Message("Write settings");
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
        // TODO: is there some way we can just persist these like policies instead of having settings for them.
        // tie into the "generate" button on the Page (CanDoNext method?)
        [HarmonyPatch(typeof(Page_CreateWorldParams))]
        [HarmonyPatch(nameof(Page_CreateWorldParams.PreOpen))]
        public static void Postfix(Page_CreateWorldParams __instance)
        {
            if (WorldGeneratorSettingsLoaded)
                return;
            
            Traverse traverse = Traverse.Create(__instance);

            WorldGeneratorSettingsLoaded = true;
        }
    }
}
