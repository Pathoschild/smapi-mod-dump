/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace RecatchLegendaryFish.Framework
{
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A keybind which toggles whether the player can recatch fish.</summary>
        public KeybindList ToggleKey { get; set; } = new();
    }
}
