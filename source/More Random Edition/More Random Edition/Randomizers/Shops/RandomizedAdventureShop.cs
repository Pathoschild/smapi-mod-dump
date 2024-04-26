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
using StardewValley.Objects;
using StardewValley.Tools;

namespace Randomizer
{
    public class RandomizedAdventureShop : RandomizedShop
    {
        public RandomizedAdventureShop() : base("AdventureShop") { }

        /// <summary>
        /// Modify the shop only if weapons or boots are randomized
        /// </summary>
        /// <returns>True if we should modify the shop</returns>
		public override bool ShouldModifyShop()
		{
			return Globals.Config.Weapons.Randomize ||
                Globals.Config.Boots.Randomize;
		}

		/// <summary>
		/// Modifies the shop so that prices are fixed for randomized items
		/// </summary>
		public override ShopData ModifyShop()
        {
			FixPrices();
            
            return CurrentShopData;
        }

        /// <summary>
        /// Fixes sale prices for randomized gear so that nothing sells for more than it's worth
        /// Currently only needs to adjust boots and melee weapons
        /// </summary>
        private void FixPrices()
        {
            foreach(ShopItemData shopData in CurrentShopData.Items)
            {
                var item = ItemRegistry.Create(shopData.ItemId, allowNull: true);
                if (item is Boots || item is MeleeWeapon)
                {
                    shopData.Price = item.salePrice();
                }
            }
        }
    }
}
