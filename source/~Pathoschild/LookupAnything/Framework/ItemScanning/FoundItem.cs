/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley;

namespace Pathoschild.Stardew.LookupAnything.Framework.ItemScanning
{
    /// <summary>An item found in the world.</summary>
    public class FoundItem
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The item instance.</summary>
        public Item Item { get; }

        /// <summary>Whether the item was found in the current player's inventory.</summary>
        public bool IsInInventory { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="item">The item instance.</param>
        /// <param name="isInInventory">Whether the item was found in the current player's inventory.</param>
        public FoundItem(Item item, bool isInInventory)
        {
            this.Item = item;
            this.IsInInventory = isInInventory;
        }
    }
}
