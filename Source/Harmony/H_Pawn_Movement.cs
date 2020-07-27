using HarmonyLib;
using PumpingSteel.Core;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
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

    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StopDead))]
    public static class H_PathFollower_StartPath
    {
        public static void Postfix(Pawn ___pawn, bool ___moving)
        {
            if (!___moving) return;

            StaminaComp staminaComp = ___pawn.TryGetComp<StaminaComp>();
            staminaComp.StartStaminaUpdate();
        }
    }

    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
    public static class H_StatWorker_Movement
    {
        public static void Postfix(StatRequest req, ref float __result, StatDef ___stat)
        {
            if (___stat.index != StatDefOf.MoveSpeed.index) return;

            if (Finder.StaminaTracker.TryGet(req.Thing as Pawn, out StaminaUnit unit))
                switch (unit.CurStaminaMod)
                {
                    case StaminaMod.Breathing:
                        __result *= 0.8f;
                        unit.staminaOffset += 5 * 1e-4f;
                        break;
                    case StaminaMod.Running:
                        __result *= 1.7f;
                        break;
                    case StaminaMod.Walking:
                        __result *= 0.9f + unit.staminaLevel / 1.5f;
                        break;
                }
        }
    }
}