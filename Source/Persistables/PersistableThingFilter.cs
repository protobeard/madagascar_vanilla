using System.Collections.Generic;
using System.Reflection;
using MadagascarVanilla.ClassExtensions;
using RimWorld;
using Verse;

namespace MadagascarVanilla.Persistables
{
    public class PersistableThingFilter : ThingFilter
    {
        // ThingDefs
        public List<string> allowedDefNames = new List<string>();
        
        //SpecialThingFilterDef 
        public List<string> disallowedSpecialFilterNames = new List<string>();

        public FloatRange allowedHitPointsPercents;
        public FloatRange allowedMentalBreakChance;
        public QualityRange allowedQualities;
        public bool onlySpecialFilters;
        
        // ThingCategoryDef
        public string overrideRootDefName;

        public PersistableThingFilter() { }
        
        public PersistableThingFilter(ThingFilter filter)
        {
            //if (MadagascarVanillaMod.Verbose()) Log.Message($"Processing allowed things.");
            allowedDefNames = filter.AllowedThingDefNames();
            
            //if (MadagascarVanillaMod.Verbose()) Log.Message($"Processing disallowed special filters.");
            disallowedSpecialFilterNames = filter.DisallowedSpecialFilterNames();
            
            //if (MadagascarVanillaMod.Verbose()) Log.Message("Processing AllowedHitPointsPercents");
            allowedHitPointsPercents = filter.AllowedHitPointsPercents;
            
            //if (MadagascarVanillaMod.Verbose()) Log.Message("Processing AllowedMentalBreakChance");
            allowedMentalBreakChance = filter.AllowedMentalBreakChance;
            
            //if (MadagascarVanillaMod.Verbose()) Log.Message("Processing OnlySpecialFilters");
            onlySpecialFilters = filter.OnlySpecialFilters;
            
            //if (MadagascarVanillaMod.Verbose()) Log.Message("Processing AllowedQualityLevels");
            allowedQualities = filter.AllowedQualityLevels;
            
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            ThingCategoryDef overrideRootDef = (ThingCategoryDef) filter.GetType().GetField("overrideRootDef", flags)?.GetValue(filter);
            if (overrideRootDef != null)
            {
                if (MadagascarVanillaMod.Verbose()) Log.Message("Processing overrideRootDefName");
                overrideRootDefName = overrideRootDef.defName;
            }
        }

        public ThingFilter ThingFilterFromSelf()
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Making ThingFilter");

            ThingFilter filter;
            if (overrideRootDefName != null)
            {
                // ReadingPolicy effectFilters use these, the others don't
                ThingCategoryDef overrideRootDef = DefDatabase<ThingCategoryDef>.GetNamed(overrideRootDefName);
                filter = new ThingFilter(overrideRootDef, onlySpecialFilters);
            }
            else
            {
                filter = new ThingFilter();
            }

            filter.AllowedHitPointsPercents = allowedHitPointsPercents;
            filter.AllowedMentalBreakChance = allowedMentalBreakChance;
            filter.AllowedQualityLevels = allowedQualities;
    
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Adding thing filters to ThingFilter");
            filter.SetAllow(allowedDefNames, true);
           
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Adding special filters to ThingFilter");
            filter.SetAllowSpecialFilters(disallowedSpecialFilterNames, false);

            return filter;
        }
        
        // Match ThingFilter ExposeData but use strings defNames instead of Defs.
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref disallowedSpecialFilterNames, "disallowedSpecialFilterNames", LookMode.Value);
            Scribe_Collections.Look(ref allowedDefNames, "allowedDefNames");
            Scribe_Values.Look(ref allowedHitPointsPercents, "allowedHitPointsPercents");
            Scribe_Values.Look(ref allowedMentalBreakChance, "allowedMentalBreakChance");
            Scribe_Values.Look(ref allowedQualities, "allowedQualityLevels");
            Scribe_Values.Look(ref onlySpecialFilters, "onlySpecialFilters", defaultValue: false);
            Scribe_Values.Look(ref overrideRootDefName, "overrideRootDefName");
        }
    }
}