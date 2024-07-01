/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/

using StardewValley;

namespace WhatDoYouWant
{
    internal class RecipeData
    {
        public string? RecipeKey { get; set; }
        public string? RecipeName { get; set; }
        public string? RecipeIngredients { get; set; }
        public bool RecipeLearned { get; set; }
        public string? RecipeCondition { get; set; }
        public string? TextureName { get; set; } // Collections Tab sort option
        public int SpriteIndex { get; set; } // Collections Tab sort option
    }

    internal class Cooking
    {
        public const string SortOrder_KnownRecipesFirst = "KnownRecipesFirst";
        public const string SortOrder_RecipeName = "RecipeName";
        public const string SortOrder_CollectionsTab = "CollectionsTab";

        public const string CookingIngredient_AnyMilk = "-6"; // hardcoded instead of StardewValley.Object.MilkCategory.ToString() so other code can switch() on it
        public const string CookingIngredient_AnyEgg = "-5"; // StardewValley.Object.EggCategory
        public const string CookingIngredient_AnyFish = "-4"; // StardewValley.Object.FishCategory

        public static void ShowCookingList(ModEntry modInstance, Farmer who)
        {
            // adapted from base game logic to calculate cooking %
            var recipeList = new List<RecipeData>();
            foreach (var keyValuePair in DataLoader.CookingRecipes(Game1.content))
            {
                // keyValuePair = e.g. <"Fried Egg", "-5 1/10 10/194/default">
                // value = list of ingredient IDs and quantities / unused / item ID of cooked dish / unlock conditions
                var key1 = keyValuePair.Key;
                var key2 = ArgUtility.SplitBySpaceAndGet(ArgUtility.Get(keyValuePair.Value.Split('/'), 2), 0);
                var recipeLearned = who.cookingRecipes.ContainsKey(key1);
                var recipeCooked = who.recipesCooked.ContainsKey(key2);
                if (recipeLearned && recipeCooked)
                {
                    continue;
                }

                var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(key2);

                var ingredients = ArgUtility.Get(keyValuePair.Value.Split('/'), 0);
                var condition = ArgUtility.Get(keyValuePair.Value.Split('/'), 3);

                recipeList.Add(new RecipeData()
                {
                    RecipeKey = key1,
                    RecipeName = dataOrErrorItem.DisplayName,
                    RecipeIngredients = ModEntry.GetIngredientText(ingredients),
                    RecipeLearned = recipeLearned,
                    RecipeCondition = condition,
                    TextureName = dataOrErrorItem.TextureName,
                    SpriteIndex = dataOrErrorItem.SpriteIndex
                });
            }

            if (recipeList.Count == 0)
            {
                var completeDescription = modInstance.Helper.Translation.Get("Cooking_Complete", new { title = ModEntry.GetTitle_Cooking() });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            var sortByKnownRecipesFirst = (modInstance.Config.CookingSortOrder == SortOrder_KnownRecipesFirst);
            var sortByRecipeName = (modInstance.Config.CookingSortOrder == SortOrder_RecipeName);
            var sortByCollectionsTab = (modInstance.Config.CookingSortOrder == SortOrder_CollectionsTab);

            var notYetLearnedPrefix = modInstance.Helper.Translation.Get("Cooking_NotYetLearned");

            var linesToDisplay = new List<string>();
            foreach (var recipe in recipeList
                .OrderBy(entry => sortByCollectionsTab ? entry.TextureName : "")
                .ThenBy(entry => sortByCollectionsTab ? entry.SpriteIndex : 0)
                .ThenByDescending(entry => sortByKnownRecipesFirst ? entry.RecipeLearned : false)
                .ThenBy(entry => entry.RecipeName)
            )
            {
                var learnedPrefix = "";
                if (!recipe.RecipeLearned)
                {
                    var conditionDescription = modInstance.GetConditionDescription(recipe: recipe.RecipeKey, condition: recipe.RecipeCondition, isCooking: true);
                    learnedPrefix = $"{notYetLearnedPrefix} ({conditionDescription}) - ";
                }
                linesToDisplay.Add($"* {recipe.RecipeName} - {learnedPrefix}{recipe.RecipeIngredients}{ModEntry.LineBreak}");
            }

            modInstance.ShowLines(linesToDisplay, title: ModEntry.GetTitle_Cooking(), longerLinesExpected: true);
        }
    }
}
