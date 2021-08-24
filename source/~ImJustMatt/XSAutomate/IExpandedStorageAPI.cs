/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Objects;

namespace XSAutomate
{
    public interface IExpandedStorageAPI
    {
        /// <summary>Checks whether an item is allowed to be added to a chest.</summary>
        /// <param name="chest">The chest to add to.</param>
        /// <param name="item">The item to be added.</param>
        /// <returns>True if chest accepts the item.</returns>
        bool AcceptsItem(Chest chest, Item item);
    }
}