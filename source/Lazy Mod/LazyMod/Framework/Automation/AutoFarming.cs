/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoFarming : Automate
{
    private readonly ModConfig config;

    public AutoFarming(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;

        // 自动耕地
        if (config.AutoTillDirt && tool is Hoe) AutoTillDirt(location, player, tool);
        // 自动清理耕地
        if (config.AutoClearTilledDirt && tool is Pickaxe) AutoClearTilledDirt(location, player, tool);
        // 自动浇水
        if (config.AutoWaterDirt && tool is WateringCan wateringCan) AutoWaterDirt(location, player, wateringCan);
        // 自动填充水壶
        if (config.AutoRefillWateringCan && (tool is WateringCan || config.FindWateringCanFromInventory)) AutoRefillWateringCan(location, player);
        // 自动播种
        if (config.AutoSeed && item?.Category == SObject.SeedsCategory) AutoSeed(location, player, item);
        // 自动施肥
        if (config.AutoFertilize && item?.Category == SObject.fertilizerCategory) AutoFertilize(location, player, item);
        // 自动收获作物
        if (config.AutoHarvestCrop) AutoHarvestCrop(location, player);
        // 自动摇晃果树
        if (config.AutoShakeFruitTree) AutoShakeFruitTree(location, player);
        // 自动清理枯萎作物
        if (config.AutoClearDeadCrop) AutoClearDeadCrop(location, player);
    }

    // 自动耕地
    private void AutoTillDirt(GameLocation location, Farmer player, Tool tool)
    {
        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoTillDirtRange).ToList();
        foreach (var tile in grid)
        {
            // 如果该瓦片不可耕地,则跳过该瓦片的处理
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            location.objects.TryGetValue(tile, out var obj);
            if (tileFeature is not null || obj is not null || location.IsTileOccupiedBy(tile, CollisionMask.All, CollisionMask.Farmers) ||
                !location.isTilePassable(tile) || location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is null)
                continue;

            if (StopAutomate(player, config.StopAutoTillDirtStamina, ref hasAddMessage)) break;
            UseToolOnTile(location, player, tool, tile);
        }
    }

    // 自动清理耕地
    private void AutoClearTilledDirt(GameLocation location, Farmer player, Tool tool)
    {
        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearTilledDirtRange).ToList();
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt { crop: null } hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                if (StopAutomate(player, config.StopAutoClearTilledDirtStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, tool, tile);
            }
        }
    }

    // 自动浇水
    private void AutoWaterDirt(GameLocation location, Farmer player, WateringCan wateringCan)
    {
        var hasAddStaminaMessage = true;
        var hasAddWaterMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoWaterDirtRange).ToList();
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                if (wateringCan.WaterLeft <= 0)
                {
                    if (!hasAddWaterMessage)
                        Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335");
                    break;
                }

                hasAddWaterMessage = false;

                if (StopAutomate(player, config.StopAutoWaterDirtStamina, ref hasAddStaminaMessage)) break;
                UseToolOnTile(location, player, wateringCan, tile);
            }
        }
    }

    // 自动填充水壶
    private void AutoRefillWateringCan(GameLocation location, Farmer player)
    {
        var wateringCan = FindToolFromInventory<WateringCan>();
        if (wateringCan is null || wateringCan.WaterLeft == wateringCan.waterCanMax)
            return;

        var origin = Game1.player.Tile;
        var grid = GetTileGrid(origin, config.AutoRefillWateringCanRange).ToList();
        foreach (var tile in grid.Where(tile => location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y)))
        {
            UseToolOnTile(location, player, wateringCan, tile);
            break;
        }
    }

    // 自动播种
    private void AutoSeed(GameLocation location, Farmer player, Item item)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoSeedRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: null } hoeDirt)
            {
                if (item.Stack <= 0)
                    break;

                if (hoeDirt.plant(item.ItemId, player, false))
                    ConsumeItem(player, item);
            }
        }
    }

    // 自动施肥
    private void AutoFertilize(GameLocation location, Farmer player, Item item)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoFertilizeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (item.Stack <= 0)
                break;
            switch (item.QualifiedItemId)
            {
                // 树肥逻辑
                case "(O)805":
                    if (terrainFeature is Tree tree && !tree.fertilized.Value && tree.growthStage.Value < Tree.treeStage &&
                        tree.fertilize())
                        ConsumeItem(player, item);
                    break;
                // 其他肥料逻辑
                default:
                    if (terrainFeature is HoeDirt hoeDirt && hoeDirt.plant(item.ItemId, player, true))
                        ConsumeItem(player, item);
                    break;
            }
        }
    }

    // 自动收获作物
    private void AutoHarvestCrop(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestCropRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                // 自动收获花逻辑
                if (!config.AutoHarvestFlower && ItemRegistry.GetData(crop.indexOfHarvest.Value)?.Category == SObject.flowersCategory)
                    continue;
                if (crop.harvest((int)tile.X, (int)tile.Y, hoeDirt))
                {
                    hoeDirt.destroyCrop(true);
                    // 姜岛金核桃逻辑
                    if (location is IslandLocation && Game1.random.NextDouble() < 0.05)
                        player.team.RequestLimitedNutDrops("IslandFarming", location, (int)tile.X * 64, (int)tile.Y * 64, 5);
                }

                if (crop.hitWithHoe((int)tile.X, (int)tile.Y, location, hoeDirt))
                    hoeDirt.destroyCrop(true);
            }
        }
    }

    // 自动摇晃果树
    private void AutoShakeFruitTree(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoShakeFruitTreeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is FruitTree fruitTree && fruitTree.fruit.Count > 0)
                fruitTree.performUseAction(tile);
        }
    }

    // 自动清理枯萎作物
    private void AutoClearDeadCrop(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestCropRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                if (crop.dead.Value)
                    hoeDirt.performToolAction(FakeScythe.Value, 0, tile);
            }
        }
    }
}