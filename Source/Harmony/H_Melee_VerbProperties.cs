using HarmonyLib;
using PumpingSteel.Fitness;
using PumpingSteel.GymUI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.AdjustedMeleeDamageAmount), typeof(Verb), typeof(Pawn))]
    public static class H_Melee_VerbProperties
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(Verb ownerVerb,
            Pawn attacker,
            ref float __result,
            VerbProperties __instance)
        {
            if (!__instance.IsMeleeAttack) return;
        }
    }

    [HarmonyPatch(typeof(Pawn_MeleeVerbs), nameof(Pawn_MeleeVerbs.TryMeleeAttack))]
    public static class H_Pawn_MeleeVerbTry
    {
        [HarmonyPriority(Priority.Low)]
        public static void Postfix(bool __result, Pawn_MeleeVerbs __instance)
        {
            if (__result == false) return;

            var pawn = __instance.Pawn;
            if (Finder.StaminaTracker.TryGet(pawn, out StaminaUnit sUnit)) Gizmo_StaminaBar.Notify_AlertDanger(sUnit);
        }
    }
}