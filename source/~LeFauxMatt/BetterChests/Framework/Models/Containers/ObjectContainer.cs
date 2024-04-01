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
using StardewValley.Objects;

/// <inheritdoc />
internal class ObjectContainer : BaseContainer<SObject>
{
    private readonly Chest chest;

    /// <summary>Initializes a new instance of the <see cref="ObjectContainer" /> class.</summary>
    /// <param name="baseOptions">The type of storage object.</param>
    /// <param name="obj">The storage object.</param>
    /// <param name="chest">The chest storage of the container.</param>
    public ObjectContainer(IStorageOptions baseOptions, SObject obj, Chest chest)
        : base(baseOptions)
    {
        this.Source = new WeakReference<SObject>(obj);
        this.chest = chest;
    }

    /// <summary>Gets the source object of the container.</summary>
    public SObject Object =>
        this.Source.TryGetTarget(out var target) ? target : throw new ObjectDisposedException(nameof(ObjectContainer));

    /// <inheritdoc />
    public override int Capacity => this.chest.GetActualCapacity();

    /// <inheritdoc />
    public override IInventory Items => this.chest.GetItemsForPlayer();

    /// <inheritdoc />
    public override GameLocation Location => this.Object.Location;

    /// <inheritdoc />
    public override Vector2 TileLocation => this.Object.TileLocation;

    /// <inheritdoc />
    public override ModDataDictionary ModData => this.Object.modData;

    /// <inheritdoc />
    public override NetMutex Mutex => this.chest.GetMutex();

    /// <inheritdoc />
    public override bool IsAlive => this.Source.TryGetTarget(out _);

    /// <inheritdoc />
    public override WeakReference<SObject> Source { get; }

    /// <inheritdoc />
    public override void ShowMenu()
    {
        Game1.player.currentLocation.localSound("openChest");
        this.chest.ShowMenu();
    }

    /// <inheritdoc />
    public override bool TryAdd(Item item, out Item? remaining)
    {
        var stack = item.Stack;
        remaining = this.chest.addItem(item);
        return remaining is null || remaining.Stack != stack;
    }

    /// <inheritdoc />
    public override bool TryRemove(Item item)
    {
        if (!this.Items.Contains(item))
        {
            return false;
        }

        this.Items.Remove(item);
        this.Items.RemoveEmptySlots();
        return true;
    }
}