using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace MadagascarVanilla.Patches
{
    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch(nameof(AreaManager.AddStartingAreas))]
    public static class AreasPatch
    {
        
        public static void Postfix(AreaManager __instance)
        {
            List<string> startingAreasList = MadagascarVanillaMod.Persistables.StartingAreasList;
            if (!startingAreasList.Any() || (startingAreasList.Count == 1 && startingAreasList.First().NullOrEmpty())) 
                return;
            
            // Remove the default Area RimWorld creates ("Area 1")
            Area defaultArea = __instance.AllAreas.Where((area => area is Area_Allowed)).FirstOrDefault();
            defaultArea?.Delete();

            // Create our desired default Areas
            foreach (string startingAreaName in startingAreasList)
            {
                if (__instance.TryMakeNewAllowed(out Area_Allowed areaAllowed))
                    areaAllowed.RenamableLabel = startingAreaName;
                else
                    Log.Error($"Attempted to create more default areas than what is allowed. Skipped creating {startingAreaName}");
            }
        }
    }
}