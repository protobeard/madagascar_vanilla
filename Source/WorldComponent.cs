using System.Collections.Generic;
using Verse;
using RimWorld.Planet;

namespace MadagascarVanilla
{
    public class WorldComponentCache : WorldComponent
    {
        public HashSet<Pawn> HaveSetSchedules; 

        public WorldComponentCache(World world) : base(world)
        {
            HaveSetSchedules = new HashSet<Pawn>();
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref HaveSetSchedules, "haveSetSchedules", LookMode.Reference);
        }
    }
}