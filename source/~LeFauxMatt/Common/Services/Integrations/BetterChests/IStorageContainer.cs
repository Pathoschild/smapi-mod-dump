/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.BetterChests;
#else
namespace StardewMods.Common.Services.Integrations.BetterChests;
#endif

using Microsoft.Xna.Framework;
using StardewValley.Inventories;
using StardewValley.Mods;
using StardewValley.Network;

/// <summary>
/// A container is the vessel which storage is attached to. It can be anything that can be associated with
/// items, a location, tile position, and has mod data.
/// </summary>
public interface IStorageContainer : IStorageOptions
{
    /// <summary>Gets the actual storage options.</summary>
    public IStorageOptions ActualOptions { get; }

    /// <summary>Gets or sets the parent storage container.</summary>
    public IStorageContainer? Parent { get; set; }

    /// <summary>Gets the capacity of the container.</summary>
    int Capacity { get; }

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

    /// <summary>Adds an option getter.</summary>
    /// <param name="storageOption">Which storage option to add.</param>
    /// <param name="options">The storage options.</param>
    public void AddOptions(StorageOption storageOption, IStorageOptions options);

    /// <summary>Gets the parent storage options.</summary>
    /// <returns>The parent storage options.</returns>
    public IStorageOptions GetParentOptions();

    /// <summary>Executes a given action for each item in the collection.</summary>
    /// <param name="action">The action to be executed for each item.</param>
    public void ForEachItem(Func<Item, bool> action);

    /// <summary>Opens an item grab menu for this container.</summary>
    /// <param name="playSound">Whether to play the container open sound.</param>
    public void ShowMenu(bool playSound = false);

    /// <summary>Tries to remove an item from the container.</summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><c>true</c> if the item was successfully taken; otherwise, <c>false</c>.</returns>
    public bool TryRemove(Item item);

    /// <summary>Tries to add an item to the container.</summary>
    /// <param name="item">The item to add.</param>
    /// <param name="remaining">When this method returns, contains the remaining item after addition, if any.</param>
    /// <returns><c>true</c> if the item was successfully given; otherwise, <c>false</c>.</returns>
    public bool TryAdd(Item item, out Item? remaining);

    /// <summary>Grabs an item from the inventory.</summary>
    /// <param name="item">The item to grab from the inventory.</param>
    /// <param name="who">The farmer who is grabbing the item.</param>
    public void GrabItemFromInventory(Item? item, Farmer who);

    /// <summary>Grabs an item from the chest.</summary>
    /// <param name="item">The item to be grabbed from the chest.</param>
    /// <param name="who">The farmer who is grabbing the item.</param>
    public void GrabItemFromChest(Item? item, Farmer who);

    /// <summary>Highlights the specified item.</summary>
    /// <param name="item">The item to highlight.</param>
    /// <returns>Returns true if the item is successfully highlighted, otherwise returns false.</returns>
    public bool HighlightItems(Item? item);
}