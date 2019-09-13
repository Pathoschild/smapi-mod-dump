using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
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
                int parentSheetIndex = ParseRecipeIndex(recipe.Value.Split('/')[2]);

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
            return ModEntry.Config.ShowAllRecipes || ModEntry.Config.AlwasyShowAllRecipes || Game1.player.cookingRecipes.ContainsKey(key);
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
            return AllRecipeIngredients[recipeParentSheetIndex];
        }

        private void AddRecipes()
        {
            Dictionary<string, string> RawRecipeData = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            foreach (KeyValuePair<string, string> recipe in RawRecipeData)
            {
                // data is ingredient amount ingredient amount ect.
                string[] ingredientsString = recipe.Value.Split('/')[0].Split(' ');
                int recipeIndex = ParseRecipeIndex(recipe.Value.Split('/')[2]);
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
            Dictionary<int, SObject> ingredient = AllRecipeIngredients[recipeParentSheetIndex];
            ingredient.Add(ingredientParentSheetIndex, ingredientObj);
        }

        private bool IsIngredient(int indexInData)
        {
            return indexInData % 2 == 0;
        }
    }
}
