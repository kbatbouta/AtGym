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
        public StaminaMod CurStaminaMod = StaminaMod.Nothing;

        public float maxStaminaLevel = 1.0f;
        public float oldstaminaLevel = 1.0f;
        public float staminaOffset = 0.0f;
        public float staminaLevel;
        
        public int DamageAlertCountDown = 0;
        public int DangerAlertCountDown = 0;
        

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
            Scribe_Values.Look(ref oldstaminaLevel, "unitOldStaminaLevel");
            Scribe_Values.Look(ref maxStaminaLevel, "unitMaxStaminaLevel");
            Scribe_Values.Look(ref CurStaminaMod, "unitStaminaMod");
            Scribe_Values.Look(ref staminaOffset, "unitStaminaOffset");
        }
    }
}