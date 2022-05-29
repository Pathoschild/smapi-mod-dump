/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using MarketDay.Utility;

namespace MarketDay.ItemPriceAndStock
{
    /// <summary>
    /// This class stores the global data for each itemstock, in order to generate and add items by ID or name
    /// to the stock
    /// </summary>
    class ItemBuilder
    {
        private Dictionary<ISalable, int[]> _itemPriceAndStock;
        private readonly ItemStock _itemStock;
        private const string CategorySearchPrefix = "%Category:";
        private const string NameSearchPrefix = "%Match:";

        public ItemBuilder(ItemStock itemStock)
        {
            _itemStock = itemStock;
        }

        /// <param name="itemPriceAndStock">the ItemPriceAndStock this builder will add items to</param>
        public void SetItemPriceAndStock(Dictionary<ISalable, int[]> itemPriceAndStock)
        {
            _itemPriceAndStock = itemPriceAndStock;
        }

        /// <summary>
        /// Takes an item name, and adds that item to the stock
        /// </summary>
        /// <param name="itemName">name of the item</param>
        /// <param name="priceMultiplier"></param>
        /// <returns></returns>
        public bool AddItemToStock(string itemName, double priceMultiplier = 1)
        {
            int id;
            if (itemName.StartsWith(CategorySearchPrefix))
            {
                var offset = CategorySearchPrefix.Length + 1;
                id = ItemsUtil.GetIndexByCategory(itemName[offset..], _itemStock.ItemType);
            }
            else if (itemName.StartsWith(NameSearchPrefix))
            {
                var offset = NameSearchPrefix.Length + 1;
                id = ItemsUtil.GetIndexByMatch(itemName[offset..], _itemStock.ItemType);
            } else {
                id = ItemsUtil.GetIndexByName(itemName, _itemStock.ItemType);
            }
            
            if (id >= 0) return AddItemToStock(id, priceMultiplier);
            var item = ItemsUtil.GetDGAObjectByName(itemName, _itemStock.ItemType);
            if (item is not null) return AddItemToStock(item, priceMultiplier);
            MarketDay.Log($"{_itemStock.ItemType} named \"{itemName}\" could not be added to the Shop {_itemStock.ShopName}", LogLevel.Trace);
            return false;
        }

        /// <summary>
        /// Takes an item id, and adds that item to the stock
        /// </summary>
        /// <param name="itemId">the id of the item</param>
        /// <param name="priceMultiplier"></param>
        /// <returns></returns>
        public bool AddItemToStock(int itemId, double priceMultiplier = 1)
        {

            MarketDay.Log($"Adding item ID {itemId} to {_itemStock.ShopName}", LogLevel.Debug, true);

            if (itemId < 0)
            {
                MarketDay.Log($"{_itemStock.ItemType} of ID {itemId} could not be added to the Shop {_itemStock.ShopName}", LogLevel.Trace);
                return false;
            }

            if (_itemStock.ItemType == "Seed" && _itemStock.FilterSeedsBySeason)
            {
                if (!ItemsUtil.IsInSeasonCrop(itemId)) return false;
            }

            var item = CreateItem(itemId);
            return item != null && AddItemToStock(item, priceMultiplier);
        }

        private bool AddItemToStock(ISalable item, double priceMultiplier)
        {
            if (item is null)
            {
                MarketDay.Log($"Null {_itemStock.ItemType} could not be added to the Shop {_itemStock.ShopName}", LogLevel.Trace);
                return false;
            }
            
            if (_itemStock.IsRecipe)
            {
                if (!ItemsUtil.RecipesList.Contains(item.Name))
                {
                    MarketDay.Log($"{item.Name} is not a valid recipe and won't be added.", LogLevel.Trace);
                    return false;
                }
            }

            var priceStockCurrency = GetPriceStockAndCurrency(item, priceMultiplier);
            _itemPriceAndStock.Add(item, priceStockCurrency);

            return true;
        }

        /// <summary>
        /// Given an itemID, return an instance of that item with the parameters saved in this builder
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private ISalable CreateItem(int itemId)
        {
            switch (_itemStock.ItemType)
            {
                case "Object":
                case "Seed":
                    return new Object(itemId, _itemStock.Stock, _itemStock.IsRecipe, quality: _itemStock.Quality);
                case "BigCraftable":
                    return new Object(Vector2.Zero, itemId) { Stack = _itemStock.Stock, IsRecipe = _itemStock.IsRecipe };
                case "Clothing":
                    return new Clothing(itemId);
                case "Ring":
                    return new Ring(itemId);
                case "Hat":
                    return new Hat(itemId);
                case "Boot":
                    return new Boots(itemId);
                case "Furniture":
                    return new Furniture(itemId, Vector2.Zero);
                case "Weapon":
                    return new MeleeWeapon(itemId);
                default: return null;
            }
        }

        /// <summary>
        /// Creates the second parameter in ItemStockAndPrice, an array that holds info on the price, stock,
        /// and if it exists, the item currency it takes
        /// </summary>
        /// <param name="item">An instance of the item</param>
        /// <param name="priceMultiplier"></param>
        /// <returns>The array that's the second parameter in ItemPriceAndStock</returns>
        private int[] GetPriceStockAndCurrency(ISalable item, double priceMultiplier)
        {
            int[] priceStockCurrency;
            //if no price is provided, use the item's sale price multiplied by defaultSellPriceMultiplier
            var price = (_itemStock.StockPrice == -1) ? (int)(item.salePrice()* _itemStock.DefaultSellPriceMultiplier) : _itemStock.StockPrice;
            price = (int)(price*priceMultiplier);

            if (_itemStock.CurrencyObjectId == -1) // no currency item
            {
                priceStockCurrency = new[] { price, _itemStock.Stock };
            }
            else if (_itemStock.StockCurrencyStack == -1) //no stack provided for currency item so defaults to 1
            {
                priceStockCurrency = new[] { price, _itemStock.Stock, _itemStock.CurrencyObjectId };
            }
            else //both currency item and stack provided
            {
                priceStockCurrency = new[] { price, _itemStock.Stock, _itemStock.CurrencyObjectId, _itemStock.StockCurrencyStack };
            }

            return priceStockCurrency;
        }
    }
}
