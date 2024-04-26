/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.Containers;

using Microsoft.Xna.Framework;
using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;
using StardewValley.Inventories;
using StardewValley.Mods;
using StardewValley.Network;

/// <inheritdoc />
internal class ChildContainer : IStorageContainer
{
    private readonly IStorageContainer child;
    private readonly IStorageContainer parent;

    /// <summary>Initializes a new instance of the <see cref="ChildContainer" /> class.</summary>
    /// <param name="parent">The parent container.</param>
    /// <param name="child">The child container.</param>
    public ChildContainer(IStorageContainer parent, IStorageContainer child)
    {
        this.parent = parent;
        this.child = child;
    }

    /// <summary>Gets the top-most parent storage.</summary>
    public IStorageContainer Parent =>
        this.parent switch { ChildContainer childStorage => childStorage.parent, _ => this.parent };

    /// <summary>Gets the bottom-most child storage.</summary>
    public IStorageContainer Child =>
        this.child switch { ChildContainer childStorage => childStorage.Child, _ => this.child };

    /// <inheritdoc />
    public string DisplayName => this.child.DisplayName;

    /// <inheritdoc />
    public string Description => this.child.Description;

    /// <inheritdoc />
    public int Capacity => this.child.Capacity;

    /// <inheritdoc />
    public IStorageOptions Options => this.child.Options;

    /// <inheritdoc />
    public IInventory Items => this.child.Items;

    /// <inheritdoc />
    public GameLocation Location => this.child.Location ?? this.parent.Location;

    /// <inheritdoc />
    public Vector2 TileLocation =>
        this.child.TileLocation.Equals(Vector2.Zero) ? this.parent.TileLocation : this.child.TileLocation;

    /// <inheritdoc />
    public ModDataDictionary ModData => this.child.ModData;

    /// <inheritdoc />
    public NetMutex? Mutex => this.child.Mutex;

    /// <inheritdoc />
    public void ForEachItem(Func<Item, bool> action) => this.child.ForEachItem(action);

    /// <inheritdoc />
    public void ShowMenu(bool playSound = false) => this.child.ShowMenu(playSound);

    /// <inheritdoc />
    public bool TryAdd(Item item, out Item? remaining) => this.child.TryAdd(item, out remaining);

    /// <inheritdoc />
    public bool TryRemove(Item item) => this.child.TryRemove(item);

    /// <inheritdoc />
    public void GrabItemFromInventory(Item item, Farmer who) => this.child.GrabItemFromInventory(item, who);

    /// <inheritdoc />
    public void GrabItemFromChest(Item item, Farmer who) => this.child.GrabItemFromChest(item, who);

    /// <inheritdoc />
    public override string ToString() => $"{this.DisplayName} in {this.Parent}";
}