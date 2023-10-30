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
using StardewArchipelago.GameModifications.Buildings;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class CarpenterInjections
    {
        public const string BUILDING_PROGRESSIVE_HOUSE = "Progressive House";
        public const string BUILDING_HOUSE_KITCHEN = "Kitchen";
        public const string BUILDING_HOUSE_KIDS_ROOM = "Kids Room";
        public const string BUILDING_HOUSE_CELLAR = "Cellar";

        public const string BUILDING_COOP = "Coop";
        public const string BUILDING_BARN = "Barn";
        public const string BUILDING_WELL = "Well";
        public const string BUILDING_SILO = "Silo";
        public const string BUILDING_MILL = "Mill";
        public const string BUILDING_SHED = "Shed";
        public const string BUILDING_FISH_POND = "Fish Pond";
        public const string BUILDING_STABLE = "Stable";
        public const string BUILDING_SLIME_HUTCH = "Slime Hutch";

        public const string BUILDING_BIG_COOP = "Big Coop";
        public const string BUILDING_DELUXE_COOP = "Deluxe Coop";
        public const string BUILDING_BIG_BARN = "Big Barn";
        public const string BUILDING_DELUXE_BARN = "Deluxe Barn";
        public const string BUILDING_BIG_SHED = "Big Shed";

        public const string BUILDING_SHIPPING_BIN = "Shipping Bin";

        public const string BUILDING_TRACTOR_GARAGE = "Tractor Garage";

        public const string BUILDING_BLUEPRINT_LOCATION_NAME = "{0} Blueprint";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public static bool AnswerDialogueAction_CarpenterConstruct_Prefix(GameLocation __instance, string questionAndAnswer, string[] questionParams, ref bool __result)
        {
            try
            {
                if (questionAndAnswer != "carpenter_Construct")
                {
                    return true; // run original logic
                }

                __result = true;

                Game1.activeClickableMenu = new CarpenterMenuArchipelago(_modHelper, _archipelago);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_CarpenterConstruct_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool CreateQuestionDialogue_CarpenterDialogOptions_Prefix(GameLocation __instance, string question, Response[] answerChoices, string dialogKey)
        {
            try
            {
                if (dialogKey != "carpenter")
                {
                    return true; // run original logic
                }

                var carpenterMenu = new CarpenterMenuArchipelago(_modHelper, _archipelago);
                var carpenterBlueprints = carpenterMenu.GetAvailableBlueprints();

                if (!carpenterBlueprints.Any())
                {
                    answerChoices = answerChoices.Where(x => x.responseKey != "Construct").ToArray();
                }

                var receivedHouseUpgrades = _archipelago.GetReceivedItemCount(BUILDING_PROGRESSIVE_HOUSE);
                if (Game1.player.HouseUpgradeLevel >= receivedHouseUpgrades)
                {
                    answerChoices = answerChoices.Where(x => x.responseKey != "Upgrade").ToArray();
                }

                __instance.lastQuestionKey = dialogKey;
                Game1.drawObjectQuestionDialogue(question, answerChoices.ToList());

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CreateQuestionDialogue_CarpenterDialogOptions_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public BluePrint(string name)
        public static void BluePrintConstructor_CheaperInAP_Postfix(BluePrint __instance, string name)
        {
            try
            {
                var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
                foreach (var itemId in __instance.itemsRequired.Keys.ToArray())
                {
                    var quantity = __instance.itemsRequired[itemId];
                    var modifiedQuantity = (int)(quantity * priceMultiplier);
                    __instance.itemsRequired[itemId] = modifiedQuantity;
                }

                var price = __instance.moneyRequired;
                var modifiedPrice = (int)(price * priceMultiplier);
                __instance.moneyRequired = modifiedPrice;

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(BluePrintConstructor_CheaperInAP_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static void GetCarpenterStock_PurchasableChecks_Postfix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var apPurchasableChecks = GetCarpenterBuildingsAPLocations();
                __result = apPurchasableChecks.Concat(__result).ToDictionary(k => k.Key, v => v.Value);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetCarpenterStock_PurchasableChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static bool HouseUpgradeOffer_OfferCheaperUpgrade_Prefix(GameLocation __instance)
        {
            try
            {
                var text = "";
                var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
                if (Math.Abs(priceMultiplier - 1.0) < 0.001)
                {
                    return true; // run original logic
                }

                switch (Game1.player.HouseUpgradeLevel)
                {
                    case 0:
                        text = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1"));
                        text = text.Replace("10,000", $"{(int)(10000 * priceMultiplier)}")
                                   .Replace("450", $"{(int)(450 * priceMultiplier)}");
                        break;
                    case 1:
                        text = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2"));
                        text = text.Replace("50,000", $"{(int)(50000 * priceMultiplier)}")
                                   .Replace("150", $"{(int)(150 * priceMultiplier)}");
                        break;
                    case 2:
                        text = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3"));
                        text = text.Replace("100,000", $"{(int)(100000 * priceMultiplier)}");
                        break;
                }

                __instance.createQuestionDialogue(text, __instance.createYesNoResponses(), "upgrade");
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(HouseUpgradeOffer_OfferCheaperUpgrade_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool HouseUpgradeOffer_OfferFreeUpgrade_Prefix(GameLocation __instance)
        {
            try
            {
                var receivedHouseUpgrades = _archipelago.GetReceivedItemCount(BUILDING_PROGRESSIVE_HOUSE);
                if (Game1.player.HouseUpgradeLevel >= receivedHouseUpgrades)
                {
                    return false; // don't run original logic
                }

                var houseUpgradeReceivedFromAP = _archipelago.GetAllReceivedItems()
                    .Where(x => x.ItemName == BUILDING_PROGRESSIVE_HOUSE).ToArray();

                var upgrade1Dialogue = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1"));
                var upgrade2Dialogue = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2"));
                var upgrade3Dialogue = Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3"));

                var apUpgradeDialogue = "I see here someone has already funded an upgrade for your house.";
                var startNowQuestion = "^Do you want me to start working on that right away?";

                switch (Game1.player.HouseUpgradeLevel)
                {
                    case 0:
                        apUpgradeDialogue +=
                            $"^{houseUpgradeReceivedFromAP[0].PlayerName} paid for a Kitchen and a Master Bedroom.{startNowQuestion}";
                        __instance.createQuestionDialogue(apUpgradeDialogue, __instance.createYesNoResponses(), "upgrade");
                        break;
                    case 1:
                        var bonusDialogue = GetChildRoomDialogue();
                        apUpgradeDialogue +=
                            $"^{houseUpgradeReceivedFromAP[1].PlayerName} paid for an entire second floor, including one extra bedroom. {bonusDialogue}{startNowQuestion}";
                        __instance.createQuestionDialogue(apUpgradeDialogue, __instance.createYesNoResponses(), "upgrade");
                        break;
                    case 2:
                        apUpgradeDialogue +=
                            $"^{houseUpgradeReceivedFromAP[2].PlayerName} paid for a Cellar and 33 Casks.^I hope I'll get to taste an aged Goat Cheese!{startNowQuestion}";
                        __instance.createQuestionDialogue(apUpgradeDialogue, __instance.createYesNoResponses(), "upgrade");
                        break;
                }
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(HouseUpgradeOffer_OfferFreeUpgrade_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static string GetChildRoomDialogue()
        {
            var player = Game1.player;
            if (!player.isMarried() && !player.isEngaged())
            {
                var who = player.friendshipData.Keys.Where(name => player.friendshipData[name].IsDating()).OrderByDescending(name => player.friendshipData[name].Points)
                    .FirstOrDefault();

                if (who == null)
                {
                    return "If you ever start something with one of the people in town, you'll be ready!";
                }

                return $"Are you planning on popping the question to {who} soon?";
            }

            var frienshipNature = player.friendshipData[player.spouse];
            var spouse = Game1.getCharacterFromName(player.spouse, true, true);


            if (player.isEngaged())
            {
                return $"You and {spouse.Name} will be ready to start your new lives!";
            }

            if (frienshipNature.RoommateMarriage)
            {
                return $"But you probably won't need the kid beds";
            }
            else
            {
                var spouseIsMale = spouse.Gender == 0;
                if (player.IsMale == spouseIsMale)
                {
                    return $"You and {spouse.Name} will be able to adopt a baby!";
                }
                else
                {
                    if (player.IsMale)
                    {
                        return $"Is {spouse.Name} expecting a baby?";
                    }
                    else
                    {
                        return $"Are you expecting a baby with {spouse.Name}?";
                    }
                }
            }

            return "";
        }

        public static bool HouseUpgradeAccept_FreeFromAP_Prefix(GameLocation __instance)
        {
            try
            {
                var receivedHouseUpgrades = _archipelago.GetReceivedItemCount(BUILDING_PROGRESSIVE_HOUSE);
                if (Game1.player.HouseUpgradeLevel >= receivedHouseUpgrades)
                {
                    return false; // don't run original logic
                }

                Game1.player.daysUntilHouseUpgrade.Value = 3;
                Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(HouseUpgradeAccept_FreeFromAP_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool HouseUpgradeAccept_CheaperInAP_Prefix(GameLocation __instance)
        {
            try
            {
                var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
                if (Math.Abs(priceMultiplier - 1.0) < 0.001)
                {
                    return true; // run original logic
                }

                switch (Game1.player.HouseUpgradeLevel)
                {
                    case 0:
                        var price1 = (int)(10000 * priceMultiplier);
                        var woodAmount = (int)(450 * priceMultiplier);
                        if (Game1.player.Money >= price1 && Game1.player.hasItemInInventory(388, woodAmount))
                        {
                            Game1.player.daysUntilHouseUpgrade.Value = 3;
                            Game1.player.Money -= price1;
                            Game1.player.removeItemsFromInventory(388, woodAmount);
                            Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                            break;
                        }
                        if (Game1.player.Money < price1)
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                            break;
                        }
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood1"));
                        break;
                    case 1:
                        var price2 = (int)(50000 * priceMultiplier);
                        var hardwoodAmount = (int)(150 * priceMultiplier);
                        if (Game1.player.Money >= price2 && Game1.player.hasItemInInventory(709, hardwoodAmount))
                        {
                            Game1.player.daysUntilHouseUpgrade.Value = 3;
                            Game1.player.Money -= price2;
                            Game1.player.removeItemsFromInventory(709, hardwoodAmount);
                            Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                            break;
                        }
                        if (Game1.player.Money < price2)
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                            break;
                        }
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood2"));
                        break;
                    case 2:
                        var price3 = (int)(100000 * priceMultiplier);
                        if (Game1.player.Money >= price3)
                        {
                            Game1.player.daysUntilHouseUpgrade.Value = 3;
                            Game1.player.Money -= price3;
                            Game1.getCharacterFromName("Robin").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted"));
                            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
                            break;
                        }
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                        break;
                }
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(HouseUpgradeAccept_CheaperInAP_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static Dictionary<ISalable, int[]> GetCarpenterBuildingsAPLocations()
        {
            var carpenterAPStock = new Dictionary<ISalable, int[]>();
            var myActiveHints = _archipelago.GetMyActiveHints();

            carpenterAPStock.AddArchipelagoHouseLocationToStock(BUILDING_HOUSE_KITCHEN, 10000, new[] { Wood(450) }, myActiveHints);
            carpenterAPStock.AddArchipelagoHouseLocationToStock(BUILDING_HOUSE_KIDS_ROOM, 50000, new[] { Hardwood(150) }, myActiveHints, 1);
            carpenterAPStock.AddArchipelagoHouseLocationToStock(BUILDING_HOUSE_CELLAR, 100000, new Item[0], myActiveHints, 2);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_COOP, 4000, new[] { Wood(300), Stone(100) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BIG_COOP, 10000, new[] { Wood(400), Stone(150) }, myActiveHints, BUILDING_COOP);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_DELUXE_COOP, 20000, new[] { Wood(500), Stone(200) }, myActiveHints, BUILDING_BIG_COOP);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BARN, 6000, new[] { Wood(350), Stone(150) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BIG_BARN, 12000, new[] { Wood(400), Stone(200) }, myActiveHints, BUILDING_BARN);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_DELUXE_BARN, 25000, new[] { Wood(500), Stone(300) }, myActiveHints, BUILDING_BIG_BARN);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_FISH_POND, 5000, new[] { Stone(100), Seaweed(5), GreenAlgae(5) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_MILL, 2500, new[] { Stone(50), Wood(150), Cloth(4) }, myActiveHints);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SHED, 15000, new[] { Wood(300) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BIG_SHED, 20000, new[] { Wood(550), Stone(300) }, myActiveHints, BUILDING_SHED);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SILO, 100, new[] { Stone(100), Clay(10), CopperBar(5) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SLIME_HUTCH, 10000, new[] { Stone(500), RefinedQuartz(10), IridiumBar(1) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_STABLE, 10000, new[] { Hardwood(100), IronBar(5) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_WELL, 1000, new[] { Stone(75) }, myActiveHints);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SHIPPING_BIN, 250, new[] { Wood(150) }, myActiveHints);
            if (_archipelago.SlotData.Mods.HasMod(ModNames.TRACTOR))
            {
                carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_TRACTOR_GARAGE, 150000, new[] { IronBar(20), IridiumBar(5), BatteryPack(5) }, myActiveHints);
            }

            return carpenterAPStock;
        }

        private static void AddArchipelagoHouseLocationToStock(this Dictionary<ISalable, int[]> stock, string houseUpgradeName, int price, Item[] materials, Hint[] myActiveHints, int requiredHouseUpgrade = 0)
        {
            var locationName = string.Format(BUILDING_BLUEPRINT_LOCATION_NAME, houseUpgradeName);
            if (_locationChecker.IsLocationChecked(locationName))
            {
                return;
            }

            var numberReceived = _archipelago.GetReceivedItemCount(BUILDING_PROGRESSIVE_HOUSE);

            if (numberReceived < requiredHouseUpgrade)
            {
                return;
            }

            AddToStock(stock, houseUpgradeName, price, materials, locationName, myActiveHints);
        }

        private static void AddArchipelagoLocationToStock(this Dictionary<ISalable, int[]> stock, string buildingName, int price, Item[] materials, Hint[] myActiveHints, string requiredBuilding = null)
        {
            var locationName = string.Format(BUILDING_BLUEPRINT_LOCATION_NAME, buildingName);
            if (_locationChecker.IsLocationChecked(locationName))
            {
                return;
            }

            if (requiredBuilding != null)
            {
                var hasReceivedRequirement = HasReceivedBuilding(requiredBuilding, out _);
                if (!hasReceivedRequirement)
                {
                    return;
                }
            }

            AddToStock(stock, buildingName, price, materials, locationName, myActiveHints);
        }

        private static void AddToStock(Dictionary<ISalable, int[]> stock, string locationDisplayName, int price, Item[] materials, string locationName, Hint[] myActiveHints)
        {
            var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
            var purchasableCheck = new PurchaseableArchipelagoLocation(locationDisplayName, locationName, _modHelper, _locationChecker, _archipelago, myActiveHints);
            foreach (var material in materials)
            {
                material.Stack = (int)(material.Stack * priceMultiplier);
                purchasableCheck.AddMaterialRequirement(material);
            }

            stock.Add(purchasableCheck, new[] { (int)(price * priceMultiplier), 1 });
        }

        public static bool HasReceivedBuilding(string buildingName, out string senderName)
        {
            if (buildingName == "TractorGarage")
            {
                buildingName = "Tractor Garage";
            }
            senderName = "";
            var numberRequired = 1;

            var bigPrefix = "Big ";
            if (buildingName­.StartsWith(bigPrefix))
            {
                numberRequired = 2;
                buildingName = buildingName.Substring(bigPrefix.Length);
            }

            var deluxePrefix = "Deluxe ";
            if (buildingName­.StartsWith(deluxePrefix))
            {
                numberRequired = 3;
                buildingName = buildingName.Substring(deluxePrefix.Length);
            }

            if (buildingName == BUILDING_COOP || buildingName == BUILDING_BARN || buildingName == BUILDING_SHED)
            {
                buildingName = $"Progressive {buildingName}";
            }

            var numberReceived = _archipelago.GetReceivedItemCount(buildingName);

            var hasReceivedEnough = numberReceived >= numberRequired;
            if (!hasReceivedEnough)
            {
                return false;
            }

            senderName = _archipelago.GetAllReceivedItems().Last(x => x.ItemName == buildingName).PlayerName;
            return true;
        }

        private static Item Wood(int amount)
        {
            return StardewObject(388, amount);
        }

        private static Item Stone(int amount)
        {
            return StardewObject(390, amount);
        }

        private static Item Seaweed(int amount)
        {
            return StardewObject(152, amount);
        }

        private static Item GreenAlgae(int amount)
        {
            return StardewObject(153, amount);
        }

        private static Item Cloth(int amount)
        {
            return StardewObject(428, amount);
        }

        private static Item Clay(int amount)
        {
            return StardewObject(330, amount);
        }

        private static Item CopperBar(int amount)
        {
            return StardewObject(334, amount);
        }

        private static Item RefinedQuartz(int amount)
        {
            return StardewObject(338, amount);
        }

        private static Item IridiumBar(int amount)
        {
            return StardewObject(337, amount);
        }

        private static Item Hardwood(int amount)
        {
            return StardewObject(709, amount);
        }

        private static Item IronBar(int amount)
        {
            return StardewObject(335, amount);
        }

        private static Item BatteryPack(int amount)
        {
            return StardewObject(787, amount);
        }

        private static Item StardewObject(int id, int amount)
        {
            return new Object(id, amount);
        }
    }
}
