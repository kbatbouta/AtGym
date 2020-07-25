using System;
using System.Collections.Generic;
using System.Linq;
using PumpingSteel.Core;
using PumpingSteel.Core.Capacities;
using PumpingSteel.Core.Hediffs;
using PumpingSteel.GymUI;
using PumpingSteel.Patches;
using RimWorld;
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

                if (def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_FitBitDebuger) ?? false) continue;

                if (def.inspectorTabsResolved == null)
                    def.inspectorTabsResolved = new List<InspectTabBase>();

                def.inspectorTabsResolved.Add(new ITab_Pawn_FitBitDebuger());
                Logging.Warning("def:" + def.defName);
            }
        }
    }
}