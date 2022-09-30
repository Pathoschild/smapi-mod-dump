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
using Omegasis.Revitalize.Framework.Constants.ItemIds.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Farming;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public class MarniesShopUtilities
    {
        /// <summary>
        /// Adds stock to marnies shop based on various conditions.
        /// </summary>
        /// <param name="shopMenu"></param>
        public static void AddStockToMarniesShop(ShopMenu shopMenu)
        {
            if (BuildingUtilities.HasBuiltTier2OrHigherBarnOrCoop() || RevitalizeModCore.SaveDataManager.shopSaveData.animalShopSaveData.getHasBuiltTier2OrHigherBarnOrCoop())
            {
                HayMaker hayMaker = RevitalizeModCore.ModContentManager.objectManager.GetItem<HayMaker>(MachineIds.HayMaker, 1);
               ShopUtilities.AddItemToShop(shopMenu, hayMaker, RevitalizeModCore.Configs.shopsConfigManager.animalShopStockConfig.HayMakerPrice, -1);
            }
        }
    }
}
