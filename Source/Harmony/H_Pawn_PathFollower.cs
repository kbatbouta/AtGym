using System;
using System.Collections;
using HarmonyLib;
using PumpingSteel.Fitness;
using Verse;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Verse.AI;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StopDead))]
    public static class H_PathFollower_StopDead
    {
        public static void Prefix(Pawn_PathFollower __instance, Pawn ___pawn)
        {
            
        }
    }
    
    
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToPayThisTick")]
    public static class H_PathFollower_CostToPay
    {
        private static int lastUpdated = -1;
        
        private static HashSet<int> _boost = new HashSet<int>();
        private static HashSet<int> _breathing = new HashSet<int>();
        
        
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(Pawn ___pawn, ref float __result)
        {
            if (Finder.StaminaTracker.TryGet(___pawn, out StaminaUnit unit))
            {
                if (unit.CurStaminaMod == StaminaMod.Running) __result *= 1.5f;
                if (unit.CurStaminaMod == StaminaMod.Breathing) __result *= 0.8f;
                if (unit.CurStaminaMod == StaminaMod.Walking) __result *= 0.8f + unit.staminaLevel / 2.0f;
            }
        }
    }
}