using HarmonyLib;
using PumpingSteel.Core;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
using Verse.AI;

namespace PumpingSteel.Patches
{
    /// <summary>
    /// When ever a pawn try to start a new pather cycle, this patch notify the staminaComp of
    /// this event inoder to maintain a accurate tracking of movement.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_PathFollower), "TrySetNewPath")]
    public static class H_PathFollower_Destination
    {
        public static void Postfix(Pawn_PathFollower __instance, Pawn ___pawn, bool ___moving, bool __result)
        {
            if (!__result) return;

            StaminaComp staminaComp = ___pawn.TryGetComp<StaminaComp>();
            staminaComp?.Notify_DestinationSet();
        }
    }


    /// <summary>
    /// Patching the main interaction point between pawns_pathfollower and the pathfinder.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_PathFollower), nameof(Pawn_PathFollower.StartPath))]
    public static class H_PathFollower_StartPath
    {
        public static void Postfix(Pawn ___pawn, bool ___moving)
        {
            StaminaComp staminaComp = ___pawn.TryGetComp<StaminaComp>();
            staminaComp?.Notify_StartedPath();
        }
    }


    /// <summary>
    /// Used to patch the movement speed stats.
    /// </summary>
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
    public static class H_StatWorker_Movement
    {
        /// <summary>
        /// Used tp "edit"/modify the movment speed of pawns.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="__result"></param>
        /// <param name="___stat"></param>
        public static void Postfix(StatRequest req, ref float __result, StatDef ___stat)
        {
            if (___stat.index != StatDefOf.MoveSpeed.index) return;
        }
    }
}