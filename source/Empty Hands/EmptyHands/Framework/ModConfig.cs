/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace EmptyHands.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keyboard input map.</summary>
        public InputMapConfiguration<SButton> Keyboard { get; set; }

        /// <summary>The controller input map.</summary>
        public InputMapConfiguration<SButton> Controller { get; set; }
    }
}
