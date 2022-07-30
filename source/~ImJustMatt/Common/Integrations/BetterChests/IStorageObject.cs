/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.BetterChests;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

/// <summary>
///     Provides access to storage data and functions.
/// </summary>
public interface IStorageObject : IStorageData, IEqualityComparer<IStorageObject>
{
    /// <summary>
    ///     Gets the actual capacity of the object's storage.
    /// </summary>
    public int ActualCapacity { get; }

    /// <summary>
    ///     Gets the context object.
    /// </summary>
    public object Context { get; }

    /// <summary>
    ///     Gets data individually assigned to the storage object.
    /// </summary>
    public IStorageData Data { get; }

    /// <summary>
    ///     Gets an ItemMatcher to represent the currently selected filters.
    /// </summary>
    public IItemMatcher FilterMatcher { get; }

    /// <summary>
    ///     Gets the items in the object's storage.
    /// </summary>
    public IList<Item?> Items { get; }

    /// <summary>
    ///     Gets the calculated capacity of the <see cref="InventoryMenu" />.
    /// </summary>
    public int MenuCapacity { get; }

    /// <summary>
    ///     Gets the extra vertical space needed for the <see cref="InventoryMenu" /> based on
    ///     <see cref="IStorageData.ResizeChestMenuRows" />.
    /// </summary>
    public int MenuExtraSpace { get; }

    /// <summary>
    ///     Gets the number of rows to display on the <see cref="InventoryMenu" /> based on
    ///     <see cref="IStorageData.ResizeChestMenuRows" />.
    /// </summary>
    public int MenuRows { get; }

    /// <summary>
    ///     Gets the ModData associated with the context object.
    /// </summary>
    public ModDataDictionary ModData { get; }

    /// <summary>
    ///     Gets the mutex required to lock this object.
    /// </summary>
    public NetMutex? Mutex { get; }

    /// <summary>
    ///     Gets the parent context where this storage is contained.
    /// </summary>
    public object? Parent { get; }

    /// <summary>
    ///     Gets the coordinate of this object.
    /// </summary>
    public Vector2 Position { get; }

    /// <summary>
    ///     Gets or sets the storage data for this type of storage.
    /// </summary>
    public IStorageData Type { get; set; }

    /// <summary>
    ///     Attempts to add an item into the storage.
    /// </summary>
    /// <param name="item">The item to stash.</param>
    /// <returns>Returns the item if it could not be added completely, or null if it could.</returns>
    public Item? AddItem(Item item);

    /// <summary>
    ///     Removes null items from the storage.
    /// </summary>
    public void ClearNulls();

    /// <summary>
    ///     Tests if a <see cref="Item" /> matches the <see cref="IStorageData.FilterItemsList" /> condition.
    /// </summary>
    /// <param name="item">The <see cref="Item" /> to test.</param>
    /// <returns>Returns true if the <see cref="Item" /> matches the filters.</returns>
    public bool FilterMatches(Item? item);

    /// <summary>
    ///     Grabs an item from a player into this storage container.
    /// </summary>
    /// <param name="item">The item to grab.</param>
    /// <param name="who">The player whose inventory to grab the item from.</param>
    public void GrabInventoryItem(Item item, Farmer who);

    /// <summary>
    ///     Grab an item from this storage container.
    /// </summary>
    /// <param name="item">The item to grab.</param>
    /// <param name="who">The player grabbing the item.</param>
    public void GrabStorageItem(Item item, Farmer who);

    /// <summary>
    ///     Organizes items in a storage.
    /// </summary>
    /// <param name="descending">Sort in descending order.</param>
    public void OrganizeItems(bool descending = false);

    /// <summary>
    ///     Creates an <see cref="ItemGrabMenu" /> for this storage container.
    /// </summary>
    public void ShowMenu();

    /// <summary>
    ///     Stashes an item into storage based on categorization and stack settings.
    /// </summary>
    /// <param name="item">The item to stash.</param>
    /// <param name="existingStacks">Whether to stash into stackable items or based on categorization.</param>
    /// <returns>Returns the <see cref="Item" /> if not all could be stashed or null if successful.</returns>
    public Item? StashItem(Item item, bool existingStacks = false);
}