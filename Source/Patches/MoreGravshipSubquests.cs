using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(QuestPart_SubquestGenerator_Gravcores))]
    [HarmonyPatch("CanGenerateSubquest")]
    [HarmonyPatch(MethodType.Getter)]
    public static class MoreGravshipSubquestsPatch
    {
        private const string AllowAdditionalGravshipSubquestsKey = "allowXAdditionalGravshipSubquests";
        
        // Recheck pendingSubQuestCount against the user specified maxAllowedGravshipSubquests
        // and modify the return value of CanGenerateSubquest if there are fewer subquests, enough
        // time has passed, and the player has a Gravship.
        //
        // Unfortunately this duplicates a bit of the logic in the original method, but I think it's
        // better to do a couple rechecks than a Prefix patch which completely duplicates the original code.
        public static void Postfix(QuestPart_SubquestGenerator_Gravcores __instance, float ___lastSubquestTick, ref bool __result)
        {
            int maxAllowedGravshipSubquests = int.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AllowAdditionalGravshipSubquestsKey));
            
            // If the original method returned false
            if (!__result)
            {
                Traverse traverse = Traverse.Create(__instance);
                int pendingSubquestCount = traverse.Property("PendingSubquestCount").GetValue<int>();
                int minTime = traverse.Property("MinTime").GetValue<int>();
                
                // If there are fewer pending subquests than our new max
                if (pendingSubquestCount < maxAllowedGravshipSubquests)
                {
                    // If not enough time has elapsed since the last quest was given
                    if (Find.TickManager.TicksGame - ___lastSubquestTick < minTime)
                        return;
                    
                    // If the player has a Grav Engine, and few pending subquests than our new max, return true.
                    foreach (Map map in Find.Maps)
                    {
                        if (map.listerBuildings.ColonistsHaveBuilding(ThingDefOf.GravEngine))
                        {
                            __result = true;
                            return;
                        }
                    }
                }
                
                // Otherwise, just let the result of the original method stand.
            }
        }
    }
}