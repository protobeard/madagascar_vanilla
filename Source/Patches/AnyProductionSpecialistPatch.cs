using System;
using System.Collections.Generic;
using System.Linq;
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

        // [HarmonyPatch(typeof(Dialog_BillConfig))]
        // [HarmonyPatch(nameof(Dialog_BillConfig.DoWindowContents))]
        // public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        // {
        //     // FIXME: need to set buttonLabel to "AnyProductionSpecialist".Translate() if that's been selected.
        //     // Whoever wrote the buttonLabel assignment line should be slapped. It's like a quintuple nested ternary? Fuck no.
        //     //
        //     // string buttonLabel = this.bill.PawnRestriction == null ? (!ModsConfig.IdeologyActive || !this.bill.SlavesOnly ? (!ModsConfig.BiotechActive || !this.bill.recipe.mechanitorOnlyRecipe ? (!ModsConfig.BiotechActive || !this.bill.MechsOnly ? (!ModsConfig.BiotechActive || !this.bill.NonMechsOnly ? (string) "AnyWorker".Translate() : (string) "AnyNonMech".Translate()) : (string) "AnyMech".Translate()) : (string) "AnyMechanitor".Translate()) : (string) "AnySlave".Translate()) : this.bill.PawnRestriction.LabelShortCap;
        //     // Widgets.Dropdown<Bill_Production, Pawn>(listing3.GetRect(30f), this.bill, (Func<Bill_Production, Pawn>) (b => b.PawnRestriction), (Func<Bill_Production, IEnumerable<Widgets.DropdownMenuElement<Pawn>>>) (b => this.GeneratePawnRestrictionOptions()), buttonLabel);
        //
        //     
        //     return instructions;
        // }

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