using RimWorld;
using HarmonyLib;
using Verse;
using Verse.AI;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(JobGiver_OptimizeApparel))]
    [HarmonyPatch("TryGiveJob")]
    public static class DontRemoveApparelWhileBleeding
    {
        private const string DisableRemovingApparelWhileBleedingKey = "disableRemovingApparelWhileBleeding";
        
        public static bool Prefix(Pawn pawn, ref Job __result)
        {
            bool disableRemovingApparelWhileBleedingKey = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, DisableRemovingApparelWhileBleedingKey));
            if (!disableRemovingApparelWhileBleedingKey)
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