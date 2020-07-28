#region

using System.Runtime.InteropServices;
using PumpingSteel.Core.Capacities;
using PumpingSteel.Fitness;
using RimWorld;
using UnityEngine;

#endregion

namespace PumpingSteel.Core
{
    public class StaminaComp : IFitnessComp<StaminaUnit>
    {
        private const float runningOffset = -0.025f;
        private const float walkingOffset = -0.015f;
        
        private const float restingOffset = 0.015f;
        private const float sleepingOffset = 0.015f;
        private const float wanderingOffset = -0.010f;
        
        private const float tiredOffset = 0.035f;
        private const float tiredWanderOffset = -0.0025f;

        private const float animalRunningLimit = 0.5f;
        private const float humansRunningLimit = 1.01f;

        private const float fatSpeedModifier = 0.65f;
        private const float thinSpeedModifier = 1.3f;
        private const float hulkSpeedModifier = 0.8f;

        private const float runningSpeedOffset = 0.5f;
        private const float walkingSpeedOffset = 0.0f;
        private const float tiredSpeedOffset = -0.3f;

        private const float smallBodySizeSpeedOffset = 0.2f;

        private StaminaMod oldMod;
        private int ticks;

        private bool moving = false;

        public override void DoTickRare()
        {
            oldMod = Unit.CurStaminaMod;

            UpdateCapacities();
            StartStaminaUpdate();
            
            UpdateSpeedOffsets();
            UpdateSpeedModifiers();
            
            ticks++;
        }

        private void UpdateCapacities()
        {
            if (ticks % 4 == 0)
                Unit.maxStaminaLevel = SelPawn.health.capacities.GetLevel(FitnessCapacitiesDefOf.StaminaCapacity) * 0.5f + Unit.bloodPumping * 0.5f +
                                       Unit.staminaOffset;
            else if ((ticks + 1) % 4 == 0)
                Unit.breathing = SelPawn.health.capacities.GetLevel(PawnCapacityDefOf.Breathing);
            else if ((ticks + 2) % 4 == 0)
                Unit.bloodPumping = SelPawn.health.capacities.GetLevel(PawnCapacityDefOf.BloodPumping);
        }
        
        public void StartStaminaUpdate()
        {
            var curJob = SelPawn.CurJobDef;
            
            switch (Unit.CurStaminaMod)
            {
                case StaminaMod.Running when moving && Unit.staminaLevel > 0.05f:
                    Running();
                    break;
                case StaminaMod.Walking when moving &&  Unit.staminaLevel > 0.05f:
                    Walking();
                    break;
                case StaminaMod.Breathing:
                    Breathing();
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
            if (moving && Unit.staminaLevel < (IsHuman ? humansRunningLimit : animalRunningLimit)) 
                Unit.CurStaminaMod = StaminaMod.Walking;
            if (moving && Unit.staminaLevel < 0.05f)
                Unit.CurStaminaMod = StaminaMod.Breathing;
            
            FinalizeStaminaUpdate(runningOffset);
        }

        internal void Walking()
        {
            if (moving && Unit.staminaLevel > (IsHuman ? humansRunningLimit : animalRunningLimit)) 
                Unit.CurStaminaMod = StaminaMod.Running;
            if (moving && Unit.staminaLevel < 0.05f) 
                Unit.CurStaminaMod = StaminaMod.Breathing;
            
            FinalizeStaminaUpdate(walkingOffset);
        }

        internal void Breathing()
        {
            var shouldChange = Unit.staminaLevel >= (IsHuman ? humansRunningLimit : animalRunningLimit);
            
            if(shouldChange && Unit.staminaLevel > (IsHuman ? humansRunningLimit : animalRunningLimit))
                Unit.CurStaminaMod = StaminaMod.Running;
            else if(shouldChange)
                Unit.CurStaminaMod = StaminaMod.Walking;
            
            if (Unit.CurStaminaMod != StaminaMod.Breathing) return;
            FinalizeStaminaUpdate(tiredOffset * Unit.bloodPumping * Unit.breathing + (moving ? tiredWanderOffset : 0f));
        }

        internal void Resting()
        {
            if (!moving && Unit.staminaLevel > 0.05f) 
                Unit.CurStaminaMod = StaminaMod.Resting;
            
            if (moving && Unit.staminaLevel > (IsHuman ? humansRunningLimit : animalRunningLimit)) 
                Unit.CurStaminaMod = StaminaMod.Running;
            if (moving && (IsHuman ? humansRunningLimit : animalRunningLimit) >= Unit.staminaLevel) 
                Unit.CurStaminaMod = StaminaMod.Walking;
            
            if(Unit.staminaLevel <= 0.05f)
                Unit.CurStaminaMod = StaminaMod.Breathing;
            
            if (Unit.CurStaminaMod != StaminaMod.Resting) return;
            FinalizeStaminaUpdate(restingOffset * Unit.bloodPumping * Unit.breathing + (SelPawn.pather.Moving ? wanderingOffset : 0f) 
                                                                 + 
                                                                 (SelPawn?.CurJob?.def?.driverClass == typeof(JobDriver_LayDown) ? sleepingOffset : 0f));
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
                Logging.Line("stamina: " + _cashedStaminaLevel + "\t " + Unit.oldStaminaLevel);
                Logging.Line(">-------- --------------- ---------<");
                Finder.TickManager.Pause();
            }
#endif
            // lose stamina capacity 0.1 over 16 hours
            if(ticks % 20 != 0) return;
            Unit.staminaOffset -= 0.0001f;
        }

