/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/itsbenter/ResourcefulFriends
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ResourcefulFriends
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.InventoryChanged += this.Player_InventoryChanged;
            helper.Events.World.ChestInventoryChanged += this.OnChestInventoryChanged;
            helper.Events.Display.MenuChanged += this.Display_MenuChanged;
        }
        private void Player_InventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            foreach (Item item in e.Added)
            {
                if (item.HasContextTag("ResourcefulFriends_DisallowQuality") && item.Quality != 0)
                {
                    Item newItem = ItemRegistry.Create(item.ItemId, item.Stack, 0);
                    //item.quality.Value = 0;
                    //remove and add to avoid multi stack of same item
                    Game1.player.removeItemFromInventory(item);
                    Game1.player.addItemByMenuIfNecessary(newItem);
                }
            }
        }
        private void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
        {
            Chest chest = e.Chest;
            foreach (Item item in e.Added)
            {
                if (item.HasContextTag("ResourcefulFriends_DisallowQuality") && item.Quality != 0)
                {
                    Item newItem = ItemRegistry.Create(item.ItemId, item.Stack, 0);
                    //item.quality.Value = 0;
                    //remove and add to avoid multi stack of same item
                    chest.Items.Remove(item);
                    chest.Items.Add(newItem);
                }
            }
        }
        private void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            //To change quality of existing item already in chest
            if (Game1.activeClickableMenu is ItemGrabMenu menu)
            {
                if (menu.sourceItem is Chest chest)
                {
                    chest.ForEachItem((item, remove, replaceWith) =>
                    {
                        if (item.HasContextTag("ResourcefulFriends_DisallowQuality") && item.Quality != 0)
                        {
                            Item newItem = ItemRegistry.Create(item.ItemId, item.Stack, 0);
                            replaceWith(newItem);
                        }
                        return true;
                    });
                }
                //To change autograbber item
                if (Game1.player.currentLocation is AnimalHouse location)
                {
                    foreach (StardewValley.Object obj in location.objects.Values)
                    {
                        if (obj.QualifiedItemId == "(BC)165" && obj.heldObject.Value is Chest heldChest)
                        {
                            heldChest.ForEachItem((item, remove, replaceWith) =>
                            {
                                if (item.HasContextTag("ResourcefulFriends_DisallowQuality") && item.Quality != 0)
                                {
                                    Item newItem = ItemRegistry.Create(item.ItemId, item.Stack, 0);
                                    replaceWith(newItem);
                                }
                                return true;
                            });
                        }
                    }
                }
            }
        }
    }
}
