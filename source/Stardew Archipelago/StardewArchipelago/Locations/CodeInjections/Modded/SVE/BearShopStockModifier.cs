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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations.ShopStockModifiers;
using StardewArchipelago.Stardew;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.CodeInjections.Modded.SVE
{
    public class BearShopStockModifier: BarterShopStockModifier
    {
        public BearShopStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _stardewItemManager = stardewItemManager;
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
                    var bearShop = shopsData["FlashShifter.StardewValleyExpandedCP_BearVendor"];
                    MakeBearBarter(bearShop);
                },
                AssetEditPriority.Late
            );
        }

        private void MakeBearBarter(ShopData shopData)
        {
            var berryItems = _stardewItemManager.GetObjectsWithPhrase("berry").ToList();
            var discount = BearDiscount();
            var stockCount = BearStockCount();
            var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + shopData.GetHashCode());
            var chosenItemGroup = berryItems.Where(x => !(x.Name.Contains("Joja") || x.Name.Contains("Seeds")) && x.SellPrice > 0 ).ToList();
            foreach (var shopItem in shopData.Items)
            {
                var isRecipe = shopItem.ItemId.Contains("Baked Berry Oatmeal") || shopItem.ItemId.Contains("Flower Cookie");
                if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Purchases) || !isRecipe)
                {
                    ReplaceCurrencyWithBarterGivenObjects(berryItems, shopItem, stockCount, discount);
                    continue;
                }
                ReplaceCurrencyWithBarterGivenObject(BerryIfChefsanityIsOn(), shopItem, stockCount, discount);

            }
        }

        private StardewObject BerryIfChefsanityIsOn()
        {
            if (Game1.season == Season.Spring)
            {
                return _stardewItemManager.GetObjectById(ObjectIds.SALMONBERRY);
            }
            if (Game1.season == Season.Summer)
            {
                return _stardewItemManager.GetObjectById(ObjectIds.SPICE_BERRY);
            }
            if (Game1.season == Season.Fall)
            {
                return _stardewItemManager.GetObjectById(ObjectIds.BLACKBERRY);
            }
            return _stardewItemManager.GetObjectById(ObjectIds.CRYSTAL_FRUIT);
        }

        private double BearDiscount()
        {
            var hasKnowledge = _archipelago.HasReceivedItem("Bear Knowledge");
            var knowledgeBuff = hasKnowledge ? 0.3f: 0f;
            var applesHearts = 0;
            if (Game1.player.friendshipData.ContainsKey("Apples"))
            {
                applesHearts = Game1.player.friendshipData["Apples"].Points / 250; // Get discount from being friends with Apples
            }
            return 1 - (knowledgeBuff + applesHearts * 0.05f);
        }

        private int BearStockCount()
        {
            var hasKnowledge = _archipelago.HasReceivedItem("Bear Knowledge");
            var knowledgeBuff = hasKnowledge ? 3: 1;
            var applesHearts = 0;
            if (Game1.player.friendshipData.ContainsKey("Apples"))
            {
                applesHearts = Game1.player.friendshipData["Apples"].Points / 250; // Get discount from being friends with Apples
            }
            return Math.Max(1, Math.Min(30, knowledgeBuff * applesHearts));
        }

    }
}