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
    // FIXME: give this a better name, maybe just PolicyType?
    public enum ExportType : byte
    {
        ApparelPolicy,
        DrugPolicy,
        FoodPolicy,
        ReadingPolicy,
    }
    
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
            Log.Message("Window.PostClose");
            
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
                    Log.Message("Not a policy dialog window.");
                    return;
            }
        }

        private static void DialogApparelPolicyPostfix(Window __instance)
        {
            bool persistApparelPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistApparelPolicies));
            if (!persistApparelPolicies)
                return;
            
            // Overwrite whatever we have in our persistables with the current outfit database
            if (MadagascarVanillaMod.Persistables.PolicyDictionary.TryGetValue(ExportType.ApparelPolicy, out ExposableList<ExposableList<IExposable>> apparelPolicy))
                apparelPolicy.Clear();

            foreach (ApparelPolicy outfit in Current.Game.outfitDatabase.AllOutfits) 
                PersistPolicy(ExportType.ApparelPolicy, new IExposable[] { outfit.filter }, outfit.label);
        }
        
        private static void DialogDrugPolicyPostfix(Window __instance)
        {
            return;
        }
        
        private static void DialogFoodPolicyPostfix(Window __instance)
        {
            return;
        }
        
        private static void DialogReadingPolicyPostfix(Window __instance)
        {
            return;
        }
        
        
        [HarmonyPatch(typeof(OutfitDatabase))]
        [HarmonyPatch(MethodType.Constructor)]
        public static void Postfix(OutfitDatabase __instance)
        {
            // Bail if persisting apparel policies is disabled, or there are no policies to persist
            bool persistApparelPolicies = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, PersistApparelPolicies));
            if (!persistApparelPolicies || !MadagascarVanillaMod.Persistables.PolicyDictionary.ContainsKey(ExportType.ApparelPolicy)) 
                return;
            
            __instance.AllOutfits.Clear();
            foreach (ExposableList<IExposable> li in MadagascarVanillaMod.Persistables.PolicyDictionary[ExportType.ApparelPolicy])
            {
                ApparelPolicy outfit = __instance.MakeNewOutfit();
                outfit.filter = (ThingFilter) li.First().exposable;
                outfit.label  = li.Name;
            }
        }
        
        private static void PersistPolicy(ExportType key, IEnumerable<IExposable> list, string name)
        {
            IEnumerable<IExposable> exposables = list as IExposable[] ?? list.ToArray();
            if (!exposables.Any()) 
                return;

            if (!MadagascarVanillaMod.Persistables.PolicyDictionary.ContainsKey(key))
                MadagascarVanillaMod.Persistables.PolicyDictionary.Add(key, new ExposableList<ExposableList<IExposable>>());

            MadagascarVanillaMod.Persistables.PolicyDictionary[key].Add(item: new ExposableList<IExposable>(exposables) { Name = name });
            MadagascarVanillaMod.Instance.WriteSettings();
        }
        
    }
    
}