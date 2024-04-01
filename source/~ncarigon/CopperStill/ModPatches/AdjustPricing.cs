/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using ProducerFrameworkMod.Controllers;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CopperStill.ModPatches {
    internal static class AdjustPricing {
        public static void Register(IModHelper helper, IMonitor monitor) {
            helper.Events.GameLoop.SaveLoaded += (s, e) => {
                var defaultPrices = new Dictionary<string, int>() {
                    // these are crops we need to reference later for direct price changes
                    { "Cactus Fruit", 75 },
                    { "Corn", 50 },
                    { "Beet", 100 },
                    { "Unmilled Rice", 30 },
                    { "Wheat", 25 },
                    { "Blackberry", -1 },
                    { "Wine", -1 },
                    { "Juice", -1 },
                    // the rest are crops with unbalanced prices we can assume would be modified
                    { "Starfruit", 750 },
                    { "Ancient Fruit", 550 },
                    { "Pineapple", 300 },
                    { "Pumpkin", 320 },
                    { "Coffee Bean", 15 }
                };
                var prices = new Dictionary<string, int>();
                var numDefault = 0;
                foreach (var item in Game1.objectData) {
                    if (defaultPrices.TryGetValue(item.Value.Name, out var price)) {
                        if (price == item.Value.Price) {
                            numDefault++;
                        }
                        prices[item.Value.Name] = item.Value.Price;
                    }
                }
                // only detect "balanced" prices if the majority of unbalanced crops are no longer default values.
                var isBalanced = numDefault < 5;
                monitor.Log($"Detected {(isBalanced ? "balanced" : "default")} prices, adjusting accordingly.", LogLevel.Debug);
                // adjust some internal item spawn prices, just to keep things consistent
                foreach (var key in Game1.objectData.Keys.ToArray()) {
                    switch (key) {
                        case "Juniper Berry":
                            Game1.objectData[key].Price = prices["Blackberry"] + 10;
                            break;
                        case "Tequila Blanco":
                            Game1.objectData[key].Price = prices["Cactus Fruit"].Multi(3).Multi(4.5);
                            break;
                        case "Tequila Anejo":
                            Game1.objectData[key].Price = prices["Cactus Fruit"].Multi(3).Multi(4.5).Multi(3);
                            break;
                        case "Moonshine":
                            Game1.objectData[key].Price = prices["Corn"].Multi(2.25).Multi(isBalanced ? 4.5 : 15);
                            break;
                        case "Whiskey":
                            Game1.objectData[key].Price = prices["Corn"].Multi(2.25).Multi(isBalanced ? 4.5 : 15).Multi(3);
                            break;
                        case "Vodka":
                            Game1.objectData[key].Price = prices["Juice"].Multi(isBalanced ? 4.5 : 12);
                            break;
                        case "Gin":
                            Game1.objectData[key].Price = prices["Juice"].Multi(isBalanced ? 4.5 : 12).Multi(1.5);
                            break;
                        case "Brandy":
                            Game1.objectData[key].Price = prices["Wine"].Multi(4.5);
                            break;
                        case "White Rum":
                            Game1.objectData[key].Price = prices["Beet"].Multi(2.25).Multi(isBalanced ? 4.5 : 12);
                            break;
                        case "Dark Rum":
                            Game1.objectData[key].Price = prices["Beet"].Multi(2.25).Multi(isBalanced ? 4.5 : 12).Multi(3);
                            break;
                        case "Sake":
                            Game1.objectData[key].Price = prices["Unmilled Rice"].Multi(isBalanced ? 2.25 : 12);
                            break;
                        case "Soju":
                            Game1.objectData[key].Price = prices["Unmilled Rice"].Multi(isBalanced ? 2.25 : 12).Multi(4.5);
                            break;
                    }
                }

                // adjust dynamic recipes
                foreach (var rule in ProducerController.GetProducerRules().ToArray()) {
                    if (string.Compare(rule?.ProducerName, "Still") == 0
                        && string.Compare(rule!.OutputIdentifier, "Vodka") == 0
                    ) {
                        rule.OutputPriceMultiplier = isBalanced ? 4.5 : 12;
                        foreach (var add in rule.AdditionalOutputs) {
                            switch (add.OutputIdentifier) {
                                case "Moonshine":
                                    add.OutputPriceMultiplier = isBalanced ? 4.5 : 15;
                                    break;
                                case "White Rum":
                                    add.OutputPriceMultiplier = isBalanced ? 4.5 : 8;
                                    break;
                            }
                        }
                        ProducerController.AddProducerItems(rule, null, "NCarigon.CopperStillPFM");
                        break;
                    }
                }
            };
        }

        private static int Multi(this int i, double d) => (int)(i * d);
    }
}
