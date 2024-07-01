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
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Serialization;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;
using ArchipelagoLocation = StardewArchipelago.Locations.InGameLocations.ArchipelagoLocation;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public class TravelingMerchantInjections
    {
        private const double BASE_STOCK = 1;
        private const double STOCK_AMOUNT_PER_UPGRADE_FOR_RANDOM_ITEMS = 1;
        private const double STOCK_AMOUNT_PER_ONE_PERCENT_CHECKS_FOR_RANDOM_ITEMS = 0.1;
        private const double STOCK_AMOUNT_REDUCTION_PER_PURCHASE = 1;

        // 0.1 + (6 * 0.1) + (100 * 0.05)
        // 10% + 60% + 100% = 6

        public const string AP_MERCHANT_DAYS = "Traveling Merchant: {0}"; // 7, One for each day
        private const string AP_MERCHANT_STOCK = "Traveling Merchant Stock Size"; // 10% base size, 6 upgrades of 15% each
        private const string AP_MERCHANT_LOCATION = "Traveling Merchant {0} Item {1}";
        private const string AP_METAL_DETECTOR = "Traveling Merchant Metal Detector"; // Base Price 140%, 8 x 10% discount
        private const string AP_WEDDING_RING_RECIPE = "Wedding Ring Recipe";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ArchipelagoStateDto _archipelagoState;

        private static Dictionary<ISalable, string> _flairOverride;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker, ArchipelagoStateDto archipelagoState)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _archipelagoState = archipelagoState;
            _flairOverride = new Dictionary<ISalable, string>();
        }

        // public bool ShouldTravelingMerchantVisitToday()
        public static bool ShouldTravelingMerchantVisitToday_ArchipelagoDays_Prefix(Forest __instance, ref bool __result)
        {
            try
            {
                __result = IsTravelingMerchantDay(Game1.dayOfMonth);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShouldTravelingMerchantVisitToday_ArchipelagoDays_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
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

                var isTravelingMerchantDay = IsTravelingMerchantDay(Game1.dayOfMonth);

                if (!isTravelingMerchantDay)
                {
                    Game1.drawObjectDialogue("The traveling merchant isn't here today.");
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(NightMarketCheckAction_IsTravelingMerchantDay_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public void SetUpShopOwner(ShopOwnerData ownerData, NPC owner = null)
        public static void SetUpShopOwner_TravelingMerchantApFlair_Postfix(ShopMenu __instance, ShopOwnerData ownerData, NPC owner)
        {
            try
            {
                if (__instance.ShopId != "Traveler" || !IsTravelingMerchantDay(Game1.dayOfMonth, out var playerName))
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

            var currentStockSize = GetRandomItemStockSize(_archipelago.GetReceivedItemCount(AP_MERCHANT_STOCK));
            var day = Days.GetDayOfWeekName(Game1.dayOfMonth);
            var locationName = Game1.currentLocation is Forest ? "Cindersap Forest" : "the Beach Night Market";
            var text = _flairOverride.Any() ? _flairOverride.Values.First() : GetFlairForToday(playerName, locationName, day, currentStockSize);
            var prettyStockSize = (int)(currentStockSize * 10);
            text += $"{Environment.NewLine}Stock: {prettyStockSize}%";
            travelingMerchantShopMenu.potraitPersonDialogue = Game1.parseText(text, Game1.dialogueFont, 304);
        }

        private static string GetFlairForToday(string playerName, string locationName, string day, double currentStockSize)
        {
            if (currentStockSize < 2)
            {
                return $"I'm sorry I don't have much to offer. Maybe do something else in the meantime?";
            }

            if (currentStockSize < 9.5)
            {
                return playerName == null || locationName == null || day == null ? "I got lots of good stuff for sale!" : $"{playerName} recommended that I visit {locationName} on {day}s.";
            }

            return "Sweety, will you please buy something? I have a family to feed";
        }

        public static bool OnPurchasedRandomItem(string[] args, TriggerActionContext context, out string error)
        {
            try
            {
                _archipelagoState.TravelingMerchantPurchases++;
                SetTravelingMerchantFlair(Game1.activeClickableMenu as ShopMenu);
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
                return false;
            }
        }

        public static bool HasStockSizeQueryDelegate(string[] query, GameStateQueryContext context)
        {
            if (!Utility.TryCreateIntervalRandom("day", string.Join(" ", query), out var random, out var error))
            {
                return false;
            }

            var stockUpgrades = _archipelago.GetReceivedItemCount(AP_MERCHANT_STOCK);
            var currentStockSize = GetRandomItemStockSize(stockUpgrades);

            var minimumStockSize = double.Parse(query[1]);

            if (currentStockSize < minimumStockSize)
            {
                return false;
            }
            if (currentStockSize > minimumStockSize + 1)
            {
                return true;
            }

            var chanceToRemain = currentStockSize - minimumStockSize;

            var shouldRemain = random.NextDouble() < chanceToRemain;
            return shouldRemain;
        }

        private static double GetRandomItemStockSize(int stockUpgrades)
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

        public static IEnumerable<ItemQueryResult> CreateMetalDetectorItems(string key, string arguments, ItemQueryContext context, bool avoidrepeat, HashSet<string> avoiditemids, Action<string, string> logerror)
        {
            _flairOverride.Clear();
            var metalDetectorUpgrades = _archipelago.GetReceivedItemCount(AP_METAL_DETECTOR);
            if (metalDetectorUpgrades < 1)
            {
                yield break;
            }


            var allDonatableItems = GetAllDonatableItems().ToList();
            var allMissingDonatableItems = GetAllMissingDonatableItems(allDonatableItems).ToList();
            var allHintedDonatableItemsWithFlair = GetAllHintedDonatableItems(allMissingDonatableItems);
            var allHintedDonatableItems = allHintedDonatableItemsWithFlair.Keys.ToList();

            for (var i = 0; i < metalDetectorUpgrades; i++)
            {
                var random = new Random((int)(Game1.startingGameSeed ?? 0) + (int)Game1.stats.DaysPlayed + i);
                var choice = random.NextDouble();
                var priceMultiplier = _merchantArtifactPriceMultipliers[random.Next(0, _merchantArtifactPriceMultipliers.Length)];
                if (allHintedDonatableItems.Any() && choice < 0.25)
                {
                    priceMultiplier *= 4;
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allHintedDonatableItems);
                    _flairOverride.Add(chosenArtifactOrMineral, allHintedDonatableItemsWithFlair[chosenArtifactOrMineral]);
                    var price = (int)(chosenArtifactOrMineral.salePrice() * priceMultiplier);
                    yield return new ItemQueryResult(chosenArtifactOrMineral)
                    {
                        OverrideBasePrice = price,
                        OverrideShopAvailableStock = 1,
                        OverrideStackSize = 1,
                    };
                }
                else if (allMissingDonatableItems.Any() && choice < 0.50)
                {
                    priceMultiplier *= 2;
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allMissingDonatableItems);
                    var price = (int)(chosenArtifactOrMineral.salePrice() * priceMultiplier);
                    yield return new ItemQueryResult(chosenArtifactOrMineral)
                    {
                        OverrideBasePrice = price,
                        OverrideShopAvailableStock = 1,
                        OverrideStackSize = 1,
                    };
                }
                else
                {
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allDonatableItems);
                    var price = (int)(chosenArtifactOrMineral.salePrice() * priceMultiplier);
                    yield return new ItemQueryResult(chosenArtifactOrMineral)
                    {
                        OverrideBasePrice = price,
                        OverrideShopAvailableStock = 1,
                        OverrideStackSize = 1,
                    };
                }
            }
        }

        private static IEnumerable<Object> GetAllDonatableItems()
        {
            for (var i = 50; i < 600; i++)
            {
                var item = new Object(i.ToString(), 1);
                if (item.Type != null && (item.Type.Equals("Arch") || item.Type.Equals("Minerals")))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<Object> GetAllMissingDonatableItems(IEnumerable<Object> allDonatableItems)
        {
            foreach (var donatableItem in allDonatableItems)
            {
                if (LibraryMuseum.HasDonatedArtifact(donatableItem.QualifiedItemId) && !_locationChecker.GetAllLocationsNotCheckedContainingWord(donatableItem.Name).Any())
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

        public static bool IsTravelingMerchantDay(int dayOfMonth)
        {
            return IsTravelingMerchantDay(dayOfMonth, out _);
        }

        public static bool IsTravelingMerchantDay(int dayOfMonth, out string sendingPlayerName)
        {
            var dayOfWeek = Days.GetDayOfWeekName(dayOfMonth);
            var requiredAPItemToSeeMerchantToday = string.Format(AP_MERCHANT_DAYS, dayOfWeek);
            var hasReceivedToday = _archipelago.HasReceivedItem(requiredAPItemToSeeMerchantToday, out sendingPlayerName);
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

        public static IEnumerable<ItemQueryResult> CreateDailyCheck(string key, string arguments, ItemQueryContext context, bool avoidrepeat, HashSet<string> avoiditemids, Action<string, string> logerror)
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
                yield break;
            }

            var random = new Random((int)(Game1.startingGameSeed ?? 0) + (int)Game1.stats.DaysPlayed);
            var chosenApItem = apItems[random.Next(0, apItems.Count)];

            var scamName = _merchantApItemNames[random.Next(0, _merchantApItemNames.Length)];
            var myActiveHints = _archipelago.GetMyActiveHints();
            var apLocation = new ArchipelagoLocation(scamName, chosenApItem, _monitor, _modHelper, _locationChecker, _archipelago, myActiveHints);
            var price = _merchantPrices[random.Next(0, _merchantPrices.Length)];

            yield return new ItemQueryResult(apLocation)
            {
                OverrideBasePrice = price,
                OverrideStackSize = 1,
                OverrideShopAvailableStock = 1,
            };
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
