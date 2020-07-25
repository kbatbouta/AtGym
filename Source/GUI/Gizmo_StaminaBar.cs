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
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.DrawBoxSolid(new Rect(9, 9, GetWidth(maxWidth) - 18 ,17), Color.white);
            Widgets.FillableBar(new Rect(10, 10, GetWidth(maxWidth) - 20 ,15),
                unit.staminaLevel/unit.maxStaminaLevel);
            
            Widgets.Label(new Rect(10, 30, 100,15),"Stamina: "  + Math.Floor((unit.staminaLevel/unit.maxStaminaLevel) * 10) + "/10 ");
            Widgets.Label(new Rect(10, 47, 100,15),"Mod: " + unit.CurStaminaMod);
            GUI.EndGroup();
            return new GizmoResult(GizmoState.Clear);
        }

        public override float GetWidth(float maxWidth)
        {
            return Mathf.Min(110f, maxWidth);
        }
    }
}