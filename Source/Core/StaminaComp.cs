using System;
using System.Collections.Generic;
using PumpingSteel.Core.Capacities;
using PumpingSteel.Core.Hediffs;
using PumpingSteel.Fitness;
using PumpingSteel.GymUI;
using PumpingSteel.Tools;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Core
{
    public class StaminaComp : IFitnessComp<StaminaUnit>
    {
        private bool IsMoving = false;

        private const float runningCost = -0.015f;
        private const float walkingCost = -0.015f;
        private const float restingCost = 0.035f;

        private const float breathCost = 0.015f;
        private const float breathCostMovingModifer = 0.015f;
        
        

        private IntVec3 position = IntVec3.Zero;

        private StaminaMod oldMod;

        public override void DoTickRare()
        {
            Unit.maxStaminaLevel = SelPawn.health.capacities.GetLevel(FitnessCapacitiesDefOf.StaminaCapacity);
            Unit.oldstaminaLevel = Unit.staminaLevel;

            oldMod = Unit.CurStaminaMod;

            StartStaminaUpdate();
        }

        public void StartStaminaUpdate()
        {
            IsMoving =  SelPawn.pather.MovingNow &&
                        (SelPawn.CurJobDef != JobDefOf.Wait_Combat) &&
                        (SelPawn.CurJobDef != JobDefOf.Wait_MaintainPosture) &&
                        (SelPawn.CurJobDef != JobDefOf.Wait) && 
                        (SelPawn.CurJobDef != JobDefOf.Wait_Wander);
            
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
            if (0.15f <= Unit.staminaLevel && Unit.staminaLevel < (IsHuman ? 1 : 0.5) && IsMoving)
                Unit.CurStaminaMod = StaminaMod.Walking;
            if (0.15f <= Unit.staminaLevel && !IsMoving) Unit.CurStaminaMod = StaminaMod.Resting;
            if (Unit.staminaLevel < 0.15f) Unit.CurStaminaMod = StaminaMod.Breathing;

            FinalizeStaminaUpdate(runningCost - (IsAnimal ? SelPawn.BodySize / 4 : 0));
        }

        internal void Walking()
        {
            if (Unit.staminaLevel >= (IsHuman ? 1 : 0.5) && IsMoving) Unit.CurStaminaMod = StaminaMod.Running;
            if (Unit.staminaLevel >= 0.15f && !IsMoving) Unit.CurStaminaMod = StaminaMod.Resting;
            if (0.15f > Unit.staminaLevel) Unit.CurStaminaMod = StaminaMod.Breathing;

            FinalizeStaminaUpdate(walkingCost);
        }

        internal void Breathing()
        {
            if (Unit.staminaLevel >= (IsHuman ? 1 : 0.85) && IsMoving) Unit.CurStaminaMod = StaminaMod.Running;
            if (Unit.staminaLevel >= 0.85 && IsMoving) Unit.CurStaminaMod = StaminaMod.Walking;
            if (Unit.staminaLevel >= 0.85 && !IsMoving) Unit.CurStaminaMod = StaminaMod.Resting;

            FinalizeStaminaUpdate(breathCost + (IsMoving ? breathCostMovingModifer : 0f));
        }

        internal void Resting()
        {
            if (Unit.staminaLevel >= (IsHuman ? 1 : 0.5) && IsMoving) Unit.CurStaminaMod = StaminaMod.Running;
            if (0.15f <= Unit.staminaLevel && Unit.staminaLevel <= (IsHuman ? 1 : 0.5) && IsMoving)
                Unit.CurStaminaMod = StaminaMod.Walking;

            FinalizeStaminaUpdate(restingCost);
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