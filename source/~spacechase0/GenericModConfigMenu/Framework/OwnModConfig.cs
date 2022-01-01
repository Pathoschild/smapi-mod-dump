/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace GenericModConfigMenu.Framework
{
    /// <summary>The mod configuration for Generic Mod Config Menu itself.</summary>
    internal class OwnModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A keybind which opens the menu.</summary>
        public KeybindList OpenMenuKey = new KeybindList(StardewModdingAPI.SButton.None);

        /// <summary>The number of field rows to offset when scrolling a config menu.</summary>
        public int ScrollSpeed { get; set; } = 120;
    }
}
