using StardewModdingAPI;

namespace SDVLife
{
    internal class ModConfig
    {
        /// <summary>The key which opens the item spawner menu.</summary>
        public SButton ModKey { get; set; } = SButton.L;

        public int Speed { get; set; } = 3;

        public bool HoeDirt { get; set; } = false;
        public bool DestroyGrass { get; set; } = false;
        public bool DestroyStumps { get; set; } = false;
        public bool DestroyImmatureTrees { get; set; } = false;
        public bool DestroyMatureTrees { get; set; } = false;
        public bool DestroyWeeds { get; set; } = false;
        public bool DestroyTwigs { get; set; } = false;
        public bool DestroyRocks { get; set; } = false;
        public bool HoeUnderObjects { get; set; } = false;
        public bool DestroyEverything { get; set; } = false;
    }
}