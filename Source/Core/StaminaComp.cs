using PumpingSteel.Core.Capacities;
using PumpingSteel.Fitness;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Core
{
    public class StaminaComp : IFitnessComp<StaminaUnit>
    {
        private int ticks = 0;

        private StaminaMod oldMod;

        public override void DoTickRare()
        {
            oldMod = Unit.CurStaminaMod;

            StartStaminaUpdate();

            ticks++;
        }


        public void StartStaminaUpdate()
        {
            if (ticks % 4 == 0)
                Unit.maxStaminaLevel = SelPawn.health.capacities.GetLevel(FitnessCapacitiesDefOf.StaminaCapacity) +
                                       Unit.staminaOffset;

            var curJob = SelPawn.CurJobDef;

            switch (Unit.CurStaminaMod)
            {
                case StaminaMod.Running:
                    Running();
                    break;
                case StaminaMod.Walking when SelPawn.pather.MovingNow:
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
        }

        internal void Walking()
        {
        }

        internal void Breathing()
        {
        }

        internal void Resting()
        {
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
            if (Unit.DEBUG) Logging.Warning("Hey" + SelPawn.Name.ToStringFull);
        }

        public void Notify_StartedPath()
        {
            if (Unit.DEBUG) Logging.Warning("Hey" + SelPawn.Name.ToStringFull);
        }

        public void Notify_DestinationSet()
        {
            if (Unit.DEBUG) Logging.Warning("Hey" + SelPawn.Name.ToStringFull);
        }
    }
}