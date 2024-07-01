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
using StardewValley.Menus;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoFishing : Automate
{
    public AutoFishing(ModConfig config, Func<int, List<Vector2>> getTileGrid): base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动放置蟹笼
        if (this.Config.AutoPlaceCarbPot.IsEnable && item is SObject { QualifiedItemId: "(O)710" } crabPot) this.AutoPlaceCrabPot(location, player, crabPot);
        // 自动添加蟹笼鱼饵
        if (this.Config.AutoAddBaitForCarbPot.IsEnable && item is SObject { Category: SObject.baitCategory } bait) this.AutoAddBaitForCrabPot(location, player, bait);
        // 自动收获蟹笼
        if (this.Config.AutoHarvestCarbPot.IsEnable) this.AutoHarvestCarbPot(location, player);
    }

    public void AutoMenuFunction()
    {
        if (Game1.activeClickableMenu is ItemGrabMenu { source: ItemGrabMenu.source_fishingChest } menu)
        {
            if (this.Config.AutoGrabTreasureItem) this.AutoGrabTreasureItem(menu);
            if (this.Config.AutoExitTreasureMenu) this.AutoExitTreasureMenu(menu);
        }
    }

    // 自动抓取宝箱物品
    private void AutoGrabTreasureItem(ItemGrabMenu menu)
    {
        var items = menu.ItemsToGrabMenu.actualInventory;
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i] is null) continue;
            if (!this.CanAddItemToInventory(items[i])) break;

            var center = menu.ItemsToGrabMenu.inventory[i].bounds.Center;
            menu.receiveLeftClick(center.X, center.Y);
        }
    }

    // 自动退出宝箱菜单
    private void AutoExitTreasureMenu(ItemGrabMenu menu)
    {
        var hasItem = menu.ItemsToGrabMenu.actualInventory.OfType<Item>().Any();
        if (!hasItem) menu.exitThisMenu();
    }

    // 自动放置蟹笼
    private void AutoPlaceCrabPot(GameLocation location, Farmer player, SObject crabPot)
    {
        var grid = this.GetTileGrid(this.Config.AutoPlaceCarbPot.Range);
        foreach (var _ in grid.Select(tile => this.GetTilePixelPosition(tile))
                     .Where(tilePixelPosition => crabPot.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)))
        {
            player.reduceActiveItemByOne();
        }
    }

    // 自动添加蟹笼鱼饵
    private void AutoAddBaitForCrabPot(GameLocation location, Farmer player, SObject bait)
    {
        var grid = this.GetTileGrid(this.Config.AutoAddBaitForCarbPot.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not CrabPot crabPot || crabPot.bait.Value is not null) continue;
            if (obj.performObjectDropInAction(bait, false, player)) this.ConsumeItem(player, bait);
        }
    }

    // 自动收获蟹笼
    private void AutoHarvestCarbPot(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoHarvestCarbPot.Range);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is CrabPot) this.HarvestMachine(player, obj);
        }
    }
}