/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using ShopTileFramework.Data;
using ShopTileFramework.ItemPriceAndStock;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace ShopTileFramework.Shop
{
    class VanillaShop : VanillaShopModel
    {
        public List<ItemPriceAndStockManager> StockManagers { get; set; }
        public Dictionary<ISalable, int[]> ItemPriceAndStock { get; set; }
        public IContentPack ContentPack { set; get; }
        public void Initialize()
        {
            StockManagers = new List<ItemPriceAndStockManager>();
        }

        public void UpdateItemPriceAndStock()
        {
            ItemPriceAndStock = new Dictionary<ISalable, int[]>();
            ModEntry.monitor.Log($"Generating stock for {ShopName}", LogLevel.Debug);
            foreach(ItemPriceAndStockManager manager in StockManagers)
            {
                manager.Update();
                ItemPriceAndStock = ItemPriceAndStock.Concat(manager.ItemPriceAndStock).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
