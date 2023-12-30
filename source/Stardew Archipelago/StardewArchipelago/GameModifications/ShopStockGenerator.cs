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
using StardewArchipelago.Constants;
using StardewArchipelago.Locations;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Util;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications
{
    public class ShopStockGenerator
    {
        private IMonitor _monitor;
        private IModHelper _modHelper;
        private ArchipelagoClient _archipelago;
        private LocationChecker _locationChecker;
        public PersistentStock PierrePersistentStock { get; }

        public ShopStockGenerator(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            PierrePersistentStock = new PersistentStock();
        }

        public Dictionary<ISalable, int[]> GetPierreShopStock(SeedShop seedShop)
        {
            var stockAlreadyExists = PierrePersistentStock.TryGetStockForToday(out var stock);
            if (!stockAlreadyExists)
            {
                stock = GeneratePierreStock();
                AddBuyBackToShop(seedShop, stock);
                PierrePersistentStock.SetStockForToday(stock);
            }

            return stock;
        }

        private Dictionary<ISalable, int[]> GeneratePierreStock()
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

        private static void AddSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddSpringSeedsToPierreStock(stock);
            AddSummerSeedsToPierreStock(stock);
            AddFallSeedsToPierreStock(stock);
        }

        private static void AddSpringSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, ShopItemIds.PARSNIP_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.BEAN_STARTER, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.CAULIFLOWER_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.POTATO_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.TULIP_BULB, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.KALE_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.JAZZ_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.GARLIC_SEEDS, true, itemSeason: "spring");
            AddToPierreStock(stock, ShopItemIds.RICE_SHOOT, true, itemSeason: "spring");
        }

        private static void AddSummerSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, ShopItemIds.MELON_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.TOMATO_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.BLUEBERRY_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.PEPPER_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.WHEAT_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.RADISH_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.POPPY_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.SPANGLE_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.HOPS_STARTER, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.CORN_SEEDS, true, itemSeason: "summer");
            AddToPierreStock(stock, ShopItemIds.SUNFLOWER_SEEDS, true, 100, "summer");
            AddToPierreStock(stock, ShopItemIds.RED_CABBAGE_SEEDS, true, itemSeason: "summer");
        }

        private static void AddFallSeedsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, ShopItemIds.PUMPKIN_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.CORN_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.EGGPLANT_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.BOK_CHOY_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.YAM_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.CRANBERRY_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.WHEAT_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.SUNFLOWER_SEEDS, true, 100, "fall");
            AddToPierreStock(stock, ShopItemIds.FAIRY_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.AMARANTH_SEEDS, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.GRAPE_STARTER, true, itemSeason: "fall");
            AddToPierreStock(stock, ShopItemIds.ARTICHOKE_SEEDS, true, itemSeason: "fall");
        }

        private void AddGrassStarterToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, ShopItemIds.GRASS_STARTER);

            ISalable grassStarterRecipe;
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                if (Game1.player.craftingRecipes.ContainsKey("Grass Starter"))
                {
                    return;
                }

                grassStarterRecipe = new Object(ShopItemIds.GRASS_STARTER, 1, true);
            }
            else
            {
                var location = "Grass Starter Recipe";
                if (!_locationChecker.IsLocationMissingAndExists(location))
                {
                    return;
                }

                var activeHints = _archipelago.GetMyActiveHints();
                grassStarterRecipe = new PurchaseableArchipelagoLocation(location, location, _modHelper, _locationChecker, _archipelago, activeHints);
            }

            stock.Add(grassStarterRecipe, new int[2]
            {
                1000,
                1,
            });
        }

        private static void AddCookingIngredientsToPierreStock(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, ShopItemIds.SUGAR);
            AddToPierreStock(stock, ShopItemIds.WHEAT_FLOUR);
            AddToPierreStock(stock, ShopItemIds.RICE);
            AddToPierreStock(stock, ShopItemIds.OIL);
            AddToPierreStock(stock, ShopItemIds.VINEGAR);
        }

        private static void AddFertilizersToShop(Dictionary<ISalable, int[]> stock)
        {
            if ((int)Game1.stats.DaysPlayed < 15)
            {
                return;
            }

            AddToPierreStock(stock, ShopItemIds.BASIC_FERTILIZER, false, 50);
            AddToPierreStock(stock, ShopItemIds.BASIC_RETAINING_SOIL, false, 50);
            AddToPierreStock(stock, ShopItemIds.SPEED_GRO, false, 50);

            if (Game1.year <= 1)
            {
                return;
            }

            AddToPierreStock(stock, ShopItemIds.QUALITY_FERTILIZER, false, 75);
            AddToPierreStock(stock, ShopItemIds.QUALITY_RETAINING_SOIL, false, 75);
            AddToPierreStock(stock, ShopItemIds.DELUXE_SPEED_GRO, false, 75);
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
                int.MaxValue,
            });
            var key2 = new Wallpaper(random.Next(56), true);
            stock.Add(key2, new int[2]
            {
                key2.salePrice(),
                int.MaxValue,
            });
            var key3 = new Furniture(1308, Vector2.Zero);
            stock.Add(key3, new int[2]
            {
                key3.salePrice(),
                int.MaxValue,
            });
        }

        private static void AddSaplingsToShop(Dictionary<ISalable, int[]> stock)
        {
            AddToPierreStock(stock, ShopItemIds.CHERRY_SAPLING, false, 1700, howManyInStock: 1);
            AddToPierreStock(stock, ShopItemIds.APRICOT_SAPLING, false, 1000, howManyInStock: 1);
            AddToPierreStock(stock, ShopItemIds.ORANGE_SAPLING, false, 2000, howManyInStock: 1);
            AddToPierreStock(stock, ShopItemIds.PEACH_SAPLING, false, 3000, howManyInStock: 1);
            AddToPierreStock(stock, ShopItemIds.POMEGRANATE_SAPLING, false, 3000, howManyInStock: 1);
            AddToPierreStock(stock, ShopItemIds.APPLE_SAPLING, false, 2000, howManyInStock: 1);
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
                    itemToBuyBack.Stack,
                });
            }
        }

        private static void AddBouquetToShop(Dictionary<ISalable, int[]> stock)
        {
            if (Game1.player.hasAFriendWithHeartLevel(8, true))
            {
                AddToPierreStock(stock, ShopItemIds.BOUQUET, howManyInStock: 1);
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
            var maxAmount = 20;

            if (basePrice == -1)
            {
                basePrice = item.salePrice();
                priceMultiplier = 1f;
            }
            else if (item.isSapling())
            {
                priceMultiplier *= Game1.MasterPlayer.difficultyModifier;
            }

            var hasStocklist = Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist");
            if (itemSeason != null && itemSeason != Game1.currentSeason)
            {
                if (!hasStocklist)
                {
                    return;
                }

                priceMultiplier *= 1.5f;
            }

            if (hasStocklist)
            {
                maxAmount *= 2;
            }

            if (Game1.player.hasCompletedCommunityCenter())
            {
                maxAmount *= 2;
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
                howManyInStock = random.Next(maxAmount);
                if (howManyInStock < 5)
                {
                    return;
                }
            }

            item.Stack = howManyInStock;
            stock.Add(item, new int[2]
            {
                price,
                howManyInStock,
            });
        }

        public Dictionary<ISalable, int[]> GetJojaStock()
        {
            var jojaStock = new Dictionary<ISalable, int[]>();
            // AutoPetter has been removed
            AddToJojaStock(jojaStock, ShopItemIds.JOJA_COLA, false, 75, 6);
            AddJojaFurnitureToShop(jojaStock);
            AddSeedsToJojaStock(jojaStock);
            AddGrassStarterToJojaStock(jojaStock);
            AddCookingIngredientsToJojaStock(jojaStock);
            return jojaStock;
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
            AddToJojaStock(stock, ShopItemIds.PARSNIP_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.BEAN_STARTER, true);
            AddToJojaStock(stock, ShopItemIds.CAULIFLOWER_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.POTATO_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.TULIP_BULB, true);
            AddToJojaStock(stock, ShopItemIds.KALE_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.JAZZ_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.GARLIC_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.RICE_SHOOT, true);
        }

        private static void AddSummerSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, ShopItemIds.MELON_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.TOMATO_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.BLUEBERRY_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.PEPPER_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.WHEAT_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.RADISH_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.POPPY_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.SPANGLE_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.HOPS_STARTER, true);
            AddToJojaStock(stock, ShopItemIds.CORN_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.SUNFLOWER_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.RED_CABBAGE_SEEDS, true);
        }

        private static void AddFallSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, ShopItemIds.PUMPKIN_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.EGGPLANT_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.BOK_CHOY_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.YAM_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.CRANBERRY_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.FAIRY_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.AMARANTH_SEEDS, true);
            AddToJojaStock(stock, ShopItemIds.GRAPE_STARTER, true);
            AddToJojaStock(stock, ShopItemIds.ARTICHOKE_SEEDS, true);
        }

        private static void AddSpecialSeedsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            if (Game1.player.friendshipData.ContainsKey("Sandy"))
            {
                AddToJojaStock(stock, ShopItemIds.RHUBARB_SEEDS);
                AddToJojaStock(stock, ShopItemIds.STARFRUIT_SEEDS);
                AddToJojaStock(stock, ShopItemIds.BEET_SEEDS);
            }

            if (TravelingMerchantInjections.HasAnyTravelingMerchantDay())
            {
                AddToJojaStock(stock, ShopItemIds.RARE_SEED, basePrice: 800, packSize: 10);
            }
        }

        private static void AddGrassStarterToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, ShopItemIds.GRASS_STARTER);
        }

        private static void AddCookingIngredientsToJojaStock(Dictionary<ISalable, int[]> stock)
        {
            AddToJojaStock(stock, ShopItemIds.SUGAR, packSize: 20);
            AddToJojaStock(stock, ShopItemIds.WHEAT_FLOUR, packSize: 20);
            AddToJojaStock(stock, ShopItemIds.RICE, packSize: 20);
            AddToJojaStock(stock, ShopItemIds.OIL, packSize: 20);
            AddToJojaStock(stock, ShopItemIds.VINEGAR, packSize: 20);
        }

        public static void AddToJojaStock(
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
                int.MaxValue,
            });
        }

        public Dictionary<ISalable, int[]> GetSandyLimitedStock(GameLocation shop)
        {
            try
            {
                var sandyStock = new Dictionary<ISalable, int[]>();
                AddSeedToSandyStock(sandyStock, ShopItemIds.CACTUS_SEEDS, (int)(75.0 * Game1.MasterPlayer.difficultyModifier));
                AddSeedToSandyStock(sandyStock, ShopItemIds.RHUBARB_SEEDS);
                AddSeedToSandyStock(sandyStock, ShopItemIds.STARFRUIT_SEEDS);
                AddSeedToSandyStock(sandyStock, ShopItemIds.BEET_SEEDS);
                AddSandyModdedStock(sandyStock);
                var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
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

        private void AddSandyModdedStock(Dictionary<ISalable, int[]> sandyStock)
        {
            AddSandyDistantLandsStock(sandyStock);
        }

        private void AddSandyDistantLandsStock(Dictionary<ISalable, int[]> sandyStock)
        {
            if (!_archipelago.SlotData.Mods.HasMod(ModNames.DISTANT_LANDS))
            {
                return;
            }

            var voidMintIdentifier = "1 2 2 3 2/spring summer fall/109/"; //Done as modded seeds have variable ID but use ID in dictionary
            var vileAncientIdentifier = "2 7 7 7 5/spring summer fall/108/";
            var cropList = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            var voidMintSeeds = cropList.FirstOrDefault(x => x.Value.Contains(voidMintIdentifier)).Key;
            var vileAncientFruitSeeds = cropList.FirstOrDefault(x => x.Value.Contains(vileAncientIdentifier)).Key;
            AddSeedToSandyStock(sandyStock, voidMintSeeds);
            AddSeedToSandyStock(sandyStock, vileAncientFruitSeeds);
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
            var seasonalPlant = new Object(Vector2.Zero, ShopItemIds.SEASONAL_PLANT);
            seasonalPlant.Stack = int.MaxValue;
            Utility.AddStock(sandyStock, seasonalPlant);
            AddToSandyStock(sandyStock, new Clothing(1000 + random.Next(sbyte.MaxValue)), 1000);
            AddToSandyStock(sandyStock, new Furniture(ShopItemIds.WALL_CACTUS, Vector2.Zero), 700);
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
            Utility.AddStock(sandyStock, item, price, limitedQuantity: howManyInStock);
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
    }
}
