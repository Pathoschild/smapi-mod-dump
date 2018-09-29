using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EquivalentExchange.Models
{
    public static class AlchemyTransmutationRecipeExtensions
    {
        // extension method to determine if a recipe exists
        public static bool HasItem(this List<AlchemyTransmutationRecipe> recipes, int itemId)
        {
            return recipes.Any(x => x.OutputId == itemId);
        }

        public static void AddRecipeLink(this List<AlchemyTransmutationRecipe> recipes, int input, int output, int cost = 1)
        {
            recipes.Add(new AlchemyTransmutationRecipe(input, output, cost));
        }

        public static List<AlchemyTransmutationRecipe> GetRecipesForOutput(this List<AlchemyTransmutationRecipe> recipes, int output)
        {
            var filteredRecipes = recipes.Where(x => x.OutputId == output).ToList();
            filteredRecipes.Sort((x, y) => {
                var comp = Util.GetItemValue(x.InputId) - Util.GetItemValue(y.InputId);            
                return comp;
            });
            // sort by whether the player has items
            filteredRecipes.Sort((x, y) => {
                var hasX = Game1.player.hasItemInInventory(x.InputId, x.GetInputCost() + 1) ? 0 : 1;
                var hasY = Game1.player.hasItemInInventory(y.InputId, y.GetInputCost() + 1) ? 0 : 1;
                return hasX - hasY;
            });
            return filteredRecipes;
        }

        public static AlchemyTransmutationRecipe FindBestRecipe(this List<AlchemyTransmutationRecipe> recipes, StardewValley.Farmer farmer)
        {            
            foreach(var recipe in recipes)
            {
                if (farmer.hasItemInInventory(recipe.InputId, recipe.GetInputCost() + 1))
                {
                    if (EquivalentExchange.CurrentEnergy + (Math.Max(0, farmer.stamina - 1)) >= recipe.GetEnergyCost())
                    {
                        return recipe;
                    }
                }
            }

            // the farmer isn't able to use any of the recipes, abort.
            return null;
        }
    }
}
