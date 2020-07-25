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
        private StaminaMod oldMod;
        
        public override void DoTickRare()
        {
            Unit.maxStaminaLevel = SelPawn.health.capacities.GetLevel(FitnessCapacitiesDefOf.StaminaCapacity);
            Unit.oldstaminaLevel = Unit.staminaLevel; 
            
            oldMod = Unit.CurStaminaMod;
            
            DoStaminaModTick();
        }

        private void DoStaminaModTick()
        {
            switch (Unit.CurStaminaMod)
            {
                case StaminaMod.Running: Running();
                    break;
                case StaminaMod.Breathing: Breathing();
                    break;
                case StaminaMod.Walking: Walking();
                    break;
                default:
                    Resting(); 
                    break;
            }
        }

        private void Notify_ModChanged(StaminaMod oldMod)
        {
            if (Unit.CurStaminaMod == StaminaMod.Breathing)
            {
                BodyUtilities.AdjustSeverity(SelPawn, FitnessHediffsDefOf.OutOfBreath,2.0f);   
            }

            if (oldMod == StaminaMod.Breathing)
            {
                BodyUtilities.AdjustSeverity(SelPawn, FitnessHediffsDefOf.OutOfBreath,1.0f);
            }
        }

        internal void Running()
        {
            if ((Unit.staminaLevel < 1.0f && IsHuman) || (Unit.staminaLevel < 0.5f &&  IsAnimal)) Unit.CurStaminaMod = StaminaMod.Walking;
            
            if (Unit.CurStaminaMod != oldMod) Notify_ModChanged(oldMod);
            else
            {
                Unit.staminaLevel = Mathf.Clamp(Unit.staminaLevel - 0.035f, 0, Unit.maxStaminaLevel);
            }
        }

        internal void Walking()
        {
            if (Unit.staminaLevel > 0.15f && !SelPawn.pather.Moving) Unit.CurStaminaMod = StaminaMod.Resting;
            if (Unit.staminaLevel <= 0.05f + SelPawn.BodySize / 4.0f) Unit.CurStaminaMod = StaminaMod.Breathing;
            
            if (Unit.CurStaminaMod != oldMod) Notify_ModChanged(oldMod);
            else
            {
                var modifer = 0f;
                if (IsMoving()) modifer = -0.015f;
                Unit.staminaLevel = Mathf.Clamp(Unit.staminaLevel - 0.005f + modifer, 0, Unit.maxStaminaLevel);
            }
        }

        internal void Breathing()
        {
            if (Unit.staminaLevel > 0.85f && !SelPawn.pather.Moving) Unit.CurStaminaMod = StaminaMod.Resting;
            if (Unit.staminaLevel > 1.00f && SelPawn.pather.Moving) Unit.CurStaminaMod = StaminaMod.Running;
            
            if (Unit.CurStaminaMod != oldMod) Notify_ModChanged(oldMod);
            else
            {
                var modifer = 0f;
                if (!SelPawn.pather.Moving) modifer = 0.015f;
                Unit.staminaLevel = Mathf.Clamp(Unit.staminaLevel + 0.015f + modifer, 0, Unit.maxStaminaLevel);
               
            }
        }

        internal void Resting()
        {
            if (SelPawn.pather.Moving) Unit.CurStaminaMod = StaminaMod.Running;
            
            if (Unit.CurStaminaMod != oldMod) Notify_ModChanged(oldMod);
            else
            {
                Unit.staminaLevel = Mathf.Clamp(Unit.staminaLevel + 0.035f, 0, Unit.maxStaminaLevel);
            }
        }

        private bool IsMoving()
        {
            return SelPawn.pather.Moving &&
                   (SelPawn.CurJob?.def != JobDefOf.Wait_Wander && SelPawn.CurJob?.def != JobDefOf.GotoWander);
        }

        public override IFitnessTracker<StaminaUnit> GetTracker()
        {
            return Finder.StaminaTracker;
        }

        public override bool ShouldDisable()
        {
            return false;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Gizmo_StaminaBar(Unit, this);
        }
    }
}