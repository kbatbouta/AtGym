using HarmonyLib;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(Need_Food), "HungerRate", MethodType.Getter)]
    public static class H_NeedFood_HungerRate
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(ref float __result, Pawn ___pawn)
        {
            if (___pawn == null) return;
        }
    }

    [HarmonyPatch(typeof(Need_Food), "HungerRateIgnoringMalnutrition", MethodType.Getter)]
    public static class H_NeedFood_HungerRate_IgnoringMal
    {
        [HarmonyPriority(Priority.VeryLow)]
        public static void Postfix(ref float __result, Pawn ___pawn)
        {
            if (___pawn == null) return;
        }
    }
}