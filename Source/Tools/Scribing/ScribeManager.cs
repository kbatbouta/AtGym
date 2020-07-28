#region

using Verse;

#endregion

namespace PumpingSteel.Tools.Scribing
{
    public class ScribeManager : GameComponent
    {
        private int idCounter;

        public ScribeManager(Game game)
        {
            Finder.ScribeManager = this;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
            }
        }

        public int GenNextUniqueID()
        {
            return idCounter++;
        }

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
            }

            Scribe_Values.Look(ref idCounter, "idCounter");
        }
    }
}