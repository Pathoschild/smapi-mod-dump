/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.BetterChests.Interfaces;

using Microsoft.Xna.Framework;
using StardewValley.Inventories;
using StardewValley.Mods;
using StardewValley.Network;

/// <summary>An instance of a game object that can store items.</summary>
public interface IStorageContainer
{
    /// <summary>Gets the name of the container.</summary>
    string DisplayName { get; }

    /// <summary>Gets the description of the container.</summary>
    string Description { get; }

    /// <summary>Gets the capacity of the container.</summary>
    int Capacity { get; }

    /// <summary>Gets options for the storage instance.</summary>
    IStorageOptions Options { get; }

    /// <summary>Gets the collection of items.</summary>
    IInventory Items { get; }

    /// <summary>Gets the game location of an object.</summary>
    GameLocation Location { get; }

    /// <summary>Gets the tile location of an object.</summary>
    Vector2 TileLocation { get; }

    /// <summary>Gets the mod data dictionary.</summary>
    ModDataDictionary ModData { get; }

    /// <summary>Gets a mutex for the container.</summary>
    NetMutex? Mutex { get; }

    /// <summary>Executes a given action for each item in the collection.</summary>
    /// <param name="action">The action to be executed for each item.</param>
    public void ForEachItem(Func<Item, bool> action);

    /// <summary>Opens an item grab menu for this container.</summary>
    /// <param name="playSound">Whether to play the container open sound.</param>
    public void ShowMenu(bool playSound = false);

    /// <summary>Tries to remove an item from the container.</summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was successfully taken; otherwise, false.</returns>
    public bool TryRemove(Item item);

    /// <summary>Tries to add an item to the container.</summary>
    /// <param name="item">The item to add.</param>
    /// <param name="remaining">When this method returns, contains the remaining item after addition, if any.</param>
    /// <returns>True if the item was successfully given; otherwise, false.</returns>
    public bool TryAdd(Item item, out Item? remaining);

    /// <summary>Grabs an item from the inventory.</summary>
    /// <param name="item">The item to grab from the inventory.</param>
    /// <param name="who">The farmer who is grabbing the item.</param>
    public void GrabItemFromInventory(Item item, Farmer who);

    /// <summary>Grabs an item from the chest.</summary>
    /// <param name="item">The item to be grabbed from the chest.</param>
    /// <param name="who">The farmer who is grabbing the item.</param>
    public void GrabItemFromChest(Item item, Farmer who);
}