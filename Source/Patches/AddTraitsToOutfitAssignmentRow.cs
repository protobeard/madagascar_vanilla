using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using XmlExtensions;

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
        private const string AddTraitsToOutFitAssignmentRow = "addTraitsToOutFitAssignmentRow";
        
        public static bool Prefix(PawnColumnWorker_Outfit __instance, Rect rect, Pawn pawn, PawnTable table)
        {
            bool startingAreasString = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, AddTraitsToOutFitAssignmentRow));
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
                // FIXME: think about TraitDefOf stuff -- maybe they should all be
                // defined in the Rimworld namepsace?
                // Actual Patch: Nudist, Bloodlust, Cannibal, Psychopath, Brawler
                List<RimWorld.TraitDef> outfitRelevantTraits = new List<RimWorld.TraitDef>
                {
                    RimWorld.TraitDefOf.Nudist,
                    RimWorld.TraitDefOf.Bloodlust,
                    TraitDefOf.Cannibal,
                    RimWorld.TraitDefOf.Psychopath,
                    RimWorld.TraitDefOf.Brawler,
                };
                
                // TODO: clean this up, can't possibly need to iterate over the list twice like this.
                string text = pawn.outfits.CurrentApparelPolicy.label;
                if (pawn.story?.traits != null)
                {
                    List<RimWorld.Trait> pawnMatchingTraits = new List<RimWorld.Trait>();
                    foreach (TraitDef traitDef in outfitRelevantTraits)
                    {
                        Trait trait = pawn.story.traits.GetTrait(traitDef);
                        if (trait != null)
                            pawnMatchingTraits.Add(trait);
                    }

                    if (pawnMatchingTraits.Any())
                    {
                        Trait last = pawnMatchingTraits.Last();
                        text = text + " (";
                        foreach (Trait trait in pawnMatchingTraits)
                        {
                            if (trait != last)
                                text += trait.Label + ", ";
                            else
                                text += trait.Label + ")";
                        }
                    }
                }

                Widgets.Dropdown(left, pawn, (Pawn p) => p.outfits.CurrentApparelPolicy, Button_GenerateMenu, text.Truncate(left.width), null, pawn.outfits.CurrentApparelPolicy.label, null, null, paintable: true);
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
        
        private static IEnumerable<Widgets.DropdownMenuElement<ApparelPolicy>> Button_GenerateMenu(Pawn pawn)
        {
            foreach (ApparelPolicy outfit in Current.Game.outfitDatabase.AllOutfits)
            {
                yield return new Widgets.DropdownMenuElement<ApparelPolicy>
                {
                    option = new FloatMenuOption(outfit.label, delegate
                    {
                        pawn.outfits.CurrentApparelPolicy = outfit;
                    }),
                    payload = outfit
                };
            }
            yield return new Widgets.DropdownMenuElement<ApparelPolicy>
            {
                option = new FloatMenuOption(string.Format("{0}...", "AssignTabEdit".Translate()), delegate
                {
                    Find.WindowStack.Add(new Dialog_ManageApparelPolicies(pawn.outfits.CurrentApparelPolicy));
                })
            };
        }
    }
}