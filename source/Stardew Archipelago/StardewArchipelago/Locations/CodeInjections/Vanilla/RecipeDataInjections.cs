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
using StardewArchipelago.Extensions;
using StardewArchipelago.GameModifications.Seasons;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class RecipeDataInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public static void InitShared()
        public static void InitShared_RemoveSkillAndFriendshipLearnConditions_Postfix()
        {
            try
            {
                RemoveCookingRecipeLearnConditions();
                // RemoveCraftingRecipeLearnConditions();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(InitShared_RemoveSkillAndFriendshipLearnConditions_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static void RemoveCookingRecipeLearnConditions()
        {
            RemoveCookingRecipesSkillsLearnConditions();
            RemoveCookingRecipesFriendshipLearnConditions();
        }

        private static void RemoveCraftingRecipeLearnConditions()
        {
            RemoveCraftingRecipesSkillsLearnConditions();
            RemoveCraftingRecipesFriendshipLearnConditions();
        }

        private static void RemoveCookingRecipesSkillsLearnConditions()
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Skills))
            {
                return;
            }

            foreach (var recipeName in CraftingRecipe.cookingRecipes.Keys.ToArray())
            {
                var recipeData = CraftingRecipe.cookingRecipes[recipeName];
                var recipeFields = recipeData.Split("/");
                var recipeUnlockCondition = recipeFields[3];
                var unlockConditionParts = recipeUnlockCondition.Split(" ");
                if (unlockConditionParts.Length < 2 || (unlockConditionParts.Length >= 3 && unlockConditionParts[0] != "s"))
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, "none");
                CraftingRecipe.cookingRecipes[recipeName] = modifiedRecipe;
            }
        }

        private static void RemoveCookingRecipesFriendshipLearnConditions()
        {
            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship))
            {
                return;
            }

            foreach (var recipeName in CraftingRecipe.cookingRecipes.Keys.ToArray())
            {
                var recipeData = CraftingRecipe.cookingRecipes[recipeName];
                var recipeFields = recipeData.Split("/");
                var recipeUnlockCondition = recipeFields[3];
                var unlockConditionParts = recipeUnlockCondition.Split(" ");
                if (unlockConditionParts.Length < 3 || unlockConditionParts[0] != "f")
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, "none");
                CraftingRecipe.cookingRecipes[recipeName] = modifiedRecipe;
            }
        }

        private static void RemoveCraftingRecipesSkillsLearnConditions()
        {
            throw new NotImplementedException();
        }

        private static void RemoveCraftingRecipesFriendshipLearnConditions()
        {
            throw new NotImplementedException();
        }
    }
}
