/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using MarketDay.ItemPriceAndStock;
using Microsoft.Xna.Framework;

namespace MarketDay.Data
{
    public abstract class ItemShopModel
    {
        public string ShopName { get; set; }
        public string StoreCurrency { get; set; } = "Money";
        public List<int> CategoriesToSellHere { get; set; } = null;
        public string PortraitPath { get; set; } = null;
        public string Quote { get; set; } = null;
        public int ShopPrice { get; set; } = -1;
        public int MaxNumItemsSoldInStore { get; set; } = int.MaxValue;
        public double DefaultSellPriceMultipler { set => DefaultSellPriceMultiplier = value; }
        public double DefaultSellPriceMultiplier { get; set; } = 1;
        public Dictionary<double, string[]> PriceMultiplierWhen { get; set; } = null;
        public ItemStock[] ItemStocks { get; set; } = Array.Empty<ItemStock>();
        public string[] When { get; set; } = null;
        public string ClosedMessage { get; set; } = null;
        public Dictionary<string, string> LocalizedQuote { get; set; }
        public Dictionary<string, string> LocalizedClosedMessage { get; set; }
        public int SignObjectIndex { get; set; }
        public Color ShopColor { get; set; }
        
        public string NpcName { get; set; }

        public string OpenSignPath { get; set; }

        public string ClosedSignPath { get; set; }
    }
}
