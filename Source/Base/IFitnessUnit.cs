using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace PumpingSteel.Fitness
{
    public abstract class IFitnessUnit : IExposable, ILoadReferenceable
    {
        public abstract string LoadPostfix { get; }

        public int loadID;

        protected Pawn pawn;

        public BodyTypeDef BodyType => pawn.story.bodyType;

        public Pawn Pawn
        {
            get => pawn;
            set
            {
                if (pawn != null)
                    pawn = value;
                else
                    throw new Exception("Tried using a null pawn for a fitness unit");
            }
        }

        public IFitnessUnit()
        {
        }

        public IFitnessUnit(Pawn pawn)
        {
            this.pawn = pawn;
            loadID = pawn.thingIDNumber;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref loadID, "LoadId_" + LoadPostfix);
            Scribe_References.Look(ref pawn, "unitPawn_" + LoadPostfix, false);
        }

        public string GetUniqueLoadID()
        {
            return "fitnessUnit_" + loadID + "_" + LoadPostfix;
        }
    }
}