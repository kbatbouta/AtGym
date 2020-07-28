using RimWorld;
using Verse;

namespace PumpingSteel.Core.AI.ThinkDefs
{
    [DefOf]
    public class FitnessJobDefOf
    {
        public static JobDef FitnessJogging;
        public static JobDef FitnessJoggingWith;
        public static JobDef FitnessPushup;
        public static JobDef FitnessCrunches;
    }

    [DefOf]
    public class FitnessWorkGiverDefOf
    {
    }

    [DefOf]
    public class FitnessWorkTypeDefOf
    {
    }

    [DefOf]
    public class FitnessTimeTableDefOf
    {
        public static TimeAssignmentDef Workout;
    }
}