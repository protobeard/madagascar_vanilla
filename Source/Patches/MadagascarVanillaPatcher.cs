using HarmonyLib;
using Verse;

namespace MadagascarVanilla.Patches
{
    
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("com.protobeard.madagascar_vanilla");
            Harmony.DEBUG = false;
            harmony.PatchAll();
            Log.Message("Initializing Madagascar Vanilla");
        }
    }
}