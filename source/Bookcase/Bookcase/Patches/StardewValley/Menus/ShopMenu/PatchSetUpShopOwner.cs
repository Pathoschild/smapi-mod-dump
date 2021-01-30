/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using Bookcase.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bookcase.Patches {

    class PatchSetUpShopOwner : IGamePatch {

        public Type TargetType => typeof(ShopMenu);

        public MethodBase TargetMethod => TargetType.GetMethod("setUpShopOwner");

        public static void Postfix(ShopMenu __instance, string who, Dictionary<Item, int[]> ___itemPriceAndStock, List<int> ___categoriesToSellHere, List<Item> ___forSale) {

            BookcaseEvents.ShopSetupEvent.Post(new ShopSetupEvent(__instance, who, ___itemPriceAndStock, ___categoriesToSellHere, ___forSale));
        }
    }
}
