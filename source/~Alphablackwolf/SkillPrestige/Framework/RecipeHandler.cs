/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework;

public static class RecipeHandler
{
    private static List<string> CurrentPlayerCraftingRecipes { get; set; } = new();
    private static List<string> CurrentPlayerCookingRecipes { get; set; } = new();

    public static void ResetRecipes()
    {
        Logger.LogVerbose("Recipe Handler - Resetting recipe lists...");
        CurrentPlayerCraftingRecipes = new List<string>();
        CurrentPlayerCookingRecipes = new List<string>();
    }
    public static void LoadRecipes()
    {
        Logger.LogVerbose("Recipe Handler - Loading recipe lists");
        foreach (var craftingRecipe in Game1.player.craftingRecipes)
            foreach (var entry in craftingRecipe)
                CurrentPlayerCraftingRecipes.Add(entry.Key);
        foreach (var cookingRecipe in Game1.player.cookingRecipes)
            foreach (var entry in cookingRecipe)
                CurrentPlayerCookingRecipes.Add(entry.Key);
    }

    public static void CheckForAndHandleAddedRecipes()
    {
        var addedCraftingRecipes = AddedCraftingRecipes().ToList();
        var addedCookingRecipes = AddedCookingRecipes().ToList();

        if (addedCraftingRecipes.Any())
        {
            Logger.LogVerbose("Recipe Handler - New crafting recipe(s) found...");
            foreach (string recipeKey in addedCraftingRecipes)
            {
                foreach (var prestige in PrestigeSet.Instance.Prestiges)
                {
                    prestige.FixDeserializedNulls();
                    if (prestige.CraftingRecipeAmountsToSave.TryGetValue(recipeKey, out int recipeCreationCount))
                    {
                        Logger.LogInformation($"Recipe Handler - {recipeKey} recipe had prior crafting count, {recipeCreationCount}, resetting value...");
                        Game1.player.craftingRecipes[recipeKey] += recipeCreationCount;
                        prestige.CraftingRecipeAmountsToSave.Remove(recipeKey);
                    }
                    CurrentPlayerCraftingRecipes.Add(recipeKey);
                }
            }
        }
        // ReSharper disable once InvertIf
        if (addedCookingRecipes.Any())
        {
            Logger.LogVerbose("Recipe Handler - New cooking recipe(s) found...");
            foreach (string recipeKey in addedCookingRecipes)
            foreach (var prestige in PrestigeSet.Instance.Prestiges)
            {
                if (prestige.CookingRecipeAmountsToSave.TryGetValue(recipeKey, out int recipeCreationCount))
                {
                    Logger.LogInformation($"Recipe Handler - {recipeKey} recipe had prior cooking count, {recipeCreationCount}, resetting value...");
                    Game1.player.cookingRecipes[recipeKey] = recipeCreationCount;
                    prestige.CookingRecipeAmountsToSave.Remove(recipeKey);
                }

                CurrentPlayerCookingRecipes.Add(recipeKey);
            }
        }
    }

    private static IEnumerable<string> AddedCraftingRecipes()
    {
        return Game1.player.craftingRecipes.SelectMany(x => x).Where(entry => !CurrentPlayerCraftingRecipes.Contains(entry.Key)).Select(x => x.Key);
    }

    private static IEnumerable<string> AddedCookingRecipes()
    {
        return Game1.player.cookingRecipes.SelectMany(x => x).Where(entry => !CurrentPlayerCookingRecipes.Contains(entry.Key)).Select(x => x.Key);
    }
}
