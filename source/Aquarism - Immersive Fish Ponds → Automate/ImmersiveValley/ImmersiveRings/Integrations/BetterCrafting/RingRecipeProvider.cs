/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using Common.Integrations;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

#endregion using directives

/// <summary>Provides <see cref="IRecipe"/> wrappers for Ring recipes with consume other Rings.</summary>
internal class RingRecipeProvider : IRecipeProvider
{
    private readonly IBetterCraftingAPI _api;

    /// <summary>
    /// The priority of this recipe provider, for sorting purposes.
    /// When handling CraftingRecipe instances, the first provider
    /// to return a result is used.
    /// </summary>
    public int RecipePriority => int.MaxValue;

    /// <summary>
    /// Whether or not additional recipes from this provider should be
    /// cached. If the list should be updated every time the player
    /// opens the menu, this should return false.
    /// </summary>
    public bool CacheAdditionalRecipes => false;

    /// <summary>Construct an instance.</summary>
    /// <param name="api">The Better Crafting API.</param>
    public RingRecipeProvider(IBetterCraftingAPI api)
    {
        _api = api;
    }

    /// <summary>Get an <see cref="IRecipe"/> wrapper for a <see cref="CraftingRecipe"/>.</summary>
    /// <param name="recipe">The vanilla <c>CraftingRecipe</c> to wrap</param>
    /// <returns>An <see cref="IRecipe"/> wrapper, or null if this provider does not handle this recipe.</returns>
    public IRecipe? GetRecipe(CraftingRecipe recipe)
    {
        // make a Ring instance to get its DisplayName.
        switch (recipe.name)
        {
            case "Glow Ring":
                var smallGlowRing = new Ring(Constants.SMALL_GLOW_RING_INDEX_I);

                // return a recipe that uses data from the vanilla crafting recipe, but with custom ingredient handling
                return _api.CreateRecipeWithIngredients(recipe, new[]
                {
                    // normal ingredients
                    _api.CreateBaseIngredient(Constants.SUN_ESSENCE_INDEX_I, 10),

                    // Ring ingredient
                    _api.CreateMatcherIngredient(
                        matcher: item => item is Ring {ParentSheetIndex: Constants.SMALL_GLOW_RING_INDEX_I},
                        quantity: 2,
                        displayName: smallGlowRing.DisplayName,
                        texture: Game1.objectSpriteSheet,
                        source: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            Constants.SMALL_GLOW_RING_INDEX_I, 16, 16)
                    )
                });

            case "Magnet Ring":
                var smallMagnetRing = new Ring(Constants.SMALL_MAGNET_RING_INDEX_I);

                // return a recipe that uses data from the vanilla crafting recipe, but with custom ingredient handling
                return _api.CreateRecipeWithIngredients(recipe, new[]
                {
                    // normal ingredients
                    _api.CreateBaseIngredient(Constants.VOID_ESSENCE_INDEX_I, 10),

                    // Ring ingredient
                    _api.CreateMatcherIngredient(
                        matcher: item => item is Ring {ParentSheetIndex: Constants.SMALL_MAGNET_RING_INDEX_I},
                        quantity: 2,
                        displayName: smallMagnetRing.DisplayName,
                        texture: Game1.objectSpriteSheet,
                        source: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            Constants.SMALL_MAGNET_RING_INDEX_I, 16, 16)
                    )
                });

            case "Glowstone Ring":
                var glowRing = new Ring(Constants.GLOW_RING_INDEX_I);
                var magnetRIng = new Ring(Constants.MAGNET_RING_INDEX_I);

                // return a recipe that uses data from the vanilla crafting recipe, but with custom ingredient handling
                return _api.CreateRecipeWithIngredients(recipe, new[]
                {
                    // normal ingredients
                    _api.CreateBaseIngredient(Constants.SUN_ESSENCE_INDEX_I, 20),
                    _api.CreateBaseIngredient(Constants.VOID_ESSENCE_INDEX_I, 20),

                    // Ring ingredient
                    _api.CreateMatcherIngredient(
                        matcher: item => item is Ring {ParentSheetIndex: Constants.GLOW_RING_INDEX_I},
                        quantity: 1,
                        displayName: glowRing.DisplayName,
                        texture: Game1.objectSpriteSheet,
                        source: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            Constants.GLOW_RING_INDEX_I, 16, 16)
                    ),
                    _api.CreateMatcherIngredient(
                        matcher: item => item is Ring {ParentSheetIndex: Constants.MAGNET_RING_INDEX_I},
                        quantity: 1,
                        displayName: magnetRIng.DisplayName,
                        texture: Game1.objectSpriteSheet,
                        source: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                            Constants.MAGNET_RING_INDEX_I, 16, 16)
                    )
                });

            default:
                return null;
        }
    }

    /// <summary>
    /// Get any additional recipes in <see cref="IRecipe"/> form. Additional recipes are
    /// those recipes not included in either <see cref="CraftingRecipe.cookingRecipes"/>
    /// or <see cref="CraftingRecipe.craftingRecipes"/>.
    /// </summary>
    /// <param name="cooking">Whether we want cooking recipes or crafting recipes.</param>
    /// <returns>An enumeration of this provider's additional recipes, or null.</returns>
    public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) => null;
}