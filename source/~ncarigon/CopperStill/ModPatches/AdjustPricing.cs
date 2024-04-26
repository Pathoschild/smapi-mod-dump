/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using ContentPatcher;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CopperStill.ModPatches {
    internal static class AdjustPricing {
        public static void Register() {
            if (ModEntry.Instance?.Helper is not null) {
                ModEntry.Instance.Helper.Events.GameLoop.GameLaunched += (_, _) =>
                    ModEntry.Instance.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher")?
                        .RegisterToken(ModEntry.Instance.ModManifest, "ArePricesBalanced",
                            () => new[] { ArePricesBalanced().ToString() }
                        );
                ModEntry.Instance.Helper.Events.GameLoop.SaveLoaded += (_, _) => {
                    ChangePrices();
                    ModEntry.Instance?.Helper.GameContent.InvalidateCache("Data/Machines");
                };
            }
        }

        public static int Multi(this int i, double d) => (int)(i * d);

        private static bool? IsBalanced = null;

        public static bool ArePricesBalanced() {
            if (!IsBalanced.HasValue) {
                var defaultPrices = new Dictionary<string, int>() {
                    // these are crops we need to reference later for direct price changes
                    { "Cactus Fruit", 75 },
                    { "Corn", 50 },
                    { "Beet", 100 },
                    { "Unmilled Rice", 30 },
                    { "Wheat", 25 },
                    // the rest are crops with unbalanced prices we can assume would be modified
                    { "Starfruit", 750 },
                    { "Ancient Fruit", 550 },
                    { "Pineapple", 300 },
                    { "Pumpkin", 320 },
                    { "Coffee Bean", 15 }
                };
                // only detect "balanced" prices if the majority of unbalanced crops are no longer default values.
                IsBalanced = 5 > Game1.objectData.Count(o => defaultPrices.TryGetValue(o.Value.Name, out var price) && o.Value.Price == price);
            }
            return IsBalanced.Value;
        }

        private static void ChangePrices() {
            IsBalanced = null;
            ArePricesBalanced();
            // adjust some internal item spawn prices, just to keep things consistent
            foreach (var obj in Game1.objectData.Select(o => new KeyValuePair<string, string>(o.Key, o.Value.Name)).ToArray()) {
                var price = CalcPrice(obj.Value);
                if (price > 0) {
                    Game1.objectData[obj.Key].Price = price;
                }
            }
            ModEntry.Instance?.Monitor?.Log($"Detected {((IsBalanced ?? false) ? "balanced" : "default")} prices, adjusted accordingly.", LogLevel.Debug);
        }

        public static int GetPrice(string ingredient) =>
            Game1.objectData.FirstOrDefault(o => o.Value.Name.Equals(ingredient)).Value?.Price ?? 0;

        public static int CalcPrice(string inputName, string? ingredient = null) {
            return inputName switch {
                "Juniper Berry" => GetPrice("Blackberry") + 10,
                "Tequila Blanco" => GetPrice("Cactus Fruit").Multi(3).Multi(4.5),
                "Tequila Anejo" => GetPrice("Cactus Fruit").Multi(3).Multi(4.5).Multi(3),
                "Moonshine" => GetPrice("Corn").Multi(2.25).Multi((IsBalanced ?? false) ? 4.5 : 15),
                "Whiskey" => GetPrice("Corn").Multi(2.25).Multi((IsBalanced ?? false) ? 4.5 : 15).Multi(3),
                "Vodka" => (ingredient is null ? GetPrice("Juice") : GetPrice(ingredient).Multi(2.25)).Multi((IsBalanced ?? false) ? 4.5 : 12),
                "Gin" => (ingredient is null ? GetPrice("Juice") : GetPrice(ingredient).Multi(2.25)).Multi((IsBalanced ?? false) ? 4.5 : 12).Multi(1.25),
                "Brandy" => (ingredient is null ? GetPrice("Wine") : GetPrice(ingredient).Multi(3)).Multi(4.5),
                "White Rum" => GetPrice("Beet").Multi(2.25).Multi((IsBalanced ?? false) ? 4.5 : 8),
                "Dark Rum" => GetPrice("Beet").Multi(2.25).Multi((IsBalanced ?? false) ? 4.5 : 8).Multi(3),
                "Sake" => GetPrice("Unmilled Rice").Multi((IsBalanced ?? false) ? 2.25 : 12),
                "Soju" => GetPrice("Unmilled Rice").Multi((IsBalanced ?? false) ? 2.25 : 12).Multi(4.5),
                _ => 0,
            };
        }
    }
}
