using RimWorld;
using HarmonyLib;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch(nameof(Pawn_GuestTracker.SetGuestStatus))]
    public static class AutoStripPatch
    {
        private const string EnableAutoStripKey = "enableAutoStrip";
        private const string EnableAutoStripArrestedColonistKey = "enableAutoStripArrestedColonist";
        
        // Automatically strip prisoners when captured. Also optionally auto strip colonists when
        // arrested.
        public static void Postfix(Pawn_GuestTracker __instance, Pawn ___pawn)
        {
            bool enableAutoStrip = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableAutoStripKey));
            bool enableAutoStripArrestedColonist = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableAutoStripArrestedColonistKey));
            
            // Bail if we're not dealing with a prisoner
            if (!enableAutoStrip || !___pawn.IsPrisonerOfColony)
                return;

            // Bail if the prisoner is a colonist unless the setting to strip them too is enabled
            if (!enableAutoStripArrestedColonist && ___pawn.IsColonist)
                return;
            
            Designation stripPawnDesignation = new Designation(___pawn, DesignationDefOf.Strip);

            if (___pawn.Map != null)
            {
                ___pawn.Map.designationManager.AddDesignation(stripPawnDesignation);
                return;
            }
            
            // Get the map from the Pawn's parent object and then add the strip designation
            ___pawn.MapHeld?.designationManager.AddDesignation(stripPawnDesignation);
        }
    }
}