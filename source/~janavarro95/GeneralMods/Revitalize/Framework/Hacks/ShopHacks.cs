/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revitalize;
using Revitalize.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Revitalize.Framework.Hacks
{
    /// <summary>
    /// Deals with modifications for SDV shops.
    /// </summary>
    public class ShopHacks
    {

        public static void OnNewMenuOpened(object Sender, StardewModdingAPI.Events.MenuChangedEventArgs args)
        {
            if (args.NewMenu != null)
            {
                if (args.NewMenu is ShopMenu)
                {
                    ShopMenu menu = (args.NewMenu as ShopMenu);
                    if (menu.portraitPerson != null)
                    {
                        string npcName = menu.portraitPerson.Name;
                        if (npcName.Equals("Robin"))
                        {
                            AddItemsToRobinsShop(menu);
                        }
                        else if (npcName.Equals("Clint"))
                        {
                            AddOreToClintsShop(menu);
                        }
                    }
                }
            }
        }

        public static void AddItemToShop(ShopMenu Menu,ISalable Item, int Price, int Stock)
        {
            Menu.forSale.Add(Item);
            Menu.itemPriceAndStock.Add(Item, new int[2] { Price, Stock });
        }

        private static void AddItemsToRobinsShop(ShopMenu Menu)
        {
            AddItemToShop(Menu,ModCore.ObjectManager.GetItem("Workbench", 1), 500, 1);
            AddItemToShop(Menu,ModCore.ObjectManager.resources.getResource("Sand", 1), 50, -1);
            AddItemToShop(Menu, new StardewValley.Object((int)Enums.SDVObject.Clay, 1), 50, -1);

        }
        /// <summary>
        /// Adds in ore to clint's shop.
        /// </summary>
        private static void AddOreToClintsShop(ShopMenu Menu)
        {
            AddItemToShop(Menu, ModCore.ObjectManager.resources.getOre("Tin", 1), ModCore.Configs.shops_blacksmithConfig.tinOreSellPrice, -1);
            AddItemToShop(Menu, ModCore.ObjectManager.resources.getOre("Bauxite", 1), ModCore.Configs.shops_blacksmithConfig.bauxiteOreSellPrice, -1);
            AddItemToShop(Menu, ModCore.ObjectManager.resources.getOre("Lead", 1), ModCore.Configs.shops_blacksmithConfig.leadOreSellPrice, -1);
            AddItemToShop(Menu, ModCore.ObjectManager.resources.getOre("Silver", 1), ModCore.Configs.shops_blacksmithConfig.silverOreSellPrice, -1);
            AddItemToShop(Menu, ModCore.ObjectManager.resources.getOre("Titanium", 1), ModCore.Configs.shops_blacksmithConfig.titaniumOreSellPrice, -1);
        }
    }
}
