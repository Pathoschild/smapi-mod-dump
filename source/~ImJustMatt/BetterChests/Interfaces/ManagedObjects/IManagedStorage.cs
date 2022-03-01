/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Interfaces.ManagedObjects;

using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley;

/// <inheritdoc cref="IStorageContainer" />
internal interface IManagedStorage : IManagedObject, IStorageContainer
{
    /// <summary>
    ///     Gets or sets a value indicating whether organize chest should sort in reverse order.
    /// </summary>
    public bool OrganizeChestOrderByDescending { get; set; }

    /// <summary>
    ///     Attempt to stash an item into the storage based on categorization and existing items.
    /// </summary>
    /// <param name="item">The item to stash.</param>
    /// <returns>Returns the item if it could not be stashed completely, or null if it could.</returns>
    public Item StashItem(Item item);
}