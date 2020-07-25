using System;
using UnityEngine;
using Verse;

namespace PumpingSteel.Fitness
{
    public enum StaminaMod
    {
        Running = 0,
        Walking = 1,
        Breathing = 2,
        Resting = 3,
        Nothing = 4
    }

    public class StaminaUnit : IFitnessUnit
    {
        public float staminaLevel;
        public float oldstaminaLevel = 1.0f;
        
        public float maxStaminaLevel = 1.0f;

        public StaminaMod CurStaminaMod = StaminaMod.Nothing;

        public override string LoadPostfix => "stamina";

        public StaminaUnit()
        {
        }

        public StaminaUnit(Pawn pawn) : base(pawn)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref staminaLevel, "unitStaminaLevel");
            Scribe_Values.Look(ref oldstaminaLevel, "unitOldStaminaLevel");
            Scribe_Values.Look(ref maxStaminaLevel, "unitMaxStaminaLevel");
            Scribe_Values.Look(ref CurStaminaMod, "unitStaminaMod");
        }
    }
}