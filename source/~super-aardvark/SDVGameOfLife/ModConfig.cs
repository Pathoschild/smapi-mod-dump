/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/super-aardvark/AardvarkMods-SDV
**
*************************************************/

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