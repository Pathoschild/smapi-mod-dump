using Microsoft.Xna.Framework.Input;

namespace RelationshipTracker
{
    class ModConfig
    {
        public enum DatableType
        {
            Bachelor,
            Bachelorette
        }
        public Keys activateKey { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public DatableType datableType { get; set; }
        public bool showPortrait { get; set; }
        public bool drawBackground { get; set; }
        public float backgroundOpacity { get; set; }
        public bool showGifts { get; set; }

        public ModConfig()
        {
            activateKey = Keys.R;
            offsetX = 2;
            offsetY = 112;
            datableType = DatableType.Bachelorette;
            showPortrait = true;
            drawBackground = true;
            backgroundOpacity = 1.0f;
            showGifts = false;
        }
    }
}
