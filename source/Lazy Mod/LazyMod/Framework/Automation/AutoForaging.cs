/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoForaging : Automate
{
    private readonly ModConfig config;

    public AutoForaging(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;

        // 自动觅食
        if (config.AutoForage) AutoForage(location, player);
        // 自动摇树
        if (config.AutoShakeTree) AutoShakeTree(location, player);
        // 自动收获苔藓
        if (config.AutoHarvestMoss && (tool is MeleeWeapon || config.FindScytheFromInventory)) AutoHarvestMoss(location, player);
        // 自动在树上浇醋
        if (config.AutoUseVinegarOnTree && item is SObject obj && item.QualifiedItemId is "(O)419") AutoUseVinegarOnTree(location, player, obj);
        // 自动清理树枝
        if (config.AutoClearTwig && (tool is Axe || config.FindAxeFromInventory)) AutoClearTwig(location, player);
        // 自动清理树种
        if (config.AutoClearTreeSeed && tool is Axe) AutoClearTreeSeed(location, player, tool);
    }

    // 自动觅食
    private void AutoForage(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoForageRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsSpawnedObject) CollectSpawnedObject(location, player, tile, obj);

            foreach (var terrainFeature in location.largeTerrainFeatures)
                if (terrainFeature is Bush bush && bush.getBoundingBox().Intersects(GetTileBoundingBox(tile)) &&
                    bush.tileSheetOffset.Value == 1 && bush.size.Value == Bush.mediumBush && !bush.townBush.Value)
                    bush.performUseAction(tile);
        }
    }

    // 自动摇树
    private void AutoShakeTree(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoShakeFruitTreeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasSeed.Value)
                tree.performUseAction(tile);
        }
    }

    // 自动收获苔藓
    private void AutoHarvestMoss(GameLocation location, Farmer player)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestMossRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasMoss.Value)
                tree.performToolAction(scythe, 0, tile);
        }
    }

    // 自动在树上浇醋
    private void AutoUseVinegarOnTree(GameLocation location, Farmer player, SObject vinegar)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoUseVinegarOnTreeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && !tree.stopGrowingMoss.Value)
            {
                var tilePixelPosition = GetTilePixelPosition(tile);
                vinegar.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player);
                ConsumeItem(player, vinegar);
            }
        }
    }

    // 自动清理树枝
    private void AutoClearTwig(GameLocation location, Farmer player)
    {
        var axe = FindToolFromInventory<Axe>();
        if (axe is null) return;

        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearTwigRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsTwig())
            {
                if (StopAutomate(player, config.StopAutoClearTreeSeedStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, axe, tile);
            }
        }
    }

    // 自动清理树种
    private void AutoClearTreeSeed(GameLocation location, Farmer player, Tool tool)
    {
        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearTreeSeedRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.growthStage.Value == Tree.seedStage)
            {
                if (StopAutomate(player, config.StopAutoClearTreeSeedStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, tool, tile);
            }
        }
    }

    private void CollectSpawnedObject(GameLocation location, Farmer player, Vector2 tile, SObject obj)
    {
        var oldQuality = obj.Quality;
        var random = Utility.CreateDaySaveRandom(tile.X, tile.Y * 777f);
        // 物品质量逻辑
        if (player.professions.Contains(16) && obj.isForage())
        {
            obj.Quality = 4;
        }
        else if (obj.isForage())
        {
            if (random.NextDouble() < player.ForagingLevel / 30f)
            {
                obj.Quality = 2;
            }
            else if (random.NextDouble() < player.ForagingLevel / 15f)
            {
                obj.Quality = 1;
            }
        }

        // 任务物品逻辑
        if (obj.questItem.Value && obj.questId.Value != null && obj.questId.Value != "0" && !player.hasQuest(obj.questId.Value)) return;

        if (player.couldInventoryAcceptThisItem(obj))
        {
            if (player.IsLocalPlayer)
            {
                location.localSound("pickUpItem");
                DelayedAction.playSoundAfterDelay("coin", 300);
            }

            player.animateOnce(279 + player.FacingDirection);
            if (!location.isFarmBuildingInterior())
            {
                if (obj.isForage())
                {
                    if (obj.SpecialVariable == 724519)
                    {
                        player.gainExperience(2, 2);
                        player.gainExperience(0, 3);
                    }
                    else
                    {
                        player.gainExperience(2, 7);
                    }
                }

                // 紫色短裤逻辑
                if (obj.ItemId.Equals("789") && location.Name.Equals("LewisBasement"))
                {
                    var bat = new Bat(Vector2.Zero, -789)
                    {
                        focusedOnFarmers = true
                    };
                    Game1.changeMusicTrack("none");
                    location.playSound("cursed_mannequin");
                    location.characters.Add(bat);
                }
            }
            else
            {
                player.gainExperience(0, 5);
            }

            player.addItemToInventoryBool(obj.getOne());
            Game1.stats.ItemsForaged++;
            if (player.professions.Contains(13) && random.NextDouble() < 0.2 && !obj.questItem.Value && player.couldInventoryAcceptThisItem(obj) &&
                !location.isFarmBuildingInterior())
            {
                player.addItemToInventoryBool(obj.getOne());
                player.gainExperience(2, 7);
            }

            location.objects.Remove(tile);
            return;
        }

        obj.Quality = oldQuality;
    }
}