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
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Mods;
using StardewValley.Network;

/// <inheritdoc />
internal sealed class FarmerContainer : BaseContainer<Farmer>
{
    /// <summary>Initializes a new instance of the <see cref="FarmerContainer" /> class.</summary>
    /// <param name="farmer">The farmer whose inventory is holding the container.</param>
    public FarmerContainer(Farmer farmer)
        : base(farmer) =>
        this.InitOptions();

    /// <inheritdoc />
    public override int Capacity => this.Farmer.MaxItems;

    /// <summary>Gets the source farmer of the container.</summary>
    /// <exception cref="ObjectDisposedException">Thrown when the Farmer is disposed.</exception>
    public Farmer Farmer =>
        this.Source.TryGetTarget(out var target) ? target : throw new ObjectDisposedException(nameof(FarmerContainer));

    /// <inheritdoc />
    public override bool IsAlive => this.Source.TryGetTarget(out _);

    /// <inheritdoc />
    public override IInventory Items => this.Farmer.Items;

    /// <inheritdoc />
    public override GameLocation Location => this.Farmer.currentLocation;

    /// <inheritdoc />
    public override ModDataDictionary ModData => this.Farmer.modData;

    /// <inheritdoc />
    public override NetMutex? Mutex => (Utility.getHomeOfFarmer(this.Farmer) as Cabin)?.inventoryMutex;

    /// <inheritdoc />
    public override Vector2 TileLocation => this.Farmer.Tile;

    /// <inheritdoc />
    public override int ResizeChestCapacity
    {
        get => this.Farmer.MaxItems;
        set
        {
            this.Farmer.MaxItems = value;
            this.Farmer.Items.RemoveEmptySlots();
            while (this.Farmer.Items.Count < this.Farmer.MaxItems)
            {
                this.Farmer.Items.Add(null);
            }
        }
    }

    /// <inheritdoc />
    public override string StorageName
    {
        get => this.Farmer.Name;
        set { }
    }

    /// <inheritdoc />
    public override void ShowMenu(bool playSound = false) =>
        Game1.activeClickableMenu = new GameMenu(playOpeningSound: playSound);

    /// <inheritdoc />
    public override bool TryAdd(Item item, out Item? remaining)
    {
        var stack = item.Stack;
        remaining = this.Farmer.addItemToInventory(item);
        return remaining is null || remaining.Stack != stack;
    }

    /// <inheritdoc />
    public override bool TryRemove(Item item)
    {
        if (!this.Items.Contains(item))
        {
            return false;
        }

        this.Farmer.removeItemFromInventory(item);
        return true;
    }

    /// <inheritdoc />
    protected override void InitOptions()
    {
        base.InitOptions();

        if (string.IsNullOrWhiteSpace(this.StorageIcon))
        {
            this.StorageIcon = "StardewValley.Vanilla/Backpack";
        }
    }
}