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
using System;
using System.Linq;

namespace Randomizer
{
	public class RandomizedClubShop : RandomizedShop
    {
        public RandomizedClubShop() : base("Casino") { }

        public override bool ShouldModifyShop()
            => Globals.Config.Shops.RandomizeClubShop;

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
        /// Sell the following:
        /// - 3-5 furniture
        /// - 1 hat or clothing item
        /// - 2-3 higher-tier misc items
        /// - 1 totem type
        /// </summary>
        private void AdjustStock()
        {
			RNG shopRNG = RNG.GetWeeklyRNG(nameof(RandomizedClubShop));
            CurrentShopData.Items.Clear();

            AddFurniture(shopRNG);
            AddHatOrClothing(shopRNG);
            AddBigCraftable(shopRNG);
            AddMiscItems(shopRNG);
            AddTotem(shopRNG);
            ReAddFireworks();
            AddFloorsAndWallpapers(shopRNG);
        }

        /// <summary>
        /// Adds 3-5 random furniture items
        /// </summary>
        /// <param name="shopRNG"></param>
        private void AddFurniture(RNG shopRNG)
        {
            var numberOfFurniture = shopRNG.NextIntWithinRange(3, 5);
            var furnitureToSell = ItemList.GetRandomFurnitureToSell(shopRNG, numberOfFurniture);
            furnitureToSell.ForEach(item => 
                AddStock(item.QualifiedItemId, 
                    $"Furniture-{item.QualifiedItemId}",
                    price: GetSalePrice(item)));
        }

        /// <summary>
        /// Adds either a hat or a clothing item
        /// </summary>
        /// <param name="shopRNG"></param>
        private void AddHatOrClothing(RNG shopRNG)
        {
            var randomHat = ItemList.GetRandomHatsToSell(shopRNG, numberToGet: 1).First();
            var randomClothing = ItemList.GetRandomClothingToSell(shopRNG, numberToGet: 1).First();
            var hatOrClothingToSell = shopRNG.NextBoolean() ? randomHat : randomClothing;
            AddStock(hatOrClothingToSell.QualifiedItemId,
                "HatOrClothing",
                price: GetSalePrice(hatOrClothingToSell));
        }

        /// <summary>
        /// Adds a BigCraftable item
        /// </summary>
        /// <param name="shopRNG"></param>
        private void AddBigCraftable(RNG shopRNG)
        {
            var bigCraftableToSell = ItemList.GetRandomBigCraftablesToSell(shopRNG, numberToGet: 1).First();
            AddStock(bigCraftableToSell.QualifiedItemId,
                "BigCraftable",
                price: GetSalePrice(bigCraftableToSell, multiplier: 2));
        }

        /// <summary>
        /// Adds 2-3 medium + misc items
        /// </summary>
        /// <param name="shopRNG">The RNG to use</param>
        private void AddMiscItems(RNG shopRNG)
        {
            var numberOfMiscItems = shopRNG.NextIntWithinRange(2, 3);
            var poolOfMiscItemsToSell = ItemList.GetItemsAtDifficulty(ObtainingDifficulties.MediumTimeRequirements)
                .Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.LargeTimeRequirements))
                .Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.UncommonItem))
                .ToList();
            var miscItemsToSell = shopRNG.GetRandomValuesFromList(poolOfMiscItemsToSell, numberOfMiscItems);
            miscItemsToSell.ForEach(item =>
                AddStock(item.QualifiedId, 
                    $"MiscItem-{item.QualifiedId}",
                    price: GetSalePrice(item, minimumValue: 100)));
        }

        /// <summary>
        /// Adds a random totem type, alwas costing 500 Qi Coins
        /// </summary>
        /// <param name="shopRNG">The RNG to use</param>
        private void AddTotem(RNG shopRNG)
        {
            var totemId = ItemList.GetRandomTotem(shopRNG).QualifiedId;
            AddStock(totemId, "Totem", price: 500, availableStock: 20);
        }

        /// <summary>
        /// Re-adds the stock of fireworks
        /// This currently isn't randomized, as the entire stock of it is here
        /// </summary>
        private void ReAddFireworks()
        {
			AddFireworkItem(UnusedObjectIndexes.FireworksRed);
			AddFireworkItem(UnusedObjectIndexes.FireworksGreen);
			AddFireworkItem(UnusedObjectIndexes.FireworksPurple);
		}

        /// <summary>
        /// Adds the given firework item to the stock
        /// </summary>
        /// <param name="fireworks">The index of the fireworks</param>
        private void AddFireworkItem(UnusedObjectIndexes fireworks)
        {
            string id = $"(O){(int)fireworks}";
			AddStock(id, id, price: 200, availableStock: 20);
		}

		/// <summary>
		/// Adds a random set of floors and wallpapers
        /// </summary>
		/// <param name="shopRNG">The RNG to use</param>
		private void AddFloorsAndWallpapers(RNG shopRNG)
        {
            AddRandomItemType(shopRNG, "(WP)", "RandomWallpaper1", maxItems: 1);
			AddRandomItemType(shopRNG, "(WP)", "RandomWallpaper2", maxItems: 1);
			AddRandomItemType(shopRNG, "(FL)", "RandomFloor", maxItems: 1);
		}

		/// <summary>
		/// Adds a random item type - currently only used with floors and wallpapers
		/// Much cheaper than the normal prices, as they are random and may not be that rare
		/// </summary>
		/// <param name="shopRNG">The RNG used to compute the price</param>
		/// <param name="qualifier">The qualifier, with parens - ex: (WP) for wallpaper</param>
		/// <param name="uniqueId">A unique ID for the stock</param>
		/// <param name="maxItems">The max items to select from</param>
		private void AddRandomItemType(
			RNG shopRNG,
			string qualifier, 
            string uniqueId, 
            int maxItems)
        {
            AddStock(qualifiedId: $"RANDOM_ITEMS {qualifier}",
                uniqueId,
                price: shopRNG.NextIntWithinRange(3000, 8000),
                maxItems: maxItems);
        }

		/// <summary>
		/// Adjust the sale price of the item to be a factor of 10 since it doesn't cost coins;
		/// it costs the club currency!
		/// </summary>
		/// <param name="item">The item</param>
		/// <param name="minimumValue">The minimum value the item should be sold at</param>
		/// <param name="multiplier">The amount to multiply the sale price by</param>
		/// <returns>The computed sale price</returns>
		private static int GetSalePrice(Item item, int minimumValue = 1000, int multiplier = 1)
        {
            return GetSalePrice(item.GetSaliableObject(), minimumValue, multiplier);
        }

        /// <summary>
        /// Adjust the sale price of the item to be a factor of 10 since it doesn't cost coins;
        /// it costs the club currency!
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="minimumValue">The minimum value the item should be sold at</param>
        /// <param name="multiplier">The amount to multiply the sale price by</param>
        /// <returns>The computed sale price</returns>
        private static int GetSalePrice(ISalable item, int minimumValue = 1000, int multiplier = 1)
        {
            var baseSalePrice = Math.Max(minimumValue, item.salePrice() / 10);
            return GetAdjustedItemPrice(item, baseSalePrice, multiplier);
        }
    }
}
