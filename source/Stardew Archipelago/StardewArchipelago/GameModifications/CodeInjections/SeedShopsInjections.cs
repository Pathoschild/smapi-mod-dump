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
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.Festival;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Util;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class SeedShopsInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static PersistentStock _pierrePersistentStock;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _pierrePersistentStock = new PersistentStock();
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
                    if (pierre != null &&
                        pierre.getTileLocation().Equals(new Vector2(4f, 17f)) &&
                        Game1.player.getTileY() > pierre.getTileY())
                    {
                        Game1.activeClickableMenu = new ShopMenu(GetPierreShopStock(seedShop), who: "Pierre", on_purchase: _pierrePersistentStock.OnPurchase);
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
                        var stock = GetSandyLimitedStock(__instance);
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

        private static Dictionary<ISalable, int[]> GetPierreShopStock(SeedShop seedShop)
        {
            var stockAlreadyExists = _pierrePersistentStock.TryGetStockForToday(out var stock);
            if (!stockAlreadyExists)
            {
                stock = GeneratePierreStock();
                AddBuyBackToShop(seedShop, stock);
                _pierrePersistentStock.SetStockForToday(stock);
            }

            return stock;
        }

        private static Dictionary<ISalable, int[]> GeneratePierreStock()
        {
            var stock = new Dictionary<ISalable, int[]>();
            AddSeedsToPierreStock(stock);
            AddGrassStarterToPierreStock(stock);
            AddCookingIngredientsToPierreStock(stock);
            AddFertilizersToShop(stock);
            AddFurnitureToShop(stock);
            AddSaplingsToShop(stock);
            AddBouquetToShop(stock);
            return stock;
        }


        public static bool GetJojaStock_FullCostco_Prefix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {

                var jojaStock = new Dictionary<ISalable, int[]>();
                // AutoPetter has been removed
                AddToJojaStock(jojaStock, JOJA_COLA, false, 75, 6);
                AddJojaFurnitureToShop(jojaStock);
                AddSeedsToJojaStock(jojaStock);
                AddGrassStarterToJojaStock(jojaStock);
                AddCookingIngredientsToJojaStock(jojaStock);

                __result = jojaStock;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetJojaStock_FullCostco_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void AddSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddSpringSeedsToPierreStock(stock);
            AddSummerSeedsToPierreStock(stock);
            AddFallSeedsToPierreStock(stock);
        }

        private static void AddSpringSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, PARSNIP_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, BEAN_STARTER, true, itemSeason: "spring");
            AddToPierreStock(stock, CAULIFLOWER_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, POTATO_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, TULIP_BULB, true, itemSeason: "spring");
            AddToPierreStock(stock, KALE_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, JAZZ_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, GARLIC_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, RICE_SHOOT, true, itemSeason: "spring");
        }

        private static void AddSummerSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, MELON_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, TOMATO_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, BLUEBERRY_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, PEPPER_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, WHEAT_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, RADISH_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, POPPY_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, SPANGLE_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, HOPS_STARTER, true, itemSeason: "summer");
            AddToPierreStock(stock, CORN_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, SUNFLOWER_SEEDS, true, 100, "summer");
            AddToPierreStock(stock, RED_CABBAGE_SEEDS, true, itemSeason: "summer");
        }

        private static void AddFallSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, PUMPKIN_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, CORN_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, EGGPLANT_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, BOK_CHOY_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, YAM_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, CRANBERRY_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, WHEAT_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, SUNFLOWER_SEEDS, true, 100, "fall");
            AddToPierreStock(stock, FAIRY_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, AMARANTH_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, GRAPE_STARTER, true, itemSeason: "fall");
            AddToPierreStock(stock, ARTICHOKE_SEEDS, true, itemSeason: "fall");
        }

        private static void AddGrassStarterToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, GRASS_STARTER);
            if (!Game1.player.craftingRecipes.ContainsKey("Grass Starter"))
            {
                stock.Add(new StardewValley.Object(GRASS_STARTER, 1, true), new int[2]
                {
                    1000,
                    1
                });
            }
        }

        private static void AddCookingIngredientsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, SUGAR);
            AddToPierreStock(stock, WHEAT_FLOUR);
            AddToPierreStock(stock, RICE);
            AddToPierreStock(stock, OIL);
            AddToPierreStock(stock, VINEGAR);
        }

        private static void AddFertilizersToShop(Dictionary<ISalable, int[]> stock)
        {
            if ((int)Game1.stats.DaysPlayed < 15)
            {
                return;
            }

            AddToPierreStock(stock, BASIC_FERTILIZER, false, 50);
            AddToPierreStock(stock, BASIC_RETAINING_SOIL, false, 50);
            AddToPierreStock(stock, SPEED_GRO, false, 50);

            if (Game1.year <= 1)
            {
                return;
            }

            AddToPierreStock(stock, QUALITY_FERTILIZER, false, 75);
            AddToPierreStock(stock, QUALITY_RETAINING_SOIL, false, 75);
            AddToPierreStock(stock, DELUXE_SPEED_GRO, false, 75);
        }

        private static void AddFurnitureToShop(Dictionary<ISalable, int[]> stock)
        {
            var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
            var which = random.Next(112);
            if (which == 21)
            {
                which = 36;
            }

            var key1 = new Wallpaper(which);
            stock.Add(key1, new int[2]
            {
                key1.salePrice(),
                int.MaxValue
            });
            var key2 = new Wallpaper(random.Next(56), true);
            stock.Add(key2, new int[2]
            {
                key2.salePrice(),
                int.MaxValue
            });
            var key3 = new Furniture(1308, Vector2.Zero);
            stock.Add(key3, new int[2]
            {
                key3.salePrice(),
                int.MaxValue
            });
        }

        private static void AddSaplingsToShop(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, CHERRY_SAPLING, false, 1700, howManyInStock: 1);
            AddToPierreStock(stock, APRICOT_SAPLING, false, 1000, howManyInStock: 1);
            AddToPierreStock(stock, ORANGE_SAPLING, false, 2000, howManyInStock: 1);
            AddToPierreStock(stock, PEACH_SAPLING, false, 3000, howManyInStock: 1);
            AddToPierreStock(stock, POMEGRANATE_SAPLING, false, 3000, howManyInStock: 1);
            AddToPierreStock(stock, APPLE_SAPLING, false, 2000, howManyInStock: 1);
        }

        private static void AddBuyBackToShop(SeedShop __instance, Dictionary<ISalable, int[]> stock)
        {
            foreach (var itemToBuyBack in __instance.itemsFromPlayerToSell)
            {
                if (itemToBuyBack.Stack <= 0)
                {
                    continue;
                }
                var buyBackPrice = itemToBuyBack.salePrice();
                if (itemToBuyBack is StardewValley.Object objectToBuyBack)
                {
                    buyBackPrice = objectToBuyBack.sellToStorePrice();
                }

                stock.Add(itemToBuyBack, new int[2]
                {
                    buyBackPrice,
                    itemToBuyBack.Stack
                });
            }
        }

        private static void AddBouquetToShop(Dictionary<ISalable, int[]> stock)
        {
            if (Game1.player.hasAFriendWithHeartLevel(8, true))
            {
                AddToPierreStock(stock, BOUQUET, howManyInStock: 1);
            }
        }

        private static void AddToPierreStock(
            Dictionary<ISalable, int[]> stock,
            int itemId,
            bool isSeed = false,
            int basePrice = -1,
            string itemSeason = null,
            int howManyInStock = -1)
        {
            var priceMultiplier = 2.0;
            var item = new StardewValley.Object(Vector2.Zero, itemId, 1);

            if (basePrice == -1)
            {
                basePrice = item.salePrice();
                priceMultiplier = 1f;
            }
            else if (item.isSapling())
            {
                priceMultiplier *= Game1.MasterPlayer.difficultyModifier;
            }

            if (itemSeason != null && itemSeason != Game1.currentSeason)
            {
                if (!Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist"))
                {
                    return;
                }

                priceMultiplier *= 1.5f;
            }
            var price = (int)(basePrice * priceMultiplier);
            if (itemSeason != null)
            {
                foreach (var (itemAlreadyInStock, saleDetails) in stock)
                {
                    if (itemAlreadyInStock is not StardewValley.Object objectInStock) continue;
                    if (!Utility.IsNormalObjectAtParentSheetIndex(objectInStock, itemId)) continue;
                    if (saleDetails.Length == 0 || price >= saleDetails[0])
                    {
                        return;
                    }

                    saleDetails[0] = price;
                    stock[objectInStock] = saleDetails;
                    return;
                }
            }

            if (howManyInStock == -1)
            {
                var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + itemId);
                howManyInStock = random.Next(20);
                if (howManyInStock < 5)
                {
                    return;
                }
            }

            item.Stack = howManyInStock;
            stock.Add(item, new int[2]
            {
                price,
                howManyInStock
            });
        }

        private static void AddSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddSpringSeedsToJojaStock(stock);
            AddSummerSeedsToJojaStock(stock);
            AddFallSeedsToJojaStock(stock);
            AddSpecialSeedsToJojaStock(stock);
        }

        private static void AddSpringSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, PARSNIP_SEEDS, true);
            AddToJojaStock(stock, BEAN_STARTER, true);
            AddToJojaStock(stock, CAULIFLOWER_SEEDS, true);
            AddToJojaStock(stock, POTATO_SEEDS, true);
            AddToJojaStock(stock, TULIP_BULB, true);
            AddToJojaStock(stock, KALE_SEEDS, true);
            AddToJojaStock(stock, JAZZ_SEEDS, true);
            AddToJojaStock(stock, GARLIC_SEEDS, true);
            AddToJojaStock(stock, RICE_SHOOT, true);
        }

        private static void AddSummerSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, MELON_SEEDS, true);
            AddToJojaStock(stock, TOMATO_SEEDS, true);
            AddToJojaStock(stock, BLUEBERRY_SEEDS, true);
            AddToJojaStock(stock, PEPPER_SEEDS, true);
            AddToJojaStock(stock, WHEAT_SEEDS, true);
            AddToJojaStock(stock, RADISH_SEEDS, true);
            AddToJojaStock(stock, POPPY_SEEDS, true);
            AddToJojaStock(stock, SPANGLE_SEEDS, true);
            AddToJojaStock(stock, HOPS_STARTER, true);
            AddToJojaStock(stock, CORN_SEEDS, true);
            AddToJojaStock(stock, SUNFLOWER_SEEDS, true);
            AddToJojaStock(stock, RED_CABBAGE_SEEDS, true);
        }

        private static void AddFallSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, PUMPKIN_SEEDS, true);
            AddToJojaStock(stock, EGGPLANT_SEEDS, true);
            AddToJojaStock(stock, BOK_CHOY_SEEDS, true);
            AddToJojaStock(stock, YAM_SEEDS, true);
            AddToJojaStock(stock, CRANBERRY_SEEDS, true);
            AddToJojaStock(stock, FAIRY_SEEDS, true);
            AddToJojaStock(stock, AMARANTH_SEEDS, true);
            AddToJojaStock(stock, GRAPE_STARTER, true);
            AddToJojaStock(stock, ARTICHOKE_SEEDS, true);
        }

        private static void AddSpecialSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            if (Game1.player.friendshipData.ContainsKey("Sandy"))
            {
                AddToJojaStock(stock, RHUBARB_SEEDS);
                AddToJojaStock(stock, STARFRUIT_SEEDS);
                AddToJojaStock(stock, BEET_SEEDS);
            }

            if (TravelingMerchantInjections.HasAnyTravelingMerchantDay())
            {
                AddToJojaStock(stock, RARE_SEED, basePrice:800, packSize: 10);
            }
        }

        private static void AddGrassStarterToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, GRASS_STARTER);
        }

        private static void AddCookingIngredientsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, SUGAR, packSize: 20);
            AddToJojaStock(stock, WHEAT_FLOUR, packSize: 20);
            AddToJojaStock(stock, RICE, packSize: 20);
            AddToJojaStock(stock, OIL, packSize: 20);
            AddToJojaStock(stock, VINEGAR, packSize: 20);
        }

        private static void AddToJojaStock(
            Dictionary<ISalable, int[]> stock,
            int itemId,
            bool isSeed = false,
            int basePrice = -1,
            int packSize = 50)
        {
            var priceMultiplier = 1.0;
            var item = new StardewValley.Object(Vector2.Zero, itemId, packSize);

            if (basePrice == -1)
            {
                basePrice = item.salePrice();
                priceMultiplier = 0.80;
            }
            else if (item.isSapling())
            {
                priceMultiplier *= Game1.MasterPlayer.difficultyModifier;
            }

            var price = (int)(basePrice * priceMultiplier * packSize);
            stock.Add(item, new int[2]
            {
                price,
                int.MaxValue
            });
        }

        public static Dictionary<ISalable, int[]> GetSandyLimitedStock(GameLocation shop)
        {
            try
            {
                var sandyStock = new Dictionary<ISalable, int[]>();
                AddSeedToSandyStock(sandyStock, CACTUS_SEEDS, (int)(75.0 * Game1.MasterPlayer.difficultyModifier));
                AddSeedToSandyStock(sandyStock, RHUBARB_SEEDS);
                AddSeedToSandyStock(sandyStock, STARFRUIT_SEEDS);
                AddSeedToSandyStock(sandyStock, BEET_SEEDS);
                Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
                AddSandyRotatingStock(sandyStock, random);
                AddSandyPermanentCosmetics(sandyStock, random);

                Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(
                    SynchronizedShopStock.SynchedShop.Sandy, sandyStock);

                return sandyStock;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetSandyLimitedStock)}:\n{ex}", LogLevel.Error);
                return null;
            }
        }

        private static void AddSandyRotatingStock(Dictionary<ISalable, int[]> sandyStock, Random random)
        {
            switch (Game1.dayOfMonth % 7)
            {
                case 0:
                    Utility.AddStock(sandyStock, new Object(233, int.MaxValue));
                    AddToSandyStock(sandyStock, new Furniture(2720, Vector2.Zero), 3000);
                    break;
                case 1:
                    Utility.AddStock(sandyStock, new Object(88, 1), 200, 10);
                    AddToSandyStock(sandyStock, new Furniture(2802, Vector2.Zero), 2000);
                    break;
                case 2:
                    Utility.AddStock(sandyStock, new Object(90, int.MaxValue));
                    AddToSandyStock(sandyStock, new Furniture(2734 + random.Next(4) * 2, Vector2.Zero), 500);
                    break;
                case 3:
                    Utility.AddStock(sandyStock, new Object(749, 1), 500, 3);
                    AddToSandyStock(sandyStock, new Furniture(2584, Vector2.Zero), 5000);
                    break;
                case 4:
                    Utility.AddStock(sandyStock, new Object(466, int.MaxValue));
                    AddToSandyStock(sandyStock, new Furniture(2794, Vector2.Zero), 2500);
                    break;
                case 5:
                    Utility.AddStock(sandyStock, new Object(340, int.MaxValue));
                    AddToSandyStock(sandyStock, new Furniture(2784, Vector2.Zero), 2500);
                    break;
                case 6:
                    Utility.AddStock(sandyStock, new Object(371, int.MaxValue), 100);
                    AddToSandyStock(sandyStock, new Furniture(2748, Vector2.Zero), 500);
                    AddToSandyStock(sandyStock, new Furniture(2812, Vector2.Zero), 500);
                    break;
            }
        }

        private static void AddSandyPermanentCosmetics(Dictionary<ISalable, int[]> sandyStock, Random random)
        {
            Object seasonalPlant = new Object(Vector2.Zero, SEASONAL_PLANT);
            seasonalPlant.Stack = int.MaxValue;
            Utility.AddStock(sandyStock, seasonalPlant);
            AddToSandyStock(sandyStock, new Clothing(1000 + random.Next(sbyte.MaxValue)), 1000);
            AddToSandyStock(sandyStock, new Furniture(WALL_CACTUS, Vector2.Zero), 700);
        }

        private static void AddSeedToSandyStock(Dictionary<ISalable, int[]> sandyStock, int itemId, int price = -1)
        {
            var sendingPlayerName = "";
            var item = new Object(itemId, int.MaxValue);

            var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + itemId);
            var howManyInStock = random.Next(20);
            if (howManyInStock < 5)
            {
                return;
            }

            item.Stack = howManyInStock;
            Utility.AddStock(sandyStock, item, price, limitedQuantity:howManyInStock);
        }

        private static void AddToSandyStock(Dictionary<ISalable, int[]> sandyStock, ISalable item, int price, int stack = int.MaxValue)
        {
            sandyStock.Add(item, new[] { price, stack });
        }

        private static void AddJojaFurnitureToShop(Dictionary<ISalable, int[]> stock)
        {
            var key1 = new Wallpaper(21);
            key1.Stack = int.MaxValue;
            var numArray1 = new int[] { 20, int.MaxValue };
            stock.Add(key1, numArray1);
            var key2 = new Furniture(1609, Vector2.Zero);
            key2.Stack = int.MaxValue;
            var numArray2 = new int[] { 500, int.MaxValue };
            stock.Add(key2, numArray2);
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
                    var strawberrySeedsApItem =
                        new PurchaseableArchipelagoLocation(salableObject.Name, FestivalLocationNames.STRAWBERRY_SEEDS,
                            _modHelper, _locationChecker, _archipelago);
                    __instance.itemPriceAndStock.Add(strawberrySeedsApItem, new[] { 1000, 1 });
                    __instance.forSale.Add(strawberrySeedsApItem);
                }
            }
        }

        private const int CATEGORY_SEEDS = -74;
        private const int JOJA_COLA = 167;

        private const int SUGAR = 245;
        private const int WHEAT_FLOUR = 246;
        private const int OIL = 247;
        private const int RICE_SHOOT = 273;
        private const int GRASS_STARTER = 297;
        private const int AMARANTH_SEEDS = 299;
        private const int GRAPE_STARTER = 301;
        private const int HOPS_STARTER = 302;
        private const int RARE_SEED = 347;
        private const int VINEGAR = 419;
        private const int RICE = 423;
        private const int FAIRY_SEEDS = 425;
        private const int TULIP_BULB = 427;
        private const int JAZZ_SEEDS = 429;
        private const int SUNFLOWER_SEEDS = 431;
        private const int POPPY_SEEDS = 453;
        private const int SPANGLE_SEEDS = 455;
        private const int PARSNIP_SEEDS = 472;
        private const int BEAN_STARTER = 473;
        private const int CAULIFLOWER_SEEDS = 474;
        private const int POTATO_SEEDS = 475;
        private const int GARLIC_SEEDS = 476;
        private const int KALE_SEEDS = 477;
        private const int RHUBARB_SEEDS = 478;
        private const int MELON_SEEDS = 479;
        private const int TOMATO_SEEDS = 480;
        private const int BLUEBERRY_SEEDS = 481;
        private const int PEPPER_SEEDS = 482;
        private const int WHEAT_SEEDS = 483;
        private const int RADISH_SEEDS = 484;
        private const int RED_CABBAGE_SEEDS = 485;
        private const int STARFRUIT_SEEDS = 486;
        private const int CORN_SEEDS = 487;
        private const int EGGPLANT_SEEDS = 488;
        private const int ARTICHOKE_SEEDS = 489;
        private const int PUMPKIN_SEEDS = 490;
        private const int BOK_CHOY_SEEDS = 491;
        private const int YAM_SEEDS = 492;
        private const int CRANBERRY_SEEDS = 493;
        private const int BEET_SEEDS = 494;
        private const int SPRING_SEEDS = 495;
        private const int SUMMER_SEEDS = 496;
        private const int FALL_SEEDS = 497;
        private const int WINTER_SEEDS = 498;
        private const int ANCIENT_SEEDS = 499;
        private const int STRAWBERRY_SEEDS = 745;
        private const int MIXED_SEEDS = 770;
        private const int CACTUS_SEEDS = 802;
        private const int PINEAPPLE_SEEDS = 833;
        private const int FIBER_SEEDS = 885;

        private const int BASIC_FERTILIZER = 368;
        private const int QUALITY_FERTILIZER = 369;
        private const int DELUXE_FERTILIZER = 919;

        private const int BASIC_RETAINING_SOIL = 370;
        private const int QUALITY_RETAINING_SOIL = 371;
        private const int DELUXE_RETAINING_SOIL = 920;

        private const int SPEED_GRO = 465;
        private const int DELUXE_SPEED_GRO = 466;
        private const int HYPER_SPEED_GRO = 918;

        private const int BANANA_SAPLING = 69;
        private const int CHERRY_SAPLING = 628;
        private const int APRICOT_SAPLING = 629;
        private const int ORANGE_SAPLING = 630;
        private const int PEACH_SAPLING = 631;
        private const int POMEGRANATE_SAPLING = 632;
        private const int APPLE_SAPLING = 633;
        private const int MANGO_SAPLING = 835;

        private const int BOUQUET = 458;
        private const int SEASONAL_PLANT = 196;
        private const int WALL_CACTUS = 2655;
    }
}
