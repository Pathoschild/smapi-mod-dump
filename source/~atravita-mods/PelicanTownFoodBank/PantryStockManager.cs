/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.StringHandler;
using Microsoft.Xna.Framework;
using PelicanTownFoodBank.Models;
using StardewModdingAPI.Utilities;

namespace PelicanTownFoodBank;

/// <summary>
/// Static class that manages the shop stock for the food pantry.
/// </summary>
internal static class PantryStockManager
{
    private static readonly PerScreen<Lazy<List<int>>> PerScreenedSellables = new(() => new Lazy<List<int>>(SetUpInventory));
    private static readonly PerScreen<HashSet<ISalable>> PerScreenedBuyBacks = new(() => new HashSet<ISalable>());

    /// <summary>
    /// Gets a list of integers that corresponds to the shop's stock.
    /// </summary>
    internal static List<int> Sellables => PerScreenedSellables.Value.Value;

    /// <summary>
    /// Gets a Dictionary consisting of the sold back objects and their quantities.
    /// </summary>
    internal static HashSet<ISalable> BuyBacks => PerScreenedBuyBacks.Value;

    /// <summary>
    /// Gets the categories of SObject the food bank deals with...
    /// </summary>
    internal static int[] FoodBankCategories { get; } = new int[]
    {
            SObject.artisanGoodsCategory,
            SObject.CookingCategory,
            SObject.EggCategory,
            SObject.FishCategory,
            SObject.FruitsCategory,
            SObject.GreensCategory,
            SObject.ingredientsCategory,
            SObject.meatCategory,
            SObject.MilkCategory,
            SObject.sellAtFishShopCategory,
            SObject.sellAtPierres,
            SObject.sellAtPierresAndMarnies,
            SObject.syrupCategory,
            SObject.VegetableCategory,
    };

    /// <summary>
    /// Gets the current food pantry menu.
    /// </summary>
    /// <returns>Food pantry menu.</returns>
    internal static FoodBankMenu GetFoodBankMenu()
    {
        Dictionary<ISalable, int[]> sellables = Sellables.ToDictionary((int i) => (ISalable)new SObject(Vector2.Zero, i, 1), (_) => new int[] { 0, 1 });
        foreach (ISalable buyback in BuyBacks)
        {
            sellables[buyback] = new[] { 0, buyback.Stack };
        }
        return new(sellables, BuyBacks);
    }

    /// <summary>
    /// Resets data structures. Call **per** player.
    /// </summary>
    internal static void Reset()
    {
        BuyBacks.Clear();
        PerScreenedSellables.Value = new Lazy<List<int>>(SetUpInventory);
    }

    /// <summary>
    /// Sets up the daily inventory.
    /// </summary>
    /// <returns>The daily inventory.</returns>
    internal static List<int> SetUpInventory()
    {
        Random seededRandom = new((int)(Game1.uniqueIDForThisGame * 2) + (int)(Game1.stats.DaysPlayed * 40));
        List<int> neededIngredients = GetNeededIngredients();
        (List<int> cookingIngredients, List<int> cookedItems) = GetOtherSellables();
        Utility.Shuffle(seededRandom, neededIngredients);
        Utility.Shuffle(seededRandom, cookingIngredients);
        Utility.Shuffle(seededRandom, cookedItems);

        List<int> sellables = new();
        sellables.AddRange(neededIngredients.Take(5).Concat(cookingIngredients.Take(5)).Concat(cookedItems.Take(3)));
        Utility.Shuffle(seededRandom, sellables);
        return sellables;
    }

    private static List<int> GetNeededIngredients()
    {
        List<int> neededIngredients = new();
        Dictionary<string, string> recipes = Game1.content.Load<Dictionary<string, string>>("Data/CookingRecipes");
        foreach ((string learned_recipe, int number_made) in Game1.player.cookingRecipes.Pairs)
        {
            if (number_made != 0 )
            {
                continue;
            }
            else if (recipes.TryGetValue(learned_recipe, out string? recipe) && recipe.IndexOf('/') is int index && index > 0)
            {
                SpanSplit ingredients = recipe[..index].SpanSplit();
                for(int i = 0; i < ingredients.Count; i += 2)
                {
                    if(int.TryParse(ingredients[i], out int ingredient) && ingredient > 0)
                    {
                        neededIngredients.Add(ingredient);
                    }
                }
            }
        }
        return neededIngredients;
    }

    private static (List<int> cookingIngredients, List<int> cookedItems) GetOtherSellables()
    {
        List<int> cookingIngredients = new();
        List<int> cookedItems = new();
        foreach ((int index, string data) in Game1.objectInformation)
        {
            SpanSplit splits = data.SpanSplit('/');
            SpanSplit typesandcategory = splits[3].SpanSplit();
            if (typesandcategory.Count > 1 && int.TryParse(typesandcategory[1], out int result)
                && FoodBankCategories.Contains(result)
                && int.TryParse(splits[1], out int price ) && price < 250)
            {
                if (result == SObject.CookingCategory)
                {
                    cookedItems.Add(index);
                }
                else
                {
                    cookingIngredients.Add(index);
                }
            }
        }
        return (cookingIngredients, cookedItems);
    }
}