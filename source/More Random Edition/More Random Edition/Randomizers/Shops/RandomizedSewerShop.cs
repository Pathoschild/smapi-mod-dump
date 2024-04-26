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
	public class RandomizedSewerShop : RandomizedShop
    {
        public RandomizedSewerShop() : base("ShadowShop") { }

		public override bool ShouldModifyShop()
			=> Globals.Config.Shops.RandomizerSewerShop;

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
        /// Replace the Monster Fireplace and Sign of the Vessel with daily equivalents
        /// Means a random Furniture and a random BigCraftable
        /// </summary>
        private void AdjustStock()
        {
            RNG shopRNG = RNG.GetDailyRNG(nameof(RandomizedSewerShop));

            string fireplaceId = FurnitureFunctions.GetQualifiedId(FurnitureIndexes.MonsterFireplace);
            ShopItemData firePlaceItem = GetShopItemById(fireplaceId);
            firePlaceItem.ItemId = FurnitureFunctions.GetRandomFurnitureQualifiedId(shopRNG);

            string vesselId = BigCraftableFunctions.GetQualifiedId(BigCraftableIndexes.SignOfTheVessel);
            ShopItemData vesselItem = GetShopItemById(vesselId);
            ISalable bigCraftableToSell = ItemList.GetRandomBigCraftablesToSell(shopRNG, numberToGet: 1).First();
            var bigCraftableSalePrice = GetAdjustedItemPrice(bigCraftableToSell, fallbackPrice: 500, multiplier: 3);
            vesselItem.Price = bigCraftableSalePrice;
            vesselItem.ItemId = BigCraftableFunctions.GetRandomBigCraftableQualifiedId(shopRNG);
        }
    }
}
