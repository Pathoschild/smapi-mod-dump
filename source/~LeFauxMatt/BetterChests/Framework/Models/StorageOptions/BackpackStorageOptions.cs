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

using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;

/// <inheritdoc />
internal sealed class BackpackStorageOptions : IStorageOptions
{
    private readonly Farmer farmer;
    private readonly IStorageOptions storageOptions;

    /// <summary>Initializes a new instance of the <see cref="BackpackStorageOptions" /> class.</summary>
    /// <param name="farmer">The farmer whose backpack storage this represents.</param>
    public BackpackStorageOptions(Farmer farmer)
    {
        this.storageOptions = new ModDataStorageOptions(farmer.modData);
        this.farmer = farmer;
    }

    /// <inheritdoc />
    public RangeOption AccessChest
    {
        get => this.storageOptions.AccessChest;
        set => this.storageOptions.AccessChest = value;
    }

    /// <inheritdoc />
    public FeatureOption AutoOrganize
    {
        get => this.storageOptions.AutoOrganize;
        set => this.storageOptions.AutoOrganize = value;
    }

    /// <inheritdoc />
    public FeatureOption CarryChest
    {
        get => this.storageOptions.CarryChest;
        set => this.storageOptions.CarryChest = value;
    }

    /// <inheritdoc />
    public FeatureOption CategorizeChest
    {
        get => this.storageOptions.CategorizeChest;
        set => this.storageOptions.CategorizeChest = value;
    }

    /// <inheritdoc />
    public FeatureOption CategorizeChestBlockItems
    {
        get => this.storageOptions.CategorizeChestBlockItems;
        set => this.storageOptions.CategorizeChestBlockItems = value;
    }

    /// <inheritdoc />
    public string CategorizeChestSearchTerm
    {
        get => this.storageOptions.CategorizeChestSearchTerm;
        set => this.storageOptions.CategorizeChestSearchTerm = value;
    }

    /// <inheritdoc />
    public FeatureOption CategorizeChestIncludeStacks
    {
        get => this.storageOptions.CategorizeChestIncludeStacks;
        set => this.storageOptions.CategorizeChestIncludeStacks = value;
    }

    /// <inheritdoc />
    public FeatureOption ChestFinder
    {
        get => this.storageOptions.ChestFinder;
        set => this.storageOptions.ChestFinder = value;
    }

    /// <inheritdoc />
    public FeatureOption ChestInfo
    {
        get => this.storageOptions.ChestInfo;
        set => this.storageOptions.ChestInfo = value;
    }

    /// <inheritdoc />
    public FeatureOption CollectItems
    {
        get => this.storageOptions.CollectItems;
        set => this.storageOptions.CollectItems = value;
    }

    /// <inheritdoc />
    public FeatureOption ConfigureChest
    {
        get => this.storageOptions.ConfigureChest;
        set => this.storageOptions.ConfigureChest = value;
    }

    /// <inheritdoc />
    public RangeOption CookFromChest
    {
        get => this.storageOptions.CookFromChest;
        set => this.storageOptions.CookFromChest = value;
    }

    /// <inheritdoc />
    public RangeOption CraftFromChest
    {
        get => this.storageOptions.CraftFromChest;
        set => this.storageOptions.CraftFromChest = value;
    }

    /// <inheritdoc />
    public int CraftFromChestDistance
    {
        get => this.storageOptions.CraftFromChestDistance;
        set => this.storageOptions.CraftFromChestDistance = value;
    }

    /// <inheritdoc />
    public FeatureOption HslColorPicker
    {
        get => this.storageOptions.HslColorPicker;
        set => this.storageOptions.HslColorPicker = value;
    }

    /// <inheritdoc />
    public FeatureOption OpenHeldChest
    {
        get => this.storageOptions.OpenHeldChest;
        set => this.storageOptions.OpenHeldChest = value;
    }

    /// <inheritdoc />
    public ChestMenuOption ResizeChest
    {
        get => this.storageOptions.ResizeChest;
        set => this.storageOptions.ResizeChest = value;
    }

    /// <inheritdoc />
    public int ResizeChestCapacity
    {
        get => this.farmer.MaxItems;
        set
        {
            this.farmer.MaxItems = value;
            while (this.farmer.Items.Count < this.farmer.MaxItems)
            {
                this.farmer.Items.Add(null);
            }
        }
    }

    /// <inheritdoc />
    public FeatureOption SearchItems
    {
        get => this.storageOptions.SearchItems;
        set => this.storageOptions.SearchItems = value;
    }

    /// <inheritdoc />
    public FeatureOption ShopFromChest
    {
        get => this.storageOptions.ShopFromChest;
        set => this.storageOptions.ShopFromChest = value;
    }

    /// <inheritdoc />
    public RangeOption StashToChest
    {
        get => this.storageOptions.StashToChest;
        set => this.storageOptions.StashToChest = value;
    }

    /// <inheritdoc />
    public int StashToChestDistance
    {
        get => this.storageOptions.StashToChestDistance;
        set => this.storageOptions.StashToChestDistance = value;
    }

    /// <inheritdoc />
    public StashPriority StashToChestPriority
    {
        get => this.storageOptions.StashToChestPriority;
        set => this.storageOptions.StashToChestPriority = value;
    }

    /// <inheritdoc />
    public string StorageName
    {
        get => this.farmer.Name;
        set { }
    }

    /// <inheritdoc />
    public IStorageOptions GetActualOptions() => this;

    /// <inheritdoc />
    public IStorageOptions GetParentOptions() => this;

    /// <inheritdoc />
    public string GetDisplayName() => I18n.Storage_Backpack_Name();

    /// <inheritdoc />
    public string GetDescription() => I18n.Storage_Backpack_Tooltip();
}