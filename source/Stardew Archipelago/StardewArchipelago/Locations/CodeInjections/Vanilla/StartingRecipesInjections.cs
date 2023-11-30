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
using Netcode;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class StartingRecipesInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static StardewItemManager _itemManager;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _itemManager = itemManager;
        }

        // private void farmerInit()
        public static void FarmerInit_RemoveStartingRecipes_Postfix(Farmer __instance)
        {
            try
            {
                SynchronizeStartingCraftingRecipesWithArchipelago(__instance);
                SynchronizeStartingCookingRecipesWithArchipelago(__instance);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(FarmerInit_RemoveStartingRecipes_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void SynchronizeStartingCraftingRecipesWithArchipelago(Farmer __instance)
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }
            var startingCraftingRecipes = new[]
            {
                "Chest",
                "Wood Fence",
                "Gate",
                "Torch",
                "Campfire",
                "Wood Path",
                "Cobblestone Path",
                "Gravel Path",
                "Wood Sign",
                "Stone Sign",
            };
            SynchronizeStartingRecipesWithArchipelago(startingCraftingRecipes, __instance.craftingRecipes);
        }

        private static void SynchronizeStartingCookingRecipesWithArchipelago(Farmer __instance)
        {
            if (_archipelago.SlotData.Chefsanity == Chefsanity.Vanilla)
            {
                return;
            }
            var startingCookingRecipes = new[]
            {
                "Fried Egg",
            };
            SynchronizeStartingRecipesWithArchipelago(startingCookingRecipes, __instance.cookingRecipes);
        }

        private static void SynchronizeStartingRecipesWithArchipelago(string[] startingRecipes, NetStringDictionary<int, NetInt> knownRecipes)
        {
            foreach (var startingRecipe in startingRecipes)
            {
                SynchronizeStartingRecipeWithArchipelago(knownRecipes, startingRecipe);
            }
        }

        private static void SynchronizeStartingRecipeWithArchipelago(NetStringDictionary<int, NetInt> knownRecipes, string startingRecipe)
        {
            var knowsRecipe = knownRecipes.ContainsKey(startingRecipe);
            var shouldKnowRecipe = _archipelago.HasReceivedItem($"{startingRecipe} Recipe");
            if (knowsRecipe == shouldKnowRecipe)
            {
                return;
            }

            if (shouldKnowRecipe)
            {
                knownRecipes.Add(startingRecipe, 0);
            }
            else
            {
                knownRecipes.Remove(startingRecipe);
            }
        }
    }
}
