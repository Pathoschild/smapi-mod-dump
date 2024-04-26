/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley.GameData.Shops;
using System.Collections.Generic;

namespace Randomizer
{
    public class RandomizedBlacksmithShop : RandomizedShop
    {
        private const string UniqueItemId = "RandomizedBlacksmithItem";
        
        public RandomizedBlacksmithShop() : base("Blacksmith") { }

        public override bool ShouldModifyShop()
            => Globals.Config.Shops.RandomizeBlacksmithShop;

		/// <summary>
		/// Modifies the shop stock - see AdjustStock for details
		/// </summary>
		/// <returns>The modified shop data</returns>
		public override ShopData ModifyShop()
        {
            AdjustStock();

            return CurrentShopData;
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
        private void AdjustStock()
        {
            RNG shopRNG = RNG.GetDailyRNG(nameof(RandomizedBlacksmithShop));

            int rolledValue = shopRNG.NextIntWithinRange(0, 99);
            if (rolledValue < 50) // 50%
            {
                DiscountAnItem(shopRNG);
            }
            else if (rolledValue < 85) // 35%
            {
                AddMetalBars(shopRNG);
            }
            else if (rolledValue < 97) // 12%
            {
                AddAnArtifact(shopRNG);
            }
            else // 3%
            {
                AddIridiumOre(shopRNG);
            }
        }

        /// <summary>
        /// Discounts an existing item in the shop from between 10-25%
        /// </summary>
        /// <param name="shopRNG"></param>
        private void DiscountAnItem(RNG shopRNG)
        {
            // Choose an ore and discount multiplier
            string itemToDiscount = 
                shopRNG.GetRandomValueFromList(CurrentShopData.Items).ItemId;
            var priceMultiplier = 1 - (shopRNG.NextIntWithinRange(10, 25) / 100f);

            // This shop has two sets of prices based on the year; we will discount both
            GetShopItemsByItemIds(new List<string> { itemToDiscount })
                .ForEach(itemToDiscount => {
                    itemToDiscount.Price = (int)(itemToDiscount.Price * priceMultiplier);
                });
        }

        /// <summary>
        /// Adds any random artifact with a stock of 1
        /// </summary>
        /// <param name="shopRNG"></param>
        private void AddAnArtifact(RNG shopRNG)
        {
            var artifact = shopRNG.GetRandomValueFromList(ItemList.GetArtifacts());
            var salePrice = GetAdjustedItemPrice(artifact, fallbackPrice: 50, multiplier: 3);
            AddStock(artifact.QualifiedId, UniqueItemId, salePrice, availableStock: 1);
        }

        /// <summary>
        /// Adds 5-15 iridium ores
        /// </summary>
        /// <param name="shopRNG"></param>
        private void AddIridiumOre(RNG shopRNG)
        {
            var iridiumOre = ObjectIndexes.IridiumOre.GetItem();
            var stock = shopRNG.NextIntWithinRange(5, 15);
            var salePrice = GetAdjustedItemPrice(iridiumOre, fallbackPrice: 50, multiplier: 5);
            AddStock(iridiumOre.QualifiedId, UniqueItemId, salePrice, stock);
        }

        /// <summary>
        /// Adds a random bar to the shop
        /// - 5% chance of 2-4 iridium
        /// - 3-8 of lesser bars
        /// Maple bars are here on purpose because lol
        /// </summary>
        /// <param name="shopRNG"></param>
        private void AddMetalBars(RNG shopRNG)
        {
            var getIridiumBar = shopRNG.NextBoolean(5);
            if (getIridiumBar)
            {
                var iridiumBar = ObjectIndexes.IridiumBar.GetItem();
                var stock = shopRNG.NextIntWithinRange(2, 4);
                var salePrice = GetAdjustedItemPrice(iridiumBar, fallbackPrice: 50, multiplier: 3);
                AddStock(iridiumBar.QualifiedId, UniqueItemId, salePrice, stock);
            }

            else
            {
                var commonMetalBars = new List<Item>()
                {
                    ObjectIndexes.CopperBar.GetItem(),
                    ObjectIndexes.IronBar.GetItem(),
                    ObjectIndexes.GoldBar.GetItem(),
                    ObjectIndexes.MapleBar.GetItem()
                };

                var stock = shopRNG.NextIntWithinRange(3, 8);
                var bar = shopRNG.GetRandomValueFromList(commonMetalBars);
                var salePrice = bar.ObjectIndex == ObjectIndexes.MapleBar
                    ? GetAdjustedItemPrice(bar, fallbackPrice: 50, multiplier: 1)
                    : GetAdjustedItemPrice(bar, fallbackPrice: 50, multiplier: 3);
                AddStock(bar.QualifiedId, UniqueItemId, salePrice, stock);
            }
        }
    }
}
