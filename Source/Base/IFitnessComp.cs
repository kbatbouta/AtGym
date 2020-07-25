using System;
using PumpingSteel.Fitness;
using PumpingSteel.Tools;
using Verse;

namespace PumpingSteel.Core
{
    public abstract class IFitnessComp<T> : ThingComp where T : IFitnessUnit
    {
        private bool disabled = false;
        private bool initialized = false;
        
        private int tick;

        private T unit;

        private bool animal = false;
        private bool human = false;

        public Pawn SelPawn { get; set; }
        
        public float bodySize;

        public T Unit
        {
            get
            {
                if (unit == null) GetTracker().TryGet(SelPawn, out unit);
                return unit;
            }
            set => unit = value;
        }

        public bool IsAnimal => animal;

        public bool IsHuman => human;

        public abstract void DoTickRare();

        public abstract bool ShouldDisable();

        public override void CompTick()
        {
            base.CompTick();
            if (tick++ % 30  != 0) return;
            
            if (disabled || !parent.Spawned) return;

            if (!initialized && parent.Spawned)
            {
                initialized = true;
                Register();
            }
            else
            {
                DoTickRare();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            Register();
        }

        public abstract IFitnessTracker<T> GetTracker();

        private void Register()
        {
            if (disabled) return;

            SelPawn = parent as Pawn;
            bodySize = SelPawn?.BodySize ?? 0.15f;
            
            tick = parent.thingIDNumber;

            // Disable for mechanized pawn.
            if (SelPawn != null && !SelPawn?.RaceProps?.IsFlesh == true)
            {
                disabled = true;
                return;
            }

            if (SelPawn != null && SelPawn.RaceProps.Animal) animal = true;
            if (SelPawn != null && SelPawn.RaceProps.Humanlike) human = true;

            if (ShouldDisable())
            {
                disabled = true;
                return;
            }

            if (GetTracker().TryGet(SelPawn, out unit)) return;

            unit = (T) Activator.CreateInstance(typeof(T), new[] {SelPawn});

            GetTracker().Register(unit);

            if (unit != null) initialized = true;
        }
    }
}