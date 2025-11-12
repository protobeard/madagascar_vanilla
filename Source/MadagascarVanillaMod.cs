using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MadagascarVanilla.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace MadagascarVanilla
{
    
    [StaticConstructorOnStartup]
    public static class MadagascarVanillaPatches
    {
        static MadagascarVanillaPatches()
        {
            List<string> packageIds;
            Harmony harmony = new Harmony(MadagascarVanillaMod.ModId);
            Harmony.DEBUG = false;
            // Patch everything that isn't in a specific compatibility category
            harmony.PatchAllUncategorized();
            
            // !LoadedModManager.RunningMods.Select(mod => mod.PackageId).Contains("lecht.AutoRepairOn")
            //if (Type.GetType("AutoRepairOn.CompMechRepairableOn, AutoRepairOn") == null)
            if (MadagascarVanillaMod.Instance.CompatibilityManager.Check("EnableMechRepair", out packageIds))
            {
                foreach (string packageId in packageIds)
                {
                    Log.Message($"{packageId} detected: skipping Madagascar Vanilla's AutoRepair patch category");
                }
            }
            else
            {
                harmony.PatchCategory("AutoRepairMechs");
            }
            
            if (MadagascarVanillaMod.Instance.CompatibilityManager.Check("EnableNightOwlSchedule", out packageIds))
            {
                foreach (string packageId in packageIds)
                {
                    Log.Message($"{packageId} detected: Removing patches that conflict with EnableNightOwlSchedule setting.");
                    
                    if (packageId == ("Mlie.XNDTinyTweaks"))
                    {
                        // Unpatch the AutoOwl feature as ours in more flexible
                        MethodInfo originalSetFaction = typeof(Pawn).GetMethod("SetFaction");
                        MethodInfo originalSpawnSetup = typeof(Thing).GetMethod("SpawnSetup");
                        
                        harmony.Unpatch(originalSetFaction, HarmonyPatchType.Postfix, "XeoNovaDan.TinyTweaks");
                        harmony.Unpatch(originalSpawnSetup, HarmonyPatchType.Postfix, "XeoNovaDan.TinyTweaks");
                    }
                }
            }
            
            if (MadagascarVanillaMod.Instance.CompatibilityManager.Check("DisableLearningHelperButton", out packageIds))
            {
                foreach (string packageId in packageIds)
                {
                    Log.Message($"{packageId} detected: skipping Madagascar Vanilla's LearningHelper patch category");
                }
            }
            else
            {
                harmony.PatchCategory("LearningHelper");
            }
            
            
        }
    }
    
    public class MadagascarVanillaMod : Mod
    {
        public const string ModId = "com.protobeard.madagascarvanilla";
        private ModCompatibilityManager _modCompatibilityManager;
        public ModCompatibilityManager CompatibilityManager => _modCompatibilityManager;
        
        // We're using "settings" to mean things that we want to save to disk. For more traditional
        // settings we're using the XML Extensions mod's settings features.
        private static MadagascarVanillaPersistables _persistables;
        public static MadagascarVanillaMod Instance;
        
        // If we instantiate persistables in the constructor, it's too soon in the RimWorld load process
        // for all the various refs to exist, and so we end up with empty Apparel Policies etc.
        // Instead, lazy load it.
        public static MadagascarVanillaPersistables Persistables => _persistables ??= Instance.GetSettings<MadagascarVanillaPersistables>();
        
        public MadagascarVanillaMod(ModContentPack content) : base(content) {
            Instance = this;
            _modCompatibilityManager = new ModCompatibilityManager();
            _modCompatibilityManager.Add("lecht.AutoRepairOn", "EnableMechRepair", () => ModsConfig.IsActive("lecht.AutoRepairOn"));
            _modCompatibilityManager.Add("Mlie.XNDTinyTweaks", "EnableNightOwlSchedule", () => ModsConfig.IsActive("Mlie.XNDTinyTweaks"));
            _modCompatibilityManager.Add("MemeGoddess.TDPack", "DisableLearningHelperButton", () => ModsConfig.IsActive("MemeGoddess.TDPack"));
        }
        
        public override string SettingsCategory()
        {
            return "MV_MadagascarVanilla".Translate();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Persistables.DoSettingsWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public static bool Verbose()
        {
            return Persistables.Verbose;
        }
    }
    
}