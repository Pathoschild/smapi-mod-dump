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

        private static ShopMenu _lastShopMenuUpdated = null;
        // public override void update(GameTime time)
        public static void Update_HandleSpiritEveShopFirstTimeOnly_Postfix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (_lastShopMenuUpdated == __instance || __instance.storeContext != "Temp" || !Game1.CurrentEvent.isSpecificFestival("fall27"))
                {
                    return;
                }

                _lastShopMenuUpdated = __instance;
                var myActiveHints = _archipelago.GetMyActiveHints();

                _shopReplacer.ReplaceShopItem(__instance.itemPriceAndStock, FestivalLocationNames.RARECROW_2, item => _shopReplacer.IsRarecrow(item, 2), myActiveHints);
                _shopReplacer.PlaceShopRecipeCheck(__instance.itemPriceAndStock, FestivalLocationNames.JACK_O_LANTERN_RECIPE, "Jack-O-Lantern", myActiveHints, new[] { 2000, 1 });

                __instance.forSale = __instance.itemPriceAndStock.Keys.ToList();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_HandleSpiritEveShopFirstTimeOnly_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
