/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace DynamicGameAssets
{
    internal class ShopEntry
    {
        public ISalable Item;
        public int Quantity;
        public int Price;
        public int? CurrencyId;

        public void AddToShop(ShopMenu shop)
        {
            int qty = this.Quantity;
            if (this.Item is StardewValley.Object { IsRecipe: true })
                qty = 1;

            this.Item.Stack = qty;
            shop.forSale.Add(this.Item);
            if (this.CurrencyId == null)
            {
                shop.itemPriceAndStock.Add(this.Item, new[]
                {
                    this.CurrencyId == null ? this.Price : 0,
                    qty
                });
            }
            else
            {
                shop.itemPriceAndStock.Add(this.Item, new[]
                {
                    0,
                    qty,
                    this.CurrencyId.Value, // Black magic
                    this.Price,
                });
            }
        }

        public void AddToShopStock(Dictionary<ISalable, int[]> stock)
        {
            int qty = this.Quantity;
            if (this.Item is StardewValley.Object { IsRecipe: true })
                qty = 1;

            this.Item.Stack = qty;
            if (this.CurrencyId == null)
            {
                stock.Add(this.Item, new[]
                {
                    this.CurrencyId == null ? this.Price : 0,
                    qty
                });
            }
            else
            {
                stock.Add(this.Item, new[]
                {
                    0,
                    qty,
                    this.CurrencyId.Value, // Black magic
                    this.Price,
                });
            }
        }
    }
}
