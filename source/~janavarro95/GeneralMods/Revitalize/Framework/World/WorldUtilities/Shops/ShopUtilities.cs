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
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public class ShopUtilities
    {

        /// <summary>
        /// Delegate method to find if a given item being searched matches a given condiiton.
        /// </summary>
        /// <param name="ItemForSale"></param>
        /// <param name="ItemPrice"></param>
        /// <param name="AmountForSale"></param>
        /// <returns></returns>
        public delegate bool ItemFoundInShopInventory(ISalable ItemForSale, int ItemPrice, int AmountForSale);
        /// <summary>
        /// Used to update the shop's inventory. Currently sued only to add, but not modifiy a shop's contents.
        /// </summary>
        /// <param name="initialShopInventory"></param>
        /// <param name="currentItemForSale"></param>
        /// <param name="price"></param>
        /// <param name="amountForSale"></param>
        /// <returns></returns>
        public delegate ShopInventory UpdateShopInventory(ShopInventory initialShopInventory, ISalable currentItemForSale, int price, int amountForSale);


        public static void OnNewDay(object Sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {
            DwarfShopUtilities.OnNewDay(Sender, args);
            RobinsShopUtilities.OnNewDay(Sender, args);

        }

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
                            RobinsShopUtilities.AddItemsToRobinsShop(menu);
                        }
                        else if (npcName.Equals("Clint"))
                        {
                            ClintsShopUtilities.AddStockToClintsShop(menu);
                        }
                        else if (npcName.Equals("Dwarf"))
                        {
                            DwarfShopUtilities.AddGeodesToDwarfShop(menu);
                        }
                        else if (npcName.Equals("Marnie"))
                        {
                            RevitalizeModCore.log("Accessing marnies shop!");
                            MarniesShopUtilities.AddStockToMarniesShop(menu);
                        }
                    }
                }
            }
        }

        public static void AddItemToShop(ShopMenu Menu, ISalable Item, int Price, int Stock)
        {
            Menu.forSale.Add(Item);
            Menu.itemPriceAndStock.Add(Item, new int[2] { Price, Stock });
        }


        /// <summary>
        /// Updates a stock of a shop in a given order based on various conditions.
        /// </summary>
        /// <param name="Menu"></param>
        /// <param name="shopPopulationMethods"></param>
        /// <returns></returns>
        public static void UpdateShopStockAndPriceInSortedOrder(ShopMenu Menu, List<ShopInventoryProbe> shopPopulationMethods)
        {
            Dictionary<ISalable, int[]> sortedPriceAndStock = new Dictionary<ISalable, int[]>();
            List<ISalable> forSaleItems = new List<ISalable>();

            foreach (KeyValuePair<ISalable, int[]> itemPriceAndStock in Menu.itemPriceAndStock)
            {

                ISalable currentItemForSaleInList = itemPriceAndStock.Key;
                int price = itemPriceAndStock.Value[0];
                int amountForSale = itemPriceAndStock.Value[1];
                forSaleItems.Add(currentItemForSaleInList);
                sortedPriceAndStock.Add(itemPriceAndStock.Key, itemPriceAndStock.Value);

                foreach (var v in shopPopulationMethods)
                {
                    if (v.searchCondition.Invoke(currentItemForSaleInList, price, amountForSale))
                    {
                        ShopInventory shopInventory = new ShopInventory(sortedPriceAndStock, forSaleItems);
                        ShopInventory updatedShopInventory = v.onSearchConditionMetAddItems.Invoke(shopInventory, currentItemForSaleInList, price, amountForSale);
                        sortedPriceAndStock = updatedShopInventory.itemPriceAndStock;
                        forSaleItems = updatedShopInventory.itemsForSale;
                    }
                }

                Menu.forSale = forSaleItems;
                Menu.itemPriceAndStock = sortedPriceAndStock;
            }
        }
    }
}
