using System.Collections.Generic;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
using Verse.AI;

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_JoggeJoinMe : JobDriver_Wait
    {
        public int Ticks = 0;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() =>
            {
                return Ticks++ > 600 || TargetA.Pawn?.CurJobDef?.driverClass != typeof(JobDriver_JoggingWith);
            });

            this.FailOnDownedOrDead(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.A);

            AddEndCondition(() =>
            {
                if (Ticks++ > 600 || TargetA.Pawn?.CurJobDef?.driverClass != typeof(JobDriver_JoggingWith))
                    return JobCondition.Succeeded;
                return JobCondition.Ongoing;
            });

            return base.MakeNewToils();
        }
    }

    public class JobDriver_JoggingWith : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnDownedOrDead(TargetIndex.A);

            Toil toil = new Toil();

            toil.AddPreInitAction(() =>
            {
                Pawn pawn = (Pawn) job.GetTarget(TargetIndex.A).Thing;
                if (pawn.CurJob?.def != JobDefOf.Wait)
                {
                    Job job = JobMaker.MakeJob(new JobDef()
                    {
                        label = "wait for TargetA.",
                        driverClass = typeof(JobDriver_JoggeJoinMe)
                    }, new LocalTargetInfo(toil.actor));
                    pawn.jobs.StartJob(job, resumeCurJobAfterwards: true);
                }
            });

            this.FailOnNotAwake(TargetIndex.A);
            this.FailOnMentalState(TargetIndex.A);

            toil.AddFinishAction(() =>
            {
                Pawn pawn = (Pawn) job.GetTarget(TargetIndex.A).Thing;
                if (pawn?.CurJobDef?.driverClass == typeof(JobDriver_JoggeJoinMe))
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            });

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
                else if (pawn?.CurJobDef?.driverClass == typeof(JobDriver_JoggeJoinMe))
                {
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
                else if (pawn?.CurJobDef?.driverClass != typeof(JobDriver_Jogging))
                {
                    this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else
                {
                }
            };

            toil.socialMode = RandomSocialMode.SuperActive;
            toil.defaultCompleteMode = ToilCompleteMode.Never;

            yield return toil;
        }
    }
}