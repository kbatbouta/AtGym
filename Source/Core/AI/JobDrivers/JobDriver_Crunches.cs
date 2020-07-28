using System.Collections.Generic;
using Verse.AI;

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_Crunches : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            throw new System.NotImplementedException();
        }
    }
}