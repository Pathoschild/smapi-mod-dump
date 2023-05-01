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
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.Festival
{
    internal class SpiritEveInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ShopReplacer _shopReplacer;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, ShopReplacer shopReplacer)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _shopReplacer = shopReplacer;
        }

        public static bool CheckForAction_SpiritEveChest_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (justCheckingForActivity || __instance.giftbox.Value || __instance.playerChest.Value)
                {
                    return true; // run original logic
                }

                if (__instance.items.Count <= 0 || __instance.items.Count > 1 || __instance.items.First().ParentSheetIndex != 373)
                {
                    return true; // run original logic
                }

                who.currentLocation.playSound("openChest");
                if (__instance.synchronized.Value)
                    __instance.GetMutex().RequestLock(() => __instance.openChestEvent.Fire());
                else
                    __instance.performOpenChest();
                
                var obj = __instance.items[0];
                __instance.items[0] = null;
                __instance.items.RemoveAt(0);
                __result = true;

                _locationChecker.AddCheckedLocation(FestivalLocationNames.GOLDEN_PUMPKIN);

                return false; // don't run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_SpiritEveChest_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public ShopMenu(Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        public static bool ShopMenu_HandleRarecrow2_Prefix(ShopMenu __instance, ref Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        {
            try
            {
                foreach (var salableItem in itemPriceAndStock.Keys.ToArray())
                {
                    _shopReplacer.ReplaceShopItem(itemPriceAndStock, salableItem, FestivalLocationNames.RARECROW_2,
                        item => _shopReplacer.IsRarecrow(item, 2));
                }
                return true; //  run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShopMenu_HandleRarecrow2_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
