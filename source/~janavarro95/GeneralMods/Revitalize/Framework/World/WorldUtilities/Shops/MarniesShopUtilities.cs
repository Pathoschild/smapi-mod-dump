/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.Objects.Farming;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Shops.RevitalizeShops;
using StardewValley;
using StardewValley.Menus;
using static Omegasis.Revitalize.Framework.World.WorldUtilities.Shops.ShopUtilities;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class MarniesShopUtilities
    {
        /// <summary>
        /// What happens when something is bought from Robin's shop by default.
        /// </summary>
        public static Func<ISalable, Farmer, int, bool> DefaultOnPurchaseMethod;


        /// <summary>
        /// Adds stock to marnies shop based on various conditions.
        /// </summary>
        /// <param name="shopMenu"></param>
        public static void AddStockToMarniesShop(ShopMenu shopMenu)
        {
            shopMenu.onPurchase = OnPurchaseFromShop;


            List<ShopInventoryProbe> shopInventoryProbes = new List<ShopInventoryProbe>();

            int hayPrice = 50;
            if (HayMakerShopUtilities.CanBuySiloRefillItem(hayPrice))
            {
                shopInventoryProbes.Add(AddItemToStockAfterHay(FarmingItems.RefillSilosFakeItem, HayMakerShopUtilities.GetPriceForSiloRefill(hayPrice)));
            }

            if (RevitalizeModCore.SaveDataManager.shopSaveData.animalShopSaveData.getHasBuiltTier2OrHigherBarnOrCoop() || BuildingUtilities.HasBuiltTier2OrHigherBarnOrCoop())
            {
                if (!PlayerUtilities.KnowsCraftingRecipe(Constants.CraftingIds.CraftingRecipeBooks.WorkbenchCraftingRecipies, Constants.CraftingIds.RecipeIds.WorkbenchRecipeIds.HayMakerRecipeId))
                {
                    shopInventoryProbes.Add(AddItemToStockAfterHay(WorkbenchBlueprintIds.HayMakerBlueprint, RevitalizeModCore.Configs.shopsConfigManager.animalShopStockConfig.HayMakerBlueprintsPrice));
                }

                shopInventoryProbes.Add(AddItemToStockAfterHay(FarmingObjectIds.HayMaker_FeedShop, RevitalizeModCore.Configs.shopsConfigManager.animalShopStockConfig.HayMakerFeedShopPrice));
            }

            ShopUtilities.UpdateShopStockAndPriceInSortedOrder(shopMenu, shopInventoryProbes);
        }

        /// <summary>
        /// Creates a <see cref="ShopInventoryProbe"/> to add an item after stone in the shop's stock.
        /// </summary>
        /// <param name="ItemToAdd"></param>
        /// <param name="SellingPrice"></param>
        /// <param name="AmountToSell"></param>
        /// <returns></returns>
        private static ShopInventoryProbe AddItemToStockAfterHay(string ItemToAdd, int SellingPrice, int AmountToSell = -1)
        {
            return ShopUtilities.CreateInventoryShopProbe(new ItemFoundInShopInventory((itemForSale, FoundItemPrice, Stock) => itemForSale.GetType().Equals(typeof(StardewValley.Object)) && (itemForSale as StardewValley.Object).parentSheetIndex == (int)Enums.SDVObject.Hay), ItemToAdd, SellingPrice, AmountToSell);
        }

        /// <summary>
        /// Called when purchasing an item from the hay maker shop.
        /// </summary>
        /// <param name="purchasedItem"></param>
        /// <param name="who"></param>
        /// <param name="AmountPurchased"></param>
        /// <returns>A bool representing if the menu should be closed or not.</returns>
        private static bool OnPurchaseFromShop(ISalable purchasedItem, Farmer who, int AmountPurchased)
        {
            if (purchasedItem is IBasicItemInfoProvider)
            {
                if ((purchasedItem as IBasicItemInfoProvider).Id.Equals(FarmingItems.RefillSilosFakeItem))
                {
                    return HayMakerShopUtilities.OnPurchaseFromShop(purchasedItem, who, AmountPurchased);
                }
            }
            return false;
        }
    }
}
