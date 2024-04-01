/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using ShopTileFramework.Framework.ItemPriceAndStock;

namespace ShopTileFramework.Framework.Data
{
    abstract class VanillaShopModel
    {
        public string ShopName { get; set; }
        public bool ReplaceInsteadOfAdd { get; set; } = false;
        public bool AddStockAboveVanilla { get; set; } = false;
        public int ShopPrice { get; set; } = -1;
        public int MaxNumItemsSoldInStore { get; set; } = int.MaxValue;
        public double DefaultSellPriceMultipler { set => this.DefaultSellPriceMultiplier = value; }
        public double DefaultSellPriceMultiplier { get; set; } = 1;
        public Dictionary<double, string[]> PriceMultiplierWhen { get; set; } = null;
        public ItemStock[] ItemStocks { get; set; }
    }
}
