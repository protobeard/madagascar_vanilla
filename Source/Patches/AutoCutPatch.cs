using System;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class AutoCutPatch
    {
        private const string EnableAutoCut = "enableAutoCut";
        
        // Enable auto-cut on all things with CompAutoCuts:
        // Wind Turbine and Animal Pens
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CompAutoCut))]
        [HarmonyPatch(nameof(CompAutoCut.PostSpawnSetup))]
        public static void CompAutoCutPostfix(CompAutoCut __instance)
        {
            bool enableAutoCut = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableAutoCut));
            __instance.autoCut = enableAutoCut;

            // If we're looking at a Wind Turbine we can force an immediate cut rather than waiting
            // for the next long tick.
            if (__instance.autoCut && __instance is CompAutoCutWindTurbine)
                __instance.DesignatePlantsToCut();
        }
        
        
        // Animals pens override CompAutoCut.PostSpawnSetup to do more setup. Since they invoke the base
        // method we still get the autocut setting from the CompAutoCut patch. But, we can't force
        // an immediate cut because the AnimalPenManager hasn't registered the new pen yet. It seems like
        // that should happen during the rest of the PostSpawnSetup so we could do another PostFix as below,
        // but even when I force a RebuildAllPens() I'm still not getting the new PenMarker -> PenState
        // set correctly in AnimalPenManager.penMarkers dictionary.
        //
        // However, just letting it sit until the next long tick works fine.
        //
        //
        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(CompAnimalPenMarker))]
        // [HarmonyPatch(nameof(CompAnimalPenMarker.PostSpawnSetup))]
        // public static void AnimalPenMarkerPostfix(CompAnimalPenMarker __instance)
        // {
        //     Log.Message("AnimalPenMarkerPostfix");
        //     bool enableAutoCut = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableAutoCut));
        //
        //     var manager = __instance.parent?.Map?.animalPenManager;
        //
        //     Log.Message($"parent: {__instance.parent}");
        //     Log.Message($"map: {__instance.parent?.Map}");
        //     Log.Message($"manager: {manager}");
        //
        //     manager?.RebuildAllPens();
        //     
        //     if (enableAutoCut && __instance.CanDesignatePlants)
        //         __instance.DesignatePlantsToCut();
        // }
    }
}