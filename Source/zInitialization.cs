using System.Collections.Generic;
using System.Linq;
using PumpingSteel.GymUI;
using PumpingSteel.Patches;
using Verse;

namespace PumpingSteel
{
    [StaticConstructorOnStartup]
    public static class Initialization
    {
        static Initialization()
        {
            HPatcher.Initialize();
            PatchThings();
        }

        public static void PatchThings()
        {
            var DefStupidPawns = DefDatabase<ThinkTreeDef>.GetNamedSilentFail("Zombie");
            var defPawns = from def in DefDatabase<ThingDef>.AllDefs
                where def.race?.IsFlesh ?? false
                select def;

            foreach (var def in defPawns)
            {
                //if (def.comps == null) def.comps = new List<CompProperties>();
                // def.comps.Add(new FitnessProperties());

                if (def.inspectorTabsResolved?.Any(tab => tab is Tab_InspecterFitnessDebuger) ?? false) continue;

                if (def.inspectorTabsResolved == null)
                    def.inspectorTabsResolved = new List<InspectTabBase>();

                def.inspectorTabsResolved.Add(new Tab_InspecterFitnessDebuger());
                Logging.Warning("def:" + def.defName);
            }
        }
    }
}