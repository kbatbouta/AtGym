using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using PumpingSteel.Core;
using PumpingSteel.Fitness;
using PumpingSteel.Tools;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PumpingSteel.GymUI
{
    public class Gizmo_StaminaBar : Gizmo
    {
        private StaminaUnit unit = null;
        private StaminaComp comp = null;

        private float staminaLevel => unit?.staminaLevel ?? 0f;

        public Gizmo_StaminaBar(StaminaUnit unit, StaminaComp comp)
        {
            this.unit = unit;
            this.comp = comp;
        }

        public override bool Visible => true;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(rect);
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(10, 10, 100,15),"Stamina: " + Math.Round(unit.staminaLevel, 2));
            Widgets.Label(new Rect(10, 25, 100,15),"Capacity: " + unit.maxStaminaLevel);
            Widgets.Label(new Rect(10, 40, 100,15),"Mod: " + unit.CurStaminaMod);
            Widgets.Label(new Rect(10, 60, 100,15),"Bodysize: " + comp.bodySize);
            GUI.EndGroup();
            return new GizmoResult(GizmoState.Clear);
        }

        public override float GetWidth(float maxWidth)
        {
            return Mathf.Min(100f, maxWidth);
        }
    }
}