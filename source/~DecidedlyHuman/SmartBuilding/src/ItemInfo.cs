/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace SmartBuilding
{
    /// <summary>
    ///     Basic Item metadata.
    /// </summary>
    public struct ItemInfo
    {
        /// <summary>
        ///     A reference to the item to be placed.
        /// </summary>
        public Item Item;

        /// <summary>
        ///     The basic type of item that it is, determined by <see cref="ModEntry.IdentifyItemType" />
        /// </summary>
        public ItemType ItemType;

        /// <summary>
        ///     Whether this item is destined to be inserted into a machine.
        /// </summary>
        public bool ToBeInserted;

        /// <summary>
        ///     Whether this item is a DGA item.
        /// </summary>
        public bool IsDgaItem;
    }
}
