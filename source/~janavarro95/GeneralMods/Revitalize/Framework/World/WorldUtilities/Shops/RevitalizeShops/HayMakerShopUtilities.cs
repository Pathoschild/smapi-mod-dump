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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops.RevitalizeShops
{
    /// <summary>
    /// Utilities for the Hay Maker Shop. 
    /// </summary>
    public static class HayMakerShopUtilities
    {
        public static string StoreContext = "Omegasis.Revitalize.Shops.HayMaker";

        public static void AddItemsToShop(ShopMenu Menu)
        {
            Menu.onPurchase = OnPurchaseFromShop;

            ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Hay), HaySellPrice(), int.MaxValue);
            if(CanBuySiloRefillItem(HaySellPrice()))
            {
                ShopUtilities.AddItemToShop(Menu,RevitalizeModCore.ModContentManager.objectManager.getItem(FarmingItems.RefillSilosFakeItem), GetPriceForSiloRefill(), -1);
            }
        }

        /// <summary>
        /// Called when purchasing an item from the hay maker shop.
        /// </summary>
        /// <param name="purchasedItem"></param>
        /// <param name="who"></param>
        /// <param name="AmountPurchased"></param>
        /// <returns>A bool representing if the menu should be closed or not.</returns>
        public static bool OnPurchaseFromShop(ISalable purchasedItem, Farmer who, int AmountPurchased)
        {
            if (purchasedItem is IBasicItemInfoProvider)
            {
                if((purchasedItem as IBasicItemInfoProvider).Id.Equals(FarmingItems.RefillSilosFakeItem))
                {
                    int addedHay = FarmUtilities.GetNumberOfHayPiecesUntilFullSilosLimitByPlayersMoney(HaySellPrice());
                    FarmUtilities.FillSilosFromSiloReillItem(RevitalizeModCore.Configs.shopsConfigManager.hayMakerShopConfig.HayMakerShopHaySellPrice);
                    SoundUtilities.PlaySound(Enums.StardewSound.Ship);
                    string infoMessage =string.Format(JsonContentPackUtilities.LoadShopDialogue(FarmUtilities.AreSilosAtMaxCapacity() ? "SiloFullRefill" : "SiloPartialRefill", "HayMakerShopDialogue.json"), addedHay, RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Hay).DisplayName, Game1.getFarm().piecesOfHay.Value, FarmUtilities.GetSiloCapacity());
                    Game1.addHUDMessage(new HUDMessage(infoMessage));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the cost for selling hay.
        /// </summary>
        /// <returns></returns>
        public static int HaySellPrice()
        {
            return RevitalizeModCore.Configs.shopsConfigManager.hayMakerShopConfig.HayMakerShopHaySellPrice;
        }

        /// <summary>
        /// Gets the price of the silo refil item using the config file price amount.
        /// </summary>
        /// <returns></returns>
        public static int GetPriceForSiloRefill()
        {
            return GetPriceForSiloRefill(HaySellPrice());
        }


        /// <summary>
        /// Gets the price of the silo refil item.
        /// </summary>
        /// <param name="HayPrice"></param>
        /// <returns></returns>
        public static int GetPriceForSiloRefill(int HayPrice)
        {
            return HayPrice * FarmUtilities.GetNumberOfHayPiecesUntilFullSilosLimitByPlayersMoney(HaySellPrice());
        }

        /// <summary>
        /// Can the player buy the silo refill item?
        /// </summary>
        /// <param name="HayPrice"></param>
        /// <returns></returns>
        public static bool CanBuySiloRefillItem(int HayPrice)
        {
            return Utility.numSilos() >= 1 && FarmUtilities.GetNumberOfHayPiecesUntilFullSilos() > 0 && Game1.player.Money >= HayPrice;
        }
    }
}
