#region

using PumpingSteel.Fitness;

#endregion

namespace PumpingSteel.Core
{
    public class FitnessComp : IFitnessComp<FitnessBodyUnit>
    {
        public override void DoTickRare()
        {
        }

        internal void DoDailyTick()
        {
        }

        internal void FixBodyType()
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