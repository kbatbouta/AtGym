using System;
using PumpingSteel.Fitness;
using PumpingSteel.Tools;
using Verse;

namespace PumpingSteel.Core
{
    public class FitnessComp : IFitnessComp<FitnessBodyUnit>
    {
        public override void DoTickRare()
        {
        }

        public override bool ShouldDisable()
        {
            return IsAnimal || !IsHuman;
        }

        public override IFitnessTracker<FitnessBodyUnit> GetTracker()
        {
            return Finder.FitnessTracker;
        }
    }
}