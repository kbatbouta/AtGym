using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PumpingSteel.Core.AI.ThinkDefs;
using RimWorld;
using Verse;

namespace PumpingSteel.Patches
{
    
    [HarmonyPatch]
    public class H_JobGiver_Work_GetPriority
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method("JobGiver_GetRest:GetPriority",new []{typeof(Pawn)});
            yield return AccessTools.Method("JobGiver_Work:GetPriority",new []{typeof(Pawn)});
            yield return AccessTools.Method("ThinkNode_Priority_GetJoy:GetPriority",new []{typeof(Pawn)});
        }

        static bool Prefix(Pawn pawn, ref float __result)
        {
            if (pawn?.timetable?.CurrentAssignment == FitnessTimeTableDefOf.Workout)
            {
                __result = 0f; return false;
            }
            return true;
        }
    }
}