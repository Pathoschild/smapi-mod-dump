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
    public interface IItemCreator {
        /// <summary>Tries to create an instance of the specified item.</summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="item">The created item, if successful, with a stack size of 1.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryCreate(in ItemKey key, out Item item);
    }
}