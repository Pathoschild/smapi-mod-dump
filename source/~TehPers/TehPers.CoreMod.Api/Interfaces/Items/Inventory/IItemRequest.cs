/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public interface IItemRequest {
        /// <summary>The quantity of this item request.</summary>
        int Quantity { get; }

        /// <summary>Whether an item matches this request.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item matches this request, false otherwise.</returns>
        bool Matches(Item item);
    }
}