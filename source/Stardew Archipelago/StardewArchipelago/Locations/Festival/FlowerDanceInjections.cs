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
using Microsoft.Xna.Framework;
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

        private static ShopMenu _lastShopMenuUpdated = null;
        // public override void update(GameTime time)
        public static void Update_HandleRarecrow5FirstTimeOnly_Postfix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (_lastShopMenuUpdated == __instance)
                {
                    return;
                }

                _lastShopMenuUpdated = __instance;
                foreach (var salableItem in __instance.itemPriceAndStock.Keys.ToArray())
                {
                    _shopReplacer.ReplaceShopItem(__instance.itemPriceAndStock, salableItem, FestivalLocationNames.RARECROW_5, item => _shopReplacer.IsRarecrow(item, 5));
                }

                __instance.forSale = __instance.itemPriceAndStock.Keys.ToList();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_HandleRarecrow5FirstTimeOnly_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
