/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Force.DeepCloner;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public abstract class ShopStockModifier
    {
        protected IMonitor _monitor;
        protected IModHelper _helper;
        protected ArchipelagoClient _archipelago;
        protected StardewItemManager _stardewItemManager;

        public ShopStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _stardewItemManager = stardewItemManager;
        }

        public abstract void OnShopStockRequested(object sender, AssetRequestedEventArgs e);

        protected bool AssetIsShops(AssetRequestedEventArgs e)
        {
            return e.NameWithoutLocale.IsEquivalentTo("Data/Shops");
        }

        protected ShopItemData CreateArchipelagoLocation(ShopItemData item, string location)
        {
            var id = $"{IDProvider.AP_LOCATION} {location}";
            var apShopItem = item.DeepClone();
            apShopItem.Id = $"{this.GetType().Name}: {id}";
            apShopItem.ItemId = id;
            apShopItem.AvailableStock = 1;
            apShopItem.IsRecipe = false;
            apShopItem.AvoidRepeat = true;

            if (apShopItem.Price <= 0 && string.IsNullOrWhiteSpace(apShopItem.TradeItemId))
            {
                apShopItem.Price = _stardewItemManager.GetItemByQualifiedId(item.ItemId)?.SellPrice ?? throw new Exception($"Could not find price for purchasable location {location}");
            }

            if (!string.IsNullOrWhiteSpace(apShopItem.Condition))
            {
                apShopItem.Condition = GameStateConditionProvider.RemoveCondition(apShopItem.Condition, GameStateConditionProvider.HAS_RECEIVED_ITEM);
            }

            return apShopItem;
        }

        protected void ReplaceWithArchipelagoCondition(ShopItemData shopItem, string item, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return;
            }

            shopItem.Condition = GameStateConditionProvider.CreateHasReceivedItemCondition(item, amount);
        }
    }
}