        private void UpdateSpeedModifiers()
        {
            if (IsHuman)
            {
                if (SelPawn.story.bodyType == BodyTypeDefOf.Fat)
                {
                    Unit.speedModifier = fatSpeedModifier;
                    Unit.meleeMofidier = 1.4f;
                }
                else if (SelPawn.story.bodyType == BodyTypeDefOf.Hulk)
                {
                    Unit.speedModifier = hulkSpeedModifier;
                    Unit.meleeMofidier = 1.8f;
                }
                else if (SelPawn.story.bodyType == BodyTypeDefOf.Thin)
                {
                    Unit.speedModifier = thinSpeedModifier;
                    Unit.meleeMofidier = 0.7f;
                }
            }
        }
        
        private void UpdateSpeedOffsets()
        {
            Unit.speedOffset = (bodySize < 0.8 ? smallBodySizeSpeedOffset : 0f);
            
            switch (Unit.CurStaminaMod)
            {
                case StaminaMod.Running:
                    Unit.speedOffset += runningSpeedOffset;
                    break;
                case StaminaMod.Walking:
                    Unit.speedOffset += walkingSpeedOffset;
                    break;
                case StaminaMod.Breathing:
                    Unit.speedOffset += tiredSpeedOffset;
                    break;
            }
        }

        public override IFitnessTracker<StaminaUnit> GetTracker()
        {
            return Finder.StaminaTracker;
        }

        public override bool ShouldDisable()
        {
            return false;
        }

        public void Notify_DestinationChanged()
        {
            if (Unit.DEBUG) Logging.Warning("Hey" + 
                                            SelPawn.Name.ToStringFull);
            SetMovingTrue(); DoTickRare();
        }

        public void Notify_StartedPath()
        {
            if (Unit.DEBUG) Logging.Warning("Hey" + 
                                            SelPawn.Name.ToStringFull);
            SetMovingTrue();
        }

        public void Notify_DestinationSet()
        {
            if (Unit.DEBUG) Logging.Warning("Hey" + 
                                            SelPawn.Name.ToStringFull);
            SetMovingTrue();
        }

        public void Notify_Stopped()
        {
            if (Unit.DEBUG) Logging.Warning("Hey" + 
                                            SelPawn.Name.ToStringFull);
            moving = false;
        }

        private void SetMovingTrue()
        {
            moving = SelPawn?.CurJob?.def != JobDefOf.GotoWander && 
                     SelPawn?.CurJob?.def != JobDefOf.Wait_Downed &&
                     SelPawn?.CurJob?.def != JobDefOf.Wait &&
                     SelPawn?.CurJob?.def != JobDefOf.Wait_Wander && 
                     SelPawn?.CurJob?.def != JobDefOf.Wait_Combat &&
                     SelPawn?.CurJob?.def.driverClass != typeof(JobDriver_GoForWalk);
        }
    }
}