/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using Force.DeepCloner;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.ShopStockModifiers
{
    public class TravelingMerchantShopStockModifier : ShopStockModifier
    {
        public TravelingMerchantShopStockModifier(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, modHelper, archipelago, stardewItemManager)
        {
        }

        public override void OnShopStockRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!AssetIsShops(e))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var shopsData = asset.AsDictionary<string, ShopData>().Data;
                    var cartShopData = shopsData["Traveler"];
                    SetUpRandomItemsStockSize(cartShopData);
                    AddMetalDetectorItems(cartShopData);
                    AddChecks(cartShopData);
                    RemoveWeddingRingCondition(cartShopData);
                },
                AssetEditPriority.Late - 1
            );
        }

        private void SetUpRandomItemsStockSize(ShopData cartShopData)
        {
            for (var i = 0; i < cartShopData.Items.Count; i++)
            {
                var item = cartShopData.Items[i];
                if (item.Id != "RandomObjects" || item.ItemId != "RANDOM_ITEMS (O) 2 789 @requirePrice @isRandomSale" || item.MaxItems <= 1 || !string.IsNullOrWhiteSpace(item.Condition))
                {
                    continue;
                }

                cartShopData.Items.RemoveAt(i);

                for (var numberItems = 0; numberItems < item.MaxItems; numberItems++)
                {
                    var newItem = item.DeepClone();
                    newItem.Id = $"RandomObjects[{numberItems}]";
                    newItem.MaxItems = 1;
                    newItem.Condition = GameStateConditionProvider.CreateHasStockSizeCondition(numberItems);
                    newItem.ActionsOnPurchase = new List<string> { TriggerActionProvider.TRAVELING_MERCHANT_PURCHASE };
                    cartShopData.Items.Insert(i + numberItems, newItem);
                }
            }
        }

        private void AddMetalDetectorItems(ShopData cartShopData)
        {
            var metalDetector = new ShopItemData()
            {
                Id = IDProvider.METAL_DETECTOR_ITEMS,
                ItemId = IDProvider.METAL_DETECTOR_ITEMS,
                AvailableStock = 1,
                IsRecipe = false,
                MaxItems = 10,
                AvoidRepeat = true,
            };
            cartShopData.Items.Add(metalDetector);
        }

        private void AddChecks(ShopData cartShopData)
        {
            var metalDetector = new ShopItemData()
            {
                Id = IDProvider.TRAVELING_CART_DAILY_CHECK,
                ItemId = IDProvider.TRAVELING_CART_DAILY_CHECK,
                AvailableStock = 1,
                IsRecipe = false,
                MaxItems = 1,
            };
            cartShopData.Items.Add(metalDetector);
        }

        private void RemoveWeddingRingCondition(ShopData cartShopData)
        {
            for (var i = 0; i < cartShopData.Items.Count; i++)
            {
                var item = cartShopData.Items[i];
                if (item.Condition == null)
                {
                    continue;
                }

                item.Condition = GameStateConditionProvider.RemoveCondition(item.Condition, "IS_MULTIPLAYER");
            }
        }
    }
}
