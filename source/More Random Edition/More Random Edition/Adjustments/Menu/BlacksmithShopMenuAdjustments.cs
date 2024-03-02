/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    internal class BlacksmithShopMenuAdjustments : ShopMenuAdjustments
    { 
        /// <summary>
        /// The blacksmith context has two shops... unsure how else to detect which shop is
        /// being opened besides checking whether this one contians a certain item
        /// For now, we will check that it contains coal
        /// </summary>
        /// <param name="menu">The shop menu</param>
        /// <param name="wasOpened">Whether the shop was opened</param>
        public override void OnChange(ShopMenu menu, bool wasOpened)
        {
            if (menu.forSale.Any(item => 
                    item is SVObject objItem && 
                    objItem.ParentSheetIndex == (int)ObjectIndexes.Coal))
            {
                base.OnChange(menu, wasOpened);
            }
        }

        /// <summary>
        /// Adds a chance at a discount, or mining-related items to show up
        /// </summary>
        /// <param name="menu">The shop menu</param>
        protected override void Adjust(ShopMenu menu)
        {
            if (!ShouldChangeShop)
            {
                RestoreShopState(menu);
                return;
            }

            if (Globals.Config.Shops.RandomizeBlackSmithShop)
            {
                AdjustStock(menu);
            }
        }

        /// <summary>
        /// Adjusts the stock
        /// - 50% of discounting an item
        /// - 35% chance of adding some metal bars to the stock 
        ///   - 95% from that for 4 other bar types, means 8.3125% for any of those
        ///   - 5% from there for iridium  1.75% chance overall
        /// - 12% chance to add any random artifact
        /// - 3% chance to add iridium ore
        /// </summary>
        /// <param name="menu">The shop menu</param>
        private static void AdjustStock(ShopMenu menu)
        {
            Random shopRNG = Globals.GetDailyRNG(nameof(BlacksmithShopMenuAdjustments));

            int rolledValue = Range.GetRandomValue(0, 99, shopRNG);
            if (rolledValue < 50) // 50%
            {
                DiscountAnItem(menu, shopRNG);
            }
            else if (rolledValue < 85) // 35%
            {
                AddMetalBars(menu, shopRNG);
            }
            else if (rolledValue < 97) // 12%
            {
                AddAnArtifact(menu, shopRNG);
            }
            else // 3%
            {
                AddIridiumOre(menu, shopRNG);
            }
        }

        /// <summary>
        /// Discounts an existing item in the shop from between 10-25%
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void DiscountAnItem(ShopMenu menu, Random shopRNG)
        {
            var itemsAlreadyInStock = menu.itemPriceAndStock.ToList();
            var item = Globals.RNGGetRandomValueFromList(itemsAlreadyInStock, shopRNG);
            var priceMultiplier = 1 - (Range.GetRandomValue(10, 25, shopRNG) / 100f);

            item.Value[0] = (int)(item.Key.salePrice() * priceMultiplier);
        }

        /// <summary>
        /// Adds any random artifact with a stock of 1
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddAnArtifact(ShopMenu menu, Random shopRNG)
        {
            var artifact = Globals.RNGGetRandomValueFromList(ItemList.GetArtifacts(), shopRNG);
            var salePrice = GetAdjustedItemPrice(artifact, fallbackPrice: 50, multiplier: 3);
            AddStock(menu, artifact.GetSaliableObject(initialStack: 1), stock: 1, salePrice);
        }

        /// <summary>
        /// Adds 5-15 iridium ores
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddIridiumOre(ShopMenu menu, Random shopRNG)
        {
            var iridiumOre = ItemList.Items[ObjectIndexes.IridiumOre];
            var stock = Range.GetRandomValue(5, 15, shopRNG);
            var salePrice = GetAdjustedItemPrice(iridiumOre, fallbackPrice: 50, multiplier: 5);
            AddStock(menu, iridiumOre.GetSaliableObject(stock), stock, salePrice);
        }

        /// <summary>
        /// Adds a random bar to the shop
        /// - 5% chance of 2-4 iridium
        /// - 3-8 of lesser bars
        /// Maple bars are here on purpose because lol
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddMetalBars(ShopMenu menu, Random shopRNG)
        {
            var getIridiumBar = Globals.RNGGetNextBoolean(5, shopRNG);
            if (getIridiumBar)
            {
                var iridiumBar = ItemList.Items[ObjectIndexes.IridiumBar];
                var stock = Range.GetRandomValue(2, 4, shopRNG);
                var salePrice = GetAdjustedItemPrice(iridiumBar, fallbackPrice: 50, multiplier: 3);
                AddStock(menu, iridiumBar.GetSaliableObject(stock), stock, salePrice);
            }

            else
            {
                var commonMetalBars = new List<Item>()
                {
                    ItemList.Items[ObjectIndexes.CopperBar],
                    ItemList.Items[ObjectIndexes.IronBar],
                    ItemList.Items[ObjectIndexes.GoldBar],
                    ItemList.Items[ObjectIndexes.MapleBar]
                };

                var stock = Range.GetRandomValue(3, 8, shopRNG);
                var bar = Globals.RNGGetRandomValueFromList(commonMetalBars, shopRNG);
                var salePrice = bar.Id == (int)ObjectIndexes.MapleBar
                    ? GetAdjustedItemPrice(bar, fallbackPrice: 50, multiplier: 1)
                    : GetAdjustedItemPrice(bar, fallbackPrice: 50, multiplier: 3);
                AddStock(menu, bar.GetSaliableObject(stock), stock, salePrice);
            }
        }
    }
}
