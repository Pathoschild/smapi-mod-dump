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
internal class DefaultStorageOptions : IStorageOptions
{
    /// <inheritdoc />
    public FeatureOption AutoOrganize { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public FeatureOption CarryChest { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public FeatureOption CategorizeChest { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public FeatureOption CategorizeChestAutomatically { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public FilterMethod CategorizeChestMethod { get; set; } = FilterMethod.GrayedOut;

    /// <inheritdoc />
    public HashSet<string> CategorizeChestTags { get; set; } = [];

    /// <inheritdoc />
    public FeatureOption ChestFinder { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public FeatureOption ChestInfo { get; set; } = FeatureOption.Disabled;

    /// <inheritdoc />
    public FeatureOption CollectItems { get; set; } = FeatureOption.Disabled;

    /// <inheritdoc />
    public FeatureOption ConfigureChest { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public RangeOption CraftFromChest { get; set; } = RangeOption.Location;

    /// <inheritdoc />
    public int CraftFromChestDistance { get; set; } = -1;

    /// <inheritdoc />
    public FeatureOption HslColorPicker { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public FeatureOption InventoryTabs { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public HashSet<string> InventoryTabList { get; set; } =
    [
        "Clothing", "Cooking", "Crops", "Equipment", "Fishing", "Materials", "Misc", "Seeds",
    ];

    /// <inheritdoc />
    public FeatureOption OpenHeldChest { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public CapacityOption ResizeChest { get; set; } = CapacityOption.Large;

    /// <inheritdoc />
    public FeatureOption SearchItems { get; set; } = FeatureOption.Enabled;

    /// <inheritdoc />
    public RangeOption StashToChest { get; set; } = RangeOption.Location;

    /// <inheritdoc />
    public int StashToChestDistance { get; set; } = 10;

    /// <inheritdoc />
    public int StashToChestPriority { get; set; }

    /// <inheritdoc />
    public virtual string GetDescription() => I18n.Storage_Other_Tooltip();

    /// <inheritdoc />
    public virtual string GetDisplayName() => I18n.Storage_Other_Name();
}