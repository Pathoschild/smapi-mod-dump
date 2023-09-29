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
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class CraftingInjections
    {
        private const string CRAFTING_LOCATION_PREFIX = "Craft ";

        private static Dictionary<string, int> _carpenterRecipes = new()
        {
            {"Wooden Brazier", 2500},
            {"Stone Brazier", 4000},
            {"Barrel Brazier", 8000},
            {"Stump Brazier", 8000},
            {"Gold Brazier", 10000},
            {"Carved Brazier", 20000},
            {"Skull Brazier", 30000},
            {"Marble Brazier", 50000},
            {"Wood Lamp-post", 5000},
            {"Iron Lamp-post", 10000},
            {"Wood Floor", 1000},
            {"Rustic Plank Floor", 2000},
            {"Stone Floor", 1000},
            {"Brick Floor", 5000},
            {"Stone Walkway Floor", 2000},
            {"Stepping Stone Path", 1000},
            {"Straw Floor", 2000},
            {"Crystal Path", 2000},
        };

        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static StardewItemManager _itemManager;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _itemManager = itemManager;
        }

        // public void checkForCraftingAchievements()
        public static void CheckForCraftingAchievements_CheckCraftsanityLocation_Postfix(Stats __instance)
        {
            try
            {
                var craftedRecipes = Game1.player.craftingRecipes;
                foreach (var recipe in craftedRecipes.Keys)
                {
                    if (craftedRecipes[recipe] <= 0)
                    {
                        continue;
                    }

                    var location = $"{CRAFTING_LOCATION_PREFIX}{recipe}";
                    _locationChecker.AddCheckedLocation(location);
                }
                
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForCraftingAchievements_CheckCraftsanityLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static Dictionary<ISalable, int[]> getCarpenterStock()
        public static void GetCarpenterStock_PurchasableRecipeChecks_Postfix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                RemoveRecipesFromStock(__result);

                var priceMultiplier = _archipelago.SlotData.BuildingPriceMultiplier;
                AddRecipeChecksToStock(__result, _carpenterRecipes, priceMultiplier);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetCarpenterStock_PurchasableRecipeChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public static Dictionary<ISalable, int[]> getDwarfShopStock()
        public static void GetDwarfShopStock_PurchasableRecipeChecks_Postfix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                RemoveRecipesFromStock(__result);

                var activeHints = _archipelago.GetMyActiveHints();
                AddRecipeCheckToStock(__result, "Weathered Floor", 5000, activeHints);

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetDwarfShopStock_PurchasableRecipeChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public Dictionary<ISalable, int[]> getShadowShopStock()
        public static void GetShadowShopStock_PurchasableRecipeChecks_Postfix(ref Dictionary<ISalable, int[]> __result)
        {
            try
            {
                RemoveRecipesFromStock(__result);

                var recipes = new Dictionary<string, int>()
                {
                    { "Crystal Floor", 5000 },
                    { "Wicked Statue", 10000 },
                };

                AddRecipeChecksToStock(__result, recipes);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetShadowShopStock_PurchasableRecipeChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void RemoveRecipesFromStock(Dictionary<ISalable, int[]> stock)
        {
            foreach (var salable in stock.Keys.ToArray())
            {
                if (salable is not Object { IsRecipe: true })
                {
                    continue;
                }

                stock.Remove(salable);
            }
        }

        private static void AddRecipeChecksToStock(Dictionary<ISalable, int[]> stock, IDictionary<string, int> recipes, double priceMultiplier = 1.0)
        {
            var activeHints = _archipelago.GetMyActiveHints();
            foreach (var (recipeName, recipePrice) in recipes)
            {
                var price = recipePrice * priceMultiplier;
                AddRecipeCheckToStock(stock, recipeName, (int)price, activeHints);
            }
        }

        private static void AddRecipeCheckToStock(Dictionary<ISalable, int[]> stock, string recipeName, int price, Hint[] activeHints)
        {
            var apLocation = $"{recipeName} Recipe";
            if (!_locationChecker.IsLocationMissingAndExists(apLocation))
            {
                return;
            }

            var purchasableCheck = new PurchaseableArchipelagoLocation(recipeName, apLocation, _helper, _locationChecker, _archipelago, activeHints);
            stock.Add(purchasableCheck, new[] { price, 1 });
        }
    }
}
