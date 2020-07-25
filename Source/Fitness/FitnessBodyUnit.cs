using UnityEngine;
using Verse;

namespace PumpingSteel.Fitness
{
    public class FitnessBodyUnit : IFitnessUnit
    {
        public override string LoadPostfix => "fitness";

        public FitnessBodyUnit()
        {
        }

        public FitnessBodyUnit(Pawn pawn) : base(pawn)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}