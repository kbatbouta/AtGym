#region

using System.Collections.Generic;
using Verse;

#endregion

namespace PumpingSteel.Core.Capacities.CapacitiesWorkers
{
    public class PawnCapacityWorker_Stamina : PawnCapacityWorker
    {
        public override float CalculateCapacityLevel(HediffSet diffSet,
            List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            var value = base.CalculateCapacityLevel(diffSet, impactors);

            // if (Finder.StaminaTracker.TryGet(diffSet.pawn, out StaminaUnit unit))
            //    value =  (value + unit.maxStaminaLevel) / 2f;

            return value;
        }
    }
}