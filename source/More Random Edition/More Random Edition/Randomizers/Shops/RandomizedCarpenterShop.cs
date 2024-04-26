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
using System.Linq;

namespace Randomizer
{
    public class RandomizedCarpenterShop : RandomizedShop
    {
        public RandomizedCarpenterShop() : base("Carpenter") { }

        /// <summary>
        /// Modify the shop if either the tapper ingredient or clay is being added
        /// </summary>
        /// <returns>True if the shop is being modified</returns>
        public override bool ShouldModifyShop()
            => ShouldAddTapperIngredent() ||
                Globals.Config.Shops.AddClayToRobinsShop;

		/// <summary>
		/// Whether we should add the tapper ingredient to the shop
		/// </summary>
		/// <returns>True if the tapper ingredient should be added, false otherwise</returns>
		private static bool ShouldAddTapperIngredent()
            => Globals.Config.Shops.AddTapperCraftItemsToRobinsShop &&
                Globals.Config.CraftingRecipes.Randomize;

		/// <summary>
		/// Adds clay and tapper craft items
		/// </summary>
		public override ShopData ModifyShop()
        {
            if (ShouldAddTapperIngredent())
            {
                AddRandomTapperCraftingIngredient();
            }

            if (Globals.Config.Shops.AddClayToRobinsShop)
            {
                AddClay();
            }

            return CurrentShopData;
        }

        /// <summary>
        /// Adds a random item to the shop that is required to make a tapper - changes daily
        /// </summary>
        private void AddRandomTapperCraftingIngredient()
        {
            var exitingStockIds = CurrentShopData.Items
                .Select(item => item.ItemId)
                .ToList();

            RNG shopRNG = RNG.GetDailyRNG($"{nameof(RandomizedCarpenterShop)}.{nameof(AddRandomTapperCraftingIngredient)}");
            var tapperItemIdsAndStock = ((CraftableItem)BigCraftableIndexes.Tapper.GetItem()).LastRecipeGenerated;
            var tapperItems = tapperItemIdsAndStock.Keys
                .Select(id => id.GetItem())
                .Where(item => !exitingStockIds.Contains(item.QualifiedId))
                .ToList();

            if (tapperItems.Any())
            {
                var tapperItemToSell = shopRNG.GetRandomValueFromList(tapperItems);
                var stock = tapperItemIdsAndStock[tapperItemToSell.ObjectIndex];
                var price = GetAdjustedItemPrice(tapperItemToSell, 50, 5);
                InsertStockAt(tapperItemToSell.QualifiedId, "TapperItem", price, stock, index: 2);
            }
        }

        /// <summary>
        /// Adds Clay to Robin's shop since it's really grindy to get
        /// Add some randomness to the price each day (between 25-75 coins each)
        /// </summary>
        private void AddClay()
        {
            RNG shopRNG = RNG.GetDailyRNG($"{nameof(RandomizedCarpenterShop)}.{nameof(AddClay)}");
            const int BaseClayPrice = 50;
            var clayStock = shopRNG.NextIntWithinRange(20, 40);
            var clayPrice = shopRNG.NextIntWithinPercentage(BaseClayPrice, 50);

            InsertStockAt(
                ItemList.GetQualifiedId(ObjectIndexes.Clay), 
                "Clay", 
                clayPrice, 
                clayStock, 
                index: 2
            );
        }
    }
}
