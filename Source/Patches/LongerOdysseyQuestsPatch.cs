using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using Verse;
using HarmonyLib;
using RimWorld.QuestGen;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(QuestNode_Root_Asteroid))]
    [HarmonyPatch("RunInt")]
    public static class LongerOdysseyQuestsPatch
    {
        private const string ExtendOdysseyQuests = "extendOdysseyQuests";
        private const string OdysseyQuestExtensionMultipler = "odysseyQuestExtensionMultipler";
        
        // The RunInt() method uses a readonly variable TimeoutDays to compute how many delayTicks the quest should
        // have before expiring, then passes that along to a couple relevant other objects. Since TicksPerDay is a nice
        // easy number to find, we can home in on that line and add our extension multiplier in right as RunInt()
        // is computing delayTicks:
        //      int delayTicks = QuestNode_Root_Asteroid.TimeoutDays.RandomInRange * GenDate.TicksPerDay;
        // We can premultiply in our extension and replace GenDate.TicksPerDay in the computation. Nice small patch.
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines)
        {
            bool extendOdysseySubquests = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, ExtendOdysseyQuests));
            int questExtensionMultiplier = int.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, OdysseyQuestExtensionMultipler));
            int ticksPerDayExtension = GenDate.TicksPerDay * questExtensionMultiplier;
            
            if (extendOdysseySubquests)
            {
                List<CodeInstruction> newLines = lines.ToList();
                int referenceLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.opcode == OpCodes.Ldc_I4 && (int) instruction.operand == 60000);

                if (referenceLineNumber != -1)
                {
                    newLines[referenceLineNumber] = new CodeInstruction(OpCodes.Ldc_I4, operand: ticksPerDayExtension);
                    return newLines.AsEnumerable();
                }
                else
                {
                    Log.Error("Madagascar Vanilla: Failed to apply odyssey quest extension transpile, returning base");
                }
            }
            return lines;
        }
    }
}