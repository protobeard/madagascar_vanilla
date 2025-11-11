using RimWorld;
using HarmonyLib;
using MadagascarVanilla.ClassExtensions;
using Verse;
using Verse.AI;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(JobGiver_OptimizeApparel))]
    [HarmonyPatch("TryGiveJob")]
    public static class DontRemoveApparelWhileBleeding
    {
        public static bool Prefix(Pawn pawn, ref Job __result)
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"TryGiveJobPatch");
            
            if (!MadagascarVanillaMod.Persistables.DisableRemovingApparelWhileBleeding)
                return true;
            
            if (MadagascarVanillaMod.Verbose()) Log.Message($"checking BleedRateTotal > 0 for {pawn.Name}");
            if (pawn.health.hediffSet.BleedRateTotal > 0f)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Don't optimize apparel on BleedRateTotal > 0");
                __result = null;
                return false;
            }
            return true;
        }
    }
}