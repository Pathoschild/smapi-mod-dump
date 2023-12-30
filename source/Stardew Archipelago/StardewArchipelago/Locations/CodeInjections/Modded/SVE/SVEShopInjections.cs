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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.GameModifications;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace StardewArchipelago.Locations.CodeInjections.Modded.SVE
{
    public class SVEShopInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static ShopReplacer _shopReplacer;
        private static ShopStockGenerator _shopStockGenerator;
        private const string ALESIA_DAGGER = "Tempered Galaxy Dagger";
        private const string ISAAC_SWORD = "Tempered Galaxy Sword";
        private const string ISAAC_HAMMER = "Tempered Galaxy Hammer";

        private static readonly Dictionary<ShopIdentification, PricedItem[]> craftsanityRecipes = new()
        {
            { new ShopIdentification("Custom_CastleVillageOutpost", "Alesia"), new[] { new PricedItem("Haste Elixir", 35000), new PricedItem("Armor Elixir", 50000) } },
            { new ShopIdentification("Custom_CastleVillageOutpost", "Isaac"), new[] { new PricedItem("Hero Elixir", 65000) } },
        };

        private static readonly Dictionary<ShopIdentification, PricedItem[]> chefsanityRecipes = new()
        {
            {
                new ShopIdentification("Saloon"),
                new[] { new PricedItem("Big Bark Burger", 5500), new PricedItem("Glazed Butterfish", 4000), new PricedItem("Mixed Berry Pie", 3500) }
            },
            { new ShopIdentification("Custom_ForestWest"), new[] { new PricedItem("Baked Berry Oatmeal", 12500), new PricedItem("Flower Cookie", 8750) } },
            { new ShopIdentification("AdventureGuild"), new[] { new PricedItem("Frog Legs", 2000), new PricedItem("Mushroom Berry Rice", 1500) } },
            { new ShopIdentification("FishShop"), new[] { new PricedItem("Seaweed Salad", 1250) } },
            { new ShopIdentification("Sewer"), new[] { new PricedItem("Void Delight", 5000), new PricedItem("Void Salmon Sushi", 5000) } },
            {
                new ShopIdentification("ResortBar"),
                new[] { new PricedItem("Big Bark Burger", 5500), new PricedItem("Glazed Butterfish", 4000), new PricedItem("Mixed Berry Pie", 3500) }
            },
        };

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago,
            LocationChecker locationChecker, ShopReplacer shopReplacer, ShopStockGenerator shopStockGenerator)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _shopReplacer = shopReplacer;
            _shopStockGenerator = shopStockGenerator;
        }

        private static ShopMenu _lastShopMenuUpdated = null;

        // public override void update(GameTime time)
        public static void Update_ReplaceSVEShopChecks_Postfix(ShopMenu __instance, GameTime time)
        {
            try
            {
                // We only run this once for each menu
                if (_lastShopMenuUpdated == __instance)
                {
                    return;
                }

                _lastShopMenuUpdated = __instance;

                RemoveSaloonRecipesFromPhone(__instance);

                var myActiveHints = _archipelago.GetMyActiveHints();
                foreach (var salableItem in __instance.itemPriceAndStock.Keys.ToArray())
                {
                    ReplaceTemperedGalaxyWeapons(__instance, salableItem, myActiveHints);
                }

                ReplaceCraftsanityRecipes(__instance, myActiveHints);
                ReplaceChefsanityRecipes(__instance, myActiveHints);

                __instance.forSale = __instance.itemPriceAndStock.Keys.ToList();
                return; //  run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Update_ReplaceSVEShopChecks_Postfix)}:\n{ex}", LogLevel.Error);
                return; // run original logic
            }
        }

        private static void RemoveSaloonRecipesFromPhone(ShopMenu shopMenu)
        {
            if (shopMenu.storeContext == "Saloon")
            {
                return;
            }

            foreach (var salable in shopMenu.itemPriceAndStock.Keys.ToArray())
            {
                if (salable.Name.Contains("Frog Leg"))
                {
                    shopMenu.itemPriceAndStock.Remove(salable);
                }
            }
        }

        private static void ReplaceTemperedGalaxyWeapons(ShopMenu shopMenu, ISalable salableItem, Hint[] myActiveHints)
        {
            _shopReplacer.ReplaceShopItem(shopMenu.itemPriceAndStock, salableItem, ALESIA_DAGGER, "Tempered Galaxy Dagger", myActiveHints);
            _shopReplacer.ReplaceShopItem(shopMenu.itemPriceAndStock, salableItem, ISAAC_SWORD, "Tempered Galaxy Sword", myActiveHints);
            _shopReplacer.ReplaceShopItem(shopMenu.itemPriceAndStock, salableItem, ISAAC_HAMMER, "Tempered Galaxy Hammer", myActiveHints);
        }

        private static void ReplaceCraftsanityRecipes(ShopMenu shopMenu, Hint[] myActiveHints)
        {
            if (!_archipelago.SlotData.Craftsanity.HasFlag(Craftsanity.All))
            {
                return;
            }

            foreach (var (shopIdentification, recipes) in craftsanityRecipes)
            {
                if (!shopIdentification.IsCorrectShop(shopMenu))
                {
                    continue;
                }

                foreach (var recipe in recipes)
                {
                    _shopReplacer.PlaceShopRecipeCheck(shopMenu.itemPriceAndStock, $"{recipe.ItemName} Recipe", recipe.ItemName, myActiveHints, recipe.Price);
                }
            }
        }

        private static void ReplaceChefsanityRecipes(ShopMenu shopMenu, Hint[] myActiveHints)
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Purchases))
            {
                return;
            }

            foreach (var (shopIdentification, recipes) in chefsanityRecipes)
            {
                if (!shopIdentification.IsCorrectShop(shopMenu))
                {
                    continue;
                }

                foreach (var recipe in recipes)
                {
                    _shopReplacer.PlaceShopRecipeCheck(shopMenu.itemPriceAndStock, $"{recipe.ItemName} Recipe", recipe.ItemName, myActiveHints, recipe.Price);
                }
            }
        }


        // Done as JojaMart was changed to be two different shop tenders (Claire and Martin); just force every shop in Joja to be the same.
        // public ShopMenu(Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null, Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
        public static bool Constructor_MakeBothJojaShopsTheSame_Prefix(ShopMenu __instance, ref Dictionary<ISalable, int[]> itemPriceAndStock, int currency, string who,
            Func<ISalable, Farmer, int, bool> on_purchase, Func<ISalable, bool> on_sell, string context)
        {
            try
            {
                if (Game1.currentLocation is not JojaMart)
                {
                    return true; // Run original logic
                }

                itemPriceAndStock = _shopStockGenerator.GetJojaStock();
                return true; // run original logic

            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Constructor_MakeBothJojaShopsTheSame_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic   
            }
        }
    }
}