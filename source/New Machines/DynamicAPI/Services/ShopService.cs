/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class ShopService
    {
        #region Private Data

        private readonly List<ShopInfo> _shopInfos = new List<ShopInfo>();

        #endregion

        #region Sigleton Access

        private static ShopService _instance;

        public static ShopService Instance => _instance ?? (_instance = new ShopService());

        private ShopService()
        {
            MenuEvents.MenuChanged += OnMenuChanged;
        }

        #endregion

        public void AddShopInfo(ShopInfo shopInfo)
        {
            _shopInfos.Add(shopInfo);
        }

        private void OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (!(e.NewMenu is ShopMenu)) return;

            var shopMenu = (ShopMenu)e.NewMenu;
            var itemsForSale = shopMenu.GetField<List<Item>>("forSale");
            var itemsPriceAndStock = shopMenu.GetField<Dictionary<Item, int[]>>("itemPriceAndStock");

            var currentShopItems = _shopInfos
                .Where(si => si.Location == Game1.currentLocation.Name)
                .SelectMany(si => si.Items)
                .ToList();

            var cookingRecipes = Game1.player.cookingRecipes;

            foreach (var shopItem in currentShopItems)
            {
                Object obj;
                int price;
                if ((shopItem as Object)?.isRecipe == true)
                {
                    if (cookingRecipes.ContainsKey(shopItem.Name)) continue;
                    obj = shopItem as Object;
                    price = obj.Price * 4;
                }
                else
                {
                    obj = new Object(Vector2.Zero, shopItem.parentSheetIndex, int.MaxValue);
                    price = obj.price;
                }
                itemsForSale.Add(obj);
                itemsPriceAndStock.Add(obj, new[] { price, obj.Stack });
            }
        }
    }
}
