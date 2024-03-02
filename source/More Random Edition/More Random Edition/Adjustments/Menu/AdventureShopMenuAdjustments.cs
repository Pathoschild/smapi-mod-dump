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
using System.Linq;

namespace Randomizer
{
    internal class AdventureShopMenuAdjustments : ShopMenuAdjustments
    {
        public AdventureShopMenuAdjustments() : base()
        {
            SkipShopSave = true;
        }

        /// <summary>
        /// Callthrough to FixPrices
        /// This shop doesn't need to be restored or anything, as we're only modifying prices and nothing else
        /// </summary>
        /// <param name="menu">The shop menu</param>
        protected override void Adjust(ShopMenu menu)
        {
            FixPrices(menu);
        }

        /// <summary>
        /// Fixes sale prices for randomized gear so that nothing sells for more than it's worth
        /// </summary>
        /// <param name="menu">The shop menu</param>
        private static void FixPrices(ShopMenu menu)
        {
            menu.itemPriceAndStock = menu.itemPriceAndStock.ToDictionary(
                item => item.Key,
                item => new[] { item.Key.salePrice(), _maxValue }
            );
        }
    }
}
