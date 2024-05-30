/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.GarbageDay.Framework.Services;

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewMods.Common.Helpers;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models.Data;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.BetterChests;
using StardewMods.Common.Services.Integrations.ContentPatcher;
using StardewMods.Common.Services.Integrations.ExpandedStorage;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.GarbageDay.Framework.Interfaces;
using StardewMods.GarbageDay.Framework.Models;
using StardewValley.GameData.BigCraftables;
using xTile;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

/// <inheritdoc />
internal sealed class AssetHandler : BaseAssetHandler
{
    private readonly Dictionary<string, FoundGarbageCan> foundGarbageCans;
    private readonly IModConfig modConfig;

    /// <summary>Initializes a new instance of the <see cref="AssetHandler" /> class.</summary>
    /// <param name="contentPatcherIntegration">Dependency for Content Patcher integration.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="foundGarbageCans">The discovered garbage cans.</param>
    /// <param name="gameContentHelper">Dependency used for loading game assets.</param>
    /// <param name="iconRegistry">Dependency used for registering and retrieving icons.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="modContentHelper">Dependency used for accessing mod content.</param>
    public AssetHandler(
        ContentPatcherIntegration contentPatcherIntegration,
        IEventManager eventManager,
        Dictionary<string, FoundGarbageCan> foundGarbageCans,
        IGameContentHelper gameContentHelper,
        IIconRegistry iconRegistry,
        IModConfig modConfig,
        IModContentHelper modContentHelper)
        : base(contentPatcherIntegration, eventManager, gameContentHelper, modContentHelper)
    {
        // Init
        this.foundGarbageCans = foundGarbageCans;
        this.modConfig = modConfig;

        this.Asset($"{Mod.Id}/Icons").Load<Texture2D>("assets/icons.png");
        this.Asset("Data/BigCraftables").Edit($"{Mod.Id}/GarbageCan", () => this.GarbageCan);
        this.Asset("Data/GarbageCans").Watch(onInvalidated: _ => this.foundGarbageCans.Clear());

        this.DynamicAsset(
            e => e.DataType == typeof(Map),
            asset => asset.Edit(this.DiscoverGarbageCans, (AssetEditPriority)int.MaxValue));

        iconRegistry.AddIcon("GarbageCan", $"{Mod.Id}/Icons", new Rectangle(0, 0, 16, 16));
    }

    private BigCraftableData GarbageCan
    {
        get
        {
            var bigCraftableData = new BigCraftableData
            {
                Name = "Garbage Can",
                DisplayName = I18n.GarbageCan_Name(),
                Description = I18n.GarbageCan_Description(),
                Fragility = 2,
                IsLamp = false,
                Texture = this.ModContentHelper.GetInternalAssetName("assets/GarbageCan.png").Name,
                CustomFields = new Dictionary<string, string>
                {
                    { "furyx639.ExpandedStorage/Enabled", "true" },
                },
            };

            var typeModel = new DictionaryModel(() => bigCraftableData.CustomFields);
            var storageData = new StorageData(typeModel);
            storageData.Frames = 3;
            storageData.CloseNearbySound = "trashcanlid";
            storageData.OpenNearby = true;
            storageData.OpenNearbySound = "trashcanlid";
            storageData.OpenSound = "trashcan";
            storageData.PlayerColor = true;

            var storageOptions = new StorageOptions(typeModel);
            storageOptions.AutoOrganize = FeatureOption.Disabled;
            storageOptions.CarryChest = FeatureOption.Disabled;
            storageOptions.CategorizeChest = FeatureOption.Disabled;
            storageOptions.CollectItems = FeatureOption.Disabled;
            storageOptions.ConfigureChest = FeatureOption.Disabled;
            storageOptions.CookFromChest = RangeOption.Disabled;
            storageOptions.CraftFromChest = RangeOption.Disabled;
            storageOptions.HslColorPicker = FeatureOption.Disabled;
            storageOptions.InventoryTabs = FeatureOption.Disabled;
            storageOptions.OpenHeldChest = FeatureOption.Disabled;
            storageOptions.ResizeChest = ChestMenuOption.Small;
            storageOptions.ResizeChestCapacity = 9;
            storageOptions.SearchItems = FeatureOption.Disabled;
            storageOptions.StashToChest = RangeOption.Disabled;
            storageOptions.StorageInfo = FeatureOption.Disabled;

            return bigCraftableData;
        }
    }

    private void DiscoverGarbageCans(IAssetData asset)
    {
        var map = asset.AsMap().Data;
        for (var x = 0; x < map.Layers[0].LayerWidth; ++x)
        {
            for (var y = 0; y < map.Layers[0].LayerHeight; ++y)
            {
                var layer = map.GetLayer("Buildings");
                var tile = layer.PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size);
                if (tile is null
                    || !tile.Properties.TryGetValue("Action", out var property)
                    || string.IsNullOrWhiteSpace(property))
                {
                    continue;
                }

                var parts = ArgUtility.SplitBySpace(property);
                if (parts.Length < 2
                    || !parts[0].Equals("Garbage", StringComparison.OrdinalIgnoreCase)
                    || string.IsNullOrWhiteSpace(parts[1])
                    || !this.TryAddFound(parts[1], asset.NameWithoutLocale, x, y))
                {
                    continue;
                }

                Log.Trace("Garbage Can found on map: {0}", parts[1]);

                // Remove base tile
                layer.Tiles[x, y] = null;

                // Remove Lid tile
                layer = map.GetLayer("Front");
                layer.Tiles[x, y - 1] = null;

                // Add NoPath to tile
                map
                    .GetLayer("Back")
                    .PickTile(new Location(x, y) * Game1.tileSize, Game1.viewport.Size)
                    ?.Properties.Add("NoPath", string.Empty);
            }
        }
    }

    private bool TryAddFound(string whichCan, IAssetName assetName, int x, int y)
    {
        if (this.foundGarbageCans.ContainsKey(whichCan))
        {
            return true;
        }

        if (!DataLoader.GarbageCans(Game1.content).GarbageCans.TryGetValue(whichCan, out var garbageCanData))
        {
            return false;
        }

        if (!this.modConfig.OnByDefault && garbageCanData.CustomFields?.GetBool($"{Mod.Id}/Enabled") != true)
        {
            return false;
        }

        this.foundGarbageCans.Add(whichCan, new FoundGarbageCan(whichCan, assetName, x, y));
        return true;
    }
}