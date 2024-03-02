/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    public abstract class ShopMenuAdjustments
    {
        internal static readonly int _maxValue = int.MaxValue;

        /// <summary>
        /// These two values track the state of the shop
        /// </summary>
        protected Dictionary<ISalable, int[]> currentItemPriceAndStock;
        protected List<ISalable> currentForSale;

        /// <summary>
        /// Certain shops don't need to be saved
        /// </summary>
        protected bool SkipShopSave;

        /// <summary>
        /// Whether we should change the shop
        /// That is - it's the first time we've opened the menu today
        /// </summary>
        public virtual bool ShouldChangeShop 
        { 
            get { return currentItemPriceAndStock == null; } 
        }

        /// <summary>
        /// Called when the shop menu was opened or closed
        /// On opened, adjusts the menu to display the changes
        /// On closed, saves the state so it can be reloaded next time it's opened
        /// </summary>
        /// <param name="menu">The shop menu</param>
        /// <param name="wasOpened">Whether the menu was opened or closed</param>
        public virtual void OnChange(ShopMenu menu, bool wasOpened)
        {
            if (wasOpened)
            {
                Adjust(menu);
                RebuildItemPriceAndStockDictionary(menu);
            }

            else
            {
                SaveShopState(menu);
                RebuildItemPriceAndStockDictionary(menu);
            }
        }

        /// <summary>
        /// The InsertAt functions will result in the order of the dictionary being different from the list order
        /// Of course - using a Dictionary to order something is unreliable, but that's how the game works!
        /// This will rebuild the dictionary to be in the same order as the list
        /// This is necessary to fix items jumping to the bottom when something is sold to the shop
        /// 
        /// Finally - we need to skip over buyback items because they are never reloaded after the shop is closed
        /// </summary>
        /// <param name="menu"></param>
        private static void RebuildItemPriceAndStockDictionary(ShopMenu menu)
        {
            var buyBackItemsCount = menu.buyBackItems.Count;
            menu.forSale.RemoveRange(menu.forSale.Count - buyBackItemsCount, buyBackItemsCount);

            Dictionary<ISalable, int[]> fixedDictionary = new();
            menu.forSale.ForEach(item => fixedDictionary[item] = menu.itemPriceAndStock[item]);
            menu.itemPriceAndStock = fixedDictionary;
        }

        /// <summary>
        /// All shops will use this as the entry point for their adjustments
        /// </summary>
        /// <param name="menu">The shop menu</param>
        protected abstract void Adjust(ShopMenu menu);

        /// <summary>
        /// Saves the state of the shop
        /// </summary>
        /// <param name="menu">The shop menu</param>
        public virtual void SaveShopState(ShopMenu menu)
        {
            if (!SkipShopSave)
            {
                currentItemPriceAndStock = menu.itemPriceAndStock;
                currentForSale = menu.forSale;
            }
        }

        /// <summary>
        /// Restores the state of the shop - this will keep track of limited stock
        /// </summary>
        /// <param name="menu">The shop menu</param>
        public virtual void RestoreShopState(ShopMenu menu)
        {
            menu.itemPriceAndStock = currentItemPriceAndStock;
            menu.forSale = currentForSale;
        }

        /// <summary>
        /// Clear out the values - should be called on day end so reset for the next day
        /// </summary>
        public virtual void ResetShopState()
        {
            currentItemPriceAndStock = null;
            currentForSale = null;
        }

        /// <summary>
        /// Adds the given item to the given shop menu
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        internal static void AddStock(
            ShopMenu menu,
            Item item,
            int stock = int.MaxValue,
            int? salePrice = null)
        {
            AddStock(menu, item.GetSaliableObject(), stock, salePrice);
        }

        /// <summary>
        /// Adds the given items to the given shop menu
        /// The stock and sale priced passed in will apply to every item in the list
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        internal static void AddStock(
            ShopMenu menu,
            List<Item> items,
            int stock = int.MaxValue,
            int? salePrice = null)
        {
            items.ForEach(item => AddStock(menu, item, stock, salePrice));
        }

        /// <summary>
        /// Adds the given item to the given shop menu
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        internal static void AddStock(
            ShopMenu menu,
            ISalable item, 
            int stock = int.MaxValue,
            int? salePrice = null)
        {
            AddToItemPriceAndStock(menu, item, stock, salePrice);
            menu.forSale.Add(item);
        }

        /// <summary>
        /// Adds the given items to the given shop menu
        /// The stock and sale priced passed in will apply to every item in the list
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        internal static void AddStock(
            ShopMenu menu,
            List<ISalable> items,
            int stock = int.MaxValue,
            int? salePrice = null)
        {
            items.ForEach(item => AddStock(menu, item, stock, salePrice));
        }

        /// <summary>
        /// Inserts the given item at the given position - defaults to the first index (0)
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        /// <param name="index">The index to insert the item to (where it will show up in the shop menu)</param>
        internal static void InsertStockAt(
            ShopMenu menu,
            Item item,
            int stock = int.MaxValue,
            int? salePrice = null,
            int index = 0)
        {
            InsertStockAt(menu, item.GetSaliableObject(), stock, salePrice, index);
        }

        /// <summary>
        /// Inserts the given item at the given position - defaults to the first index (0)
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        /// <param name="index">The index to insert the item to (where it will show up in the shop menu)</param>
        internal static void InsertStockAt(
            ShopMenu menu,
            ISalable item, 
            int stock = int.MaxValue,
            int? salePrice = null,
            int index = 0)
        {
            AddToItemPriceAndStock(menu, item, stock, salePrice);
            menu.forSale.Insert(index, item);
        }

        /// <summary>
        /// Adds the item to the menu's itemPriceAndStock dictionary
        /// </summary>
        /// <param name="menu">The shop menu<param>
        /// <param name="item">The item to add</param>
        /// <param name="stock">The amount to sell - defaults to max</param>
        /// <param name="salePrice">The amount to sell at - defaults to the item's sale price</param>
        internal static void AddToItemPriceAndStock(
            ShopMenu menu, 
            ISalable item, 
            int stock = int.MaxValue, 
            int? salePrice = null)
        {
            var price = salePrice ?? item.salePrice();
            if (price <= 0)
            {
                price = 25; // Put a default on this just in case
            }
            menu.itemPriceAndStock.Add(item, new[] { price, stock });
        }

        /// <summary>
        /// Empty a given shop's stock
        /// </summary>
        /// <param name="menu">The menu of the shop</param>
        internal static void EmptyStock(ShopMenu menu)
        {
            while (menu.itemPriceAndStock.Any())
            {
                ISalable obj = menu.itemPriceAndStock.Keys.ElementAt(0);
                menu.itemPriceAndStock.Remove(obj);
                menu.forSale.Remove(obj);
            }
        }

        /// <summary>
        /// Removes an item fom the shop stock
        /// </summary>
        /// <param name="menu">The menu of the shop</param>
        /// <param name="itemId">The item to remove</param>
        /// <returns>Whether an item was removed</returns>
        internal static bool RemoveFromStock(ShopMenu menu, int itemId)
        {
            var itemToRemove = menu.itemPriceAndStock.Keys
                .Where(item => item is SVObject && (item as SVObject).ParentSheetIndex == itemId)
                .FirstOrDefault();

            if (itemToRemove != null)
            {
                menu.itemPriceAndStock.Remove(itemToRemove);
                menu.forSale.Remove(itemToRemove);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a new price for the item
        /// Takes the greater of the item's price and the fallback price and multiplies them by
        /// the multiplier and the difficulty level
        /// </summary>
        /// <param name="item">The item to get the price for</param>
        /// <param name="fallbackPrice">The price to use if the item costs too little</param>
        /// <param name="multiplier">The multiplier</param>
        /// <returns>The new item price</returns>
        internal static int GetAdjustedItemPrice(Item item, int fallbackPrice, int multiplier)
        {
            return GetAdjustedItemPrice(item.GetSaliableObject(), fallbackPrice, multiplier);
        }

        /// <summary>
        /// Gets a new price for the item
        /// Takes the greater of the item's price and the fallback price and multiplies them by
        /// the multiplier and the difficulty level
        /// </summary>
        /// <param name="item">The item to get the price for</param>
        /// <param name="fallbackPrice">The price to use if the item costs too little</param>
        /// <param name="multiplier">The multiplier</param>
        /// <returns>The new item price</returns>
        internal static int GetAdjustedItemPrice(ISalable item, int fallbackPrice, int multiplier)
        {
            return Math.Max(fallbackPrice, (int)(item.salePrice() * Game1.MasterPlayer.difficultyModifier)) * multiplier;
        }
    }
}
