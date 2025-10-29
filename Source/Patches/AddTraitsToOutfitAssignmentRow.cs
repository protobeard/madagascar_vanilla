using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using HarmonyLib;
using MadagascarVanilla.ModExtensions;
using UnityEngine;
using XmlExtensions;
using XmlExtensions.Setting;
using TraitDefOf = MadagascarVanilla.DefOfs.TraitDefOf;

namespace MadagascarVanilla.Patches
{
    //
    // This is a copy of DoCell
    // All returns have been changed to return false
    // Actual patch is marked inline.
    // It might be better to do a Transpiler, but...
    //
    [HarmonyPatch(typeof(PawnColumnWorker_Outfit))]
    [HarmonyPatch(nameof(PawnColumnWorker_Outfit.DoCell))]
    public static class AddTraitsToOutfitAssignmentRowPatch
    {
        private const string AddTraitsToOutFitAssignmentRowKey = "addTraitsToOutFitAssignmentRow";
        
        private static readonly List<TraitDef> OutfitRelevantTraitDefs = new List<TraitDef>
        {
            RimWorld.TraitDefOf.Bloodlust,
            RimWorld.TraitDefOf.Brawler,
            TraitDefOf.Cannibal,
            RimWorld.TraitDefOf.Nudist,
            RimWorld.TraitDefOf.Psychopath,
        };
        
        // Check the pawn for traits that impact what types of apparel they are happy to wear,
        // then display them on the Assignment menu.
        public static bool Prefix(PawnColumnWorker_Outfit __instance, Rect rect, Pawn pawn)
        {
            // Let the original method run if setting is false
            bool addTraitsToOutFitAssignmentRow = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AddTraitsToOutFitAssignmentRowKey));
            if (!addTraitsToOutFitAssignmentRow)
                return true;
            
            // Let the original method run if we can't find the private menuGenerator method.
            MethodInfo buttonGenerateMenuMethodInfo = AccessTools.Method(typeof(PawnColumnWorker_Outfit),"Button_GenerateMenu", new Type[] { typeof(Pawn) });
            if (buttonGenerateMenuMethodInfo == null)
            {
                Log.Error("PawnColumnWorker_Outfit.Prefix: Couldn't find Button_GenerateMenu method. Skipping patch.");
                return true;
            }
            
            if (pawn.outfits == null)
            {
                return false;
            }
            Rect rect2 = rect.ContractedBy(0f, 2f);
            bool somethingIsForced = pawn.outfits.forcedHandler.SomethingIsForced;
            Rect left = rect2;
            Rect right = default(Rect);
            if (somethingIsForced)
            {
                rect2.SplitVerticallyWithMargin(out left, out right, 4f);
            }
            if (pawn.IsQuestLodger())
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(left, "Unchangeable".Translate().Truncate(left.width));
                TooltipHandler.TipRegionByKey(left, "QuestRelated_Outfit");
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                // Actual Patch (minus the checks at the top and return changes)
                string text = pawn.outfits.CurrentApparelPolicy.label;
                if (pawn.story?.traits != null)
                {
                    var outfitRelevantTraits = DefDatabase<TraitDef>.AllDefs.Where(td => td.HasModExtension<OutfitRelevantTraitExtension>() && td.GetModExtension<OutfitRelevantTraitExtension>().OutfitRelevant);
                    var pawnMatchingTraitLabels = outfitRelevantTraits.Where(traitDef => pawn.story.traits.HasTrait(traitDef)).Select(traitDef => pawn.story.traits.GetTrait(traitDef).Label);

                    if (pawnMatchingTraitLabels.Any())
                        text += " (" + String.Join(", ", pawnMatchingTraitLabels) + ")";
                }

                // Create a Func from the methodInfo of Button_GenerateMenu that we grabbed above, bound to the instance of PawnColumnWorker_Outfit.
                Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<ApparelPolicy>>> buttonGenerateMenuMethod = 
                    (Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<ApparelPolicy>>>) Delegate.CreateDelegate(typeof(Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<ApparelPolicy>>>),
                        __instance,
                        buttonGenerateMenuMethodInfo);
                
                Widgets.Dropdown(left, pawn, (Pawn p) => p.outfits.CurrentApparelPolicy, buttonGenerateMenuMethod, text.Truncate(left.width), null, pawn.outfits.CurrentApparelPolicy.label, null, null, paintable: true);
            }
            if (!somethingIsForced)
            {
                return false;
            }
            if (Widgets.ButtonText(right, "ClearForcedApparel".Translate()))
            {
                pawn.outfits.forcedHandler.Reset();
            }
            if (!Mouse.IsOver(right))
            {
                return false;
            }
            TooltipHandler.TipRegion(right, new TipSignal(delegate
            {
                string text = "ForcedApparel".Translate() + ":\n";
                foreach (Apparel item in pawn.outfits.forcedHandler.ForcedApparel)
                {
                    text = text + "\n   " + item.LabelCap;
                }
                return text;
            }, pawn.GetHashCode() * 612));

            return false;
        }
    }
}