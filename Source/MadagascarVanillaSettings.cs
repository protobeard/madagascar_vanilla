using RimWorld;
using Verse;

namespace MadagascarVanilla
{
    public class MadagascarVanillaSettings : ModSettings
    {
        public bool testSetting = false;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.testSetting, "testSetting", false, false);
        }
    }
}