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
using StardewValley.Objects;
using StardewValley.Tools;
using Common;
using LazyMod.Framework.Config;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace LazyMod.Framework.Automation;

internal class AutoMining : Automate
{
    public AutoMining(ModConfig config, Func<int, List<Vector2>> getTileGrid): base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动清理石头
        if (Config.AutoClearStone.IsEnable && (tool is Pickaxe || Config.FindPickaxeFromInventory)) AutoClearStone(location, player);
        // 自动收集煤炭
        if (Config.AutoCollectCoal.IsEnable) AutoCollectCoal(location, player);
        // 自动破坏容器
        if (Config.AutoBreakContainer.IsEnable && (tool is MeleeWeapon || Config.FindToolForBreakContainer)) AutoBreakContainer(location);
        // 自动打开宝藏
        if (Config.AutoOpenTreasure.IsEnable) AutoOpenTreasure(location, player);
        // 自动清理水晶
        if (Config.AutoClearCrystal.IsEnable) AutoClearCrystal(location);
        // 自动冷却岩浆
        if (Config.AutoCoolLava.IsEnable && (tool is WateringCan || Config.FindToolForCoolLava)) AutoCoolLava(location, player);
    }

    // 自动清理石头
    private void AutoClearStone(GameLocation location, Farmer player)
    {
        if (player.Stamina <= Config.StopClearStoneStamina) return;
        if (!Config.ClearStoneOnMineShaft && location is MineShaft) return;
        if (!Config.ClearStoneOnVolcano && location is VolcanoDungeon) return;

        var pickaxe = FindToolFromInventory<Pickaxe>();
        if (pickaxe is null) return;

        var stoneTypes = new Dictionary<HashSet<string>, bool>
        {
            { ItemRepository.FarmStone, Config.ClearFarmStone },
            { ItemRepository.OtherStone, Config.ClearOtherStone },
            { ItemRepository.IslandStone, Config.ClearIslandStone },
            { ItemRepository.OreStone, Config.ClearOreStone },
            { ItemRepository.GemStone, Config.ClearGemStone },
            { ItemRepository.GeodeStone, Config.ClearGeodeStone },
            { ItemRepository.CalicoEggStone, Config.ClearCalicoEggStone }
        };

        var grid = GetTileGrid(Config.AutoClearStone.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null)
            {
                foreach (var stoneType in stoneTypes)
                {
                    if (stoneType.Value && stoneType.Key.Contains(obj.QualifiedItemId))
                    {
                        UseToolOnTile(location, player, pickaxe, tile);
                        break;
                    }
                }
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(GetTileBoundingBox(tile))) continue;

                var clear = false;
                var requiredUpgradeLevel = Tool.stone;

                if (Config.ClearMeteorite && clump.parentSheetIndex.Value == ResourceClump.meteoriteIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.gold;
                }
                else
                    switch (Config.ClearBoulder)
                    {
                        case true when clump.parentSheetIndex.Value == ResourceClump.boulderIndex:
                            clear = true;
                            requiredUpgradeLevel = Tool.steel;
                            break;
                        case true when ResourceClumpRepository.MineBoulder.Contains(clump.parentSheetIndex.Value):
                            clear = true;
                            requiredUpgradeLevel = Tool.stone;
                            break;
                    }

                if (clear && pickaxe.UpgradeLevel >= requiredUpgradeLevel)
                {
                    UseToolOnTile(location, player, pickaxe, tile);
                    break;
                }
            }
        }
    }

    // 自动收集煤炭
    private void AutoCollectCoal(GameLocation location, Farmer player)
    {
        if (location is not MineShaft) return;
        
        var grid = GetTileGrid(Config.AutoCollectCoal.Range);
        foreach (var tile in grid)
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 194)
                CheckTileAction(location, player, tile);
    }

    // 自动破坏容器
    private void AutoBreakContainer(GameLocation location)
    {
        var weapon = FindToolFromInventory<MeleeWeapon>();
        if (weapon is null) return;

        var grid = GetTileGrid(Config.AutoBreakContainer.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is BreakableContainer)
                obj.performToolAction(weapon);
        }
    }

    // 自动打开宝藏
    private void AutoOpenTreasure(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(Config.AutoBreakContainer.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null || obj.QualifiedItemId != "(O)-1") continue;
            obj.checkForAction(player);
        }
    }

    // 自动清理水晶
    private void AutoClearCrystal(GameLocation location)
    {
        var tool = FindToolFromInventory<MeleeWeapon>();
        if (tool is null) return;

        var grid = GetTileGrid(Config.AutoClearCrystal.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)319" or "(O)320" or "(O)321")
            {
                if (obj.performToolAction(tool)) location.removeObject(tile, false);
            }
        }
    }

    // 自动冷却岩浆
    private void AutoCoolLava(GameLocation location, Farmer player)
    {
        if (location is not VolcanoDungeon dungeon) return;
        var wateringCan = FindToolFromInventory<WateringCan>();
        if (wateringCan is null) return;

        var hasAddWaterMessage = true;
        var grid = GetTileGrid(Config.AutoCoolLava.Range);
        foreach (var tile in grid)
        {
            if (wateringCan.WaterLeft <= 0)
            {
                if (!hasAddWaterMessage) Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335");
                break;
            }

            hasAddWaterMessage = false;

            if (player.Stamina <= Config.StopCoolLavaStamina) return;
            if (!CanCoolLave(dungeon, tile)) continue;
            UseToolOnTile(location, player, wateringCan, tile);
            if (wateringCan.WaterLeft > 0 && player.ShouldHandleAnimationSound())
                player.playNearbySoundLocal("wateringCan");
        }
    }

    private bool CanCoolLave(VolcanoDungeon dungeon, Vector2 tile)
    {
        var x = (int)tile.X;
        var y = (int)tile.Y;
        return !dungeon.CanRefillWateringCanOnTile(x, y) &&
               dungeon.isTileOnMap(tile) &&
               dungeon.waterTiles[x, y] &&
               !dungeon.cooledLavaTiles.ContainsKey(tile);
    }
}