using PumpingSteel.Core.AI.ThinkDefs;
using RimWorld;
using Verse;
using Verse.AI;

namespace PumpingSteel.Core.AI.ThinkNodes
{
    public class ThinkNode_Conditional_WorkoutRoot : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (!pawn.RaceProps.IsFlesh || pawn.Downed) 
                return false;

            if (pawn.Downed || pawn.health.HasHediffsNeedingTend() || HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn) || HealthAIUtility.ShouldBeTendedNowByPlayerUrgent(pawn))
                return false;

            if (pawn.needs.food.Starving || pawn.needs.food.PercentageThreshHungry < pawn.needs.food.CurLevelPercentage)
                return false;

            return pawn?.timetable?.CurrentAssignment == FitnessTimeTableDefOf.Workout;
        }
    }
}