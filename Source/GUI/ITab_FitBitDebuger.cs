using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PumpingSteel.Fitness;
using PumpingSteel.Tools;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PumpingSteel.GymUI
{
    public class ITab_Pawn_FitBitDebuger : ITabsPlus
    {
        public Vector2 Size = new Vector2(300f, 500f);

        public override bool IsVisible => SelPawn.RaceProps.Humanlike && Prefs.DevMode;

        public ITab_Pawn_FitBitDebuger()
        {
            labelKey = "TabFitBitDebug";
            tutorTag = "FitBitDebug";

            size = Size;
        }

        public override void TabUpdate()
        {
            base.TabUpdate();
        }


        public override void FillTab()
        {
            if (Finder.StaminaTracker.TryGet(SelPawn, out var unit))
            {
                var yOffset = 10f;
                var tyOffset = yOffset;

                Text.Font = GameFont.Small;

                Widgets.DrawLine(new Vector2(10, yOffset), new Vector2(290f, yOffset), Color.white, 2f);
                yOffset += 5;
            
                Widgets.Label(new Rect(10, yOffset, 280f, 35), "Set body size");
                yOffset += 25;

                if (Widgets.ButtonText(new Rect(10, yOffset, 70, 30), "skinny"))
                    SelPawn.SetBodySize(BodyTypeDefOf.Thin);

                if (Widgets.ButtonText(new Rect(80, yOffset, 70, 30), "medium"))
                {
                    if (SelPawn.gender == Gender.Male)
                        SelPawn.SetBodySize(BodyTypeDefOf.Male);
                    else
                        SelPawn.SetBodySize(BodyTypeDefOf.Female);
                }

                if (Widgets.ButtonText(new Rect(150, yOffset, 70, 30), "hulk")) SelPawn.SetBodySize(BodyTypeDefOf.Hulk);

                if (Widgets.ButtonText(new Rect(220, yOffset, 70, 30), "fat")) SelPawn.SetBodySize(BodyTypeDefOf.Fat);

                yOffset += 35;

                Text.Font = GameFont.Tiny;

                Widgets.DrawLine(new Vector2(10, yOffset), new Vector2(290f, yOffset), Color.white, 2f);
                yOffset += 5;
                
                Widgets.Label(new Rect(220, yOffset, 70, 30), "Stamina Level" + unit.staminaLevel);
                yOffset += 25;
            }
            else
            {
                Widgets.Label(new Rect(10, 10, 100, 30), "No Fitness Component Found");
                Logging.Error("No Fitness Component Found");
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
        }

        public override void CloseTab()
        {
            base.CloseTab();
        }
    }
}