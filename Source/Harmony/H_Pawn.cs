using HarmonyLib;
using PumpingSteel.Fitness;
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
            {
                sUnit.DangerAlertCountDown += 10;
                sUnit.staminaLevel = Mathf.Clamp(sUnit.staminaLevel + 0.03f, 0f, sUnit.maxStaminaLevel);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.PostApplyDamage))]
    public static class H_Pawn_PostApplyDamage
    {
        public static void Postfix(Pawn __instance)
        {
            if (Finder.StaminaTracker.TryGet(__instance, out StaminaUnit sUnit))
            {
                sUnit.staminaLevel = Mathf.Clamp(sUnit.staminaLevel + 0.04f, 0f, sUnit.maxStaminaLevel);
                sUnit.DamageAlertCountDown += 10;
            }
        }
    }
}