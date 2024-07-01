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
using Force.DeepCloner;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Locations.ShopStockModifiers;
using StardewArchipelago.Stardew;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Shops;

namespace StardewArchipelago.Locations.CodeInjections.Modded.SVE
{
    public class TemperedShopStockModifier : ShopStockModifier
    {
        private static new ArchipelagoClient _archipelago;
        private static new StardewItemManager _stardewItemManager;
        private static readonly string[] _shopsWithTemperedWeapons = new []{"FlashShifter.StardewValleyExpandedCP_AlesiaVendor", "FlashShifter.StardewValleyExpandedCP_IsaacVendor"};
        public TemperedShopStockModifier(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager stardewItemManager) : base(monitor, helper, archipelago, stardewItemManager)
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
                    ReplaceShopsWithTemperedWeapons(shopsData);
                },
                AssetEditPriority.Late
            );
        }

        private void ReplaceShopsWithTemperedWeapons(IDictionary<string, ShopData> shops)
        {
            const string daggerLocationName = "Tempered Galaxy Dagger";
            const string swordLocationName = "Tempered Galaxy Sword";
            const string hammerLocationName = "Tempered Galaxy Hammer";
            var weaponIdsToLocations = new Dictionary<string, string>(){
                {ModItemIds.TEMPERED_DAGGER, daggerLocationName},
                {ModItemIds.TEMPERED_SWORD, swordLocationName},
                {ModItemIds.TEMPERED_HAMMER, hammerLocationName},
            };
            foreach (var shopName in _shopsWithTemperedWeapons)
            {
                var shopData = shops[shopName];
                for (var i = shopData.Items.Count - 1; i >=0; i--)
                {
                    var item = shopData.Items[i];
                    if (weaponIdsToLocations.ContainsKey(item.Id))
                    {
                        var apShopItem = CreateArchipelagoLocation(item, weaponIdsToLocations[item.Id]);
                        shopData.Items.RemoveAt(i);
                        shopData.Items.Insert(i, apShopItem);
                    }
                }
            }
        }

        public int[] ExchangeRate(int soldItemValue, int requestedItemValue)
        {
            if (IsOnePriceAMultipleOfOther(soldItemValue, requestedItemValue, out var exchangeRate))
            {
                return exchangeRate;
            }
            var greatestCommonDivisor = GreatestCommonDivisor(soldItemValue, requestedItemValue);
            var leastCommonMultiple = soldItemValue * requestedItemValue / greatestCommonDivisor;
            var soldItemCount = leastCommonMultiple / soldItemValue;
            var requestedItemCount = leastCommonMultiple / requestedItemValue;
            var lowestCount = 5; // This is for us to change if we want to move this value around easily in testing
            var finalCounts = MakeMinimalCountBelowGivenCount(soldItemCount, requestedItemCount, lowestCount);
            return finalCounts;
        }

        private bool IsOnePriceAMultipleOfOther(int soldItemValue, int requestedItemValue, out int[] exchangeRate)
        {
            exchangeRate = null;
            if (soldItemValue > requestedItemValue && soldItemValue % requestedItemValue == 0)
            {
                exchangeRate = new int[2] { 1, soldItemValue / requestedItemValue };
                return true;
            }
            if (soldItemValue <= requestedItemValue && requestedItemValue % soldItemValue == 0)
            {
                exchangeRate = new int[2] { requestedItemValue / soldItemValue, 1 };
                return true;
            }

            return false;
        }

        private int[] MakeMinimalCountBelowGivenCount(int soldItemCount, int requestedItemCount, int givenCount)
        {
            if (Math.Min(soldItemCount, requestedItemCount) > givenCount)
            {
                var closestCount = (int)Math.Pow(givenCount, (int)(Math.Log10(Math.Min(soldItemCount, requestedItemCount)) / Math.Log10(givenCount)));
                soldItemCount /= closestCount;
                requestedItemCount /= closestCount;
                var greatestCommonDivisor = GreatestCommonDivisor(soldItemCount, requestedItemCount); // Due to the rounding we may find the two aren't relatively prime anymore
                soldItemCount /= greatestCommonDivisor;
                requestedItemCount /= greatestCommonDivisor;
            }
            return new int[2] { soldItemCount, requestedItemCount };
        }

        private static int GreatestCommonDivisor(int firstValue, int secondValue) //Seemingly no basic method outside of BigInteger?
        {
            var largestValue = Math.Max(firstValue, secondValue);
            var lowestValue = Math.Min(firstValue, secondValue);
            var remainder = largestValue % lowestValue;
            if (remainder == 0)
            {
                return lowestValue;
            }
            while (remainder != 0)
            {
                largestValue = lowestValue;
                lowestValue = remainder;
                if (largestValue % lowestValue == 0)
                    break;
                remainder = largestValue % lowestValue;
            }
            return remainder;
        }
    }
}

