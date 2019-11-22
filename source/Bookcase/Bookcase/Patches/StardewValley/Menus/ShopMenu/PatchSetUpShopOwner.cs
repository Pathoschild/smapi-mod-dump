using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection;
using System;
using Bookcase.Events;

namespace Bookcase.Patches {

    class PatchSetUpShopOwner : IGamePatch {

        public Type TargetType => typeof(ShopMenu);

        public MethodBase TargetMethod => TargetType.GetMethod("setUpShopOwner");

        public static void Postfix(ShopMenu __instance, string who, Dictionary<Item, int[]> ___itemPriceAndStock, List<int> ___categoriesToSellHere, List<Item> ___forSale) {

            BookcaseEvents.ShopSetupEvent.Post(new ShopSetupEvent(__instance, who, ___itemPriceAndStock, ___categoriesToSellHere, ___forSale));
        }
    }
}
