using RimWorld;
using Verse;
using Verse.AI;

namespace PumpingSteel.Core.AI.ThinkDefs.ThinkNodes
{
    public class ThinkNode_Jogging : ThinkNode
    {
        private bool TryGiveJogging(Pawn pawn, out Job job)
        {
            job = JobMaker.MakeJob(FitnessJobDefOf.FitnessJogging, new LocalTargetInfo(GetWanderRoot(pawn)));
            return Rand.Chance(0.5f);
        }

        private bool TryGiveJoggingWith(Pawn pawn, out Job job)
        {
            job = null;
            if (TryGetOthersDoing(pawn, FitnessJobDefOf.FitnessJogging, out Pawn other))
            {
                job = JobMaker.MakeJob(FitnessJobDefOf.FitnessJoggingWith, new LocalTargetInfo(other));
                return true;
            }

            return false;
        }

        private bool TryGetOthersDoing(Pawn pawn, JobDef jobDef, out Pawn other)
        {
            foreach (var p in pawn.Map.mapPawns.FreeColonistsSpawned)
                if (p?.CurJob?.def == jobDef && p != pawn)
                {
                    other = p;
                    return true;
                }

            other = null;
            return false;
        }

        protected IntVec3 GetWanderRoot(Pawn pawn)
        {
            IntVec3 spot;
            RCellFinder.TryFindRandomExitSpot(pawn, out spot);

            return spot;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if (pawn?.timetable?.CurrentAssignment != FitnessTimeTableDefOf.Workout)
                return ThinkResult.NoJob;

            Job job = null;

            if (TryGiveJoggingWith(pawn, out job))
                return new ThinkResult(job, this);
            if (TryGiveJogging(pawn, out job))
                return new ThinkResult(job, this);

            return ThinkResult.NoJob;
        }
    }
}