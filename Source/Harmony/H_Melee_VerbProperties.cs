using HarmonyLib;
using PumpingSteel.Fitness;
using UnityEngine;
using Verse;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.AdjustedMeleeDamageAmount),
        new[] {typeof(Verb), typeof(Pawn)})]
    public static class H_Melee_VerbProperties
    {
        public static void Postfix(Verb ownerVerb,
            Pawn attacker,
            ref float __result,
            VerbProperties __instance)
        {
            if (!__instance.IsMeleeAttack) return;

            if (Finder.StaminaTracker.TryGet(attacker, out StaminaUnit unit))
            {
                if (unit.staminaLevel > 0.5f)
                {
                    __result *= 1.3f;
                    unit.staminaLevel = Mathf.Clamp(unit.staminaLevel - 0.01f, 0f, 1f);
                }
            }
        }
    }
}