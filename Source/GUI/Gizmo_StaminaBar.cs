using PumpingSteel.Fitness;
using UnityEngine;
using Verse;

namespace PumpingSteel.GymUI
{
    public class Gizmo_StaminaBar : Gizmo
    {
        private readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        private readonly Texture2D FullShieldBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.15f, 0.32f, 0.15f));

        private readonly Texture2D LowShieldBarTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(0.15f, 0.32f, 0.32f));

        public StaminaUnit unit;

        public Gizmo_StaminaBar(StaminaUnit unit)
        {
            this.unit = unit;
        }

        private float staminaLevel => unit?.staminaLevel ?? 0f;

        public override bool Visible => true;

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            var rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            var rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, "Stamina (" + unit.CurStaminaMod + ")");
            var rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            var fillPercent = unit.staminaLevel / unit.maxStaminaLevel;
            var tex = unit.CurStaminaMod != StaminaMod.Breathing ? FullShieldBarTex : LowShieldBarTex;

            if ((unit.extras.DamageAlertCountDown > 0 || unit.extras.DangerAlertCountDown > 0) &&
                Finder.GameTicks - unit.extras.GUIlastTick > 30)
            {
                unit.extras.DamageAlertCountDown = 0;
                unit.extras.DangerAlertCountDown = 0;
                unit.extras.GUIlastTick = unit.extras.GUIlastTick;
            }

            if (unit.extras.DamageAlertCountDown > 0)
            {
                tex = SolidColorMaterials.NewSolidColorTexture(new Color(
                    Mathf.Clamp(unit.extras.DamageAlertCountDown / 10, 0, 1),
                    0.1f,
                    0.1f));
                unit.extras.DamageAlertCountDown--;
            }
            else if (unit.extras.DangerAlertCountDown > 0)
            {
                tex = SolidColorMaterials.NewSolidColorTexture(new Color(
                    Mathf.Clamp(unit.extras.DangerAlertCountDown / 10, 0, 1),
                    0.1f,
                    0.1f));
                unit.extras.DangerAlertCountDown--;
            }

            Widgets.FillableBar(rect4, fillPercent,
                tex, EmptyShieldBarTex,
                false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4,
                (unit.staminaLevel * 100f).ToString("F0") + " / " + (unit.maxStaminaLevel * 100f).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;

            return new GizmoResult(GizmoState.Clear);
        }

        public static void Notify_AlertDanger(StaminaUnit unit)
        {
            unit.extras.DangerAlertCountDown += 10;
            unit.extras.GUIlastTick = unit.extras.GUIlastTick;
        }

        public static void Notify_AlertDamage(StaminaUnit unit)
        {
            unit.extras.DamageAlertCountDown += 10;
            unit.extras.GUIlastTick = unit.extras.GUIlastTick;
        }


        public override float GetWidth(float maxWidth)
        {
            return Mathf.Min(140f, maxWidth);
        }
    }
}