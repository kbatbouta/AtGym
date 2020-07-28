using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PumpingSteel.Core.AI.ThinkDefs;
using RimWorld;
using Verse;

namespace PumpingSteel.Patches
{
    /// <summary>
    /// Fix for the infamous "return not implemented Exception" in each of the main roots (GetPriority) for each possible timeslot activity.
    /// The solution is to deny entry to this tree if the current time slot is Workout to avoid the error. 
    /// </summary>
    [HarmonyPatch]
    public class H_JobGiver_Work_GetPriority
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method("JobGiver_GetRest:GetPriority", new[] {typeof(Pawn)});
            yield return AccessTools.Method("JobGiver_Work:GetPriority", new[] {typeof(Pawn)});
            yield return AccessTools.Method("ThinkNode_Priority_GetJoy:GetPriority", new[] {typeof(Pawn)});
        }

        [UsedImplicitly]
        private static bool Prefix(Pawn pawn, ref float __result)
        {
            if (pawn?.timetable?.CurrentAssignment == FitnessTimeTableDefOf.Workout)
            {
                __result = 0f;
                return false;
            }

            return true;
        }
    }
}