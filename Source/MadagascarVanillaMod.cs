using System.Collections.Generic;
using MadagascarVanilla.Patches;
using Verse;
using UnityEngine;

namespace MadagascarVanilla
{
    public class MadagascarVanillaMod : Mod
    {
        public const string ModId = "com.protobeard.madagascarvanilla";
        
        // FIXME: pull all XML Extensions setting keys in here? Rename them all to include setting? or key?
        public const string VerboseSetting = "verboseMode";
        
        public override string SettingsCategory() => "Madagascar Vanilla";
        
        // We're using "settings" to mean things that we want to save to disk. For more traditional
        // settings we're using the XML Extensions mod's settings features.
        public static MadagascarVanillaPersistables Persistables;
        public static MadagascarVanillaMod Instance;
        
        //public static MadagascarVanillaSettings Persistables => persistables ??= Instance.GetSettings<MadagascarVanillaSettings>();
        
        public MadagascarVanillaMod(ModContentPack content) : base(content) {
            Instance = this;
            Persistables = GetSettings<MadagascarVanillaPersistables>();
        }
    }
    public class MadagascarVanillaPersistables : ModSettings
    {
        public Dictionary<ExportType, ExposableList<ExposableList<IExposable>>> PolicyDictionary = new Dictionary<ExportType, ExposableList<ExposableList<IExposable>>>();
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(dict: ref PolicyDictionary, label: "persistablePolicies", valueLookMode: LookMode.Deep);
        }
    }

}