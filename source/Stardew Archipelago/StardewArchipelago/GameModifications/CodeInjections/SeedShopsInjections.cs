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
using StardewArchipelago.Locations;
using StardewArchipelago.Locations.Festival;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class SeedShopsInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ShopStockGenerator _shopStockGenerator;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, ShopStockGenerator shopStockGenerator)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _shopStockGenerator = shopStockGenerator;
        }

        public static bool OpenShopMenu_PierreAndSandyPersistentEvent_Prefix(GameLocation __instance, string which, ref bool __result)
        {
            try
            {
                if (which.Equals("Fish"))
                {
                    return true; // run original logic
                }

                if ((__instance is SeedShop seedShop))
                {
                    var pierre = __instance.getCharacterFromName("Pierre");
                    if (pierre != null && pierre.getTileLocation().Equals(new Vector2(4f, 17f)) && Game1.player.getTileY() > pierre.getTileY())
                    {
                        ActivatePierreShopMenu(seedShop);
                        __result = true;
                        return false; // don't run original logic
                    }

                    if (pierre == null && Game1.IsVisitingIslandToday("Pierre"))
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
                        Game1.afterDialogues = () => ActivatePierreShopMenu(seedShop);
                        __result = true;
                        return false; // don't run original logic
                    }

                    return true; // run original logic
                }

                if (__instance.Name.Equals("SandyHouse"))
                {
                    var sandy = __instance.getCharacterFromName("Sandy");
                    if (sandy != null && sandy.currentLocation == __instance)
                    {
                        var stock = _shopStockGenerator.GetSandyLimitedStock(__instance);
                        var onSandyShopPurchaseMethod = _modHelper.Reflection.GetMethod(__instance, "onSandyShopPurchase");
                        Func<ISalable, Farmer, int, bool> onSandyShopPurchase = (item, farmer, amount) => onSandyShopPurchaseMethod.Invoke<bool>(item, farmer, amount);
                        Game1.activeClickableMenu = new ShopMenu(stock, who: "Sandy", on_purchase: onSandyShopPurchase);
                    }

                    __result = true;
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(OpenShopMenu_PierreAndSandyPersistentEvent_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void ActivatePierreShopMenu(SeedShop seedShop)
        {
            Game1.activeClickableMenu = new ShopMenu(_shopStockGenerator.GetPierreShopStock(seedShop), who: "Pierre",
                on_purchase: _shopStockGenerator.PierrePersistentStock.OnPurchase);
        }


        public static bool GetJojaStock_FullCostco_Prefix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                __result = _shopStockGenerator.GetJojaStock();
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetJojaStock_FullCostco_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static ShopMenu _lastShopMenuUpdated = null;
        // public override void update(GameTime time)
        public static void Update_SeedShuffleFirstTimeOnly_Postfix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (_lastShopMenuUpdated == __instance)
                {
                    return;
                }

                _lastShopMenuUpdated = __instance;
                DisableSeedsIfNeeded(__instance);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_SeedShuffleFirstTimeOnly_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void DisableSeedsIfNeeded(ShopMenu __instance)
        {
            if (_archipelago.SlotData.Cropsanity != Cropsanity.Shuffled)
            {
                return;
            }

            foreach (var salableItem in __instance.itemPriceAndStock.Keys.ToArray())
            {
                if (salableItem is not Object salableObject || salableObject.Category != CATEGORY_SEEDS || salableObject.ParentSheetIndex == MIXED_SEEDS)
                {
                    continue;
                }

                if (!_archipelago.HasReceivedItem(salableObject.Name))
                {
                    __instance.itemPriceAndStock.Remove(salableItem);
                    __instance.forSale.Remove(salableItem);
                }

                if (salableObject.ParentSheetIndex != STRAWBERRY_SEEDS)
                {
                    continue;
                }

                if (_locationChecker.IsLocationMissingAndExists(FestivalLocationNames.STRAWBERRY_SEEDS))
                {
                    var myActiveHints = _archipelago.GetMyActiveHints();
                    var strawberrySeedsApItem =
                        new PurchaseableArchipelagoLocation(salableObject.Name, FestivalLocationNames.STRAWBERRY_SEEDS,
                            _modHelper, _locationChecker, _archipelago, myActiveHints);
                    __instance.itemPriceAndStock.Add(strawberrySeedsApItem, new[] { 1000, 1 });
                    __instance.forSale.Add(strawberrySeedsApItem);
                }
            }
        }

        private const int CATEGORY_SEEDS = -74;
        private const int STRAWBERRY_SEEDS = 745;
        private const int MIXED_SEEDS = 770;
    }
}
