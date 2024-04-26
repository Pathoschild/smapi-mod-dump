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
using System.Linq;

namespace Randomizer
{
    public class RandomizedOasisShop : RandomizedShop
    {
        public RandomizedOasisShop() : base("Sandy") { }

		public override bool ShouldModifyShop()
	        => Globals.Config.Shops.RandomizeOasisShop;

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
        /// Adjust the oasis shop stock to sell more exotic items, including furnature
        /// Has some logic based on the day of the week
        /// </summary>
        private void AdjustStock()
        {
            // Track the seeds so we can add them back and add the matching crop every Tuesday
            var desertShopSeeds = CurrentShopData.Items
                .Select(shopData => ItemList.GetItemFromStringId(shopData.ItemId))
                .Where(item => item is SeedItem)
                .Cast<SeedItem>()
                .ToList();

            CurrentShopData.Items.Clear();

            // Most of the stock will change every Monday, with a couple exceptions
            RNG weeklyShopRNG = RNG.GetWeeklyRNG(nameof(RandomizedOasisShop));
            RNG dailyShopRNG = RNG.GetDailyRNG(nameof(RandomizedOasisShop));

            desertShopSeeds.ForEach(seed => AddStock(seed.QualifiedId, $"SeedItem-{seed.QualifiedId}"));
            AddDaySpecificItems(desertShopSeeds, weeklyShopRNG);
            AddRandomItems(weeklyShopRNG);
            AddClothingAndFurnatureItems(weeklyShopRNG, dailyShopRNG);
        }

        /// <summary>
        /// Adds... 
        /// - a random desert foragable every weekday
        /// - a crop corresponding to the seeds sold here every Tuesday
        /// - a random cooked item every weekend
        /// All refreshing on Monday
        /// </summary>
        /// <param name="desertShopSeeds">The list of seeds normally sold in this shop - used to get the corresponding crop</param>
        /// <param name="weeklyShopRNG">The weekly RNG</param>
        private void AddDaySpecificItems(List<SeedItem> desertShopSeeds, RNG weeklyShopRNG)
        {
            // Every weekday, add a desert foragable
            var desertForagable = weeklyShopRNG.GetRandomValueFromList(ItemList.GetUniqueDesertForagables());
            int foragablePrice = GetAdjustedItemPrice(desertForagable, fallbackPrice: 50, multiplier: 2);
            int foragableStock = weeklyShopRNG.NextIntWithinRange(1, 5);
            AddStock(desertForagable.QualifiedId, 
                "WeekdayForagable", 
                foragablePrice,
                foragableStock,
                condition: DayFunctions.GetConditionForWeekday());

            // Every Tuesday, add a crop corresponding to the seeds sold here
            var desertShopCrops = desertShopSeeds
                .Select(item => ItemList.Items[item.CropId])
                .ToList();
            var desertCrop = weeklyShopRNG.GetRandomValueFromList(desertShopCrops);
            int desertCropPrice = GetAdjustedItemPrice(desertCrop, fallbackPrice: 50, multiplier: 2);
            int desertCropStock = weeklyShopRNG.NextIntWithinRange(3, 8);
            AddStock(desertCrop.QualifiedId, 
                "TuesdayCrop", 
                desertCropPrice, 
                desertCropStock,
                condition: DayFunctions.GetCondition(Days.Tuesday));

            // Every weekend, add a random cooked item
            var cookedItem = weeklyShopRNG.GetRandomValueFromList(ItemList.GetCookedItems());
            AddStock(cookedItem.QualifiedId, 
                "WeekendFood", 
                condition: DayFunctions.GetConditionForWeekend());
        }

        /// <summary>
        /// Adds...
        /// - a random craftable item in the moderate category
        /// - a random recource item
        /// </summary>
        /// <param name="weeklyShopRNG">The weekly RNG</param>
        private void AddRandomItems(RNG weeklyShopRNG)
        {
            var craftableItem = weeklyShopRNG.GetRandomValueFromList(
                ItemList.GetCraftableItems(CraftableCategories.Moderate));
            int craftableSalePrice = GetAdjustedItemPrice(craftableItem, fallbackPrice: 50, multiplier: 2);
            AddStock(craftableItem.QualifiedId, "CraftableItem", price: craftableSalePrice);

            var resourceItem = ItemList.GetRandomResourceItem(weeklyShopRNG);
            int resourceSalePrice = GetAdjustedItemPrice(resourceItem, fallbackPrice: 30, multiplier: 4);
            AddStock(resourceItem.QualifiedId, "ResourceItem", price: resourceSalePrice);
        }

        /// <summary>
        /// Adds...
        /// - a daily clothing item
        /// - a daily furnature item
        /// - 4 weekly furniture items
        /// </summary>
        /// <param name="weeklyShopRNG">The weekly RNG</param>
        /// <param name="dailyShopRNG">The daily RNG</param>
        private void AddClothingAndFurnatureItems(RNG weeklyShopRNG, RNG dailyShopRNG)
        {
            var dailyClothingItemId = ClothingFunctions.GetRandomClothingQualifiedId(dailyShopRNG);
            var dailyFurnitureItemId = FurnitureFunctions.GetRandomFurnitureQualifiedId(dailyShopRNG);
            var weeklyFurnitureItemIds = FurnitureFunctions.GetRandomFurnitureQualifiedIds(
                weeklyShopRNG,
                numberToGet: 4,
                new List<string>() { dailyClothingItemId, dailyFurnitureItemId });

            AddStock(dailyClothingItemId, "DailyClothing");
            AddStock(dailyFurnitureItemId, "DailyFurniture");
            weeklyFurnitureItemIds.ForEach(itemId =>
                AddStock(itemId, $"WeeklyFurniture-{itemId}"));
        }
    }
}
