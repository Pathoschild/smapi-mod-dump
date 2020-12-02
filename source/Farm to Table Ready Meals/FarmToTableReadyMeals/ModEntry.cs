/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FarmToTableReadyMeals
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using StardewValley;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FarmToTableReadyMeals
{
    public class ModEntry : Mod
    {
        internal new static IModHelper Helper;
        internal new static IMonitor Monitor;

        List<CookingRecipe> craftingResults = new List<CookingRecipe>();

        Dictionary<string, List<string>> RecipesAdded = new Dictionary<string, List<string>>();

        public override void Entry(IModHelper helper)
        {
            Helper = helper;
            Monitor = base.Monitor;
            
            Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        }

        private void InternalSetup()
        {
            craftingResults.Clear();
            RecipesAdded.Clear();

            string[] recipeData;
            string[] ingredientPairs;
            CookingRecipe recipe;
            
            //Construct a list of recipes for our internal use
            foreach (KeyValuePair<string, string> kvp in CraftingRecipe.cookingRecipes)
            {
                recipeData = kvp.Value.Split('/');
                ingredientPairs = recipeData[0].Split(' ');

                recipe = new CookingRecipe
                {
                    Name = kvp.Key,
                    Source = recipeData[3],
                    MysteryText = recipeData[1]
                };

                if(Regex.IsMatch(recipeData[2], "[0-9]+ [0-9]+"))
                {
                    recipe.OutputID = int.Parse(recipeData[2].Split(' ')[0]);
                    recipe.Amount  = int.Parse(recipeData[2].Split(' ')[1]);
                }
                else
                {
                    recipe.OutputID = int.Parse(recipeData[2]);
                }

                for (int i = 0; i < ingredientPairs.Length; i = i + 2)
                {
                    int ingredientId = int.Parse(ingredientPairs[i]);
                    int ingredientAmount = int.Parse(ingredientPairs[i + 1]);

                    recipe.AddIngredient(ingredientId, ingredientAmount);
                }

                craftingResults.Add(recipe);
            }

            int AddedRecipes = 0;

            Dictionary<int, int> AllIngredients = new Dictionary<int, int>();
            foreach (CookingRecipe testRecipe in craftingResults)
            {
                AllIngredients.Clear();

                AllIngredients = GetAllIngredientsFromChildren(testRecipe.OutputID);

                bool isNew = !testRecipe.Ingredients.ContentEquals(AllIngredients);

                if (isNew)
                {
                    CookingRecipe newRecipe = new CookingRecipe()
                    {
                        Name = testRecipe.Name,
                        Source = "null",// testRecipe.Source,
                        MysteryText = testRecipe.MysteryText,
                        OutputID = testRecipe.OutputID,
                        Ingredients = AllIngredients
                    };



                    string NameToAdd = newRecipe.GetKey() + " ";

                    if (RecipesAdded.TryGetValue(newRecipe.GetKey(), out List<string> SecondaryRecipes))
                    {
                        //We have already created a list

                        for (int i = 0; i < SecondaryRecipes.Count; i++)
                        {
                            NameToAdd = NameToAdd + " "; //Pad that shit to avoid dupes
                        }

                        SecondaryRecipes.Add(NameToAdd);
                    }
                    else
                    {
                        RecipesAdded.Add(newRecipe.GetKey(), new List<string>() { NameToAdd });
                    }

                    CraftingRecipe.cookingRecipes.Add(NameToAdd, newRecipe.GetValue());
                    AddedRecipes++;
                }
            }
            Monitor.Log($"Added {AddedRecipes} recipes!", LogLevel.Info);
        }
        
        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            InternalSetup();
            Game1.player.cookingRecipes.OnValueAdded += (string key, int value)=>
            {
                if (RecipesAdded.ContainsKey(key))
                {
                    foreach (string s in RecipesAdded[key])
                    {
                        Game1.player.cookingRecipes.Add(s, 0);
                        Monitor.Log($"Unlocked \"{s}\" for player as a related recipe was unlocked!", LogLevel.Info);
                    }
                }
            };
            Game1.player.cookingRecipes.OnValueRemoved += (string key, int value) =>
            {
                if (RecipesAdded.ContainsKey(key))
                {
                    foreach (string s in RecipesAdded[key])
                    {
                        Game1.player.cookingRecipes.Remove(s);
                        Monitor.Log($"Removed \"{s}\" for player as a related recipe was removed!", LogLevel.Info);
                    }
                }
            };

            //Unlock any recipes we added as relevant
            foreach(var kvp in RecipesAdded)
            {
                if (Game1.player.cookingRecipes.ContainsKey(kvp.Key))
                {
                    foreach(string s in kvp.Value)
                    {
                        if (Game1.player.cookingRecipes.ContainsKey(s))
                        {
                            continue;
                        }
                        Game1.player.cookingRecipes.Add(s, 0);
                    }
                }
            }
        }

        internal Dictionary<int, int> GetAllIngredientsFromChildren(int ItemCreated)
        {
            Dictionary<int, int> ingredientsFound = new Dictionary<int, int>();

            //ItemCreated is an item we can cook
            IEnumerable<CookingRecipe> recipes = craftingResults.Where(x => x.OutputID == ItemCreated);
            foreach (CookingRecipe ingredientsForChild in recipes)
            {
                foreach(KeyValuePair<int, int> IngredientPairs in ingredientsForChild.Ingredients)
                {
                    Dictionary<int, int> newToAdd = GetAllIngredientsFromChildren(IngredientPairs.Key);

                    if (newToAdd.Count == 0)
                    {
                        if (ingredientsFound.TryGetValue(IngredientPairs.Key, out int amount))
                        {
                            ingredientsFound[IngredientPairs.Key] += amount;
                        }
                        else
                        {
                            ingredientsFound.Add(IngredientPairs.Key, IngredientPairs.Value);
                        }
                    }
                    else
                    {
                        foreach (var value in newToAdd)
                        {

                            if (ingredientsFound.TryGetValue(value.Key, out int amount))
                            {
                                ingredientsFound[value.Key] += value.Value * IngredientPairs.Value;
                            }
                            else
                            {
                                ingredientsFound.Add(value.Key, value.Value * IngredientPairs.Value);
                            }
                        }
                    }
                }
            }

            return ingredientsFound;
        }
    }
}
