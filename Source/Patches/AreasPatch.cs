using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    [HarmonyPatch(typeof(AreaManager))]
    [HarmonyPatch(nameof(AreaManager.AddStartingAreas))]
    public static class AreasPatch
    {
        private const string StartingAreasList = "startingAreasList";
        private const char StartingAreaDelimiter = ',';
        
        public static void Postfix(AreaManager __instance)
        {
            string startingAreasString = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, StartingAreasList));
            List<string> startingAreasList = startingAreasString.Split(StartingAreaDelimiter).ToList();

            if (startingAreasList.Any())
            {
                // Remove the default Area RimWorld creates ("Area 1")
                Area defaultArea = __instance.AllAreas.Where<Area>((Func<Area, bool>)(a => a is Area_Allowed)).First();
                defaultArea.Delete();

                // Create our desired default Areas
                foreach (string startingAreaName in startingAreasList)
                {
                    if (__instance.TryMakeNewAllowed(out Area_Allowed areaAllowed))
                    {
                        areaAllowed.RenamableLabel = startingAreaName;
                    }
                    else
                    {
                        Log.Message("Attempted to create more default areas than what is allowed.");
                    }
                }
            }

        }
    }
}