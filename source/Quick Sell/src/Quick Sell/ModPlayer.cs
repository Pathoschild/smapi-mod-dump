/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Quick_Sell
{
    internal class ModPlayer
    {
        private readonly IModHelper Helper;
        private readonly ModConfig Config;

        private readonly ModUtils Utils;
        private ModLogger Logger;

        public ModPlayer(IModHelper helper, ModConfig config, ModLogger logger, ModUtils utils)
        {
            this.Config = config;
            this.Helper = helper;

            this.Logger = logger;

            this.Utils = utils;
        }

        public void RemoveItemFromPlayerInventory(Item item)
        {
            Game1.player.removeItemFromInventory(item);
        }

        public void AddItemToPlayerShippingBin(Item item)
        {
            Game1.getFarm().getShippingBin(Game1.player).Add(item);
        }

        public Item GetHeldItem()
        {
            Item heldItem = Game1.player.CurrentItem;

            return heldItem;
        }

        public Item GetHoveredItem()
        {
            IClickableMenu currentMenu = (Game1.activeClickableMenu as GameMenu)?.GetCurrentPage() ?? Game1.activeClickableMenu;
            Item currentItem = null;

            switch (currentMenu)
            {
                // Chests
                //case MenuWithInventory menu:
                //    currentItem = Game1.player.CursorSlotItem ?? menu.heldItem ?? menu.hoveredItem;
                //    break;

                //case ProfileMenu menu:
                //    currentItem = menu.hoveredItem;
                //    break;

                case InventoryPage menu:
                    currentItem = Game1.player.CursorSlotItem ?? this.Helper.Reflection.GetField<Item>(menu, "hoveredItem").GetValue();
                    break;

                default:
                    string message = "You are not in the inventory!";

                    this.Logger.Debug(message);
                    this.Utils.SendHUDMessage(message, HUDMessage.error_type);
                    break;
            }

            return currentItem;
        }

        public bool CheckIfItemCanBeShipped(Item item)
        {
            if (this.Config.CheckIfItemsCanBeShipped == true)
            {
                Object itemTmp = item as Object;

                if (itemTmp == null || itemTmp.canBeShipped() == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}