/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

namespace ExpandedStorage
{
    internal class ModConfig
    {
        /// <summary>Whether to allow modded storage to have capacity other than 36 slots.</summary>
        public bool AllowModdedCapacity { get; set; } = true;

        /// <summary>Expands Vanilla Chests to hold double their inventory (72 slots).</summary>
        public bool ExpandVanillaChests { get; set; } = true;
        
        /// <summary>Adds three extra rows to the Inventory Menu.</summary>
        public bool ExpandInventoryMenu { get; set; } = true;
        
        /// <summary>Adds clickable arrows to indicate when there are more items in the chest.</summary>
        public bool ShowOverlayArrows { get; set; } = true;

        /// <summary>Allows filtering Inventory Menu by searching for the the item name.</summary>
        public bool ShowSearchBar { get; set; } = true;
    }
}