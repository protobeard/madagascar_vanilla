using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch]
    public static class PersistPolicies
    {
        // FIXME: move these into MadagascarVanillaMod
        private const string PersistApparelPolicies = "persistApparelPolicies";
        private const string PersistDrugPolicies = "persistDrugPolicies";
        private const string PersistFoodPolicies = "persistFoodPolicies";
        private const string PersistReadingPolicies = "persistReadingPolicies";
        
        [HarmonyPatch(typeof(Window))]
        [HarmonyPatch(nameof(Window.PostClose))]
        public static void Postfix(Window __instance)
        {
            switch (__instance)
            {
                // TODO: Might as well have the medical defaults covered here too. They aren't a "policy" but
                // we need to patch the same stuff (basically)/follow similar logic.
                // case Dialog_MedicalDefaults dialogMedicalDefaults:
                //     DialogMedicalDefaultsClosePostfix();
                //     break;
                case Dialog_ManagePolicies<ApparelPolicy> dialogManagePolicies:
                    DialogApparelPolicyPostfix(dialogManagePolicies);
                    break;
                case Dialog_ManagePolicies<DrugPolicy> dialogManagePolicies:
                    DialogDrugPolicyPostfix(dialogManagePolicies);
                    break;
                case Dialog_ManagePolicies<FoodPolicy> dialogManagePolicies:
                    DialogFoodPolicyPostfix(dialogManagePolicies);
                    break;
                case Dialog_ManagePolicies<ReadingPolicy> dialogManagePolicies:
                    DialogReadingPolicyPostfix(dialogManagePolicies);
                    break;
                default:
                    if (MadagascarVanillaMod.Verbose())
                        Log.Message("Not a policy dialog window.");
                    return;
            }
        }

        private static void DialogApparelPolicyPostfix(Window __instance)
        {
            bool persistApparelPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistApparelPolicies));
            if (!persistApparelPolicies)
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Save Apparel Polices into Persistables");   
            
            MadagascarVanillaMod.Persistables.ApparelPolicies.Clear();
            
            foreach (ApparelPolicy apparelPolicy in Current.Game.outfitDatabase.AllOutfits)
                MadagascarVanillaMod.Persistables.ApparelPolicies.Add(apparelPolicy);
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        private static void DialogDrugPolicyPostfix(Window __instance)
        {
            bool persistDrugPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistDrugPolicies));
            if (!persistDrugPolicies)
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Save Drug Polices into Persistables");
            
            MadagascarVanillaMod.Persistables.DrugPolicies.Clear();
            
            foreach (DrugPolicy drugPolicy in Current.Game.drugPolicyDatabase.AllPolicies)
                MadagascarVanillaMod.Persistables.DrugPolicies.Add(drugPolicy);
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        private static void DialogFoodPolicyPostfix(Window __instance)
        {
            bool persistFoodPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistFoodPolicies));
            if (!persistFoodPolicies)
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Save Food Policies into Persistables");
            
            MadagascarVanillaMod.Persistables.FoodPolicies.Clear();

            foreach (FoodPolicy foodPolicy in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
                MadagascarVanillaMod.Persistables.FoodPolicies.Add(foodPolicy);
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        private static void DialogReadingPolicyPostfix(Window __instance)
        {
            bool persistReadingPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistReadingPolicies));
            if (!persistReadingPolicies)
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Save Reading Policies into Persistables");
            
            MadagascarVanillaMod.Persistables.ReadingPolicies.Clear();

            foreach (ReadingPolicy readingPolicy in Current.Game.readingPolicyDatabase.AllReadingPolicies)
                MadagascarVanillaMod.Persistables.ReadingPolicies.Add(readingPolicy);
            
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
        // Note: first outfit in the OutfitDatabase list is the default, so I shouldn't need to do anything
        // special to preserve that.
        [HarmonyPatch(typeof(OutfitDatabase))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(OutfitDatabase __instance)
        {
            // Bail if persisting apparel policies is disabled, or there are no policies to load
            bool persistApparelPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistApparelPolicies));
            if (!persistApparelPolicies || MadagascarVanillaMod.Persistables.ApparelPolicies == null || !MadagascarVanillaMod.Persistables.ApparelPolicies.Any()) 
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Loading Apparel Policies into Database");
            
            __instance.AllOutfits.Clear();
            foreach (ApparelPolicy apparelPolicy in MadagascarVanillaMod.Persistables.ApparelPolicies)
                __instance.AllOutfits.Add(apparelPolicy);
        }
        
        // Note: first policy in the DrugDatabase list is the default, so I shouldn't need to do anything
        // special to preserve that.
        [HarmonyPatch(typeof(DrugPolicyDatabase))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(DrugPolicyDatabase __instance)
        {
            // Bail if persisting drug policies is disabled, or there are no policies to load
            bool persistDrugPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistDrugPolicies));
            if (!persistDrugPolicies || MadagascarVanillaMod.Persistables.DrugPolicies == null || !MadagascarVanillaMod.Persistables.DrugPolicies.Any()) 
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Loading Drug Policies into Database");

            __instance.AllPolicies.Clear();
            foreach (DrugPolicy drugPolicy in MadagascarVanillaMod.Persistables.DrugPolicies)
                __instance.AllPolicies.Add(drugPolicy);
        }
        
        // Note: first policy in the FoodRestrictionDatabase list is the default, so I shouldn't need to do anything
        // special to preserve that.
        [HarmonyPatch(typeof(FoodRestrictionDatabase))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(FoodRestrictionDatabase __instance)
        {
            // Bail if persisting food policies is disabled, or there are no policies to load
            bool persistFoodPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistFoodPolicies));
            if (!persistFoodPolicies || MadagascarVanillaMod.Persistables.FoodPolicies == null || !MadagascarVanillaMod.Persistables.FoodPolicies.Any()) 
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Loading Food Policies into Database");
            
            __instance.AllFoodRestrictions.Clear();
            foreach (FoodPolicy foodPolicy in MadagascarVanillaMod.Persistables.FoodPolicies)
                __instance.AllFoodRestrictions.Add(foodPolicy);
        }
        
        // Note: first policy in the ReadingPolicyDatabase list is the default, so I shouldn't need to do anything
        // special to preserve that.
        [HarmonyPatch(typeof(ReadingPolicyDatabase))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(ReadingPolicyDatabase __instance)
        {
            // Bail if persisting reading policies is disabled, or there are no policies to load
            bool persistReadingPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistReadingPolicies));
            if (!persistReadingPolicies || MadagascarVanillaMod.Persistables.ReadingPolicies == null || !MadagascarVanillaMod.Persistables.ReadingPolicies.Any()) 
                return;
            
            if (MadagascarVanillaMod.Verbose())
                Log.Message("Loading Reading Policies into Database");
            
            __instance.AllReadingPolicies.Clear();
            foreach (ReadingPolicy readingPolicy in MadagascarVanillaMod.Persistables.ReadingPolicies)
                __instance.AllReadingPolicies.Add(readingPolicy);
        }
    }
}