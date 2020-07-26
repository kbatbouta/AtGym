using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PumpingSteel.Fitness;
using PumpingSteel.GymUI;
using Verse;

namespace PumpingSteel.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class H_Pawn_GetGizmo
    {
        private static Dictionary<int,Gizmo> _cashable = new Dictionary<int,Gizmo>();
        
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (Finder.StaminaTracker.TryGet(__instance, out StaminaUnit unit))
            {
                if (_cashable.TryGetValue(__instance.thingIDNumber, out Gizmo gizmo))
                    __result = __result.AddItem(gizmo);
                else
                {
                    _cashable[__instance.thingIDNumber] = new Gizmo_StaminaBar(unit);
                    _cashable[__instance.thingIDNumber].order = 0;    
                }
            }
        } 
    }
}