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

using System.Globalization;
using System.Text;
using Microsoft.Xna.Framework;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.StorageOptions;
using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Mods;
using StardewValley.Network;

/// <inheritdoc cref="IStorageContainer{TSource}" />
internal abstract class BaseContainer<TSource> : BaseContainer, IStorageContainer<TSource>
    where TSource : class
{
    /// <summary>Initializes a new instance of the <see cref="BaseContainer{TSource}" /> class.</summary>
    /// <param name="baseOptions">The type of storage object.</param>
    protected BaseContainer(IStorageOptions baseOptions)
        : base(baseOptions, typeof(TSource) == typeof(Farmer)) { }

    /// <inheritdoc />
    public abstract bool IsAlive { get; }

    /// <inheritdoc />
    public abstract WeakReference<TSource> Source { get; }
}

/// <inheritdoc />
internal abstract class BaseContainer : IStorageContainer
{
    private readonly IStorageOptions baseOptions;
    private readonly Lazy<IStorageOptions> storageOptions;

    /// <summary>Initializes a new instance of the <see cref="BaseContainer" /> class.</summary>
    /// <param name="baseOptions">The type of storage object.</param>
    /// <param name="isFarmer">Indicates if the container is for a Farmer.</param>
    protected BaseContainer(IStorageOptions baseOptions, bool isFarmer = false)
    {
        this.baseOptions = baseOptions;
        if (isFarmer)
        {
            this.storageOptions = new Lazy<IStorageOptions>(() => baseOptions);
            return;
        }

        this.storageOptions = new Lazy<IStorageOptions>(
            () => new ChildStorageOptions(() => baseOptions, new ModDataStorageOptions(this.ModData)));
    }

    /// <inheritdoc />
    public string DisplayName => this.baseOptions.GetDisplayName();

    /// <inheritdoc />
    public string Description => this.baseOptions.GetDescription();

    /// <inheritdoc />
    public abstract int Capacity { get; }

    /// <inheritdoc />
    public IStorageOptions Options => this.storageOptions.Value;

    /// <inheritdoc />
    public abstract IInventory Items { get; }

    /// <inheritdoc />
    public abstract GameLocation Location { get; }

    /// <inheritdoc />
    public abstract Vector2 TileLocation { get; }

    /// <inheritdoc />
    public abstract ModDataDictionary ModData { get; }

    /// <inheritdoc />
    public abstract NetMutex? Mutex { get; }

    /// <inheritdoc />
    public void ForEachItem(Func<Item, bool> action)
    {
        for (var index = this.Items.Count - 1; index >= 0; --index)
        {
            if (this.Items[index] is null)
            {
                continue;
            }

            if (!action(this.Items[index]))
            {
                break;
            }
        }
    }

    /// <inheritdoc />
    public virtual void ShowMenu(bool playSound = false) { }

    /// <inheritdoc />
    public abstract bool TryAdd(Item item, out Item? remaining);

    /// <inheritdoc />
    public abstract bool TryRemove(Item item);

    /// <inheritdoc />
    public virtual void GrabItemFromInventory(Item item, Farmer who)
    {
        if (item is null)
        {
            return;
        }

        if (item.Stack == 0)
        {
            item.Stack = 1;
        }

        if (!this.TryAdd(item, out var remaining))
        {
            return;
        }

        if (remaining == null)
        {
            who.removeItemFromInventory(item);
        }
        else
        {
            remaining = who.addItemToInventory(remaining);
        }

        var oldID = Game1.activeClickableMenu.currentlySnappedComponent != null
            ? Game1.activeClickableMenu.currentlySnappedComponent.myID
            : -1;

        this.ShowMenu();
        if (Game1.activeClickableMenu is not ItemGrabMenu itemGrabMenu)
        {
            return;
        }

        itemGrabMenu.heldItem = remaining;
        if (oldID == -1)
        {
            return;
        }

        Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
        Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
    }

    /// <inheritdoc />
    public virtual void GrabItemFromChest(Item item, Farmer who)
    {
        if (item is null || !who.couldInventoryAcceptThisItem(item))
        {
            return;
        }

        this.Items.Remove(item);
        this.Items.RemoveEmptySlots();
        this.ShowMenu();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(
            !string.IsNullOrWhiteSpace(this.Options.StorageName) ? this.Options.StorageName.Trim() : this.DisplayName);

        sb.Append(" at ");
        sb.Append(this.Location?.DisplayName ?? "Unknown");
        sb.Append(CultureInfo.InvariantCulture, $"({this.TileLocation.X:n0}, {this.TileLocation.Y:n0})");
        return sb.ToString();
    }
}