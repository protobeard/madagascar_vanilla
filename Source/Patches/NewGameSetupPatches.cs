using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    public enum CommitmentModeType
    {
        Commitment,
        ReloadAnytime
    }
    
    [HarmonyPatch]
    public static class NewGameSetupPatches
    {
        private const string CommitmentMode = "commitmentMode";
        private const string SelectedStoryteller = "selectedStoryteller";
        private const string SelectedDifficulty = "selectedDifficulty";
        
        [HarmonyPatch(typeof(Page_SelectStoryteller))]
        [HarmonyPatch(nameof(Page_SelectStoryteller.PreOpen))]
        public static void Postfix(Page_SelectStoryteller __instance)
        {
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Page_SelectStoryteller patch");
            
            bool selectedStorytellerSettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, SelectedStoryteller, out string selectedStorytellerName);
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
            
            bool selectedDifficultySettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, SelectedDifficulty, out string selectedDifficultyName);
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
            
            bool commitmentModeSettingExists = SettingsManager.TryGetSetting(MadagascarVanillaMod.ModId, CommitmentMode, out string commitmentModeName);
            if (commitmentModeSettingExists && Enum.TryParse(commitmentModeName, false, out CommitmentModeType commitmentMode))
            {
                if (MadagascarVanillaMod.Verbose())
                    Log.Message($"GameInitData set commitment mode to: {CommitmentModeType.Commitment == commitmentMode}");
                
                Find.GameInitData.permadeath  = CommitmentModeType.Commitment == commitmentMode;
                Find.GameInitData.permadeathChosen = true;
            }
        }
    }
}
