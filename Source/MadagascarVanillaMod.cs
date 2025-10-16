using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MadagascarVanilla
{
    
    [StaticConstructorOnStartup]
    public static class MadagascarVanillaPatches
    {
        static MadagascarVanillaPatches()
        {
            Harmony harmony = new Harmony(MadagascarVanillaMod.ModId);
            Harmony.DEBUG = false;
            harmony.PatchAll();
            Log.Message("Initializing Madagascar Vanilla");
            
            // Force RimWorld to load in our Persistables
            MadagascarVanillaMod.Persistables.GetHashCode();
        }
    }
    
    public class MadagascarVanillaMod : Mod
    {
        public const string ModId = "com.protobeard.madagascarvanilla";
        
        // FIXME: pull all XML Extensions setting keys in here? Rename them all to include setting? or key?
        public const string VerboseSetting = "verboseMode";
        
        public override string SettingsCategory() => "Madagascar Vanilla";
        
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
        }
    }
    public class MadagascarVanillaPersistables : ModSettings
    {
        public List<ApparelPolicy> ApparelPolicies = new List<ApparelPolicy>();
        public List<DrugPolicy> DrugPolicies = new List<DrugPolicy>();
        public List<FoodPolicy> FoodPolicies = new List<FoodPolicy>();
        public List<ReadingPolicy> ReadingPolicies = new List<ReadingPolicy>();
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ApparelPolicies, "apparelPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref DrugPolicies, "drugPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref FoodPolicies, "foodPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref ReadingPolicies, "readingPolicies", LookMode.Deep);
        }
    }

}