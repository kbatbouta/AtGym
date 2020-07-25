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
        
        private static Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.15f, 0.32f, 0.15f));

        private static Texture2D LowShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.15f, 0.32f, 0.32f));

        private static Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        private float staminaLevel => unit?.staminaLevel ?? 0f;

        public Gizmo_StaminaBar(StaminaUnit unit, StaminaComp comp)
        {
            this.unit = unit;
            this.comp = comp;
        }

        public override bool Visible => true;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, "Stamina (" + unit.CurStaminaMod + ")");
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = unit.staminaLevel / unit.maxStaminaLevel;
            
            Widgets.FillableBar(rect4, fillPercent, unit.CurStaminaMod != StaminaMod.Breathing ? FullShieldBarTex : LowShieldBarTex, EmptyShieldBarTex, doBorder: false);
            
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            
            Widgets.Label(rect4, (unit.staminaLevel * 100f).ToString("F0") + " / " + (unit.maxStaminaLevel * 100f).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            
            return new GizmoResult(GizmoState.Clear);
        }

        public override float GetWidth(float maxWidth)
        {
            return Mathf.Min(140f, maxWidth);
        }
    }
}