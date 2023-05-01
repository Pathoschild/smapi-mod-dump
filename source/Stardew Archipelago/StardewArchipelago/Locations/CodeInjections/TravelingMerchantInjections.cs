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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections
{
    public class TravelingMerchantInjections
    {
        private const double BASE_STOCK = 0.1;
        private const double STOCK_AMOUNT_PER_UPGRADE = 0.15;
        private const double BASE_PRICE = 1.4;
        private const double DISCOUNT_PER_UPGRADE = 0.1;
        private const string AP_MERCHANT_DAYS = "Traveling Merchant: {0}"; // 7, One for each day
        private const string AP_MERCHANT_STOCK = "Traveling Merchant Stock Size"; // 10% base size, 6 upgrades of 15% each
        private const string AP_MERCHANT_DISCOUNT = "Traveling Merchant Discount"; // Base Price 140%, 8 x 10% discount
        private const string AP_MERCHANT_LOCATION = "Traveling Merchant {0} Item {1}";
        private const string AP_METAL_DETECTOR = "Traveling Merchant Metal Detector"; // Base Price 140%, 8 x 10% discount

        private static readonly string[] _days = new[]
            { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        private static Dictionary<ISalable, string> _flairOverride;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
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
        public static bool NightMarketCheckAction_IsTravelingMerchantDay_Prefix(BeachNightMarket __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
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
                    TravelingMerchantInjections.IsTravelingMerchantDay(Game1.dayOfMonth, out var sendingPlayer);

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

                var day = GetDayOfWeekName(Game1.dayOfMonth);
                var locationName = Game1.currentLocation is Forest ? "Cindersap Forest" : "the Beach Night Market";
                var text = _flairOverride.Any() ? _flairOverride.Values.First() : $"{playerName} recommended that I visit {locationName} on {day}s. Take a look at my wares!";
                __instance.potraitPersonDialogue = Game1.parseText(text, Game1.dialogueFont, 304);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(SetUpShopOwner_TravelingMerchantApFlair_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
        
        public static void GenerateLocalTravelingMerchantStock_APStock_Postfix(int seed, ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var priceUpgrades = _archipelago.GetReceivedItemCount(AP_MERCHANT_DISCOUNT);
                var priceMultiplier = BASE_PRICE - (priceUpgrades * DISCOUNT_PER_UPGRADE);
                var stockUpgrades = _archipelago.GetReceivedItemCount(AP_MERCHANT_STOCK);
                var chanceForItemToRemain = BASE_STOCK + (stockUpgrades * STOCK_AMOUNT_PER_UPGRADE);

                var random = new Random(seed);

                AddMetalDetectorItems(__result, seed);

                var itemsToRemove = new List<ISalable>();

                foreach (var (item, prices) in __result)
                {
                    if (random.NextDouble() > chanceForItemToRemain)
                    {
                        itemsToRemove.Add(item);
                    }

                    prices[0] = ModifyPrice(prices[0], priceMultiplier);
                }

                AddApStock(ref __result, random, priceMultiplier);

                foreach (var itemToRemove in itemsToRemove)
                {
                    __result.Remove(itemToRemove);
                    if (_flairOverride.ContainsKey(itemToRemove))
                    {
                        _flairOverride.Remove(itemToRemove);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GenerateLocalTravelingMerchantStock_APStock_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
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
                var price = new[] { _merchantArtifactPrices[random.Next(0, _merchantArtifactPrices.Length)], 1 };
                if (allHintedDonatableItems.Any() && choice < 0.25)
                {
                    price[0] *= 4;
                    var chosenArtifactOrMineral = GetRandomArtifactOrMineral(random, allHintedDonatableItems);
                    _flairOverride.Add(chosenArtifactOrMineral, allHintedDonatableItemsWithFlair[chosenArtifactOrMineral]);
                    stock.Add(chosenArtifactOrMineral, price);
                }
                else if (allMissingDonatableItems.Any() && choice < 0.50)
                {
                    price[0] *= 2;
                    stock.Add(GetRandomArtifactOrMineral(random, allMissingDonatableItems), price);
                }
                else
                {
                    stock.Add(GetRandomArtifactOrMineral(random, allDonatableItems), price);
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
                if (museum.museumAlreadyHasArtifact(donatableItem.ParentSheetIndex))
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
            var isDirectlyNeeded = locationName == $"Museumsanity: {item.Name}";
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
            var dayOfWeek = GetDayOfWeekName(dayOfMonth);
            var requiredAPItemToSeeMerchantToday = string.Format(AP_MERCHANT_DAYS, dayOfWeek);
            var hasReceivedToday = _archipelago.HasReceivedItem(requiredAPItemToSeeMerchantToday, out playerName);
            if (!hasReceivedToday)
            {
                // Scout the item
            }

            return hasReceivedToday;
        }

        private static void AddApStock(ref Dictionary<ISalable, int[]> currentStock, Random random, double priceMultiplier)
        {
            var dayOfWeek = GetDayOfWeekName(Game1.dayOfMonth);
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
            var apLocation =
                new PurchaseableArchipelagoLocation(scamName, chosenApItem, _modHelper, _locationChecker, _archipelago);
            var price = ModifyPrice(_merchantPrices[random.Next(0, _merchantPrices.Length)], priceMultiplier);

            currentStock.Add(apLocation, new []{price, 1});
        }

        private static string GetDayOfWeekName(int day)
        {
            var dayOfWeek = day % 7;
            switch (dayOfWeek)
            {
                case 0:
                    return "Sunday";
                case 1:
                    return "Monday";
                case 2:
                    return "Tuesday";
                case 3:
                    return "Wednesday";
                case 4:
                    return "Thursday";
                case 5:
                    return "Friday";
                case 6:
                    return "Saturday";
            }

            throw new ArgumentException($"Invalid day: {day}");
        }

        private static readonly int[] _merchantArtifactPrices = new[]
        {
            10,
            25,
            50,
            75,
            100,
            250,
            500,
            750,
            1000,
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
