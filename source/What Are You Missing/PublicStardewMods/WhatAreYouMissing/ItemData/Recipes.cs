using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class Recipes : Items, ISpecificItems
    {
        private Dictionary<string, string> RawRecipeData;

        public Recipes() : base()
        {
            RawRecipeData = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            AddData();
        }
        public Dictionary<int, SObject> GetItems()
        {
            return items;
        }

        protected override void AddItems() { }

        private void AddData()
        {
            foreach(KeyValuePair<string, string> recipe in RawRecipeData)
            {
                //its possible that the recipe is "recipeIndex count" in some mods like 
                //animal husbandry so I need the second split
                int parentSheetIndex = ParseRecipeIndex(recipe.Value.Split('/')[2].Split(' ')[0]);

                if(ShouldRecipeBeAdded(recipe.Key))
                {
                    AddOneCommonObject(parentSheetIndex);
                }
            }
        }

        private int ParseRecipeIndex(string index)
        {
            bool successful = int.TryParse(index, out int intIndex);
            if (!successful)
            {
                ModEntry.Logger.LogRecipeIndexParseFail(index);
                return -1;
            }
            return intIndex;
        }

        private bool ShouldRecipeBeAdded(string key)
        {
            return ModEntry.modConfig.ShowAllRecipes || ModEntry.modConfig.AlwaysShowAllRecipes || Game1.player.cookingRecipes.ContainsKey(key);
        }
    }

    public class RecipeIngredients
    {
        //<Recipe ParentSheetIndex, <Ingredient ParentSheetIndex, Ingredient>>
        private Dictionary<int, Dictionary<int, SObject>> AllRecipeIngredients;

        public RecipeIngredients()
        {
            AllRecipeIngredients = new Dictionary<int, Dictionary<int, SObject>>();
            AddRecipes();
        }

        public Dictionary<int, Dictionary<int, SObject>> GetRecipeAndIngredients()
        {
            return AllRecipeIngredients;
        }

        public Dictionary<int, SObject> GetRecipeIngredients(int recipeParentSheetIndex)
        {
            if (AllRecipeIngredients.ContainsKey(recipeParentSheetIndex))
            {
                return AllRecipeIngredients[recipeParentSheetIndex];
            }
            return null;
        }

        private void AddRecipes()
        {
            Dictionary<string, string> RawRecipeData = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            foreach (KeyValuePair<string, string> recipe in RawRecipeData)
            {
                // data is "ingredient amount ingredient amount" ect.
                string[] ingredientsString = recipe.Value.Split('/')[0].Split(' ');

                //its possible that the recipe is "recipeIndex count" in some mods like 
                //animal husbandry so I need the second split
                int recipeIndex = ParseRecipeIndex(recipe.Value.Split('/')[2].Split(' ')[0]);
                for(int i = 0; i < ingredientsString.Length; ++i)
                {
                    if (IsIngredient(i))
                    {
                        int ingredientIndex = ParseIngredientIndex(ingredientsString[i]);
                        int amount = ParseAmount(ingredientsString[i + 1]);

                        AddIngredient(recipeIndex, ingredientIndex, amount);
                    }
                }
            }
        }

        private int ParseRecipeIndex(string index)
        {
            bool successful = int.TryParse(index, out int intIndex);
            if (!successful)
            {
                ModEntry.Logger.LogRecipeIndexParseFail(index);
                return -1;
            }
            return intIndex;
        }

        private int ParseIngredientIndex(string index)
        {
            bool successful = int.TryParse(index, out int intIndex);
            if (!successful)
            {
                ModEntry.Logger.LogIngredientIndexParseFail(index);
                return -1;
            }
            return intIndex;
        }

        private int ParseAmount(string amount)
        {
            bool successful = int.TryParse(amount, out int intAmount);
            if(!successful)
            {
                ModEntry.Logger.LogIngredientAmountParseFail(amount);
                return 1; //In most cases its 1
            }
            return intAmount;
        }

        private void AddIngredient(int recipeIndex, int ingredientIndex, int amount)
        {
            if (AllRecipeIngredients.ContainsKey(recipeIndex))
            {
                AddIngredientOnly(recipeIndex, ingredientIndex, amount);
            }
            else
            {
                AddRecipeAndIngredient(recipeIndex, ingredientIndex, amount);
            }
        }

        private void AddRecipeAndIngredient(int recipeParentSheetIndex, int ingredientParentSheetIndex, int amount)
        {
            SObject ingredientObj = new SObject(ingredientParentSheetIndex, amount);
            Dictionary<int, SObject> ingredient = new Dictionary<int, SObject> { [ingredientParentSheetIndex] = ingredientObj };
            AllRecipeIngredients.Add(recipeParentSheetIndex, ingredient);
        }

        private void AddIngredientOnly(int recipeParentSheetIndex, int ingredientParentSheetIndex, int amount)
        {
            SObject ingredientObj = new SObject(ingredientParentSheetIndex, amount);
            Dictionary<int, SObject> ingredients = AllRecipeIngredients[recipeParentSheetIndex];

            //Though I don't have to check this if its just the base
            //game, some other mods people download can cause issues
            //so just to be safe I'll check
            if (!ingredients.ContainsKey(ingredientParentSheetIndex))
            {
                ingredients.Add(ingredientParentSheetIndex, ingredientObj);
            }
            else
            {
                //Maybe the mod adds ingredients one by one instead of
                //using a count for the same ingredient
                SObject currentIngredient = ingredients[ingredientParentSheetIndex];
                currentIngredient.Stack += amount;

                ModEntry.Logger.LogDuplicateIngredient(recipeParentSheetIndex, ingredientParentSheetIndex, amount, currentIngredient.Stack);
            }
        }

        private bool IsIngredient(int indexInData)
        {
            return indexInData % 2 == 0;
        }
    }
}
