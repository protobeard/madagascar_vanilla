using RimWorld;
using HarmonyLib;
using MadagascarVanilla.ClassExtensions;
using Verse;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompMilkable))]
    [HarmonyPatch(nameof(CompMilkable.CompInspectStringExtra))]
    public static class MilkableDisplayPatch
    {
        public static void Postfix(CompMilkable __instance, ref string __result)
        {
            // Bail if there is no inspect string or the type of resource being produced is milk.
            if (__result == null || __instance.Props.milkDef.IsMilk())
                return;
            
            if (MadagascarVanillaMod.Persistables.EnableCompMilkableDisplayProperItem)
            {
                __result = __instance.Props.milkDef.LabelCap;
                __result += " " + "MilkableResourceFullness".Translate() + ": ";
                __result += __instance.Fullness.ToStringPercent();
            }
        }
    }
}