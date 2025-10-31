using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(PlaySettings))]
    [HarmonyPatch("DoMapControls")]
    public static class LearningHelperPatch
    {
        private const string HideLearningHelperButtonKey = "hideLearningHelperButton";
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines)
        {
            bool hideLearningHelperButton = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, HideLearningHelperButtonKey));
            
            if (!hideLearningHelperButton)
                return lines;
            
            // Looking for:
            //
            // IL_0000: ldarg.1      // row
            // IL_0001: ldarg.0      // this
            // IL_0002: ldflda       bool RimWorld.PlaySettings::showLearningHelper
            // IL_0007: ldsfld       class [UnityEngine.CoreModule]UnityEngine.Texture2D Verse.TexButton::ShowLearningHelper
            // IL_000c: ldstr        "ShowLearningHelperWhenEmptyToggleButton"
            // IL_0011: call         valuetype Verse.TaggedString Verse.Translator::Translate(string)
            // IL_0016: call         string Verse.TaggedString::op_Implicit(valuetype Verse.TaggedString)
            // IL_001b: ldsfld       class Verse.SoundDef RimWorld.SoundDefOf::Mouseover_ButtonToggle
            // IL_0020: ldnull
            // IL_0021: callvirt     instance void Verse.WidgetRow::ToggleableIcon(bool&, class [UnityEngine.CoreModule]UnityEngine.Texture2D, string, class Verse.SoundDef, string)
            //
            // We want to Noop all of the above lines -- so look for "ShowLearningHelperWhenEmptyToggleButton", then no-op
            // everything from the previous ldarg.1 to callvirt
            List<CodeInstruction> newLines = lines.ToList();
            int referenceLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.operand as string == "ShowLearningHelperWhenEmptyToggleButton");
            int startLineNumber = referenceLineNumber - 4;
            int endLineNumber = referenceLineNumber + 5;
            int numLinesToRemove = 10;
            
            // Let's do a little more sanity checking
            bool foundStart = newLines[startLineNumber].opcode == OpCodes.Ldarg_1;
            bool foundEnd = newLines[endLineNumber].opcode == OpCodes.Callvirt;
            
            if (foundStart && foundEnd)
            {
                newLines.RemoveRange(startLineNumber, numLinesToRemove);
                return newLines.AsEnumerable();
            }
            else
            {
                Log.Error("Madagascar Vanilla: Failed to apply hide learning helper control transpile, returning base");
            }
            return lines;
        }
    }
}