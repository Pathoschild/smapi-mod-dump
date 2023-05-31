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
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.World.Objects;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    /// <summary>
    /// Used to sell more things at the movie theater.
    /// </summary>
    public class MovieTheaterShopUtilities
    {
        /// <summary>
        /// What happens when something is bought from Robin's shop by default.
        /// </summary>
        public static Func<ISalable, Farmer, int, bool> DefaultOnPurchaseMethod;

        public static void AddStockToShop(ShopMenu shop)
        {
            DefaultOnPurchaseMethod = shop.onPurchase;
            shop.onPurchase = OnItemBought;

            if (!RevitalizeModCore.SaveDataManager.playerSaveData.hasMovieTheaterTicketSubscription)
            {
                ShopUtilities.AddItemToShop(shop, RevitalizeModCore.ModContentManager.objectManager.getItem(MiscItemIds.MovieTheaterTicketSubscription), RevitalizeModCore.Configs.shopsConfigManager.movieTheaterShopConfig.MovieTicketSubscriptionPrice, 1);
            }
        }

        public static bool OnItemBought(ISalable purchasedItem, Farmer who, int AmountPurchased)
        {
            if(purchasedItem is CustomItem)
            {
                CustomItem item = (purchasedItem as CustomItem);
                if (item.Id.Equals(MiscItemIds.MovieTheaterTicketSubscription))
                {
                    RevitalizeModCore.SaveDataManager.playerSaveData.hasMovieTheaterTicketSubscription = true;
                    return true;
                }
            }
            return DefaultOnPurchaseMethod.Invoke(purchasedItem, who, AmountPurchased);
        }

    }
}
