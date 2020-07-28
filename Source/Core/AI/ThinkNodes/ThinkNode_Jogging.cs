#region

using System;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI;

#pragma warning disable 1591

#endregion

namespace PumpingSteel.Core.AI.ThinkDefs.ThinkNodes
{
    
    [UsedImplicitly]
    public class ThinkNode_Jogging : ThinkNode
    {
        private static bool TryGiveJogging(Pawn pawn, out Job job)
        {
            job = JobMaker.MakeJob(FitnessJobDefOf.FitnessJogging, new LocalTargetInfo(GetWanderRoot(pawn)));
            return Rand.Chance(0.5f);
        }

        /// <summary>
        /// If an other jogging pawn is found then use him as a target so we can emulate a group movement.
        /// </summary>
        /// <param name="pawn">Self</param>
        /// <param name="job">resulting job</param>
        /// <returns></returns>
        private static bool TryGiveJoggingWith(Pawn pawn, out Job job)
        {
            job = null;
            
            if (TryGetOthersDoing(pawn, FitnessJobDefOf.FitnessJogging, out Pawn other))
            {
                job = JobMaker.MakeJob(FitnessJobDefOf.FitnessJoggingWith, new LocalTargetInfo(other));
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Find the first pawn that is currently doing said job
        /// </summary>
        /// <param name="pawn">Self</param>
        /// <param name="jobDef">Target job def</param>
        /// <param name="other">output: the resulting pawn</param>
        /// <returns>found a pawn or not</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool TryGetOthersDoing([NotNull] Pawn pawn, JobDef jobDef, out Pawn other)
        {
            if (pawn == null) throw new ArgumentNullException(nameof(pawn));
            
            foreach (var p in pawn.Map.mapPawns.FreeColonistsSpawned)
                if (p?.CurJob?.def == jobDef && p != pawn)
                {
                    other = p;
                    return true;
                }

            other = null;
            return false;
        }

        private static IntVec3 GetWanderRoot(Pawn pawn)
        {
            RCellFinder.TryFindRandomExitSpot(pawn, out var spot);

            return spot;
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            // testing the time assignment to only allow jogging while in workout time!!
            if (pawn?.timetable?.CurrentAssignment != FitnessTimeTableDefOf.Workout)
                return ThinkResult.NoJob;

            if (TryGiveJoggingWith(pawn, out Job job))
                return new ThinkResult(job, this);
            
            return TryGiveJogging(pawn, out job) ? new ThinkResult(job, this) : ThinkResult.NoJob;
        }
    }
}