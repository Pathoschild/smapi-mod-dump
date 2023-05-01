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
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications.Buildings;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections
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

                var carpenterMenu = new CarpenterMenuArchipelago(_archipelago);
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

        private static Dictionary<ISalable, int[]> GetCarpenterBuildingsAPLocations()
        {
            var carpenterAPStock = new Dictionary<ISalable, int[]>();

            carpenterAPStock.AddArchipelagoHouseLocationToStock(BUILDING_HOUSE_KITCHEN, 10000, new[] { Wood(450) });
            carpenterAPStock.AddArchipelagoHouseLocationToStock(BUILDING_HOUSE_KIDS_ROOM, 50000, new[] { Hardwood(150) }, 1);
            carpenterAPStock.AddArchipelagoHouseLocationToStock(BUILDING_HOUSE_CELLAR, 100000, new Item[0], 2);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_COOP, 4000, new[] { Wood(300), Stone(100) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BIG_COOP, 10000, new[] { Wood(400), Stone(150) }, BUILDING_COOP);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_DELUXE_COOP, 20000, new[] { Wood(500), Stone(200) }, BUILDING_BIG_COOP);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BARN, 6000, new[] { Wood(350), Stone(150) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BIG_BARN, 12000, new[] { Wood(400), Stone(200) }, BUILDING_BARN);
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_DELUXE_BARN, 25000, new[] { Wood(500), Stone(300) }, BUILDING_BIG_BARN);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_FISH_POND, 5000, new[] { Stone(100), Seaweed(5), GreenAlgae(5) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_MILL, 2500, new[] { Stone(50), Wood(150), Cloth(4)});

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SHED, 15000, new[] { Wood(300) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_BIG_SHED, 20000, new[] { Wood(550), Stone(300) }, BUILDING_SHED);

            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SILO, 100, new[] { Stone(100), Clay(10), CopperBar(5) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SLIME_HUTCH, 10000, new[] { Stone(500), RefinedQuartz(10), IridiumBar(1) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_STABLE, 10000, new[] { Hardwood(100), IronBar(5) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_WELL, 1000, new[] { Stone(75) });
            carpenterAPStock.AddArchipelagoLocationToStock(BUILDING_SHIPPING_BIN, 250, new[] { Wood(150) });

            return carpenterAPStock;
        }

        private static void AddArchipelagoHouseLocationToStock(this Dictionary<ISalable, int[]> stock, string houseUpgradeName, int price, Item[] materials, int requiredHouseUpgrade = 0)
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

            var purchasableCheck = new PurchaseableArchipelagoLocation(houseUpgradeName, locationName, _modHelper, _locationChecker, _archipelago);
            foreach (var material in materials)
            {
                purchasableCheck.AddMaterialRequirement(material);
            }

            stock.Add(purchasableCheck, new[] { price, 1 });
        }

        private static void AddArchipelagoLocationToStock(this Dictionary<ISalable, int[]> stock, string buildingName, int price, Item[] materials, string requiredBuilding = null)
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

            var purchasableCheck = new PurchaseableArchipelagoLocation(buildingName, locationName, _modHelper, _locationChecker, _archipelago);
            foreach (var material in materials)
            {
                purchasableCheck.AddMaterialRequirement(material);
            }

            stock.Add(purchasableCheck, new[] { price, 1 });
        }

        public static bool HasReceivedBuilding(string buildingName, out string senderName)
        {
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

        private static Item StardewObject(int id, int amount)
        {
            return new Object(id, amount);
        }
    }
}
