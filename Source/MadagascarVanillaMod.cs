using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using XmlExtensions;

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
        public const string VerboseSettingKey = "verboseMode";
        
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

        public static bool Verbose()
        {
            return bool.Parse(SettingsManager.GetSetting(ModId, VerboseSettingKey));
        }
    }
    public class MadagascarVanillaPersistables : ModSettings
    {
        // New Game Setup
        private StorytellerDef _storyteller;
        private DifficultyDef _difficultyDef;
        private Difficulty _difficulty;
        
        public StorytellerDef StorytellerDef { get => _storyteller; set => _storyteller = value; }
        public DifficultyDef DifficultyDef { get => _difficultyDef; set => _difficultyDef = value; }
        public Difficulty Difficulty { get => _difficulty ??= new Difficulty(); set => _difficulty = value; }
        public bool Permadeath;

        // Policies
        private List<ApparelPolicy> _apparelPolicies;
        private List<DrugPolicy> _drugPolicies;
        private List<FoodPolicy> _foodPolicies;
        private List<ReadingPolicy> _readingPolicies;
        
        public List<ApparelPolicy> ApparelPolicies => _apparelPolicies ??= new List<ApparelPolicy>();
        public List<DrugPolicy> DrugPolicies => _drugPolicies ??= new List<DrugPolicy>();
        public List<FoodPolicy> FoodPolicies => _foodPolicies ??= new List<FoodPolicy>();
        public List<ReadingPolicy> ReadingPolicies => _readingPolicies ??= new List<ReadingPolicy>();
        
        public override void ExposeData()
        {
            base.ExposeData();
            
            // Persist New Game Setup
            Scribe_Defs.Look(ref _storyteller, "storyteller");
            Scribe_Defs.Look(ref _difficultyDef, "difficultyDef");
            Scribe_Deep.Look(ref _difficulty, "difficulty");
            Scribe_Values.Look(ref Permadeath, "permadeath");
                
            // Persist Policies
            Scribe_Collections.Look(ref _apparelPolicies, "apparelPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref _drugPolicies, "drugPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref _foodPolicies, "foodPolicies", LookMode.Deep);
            Scribe_Collections.Look(ref _readingPolicies, "readingPolicies", LookMode.Deep);
        }
    }

}