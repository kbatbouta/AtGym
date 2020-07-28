#region

using Verse;

#endregion

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
        public StaminaMod CurStaminaMod = StaminaMod.Nothing;

        public Extras extras = new Extras();

        public float maxStaminaLevel = 1.0f;
        public float oldStaminaLevel = 1.0f;

        public float staminaLevel = 1.0f;
        public float staminaOffset =  Rand.Range(0f, 1.0f);
        
        public float speedModifier = 1f;
        public float speedOffset = 0.0f;
        public float breathing = 1.0f;
        public float bloodPumping = 1.0f;

        public float meleeMofidier = 1.0f;
        public float hungerMofidier = 1.0f;

        public StaminaUnit()
        {
        }

        public StaminaUnit(Pawn pawn) : base(pawn)
        {
        }

        public override string LoadPostfix => "stamina";

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref staminaLevel, "unitStaminaLevel");
            Scribe_Values.Look(ref oldStaminaLevel, "unitOldStaminaLevel");
            Scribe_Values.Look(ref maxStaminaLevel, "unitMaxStaminaLevel");
            Scribe_Values.Look(ref CurStaminaMod, "unitStaminaMod");
            Scribe_Values.Look(ref staminaOffset, "unitStaminaOffset");
            Scribe_Values.Look(ref staminaOffset, "unitStaminaXP");
            
            Scribe_Values.Look(ref speedOffset, "unitMoveSpeedOffset");
            Scribe_Values.Look(ref speedModifier, "unitMoveSpeedModifier");
        }

        public class Extras
        {
            public int DamageAlertCountDown = 0;
            public int DangerAlertCountDown = 0;

            public int GUIlastTick = -1;
        }
    }
}