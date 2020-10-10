/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public interface IInventory : IEnumerable<Item> {
        /// <summary>Adds as much of an item to this inventory as possible.</summary>
        /// <param name="item">The item to add to this inventory.</param>
        /// <returns>The remaining item.</returns>
        Item Add(Item item);

        /// <summary>Removes an item from this inventory. Items are compared by reference.</summary>
        /// <param name="item">The item to remove from this inventory.</param>
        /// <returns>True if removed, false otherwise.</returns>
        bool Remove(Item item);

        /// <summary>Tries to remove all of the requested items. Items are only removed if they can all be removed.</summary>
        /// <param name="items">The item requests which must be fulfilled.</param>
        /// <param name="remaining">If failed, the amount of each request that could not be fulfilled. This may or may not contain entries for requests that can be fulfilled. Use <c>out _</c> to ignore this result.</param>
        /// <returns>True if the items were removed, false otherwise.</returns>
        bool TryRemoveAll(IEnumerable<IItemRequest> items, out IDictionary<IItemRequest, int> remaining);

        /// <summary>Checks whether this inventory can fulfill all of the given item requests.</summary>
        /// <param name="items">The item requests to check for.</param>
        /// <returns>True if all the item requests can be fulfilled, false otherwise.</returns>
        bool Contains(params IItemRequest[] items);

        /// <summary>Checks whether this inventory can fulfill all of the given item requests.</summary>
        /// <param name="items">The item requests to check for.</param>
        /// <returns>True if all the item requests can be fulfilled, false otherwise.</returns>
        bool Contains(IEnumerable<IItemRequest> items);
    }
}