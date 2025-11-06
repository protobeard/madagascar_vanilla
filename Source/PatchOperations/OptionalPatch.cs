using System.Xml;
using Verse;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace MadagascarVanilla.PatchOperations
{
    // Check whether the specified mod setting key is true, and only apply the patch if so.
    public class OptionalPatch : PatchOperation
    {
        private PatchOperation match;
        private string key;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            //bool apply = (bool) MadagascarVanillaMod.Persistables.GetType().GetField(key).GetValue(MadagascarVanillaMod.Persistables);
            bool apply = true;
            
            if (apply && match != null)
            {
                return match.Apply(xml);
            }

            return true;
        }
    }
}
