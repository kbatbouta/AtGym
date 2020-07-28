#region

using HarmonyLib;
using PumpingSteel.Fitness;
using PumpingSteel.GymUI;
using UnityEngine;
using Verse;

#endregion

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Notify_BulletImpactNearby))]
    public static class H_Pawn_BulletImpactNearby
    {
        public static void Postfix(Pawn __instance)
        {
            if (Finder.StaminaTracker.TryGet(__instance, out StaminaUnit unit))
            {
                Gizmo_StaminaBar.Notify_AlertDanger(unit);
                unit.staminaLevel = Mathf.Clamp(unit.staminaLevel + 0.03f, 0f, unit.maxStaminaLevel);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.PostApplyDamage))]
    public static class H_Pawn_PostApplyDamage
    {
        public static void Postfix(Pawn __instance)
        {
            if (Finder.StaminaTracker.TryGet(__instance, out StaminaUnit unit))
            {
                Gizmo_StaminaBar.Notify_AlertDamage(unit);
                unit.staminaLevel = Mathf.Clamp(unit.staminaLevel + 0.04f, 0f, unit.maxStaminaLevel);
            }
        }
    }
}