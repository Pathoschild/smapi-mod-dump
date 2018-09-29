using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicApi2.Data;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class ShopService
    {
        private static readonly Lazy<ShopService> Lazy = new Lazy<ShopService>(() => new ShopService());
        public static ShopService Instance => Lazy.Value;
        public event EventHandler<EventArgsClickableMenuChanged> MenuItemsAdded;

        private ISet<string> Shops { get; } = new HashSet<string>();
        private IDictionary<string, IList<ShopItemInfo>> ShopFurniture { get; } = new Dictionary<string, IList<ShopItemInfo>>();

        private ShopService()
        {
            MenuEvents.MenuChanged += OnMenuChanged;
        }

        public ShopService AddFurniture(string location, ShopItemInfo shopItemInfo)
        {
            if (!ShopFurniture.TryGetValue(location, out var items))
            {
                ShopFurniture.Add(location, items = new List<ShopItemInfo>());
            }
            items.Add(shopItemInfo);
            Shops.Add(location);
            return this;
        }

        private void OnMenuChanged(object sender, EventArgsClickableMenuChanged args)
        {
            var locationName = Game1.currentLocation?.Name;
            if (!Shops.Contains(locationName) || !(args.NewMenu is ShopMenu shopMenu)) return;

            var menuProxy = new ShopMenuProxy(shopMenu);
            AddItems(locationName, menuProxy, ShopFurniture, si => new Furniture(si.ID, Vector2.Zero));
            // TODO: Other items.

            MenuItemsAdded?.Invoke(this, args);
        }

        private void AddItems(string locationName, ShopMenuProxy shopMenu, IDictionary<string, IList<ShopItemInfo>> shopItemsInfo, Func<ShopItemInfo, Object> createItem)
        {
            if (!shopItemsInfo.TryGetValue(locationName, out var shopItems)) return;
            foreach (var shopItem in shopItems)
            {
                shopMenu.AddItem(createItem(shopItem));
            }
        }
    }
}