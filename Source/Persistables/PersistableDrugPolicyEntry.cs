using RimWorld;
using Verse;

namespace MadagascarVanilla.Persistables
{
    public class PersistableDrugPolicyEntry : IExposable
    {
        // ThingDef
        public string drugName;
        public bool allowedForAddiction;
        public bool allowedForJoy;
        public bool allowScheduled;
        public float daysFrequency = 1f;
        public float onlyIfMoodBelow = 1f;
        public float onlyIfJoyBelow = 1f;
        public int takeToInventory;
        
        public PersistableDrugPolicyEntry() {}

        public PersistableDrugPolicyEntry(DrugPolicyEntry drugPolicyEntry)
        {
            drugName = drugPolicyEntry.drug.defName;
            allowedForAddiction = drugPolicyEntry.allowedForAddiction;
            allowedForJoy = drugPolicyEntry.allowedForJoy;
            allowScheduled = drugPolicyEntry.allowScheduled;
            daysFrequency = drugPolicyEntry.daysFrequency;
            onlyIfMoodBelow = drugPolicyEntry.onlyIfMoodBelow;
            onlyIfJoyBelow = drugPolicyEntry.onlyIfJoyBelow;
            takeToInventory = drugPolicyEntry.takeToInventory;
        }

        public DrugPolicyEntry DrugPolicyEntryFromSelf()
        {
            DrugPolicyEntry drugPolicyEntry = new DrugPolicyEntry();
            drugPolicyEntry.drug = DefDatabase<ThingDef>.GetNamed(drugName);
            drugPolicyEntry.allowedForAddiction  = allowedForAddiction;
            drugPolicyEntry.allowedForJoy = allowedForJoy; 
            drugPolicyEntry.allowScheduled = allowScheduled;
            drugPolicyEntry.daysFrequency = daysFrequency;
            drugPolicyEntry.onlyIfMoodBelow = onlyIfMoodBelow;
            drugPolicyEntry.onlyIfJoyBelow = onlyIfJoyBelow;
            drugPolicyEntry.takeToInventory = takeToInventory;
            return drugPolicyEntry;
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref drugName, "drugName");
            Scribe_Values.Look(ref allowedForAddiction, "allowedForAddiction", defaultValue: false);
            Scribe_Values.Look(ref allowedForJoy, "allowedForJoy", defaultValue: false);
            Scribe_Values.Look(ref allowScheduled, "allowScheduled", defaultValue: false);
            Scribe_Values.Look(ref daysFrequency, "daysFrequency", 1f);
            Scribe_Values.Look(ref onlyIfMoodBelow, "onlyIfMoodBelow", 1f);
            Scribe_Values.Look(ref onlyIfJoyBelow, "onlyIfJoyBelow", 1f);
            Scribe_Values.Look(ref takeToInventory, "takeToInventory", 0);
        }
    }
}