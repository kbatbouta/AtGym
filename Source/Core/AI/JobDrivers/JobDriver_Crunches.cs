#region

using System;
using System.Collections.Generic;
using Verse.AI;

#endregion

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_Crunches : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            throw new NotImplementedException();
        }
    }
}