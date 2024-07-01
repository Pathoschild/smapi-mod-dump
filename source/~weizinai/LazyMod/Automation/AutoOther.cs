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
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.GameData.Machines;
using StardewValley.Objects;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoOther : Automate
{
    private const string UniqueBuffId = "weizinai.LazyMod";

    public AutoOther(ModConfig config, Func<int, List<Vector2>> getTileGrid): base(config, getTileGrid) 
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 增加磁力范围
        this.MagneticRadiusIncrease(player);
        // 自动清理杂草
        if (this.Config.AutoClearWeeds.IsEnable && (tool is MeleeWeapon || this.Config.AutoClearWeeds.FindToolFromInventory)) this.AutoClearWeeds(location);
        // 自动挖掘斑点
        if (this.Config.AutoDigSpots.IsEnable && (tool is Hoe || this.Config.AutoDigSpots.FindToolFromInventory)) this.AutoDigSpots(location, player);
        // 自动收获机器
        if (this.Config.AutoHarvestMachine.IsEnable) this.AutoHarvestMachine(location, player);
        // 自动触发机器
        if (this.Config.AutoTriggerMachine.IsEnable && item is not null) this.AutoTriggerMachine(location, player, item);
        // 自动使用仙尘
        if (this.Config.AutoUseFairyDust.IsEnable && item?.QualifiedItemId is "(O)872") this.AutoUseFairyDust(location, player);
        // 自动翻垃圾桶
        if (this.Config.AutoGarbageCan.IsEnable) this.AutoGarbageCan(location, player);
        // 自动放置地板
        if (this.Config.AutoPlaceFloor.IsEnable && item is SObject floor && floor.IsFloorPathItem()) this.AutoPlaceFloor(location, player, floor);
    }

    // 增加磁力范围
    private void MagneticRadiusIncrease(Farmer player)
    {
        if (this.Config.MagneticRadiusIncrease == 0)
        {
            player.buffs.Remove(UniqueBuffId);
            return;
        }

        player.buffs.AppliedBuffs.TryGetValue(UniqueBuffId, out var buff);
        if (buff is null || buff.millisecondsDuration <= 5000 || Math.Abs(buff.effects.MagneticRadius.Value - this.Config.MagneticRadiusIncrease) > 0.1f)
        {
            buff = new Buff(
                id: UniqueBuffId,
                source: "Lazy Mod",
                duration: 60000,
                effects: new BuffEffects
                {
                    MagneticRadius = { Value = this.Config.MagneticRadiusIncrease * 64 }
                });
            player.applyBuff(buff);
        }
    }

    // 自动清理杂草
    private void AutoClearWeeds(GameLocation location)
    {
        var scythe = this.FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoClearWeeds.Range);
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
                if (!clump.getBoundingBox().Intersects(this.GetTileBoundingBox(tile))) continue;

                if (this.Config.ClearLargeWeeds && clump.parentSheetIndex.Value is 44 or 46)
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
        if (player.Stamina <= this.Config.AutoDigSpots.StopStamina) return;

        var hoe = this.FindToolFromInventory<Hoe>();
        if (hoe is null)
            return;

        var grid = this.GetTileGrid(this.Config.AutoDigSpots.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is not ("(O)590" or "(O)SeedSpot")) continue;
            this.UseToolOnTile(location, player, hoe, tile);
        }
    }

    // 自动收获机器
    private void AutoHarvestMachine(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoHarvestMachine.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) continue;
            this.HarvestMachine(player, obj);
        }
    }

    // 自动触发机器
    private void AutoTriggerMachine(GameLocation location, Farmer player, Item item)
    {
        var grid = this.GetTileGrid(this.Config.AutoTriggerMachine.Range);
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

    // 自动使用仙尘
    private void AutoUseFairyDust(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoUseFairyDust.Range);

        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj.TryApplyFairyDust()) player.reduceActiveItemByOne();
        }
    }

    // 自动翻垃圾桶
    private void AutoGarbageCan(GameLocation location, Farmer player)
    {
        if (this.CheckNPCNearTile(location, player) && this.Config.StopGarbageCanNearVillager) return;
        var grid = this.GetTileGrid(this.Config.AutoGarbageCan.Range);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 78)
            {
                var action = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");
                if (action?.StartsWith("Garbage") ?? false) this.CheckTileAction(location, player, tile);
            }
        }
    }

    // 自动放置地板
    private void AutoPlaceFloor(GameLocation location, Farmer player, SObject floor)
    {
        var grid = this.GetTileGrid(this.Config.AutoPlaceFloor.Range);
        foreach (var tile in grid)
        {
            var tilePixelPosition = this.GetTilePixelPosition(tile);
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