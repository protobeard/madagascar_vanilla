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
        private const string CommitmentModeKey = "commitmentMode";
        private const string SelectedStorytellerKey = "selectedStoryteller";
        private const string SelectedDifficultyKey = "selectedDifficulty";
        
        private const string CommitmentMode = "CommitmentMode";
        
        // Track whether we've loaded our settings since the user can go forward and back through
        // the new game setup UI, and we don't want to overwrite any changes they may have made
        // on each page.
        private static bool StorytellerSettingsLoaded;
        private static bool WorldGeneratorSettingsLoaded;
        
        // Set defaults from mod settings for:
        //  - Storyteller
        //  - Difficulty
        //  - Commitment Mode
        //
        // TODO: is there some way we can just persist these like policies instead of having settings for them.
        // seems like especially for difficulty they aren't going to change a lot. Just using what you used last game
        // makes a lot of sense.
        // Would also need to patch the in game difficulty change UI as well. Probably just need to tie into the "Next" button
        // on the Page.
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch(nameof(Page_SelectStoryteller.PreOpen))]
        public static void Postfix(Page_SelectStoryteller __instance)
        {
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Page_SelectStoryteller patch");
            
            if (StorytellerSettingsLoaded)
                return;
            
            bool selectedStorytellerSettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, SelectedStorytellerKey, out string selectedStorytellerName);
            if (selectedStorytellerSettingExists)
            {
                StorytellerDef storytellerDef = DefDatabase<StorytellerDef>.GetNamed(selectedStorytellerName);
                if (storytellerDef != null)
                {
                    if (MadagascarVanillaMod.Verbose())
                        Log.Message($"Page_SelectStoryteller patch set storyteller: {storytellerDef.defName}");
                    Traverse.Create(__instance).Field("storyteller").SetValue(storytellerDef);
                }
                else
                {
                    Log.Error("Madagascar Vanilla: Unknown storyteller, skip trying to set a default.");
                }
            }
            
            bool selectedDifficultySettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, SelectedDifficultyKey, out string selectedDifficultyName);
            if (selectedDifficultySettingExists)
            {
                DifficultyDef difficultyDef = DefDatabase<DifficultyDef>.GetNamed(selectedDifficultyName);
                if (difficultyDef != null)
                {
                    if (MadagascarVanillaMod.Verbose())
                        Log.Message($"Page_SelectStoryteller patch set difficulty: {difficultyDef.defName}");
                    Traverse.Create(__instance).Field("difficulty").SetValue(difficultyDef);
                    Traverse.Create(__instance).Field("difficultyValues").SetValue(new Difficulty(difficultyDef));
                }
                else
                { 
                    Log.Error("Madagascar Vanilla: Unknown difficulty, skip trying to set a default.");
                }
            }
            
            bool commitmentModeSettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, CommitmentModeKey, out string commitmentModeName);
            if (commitmentModeSettingExists)
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"GameInitData set commitment mode to: {commitmentModeName == CommitmentMode}");
                
                Find.GameInitData.permadeath = commitmentModeName == CommitmentMode;
                Find.GameInitData.permadeathChosen = true;
            }

            StorytellerSettingsLoaded = true;
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
