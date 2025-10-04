using HarmonyLib;
using Verse;

namespace MadagascarVanilla.Patches
{
    
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony(MadagascarVanillaMod.modId);
            Harmony.DEBUG = false;
            harmony.PatchAll();
            Log.Message("Initializing Madagascar Vanilla");
        }
    }
}