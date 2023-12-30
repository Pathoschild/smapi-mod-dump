/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Linq;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
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

        // public static void InitShared()
        public void RemoveSkillAndFriendshipLearnConditions()
        {
            RemoveCookingRecipeLearnConditions();
            RemoveCraftingRecipeLearnConditions();
        }

        private void RemoveCookingRecipeLearnConditions()
        {
            RemoveCookingRecipesSkillsLearnConditions();
            RemoveCookingRecipesFriendshipLearnConditions();
        }

        private void RemoveCraftingRecipeLearnConditions()
        {
            RemoveCraftingRecipesSkillsLearnConditions();
            RemoveCraftingRecipesFriendshipLearnConditions();
        }

        private void RemoveCookingRecipesSkillsLearnConditions()
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
                if (unlockConditionParts.Length < 2 || unlockConditionParts.Length >= 3 && unlockConditionParts[0] != "s")
                {
                    continue;
                }

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, "none");
                CraftingRecipe.cookingRecipes[recipeName] = modifiedRecipe;
            }
        }

        private void RemoveCookingRecipesFriendshipLearnConditions()
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

                var modifiedRecipe = recipeData.Replace(recipeUnlockCondition, $"none:{recipeUnlockCondition}");
                CraftingRecipe.cookingRecipes[recipeName] = modifiedRecipe;
            }
        }

        private void RemoveCraftingRecipesSkillsLearnConditions()
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            // This is not necessary, we let players learn recipes through level ups
        }

        private void RemoveCraftingRecipesFriendshipLearnConditions()
        {
            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            // This is not necessary, we let players learn recipes through friendships
        }
    }
}
