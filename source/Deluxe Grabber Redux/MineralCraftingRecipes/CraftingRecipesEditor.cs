/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace MineralCraftingRecipes
{
    class CraftingRecipesEditor : IAssetEditor
    {
        private static readonly List<Recipe> Recipes = new List<Recipe>
        {
            new Recipe
            {
                Name = "Prismatic Shard",
                OutputItemId = 74,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        ItemId = 337, // Iridium Bar
                        Amount = 5
                    },
                    new Ingredient
                    {
                        ItemId = 72, // Diamond
                        Amount = 1
                    }
                }
            },
            new Recipe
            {
                Name = "Diamond",
                OutputItemId = 72,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        ItemId = 338, // Refined Quartz
                        Amount = 10
                    }
                }
            }
        };
        private readonly Mod Mod;

        public CraftingRecipesEditor(Mod mod)
        {
            Mod = mod;
        }

        public void LearnRecipes()
        {
            foreach (var recipe in Recipes)
            {
                if (!Game1.player.knowsRecipe(recipe.Name))
                {
                    Game1.player.craftingRecipes.Add(recipe.Name, 0);
                }
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (var recipe in Recipes)
            {
                if (data.ContainsKey(recipe.Name))
                {
                    Mod.Monitor.Log($"Recipe {recipe.Name} already exists, skipping.", LogLevel.Warn);
                    continue;
                }
                else
                {
                    var ingredients = string.Join(" ", recipe.Ingredients);
                    var recipeData = $"{ingredients}/Home/{recipe.OutputItemId}/false/null";

                    Mod.Monitor.Log($"Adding recipe {recipe.Name}: \"{recipeData}\"", LogLevel.Trace);
                    data[recipe.Name] = recipeData;
                }
            }
        }
    }
}
