/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace ExperienceBars.Framework
{
    /// <summary>The colors to use for the general experience bars UI.</summary>
    internal class ModBaseColorsConfig
    {
        /// <summary>The color of the border around each bar.</summary>
        public Color BarBorder { get; set; } = Color.DarkGoldenrod;

        /// <summary>The color of the bar background.</summary>
        public Color BarBackground { get; set; } = Color.Black;

        /// <summary>The color of the tick lines over the background color.</summary>
        public Color BarBackgroundTick { get; set; } = new(50, 50, 50);

        /// <summary>The color of the tick lines over the foreground color.</summary>
        public Color BarForegroundTick { get; set; } = new(120, 120, 120);
    }
}
