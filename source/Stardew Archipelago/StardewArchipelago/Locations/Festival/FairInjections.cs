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
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.Festival
{
    internal class FairInjections
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

        // public override void update(GameTime time)
        public static bool StrengthGameUpdate_StrongEnough_Prefix(StrengthGame __instance, GameTime time)
        {
            try
            {
                var changeSpeedField = _modHelper.Reflection.GetField<float>(__instance, "changeSpeed");
                var changeSpeed = changeSpeedField.GetValue();
                if (changeSpeed != 0.0)
                {
                    return true; // run original logic
                }

                var powerField = _modHelper.Reflection.GetField<float>(__instance, "power");
                var power = powerField.GetValue();
                if (power >= 99.0 || (power < 2.0 && _archipelago.SlotData.FestivalLocations != FestivalLocations.Hard))
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.STRENGTH_GAME);
                }
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(StrengthGameUpdate_StrongEnough_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void interpretGrangeResults()
        public static void InterpretGrangeResults_Success_Postfix(Event __instance)
        {
            try
            {
                var isEasyMode = _archipelago.SlotData.FestivalLocations != FestivalLocations.Hard;
                if (__instance.grangeScore >= 90 || ((__instance.grangeScore >= 60 || __instance.grangeScore == -666) && isEasyMode))
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.GRANGE_DISPLAY);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(InterpretGrangeResults_Success_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static ShopMenu _lastShopMenuUpdated = null;
        // public override void update(GameTime time)
        public static void Update_HandleFairItemsFirstTimeOnly_Postfix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (_lastShopMenuUpdated == __instance || __instance.currency != 1)
                {
                    return;
                }

                _lastShopMenuUpdated = __instance;
                var myActiveHints = _archipelago.GetMyActiveHints();
                foreach (var salableItem in __instance.itemPriceAndStock.Keys.ToArray())
                {
                    _shopReplacer.ReplaceShopItem(__instance.itemPriceAndStock, salableItem, FestivalLocationNames.RARECROW_1, item => _shopReplacer.IsRarecrow(item, 1), myActiveHints);
                    _shopReplacer.ReplaceShopItem(__instance.itemPriceAndStock, salableItem, FestivalLocationNames.FAIR_STARDROP, (Object item) => item.ParentSheetIndex == 434, myActiveHints);
                }

                __instance.forSale = __instance.itemPriceAndStock.Keys.ToList();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_HandleFairItemsFirstTimeOnly_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
