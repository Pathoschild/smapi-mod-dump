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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Shops.RevitalizeShops;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public class ShopUtilities
    {

        public static string MovieTheaterTicketWindowShopContext = "";

        /// <summary>
        /// Delegate method to find if a given item being searched matches a given condiiton. Also can include additional paramaters to determine if the item should be added or not.
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
                            BlacksmithShopUtilities.AddStockToClintsShop(menu);
                        }
                        else if (npcName.Equals("Dwarf"))
                        {
                            DwarfShopUtilities.AddStockToDwarfShop(menu);
                        }
                        else if (npcName.Equals("Marnie"))
                        {
                            MarniesShopUtilities.AddStockToMarniesShop(menu);
                        }
                        else if (npcName.Equals("Marlon"))
                        {
                            AdventureGuildShopUtilities.AddStockToAdventureGuildShop(menu);
                        }
                    }
                    else if(menu.currency==ShopMenu.currency_qiGems && menu.storeContext.Equals("QiGemShop"))
                    {
                        WalnutRoomShopUtilities.AddItemsToShop(menu);
                    }

                    //Only way to validate the movie theater currently.
                    else if (menu.potraitPersonDialogue.Equals(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:MovieTheaterBoxOffice"), Game1.dialogueFont, 304)))
                    {
                        MovieTheaterShopUtilities.AddStockToShop(menu);
                    }

                    else if (menu.storeContext.Equals(HayMakerShopUtilities.StoreContext))
                    {
                        HayMakerShopUtilities.AddItemsToShop(menu);
                    }
                    else if (menu.storeContext.Equals("IslandNorth"))
                    {
                        IslandTraderShopUtilities.AddStockToShop(menu);
                    }
                    else if (menu.storeContext.Equals("Desert"))
                    {
                        DesertTraderShopUtilities.AddStockToShop(menu);
                    }
                    else if (menu.storeContext.Equals("VolcanoShop"))
                    {
                        //More late game shopping, but not as hard as walnut room.
                        VolcanoShopUtilities.AddStockToShop(menu);
                    }
                    //Add support for Casino, Oasis, and Krobus for selling things.
                    //Krobus is mid early game, Oasis is mid-early game, and Casino can have more fun things there.

                    else
                    {
                        Game1.showRedMessage("ShopContext: " + menu.storeContext);
                    }
                }
            }
        }

        public static void AddToShopIfCraftingRecipeNotKnown(ShopMenu Menu, string CraftingBookName, string CraftingRecipe, ISalable Item, int Price, int Stock = 1, bool AdditionalConditions=true)
        {
            if (!PlayerUtilities.KnowsCraftingRecipe(CraftingBookName, CraftingRecipe) && AdditionalConditions)
            {
                ShopUtilities.AddItemToShop(Menu, Item, Price, Stock);
            }
        }

        /// <summary>
        /// Adds an item to the walnut room shop stock if a given crafting recipe for it is not known.
        /// </summary>
        /// <param name="Menu"></param>
        /// <param name="CraftingBookName"></param>
        /// <param name="CraftingRecipe"></param>
        /// <param name="Item"></param>
        /// <param name="Price"></param>
        /// <param name="Stock"></param>
        /// <param name="AdditionalConditions"></param>
        public static void AddToWalnutShopIfCraftingRecipeNotKnown(ShopMenu Menu, string CraftingBookName, string CraftingRecipe, ISalable Item, int Price, int Stock = 1, bool AdditionalConditions = true)
        {
            if (!PlayerUtilities.KnowsCraftingRecipe(CraftingBookName, CraftingRecipe) && AdditionalConditions)
            {
                ShopUtilities.AddItemToWalnutRoomShop(Menu, Item, Price, Stock);
            }
        }

        /// <summary>
        /// Utility method to add items to a normal shop that uses gold.
        /// </summary>
        /// <param name="Menu"></param>
        /// <param name="Item"></param>
        /// <param name="Price"></param>
        /// <param name="Stock"></param>
        public static void AddItemToShop(ShopMenu Menu, ISalable Item, int Price, int Stock)
        {
            Menu.forSale.Add(Item);
            Menu.itemPriceAndStock.Add(Item, new int[2] { Price, Stock });
        }

        /// <summary>
        /// Adds an item to a shop's stock, but requires another item as the currency to buy that item.
        /// </summary>
        /// <param name="Menu"></param>
        /// <param name="Item"></param>
        /// <param name="Price"></param>
        /// <param name="ObjectRequiredAsCurrency">The item required to make the trade.</param>
        /// <param name="Stock"></param>
        public static void AddItemToShop(ShopMenu Menu, ISalable Item, int Price, Enums.SDVObject ObjectRequiredAsCurrency, int Stock= int.MaxValue)
        {
            Menu.forSale.Add(Item);
            Menu.itemPriceAndStock.Add(Item, new int[4] { 0, Stock, (int)ObjectRequiredAsCurrency, Price });
        }
        /// <summary>
        /// Adds an item to a shop's stock, but requires another item as the currency to buy that item only if the crafting recipe isn't known.
        /// </summary>
        /// <param name="Menu"></param>
        /// <param name="CraftingBookName"></param>
        /// <param name="CraftingRecipe"></param>
        /// <param name="Item"></param>
        /// <param name="Price"></param>
        /// <param name="ObjectRequiredAsCurrency"></param>
        /// <param name="Stock"></param>
        /// <param name="AdditionalConditions"></param>
        public static void AddItemToShopIfCraftingRecipeNotKnown(ShopMenu Menu, string CraftingBookName, string CraftingRecipe, ISalable Item, int Price, Enums.SDVObject ObjectRequiredAsCurrency, int Stock = int.MaxValue, bool AdditionalConditions=true)
        {
            if (!PlayerUtilities.KnowsCraftingRecipe(CraftingBookName, CraftingRecipe) && AdditionalConditions)
            {
                ShopUtilities.AddItemToShop(Menu, Item, Price,ObjectRequiredAsCurrency,Stock);
            }
        }

        /// <summary>
        /// Utility method to add items to the shop in the walnut room..
        /// </summary>
        /// <param name="Menu"></param>
        /// <param name="Item"></param>
        /// <param name="Price"></param>
        /// <param name="Stock"></param>
        public static void AddItemToWalnutRoomShop(ShopMenu Menu, ISalable Item, int Price, int Stock= int.MaxValue)
        {
            AddItemToShop(Menu, Item, Price, Enums.SDVObject.QiGem, Stock);
        }

        /// <summary>
        /// Helper method for creating <see cref="ShopInventoryProbe"/>s which allow for adding items to a shop in a specific order based on given conditions. If true, then the new item is found after the one matching this param.
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToAdd"></param>
        /// <returns></returns>
        public static ShopInventoryProbe CreateInventoryShopProbe(ItemFoundInShopInventory ItemFoundInShopConditional, Enums.SDVObject itemToAdd, int SellingPrice, int AmountToAdd=-1)
        {
            return CreateInventoryShopProbe(ItemFoundInShopConditional ,new ItemReference(itemToAdd), SellingPrice, AmountToAdd);
        }

        /// <summary>
        /// Helper method for creating <see cref="ShopInventoryProbe"/>s which allow for adding items to a shop in a specific order based on given conditions. If true, then the new item is found after the one matching this param.
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToAdd"></param>
        /// <returns></returns>
        public static ShopInventoryProbe CreateInventoryShopProbe(ItemFoundInShopInventory ItemFoundInShopConditional , Enums.SDVBigCraftable itemToAdd, int SellingPrice, int AmountToAdd=-1)
        {
            return CreateInventoryShopProbe(ItemFoundInShopConditional, new ItemReference(itemToAdd), SellingPrice, AmountToAdd);
        }

        /// <summary>
        /// Helper method for creating <see cref="ShopInventoryProbe"/>s which allow for adding items to a shop in a specific order based on given conditions. If true, then the new item is found after the one matching this param.
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToAdd"></param>
        /// <returns></returns>
        public static ShopInventoryProbe CreateInventoryShopProbe(ItemFoundInShopInventory ItemFoundInShopConditional , string itemToAdd, int SellingPrice, int AmountToAdd=-1)
        {
            return CreateInventoryShopProbe(ItemFoundInShopConditional, new ItemReference(itemToAdd), SellingPrice, AmountToAdd);
        }

        /// <summary>
        /// Helper method for creating <see cref="ShopInventoryProbe"/>s which allow for adding items to a shop in a specific order based on given conditions. If true, then the new item is found after the one matching this param.
        /// </summary>
        /// <param name="ItemFoundInShopConditional"></param>
        /// <param name="itemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToAdd"></param>
        /// <returns></returns>
        public static ShopInventoryProbe CreateInventoryShopProbe(ItemFoundInShopInventory ItemFoundInShopConditional ,ItemReference itemToAdd, int SellingPrice, int AmountToAdd=-1)
        {
            return new ShopInventoryProbe(
                ItemFoundInShopConditional,
                new UpdateShopInventory((ShopInventory, ItemForSale, Price, Stock) =>
                {
                    ShopInventory.addItemForSale(itemToAdd.getItem(), SellingPrice, AmountToAdd);
                    return ShopInventory;
                }
            ));
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
