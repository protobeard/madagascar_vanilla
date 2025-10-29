using RimWorld;
using HarmonyLib;
using Verse;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(CompMilkable))]
    [HarmonyPatch(nameof(CompMilkable.CompInspectStringExtra))]
    public static class MilkableDisplayPatch
    {
        private const string EnableCompMilkableDisplayProperItemKey = "enableCompMilkableDisplayProperItem";
        
        public static void Postfix(CompMilkable __instance, ref string __result)
        {
            bool enableCompMilkableDisplayProperItem = bool.Parse(SettingsManager.GetSetting(MadagascarVanillaMod.ModId, EnableCompMilkableDisplayProperItemKey));

            // Bail if there is no inspect string or the type of resource being produced is milk.
            if (__result == null || __instance.Props.milkDef.IsMilk())
                return;
            
            if (enableCompMilkableDisplayProperItem)
            {
                __result = __instance.Props.milkDef.LabelCap;
                __result += " " + "MilkableResourceFullness".Translate() + ": ";
                __result += __instance.Fullness.ToStringPercent();
            }
        }
        
        public static bool IsMilk(this ThingDef def)
        {
            if (def.label.Contains("Milk"))
                return true;
            return false;
        }
    }
}