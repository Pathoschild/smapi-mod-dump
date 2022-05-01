/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using MarketDay.Shop;
using MarketDay.Utility;

namespace MarketDay.ItemPriceAndStock
{
    /// <summary>
    /// This class manages the total inventory for each shop
    /// </summary>
    public class ItemPriceAndStockManager
    {
        public Dictionary<ISalable, int[]> ItemPriceAndStock { get; set; }
        private readonly ItemStock[] _itemStocks;
        private readonly double _defaultSellPriceMultipler;
        private readonly Dictionary<double, string[]> _priceMultiplierWhen;
        private readonly int _maxNumItemsSoldInStore;
        private readonly string _shopName;
        private readonly int _shopPrice;

        /// <summary>
        /// Initializes the manager with the itemstocks, and how many items max this shop will contain
        /// </summary>
        /// <param name="data"></param>
        public ItemPriceAndStockManager(ItemShop data)
        {
            _defaultSellPriceMultipler = data.DefaultSellPriceMultiplier;
            _priceMultiplierWhen = data.PriceMultiplierWhen;
            _itemStocks = data.ItemStocks;
            _maxNumItemsSoldInStore = data.MaxNumItemsSoldInStore;
            _shopName = data.ShopName;
            _shopPrice = data.ShopPrice;
        }
        
        public void Initialize()
        {
            //initialize each stock
            foreach (ItemStock stock in _itemStocks)
            {
                stock.Initialize(_shopName, _shopPrice,_defaultSellPriceMultipler,_priceMultiplierWhen);
            }
        }

        /// <summary>
        /// Refreshes the stock of all items, doing condition checking and randomization
        /// </summary>
        public void Update()
        {
            ItemPriceAndStock = new Dictionary<ISalable, int[]>();
            MarketDay.Log($"Updating {_shopName}", LogLevel.Trace, true);

            foreach (ItemStock stock in _itemStocks)
            {
                var priceAndStock = stock.Update();
                //null is returned if conhditions aren't met, skip adding this stock
                if (priceAndStock == null) 
                    continue;

                Add(priceAndStock);
            }

            //randomly reduces the stock of the whole store down to maxNumItemsSoldInStore
            ItemsUtil.RandomizeStock(ItemPriceAndStock,_maxNumItemsSoldInStore);

        }

        /// <summary>
        /// Adds the stock from each ItemStock to the overall inventory
        /// </summary>
        /// <param name="dict"></param>
        private void Add(Dictionary<ISalable, int[]> dict)
        {
            foreach (var kvp in dict)
            {
                ItemPriceAndStock.Add(kvp.Key, kvp.Value);
            }
        }

    }
}
