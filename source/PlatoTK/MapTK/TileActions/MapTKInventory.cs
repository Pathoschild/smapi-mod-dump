/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace MapTK.TileActions
{
    internal class MapTKInventory
    {
        internal const string InventoryDataAsset = @"Data/MapTK/Inventories";


        public string[] Includes { get; set; } = new string[0];

        public List<ShopItem> Inventory { get; set; }

        
        internal static Dictionary<ISalable, int[]> GetInventory(IModHelper helper, string id, string shop)
        {
            return GetPriceAndStock(shop, id, helper);
        }

        private static Dictionary<ISalable, int[]> GetPriceAndStock(string shop, string id, IModHelper helper)
        {
            if (helper.Content.Load<Dictionary<string, MapTKInventory>>(InventoryDataAsset, ContentSource.GameContent).TryGetValue(id, out MapTKInventory inv))
            {
                Dictionary<ISalable, int[]> priceAndStock = new Dictionary<ISalable, int[]>();

                foreach (var item in inv.Inventory)
                    if (GetSalable(item, helper) is ISalable salable)
                        priceAndStock.Add(salable, new int[4] { item.Price == -1 ? salable.salePrice() : item.Price, item.Stock, item.ItemCurrency, item.ItemAmount});

                if (inv.Includes.Length > 0)
                    foreach (var i in inv.Includes)
                        foreach (var include in GetInventory(helper, i, shop))
                            priceAndStock.Add(include.Key, include.Value);

                return priceAndStock;
            }

            return new Dictionary<ISalable, int[]>();
        }
        
        private static ISalable GetSalable(ShopItem shopItem, IModHelper helper)
        {
            return helper.GetPlatoHelper().Utilities.GetSalable(shopItem.Type, shopItem.Index, shopItem.Name);
        }

    }
}
