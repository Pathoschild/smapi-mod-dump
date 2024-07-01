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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewArchipelago.GameModifications
{
    public class RecipeDataRemover
    {
        private IMonitor _monitor;
        private IModHelper _helper;
        private ArchipelagoClient _archipelago;

        public RecipeDataRemover(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
        }

        public void OnCookingRecipesRequested(object sender, AssetRequestedEventArgs e)
        {
            if (_archipelago.SlotData.Chefsanity == Chefsanity.Vanilla)
            {
                return;
            }

            if (!e.NameWithoutLocale.IsEquivalentTo("Data/CookingRecipes"))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var cookingRecipesData = asset.AsDictionary<string, string>().Data;
                    RemoveObsoleteCookingLearnConditions(cookingRecipesData);
                },
                AssetEditPriority.Late
            );
        }

        public void OnCraftingRecipesRequested(object sender, AssetRequestedEventArgs e)
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            if (!e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                return;
            }

            e.Edit(asset =>
                {
                    var craftingRecipesData = asset.AsDictionary<string, string>().Data;
                    RemoveObsoleteCraftingLearnConditions(craftingRecipesData);
                },
                AssetEditPriority.Late
            );
        }

        private void RemoveObsoleteCookingLearnConditions(IDictionary<string, string> cookingRecipesData)
        {
            RemoveCookingRecipesStarterLearnConditions(cookingRecipesData);
            RemoveCookingRecipesSkillsLearnConditions(cookingRecipesData);
            RemoveCookingRecipesFriendshipLearnConditions(cookingRecipesData);
        }

        private void RemoveObsoleteCraftingLearnConditions(IDictionary<string, string> craftingRecipesData)
        {
            RemoveCraftingRecipesStarterLearnConditions(craftingRecipesData);
            RemoveCraftingRecipesSkillsLearnConditions(craftingRecipesData);
            RemoveCraftingRecipesFriendshipLearnConditions(craftingRecipesData);
        }

        private void RemoveCookingRecipesStarterLearnConditions(IDictionary<string, string> cookingRecipesData)
        {
            if (_archipelago.SlotData.Chefsanity == Chefsanity.Vanilla)
            {
                return;
            }

            foreach (var recipeName in cookingRecipesData.Keys.ToArray())
            {
                var recipeData = cookingRecipesData[recipeName];
                var recipeUnlockCondition = GetCookingRecipeUnlockCondition(recipeData);
                if (!recipeUnlockCondition.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, "none");
                cookingRecipesData[recipeName] = modifiedRecipe;
                foreach (var farmer in Game1.getAllFarmers())
                {
                    if (farmer.cookingRecipes.ContainsKey(recipeName) && !_archipelago.HasReceivedItem($"{recipeName} Recipe"))
                    {
                        farmer.cookingRecipes.Remove(recipeName);
                    }
                }
            }
        }

        private void RemoveCookingRecipesSkillsLearnConditions(IDictionary<string, string> cookingRecipesData)
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Skills))
            {
                return;
            }

            foreach (var recipeName in cookingRecipesData.Keys.ToArray())
            {
                var recipeData = cookingRecipesData[recipeName];
                var recipeUnlockCondition = GetCookingRecipeUnlockCondition(recipeData);
                var unlockConditionParts = recipeUnlockCondition.Split(" ");
                if (unlockConditionParts.Length < 3 || unlockConditionParts[0] != "s")
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, "none");
                cookingRecipesData[recipeName] = modifiedRecipe;
            }
        }

        private void RemoveCookingRecipesFriendshipLearnConditions(IDictionary<string, string> cookingRecipesData)
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship))
            {
                return;
            }

            foreach (var recipeName in cookingRecipesData.Keys.ToArray())
            {
                var recipeData = cookingRecipesData[recipeName];
                var recipeUnlockCondition = GetCookingRecipeUnlockCondition(recipeData);
                var unlockConditionParts = recipeUnlockCondition.Split(" ");
                if (unlockConditionParts.Length < 3 || unlockConditionParts[0] != "f")
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, $"none:{recipeUnlockCondition}");
                cookingRecipesData[recipeName] = modifiedRecipe;
                var npcName = unlockConditionParts[1];
                Game1.player.RemoveMail($"{npcName}Cooking", true);
            }
        }

        private void RemoveCraftingRecipesStarterLearnConditions(IDictionary<string, string> craftingRecipesData)
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            foreach (var recipeName in craftingRecipesData.Keys.ToArray())
            {
                var recipeData = craftingRecipesData[recipeName];
                var recipeUnlockCondition = GetCraftingRecipeUnlockCondition(recipeData);
                if (!recipeUnlockCondition.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, "none");
                craftingRecipesData[recipeName] = modifiedRecipe;
                foreach (var farmer in Game1.getAllFarmers())
                {
                    if (farmer.craftingRecipes.ContainsKey(recipeName) && !_archipelago.HasReceivedItem($"{recipeName} Recipe"))
                    {
                        farmer.craftingRecipes.Remove(recipeName);
                    }
                }
            }
        }

        private void RemoveCraftingRecipesSkillsLearnConditions(IDictionary<string, string> craftingRecipesData)
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            // This is not necessary, we let players learn recipes through level ups
        }

        private void RemoveCraftingRecipesFriendshipLearnConditions(IDictionary<string, string> craftingRecipesData)
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            // This is not necessary, we let players learn recipes through friendships
        }

        private static string GetCookingRecipeUnlockCondition(string recipeData)
        {
            return GetRecipeUnlockCondition(recipeData, 3);
        }

        private static string GetCraftingRecipeUnlockCondition(string recipeData)
        {
            return GetRecipeUnlockCondition(recipeData, 4);
        }

        private static string GetRecipeUnlockCondition(string recipeData, int fieldIndex)
        {
            var recipeFields = recipeData.Split("/");
            var recipeUnlockCondition = recipeFields[fieldIndex];
            return recipeUnlockCondition;
        }
    }
}
