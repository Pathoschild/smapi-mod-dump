/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

namespace TaintedCellar
{
    /// <summary>The mod configuration.</summary>
    public class CellarConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Only unlock the underground cellar after the house is fully upgraded.</summary>
        public bool OnlyUnlockAfterFinalHouseUpgrade { get; set; }

        /// <summary>Show the cellar entrance on the right side of the house instead of the left.</summary>
        public bool FlipCellarEntrance { get; set; }

        /// <summary>An X offset applied to the entrance position.</summary>
        public int XPositionOffset { get; set; }

        /// <summary>A Y offset applied to the entrance position.</summary>
        public int YPositionOffset { get; set; }
    }
}
