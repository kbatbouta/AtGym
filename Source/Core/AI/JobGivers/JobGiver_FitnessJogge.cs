using PumpingSteel.Core.AI.ThinkDefs;
using PumpingSteel.Core.AI.ThinkDefs.JobDrivers;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
using Verse.AI;

namespace PumpingSteel.Core.AI.JobGivers
{
    public class JobGiver_FitnessJogge : ThinkNode_JobGiver
    {
        protected IntVec3 GetWanderRoot(Pawn pawn)
        {
            return WanderUtility.GetColonyWanderRoot(pawn);
        }

        public override float GetPriority(Pawn pawn)
        {
            if (HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                return 15f;
            if (pawn.timetable != null && pawn.timetable.CurrentAssignment == FitnessTimeTableDefOf.Workout)
                return 10f;
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            return JobMaker.MakeJob(new JobDef()
            {
                defName = "Jogging",
                label = "Jogging",
                description = "Jogging to increase stamina capacity.",
                driverClass = typeof(JobDriver_Jogging),
                joyGainRate = 0.01f,
                casualInterruptible = true,
                suspendable = true,
                playerInterruptible = true,
                neverFleeFromEnemies = false
            }, 
                new LocalTargetInfo(GetWanderRoot(pawn)),
                new LocalTargetInfo(GetWanderRoot(pawn)),
                new LocalTargetInfo(pawn.Position));
        }
    }
}