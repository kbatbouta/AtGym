using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PumpingSteel.Core.AI.ThinkDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(TimeAssignmentSelector), nameof(TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid))]
    public static class H_Draw_TimeTable
    {
        private static MethodInfo mrect = AccessTools.Method("H_Draw_TimeTable:GetFitnessRect");
        
        private static void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
        {
            rect = rect.ContractedBy(2f);
            GUI.DrawTexture(rect, ta.ColorTexture);
            if (Widgets.ButtonInvisible(rect))
            {
                TimeAssignmentSelector.selectedAssignment = ta;
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            }
            GUI.color = Color.white;
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = Color.white;
            Widgets.Label(rect, ta.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            if (TimeAssignmentSelector.selectedAssignment == ta)
            {
                Widgets.DrawBox(rect, 2);
            }
            else
            {
                UIHighlighter.HighlightOpportunity(rect, ta.cachedHighlightNotSelectedTag);
            }
        }

        public static void GetFitnessRect(Rect rect2)
        {
            if (!ModsConfig.RoyaltyActive)
            {
                rect2.x += rect2.width;
                rect2.y -= rect2.height;
            }
            else
            {
                rect2.x += rect2.width;
            }
            
            DrawTimeAssignmentSelectorFor(rect2, FitnessTimeTableDefOf.Workout);
        }
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count - 1; i++) yield return codes[i];

            var firstCode = new CodeInstruction(OpCodes.Ldloc_0);
            var lastCode = codes[codes.Count - 1];
            if (lastCode.labels.Count > 0) firstCode.labels.Add(lastCode.labels[0]);
            yield return firstCode;
            yield return new CodeInstruction(OpCodes.Call, operand: mrect);
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}