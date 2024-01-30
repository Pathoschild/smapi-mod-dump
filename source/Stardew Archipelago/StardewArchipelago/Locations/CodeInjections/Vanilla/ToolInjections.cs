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
using Archipelago.MultiClient.Net.Models;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Unlocks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class ToolInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public static bool AnswerDialogueAction_ToolUpgrade_Prefix(GameLocation __instance, string questionAndAnswer,
            string[] questionParams, ref bool __result)
        {
            try
            {
                if (questionAndAnswer != "Blacksmith_Upgrade")
                {
                    return true; // run original logic
                }

                __result = true;

                var utilityPriceForToolMethod = _modHelper.Reflection.GetMethod(typeof(Utility), "priceForToolUpgradeLevel");
                var indexOfExtraMaterialForToolMethod = _modHelper.Reflection.GetMethod(typeof(Utility), "indexOfExtraMaterialForToolUpgrade");

                var myActiveHints = _archipelago.GetMyActiveHints();
                var blacksmithUpgradeStock = new Dictionary<ISalable, int[]>();
                AddToolUpgradeToStock("Axe", blacksmithUpgradeStock, utilityPriceForToolMethod, indexOfExtraMaterialForToolMethod, myActiveHints);
                AddToolUpgradeToStock("Watering Can", blacksmithUpgradeStock, utilityPriceForToolMethod, indexOfExtraMaterialForToolMethod, myActiveHints);
                AddToolUpgradeToStock("Pickaxe", blacksmithUpgradeStock, utilityPriceForToolMethod, indexOfExtraMaterialForToolMethod, myActiveHints);
                AddToolUpgradeToStock("Hoe", blacksmithUpgradeStock, utilityPriceForToolMethod, indexOfExtraMaterialForToolMethod, myActiveHints);
                AddToolUpgradeToStock("Trash Can", blacksmithUpgradeStock, utilityPriceForToolMethod, indexOfExtraMaterialForToolMethod, myActiveHints);

                Game1.activeClickableMenu = new ShopMenu(blacksmithUpgradeStock, who: "ClintUpgrade");

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AnswerDialogueAction_ToolUpgrade_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void AddToolUpgradeToStock(string toolGenericName, Dictionary<ISalable, int[]> blacksmithUpgradeStock, 
            IReflectedMethod utilityPriceForToolMethod, IReflectedMethod indexOfExtraMaterialForToolMethod, Hint[] myActiveHints)
        {
            var priceMultiplier = _archipelago.SlotData.ToolPriceMultiplier;
            for (var upgradeLevel = 1; upgradeLevel < 5; upgradeLevel++)
            {
                if (!ShouldShowToolUpgradeInShop(toolGenericName, upgradeLevel))
                {
                    continue;
                }

                var metalName = GetMetalNameForTier(upgradeLevel);
                var locationName = $"{metalName} {toolGenericName} Upgrade";

                var toolApLocation = new PurchaseableArchipelagoLocation(locationName, _modHelper, _locationChecker, _archipelago, myActiveHints, () => Game1.playSound("parry"));

                var priceForUpgrade = utilityPriceForToolMethod.Invoke<int>(upgradeLevel);
                if (toolGenericName == "Trash Can")
                {
                    priceForUpgrade /= 2;
                }

                priceForUpgrade = (int)(priceForUpgrade * priceMultiplier);

                blacksmithUpgradeStock.Add(toolApLocation, new int[4]
                {
                    priceForUpgrade,
                    1,
                    indexOfExtraMaterialForToolMethod.Invoke<int>(upgradeLevel),
                    (int)(5 * priceMultiplier),
                });
            }
        }

        // public static Dictionary<ISalable, int[]> getBlacksmithUpgradeStock(Farmer who)
        public static void GetBlacksmithUpgradeStock_PriceReductionFromAp_Postfix(Farmer who, ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                var priceMultiplier = _archipelago.SlotData.ToolPriceMultiplier;
                foreach (var resultKey in __result.Keys.ToArray())
                {
                    var prices = __result[resultKey];
                    var newPrices = new int[4]
                    {
                        (int)(prices[0] * priceMultiplier),
                        prices[1],
                        prices[2],
                        (int)(5 * priceMultiplier),
                    };
                    __result[resultKey] = newPrices;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetBlacksmithUpgradeStock_PriceReductionFromAp_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static bool ShouldShowToolUpgradeInShop(string toolGenericName, int upgradeLevel)
        {
            var metalName = GetMetalNameForTier(upgradeLevel);
            if (_locationChecker.IsLocationChecked($"{metalName} {toolGenericName} Upgrade"))
            {
                return false;
            }

            var progressiveToolItemName = $"{VanillaUnlockManager.PROGRESSIVE_TOOL_AP_PREFIX}{toolGenericName}";
            var receivedToolsOfThatType = _archipelago.GetReceivedItemCount(progressiveToolItemName);
            return receivedToolsOfThatType >= upgradeLevel - 1;
        }

        private static string GetMetalNameForTier(int upgradeLevel)
        {
            switch (upgradeLevel)
            {
                case 0:
                    return "Basic";
                case 1:
                    return "Copper";
                case 2:
                    return "Iron";
                case 3:
                    return "Gold";
                case 4:
                    return "Iridium";
                default:
                    throw new ArgumentException($"Tier {upgradeLevel} is not a value upgrade level for a tool");
            }
        }
    }
}
