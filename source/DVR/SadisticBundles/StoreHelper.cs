/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;

namespace SadisticBundles
{
    public class StoreHelper : IDisposable
    {
        public IDictionary<Item, int[]> stock;
        public IList<Item> forsale;
        private ShopMenu menu;

        static FieldInfo invInfo = typeof(ShopMenu).GetField("itemPriceAndStock", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        static FieldInfo forSaleInfo = typeof(ShopMenu).GetField("forSale", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public static StoreHelper Read(ShopMenu m)
        {
            return new StoreHelper
            {
                menu = m,
                stock = (Dictionary<Item, int[]>)invInfo.GetValue(m),
                forsale = (List<Item>)forSaleInfo.GetValue(m),
            };
        }

        public void Write(ShopMenu m)
        {
            invInfo.SetValue(m, stock);
            forSaleInfo.SetValue(m, forsale);
        }

        public void AddItem(int id, int price, int qty = 1, int quality = 0)
        {
            var objectToAdd = new StardewValley.Object(id, 1, quality: quality);
            stock.Add(objectToAdd, new int[2] { price, qty });
            forsale.Add(objectToAdd);
        }

        public void Dispose()
        {
            Write(menu);
        }
    }
}
