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
    [HarmonyPatch("TryFindSiteTile")]
    public static class DistantOdysseyQuestsPatch
    {
        private const string OdysseyQuestRangeExtender = "odysseyQuestRangeExtender";
        private const char RangeSplitter = '~';
        
        // QuestNode_Root_Asteroid.TryFindSiteTile uses two constants for the min and max distance from the colony,
        // so we can't just change them in a Prefix and let the method run as normal. A Postfix would pretty much
        // have to duplicate all the logic in the original method. So, to allow the original method to run
        // we need a Transpiler so that we can insert our new min and max values on the one line they are used.
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines)
        {
            
            string extendOdysseySubquests = (SettingsManager.GetSetting(MadagascarVanillaMod.ModId, OdysseyQuestRangeExtender));
            List<string> rangeBoundaries = extendOdysseySubquests.Split(RangeSplitter).ToList();
            
            if (rangeBoundaries.Count != 2)
                return lines;
            
            float min = float.Parse(rangeBoundaries[0]);
            float max = float.Parse(rangeBoundaries[1]);
            
            
            // Looking for:
            //
            // FastTileFinder.TileQueryParams query = new FastTileFinder.TileQueryParams(origin, QuestNode_Root_Asteroid.MinDistanceFromColony, QuestNode_Root_Asteroid.MaxDistanceFromColony);
            //
            // IL_004e: ldloca.s     query
            // IL_0050: ldloc.0      // origin
            // IL_0051: ldc.r4       1
            // IL_0056: ldc.r4       3
            List<CodeInstruction> newLines = lines.ToList();
            
            // The 1 and 3 are the only ldc.r4 opcodes in the whole method, so just replace their operands with our new min and max.
            int referenceLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.opcode == OpCodes.Ldc_R4);

            if (referenceLineNumber != -1)
            {
                newLines[referenceLineNumber] = new CodeInstruction(OpCodes.Ldc_R4, operand: min);
                newLines[referenceLineNumber + 1] = new CodeInstruction(OpCodes.Ldc_R4, operand: max);
                return newLines.AsEnumerable();
            }
            else
            {
                Log.Error("Madagascar Vanilla: Failed to apply odyssey quest extension transpile, returning base");
            }
            return lines;
        }
    }
}