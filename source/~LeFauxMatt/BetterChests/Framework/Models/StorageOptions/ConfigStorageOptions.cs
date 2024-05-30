/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.StorageOptions;

using StardewMods.Common.Services.Integrations.BetterChests;

/// <inheritdoc />
public class ConfigStorageOptions : IStorageOptions
{
    private readonly Func<string> getDescription;
    private readonly Func<string> getDisplayName;
    private readonly Func<IStorageOptions> getOptions;

    /// <summary>Initializes a new instance of the <see cref="ConfigStorageOptions" /> class.</summary>
    /// <param name="getOptions">Get the storage options.</param>
    /// <param name="getDescription">Get method for the description.</param>
    /// <param name="getDisplayName">Get method for the display name.</param>
    public ConfigStorageOptions(
        Func<IStorageOptions> getOptions,
        Func<string> getDescription,
        Func<string> getDisplayName)
    {
        this.getOptions = getOptions;
        this.getDescription = getDescription;
        this.getDisplayName = getDisplayName;
    }

    /// <inheritdoc />
    public string Description => this.getDescription();

    /// <inheritdoc />
    public string DisplayName => this.getDisplayName();

    /// <inheritdoc />
    public RangeOption AccessChest
    {
        get => this.Options.AccessChest;
        set => this.Options.AccessChest = value;
    }

    /// <inheritdoc />
    public int AccessChestPriority
    {
        get => this.Options.AccessChestPriority;
        set => this.Options.AccessChestPriority = value;
    }

    /// <inheritdoc />
    public FeatureOption AutoOrganize
    {
        get => this.Options.AutoOrganize;
        set => this.Options.AutoOrganize = value;
    }

    /// <inheritdoc />
    public FeatureOption CarryChest
    {
        get => this.Options.CarryChest;
        set => this.Options.CarryChest = value;
    }

    /// <inheritdoc />
    public FeatureOption CategorizeChest
    {
        get => this.Options.CategorizeChest;
        set => this.Options.CategorizeChest = value;
    }

    /// <inheritdoc />
    public FeatureOption CategorizeChestBlockItems
    {
        get => this.Options.CategorizeChestBlockItems;
        set => this.Options.CategorizeChestBlockItems = value;
    }

    /// <inheritdoc />
    public FeatureOption CategorizeChestIncludeStacks
    {
        get => this.Options.CategorizeChestIncludeStacks;
        set => this.Options.CategorizeChestIncludeStacks = value;
    }

    /// <inheritdoc />
    public string CategorizeChestSearchTerm
    {
        get => this.Options.CategorizeChestSearchTerm;
        set => this.Options.CategorizeChestSearchTerm = value;
    }

    /// <inheritdoc />
    public FeatureOption ChestFinder
    {
        get => this.Options.ChestFinder;
        set => this.Options.ChestFinder = value;
    }

    /// <inheritdoc />
    public FeatureOption CollectItems
    {
        get => this.Options.CollectItems;
        set => this.Options.CollectItems = value;
    }

    /// <inheritdoc />
    public FeatureOption ConfigureChest
    {
        get => this.Options.ConfigureChest;
        set => this.Options.ConfigureChest = value;
    }

    /// <inheritdoc />
    public RangeOption CookFromChest
    {
        get => this.Options.CookFromChest;
        set => this.Options.CookFromChest = value;
    }

    /// <inheritdoc />
    public RangeOption CraftFromChest
    {
        get => this.Options.CraftFromChest;
        set => this.Options.CraftFromChest = value;
    }

    /// <inheritdoc />
    public int CraftFromChestDistance
    {
        get => this.Options.CraftFromChestDistance;
        set => this.Options.CraftFromChestDistance = value;
    }

    /// <inheritdoc />
    public FeatureOption HslColorPicker
    {
        get => this.Options.HslColorPicker;
        set => this.Options.HslColorPicker = value;
    }

    /// <inheritdoc />
    public FeatureOption InventoryTabs
    {
        get => this.Options.InventoryTabs;
        set => this.Options.InventoryTabs = value;
    }

    /// <inheritdoc />
    public FeatureOption OpenHeldChest
    {
        get => this.Options.OpenHeldChest;
        set => this.Options.OpenHeldChest = value;
    }

    /// <inheritdoc />
    public ChestMenuOption ResizeChest
    {
        get => this.Options.ResizeChest;
        set => this.Options.ResizeChest = value;
    }

    /// <inheritdoc />
    public int ResizeChestCapacity
    {
        get => this.Options.ResizeChestCapacity;
        set => this.Options.ResizeChestCapacity = value;
    }

    /// <inheritdoc />
    public FeatureOption SearchItems
    {
        get => this.Options.SearchItems;
        set => this.Options.SearchItems = value;
    }

    /// <inheritdoc />
    public FeatureOption ShopFromChest
    {
        get => this.Options.ShopFromChest;
        set => this.Options.ShopFromChest = value;
    }

    /// <inheritdoc />
    public FeatureOption SortInventory
    {
        get => this.Options.SortInventory;
        set => this.Options.SortInventory = value;
    }

    /// <inheritdoc />
    public string SortInventoryBy
    {
        get => this.Options.SortInventoryBy;
        set => this.Options.SortInventoryBy = value;
    }

    /// <inheritdoc />
    public RangeOption StashToChest
    {
        get => this.Options.StashToChest;
        set => this.Options.StashToChest = value;
    }

    /// <inheritdoc />
    public int StashToChestDistance
    {
        get => this.Options.StashToChestDistance;
        set => this.Options.StashToChestDistance = value;
    }

    /// <inheritdoc />
    public StashPriority StashToChestPriority
    {
        get => this.Options.StashToChestPriority;
        set => this.Options.StashToChestPriority = value;
    }

    /// <inheritdoc />
    public string StorageIcon
    {
        get => this.Options.StorageIcon;
        set => this.Options.StorageIcon = value;
    }

    /// <inheritdoc />
    public FeatureOption StorageInfo
    {
        get => this.Options.StorageInfo;
        set => this.Options.StorageInfo = value;
    }

    /// <inheritdoc />
    public FeatureOption StorageInfoHover
    {
        get => this.Options.StorageInfoHover;
        set => this.Options.StorageInfoHover = value;
    }

    /// <inheritdoc />
    public string StorageName
    {
        get => this.Options.StorageName;
        set => this.Options.StorageName = value;
    }

    private IStorageOptions Options => this.getOptions();
}