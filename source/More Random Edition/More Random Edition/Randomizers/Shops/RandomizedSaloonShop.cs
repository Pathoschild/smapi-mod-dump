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
    public class RandomizedSaloonShop : RandomizedShop
    {
        public RandomizedSaloonShop() : base("Saloon") { }

        public override bool ShouldModifyShop()
            => Globals.Config.Shops.RandomizeSaloonShop;

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
        /// The saloon shop will be mostly random now - cycling every Monday
        /// - Beer and Coffee will still be available
        /// - 3-5 random cooked foods will be sold
        /// - 3-5 random recipes will be sold (not shown if the player has them)
        /// </summary>
        private void AdjustStock()
        {
            // Stock will change every Monday
            RNG shopRNG = RNG.GetWeeklyRNG(nameof(RandomizedSaloonShop));
            CurrentShopData.Items.Clear();

            // Beer and coffee will always be available
            AddStock(ItemList.GetQualifiedId(ObjectIndexes.Beer), "BeerItem");
            AddStock(ItemList.GetQualifiedId(ObjectIndexes.Coffee), "CoffeeItem");

            // Random Cooked Items - pick 3-5 random dishes each week
            var numberOfCookedItems = shopRNG.NextIntWithinRange(3, 5);
            List<string> allCookedItemIds = ItemList.GetCookedItems()
                .Select(item => item.QualifiedId)
                .ToList();
            List<string> gusFoodList =
                shopRNG.GetRandomValuesFromList(allCookedItemIds, numberOfCookedItems);
            gusFoodList.ForEach(itemId => AddStock(itemId, $"FoodItem-{itemId}"));

            // Random Cooking Recipes - pick 3-5 random recipes each week
            // Note that the game will not include these if they are already learned
            var numberOfRecipes = shopRNG.NextIntWithinRange(3, 5);
            List<string> gusRecipeList = 
                shopRNG.GetRandomValuesFromList(allCookedItemIds, numberOfRecipes);
            gusRecipeList.ForEach(itemId =>
                AddStock(itemId, $"RecipeItem-{itemId}", isRecipe: true));
        }
    }
}
