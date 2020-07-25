using System;
using System.Collections.Generic;
using HarmonyLib;
using PumpingSteel.Fitness;
using PumpingSteel.Tools.Scribing;
using RimWorld;
using Verse;

namespace PumpingSteel
{
    public static class Finder
    {
        public static readonly string packageID = "krk.atgym";

        public static readonly HashSet<StatDef> CachedAffectedStats = new HashSet<StatDef>();

        public static Harmony Harmony;

        public static FitnessTracker FitnessTracker;
        public static StaminaTracker StaminaTracker;

        public static ScribeManager ScribeManager;

        public static bool DEBUG = false;

        private static TickManager tickManager;


        public static int GameTicks => TickManager.TicksGame;

        public static TickManager TickManager
        {
            get
            {
                if (tickManager != null) return tickManager;

                tickManager = Find.TickManager;
                return tickManager;
            }
        }
    }
}