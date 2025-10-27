using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using HarmonyLib;
using Verse;
using Verse.AI;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    // Set bill defaults:
    // - repeatMode
    //  - If TargetCount, hitpointRange
    //  - If TargetCount, qualityRange
    // - storeMode
    // - ingredientSearchRadius
    //
    // For tailoring bills, disable ingredients:
    // - cloth
    // - devilstrand, hyperweave, synthread, thrumbofur, thrumbomane
    // - human leather, dread leather
    [HarmonyPatch]
    public static class AnyProductionSpecialistPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Dialog_BillConfig))]
        [HarmonyPatch("GeneratePawnRestrictionOptions")]
        public static void Postfix(Dialog_BillConfig __instance, ref IEnumerable<Widgets.DropdownMenuElement<Pawn>> __result)
        {
            // FIXME: only add if there are any production specialists
            if (ModsConfig.IdeologyActive)
            {
                Bill_Production bill = Traverse.Create(__instance).Field("bill").GetValue<Bill_Production>();
                
                Widgets.DropdownMenuElement<Pawn> anyProductionSpecialistWidget = new Widgets.DropdownMenuElement<Pawn>
                {
                    option = new FloatMenuOption("AnyProductionSpecialist".Translate(), delegate
                    {
                        bill.SetAnyPawnRestriction();
                        BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor(bill);
                        Log.Message($"GeneratePawnRestrictionOptions: {bill.GetUniqueLoadID()} attrs: {attrs?.ProductionSpecialistOnly}");
                        if (attrs != null)
                            attrs.ProductionSpecialistOnly = true;
                    }),
                    payload = null
                };

                __result = __result.Prepend(anyProductionSpecialistWidget);
            }
        }

        [HarmonyPatch(typeof(Dialog_BillConfig))]
        [HarmonyPatch(nameof(Dialog_BillConfig.DoWindowContents))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> lines)
        {
            // FIXME: need to set buttonLabel to "AnyProductionSpecialist".Translate() if that's been selected.
            // Whoever wrote the buttonLabel assignment line should be slapped. It's like a quintuple nested ternary? Fuck no.
            //
            // string buttonLabel = this.bill.PawnRestriction == null ? (!ModsConfig.IdeologyActive || !this.bill.SlavesOnly ? (!ModsConfig.BiotechActive || !this.bill.recipe.mechanitorOnlyRecipe ? (!ModsConfig.BiotechActive || !this.bill.MechsOnly ? (!ModsConfig.BiotechActive || !this.bill.NonMechsOnly ? (string) "AnyWorker".Translate() : (string) "AnyNonMech".Translate()) : (string) "AnyMech".Translate()) : (string) "AnyMechanitor".Translate()) : (string) "AnySlave".Translate()) : this.bill.PawnRestriction.LabelShortCap;
            // Widgets.Dropdown<Bill_Production, Pawn>(listing3.GetRect(30f), this.bill, (Func<Bill_Production, Pawn>) (b => b.PawnRestriction), (Func<Bill_Production, IEnumerable<Widgets.DropdownMenuElement<Pawn>>>) (b => this.GeneratePawnRestrictionOptions()), buttonLabel);

            // What we want:
            //
            // string buttonLabel = "Whatever bs from above";
            // BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor((Bill_Production) this.bill);
            // if (attrs != null && attrs.ProductionSpecialistOnly)
            //     buttonLabel = "AnyProductionSpecialist".Translate();
            
            CodeMatcher matcher = new CodeMatcher(lines);
            MethodInfo getSlavesOnly = AccessTools.PropertyGetter(typeof(Bill_Production), "AnySlave");
            MethodInfo getMechanitorOnly = AccessTools.PropertyGetter(typeof(RecipeDef), "mechanitorOnlyRecipe");
            MethodInfo getMechsOnly = AccessTools.PropertyGetter(typeof(Bill_Production), "MechsOnly");
            MethodInfo getNonMechsOnly = AccessTools.PropertyGetter(typeof(Bill_Production), "NonMechsOnly");
            //MethodInfo getAnyWorker = AccessTools.PropertyGetter(typeof(Bill_Production), "AnyWorker");
            
            //MethodInfo updateButtonLabelMethod = SymbolExtensions.GetMethodInfo(() => UpdateButtonLabel(default, default));

             // Bill_Production bp = new Bill_Production();
             // string x = "";
             // x = UpdateButtonLabel(bp, x);
            
            // A line above our target line:
            // IL_0916: ldloc.s      listingStandard1
            // IL_0918: ldc.r4       12
            // IL_091d: callvirt     instance void Verse.Listing::Gap(float32)

            //matcher.MatchStartForward(
                    //new CodeMatch(OpCodes.Callvirt, getSlavesOnly))
                    //new CodeMatch(OpCodes.Ldfld, getMechanitorOnly), 
                    //new CodeMatch(OpCodes.Callvirt, getMechsOnly),
                    //new CodeMatch(OpCodes.Callvirt, getNonMechsOnly)
                    //CodeMatch.Calls(getNonMechsOnly))

                    // new CodeMatch(OpCodes.Ldloc_S, "listingStandard1"),
                    // new CodeMatch(OpCodes.Ldc_R4, 12),
                    // new CodeMatch(OpCodes.Callvirt, "instance void Verse.Listing::Gap(float32)"))

                // .ThrowIfNotMatch($"Could not find buttonLabel assignment line.")
                // .Advance(10);
                // .InsertAfter(
                //     CodeInstruction.LoadArgument(0),
                //     CodeInstruction.LoadField(typeof(RimWorld.Dialog_BillConfig), "bill"),
                //     new CodeInstruction(OpCodes.Ldloc_S),
                //     CodeInstruction.Call(() => UpdateButtonLabel(default, default)),
                //     new CodeInstruction(OpCodes.Stloc_S)
                // );
                
                
                // .Insert(
                //     new CodeInstruction(OpCodes.Ldarg_0),  // load this onto stack (Dialog_BillConfig)
                //     new CodeInstruction(OpCodes.Ldfld, operand: "class RimWorld.Bill_Production RimWorld.Dialog_BillConfig::bill"), // get the Bill onto the stack from this: maybe?
                //     new CodeInstruction(OpCodes.Ldloc_S, "buttonLabel"), // load buttonLabel onto the stack
                //     new CodeInstruction(OpCodes.Call, UpdateButtonLabelMethod),
                //     new CodeInstruction(OpCodes.Stloc_S, "buttonLabel")
                //     );
            
            // return matcher.InstructionEnumeration();
            
            
            List<CodeInstruction> newLines = lines.ToList();
            // Let's do a little more sanity checking -- all of these are only references once in DoWindowContents, and in this order.
            // So we can confirm that we've found the area we want by confirming that.
            int anySlaveLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.operand as string == "AnySlave");
            int anyMechanitorLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.operand as string == "AnyMechanitor");
            int anyMechLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.operand as string == "AnyMech");
            int anyNonMechLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.operand as string == "AnyNonMech");
            int anyWorkerLineNumber = newLines.FirstIndexOf((CodeInstruction instruction) => instruction.operand as string == "AnyWorker");
            
            MethodInfo updateButtonLabelMethod = AccessTools.Method(typeof(AnyProductionSpecialistPatch), nameof(UpdateButtonLabel));
            
            Log.Message($"{anySlaveLineNumber}, {anyMechanitorLineNumber}, {anyMechLineNumber}, {anyNonMechLineNumber}, {anyWorkerLineNumber}");
            
            if (!(anySlaveLineNumber < anyMechanitorLineNumber) && (anyMechLineNumber < anyNonMechLineNumber))
            {
                Log.Error("Madagascar Vanilla: Failed to apply Dialog_BillConfig transpile, returning base");
                return lines;
            }

            // IL_0a03: ldstr        "AnyWorker"
            // IL_0a08: call         valuetype Verse.TaggedString Verse.Translator::Translate(string)
            // IL_0a0d: call         string Verse.TaggedString::op_Implicit(valuetype Verse.TaggedString)
            // IL_0a12: stloc.s      buttonLabel
            //
            // // [225 5 - 225 265]
            // IL_0a14: ldloc.s      listing3
            
            int targetInsertionLineNumber = anyWorkerLineNumber + 4;
            // copy this from an instruction that already references the local variable we want (buttonLabel).
            object buttonLabelOperand = newLines[targetInsertionLineNumber - 1].operand;
            
            Log.Message("Before insertion");
            Log.Message($"prev: {newLines[targetInsertionLineNumber - 1].opcode}, {newLines[targetInsertionLineNumber - 1].operand}");
            Log.Message($"Target insertion line: {newLines[targetInsertionLineNumber].opcode}, {newLines[targetInsertionLineNumber].operand}");
            Log.Message($"next: {newLines[targetInsertionLineNumber + 1].opcode}, {newLines[targetInsertionLineNumber + 1].operand}");
            Log.Message($"next: {newLines[targetInsertionLineNumber + 2].opcode}, {newLines[targetInsertionLineNumber + 2].operand}");
            
            newLines.Insert(targetInsertionLineNumber, new CodeInstruction(OpCodes.Ldarg_0));
            newLines.Insert(targetInsertionLineNumber + 1, CodeInstruction.LoadField(typeof(Dialog_BillConfig), "bill"));
            newLines.Insert(targetInsertionLineNumber + 2, new CodeInstruction(OpCodes.Ldloc_S, buttonLabelOperand));
            newLines.Insert(targetInsertionLineNumber + 3, new CodeInstruction(OpCodes.Call, updateButtonLabelMethod));
            newLines.Insert(targetInsertionLineNumber + 4, new CodeInstruction(OpCodes.Stloc_S, buttonLabelOperand));
            
            Log.Message("After insertion");
            Log.Message($"Target insertion line: {newLines[targetInsertionLineNumber].opcode}, {newLines[targetInsertionLineNumber].operand}");
            Log.Message($"next: {newLines[targetInsertionLineNumber + 1].opcode}, {newLines[targetInsertionLineNumber + 1].operand}");
            Log.Message($"next: {newLines[targetInsertionLineNumber + 2].opcode}, {newLines[targetInsertionLineNumber + 2].operand}");
            
            return newLines.AsEnumerable();
        }

        public static string UpdateButtonLabel(Bill_Production bill, string label)
        {
            Log.Message($"UpdateButtonLabel: {label}, {bill.GetUniqueLoadID()}");
            BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor(bill);
            
            if (attrs != null && attrs.ProductionSpecialistOnly)
                return "AnyProductionSpecialist".Translate();
            
            return label;
        }

        // If the pawn would otherwise be allowed to start the job, check that
        // they are a production specialist and reject them if they aren't.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bill))]
        [HarmonyPatch(nameof(Bill.PawnAllowedToStartAnew))]
        public static void Postfix(Bill __instance, Pawn p, ref bool __result)
        {
            if (__instance is Bill_Production)
            {
                BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor((Bill_Production) __instance);
                if (ModsConfig.IdeologyActive && __result && attrs != null && attrs.ProductionSpecialistOnly)
                {
                    Log.Message($"PawnAllowedToStartAnew: Checking if {p.Name} is a production specialist: {p.IsProductionSpecialist()}");
                    __result = p.IsProductionSpecialist();

                    if (!__result)
                        JobFailReason.Is("NotAProductionSpecialist".Translate());
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bill_Production))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] {typeof(RecipeDef), typeof(Precept_ThingStyle)})]
        public static void Postfix(Bill_Production __instance)
        {
            Log.Message("Bill_Production.Constructor.Postfix creating additional attributes");
            BillAdditionalAttributes attrs = new BillAdditionalAttributes(__instance);
        }
        
        // BILL ADDITIONAL ATTRIBUTES
        
        // Create a new class BillAdditionalAttributes containing the productionSpecialistOnly boolean and a ref to the bill
        //  - make it IExposable
        //  - instantiate in Bill_Production constructor and give it the new bill, set productionSpecialistOnly to false
        //  - save in ExposeData
        // In various Dialog_BillConfig methods, look up the BillAdditionalAttributes by bill and check productionSpecialistOnly
        public class BillAdditionalAttributes : IExposable
        {
            private static readonly Dictionary<string, BillAdditionalAttributes> AdditionalAttributesMap = new Dictionary<string, BillAdditionalAttributes>(); 
            
            private bool _productionSpecialistOnly;
            public bool ProductionSpecialistOnly { get => _productionSpecialistOnly; set => _productionSpecialistOnly = value; }

            private Bill_Production _bill = null;
            public Bill_Production Bill { get => _bill; set => _bill = value; }
            
            public BillAdditionalAttributes(Bill_Production bill)
            {
                Log.Message($"Creating attributes for bill {bill.GetUniqueLoadID()}");
                this.Bill = bill;
                AdditionalAttributesMap.Add(bill.GetUniqueLoadID(), this);
            }
            
            public void ExposeData()
            {
                Scribe_References.Look(ref _bill, "bill");
            }

            public static BillAdditionalAttributes GetAttributesFor(Bill_Production bill)
            {
                return AdditionalAttributesMap.TryGetValue(bill.GetUniqueLoadID(), out BillAdditionalAttributes result) ? result : null;
            }
        }
        
        // PAWN EXTENSION METHODS

        public static bool IsProductionSpecialist(this Pawn pawn)
        {
            Precept_Role role = pawn.Ideo.GetRole(pawn);

            // TODO: remove magic number
            if (role != null && role.def.defName == "IdeoRole_ProductionSpecialist")
            {
                return true;
            }
            return false;
        }
    }
}