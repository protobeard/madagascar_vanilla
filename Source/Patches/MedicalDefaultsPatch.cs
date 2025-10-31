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
        // I'd love to patch something more specific than Window, but since PostClose is a virtual
        // method and not overriden in Dialog_MedicalDefaults Harmony won't patch it directly
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch(nameof(Window.PostClose))]
        public static void DialogMedicalDefaultsPostClosePatchPostfix(Window __instance)
        {
            if (!(__instance is Dialog_MedicalDefaults))
                return;
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            PlaySettings playSettings = Find.PlaySettings;
            if (persistMedicalSettings && playSettings != null)
                MedicalDefaults.PersistMedicalSettings(playSettings);
        }
        
        // Load mod medical settings into Playsettings before opening the medical defaults dialog window.
        // I'd love to patch something more specific than Window, but since PreOpen is a virtual
        // method and not overriden in Dialog_MedicalDefaults Harmony won't patch it directly
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch(nameof(Window.PreOpen))]
        public static void DialogMedicalDefaultsPreOpenPatchPostfix(Window __instance)
        {
            if (!(__instance is Dialog_MedicalDefaults))
                return;
            
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            PlaySettings playSettings = Find.PlaySettings;
            if (persistMedicalSettings && playSettings != null)
                MedicalDefaults.LoadMedicalSettingsIntoPlaySettings(playSettings);
        }
        
        // Load mod medical settings into Playsettings on game load (in case they have been modified outside the game).
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("LoadGame")]
        public static void LoadGamePostfix(Game __instance)
        {
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            if (persistMedicalSettings)
                MedicalDefaults.LoadMedicalSettingsIntoPlaySettings(__instance.playSettings);
        }
        
        // On New Game after PlaySetting instantiation pull the medical default settings from our mod settings
        // and configure the PlaySettings. InitNewGame postfix is too late (starting pawns already have medical settings set).
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlaySettings))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void PlaySettingsConstructorPostfix(PlaySettings __instance)
        {
            bool persistMedicalSettings = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, MedicalDefaults.PersistMedicalSettingsKey));
            if (persistMedicalSettings)
                MedicalDefaults.LoadMedicalSettingsIntoPlaySettings(__instance);
        }
    }
}