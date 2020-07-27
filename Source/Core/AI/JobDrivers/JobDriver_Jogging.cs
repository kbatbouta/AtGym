using System.Collections.Generic;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;
using Verse.AI;

namespace PumpingSteel.Core.AI.ThinkDefs.JobDrivers
{
    public class JobDriver_Jogging : JobDriver
    {
        private static StaminaUnit _staminaUnit;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return Finder.StaminaTracker.TryGet(pawn, out _staminaUnit);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => pawn.needs.food.Starving);
            yield return Toils_General.Do(()=>
            {
                if (pawn.gender == Gender.Male)
                {
                    
                }
                else
                {
                    
                }
            });
            
            yield return GotoCell(TargetIndex.A, PathEndMode.Touch);
            if (Rand.Chance(0.5f)) yield return Toils_General.Wait(60);
            
            yield return GotoCell(TargetIndex.B, PathEndMode.Touch);
            if (Rand.Chance(0.75f)) yield return Toils_General.Wait(60);
            // Go back to where you started
            if (Rand.Chance(0.75f)) yield return GotoCell(TargetIndex.C, PathEndMode.Touch);
        }
        
        public static Toil GotoCell(TargetIndex ind, PathEndMode peMode)
        {
            Toil toil = new Toil();
            
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                actor.pather.StartPath(actor.jobs.curJob.GetTarget(ind), peMode);
            };
            
            toil.tickAction = delegate
            {
                _staminaUnit.staminaOffset += 0.0002f;
            };
            
            toil.socialMode = RandomSocialMode.SuperActive;
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedOrNull(ind);
            return toil;
        }

    }
}