#region

using HarmonyLib;

#endregion

namespace PumpingSteel.Patches
{
    public static class HPatcher
    {
        public static void Initialize()
        {
            Finder.Harmony = new Harmony(Finder.packageID);
            Finder.Harmony.PatchAll();

            H_Toils_Ingest.Patch();
        }
    }
}