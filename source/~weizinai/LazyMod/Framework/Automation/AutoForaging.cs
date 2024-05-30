/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using LazyMod.Framework.Config;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

internal class AutoForaging : Automate
{
    public AutoForaging(ModConfig config, Func<int, List<Vector2>> getTileGrid): base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动觅食
        if (Config.AutoForage.IsEnable) AutoForage(location, player);
        // 自动砍树
        if (Config.AutoChopTree.IsEnable && tool is Axe) AutoChopTree(location, player, tool);
        // 自动收获姜
        if (Config.AutoHarvestGinger.IsEnable && (tool is Hoe || Config.FindToolForHarvestGinger)) AutoHarvestGinger(location, player);
        // 自动摇树
        if (Config.AutoShakeTree.IsEnable) AutoShakeTree(location);
        // 自动装备采集器
        if (Config.AutoPlaceTapper.IsEnable && item is SObject { QualifiedItemId: "(BC)105" or "(BC)264" } tapper) AutoPlaceTapper(location, player, tapper);
        // 自动收获苔藓
        if (Config.AutoHarvestMoss.IsEnable && (tool is MeleeWeapon || Config.FindScytheFromInventory)) AutoHarvestMoss(location);
        // 自动在树上浇醋
        if (Config.AutoPlaceVinegar.IsEnable && item is SObject { QualifiedItemId: "(O)419" } vinegar) AutoPlaceVinegar(location, player, vinegar);
        // 自动清理木头
        if (Config.AutoClearWood.IsEnable && (tool is Axe || Config.FindAxeFromInventory)) AutoClearWood(location, player);
    }

    // 自动觅食
    private void AutoForage(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(Config.AutoForage.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsSpawnedObject) CollectSpawnedObject(location, player, tile, obj);

            foreach (var terrainFeature in location.largeTerrainFeatures)
                if (terrainFeature is Bush bush && CanForageBerry(tile, bush))
                    bush.performUseAction(tile);
        }
    }

    // 自动收获姜
    private void AutoHarvestGinger(GameLocation location, Farmer player)
    {
        if (player.Stamina <= Config.StopHarvestGingerStamina) return;

        var hoe = FindToolFromInventory<Hoe>();
        if (hoe is null) return;

        var grid = GetTileGrid(Config.AutoHarvestGinger.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                if (hoeDirt.crop.hitWithHoe((int)tile.X, (int)tile.Y, location, hoeDirt)) hoeDirt.destroyCrop(true);
            }
        }
    }

    // 自动砍树
    private void AutoChopTree(GameLocation location, Farmer player, Tool tool)
    {
        if (player.Stamina <= Config.StopChopTreeStamina) return;

        var treeType = new Dictionary<string, Dictionary<int, bool>>
        {
            { Tree.bushyTree, Config.ChopOakTree },
            { Tree.leafyTree, Config.ChopMapleTree },
            { Tree.pineTree, Config.ChopPineTree },
            { Tree.mahoganyTree, Config.ChopMahoganyTree },
            { Tree.palmTree, Config.ChopPalmTree },
            { Tree.palmTree2, Config.ChopPalmTree },
            { Tree.mushroomTree, Config.ChopMushroomTree },
            { Tree.greenRainTreeBushy, Config.ChopGreenRainTree },
            { Tree.greenRainTreeLeafy, Config.ChopGreenRainTree },
            { Tree.greenRainTreeFern, Config.ChopGreenRainTree },
            { Tree.mysticTree, Config.ChopMysticTree }
        };

        var grid = GetTileGrid(Config.AutoChopTree.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree)
            {
                if (tree.tapped.Value && !Config.ChopTapperTree) continue;
                if (tree.stopGrowingMoss.Value && !Config.ChopVinegarTree) continue;

                foreach (var (key, value) in treeType)
                {
                    // 树逻辑
                    if (tree.treeType.Value.Equals(key))
                    {
                        foreach (var (stage, chopTree) in value)
                        {
                            if (tree.growthStage.Value < 5 && tree.growthStage.Value == stage && chopTree ||
                                tree.growthStage.Value >= 5 && !tree.stump.Value && value[5])
                            {
                                UseToolOnTile(location, player, tool, tile);
                                break;
                            }
                        }

                        break;
                    }

                    // 树桩逻辑
                    if (tree.stump.Value && value[-1])
                    {
                        Game1.chatBox.addInfoMessage("砍树桩");
                        UseToolOnTile(location, player, tool, tile);
                        break;
                    }
                }
            }
        }
    }

    // 自动摇树
    private void AutoShakeTree(GameLocation location)
    {
        var grid = GetTileGrid(Config.AutoShakeTree.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasSeed.Value)
                tree.performUseAction(tile);
        }
    }

    // 自动收获苔藓
    private void AutoHarvestMoss(GameLocation location)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var grid = GetTileGrid(Config.AutoHarvestMoss.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasMoss.Value)
                tree.performToolAction(scythe, 0, tile);
        }
    }

    // 自动放置采集器
    private void AutoPlaceTapper(GameLocation location, Farmer player, SObject tapper)
    {
        var grid = GetTileGrid(Config.AutoPlaceVinegar.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && !tree.tapped.Value)
            {
                var tilePixelPosition = GetTilePixelPosition(tile);
                if (tapper.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
            }
        }
    }

    // 自动在树上浇醋
    private void AutoPlaceVinegar(GameLocation location, Farmer player, SObject vinegar)
    {
        var grid = GetTileGrid(Config.AutoPlaceVinegar.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && !tree.stopGrowingMoss.Value)
            {
                var tilePixelPosition = GetTilePixelPosition(tile);
                if (vinegar.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
            }
        }
    }

    // 自动清理木头
    private void AutoClearWood(GameLocation location, Farmer player)
    {
        if (player.Stamina <= Config.StopClearWoodStamina) return;

        var axe = FindToolFromInventory<Axe>();
        if (axe is null) return;

        var grid = GetTileGrid(Config.AutoClearWood.Range);
        foreach (var tile in grid)
        {
            if (Config.ClearTwig)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is not null && obj.IsTwig())
                {
                    UseToolOnTile(location, player, axe, tile);
                }
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(GetTileBoundingBox(tile))) continue;

                var clear = false;
                var requiredUpgradeLevel = Tool.stone;

                if (Config.ClearStump && clump.parentSheetIndex.Value == ResourceClump.stumpIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.copper;
                }

                if (Config.ClearHollowLog && clump.parentSheetIndex.Value == ResourceClump.hollowLogIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.steel;
                }

                if (clear && axe.UpgradeLevel >= requiredUpgradeLevel)
                {
                    UseToolOnTile(location, player, axe, tile);
                    break;
                }
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

    private bool CanForageBerry(Vector2 tile, Bush bush)
    {
        return (Game1.season is Season.Spring || Game1.season is Season.Fall) &&
               bush.getBoundingBox().Intersects(GetTileBoundingBox(tile)) &&
               bush.tileSheetOffset.Value == 1 &&
               bush.size.Value == Bush.mediumBush &&
               !bush.townBush.Value;
    }
}