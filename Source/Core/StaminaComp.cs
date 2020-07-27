using PumpingSteel.Core.Capacities;
using PumpingSteel.Fitness;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Core
{
    public class StaminaComp : IFitnessComp<StaminaUnit>
    {
        private const float runningCost = -0.015f;
        private const float walkingCost = -0.015f;
        private const float restingCost = 0.035f;

        private const float breathCost = 0.015f;
        private const float breathCostMovingModifer = 0.015f;
        
        private bool IsMoving;
        private int lastPuff = 0;
        
        private StaminaMod oldMod;


        private IntVec3 position = IntVec3.Zero;

        public override void DoTickRare()
        {
            Unit.maxStaminaLevel = SelPawn.health.capacities.GetLevel(FitnessCapacitiesDefOf.StaminaCapacity) + Unit.staminaOffset;
            Unit.oldstaminaLevel = Unit.staminaLevel;

            oldMod = Unit.CurStaminaMod;

            StartStaminaUpdate();
        }

        public void StartStaminaUpdate()
        {
            IsMoving = SelPawn.pather.MovingNow &&
                       SelPawn.CurJobDef != JobDefOf.Wait_Combat &&
                       SelPawn.CurJobDef != JobDefOf.Wait_MaintainPosture &&
                       SelPawn.CurJobDef != JobDefOf.Wait &&
                       SelPawn.CurJobDef != JobDefOf.Wait_Wander &&
                       SelPawn.CurJobDef != JobDefOf.GotoWander;

            switch (Unit.CurStaminaMod)
            {
                case StaminaMod.Running:
                    Running();
                    break;
                case StaminaMod.Breathing:
                    Breathing();
                    break;
                case StaminaMod.Walking:
                    Walking();
                    break;
                default:
                    Resting();
                    break;
            }
        }

        private void Notify_ModChanged(StaminaMod oldMod)
        {
           
        }

        internal void Running()
        {
            if (0.15f <= Unit.staminaLevel && Unit.staminaLevel < (IsHuman ? 1 : 0.9) && IsMoving)
                Unit.CurStaminaMod = StaminaMod.Walking;
            if (0.15f <= Unit.staminaLevel && !IsMoving) Unit.CurStaminaMod = StaminaMod.Resting;
            if (Unit.staminaLevel < 0.15f) Unit.CurStaminaMod = StaminaMod.Breathing;

            FinalizeStaminaUpdate(runningCost - Mathf.Min(IsAnimal ? SelPawn.BodySize / 8 : 0f, 0.08f));
        }

        internal void Walking()
        {
            if (Unit.staminaLevel >= (IsHuman ? 1 : 0.9) && IsMoving) Unit.CurStaminaMod = StaminaMod.Running;
            if (Unit.staminaLevel >= 0.15f && !IsMoving) Unit.CurStaminaMod = StaminaMod.Resting;
            if (0.15f > Unit.staminaLevel) Unit.CurStaminaMod = StaminaMod.Breathing;

            FinalizeStaminaUpdate(walkingCost - Mathf.Min(IsAnimal ? SelPawn.BodySize / 8f : 0f, 0.06f));
        }

        internal void Breathing()
        {
            if (Unit.staminaLevel >= (IsHuman ? 1 : 0.9) && IsMoving) Unit.CurStaminaMod = StaminaMod.Running;
            if (Unit.staminaLevel >= 0.85 && IsMoving) Unit.CurStaminaMod = StaminaMod.Walking;
            if (Unit.staminaLevel >= 0.85 && !IsMoving) Unit.CurStaminaMod = StaminaMod.Resting;

            FinalizeStaminaUpdate(breathCost + (IsMoving ? breathCostMovingModifer : 0f));
        }

        internal void Resting()
        {
            if (Unit.staminaLevel >= (IsHuman ? 1 : 0.9) && IsMoving) Unit.CurStaminaMod = StaminaMod.Running;
            if (0.15f <= Unit.staminaLevel && Unit.staminaLevel <= (IsHuman ? 1 : 0.5) && IsMoving)
                Unit.CurStaminaMod = StaminaMod.Walking;

            FinalizeStaminaUpdate(restingCost - (SelPawn.CurJobDef == JobDefOf.GotoWander ? 0.04f : 0f));
        }

        private void FinalizeStaminaUpdate(float delta)
        {
            if (Unit.CurStaminaMod != oldMod) Notify_ModChanged(oldMod);

            Unit.staminaLevel = Mathf.Clamp(Unit.staminaLevel + delta, 0, Unit.maxStaminaLevel);

#if DEBUG && TRACE && SHITMYSELF
            if (Unit.DEBUG)
            {
                Log.TryOpenLogWindow();
                Logging.Line("pawn: " + SelPawn.Name.ToString() + " " + delta + " \t" + Unit.CurStaminaMod + "\t <- " +
                             oldMod);
                Logging.Line("stamina: " + Unit.staminaLevel + "\t " + Unit.oldstaminaLevel);
                Logging.Line(">-------- --------------- ---------<");
                Finder.TickManager.Pause();
            }
#endif
        }

        public override IFitnessTracker<StaminaUnit> GetTracker()
        {
            return Finder.StaminaTracker;
        }

        public override bool ShouldDisable()
        {
            return false;
        }
    }
}