using System.Collections.Generic;
using System.Linq;
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
        private const string OdysseyQuestExtensionMultiplerKey = "odysseyQuestExtensionMultipler";
        
        // The RunInt() method uses a readonly variable TimeoutDays to compute how many delayTicks the quest should
        // have before expiring, then passes that along to a couple relevant other objects. Since TicksPerDay is a nice
        // easy number to find, we can home in on that line and add our extension multiplier in right as RunInt()
        // is computing delayTicks:
        //      int delayTicks = QuestNode_Root_Asteroid.TimeoutDays.RandomInRange * GenDate.TicksPerDay;
        // We can premultiply in our extension and replace GenDate.TicksPerDay in the computation. Nice small patch.
        //
        // Looking for line:
        // IL_00be: ldsfld       valuetype Verse.IntRange RimWorld.QuestGen.QuestNode_Root_Asteroid::TimeoutDays
        // IL_00c3: stloc.s      V_7
        // IL_00c5: ldloca.s     V_7
        // IL_00c7: call         instance int32 Verse.IntRange::get_RandomInRange()
        // IL_00cc: ldc.i4       60000 // 0x0000ea60
        // IL_00d1: mul
        // IL_00d2: stloc.s      delayTicks
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int questExtensionMultiplier = int.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, OdysseyQuestExtensionMultiplerKey));
            int ticksPerDayExtension = GenDate.TicksPerDay * questExtensionMultiplier;
            
            List<CodeInstruction> newInstructions = instructions.ToList();
            int referenceLineNumber = newInstructions.FirstIndexOf((CodeInstruction instruction) => instruction.opcode == OpCodes.Ldc_I4 && (int) instruction.operand == GenDate.TicksPerDay);

            if (referenceLineNumber != -1)
            {
                newInstructions[referenceLineNumber] = new CodeInstruction(OpCodes.Ldc_I4, operand: ticksPerDayExtension);
                return newInstructions.AsEnumerable();
            }
            else
            {
                Log.Error("Madagascar Vanilla: Failed to apply odyssey quest time extension transpile, returning base");
            }
            return instructions;
        }
    }
}