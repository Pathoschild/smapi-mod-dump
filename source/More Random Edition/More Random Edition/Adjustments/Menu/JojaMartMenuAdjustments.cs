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
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    internal class JojaMartMenuAdjustments : ShopMenuAdjustments
    {
        /// <summary>
        /// Adds the item of the week
        /// </summary>
        /// <param name="menu">The shop menu</param>
        protected override void Adjust(ShopMenu menu)
        {
            if (!ShouldChangeShop)
            {
                RestoreShopState(menu);
                return;
            }

            if (Globals.Config.Shops.AddJojaMartItemOfTheWeek)
            {
                AddItemOfTheWeek(menu);
            }
        }

        /// <summary>
        /// Adds a random item for item of the week
        /// Better chance of a better item - and cheaper than the seed shop - the Joja spirit!
        /// </summary>
        /// <param name="menu">The shop menu</param>
        private static void AddItemOfTheWeek(ShopMenu menu)
        {
            Random shopRNG = Globals.GetWeeklyRNG(nameof(JojaMartMenuAdjustments));

            // Build list of possible items
            var itemsAlreadyInStock = menu.itemPriceAndStock.Keys
                .Where(shopKey => shopKey is SVObject)
                .Select(item => (item as SVObject).ParentSheetIndex)
                .ToList();

            // 15% chance of there being a better item in stock
            var validItems = Globals.RNGGetNextBoolean(15, shopRNG)
                ? ItemList.GetItemsAtDifficulty(ObtainingDifficulties.MediumTimeRequirements)
                    .Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.LargeTimeRequirements))
                    .Where(x => !itemsAlreadyInStock.Contains(x.Id))
                    .Distinct()
                    .ToList()
                : ItemList.GetCraftableItems(CraftableCategories.Easy)
                    .Concat(ItemList.GetCraftableItems(CraftableCategories.EasyAndNeedMany))
                    .Concat(ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.MediumTimeRequirements))
                    .Where(x => !itemsAlreadyInStock.Contains(x.Id))
                    .Distinct()
                    .ToList();

            // Select an item to be the item of the week
            Item itemOfTheWeek = validItems[shopRNG.Next(validItems.Count)];
            int stock = itemOfTheWeek.IsCraftable &&
                    ((itemOfTheWeek as CraftableItem).Category == CraftableCategories.EasyAndNeedMany)
                ? Range.GetRandomValue(30, 50, shopRNG)
                : Range.GetRandomValue(3, 15, shopRNG);
            int salePrice = GetAdjustedItemPrice(itemOfTheWeek, fallbackPrice: 15, multiplier: 2);
            InsertStockAt(menu, itemOfTheWeek.GetSaliableObject(initialStack: stock), stock, salePrice);
        }
    }
}
