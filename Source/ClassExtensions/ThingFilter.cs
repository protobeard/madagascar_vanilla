using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace MadagascarVanilla.ClassExtensions
{
    public static class ThingFilterExtensions
    {
        public static List<string> AllowedThingDefNames(this ThingFilter filter)
        {
            return filter.AllowedThingDefs.Select(thingDef => thingDef.defName).ToList();
        }

        public static List<string> DisallowedSpecialFilterNames(this ThingFilter filter)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            List<SpecialThingFilterDef> disallowedSpecialFilters = (List<SpecialThingFilterDef>)filter.GetType().GetField("disallowedSpecialFilters", flags)?.GetValue(filter);
            return disallowedSpecialFilters?.Select(specialThingFilterDef => specialThingFilterDef.defName).ToList() ?? new List<string>();
        }

        // For each of these, soft check the DefDatabase for errors, and skip setting allow
        // if they aren't found. That should silently skip any mod content which is no longer available.
        public static void SetAllow(this ThingFilter filter, List<string> thingDefNames, bool value)
        {
            foreach (string thingDefName in thingDefNames)
            {
                ThingDef thingDef = DefDatabase<ThingDef>.GetNamed(thingDefName, false);
                if (thingDef != null)
                    filter.SetAllow(thingDef, value);
                else
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"ThingDef {thingDefName} could not be found for ThingFilter.");
            }
        }
        
        // For each of these, soft check the DefDatabase for errors, and skip setting allow
        // if they aren't found. That should silently skip any mod content which is no longer available.
        public static void SetAllowSpecialFilters(this ThingFilter filter, List<string> specialThingFilterDefNames, bool value)
        {
            foreach (string specialThingFilterDefName in specialThingFilterDefNames)
            {
                SpecialThingFilterDef specialThingFilterDef = DefDatabase<SpecialThingFilterDef>.GetNamed(specialThingFilterDefName, false);
                if (specialThingFilterDef != null)
                    filter.SetAllow(specialThingFilterDef, value);
                else
                    if (MadagascarVanillaMod.Verbose()) Log.Message($"SpecialThingFilterDef {specialThingFilterDefName} could not be found for ThingFilter.");
            }
        }
    }
}