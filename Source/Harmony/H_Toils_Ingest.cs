using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PumpingSteel.Fitness;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Patches
{
    public static class H_Toils_Ingest
    {
        private static readonly MethodInfo meth =
            PatchProcessor.GetOriginalInstructions(typeof(Toils_Ingest).GetMethod("FinalizeIngest"))
                .First(inst => inst.opcode == OpCodes.Ldftn).operand as MethodInfo;

        private static readonly MethodInfo mtrans = AccessTools.Method("H_Toils_Ingest:Transpiler");

        private static readonly MethodInfo mtarg = AccessTools.Method("H_Toils_Ingest:HungerReduced");


        private static readonly FieldInfo pawnInfo =
            (PatchProcessor.GetOriginalInstructions(typeof(Toils_Ingest).GetMethod("FinalizeIngest"))
                .First(inst => inst.opcode == OpCodes.Ldftn).operand as MethodInfo)!.DeclaringType.GetField("ingester");

        public static void Patch()
        {
            _ = Finder.Harmony.Patch(meth, transpiler: new HarmonyMethod(mtrans));
        }

        public static void HungerReduced(Pawn ingestor, float amount)
        {
            if (ingestor == null) return;

            if (Finder.StaminaTracker.TryGet(ingestor, out StaminaUnit sUnit))
                sUnit.staminaLevel = Mathf.Clamp(sUnit.staminaLevel - 0.05f, 0f, sUnit.maxStaminaLevel);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            var curFoodLevel = AccessTools.PropertyGetter(typeof(Need), nameof(Need.CurLevel));
            var done = false;
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (!done)
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == curFoodLevel)
                    {
                        done = true;

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, pawnInfo);
                        yield return new CodeInstruction(OpCodes.Ldloc_3);
                        yield return new CodeInstruction(OpCodes.Call, mtarg);
                    }
            }
        }
    }
}