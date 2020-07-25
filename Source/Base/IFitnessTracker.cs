using System.Collections.Generic;
using RimWorld.Planet;
using Verse;

namespace PumpingSteel.Fitness
{
    /// <summary>
    /// FitnessTracker is WorldComponent used as container and shell for storing FitnessUnits for each pawn.
    /// It provide caching and scribing operation. 
    /// </summary>
    public abstract class IFitnessTracker<T> : WorldComponent where T : IFitnessUnit
    {
        /// <summary>
        /// Provide null safe and access to FitnessUnits.
        /// </summary>
        /// <param name="pawn">SelPawn</param>
        /// <param name="unit">Output FitnessUnit for said pawn</param>
        /// <returns>(bool) Found</returns>
        public abstract bool TryGet(Pawn pawn, out T unit);

        /// <summary>
        /// Used to register a new pawn with the tracker.
        /// NOTE: field FitnessUnit.pawn must be not null and set to a pawn.
        /// </summary>
        /// <param name="unit"></param>
        public abstract void Register(T unit);

        protected IFitnessTracker(World world) : base(world)
        {
        }
    }
}