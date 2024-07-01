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
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoForaging : Automate
{
    public AutoForaging(ModConfig config, Func<int, List<Vector2>> getTileGrid): base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动觅食
        if (this.Config.AutoForage.IsEnable) this.AutoForage(location, player);
        // 自动砍树
        if (this.Config.AutoChopTree.IsEnable && tool is Axe) this.AutoChopTree(location, player, tool);
        // 自动收获姜
        if (this.Config.AutoHarvestGinger.IsEnable && (tool is Hoe || this.Config.AutoHarvestGinger.FindToolFromInventory)) this.AutoHarvestGinger(location, player);
        // 自动摇树
        if (this.Config.AutoShakeTree.IsEnable) this.AutoShakeTree(location);
        // 自动装备采集器
        if (this.Config.AutoPlaceTapper.IsEnable && item is SObject { QualifiedItemId: "(BC)105" or "(BC)264" } tapper) this.AutoPlaceTapper(location, player, tapper);
        // 自动收获苔藓
        if (this.Config.AutoHarvestMoss.IsEnable && (tool is MeleeWeapon || this.Config.AutoHarvestMoss.FindToolFromInventory)) this.AutoHarvestMoss(location);
        // 自动在树上浇醋
        if (this.Config.AutoPlaceVinegar.IsEnable && item is SObject { QualifiedItemId: "(O)419" } vinegar) this.AutoPlaceVinegar(location, player, vinegar);
        // 自动清理木头
        if (this.Config.AutoClearWood.IsEnable && (tool is Axe || this.Config.AutoClearWeeds.FindToolFromInventory)) this.AutoClearWood(location, player);
    }

    // 自动觅食
    private void AutoForage(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoForage.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsSpawnedObject) this.CollectSpawnedObject(location, player, tile, obj);

            foreach (var terrainFeature in location.largeTerrainFeatures)
                if (terrainFeature is Bush bush && this.CanForageBerry(tile, bush))
                    bush.performUseAction(tile);
        }
    }

    // 自动收获姜
    private void AutoHarvestGinger(GameLocation location, Farmer player)
    {
        if (player.Stamina <= this.Config.AutoHarvestGinger.StopStamina) return;

        var hoe = this.FindToolFromInventory<Hoe>();
        if (hoe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoHarvestGinger.Range);
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
        if (player.Stamina <= this.Config.AutoChopTree.StopStamina) return;

        var treeType = new Dictionary<string, Dictionary<int, bool>>
        {
            { Tree.bushyTree, this.Config.ChopOakTree },
            { Tree.leafyTree, this.Config.ChopMapleTree },
            { Tree.pineTree, this.Config.ChopPineTree },
            { Tree.mahoganyTree, this.Config.ChopMahoganyTree },
            { Tree.palmTree, this.Config.ChopPalmTree },
            { Tree.palmTree2, this.Config.ChopPalmTree },
            { Tree.mushroomTree, this.Config.ChopMushroomTree },
            { Tree.greenRainTreeBushy, this.Config.ChopGreenRainTree },
            { Tree.greenRainTreeLeafy, this.Config.ChopGreenRainTree },
            { Tree.greenRainTreeFern, this.Config.ChopGreenRainTree },
            { Tree.mysticTree, this.Config.ChopMysticTree }
        };

        var grid = this.GetTileGrid(this.Config.AutoChopTree.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree)
            {
                if (tree.tapped.Value && !this.Config.ChopTapperTree) continue;
                if (tree.stopGrowingMoss.Value && !this.Config.ChopVinegarTree) continue;

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
                                this.UseToolOnTile(location, player, tool, tile);
                                break;
                            }
                        }

                        break;
                    }

                    // 树桩逻辑
                    if (tree.stump.Value && value[-1])
                    {
                        this.UseToolOnTile(location, player, tool, tile);
                        break;
                    }
                }
            }
        }
    }

    // 自动摇树
    private void AutoShakeTree(GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoShakeTree.Range);
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
        var scythe = this.FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoHarvestMoss.Range);
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
        var grid = this.GetTileGrid(this.Config.AutoPlaceVinegar.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && !tree.tapped.Value)
            {
                var tilePixelPosition = this.GetTilePixelPosition(tile);
                if (tapper.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
            }
        }
    }

    // 自动在树上浇醋
    private void AutoPlaceVinegar(GameLocation location, Farmer player, SObject vinegar)
    {
        var grid = this.GetTileGrid(this.Config.AutoPlaceVinegar.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && !tree.stopGrowingMoss.Value)
            {
                var tilePixelPosition = this.GetTilePixelPosition(tile);
                if (vinegar.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
            }
        }
    }

    // 自动清理木头
    private void AutoClearWood(GameLocation location, Farmer player)
    {
        if (player.Stamina <= this.Config.AutoClearWood.StopStamina) return;

        var axe = this.FindToolFromInventory<Axe>();
        if (axe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoClearWood.Range);
        foreach (var tile in grid)
        {
            if (this.Config.ClearTwig)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is not null && obj.IsTwig())
                {
                    this.UseToolOnTile(location, player, axe, tile);
                }
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(this.GetTileBoundingBox(tile))) continue;

                var clear = false;
                var requiredUpgradeLevel = Tool.stone;

                if (this.Config.ClearStump && clump.parentSheetIndex.Value == ResourceClump.stumpIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.copper;
                }

                if (this.Config.ClearHollowLog && clump.parentSheetIndex.Value == ResourceClump.hollowLogIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.steel;
                }

                if (clear && axe.UpgradeLevel >= requiredUpgradeLevel)
                {
                    this.UseToolOnTile(location, player, axe, tile);
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
               bush.getBoundingBox().Intersects(this.GetTileBoundingBox(tile)) &&
               bush.tileSheetOffset.Value == 1 &&
               bush.size.Value == Bush.mediumBush &&
               !bush.townBush.Value;
    }
}