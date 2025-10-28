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

    // Add "Any Production Specialist" option to bill work assignment restrictions.
    [HarmonyPatch]
    public static class AnyProductionSpecialistPatch
    {
        private const string EnableProductionSpecialistOnlyBillAssignmentKey = "enableProductionSpecialistOnlyBillAssignment";
        private const string EnableInspiredOnlyBillAssignmentKey = "enableInspiredOnlyBillAssignment";
        private const string ProductionSpecialistRoleName = "IdeoRole_ProductionSpecialist";
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Dialog_BillConfig))]
        [HarmonyPatch("GeneratePawnRestrictionOptions")]
        public static void Postfix(Dialog_BillConfig __instance, ref IEnumerable<Widgets.DropdownMenuElement<Pawn>> __result)
        {
            bool enableProductionSpecialistOnlyBillAssignment = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableProductionSpecialistOnlyBillAssignmentKey));
            bool enableInspiredOnlyBillAssignment = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableInspiredOnlyBillAssignmentKey));

            if (enableProductionSpecialistOnlyBillAssignment && ModsConfig.IdeologyActive)
            {
                PreceptDef productionSpecialistRole = DefDatabase<PreceptDef>.GetNamed(ProductionSpecialistRoleName);
                bool hasIdeoWithProductionSpecialist = Faction.OfPlayer.ideos.AllIdeos.Select(ideo => ideo.RolesListForReading.Select(role => role.def).Contains(productionSpecialistRole)).Any();

                if (hasIdeoWithProductionSpecialist)
                {
                    Bill_Production bill = Traverse.Create(__instance).Field("bill").GetValue<Bill_Production>();
                
                    Widgets.DropdownMenuElement<Pawn> anyProductionSpecialistWidget = new Widgets.DropdownMenuElement<Pawn>
                    {
                        option = new FloatMenuOption("AnyProductionSpecialist".Translate(), delegate
                        {
                            bill.SetAnyPawnRestriction();
                            BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor(bill);
                            if (MadagascarVanillaMod.Verbose()) Log.Message($"GeneratePawnRestrictionOptions: {bill.GetUniqueLoadID()} attrs: {attrs?.ProductionSpecialistOnly}");
                            if (attrs != null)
                                attrs.ProductionSpecialistOnly = true;
                        }),
                        payload = null
                    }; 
                    __result = __result.Prepend(anyProductionSpecialistWidget);
                }
            }

            if (enableInspiredOnlyBillAssignment)
            {
                Bill_Production bill = Traverse.Create(__instance).Field("bill").GetValue<Bill_Production>();
                 
                Widgets.DropdownMenuElement<Pawn> anyInspiredWidget = new Widgets.DropdownMenuElement<Pawn>
                {
                    option = new FloatMenuOption("AnyCreativityInspired".Translate(), delegate
                    {
                        bill.SetAnyPawnRestriction();
                        BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor(bill);
                        if (MadagascarVanillaMod.Verbose()) Log.Message($"GeneratePawnRestrictionOptions: {bill.GetUniqueLoadID()} attrs: {attrs?.InspiredOnly}");
                        if (attrs != null)
                            attrs.InspiredOnly = true;
                    }),
                    payload = null
                };
                __result = __result.Prepend(anyInspiredWidget);
            }
        }

        // Whoever wrote the buttonLabel assignment line should be slapped. It's like a quintuple nested ternary? Fuck no.
        //
        // string buttonLabel = this.bill.PawnRestriction == null ? (!ModsConfig.IdeologyActive || !this.bill.SlavesOnly ? (!ModsConfig.BiotechActive || !this.bill.recipe.mechanitorOnlyRecipe ? (!ModsConfig.BiotechActive || !this.bill.MechsOnly ? (!ModsConfig.BiotechActive || !this.bill.NonMechsOnly ? (string) "AnyWorker".Translate() : (string) "AnyNonMech".Translate()) : (string) "AnyMech".Translate()) : (string) "AnyMechanitor".Translate()) : (string) "AnySlave".Translate()) : this.bill.PawnRestriction.LabelShortCap;
        // Widgets.Dropdown<Bill_Production, Pawn>(listing3.GetRect(30f), this.bill, (Func<Bill_Production, Pawn>) (b => b.PawnRestriction), (Func<Bill_Production, IEnumerable<Widgets.DropdownMenuElement<Pawn>>>) (b => this.GeneratePawnRestrictionOptions()), buttonLabel);
        //
        // We're looking for this (we want to insert our code right after buttonLabel is assigned):
        //
        // IL_0a03: ldstr        "AnyWorker"
        // IL_0a08: call         valuetype Verse.TaggedString Verse.Translator::Translate(string)
        // IL_0a0d: call         string Verse.TaggedString::op_Implicit(valuetype Verse.TaggedString)
        // IL_0a12: stloc.s      buttonLabel
        //
        // // [225 5 - 225 265]
        // IL_0a14: ldloc.s      listing3      <----------- Insertion Point We Want (start of the Widgets.Dropdown line above)
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Dialog_BillConfig))]
        [HarmonyPatch(nameof(Dialog_BillConfig.DoWindowContents))]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = instructions.ToList();
            
            // Let's do a little sanity checking -- all of these are only references once in DoWindowContents, and in this order.
            // So we can confirm that we've found the area we want by confirming that.
            int anySlaveLineNumber = newInstructions.FirstIndexOf(instruction => instruction.operand as string == "AnySlave");
            int anyMechanitorLineNumber = newInstructions.FirstIndexOf(instruction => instruction.operand as string == "AnyMechanitor");
            int anyMechLineNumber = newInstructions.FirstIndexOf(instruction => instruction.operand as string == "AnyMech");
            int anyNonMechLineNumber = newInstructions.FirstIndexOf(instruction => instruction.operand as string == "AnyNonMech");
            int anyWorkerLineNumber = newInstructions.FirstIndexOf(instruction => instruction.operand as string == "AnyWorker");
            
            MethodInfo updateButtonLabelMethod = AccessTools.Method(typeof(AnyProductionSpecialistPatch), nameof(UpdateButtonLabel));
            int buttonLabelLineNumber = anyWorkerLineNumber + 3;
            int targetInsertionLineNumber = anyWorkerLineNumber + 4;
            object buttonLabelOperand = null;
            
            if (!(anySlaveLineNumber < anyMechanitorLineNumber) || !(anyMechLineNumber < anyNonMechLineNumber))
            {
                Log.Error("Madagascar Vanilla: Failed to apply Dialog_BillConfig transpile (failed restriction lookups), returning base");
                return instructions;
            }
            
            // copy buttonLabelOperand from an instruction that already references the local variable we want (buttonLabel).
            if (newInstructions[buttonLabelLineNumber].opcode == OpCodes.Stloc_S && newInstructions[buttonLabelLineNumber].operand is LocalBuilder)
            {
                buttonLabelOperand = newInstructions[buttonLabelLineNumber].operand;
            }
            else
            {
                Log.Error("Madagascar Vanilla: Failed to apply Dialog_BillConfig transpile (failed to find buttonLabel), returning base");
                return instructions;
            }
            
            newInstructions.Insert(targetInsertionLineNumber, new CodeInstruction(OpCodes.Ldarg_0));
            newInstructions.Insert(targetInsertionLineNumber + 1, CodeInstruction.LoadField(typeof(Dialog_BillConfig), "bill"));
            newInstructions.Insert(targetInsertionLineNumber + 2, new CodeInstruction(OpCodes.Ldloc_S, buttonLabelOperand));
            newInstructions.Insert(targetInsertionLineNumber + 3, new CodeInstruction(OpCodes.Call, updateButtonLabelMethod));
            newInstructions.Insert(targetInsertionLineNumber + 4, new CodeInstruction(OpCodes.Stloc_S, buttonLabelOperand));
            
            return newInstructions.AsEnumerable();
        }

        public static string UpdateButtonLabel(Bill_Production bill, string label)
        {
            bool enableProductionSpecialistOnlyBillAssignment = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableProductionSpecialistOnlyBillAssignmentKey));
            bool enableInspiredOnlyBillAssignment = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableInspiredOnlyBillAssignmentKey));

            if (!enableProductionSpecialistOnlyBillAssignment && !enableInspiredOnlyBillAssignment)
                return label;

            if (MadagascarVanillaMod.Verbose()) Log.Message($"UpdateButtonLabel: {label}, {bill.GetUniqueLoadID()}");
            BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor(bill);

            if (attrs == null)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message($"Bill {bill.Label} has no BillAdditionalAttributes. Likely created while Madagascar Vanilla Bill extensions were disabled. Skipping.");
                return label;
            }
            
            if (enableProductionSpecialistOnlyBillAssignment && attrs.ProductionSpecialistOnly)
                return "AnyProductionSpecialist".Translate();

            if (enableInspiredOnlyBillAssignment && attrs.InspiredOnly)
                return "AnyCreativityInspired".Translate();
            
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
                bool enableProductionSpecialistOnlyBillAssignment = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableProductionSpecialistOnlyBillAssignmentKey));
                bool enableInspiredOnlyBillAssignment = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableInspiredOnlyBillAssignmentKey));
                if (!enableProductionSpecialistOnlyBillAssignment && !enableInspiredOnlyBillAssignment)
                    return;
            
                BillAdditionalAttributes attrs = BillAdditionalAttributes.GetAttributesFor((Bill_Production) __instance);
                 if (attrs == null)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"Bill {__instance.Label} has no BillAdditionalAttributes. Likely created while Madagascar Vanilla Bill extensions were disabled. Skipping.");
                    return;
                }
                 
                if (ModsConfig.IdeologyActive && __result && attrs.ProductionSpecialistOnly)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"PawnAllowedToStartAnew: Checking if {p.Name} is a production specialist: {p.IsProductionSpecialist()}");
                    __result = p.IsProductionSpecialist();

                    if (!__result)
                        JobFailReason.Is("NotAProductionSpecialist".Translate());
                }
                
                if (__result && attrs.InspiredOnly)
                {
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"PawnAllowedToStartAnew: Checking if {p.Name} is has a creativity inspiration: {p.InspirationDef == InspirationDefOf.Inspired_Creativity}");
                    __result = p.InspirationDef == InspirationDefOf.Inspired_Creativity;

                    if (!__result)
                        JobFailReason.Is("NotInspired".Translate());
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bill_Production))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[] {typeof(RecipeDef), typeof(Precept_ThingStyle)})]
        public static void Postfix(Bill_Production __instance)
        {
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

            private bool _inspiredOnly;
            public bool InspiredOnly { get => _inspiredOnly; set => _inspiredOnly = value; }
            
            private Bill_Production _bill = null;
            public Bill_Production Bill { get => _bill; set => _bill = value; }
            
            public BillAdditionalAttributes(Bill_Production bill)
            {
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
            PreceptDef productionSpecialistRole = DefDatabase<PreceptDef>.GetNamed(ProductionSpecialistRoleName);
            
            return role != null && role.def == productionSpecialistRole;
        }
    }
}