/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Shops;
using System.Linq;

namespace Randomizer
{
	public class RandomizedSeedShop : RandomizedShop
    {
        public RandomizedSeedShop() : base("SeedShop") { }

        public override bool ShouldModifyShop()
            => Globals.Config.RandomizeFruitTrees ||
                Globals.Config.Shops.AddSeedShopItemOfTheWeek;

        /// <summary>
        /// Adjusts fruit tree prices and adds the item of the week
        /// </summary>
        /// <returns>The modified shop data</returns>
        public override ShopData ModifyShop()
        {
            FixFruitTreePrices();
            AddItemOfTheWeek();

            return CurrentShopData;
        }

        /// <summary>
        /// Fruit tree prices are NOT set to -1 by default, resulting in hard-coded prices
        /// For randomized fruit trees, we modify the price, so set it accordingly
        /// Note that this shop has a 2x multiplier, so we do NOT want to set the price to -1
        /// </summary>
        private void FixFruitTreePrices()
        {
            if (!Globals.Config.RandomizeFruitTrees)
            {
                return;
            }

            foreach(ShopItemData shopItemData in CurrentShopData.Items)
            {
                Item matchingItem = ItemList.GetItemFromStringId(shopItemData.ItemId);
                if (matchingItem != null && matchingItem.IsRandomizedFruitTree)
                {
                    shopItemData.Price = Game1.objectData[matchingItem.Id].Price;
                }
            }
        }

        /// <summary>
        /// Adds an item of the week to Pierre's shop, refreshing every Monday
        /// Consists of a more expensive than usual item has a small chance of being hard to get
        /// </summary>
        private void AddItemOfTheWeek()
        {
            if (!Globals.Config.Shops.AddSeedShopItemOfTheWeek)
            {
                return;
            }

            RNG shopRNG = RNG.GetWeeklyRNG(nameof(RandomizedSeedShop));

            // Don't choose pre-existing items
            var itemsAlreadyInStock = CurrentShopData.Items
                .Select(item => item.ItemId)
                .ToList();

            // 1/10 chance of there being a better item in stock
            var validItems = shopRNG.NextBoolean(10)
                ? ItemList.GetItemsAtDifficulty(ObtainingDifficulties.MediumTimeRequirements)
                    .Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.LargeTimeRequirements))
                    .Where(x => !itemsAlreadyInStock.Contains(x.QualifiedId))
                    .Distinct()
                    .ToList()
                : ItemList.GetCraftableItems(CraftableCategories.Easy)
                    .Concat(ItemList.GetCraftableItems(CraftableCategories.EasyAndNeedMany))
                    .Concat(ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.MediumTimeRequirements))
                    .Where(x => !itemsAlreadyInStock.Contains(x.QualifiedId))
                    .Distinct()
                    .ToList();

            // Select an item to be the item of the week, and then add it
            Item itemOfTheWeek = validItems[shopRNG.Next(validItems.Count)];
            int salePrice = GetAdjustedItemPrice(itemOfTheWeek, fallbackPrice: 20, multiplier: 3);
            int stock = itemOfTheWeek.IsCraftable &&
                    ((itemOfTheWeek as CraftableItem).CraftableCategory == CraftableCategories.EasyAndNeedMany)
                ? shopRNG.NextIntWithinRange(30, 50)
                : shopRNG.NextIntWithinRange(3, 15);

            InsertStockAt(itemOfTheWeek.QualifiedId, "IoTW", salePrice, stock);
        }
    }
}
