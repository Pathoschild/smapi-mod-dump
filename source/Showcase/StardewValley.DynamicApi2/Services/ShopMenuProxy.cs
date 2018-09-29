using System;
using System.Collections.Generic;
using System.Reflection;
using Igorious.StardewValley.DynamicApi2.Extensions;
using StardewValley;
using StardewValley.Menus;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class ShopMenuProxy
    {
        private static readonly Lazy<FieldInfo> ForSaleField = typeof(ShopMenu).GetLazyInstanceField("forSale");
        private static readonly Lazy<FieldInfo> ItemPriceAndStockField = typeof(ShopMenu).GetLazyInstanceField("itemPriceAndStock");
        private static readonly Lazy<FieldInfo> HeldItemField = typeof(ShopMenu).GetLazyInstanceField("heldItem");

        private readonly ShopMenu _menu;

        public ShopMenuProxy(ShopMenu menu)
        {
            _menu = menu;
            ItemsForSale = menu.GetFieldValue<List<Item>>(ForSaleField);
            ItemsPriceAndStock = menu.GetFieldValue<Dictionary<Item, int[]>>(ItemPriceAndStockField);
        }

        public Dictionary<Item, int[]> ItemsPriceAndStock { get; }
        public List<Item> ItemsForSale { get; }

        public Item HeldItem
        {
            get { return _menu.GetFieldValue<Item>(HeldItemField); }
            set { _menu.SetFieldValue(HeldItemField, value); }
        }

        public void AddItem(Item item, int stack = int.MaxValue, int? price = null)
        {
            ItemsForSale.Add(item);
            ItemsPriceAndStock.Add(item, new[] { price ?? item.salePrice(), stack });
        }
    }
}