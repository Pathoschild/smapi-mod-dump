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

namespace TehPers.CoreMod.Api.Items {
    public interface IItemComparer {
        /// <summary>Checks if an item is associated with a particular key.</summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="item">The item to compare against the key.</param>
        /// <returns>True if the item and key are associated, false otherwise.</returns>
        bool IsInstanceOf(in ItemKey key, Item item);
    }
}