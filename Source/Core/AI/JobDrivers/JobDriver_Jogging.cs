#region

using System.Collections.Generic;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
using Verse.AI;

#endregion

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_Jogging : JobDriver
    {
        private StaminaUnit _staminaUnit;

        private int Ticks;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // End the current job if the timetable assignment change to something other than workout
            AddEndCondition(() =>
            {
                if (pawn?.timetable?.CurrentAssignment != FitnessTimeTableDefOf.Workout || Ticks++ >= 2400)
                    return JobCondition.Succeeded;
                return JobCondition.Ongoing;
            });

            return Finder.StaminaTracker.TryGet(pawn, out _staminaUnit);
        }

        /// <inheritdoc />
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return GotoCell(TargetIndex.A, PathEndMode.Touch);
        }

        private Toil GotoCell(TargetIndex ind, PathEndMode peMode)
        {
            Toil toil = new Toil();
            
            // starting the pather (path control)
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                actor.pather.StartPath(actor.jobs.curJob.GetTarget(ind), peMode);
            };

            toil.tickAction = delegate
            {
                if (Finder.GameTicks % 30 != 0) return;
                
                // TODO: modify stamina here.
                _staminaUnit.staminaOffset += 0.00024f;
                
                this.pawn.needs.rest.CurLevelPercentage -= 0.005f;
                this.pawn.needs.food.CurLevelPercentage -= 0.005f;
            };

            // Used to encourage interactions.
            toil.socialMode = RandomSocialMode.SuperActive;
            
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedOrNull(ind);
            return toil;
        }
    }
}