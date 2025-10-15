using HarmonyLib;
using Verse;

namespace MadagascarVanilla.Patches
{
    
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony(MadagascarVanillaMod.ModId);
            Harmony.DEBUG = false;
            harmony.PatchAll();
            Log.Message("Initializing Madagascar Vanilla");
            
            // FIXME: why do we need to do this? If I don't, the Persistables dictionary is always null
            MadagascarVanillaMod.Persistables.GetHashCode();
        }
    }
}