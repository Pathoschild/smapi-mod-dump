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

namespace StardewArchipelago.Locations.Festival
{
    public static class FlowerDanceInjections
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

        // public void setUpFestivalMainEvent()
        public static void SetUpFestivalMainEvent_FlowerDance_Postfix(Event __instance)
        {
            try
            {
                if (!__instance.isSpecificFestival("spring24"))
                {
                    return;
                }

                if (Game1.player.dancePartner.Value == null)
                {
                    return;
                }

                _locationChecker.AddCheckedLocation(FestivalLocationNames.FLOWER_DANCE);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUpFestivalMainEvent_FlowerDance_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public ShopMenu(Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        public static bool ShopMenu_HandleRarecrow5_Prefix(ShopMenu __instance, ref Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        {
            try
            {
                foreach (var salableItem in itemPriceAndStock.Keys.ToArray())
                {
                    _shopReplacer.ReplaceShopItem(itemPriceAndStock, salableItem, FestivalLocationNames.RARECROW_5, item => _shopReplacer.IsRarecrow(item, 5));
                }
                return true; //  run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShopMenu_HandleRarecrow5_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
