/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Models.Config;

using System.Collections.Generic;
using StardewMods.BetterChests.Enums;
using StardewMods.BetterChests.Interfaces.Config;

/// <inheritdoc />
internal class StorageData : IStorageData
{
    /// <inheritdoc />
    public FeatureOption AutoOrganize { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOption CarryChest { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOption ChestMenuTabs { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public HashSet<string> ChestMenuTabSet { get; set; } = new();

    /// <inheritdoc />
    public FeatureOption CollectItems { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOptionRange CraftFromChest { get; set; } = FeatureOptionRange.Default;

    /// <inheritdoc />
    public HashSet<string> CraftFromChestDisableLocations { get; set; } = new();

    /// <inheritdoc />
    public int CraftFromChestDistance { get; set; }

    /// <inheritdoc />
    public FeatureOption CustomColorPicker { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOption FilterItems { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public HashSet<string> FilterItemsList { get; set; } = new();

    /// <inheritdoc />
    public FeatureOption OpenHeldChest { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOption OrganizeChest { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public GroupBy OrganizeChestGroupBy { get; set; } = GroupBy.Default;

    /// <inheritdoc />
    public SortBy OrganizeChestSortBy { get; set; } = SortBy.Default;

    /// <inheritdoc />
    public FeatureOption ResizeChest { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public int ResizeChestCapacity { get; set; }

    /// <inheritdoc />
    public FeatureOption ResizeChestMenu { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public int ResizeChestMenuRows { get; set; }

    /// <inheritdoc />
    public FeatureOption SearchItems { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOptionRange StashToChest { get; set; } = FeatureOptionRange.Default;

    /// <inheritdoc />
    public HashSet<string> StashToChestDisableLocations { get; set; } = new();

    /// <inheritdoc />
    public int StashToChestDistance { get; set; }

    /// <inheritdoc />
    public int StashToChestPriority { get; set; }

    /// <inheritdoc />
    public FeatureOption StashToChestStacks { get; set; } = FeatureOption.Default;

    /// <inheritdoc />
    public FeatureOption UnloadChest { get; set; } = FeatureOption.Default;
}