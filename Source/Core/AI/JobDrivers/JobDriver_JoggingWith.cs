#region

using System;
using System.Collections.Generic;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
using Verse.AI;
#pragma warning disable 1591

#endregion

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_JoggeJoinMe : JobDriver_Wait
    {
        public int Ticks;

        // Used by the new jogger to force the old one to wait for him.
        // However this can lead to a dead lock, Thus this custom wait jobdriver is used rather than normal wait.
        protected override IEnumerable<Toil> MakeNewToils()
        {
            // If the requesting pawn is not jogging anymore cancel.
            this.FailOn(() => Ticks++ > 600 || TargetA.Pawn?.CurJobDef?.driverClass != typeof(JobDriver_JoggingWith));

            this.FailOnDownedOrDead(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.A);

            AddEndCondition(() =>
            {
                // If the requesting pawn is not jogging anymore cancel.
                if (Ticks++ > 600 || TargetA.Pawn?.CurJobDef?.driverClass != typeof(JobDriver_JoggingWith))
                    return JobCondition.Succeeded;
                return JobCondition.Ongoing;
            });

            return base.MakeNewToils();
        }
    }

    public class JobDriver_JoggingWith : JobDriver
    {
        private StaminaUnit _staminaUnit;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return Finder.StaminaTracker.TryGet(pawn, out _staminaUnit);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnDownedOrDead(TargetIndex.A);

            Toil toil = new Toil();

            // Force the old jogger to wait the new one by stoping and requeueing the jogging job.
            toil.AddPreInitAction(() =>
            {
                Pawn pawn = (Pawn) job.GetTarget(TargetIndex.A).Thing;

                if (pawn.CurJob?.def == JobDefOf.Wait) return;
                Job waitJob = JobMaker.MakeJob(new JobDef
                {
                    label = "wait for TargetA.",
                    driverClass = typeof(JobDriver_JoggeJoinMe)
                }, new LocalTargetInfo(toil.actor));
                // The resumeCurJobAfterwards is used to force the job back into the queue.
                pawn.jobs.StartJob(waitJob, resumeCurJobAfterwards: true);
            });

            this.FailOnNotAwake(TargetIndex.A);
            this.FailOnMentalState(TargetIndex.A);

            // When this pawn become near the old jogger end the wait job of the ladder.
            toil.AddFinishAction(() =>
            {
                Pawn pawn = (Pawn) job.GetTarget(TargetIndex.A).Thing;
                if (pawn?.CurJobDef?.driverClass == typeof(JobDriver_JoggeJoinMe))
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            });

            // The maing jogge with me loop
            // taken from the Jobdriver_follow mainly with some modifcation.
            toil.tickAction = delegate
            {
                Pawn pawn = (Pawn) job.GetTarget(TargetIndex.A).Thing;

                if (!this.pawn.Position.InHorDistOf(pawn.Position, 4f) ||
                    !this.pawn.Position.WithinRegions(pawn.Position, Map, 2, TraverseParms.For(this.pawn)))
                {
                    if (!this.pawn.CanReach(pawn, PathEndMode.Touch, Danger.Deadly))
                        EndJobWith(JobCondition.Incompletable);
                    else if (!this.pawn.pather.Moving || this.pawn.pather.Destination != pawn)
                        this.pawn.pather.StartPath(pawn, PathEndMode.Touch);
                }
                // if the target pawn is still waiting even so the new jogger is in range end the wait.
                else if (pawn?.CurJobDef?.driverClass == typeof(JobDriver_JoggeJoinMe))
                {
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
                //  If jogger abandon jogging.
                else if (pawn?.CurJobDef?.driverClass != typeof(JobDriver_Jogging))
                {
                    this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else
                {
                    // TODO: modify stamina here.
                    if (Finder.GameTicks % 30 != 0) return;
                        
                    _staminaUnit.staminaOffset += 0.00024f;
                    
                    this.pawn.needs.rest.CurLevelPercentage -= 0.005f;
                    this.pawn.needs.food.CurLevelPercentage -= 0.005f;
                }
            };

            // Used to encourage interactions.
            toil.socialMode = RandomSocialMode.SuperActive;
            toil.defaultCompleteMode = ToilCompleteMode.Never;

            yield return toil;
        }
    }
}