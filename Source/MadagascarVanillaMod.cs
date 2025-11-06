using HarmonyLib;
using MadagascarVanilla.Settings;
using UnityEngine;
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
            //MadagascarVanillaMod.Persistables.GetHashCode();
        }
    }
    
    public class MadagascarVanillaMod : Mod
    {
        public const string ModId = "com.protobeard.madagascarvanilla";
        
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