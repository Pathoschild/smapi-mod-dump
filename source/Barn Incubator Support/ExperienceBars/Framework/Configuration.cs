/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ExperienceBars.Framework
{
    /// <summary>The mod's configuration options.</summary>
    internal class Configuration
    {
        /// <summary>The button which shows or hides the experience bars display.</summary>
        public SButton ToggleBars { get; set; } = SButton.X;

        /// <summary>The pixel position at which to draw the experience bars, relative to the top-left corner of the screen.</summary>
        public Point Position { get; set; } = new(10, 10);

        /// <summary>The base colors to use for the experience bars.</summary>
        public ModBaseColorsConfig BaseColors { get; set; } = new();

        /// <summary>The colors to use for each skill.</summary>
        public ModSkillColorsConfig SkillColors { get; set; } = new();
    }
}
