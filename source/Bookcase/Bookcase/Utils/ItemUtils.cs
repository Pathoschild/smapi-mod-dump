/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace Bookcase.Utils {

    public class ItemUtils {

        /// <summary>
        /// This method allows you to get the selling price of an item.
        /// </summary>
        /// <param name="item">The item to get the selling price of.</param>
        /// <param name="defaultToBuying">Whether or not this should default to half of the buying price.</param>
        /// <returns>The price of the item.</returns>
        public static int GetSellPrice(Item item, bool defaultToBuying) {

            // Check if the item has a sell to store price.
            if (item is StardewValley.Object obj) {

                return obj.sellToStorePrice();
            }

            // If defaultToSale is true, do half of the player buying price. Otherwise this is 0.
            return defaultToBuying ? item.salePrice() / 2 : 0;
        }

        /// <summary>
        /// Attempts to get the item that the player is hovering over.
        /// </summary>
        /// <returns>The item that the player is hovering over. This can be a null value!</returns>
        public static Item GetHoveredItem() {

            Item hoveredItem = null;

            // Search through all the on screen huds
            foreach (IClickableMenu hud in Game1.onScreenMenus) {

                // Found the toolbar?
                if (hud is Toolbar toolbar) {

                    // Set item to hovered toolbar item, and break out of loop.
                    hoveredItem = BookcaseMod.reflection.GetField<Item>(toolbar, "hoverItem").GetValue();
                }
            }

            // Attempt to get hovered item from an inventory (chest) GUI
            if (Game1.activeClickableMenu is ItemGrabMenu) {

                hoveredItem = (Game1.activeClickableMenu as MenuWithInventory).hoveredItem;
            }

            // Attempt to get hovered item from an inventory pages (player inventory)
            if (Game1.activeClickableMenu is GameMenu) {

                List<IClickableMenu> menuList = BookcaseMod.reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();

                foreach (var menu in menuList) {

                    if (menu is InventoryPage) {

                        hoveredItem = BookcaseMod.reflection.GetField<Item>(menu, "hoveredItem").GetValue();
                    }
                }
            }

            return hoveredItem;
        }
    }
}