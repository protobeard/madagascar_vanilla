using Verse;
using UnityEngine;

namespace MadagascarVanilla
{
    public class MadagascarVanillaMod : Mod
    {
        public const string modId = "protobeard.madagascarvanilla";
        public static MadagascarVanillaSettings settings;
        
        public MadagascarVanillaMod(ModContentPack content) : base(content) {
            settings = GetSettings<MadagascarVanillaSettings>();
        }

        public override string SettingsCategory()
        {
            return "MadagascarVanilla.Settings".Translate();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            // FIXME: Just get rid of this in favor of XML Extensions settings?
            // Doesn't really seem like two types of settings make much sense, esp. since I don't have any yet.
        }
    }
}