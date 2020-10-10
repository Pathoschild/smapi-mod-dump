/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting {
    public interface ICraftingApi {
        /// <summary>Registers a recipe to be tracked by the game.</summary>
        /// <param name="localKey">The local key for the recipe, unique to your mod.</param>
        /// <param name="recipe">The recipe to register.</param>
        /// <returns>A global key which uniquely identifies the registered recipe.</returns>
        CraftingKey RegisterRecipe(string localKey, IRecipe recipe);

        /// <summary>Tries to get a recipe from its key.</summary>
        /// <param name="localKey">The local key for the recipe which uniquely identifies it within your mod.</param>
        /// <param name="recipe">If successful, the recipe with the given key.</param>
        /// <returns>True if successfully found, false otherwise.</returns>
        bool TryGetRecipe(string localKey, out IRecipe recipe);

        /// <summary>Tries to get a recipe from its key.</summary>
        /// <param name="key">The global key for the recipe which uniquely identifies it within the game.</param>
        /// <param name="recipe">If successful, the recipe with the given key.</param>
        /// <returns>True if successfully found, false otherwise.</returns>
        bool TryGetRecipe(CraftingKey key, out IRecipe recipe);

        /// <summary>Adds a recipe to the list of possible crafting recipes. The recipe can be given to the main player with <c>Game1.player.craftingRecipes.Add(<paramref name="key"/>, 0);</c>.</summary>
        /// <param name="key">The global key for the recipe which uniquely identifies it within the game.</param>
        void AddCraftingRecipe(CraftingKey key);

        /// <summary>Adds a recipe to the list of possible cooking recipes. The recipe can be given to the main player with <c>Game1.player.cookingRecipes.Add(<paramref name="key"/>, 0);</c>.</summary>
        /// <param name="key">The global key for the recipe which uniquely identifies it within the game.</param>
        void AddCookingRecipe(CraftingKey key);
    }
}
