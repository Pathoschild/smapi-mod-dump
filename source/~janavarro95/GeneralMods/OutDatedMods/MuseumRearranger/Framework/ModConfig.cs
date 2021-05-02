/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewModdingAPI;

namespace Omegasis.MuseumRearranger.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>The key which shows the museum rearranging menu.</summary>
        public SButton ShowMenuKey { get; set; } = SButton.R;

        /// <summary>The key which toggles the inventory box when the menu is open.</summary>
        public SButton ToggleInventoryKey { get; set; } = SButton.T;
    }
}
