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
using System;
using ProducerFrameworkMod.ContentPack;

namespace CopperStill.ModPatches {
    internal static class AdjustPricing {
        public static void Register(IModHelper helper) {
            helper.Events.GameLoop.SaveLoaded += (s, e) => {
                var prices = new Dictionary<string, int>();
                var numDefault = 0;
                foreach (var item in Game1.objectInformation) {
                    var parts = item.Value.Split('/');
                    switch (parts[0]) {
                        // these are crops we need to reference later for direct price changes
                        case "Cactus Fruit":
                            if (parts[1] == "75")
                                numDefault++;
                            if (int.TryParse(parts[1], out var price))
                                prices[parts[0]] = price;
                            break;
                        case "Corn":
                            if (parts[1] == "50")
                                numDefault++;
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        case "Beet":
                            if (parts[1] == "100")
                                numDefault++;
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        case "Unmilled Rice":
                            if (parts[1] == "30")
                                numDefault++;
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        case "Wheat":
                            if (parts[1] == "25")
                                numDefault++;
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        case "Blackberry":
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        case "Wine":
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        case "Juice":
                            if (int.TryParse(parts[1], out price))
                                prices[parts[0]] = price;
                            break;
                        // the rest are crops with unbalanced prices we can assume would be modified
                        case "Starfruit":
                            if (parts[1] == "750")
                                numDefault++;
                            break;
                        case "Ancient Fruit":
                            if (parts[1] == "550")
                                numDefault++;
                            break;
                        case "Pineapple":
                            if (parts[1] == "300")
                                numDefault++;
                            break;
                        case "Pumpkin":
                            if (parts[1] == "320")
                                numDefault++;
                            break;
                        case "Coffee Bean":
                            if (parts[1] == "15")
                                numDefault++;
                            break;
                    }
                }
                // only detect "balanced" prices if the majority of unbalanced crops are no longer default values.
                var isBalanced = numDefault < 5;
                // adjust some internal item spawn prices, just to keep things consistent
                foreach (var key in Game1.objectInformation.Keys.ToArray()) {
                    var parts = Game1.objectInformation[key].Split('/');
                    switch (parts[0]) {
                        case "Juniper Berry":
                            parts[1] = (prices["Blackberry"] + 10).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Tequila Blanco":
                            parts[1] = prices["Cactus Fruit"].Multi(3).Multi(4.5).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Tequila Anejo":
                            parts[1] = prices["Cactus Fruit"].Multi(3).Multi(4.5).Multi(3).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Moonshine":
                            parts[1] = prices["Corn"].Multi(2.25).Multi(isBalanced ? 4.5 : 15).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Whiskey":
                            parts[1] = prices["Corn"].Multi(2.25).Multi(isBalanced ? 4.5 : 15).Multi(3).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Vodka":
                            parts[1] = prices["Juice"].Multi(isBalanced ? 4.5 : 12).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Gin":
                            parts[1] = prices["Juice"].Multi(isBalanced ? 4.5 : 12).Multi(2).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Brandy":
                            parts[1] = prices["Wine"].Multi(4.5).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "White Rum":
                            parts[1] = prices["Beet"].Multi(2.25).Multi(isBalanced ? 4.5 : 12).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Dark Rum":
                            parts[1] = prices["Beet"].Multi(2.25).Multi(isBalanced ? 4.5 : 12).Multi(3).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Sake":
                            parts[1] = prices["Unmilled Rice"].Multi(isBalanced ? 2.25 : 12).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                        case "Soju":
                            parts[1] = prices["Unmilled Rice"].Multi(isBalanced ? 2.25 : 12).Multi(4.5).ToString();
                            Game1.objectInformation[key] = string.Join('/', parts);
                            break;
                    }
                }

                var rules = helper.Data.ReadJsonFile<ProducerRule[]>("DynamicProducerRules.json") ?? Array.Empty<ProducerRule>();
                foreach (var rule in rules) {
                    if (rule.ProducerName == "Still") {
                        if (rule.OutputIdentifier == "Vodka") {
                            // 12 seems to be a reasonable multiplier for unspecified juice >> vodka
                            rule.OutputPriceMultiplier = isBalanced ? 4.5 : 12;
                            foreach (var add in rule.AdditionalOutputs) {
                                if (add.OutputIdentifier == "Moonshine") {
                                    // moonshine and whiskey need a little more bump, so 15 works better
                                    add.OutputPriceMultiplier = isBalanced ? 4.5 : 15;
                                } else if (add.OutputIdentifier == "White Rum") {
                                    // rum gets a bit too pricey, so 8 is better
                                    add.OutputPriceMultiplier = isBalanced ? 4.5 : 8;
                                }
                            }
                            ProducerController.AddProducerItems(rule, null, "NCarigon.CopperStillPFM");
                        }
                    }
                }
            };
        }

        private static int Multi(this int i, double d) => (int)(i * d);
    }
}
