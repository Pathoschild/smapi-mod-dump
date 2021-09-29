/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace XSPlus.Models
{
    using System;
    using StardewValley.Menus;
    using StardewValley.Objects;

    /// <summary>
    /// Arguments for ItemGrabMenu events.
    /// </summary>
    internal class ItemGrabMenuEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemGrabMenuEventArgs"/> class.
        /// </summary>
        /// <param name="itemGrabMenu">The ItemGrabMenu currently active or null.</param>
        /// <param name="chest">The Chest for the ItemGrabMenu or null.</param>
        public ItemGrabMenuEventArgs(ItemGrabMenu? itemGrabMenu, Chest? chest)
        {
            this.ItemGrabMenu = itemGrabMenu;
            this.Chest = chest;
        }

        /// <summary>
        /// Gets the ItemGrabMenu if it is the currently active menu.
        /// </summary>
        public ItemGrabMenu? ItemGrabMenu { get; }

        /// <summary>
        /// Gets the Chest for which the ItemGrabMenu was opened.
        /// </summary>
        public Chest? Chest { get; }
    }
}