/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace Quick_Sell
{
    internal class ModPlayer
    {
        public static Item GetHoveredItem()
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
                    currentItem = Game1.player.CursorSlotItem ?? ModEntry.Instance.Helper.Reflection.GetField<Item>(menu, "hoveredItem").GetValue();
                    break;

                default:
                    string message = "Quick Sell: You are not in the inventory!";

                    ModLogger.Trace(message);
                    ModUtils.SendHUDMessageRespectingConfig(message, HUDMessage.error_type);
                    break;
            }

            return currentItem;
        }

        public static bool CheckIfItemCanBeShipped(Item item)
        {
            Object itemAsObject = item as Object;

            if (itemAsObject == null || itemAsObject.canBeShipped() == false)
            {
                return false;
            }

            return true;
        }
    }
}