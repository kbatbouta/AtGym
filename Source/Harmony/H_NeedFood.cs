using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PumpingSteel.Fitness;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(Need_Food), "HungerRate", MethodType.Getter)]
    public static class H_NeedFood_HungerRate
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(ref float __result, Pawn ___pawn)
        {
            if (___pawn == null) return;

            if (Finder.FitnessTracker.TryGet(___pawn, out var unit)) __result = __result;

            if (Finder.StaminaTracker.TryGet(___pawn, out StaminaUnit sUnit))
            {
                if (sUnit.CurStaminaMod == StaminaMod.Walking) __result *= 1.2f;
            }
        }
    }
    
    [HarmonyPatch(typeof(Need_Food), "HungerRateIgnoringMalnutrition", MethodType.Getter)]
    public static class H_NeedFood_HungerRate_IgnoringMal
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(ref float __result, Pawn ___pawn)
        {
            if (___pawn == null) return;
            
            if (Finder.StaminaTracker.TryGet(___pawn, out StaminaUnit sUnit))
            {
                if (sUnit.CurStaminaMod == StaminaMod.Walking) __result *= 1.2f;
            }
        }
    }
}