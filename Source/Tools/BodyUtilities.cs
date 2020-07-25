using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PumpingSteel.Core.Hediffs;
using PumpingSteel.Fitness;
using RimWorld;
using Verse;

namespace PumpingSteel.Tools
{
    public static class BodyUtilities
    {
        private static Dictionary<int, Hediff> _hediffs = new Dictionary<int, Hediff>();
        
        public static void SetBodySize(this Pawn pawn, BodyTypeDef bodyTypeDef)
        {
            pawn.story.bodyType = bodyTypeDef;
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();

            if (pawn.Spawned) RepairBody(pawn);

            SetBodyHediff(pawn);
        }

        public static void SetBodyHediff(Pawn pawn, int curStaminaPercentage = 1)
        {
            if (curStaminaPercentage == -1) return;

            AdjustSeverity(pawn, FitnessHediffsDefOf.OutOfBreath, curStaminaPercentage);
            RepairBody(pawn);
        }

        public static void RepairBody(Pawn pawn, Hediff hdiff = null)
        {
            if (hdiff != null && pawn.story.bodyType == BodyTypeDefOf.Fat && hdiff?.Severity != 1)
                AdjustSeverity(pawn, FitnessHediffsDefOf.Fitness, 1, hdiff);

            if (hdiff != null && pawn.story.bodyType == BodyTypeDefOf.Hulk && hdiff?.Severity != 3)
                AdjustSeverity(pawn, FitnessHediffsDefOf.Fitness, 3, hdiff);

            if (hdiff != null && pawn.story.bodyType == BodyTypeDefOf.Thin && hdiff?.Severity != 4)
                AdjustSeverity(pawn, FitnessHediffsDefOf.Fitness, 4, hdiff);

            if (pawn != null && (pawn.story.bodyType == BodyTypeDefOf.Male ||
                                 pawn.story.bodyType == BodyTypeDefOf.Female && hdiff?.Severity != 2))
                AdjustSeverity(pawn, FitnessHediffsDefOf.Fitness, 2, hdiff);
        }

        public static void AdjustSeverity(Pawn pawn,
            HediffDef hdDef,
            float severity,
            Hediff hdiff = null)
        {
            if (hdiff != null)
                hdiff.Severity = severity;
            else if (_hediffs.TryGetValue(pawn.thingIDNumber, out hdiff))
            {
                if (hdiff != null)
                {
                    hdiff.Severity = severity; return;
                }
            }

            var firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hdDef);
            if (firstHediffOfDef != null)
            {
                firstHediffOfDef.Severity = severity;
            }
            else
            {
                firstHediffOfDef = HediffMaker.MakeHediff(hdDef, pawn);
                firstHediffOfDef.Severity = severity;
                pawn.health.AddHediff(firstHediffOfDef);
            }
            
            _hediffs.Add(pawn.thingIDNumber, firstHediffOfDef);
        }
    }
}