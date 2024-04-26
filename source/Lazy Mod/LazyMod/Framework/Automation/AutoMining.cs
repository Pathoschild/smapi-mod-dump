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
using StardewValley.Objects;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoMining : Automate
{
    private readonly ModConfig config;

    public AutoMining(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;
        
        // 自动收集煤炭
        if (config.AutoCollectCoal) AutoCollectCoal(location, player);
        // 自动破坏容器
        if (config.AutoBreakContainer && (tool is MeleeWeapon || config.FindWeaponFromInventory)) AutoBreakContainer(location, player);
        // 自动打开宝藏
        if (config.AutoOpenTreasure) AutoOpenTreasure(location, player);
        // 自动清理水晶
        if (config.AutoClearCrystal) AutoClearCrystal(location, player);
    }

    // 自动收集煤炭
    private void AutoCollectCoal(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoCollectCoalRange);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 194)
                CheckTileAction(location, player, tile);
        }
    }
    
    // 自动破坏容器
    private void AutoBreakContainer(GameLocation location, Farmer player)
    {
        var weapon = FindToolFromInventory<MeleeWeapon>();
        if (weapon is null) return;
        
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoBreakContainerRange);
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
        // if (location is not MineShaft) return;
        
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoBreakContainerRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null || obj.QualifiedItemId != "(O)-1") continue;
            obj.checkForAction(player);
        }
    }
    
    // 自动清理水晶
    private void AutoClearCrystal(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearCrystalRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)319" or "(O)320" or "(O)321")
            {
                obj.performToolAction(FakeScythe.Value);
                location.removeObject(tile, false);
            }
        }
    }
}