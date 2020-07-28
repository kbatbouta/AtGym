using System.Collections.Generic;
using PumpingSteel.Fitness;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_Jogging : JobDriver
    {
        private static StaminaUnit _staminaUnit;

        private int Ticks = 0;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            AddEndCondition(() =>
            {
                if (pawn?.timetable?.CurrentAssignment != FitnessTimeTableDefOf.Workout || Ticks++ >= 2400)
                    return JobCondition.Succeeded;
                return JobCondition.Ongoing;
            });

            return Finder.StaminaTracker.TryGet(pawn, out _staminaUnit);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return GotoCell(TargetIndex.A, PathEndMode.Touch);
        }

        public Toil GotoCell(TargetIndex ind, PathEndMode peMode)
        {
            Toil toil = new Toil();

            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                actor.pather.StartPath(actor.jobs.curJob.GetTarget(ind), peMode);
            };

            toil.tickAction = delegate
            {
                if (Finder.GameTicks % 30 != 0) return;
            };

            toil.socialMode = RandomSocialMode.SuperActive;
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedOrNull(ind);
            return toil;
        }
    }
}