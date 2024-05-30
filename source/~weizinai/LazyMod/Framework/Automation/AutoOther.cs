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
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.GameData.Machines;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

internal class AutoOther : Automate
{
    private const string UniqueBuffId = "weizinai.LazyMod";

    public AutoOther(ModConfig config, Func<int, List<Vector2>> getTileGrid): base(config, getTileGrid) 
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 增加磁力范围
        MagneticRadiusIncrease(player);
        // 自动清理杂草
        if (Config.AutoClearWeeds.IsEnable && (tool is MeleeWeapon || Config.FindToolForClearWeeds)) AutoClearWeeds(location);
        // 自动挖掘斑点
        if (Config.AutoDigSpots.IsEnable && (tool is Hoe || Config.FindHoeFromInventory)) AutoDigSpots(location, player);
        // 自动收获机器
        if (Config.AutoHarvestMachine.IsEnable) AutoHarvestMachine(location, player);
        // 自动触发机器
        if (Config.AutoTriggerMachine.IsEnable && item is not null) AutoTriggerMachine(location, player, item);
        // 自动翻垃圾桶
        if (Config.AutoGarbageCan.IsEnable) AutoGarbageCan(location, player);
        // 自动放置地板
        if (Config.AutoPlaceFloor.IsEnable && item is SObject floor && floor.IsFloorPathItem()) AutoPlaceFloor(location, player, floor);
    }

    // 增加磁力范围
    private void MagneticRadiusIncrease(Farmer player)
    {
        if (Config.MagneticRadiusIncrease == 0)
        {
            player.buffs.Remove(UniqueBuffId);
            return;
        }

        player.buffs.AppliedBuffs.TryGetValue(UniqueBuffId, out var buff);
        if (buff is null || buff.millisecondsDuration <= 5000 || Math.Abs(buff.effects.MagneticRadius.Value - Config.MagneticRadiusIncrease) > 0.1f)
        {
            buff = new Buff(
                id: UniqueBuffId,
                source: "Lazy Mod",
                duration: 60000,
                effects: new BuffEffects
                {
                    MagneticRadius = { Value = Config.MagneticRadiusIncrease * 64 }
                });
            player.applyBuff(buff);
        }
    }

    // 自动清理杂草
    private void AutoClearWeeds(GameLocation location)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var grid = GetTileGrid(Config.AutoClearWeeds.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsWeeds() && obj.QualifiedItemId is not ("(O)319" or "(O)320" or "(O)321"))
            {
                obj.performToolAction(scythe);
                location.removeObject(tile, false);
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(GetTileBoundingBox(tile))) continue;

                if (Config.ClearLargeWeeds && clump.parentSheetIndex.Value is 44 or 46)
                {
                    scythe.swingTicker++;
                    if (clump.performToolAction(scythe, 1, tile))
                    {
                        location.resourceClumps.Remove(clump);
                        break;
                    }
                }
            }
        }
    }

    // 自动挖掘斑点
    private void AutoDigSpots(GameLocation location, Farmer player)
    {
        if (player.Stamina <= Config.StopDigSpotsStamina) return;

        var hoe = FindToolFromInventory<Hoe>();
        if (hoe is null)
            return;

        var grid = GetTileGrid(Config.AutoDigSpots.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is not ("(O)590" or "(O)SeedSpot")) continue;
            UseToolOnTile(location, player, hoe, tile);
        }
    }

    // 自动收获机器
    private void AutoHarvestMachine(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(Config.AutoHarvestMachine.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) continue;
            HarvestMachine(player, obj);
        }
    }

    // 自动触发机器
    private void AutoTriggerMachine(GameLocation location, Farmer player, Item item)
    {
        var grid = GetTileGrid(Config.AutoTriggerMachine.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null) continue;
            var machineData = obj.GetMachineData();
            if (machineData is null) continue;

            if (machineData.AdditionalConsumedItems is not null &&
                !MachineDataUtility.HasAdditionalRequirements(SObject.autoLoadFrom ?? player.Items, machineData.AdditionalConsumedItems, out _))
                continue;

            if (obj.PlaceInMachine(machineData, item, false, player))
            {
                MachineDataUtility.TryGetMachineOutputRule(obj, machineData, MachineOutputTrigger.ItemPlacedInMachine, item, player, location,
                    out _, out var triggerRule, out _, out _);
                if (item.Stack <= triggerRule?.RequiredCount) break;
            }
        }
    }

    // 自动翻垃圾桶
    private void AutoGarbageCan(GameLocation location, Farmer player)
    {
        if (CheckNPCNearTile(location, player) && Config.StopGarbageCanNearVillager) return;
        var grid = GetTileGrid(Config.AutoGarbageCan.Range);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 78)
            {
                var action = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");
                if (action?.StartsWith("Garbage") ?? false) CheckTileAction(location, player, tile);
            }
        }
    }

    // 自动放置地板
    private void AutoPlaceFloor(GameLocation location, Farmer player, SObject floor)
    {
        var grid = GetTileGrid(Config.AutoPlaceFloor.Range);
        foreach (var tile in grid)
        {
            var tilePixelPosition = GetTilePixelPosition(tile);
            if (floor.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
        }
    }

    /// <summary>
    /// 检测周围是否有NPC
    /// </summary>
    /// <returns>如果有,则返回true,否则返回false</returns>
    private bool CheckNPCNearTile(GameLocation location, Farmer player)
    {
        var tile = player.Tile;
        var npcs = Utility.GetNpcsWithinDistance(tile, 7, location).ToList();
        if (!npcs.Any()) return false;
        var horse = npcs.FirstOrDefault(npc => npc is Horse);
        return horse is null || npcs.Count != 1;
    }
}