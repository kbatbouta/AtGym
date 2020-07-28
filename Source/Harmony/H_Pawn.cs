using HarmonyLib;
using PumpingSteel.Fitness;
using PumpingSteel.GymUI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Notify_BulletImpactNearby))]
    public static class H_Pawn_BulletImpactNearby
    {
        public static void Postfix(Pawn __instance)
        {
            if (Finder.StaminaTracker.TryGet(__instance, out StaminaUnit sUnit))
                Gizmo_StaminaBar.Notify_AlertDanger(sUnit);
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.PostApplyDamage))]
    public static class H_Pawn_PostApplyDamage
    {
        public static void Postfix(Pawn __instance)
        {
            if (Finder.StaminaTracker.TryGet(__instance, out StaminaUnit sUnit))
                Gizmo_StaminaBar.Notify_AlertDamage(sUnit);
        }
    }
}