using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MadagascarVanilla.Persistables
{
    public class PersistableDrugPolicy : DrugPolicy
    {
        // DrugPolicyDef
        public string sourceDefName;

        // List<DrugPolicyEntry>
        public List<PersistableDrugPolicyEntry> entriesIntNoDef = new List<PersistableDrugPolicyEntry>();

        public PersistableDrugPolicy() { }

        public PersistableDrugPolicy(DrugPolicy drugPolicy)
        {
            id = drugPolicy.id;
            label = drugPolicy.label;
            sourceDefName = drugPolicy.sourceDef?.defName;
            
            for (int i = 0; i < drugPolicy.Count; i++)
            {
                PersistableDrugPolicyEntry persistableDrugPolicyEntry = new PersistableDrugPolicyEntry(drugPolicy[i]);
                entriesIntNoDef.Add(persistableDrugPolicyEntry);
            }
        }
        
        public DrugPolicy DrugPolicyFromSelf()
        {
            if (MadagascarVanillaMod.Verbose()) Log.Message($"Making DrugPolicy");
            DrugPolicy drugPolicy = new DrugPolicy(id, label);
            drugPolicy.sourceDef = DefDatabase<DrugPolicyDef>.GetNamed(sourceDefName);

            for (int i = 0; i < drugPolicy.Count; i++)
            {
                //Log.Message($"Restoring policy for {drugPolicy[i].drug.defName}");

                foreach (PersistableDrugPolicyEntry noDefPolicy in entriesIntNoDef)
                {
                    // Log.Message($"Evaluating {noDefPolicy.drugName}");
                    if (noDefPolicy.drugName == drugPolicy[i].drug.defName)
                    {
                        drugPolicy[i] = noDefPolicy.DrugPolicyEntryFromSelf();
                        break;
                    }
                }
            }

            return drugPolicy;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref id, "id", 0);
            Scribe_Values.Look(ref label, "label");
            Scribe_Collections.Look(ref entriesIntNoDef, "drugs", LookMode.Deep);
            Scribe_Values.Look(ref sourceDefName, "sourceDefName");
        }
    }
}