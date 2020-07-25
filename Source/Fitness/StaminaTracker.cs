using System.Collections.Generic;
using RimWorld.Planet;
using Verse;

namespace PumpingSteel.Fitness
{
    /// <summary>
    /// FitnessTracker is WorldComponent used as container and shell for storing FitnessUnits for each pawn.
    /// It provide caching and scribing operation. 
    /// </summary>
    public class StaminaTracker : IFitnessTracker<StaminaUnit>
    {
        // ReSharper disable once InconsistentNaming
        private List<StaminaUnit> units = new List<StaminaUnit>();

        private Dictionary<int, StaminaUnit> UnitsPawnsPairs { get; } = new Dictionary<int, StaminaUnit>(10);

        /// <summary>
        /// Return all available FitnessUnits. 
        /// </summary>
        public IEnumerable<IFitnessUnit> Units => units;

        public StaminaTracker(World world) : base(world)
        {
            Finder.StaminaTracker = this;
        }

        /// <summary>
        /// Provide null safe and access to FitnessUnits.
        /// </summary>
        /// <param name="pawn">SelPawn</param>
        /// <param name="unit">Output FitnessUnit for said pawn</param>
        /// <returns>(bool) Found</returns>
        public override bool TryGet(Pawn pawn, out StaminaUnit unit)
        {
            switch (pawn)
            {
                case null:
                    unit = null;
                    return false;
                default:
                    return UnitsPawnsPairs.TryGetValue(pawn.thingIDNumber, out unit);
            }
        }

        /// <summary>
        /// Used to register a new pawn with the tracker.
        /// NOTE: field FitnessUnit.pawn must be not null and set to a pawn.
        /// </summary>
        /// <param name="unit"></param>
        public override void Register(StaminaUnit unit)
        {
            if (unit?.Pawn == null || units.Contains(unit)) return;

            units.Add(unit);
            UnitsPawnsPairs[unit.Pawn.thingIDNumber] = unit;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref units, false, "fitnessUnits", LookMode.Deep);

            // Check if the game is being loaded or saved.
            if (Scribe.mode != LoadSaveMode.PostLoadInit) return;

            // while loading,
            // Rebuilding the caching dict.
            UnitsPawnsPairs.Clear();
            foreach (var unit in units)
            {
                if (unit == null || unit?.Pawn == null) continue;
                UnitsPawnsPairs[unit.Pawn.thingIDNumber] = unit;
            }
        }
    }
}