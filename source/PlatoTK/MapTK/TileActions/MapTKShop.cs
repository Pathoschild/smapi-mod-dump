/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace MapTK.TileActions
{

    internal class MapTKShop
    {
        internal const string ShopDataAsset = @"Data/MapTK/Shops";
        internal static readonly Dictionary<string, List<BoughtItem>> Bought = new Dictionary<string, List<BoughtItem>>();

        public string ShopKeeper { get; set; }

        public string Message { get; set; } = "";

        public string Inventory { get; set; } = "";

        public string Restock { get; set; } = "Never";

        public int Currency { get; set; } = 0;

        internal static Dictionary<ISalable, int[]> GetInventory(string shopid, IModHelper helper)
        {
            var priceAndStock = new Dictionary<ISalable, int[]>();

            if (GetShop(helper, shopid) is MapTKShop shop)
            {
                priceAndStock = MapTKInventory.GetInventory(helper, shop.Inventory, shopid);

                if (shop.Restock != "Always")
                {
                    if (!Bought.ContainsKey(shopid))
                        Bought.Add(shopid, new List<BoughtItem>());
                    else
                    priceAndStock.Keys.Where(k => Bought[shopid].Any(s => s.ItemName == k.Name)).ToList().ForEach((s) =>
                    {
                        priceAndStock[s][1] -= Bought[shopid].FirstOrDefault(b => b.ItemName == s.Name)?.Stock ?? 0;
                        if (priceAndStock[s][1] <= 0)
                            priceAndStock.Remove(s);
                    });
                }
            }

            return  priceAndStock;
        }

        internal static MapTKShop GetShop(IModHelper helper, string id)
        {
            var dict = helper.GameContent.Load<Dictionary<string, MapTKShop>>(ShopDataAsset);

            if(dict.TryGetValue(id, out MapTKShop shop))
                return shop;

            return null;
        }
    }
}
