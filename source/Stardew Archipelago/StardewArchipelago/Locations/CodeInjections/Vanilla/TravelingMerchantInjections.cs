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
using Archipelago.MultiClient.Net.Models;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.GameModifications;
using StardewArchipelago.Serialization;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public class TravelingMerchantInjections
    {
        private const double BASE_STOCK = 0.1;
        private const double STOCK_AMOUNT_PER_UPGRADE_FOR_EXCLUSIVE_ITEMS = 0.15;
        private const double STOCK_AMOUNT_PER_UPGRADE_FOR_RANDOM_ITEMS = 0.1;
        private const double STOCK_AMOUNT_PER_ONE_PERCENT_CHECKS_FOR_RANDOM_ITEMS = 0.01;
        private const double STOCK_AMOUNT_REDUCTION_PER_PURCHASE = 0.1;

        // 0.1 + (6 * 0.1) + (100 * 0.05)
        // 10% + 60% + 100% = 6

        private const double BASE_PRICE = 1.4;
        private const double DISCOUNT_PER_UPGRADE = 0.1;
        public const string AP_MERCHANT_DAYS = "Traveling Merchant: {0}"; // 7, One for each day
        private const string AP_MERCHANT_STOCK = "Traveling Merchant Stock Size"; // 10% base size, 6 upgrades of 15% each
        private const string AP_MERCHANT_DISCOUNT = "Traveling Merchant Discount"; // Base Price 140%, 8 x 10% discount
        private const string AP_MERCHANT_LOCATION = "Traveling Merchant {0} Item {1}";
        private const string AP_METAL_DETECTOR = "Traveling Merchant Metal Detector"; // Base Price 140%, 8 x 10% discount
        private const string AP_WEDDING_RING_RECIPE = "Wedding Ring Recipe";

        private static readonly string[] _exclusiveStock = new[]
            { "Rare Seed", "Rarecrow", "Coffee Bean", "Wedding Ring Recipe" };

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ArchipelagoStateDto _archipelagoState;
        private static PersistentStock _persistentStock;

        private static Dictionary<ISalable, string> _flairOverride;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, ArchipelagoStateDto archipelagoState)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _archipelagoState = archipelagoState;
            _persistentStock = new PersistentStock();
            _flairOverride = new Dictionary<ISalable, string>();
        }

        public static void DayUpdate_IsTravelingMerchantDay_Postfix(Forest __instance, int dayOfMonth)
        {
            try
            {
                UpdateTravelingMerchantForToday(__instance, dayOfMonth);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DayUpdate_IsTravelingMerchantDay_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        public static bool NightMarketCheckAction_IsTravelingMerchantDay_Prefix(BeachNightMarket __instance, Location tileLocation, Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                if (__instance.map.GetLayer("Buildings").Tiles[tileLocation] == null)
                {
                    return true; // run original logic
                }

                var tileIndex = __instance.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex;
                if (tileIndex != 399)
                {
                    return true; // run original logic
                }

                if (Game1.timeOfDay < 1700)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_Closed"));
                    return false; // don't run original logic
                }

                var isTravelingMerchantDay =
                    IsTravelingMerchantDay(Game1.dayOfMonth, out var sendingPlayer);

                if (!isTravelingMerchantDay)
                {
                    Game1.drawObjectDialogue("The traveling merchant isn't here today.");
                    return false; // don't run original logic
                }

                Game1.activeClickableMenu = new ShopMenu(Utility.getTravelingMerchantStock((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)), who: "TravelerNightMarket", on_purchase: Utility.onTravelingMerchantShopPurchase);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(NightMarketCheckAction_IsTravelingMerchantDay_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static void UpdateTravelingMerchantForToday(Forest __instance, int dayOfMonth)
        {
            if (IsTravelingMerchantDay(dayOfMonth, out _))
            {
                __instance.travelingMerchantDay = true;
                __instance.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1472, 640, 492, 116));
                __instance.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1652, 744, 76, 48));
                __instance.travelingMerchantBounds.Add(new Microsoft.Xna.Framework.Rectangle(1812, 744, 104, 48));
                foreach (var travelingMerchantBound in __instance.travelingMerchantBounds)
                {
                    Utility.clearObjectsInArea(travelingMerchantBound, __instance);
                }

                if (Game1.IsMasterGame && Game1.netWorldState.Value.VisitsUntilY1Guarantee >= 0 && (dayOfMonth % 7 % 5 != 0))
                {
                    --Game1.netWorldState.Value.VisitsUntilY1Guarantee;
                }
            }
            else
            {
                __instance.travelingMerchantBounds.Clear();
                __instance.travelingMerchantDay = false;
            }
        }

        public static void SetUpShopOwner_TravelingMerchantApFlair_Postfix(ShopMenu __instance, string who)
        {
            try
            {
                if (who != "Traveler" || !IsTravelingMerchantDay(Game1.dayOfMonth, out var playerName))
                {
                    return;
                }

                SetTravelingMerchantFlair(__instance, playerName);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUpShopOwner_TravelingMerchantApFlair_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void SetTravelingMerchantFlair(ShopMenu travelingMerchantShopMenu, string playerName = null)
        {
            if (travelingMerchantShopMenu == null)
            {
                return;
            }

            var currentStockSize = GetChanceForRandomStockItemToRemain(_archipelago.GetReceivedItemCount(AP_MERCHANT_STOCK));
            var day = Days.GetDayOfWeekName(Game1.dayOfMonth);
            var locationName = Game1.currentLocation is Forest ? "Cindersap Forest" : "the Beach Night Market";
            var text = _flairOverride.Any() ? _flairOverride.Values.First() : GetFlairForToday(playerName, locationName, day, currentStockSize);
            var prettyStockSize = (int)(currentStockSize * 100);
            text += $"{Environment.NewLine}Stock: {prettyStockSize}%";
            travelingMerchantShopMenu.potraitPersonDialogue = Game1.parseText(text, Game1.dialogueFont, 304);
        }

        private static string GetFlairForToday(string playerName, string locationName, string day, double currentStockSize)
        {
            if (currentStockSize < 0.2)
            {
                return $"I'm sorry I don't have much to offer. Maybe do something else in the meantime?";
            }

            if (currentStockSize < 0.95)
            {
                return playerName == null || locationName == null || day == null ? "I got lots of good stuff for sale!" : $"{playerName} recommended that I visit {locationName} on {day}s.";
            }

            return "Sweety, will you please buy something? I have a family to feed";
        }

        // public static Dictionary<ISalable, int[]> getTravelingMerchantStock(int seed)
        public static bool GetTravelingMerchantStock_APStock_Prefix(int seed, ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var stock = GetTravelingMerchantShopStock(seed);

                __result = stock;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetTravelingMerchantStock_APStock_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static Dictionary<ISalable, int[]> GetTravelingMerchantShopStock(int seed)
        {
            var stockAlreadyExists = _persistentStock.TryGetStockForToday(out var stock);
            if (stockAlreadyExists)
            {
                return stock;
            }

            var generateLocalTravelingMerchantStockMethod = _modHelper.Reflection.GetMethod(typeof(Utility), "generateLocalTravelingMerchantStock");
            stock = generateLocalTravelingMerchantStockMethod.Invoke<Dictionary<ISalable, int[]>>(seed);

            AddWeddingRingRecipeToStock(stock);

            var priceUpgrades = _archipelago.GetReceivedItemCount(AP_MERCHANT_DISCOUNT);
            var priceMultiplier = BASE_PRICE - (priceUpgrades * DISCOUNT_PER_UPGRADE);
            var random = new Random(seed);

            var itemsToRemove = ChooseItemsToRemove(stock, random);

            AddMetalDetectorItems(stock, seed);
            AdjustPrices(stock, priceMultiplier);
            AddApStock(stock, random, priceMultiplier);
            RemoveItemsFromStock(stock, itemsToRemove);
            _persistentStock.SetStockForToday(stock);

            return stock;
        }

        private static void AddWeddingRingRecipeToStock(Dictionary<ISalable, int[]> stock)
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                if (Game1.player.craftingRecipes.ContainsKey("Wedding Ring"))
                {
                    return;
                }

                var weddingRingRecipe = new Object(801, 1, true);
                stock.Add(weddingRingRecipe, new int[2]
                {
                    500,
                    1
                });
                return;
            }

            if (!_locationChecker.IsLocationMissing(AP_WEDDING_RING_RECIPE))
            {
                return;
            }
            var activeHints = _archipelago.GetMyActiveHints();
            var weddingRingRecipeCheck = new PurchaseableArchipelagoLocation(AP_WEDDING_RING_RECIPE, _monitor, _modHelper, _locationChecker, _archipelago, activeHints);
            stock.Add(weddingRingRecipeCheck, new int[2]
            {
                500,
                1
            });
        }

        private static List<ISalable> ChooseItemsToRemove(Dictionary<ISalable, int[]> stock, Random random)
        {
            var stockUpgrades = _archipelago.GetReceivedItemCount(AP_MERCHANT_STOCK);
            var chanceForExclusiveItemToRemain = GetChanceForExclusiveStockItemToRemain(stockUpgrades);
            var chanceForRandomItemToRemain = GetChanceForRandomStockItemToRemain(stockUpgrades);

            var itemsToRemove = new List<ISalable>();
            foreach (var item in stock.Keys.ToArray())
            {
                var itemIsExclusive = _exclusiveStock.Contains(item.Name);
                var randomRoll = random.NextDouble();
                if (itemIsExclusive)
                {
                    RemoveExclusiveItemBasedOnStockSize(randomRoll, chanceForExclusiveItemToRemain, itemsToRemove, item);
                }
                else
                {
                    HandleRandomItemBasedOnStockSize(stock, randomRoll, chanceForRandomItemToRemain, itemsToRemove, item);
                }
            }

            return itemsToRemove;
        }

        private static void RemoveExclusiveItemBasedOnStockSize(double randomRoll, double chanceForExclusiveItemToRemain,
            List<ISalable> itemsToRemove, ISalable item)
        {
            if (randomRoll > chanceForExclusiveItemToRemain)
            {
                itemsToRemove.Add(item);
            }
        }

        private static void HandleRandomItemBasedOnStockSize(Dictionary<ISalable, int[]> stock, double randomRoll,
            double chanceForRandomItemToRemain, List<ISalable> itemsToRemove, ISalable salable)
        {
            if (randomRoll > chanceForRandomItemToRemain)
            {
                itemsToRemove.Add(salable);
            }
            else
            {
                if (salable is not Item item)
                {
                    return;
                }

                var archipelagoItem = new TravelingMerchantItem(item, _archipelagoState);
                stock.Add(archipelagoItem, new []{ stock[salable][0], 1 });
                stock.Remove(salable);
            }
        }

        private static void AdjustPrices(Dictionary<ISalable, int[]> stock, double priceMultiplier)
        {
            foreach (var (item, prices) in stock)
            {
                prices[0] = ModifyPrice(prices[0], priceMultiplier);
            }
        }

        private static void RemoveItemsFromStock(Dictionary<ISalable, int[]> stock, List<ISalable> itemsToRemove)
        {
            foreach (var itemToRemove in itemsToRemove)
            {
                stock.Remove(itemToRemove);
                if (_flairOverride.ContainsKey(itemToRemove))
                {
                    _flairOverride.Remove(itemToRemove);
                }
            }
        }

        private static double GetChanceForExclusiveStockItemToRemain(int stockUpgrades)
        {
            return BASE_STOCK + (stockUpgrades * STOCK_AMOUNT_PER_UPGRADE_FOR_EXCLUSIVE_ITEMS);
        }

        private static double GetChanceForRandomStockItemToRemain(int stockUpgrades)
        {
            double checksCompleted = _archipelago.Session.Locations.AllLocationsChecked.Count;
            double totalChecks = _archipelago.Session.Locations.AllLocations.Count;
            var checksPercentComplete = (checksCompleted / totalChecks) * 100;
            var totalPurchases = _archipelagoState.TravelingMerchantPurchases;
            var stockFromApItems = stockUpgrades * STOCK_AMOUNT_PER_UPGRADE_FOR_RANDOM_ITEMS;
            var stockFromChecks = checksPercentComplete * STOCK_AMOUNT_PER_ONE_PERCENT_CHECKS_FOR_RANDOM_ITEMS;
            var stockReductionFromPurchases = totalPurchases * STOCK_AMOUNT_REDUCTION_PER_PURCHASE;

            return BASE_STOCK + stockFromApItems + stockFromChecks - stockReductionFromPurchases;
        }

        private static void AddMetalDetectorItems(Dictionary<ISalable, int[]> stock, int seed)
        {
            _flairOverride.Clear();
            var metalDetectorUpgrades = _archipelago.GetReceivedItemCount(AP_METAL_DETECTOR);
            if (metalDetectorUpgrades < 1)
            {
                return;
            }

            var allDonatableItems = GetAllDonatableItems().ToList();
            var allMissingDonatableItems = GetAllMissingDonatableItems(allDonatableItems).ToList();
            var allHintedDonatableItemsWithFlair = GetAllHintedDonatableItems(allMissingDonatableItems);
            var allHintedDonatableItems = allHintedDonatableItemsWithFlair.Keys.ToList();

            for (var i = 0; i < metalDetectorUpgrades; i++)
            {
                var random = new Random(seed + i);
                var choice = random.NextDouble();
                var priceMultiplier = _merchantArtifactPriceMultipliers[random.Next(0, _merchantArtifactPriceMultipliers.Length)];
                if (allHintedDonatableItems.Any() && choice < 0.25)
                {
                    priceMultiplier *= 4;
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allHintedDonatableItems);
                    _flairOverride.Add(chosenArtifactOrMineral, allHintedDonatableItemsWithFlair[chosenArtifactOrMineral]);
                    var price = (int)(chosenArtifactOrMineral.salePrice() * priceMultiplier);
                    stock.TryAdd(chosenArtifactOrMineral, new[] { price, 1 });
                }
                else if (allMissingDonatableItems.Any() && choice < 0.50)
                {
                    priceMultiplier *= 2;
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allMissingDonatableItems);
                    var price = (int)(chosenArtifactOrMineral.salePrice() * priceMultiplier);
                    stock.TryAdd(chosenArtifactOrMineral, new[] { price, 1 });
                }
                else
                {
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allDonatableItems);
                    var price = (int)(chosenArtifactOrMineral.salePrice() * priceMultiplier);
                    stock.TryAdd(chosenArtifactOrMineral, new[] { price, 1 });
                }
            }
        }

        private static IEnumerable<Object> GetAllDonatableItems()
        {
            for (var i = 50; i < 600; i++)
            {
                var item = new Object(i, 1);
                if (item != null && item.Type != null && (item.Type.Equals("Arch") || item.Type.Equals("Minerals")))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<Object> GetAllMissingDonatableItems(IEnumerable<Object> allDonatableItems)
        {
            var museum = Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum;
            foreach (var donatableItem in allDonatableItems)
            {
                if (museum.museumAlreadyHasArtifact(donatableItem.ParentSheetIndex) && !_locationChecker.GetAllLocationsNotCheckedContainingWord(donatableItem.Name).Any())
                {
                    continue;
                }

                yield return donatableItem;
            }
        }

        private static Dictionary<Object, string> GetAllHintedDonatableItems(IEnumerable<Object> allMissingDonatableItems)
        {
            var hints = _archipelago.GetHints().Where(x => !x.Found && _archipelago.GetPlayerName(x.FindingPlayer) == _archipelago.SlotData.SlotName).ToArray();
            var donatableItems = new Dictionary<Object, string>();
            foreach (var donatableItem in allMissingDonatableItems)
            {
                var relevantHint = hints.FirstOrDefault(hint => IsItemRelevantForHint(hint, donatableItem));
                if (relevantHint == null)
                {
                    continue;
                }

                var playerName = _archipelago.GetPlayerName(relevantHint.ReceivingPlayer);
                var text =
                    $"I found something interesting, and I hear {playerName} wants someone to donate it to the Museum...";
                donatableItems.Add(donatableItem, text);
            }

            return donatableItems;
        }

        private static bool IsItemRelevantForHint(Hint hint, Object item)
        {
            var locationName = _archipelago.GetLocationName(hint.LocationId);
            var isDirectlyNeeded = locationName.Contains(item.Name, StringComparison.InvariantCultureIgnoreCase);
            if (isDirectlyNeeded)
            {
                return true;
            }

            if (locationName == $"{MuseumInjections.MUSEUMSANITY_PREFIX} {MuseumInjections.MUSEUMSANITY_DWARF_SCROLLS}")
            {
                return item.Name.Contains("Dwarf Scroll");
            }

            if (locationName == $"{MuseumInjections.MUSEUMSANITY_PREFIX} {MuseumInjections.MUSEUMSANITY_SKELETON_FRONT}")
            {
                return item.Name is "Prehistoric Skull" or "Skeletal Hand" or "Prehistoric Scapula";
            }

            if (locationName == $"{MuseumInjections.MUSEUMSANITY_PREFIX} {MuseumInjections.MUSEUMSANITY_SKELETON_MIDDLE}")
            {
                return item.Name is "Prehistoric Rib" or "Prehistoric Vertebra";
            }

            if (locationName == $"{MuseumInjections.MUSEUMSANITY_PREFIX} {MuseumInjections.MUSEUMSANITY_SKELETON_BACK}")
            {
                return item.Name is "Prehistoric Tibia" or "Skeletal Tail";
            }

            return false;
        }

        private static Object GetRandomArtifactOrMineral(Random random, List<Object> allDonatableItems)
        {
            var chosenIndex = random.Next(0, allDonatableItems.Count);
            var chosenObject = allDonatableItems[chosenIndex];
            allDonatableItems.RemoveAt(chosenIndex);
            return chosenObject;
        }

        private static int ModifyPrice(int price, double priceMultiplier)
        {
            return (int)Math.Round(price * priceMultiplier, MidpointRounding.ToEven);
        }

        public static bool IsTravelingMerchantDay(int dayOfMonth, out string playerName)
        {
            var dayOfWeek = Days.GetDayOfWeekName(dayOfMonth);
            var requiredAPItemToSeeMerchantToday = string.Format(AP_MERCHANT_DAYS, dayOfWeek);
            var hasReceivedToday = _archipelago.HasReceivedItem(requiredAPItemToSeeMerchantToday, out playerName);
            if (!hasReceivedToday)
            {
                // Scout the salable
            }

            return hasReceivedToday;
        }

        public static bool HasAnyTravelingMerchantDay()
        {
            foreach (var day in Days.DaysOfWeek)
            {
                var requiredAPItemToSeeMerchantToday = string.Format(AP_MERCHANT_DAYS, day);
                if (_archipelago.HasReceivedItem(requiredAPItemToSeeMerchantToday, out _))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddApStock(Dictionary<ISalable, int[]> currentStock, Random random, double priceMultiplier)
        {
            var dayOfWeek = Days.GetDayOfWeekName(Game1.dayOfMonth);
            var apItems = new List<string>();
            for (var i = 1; i < 4; i++)
            {
                var apLocationName = string.Format(AP_MERCHANT_LOCATION, dayOfWeek, i);

                if (_locationChecker.IsLocationChecked(apLocationName))
                {
                    continue;
                }

                apItems.Add(apLocationName);
            }

            if (!apItems.Any())
            {
                return;
            }

            var chosenApItem = apItems[random.Next(0, apItems.Count)];

            var scamName = _merchantApItemNames[random.Next(0, _merchantApItemNames.Length)];
            var myActiveHints = _archipelago.GetMyActiveHints();
            var apLocation = new PurchaseableArchipelagoLocation(scamName, chosenApItem, _monitor, _modHelper, _locationChecker, _archipelago, myActiveHints);
            var price = ModifyPrice(_merchantPrices[random.Next(0, _merchantPrices.Length)], priceMultiplier);

            currentStock.Add(apLocation, new[] { price, 1 });
        }

        private static readonly double[] _merchantArtifactPriceMultipliers = new[] // Strong odds of a price slightly above the normal, small odds of significantly cheaper or significantly more expensive
        {
            0.1,
            0.25,
            0.5,
            1, 1,
            2, 2,
            3, 3,
            4, 4,
            5, 5,
            10,
            20,
            50,
        };

        private static readonly int[] _merchantPrices = new[]
        {
            250,
            500,
            1000,
            2000,
            5000,
            10000,
        };

        private static readonly string[] _merchantApItemNames = new[]
        {
            "Snake Oil",
            "Glass of time",
            "Orb of Slope Detection",
            "Dagger of Time",
            "Harpy's Quill",
            "Oil of Slipperiness",
            "Gauntlets of Touch",
            "Disk of Enlargement",
            "Torch of Night Vision",
            "Potion of Hydration",
            "Viper Liquid",
            "Fire Distinguisher",
            "Bag of Holding",
            "Stone of Weather Detection",
            "Tigerbane Stone",
            "Eyepatch of 2D Vision",
            "Mirror of Reflection",
            "Potion of Courage",
            "Moveable Rod",
            "Orb of shattering",
            "Spectacles of Darkness",
            "Leash of Holding",
            "Dagger of Desperation",
            "Dihydrogen Monoxide Grenade",
            "Pan of Frying",
            "Pan of Drying",
            "Ringing Ring",
        };
    }
}
