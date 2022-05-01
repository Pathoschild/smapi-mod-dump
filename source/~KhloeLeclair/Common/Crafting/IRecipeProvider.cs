/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System.Collections.Generic;

using StardewValley;

namespace Leclair.Stardew.Common.Crafting;

// Remember to update IBetterCrafting whenever this changes!

/// <summary>
/// Better Crafting uses <c>IRecipeProvider</c> to discover crafting recipes
/// for display in the menu.
/// </summary>
public interface IRecipeProvider {
	/// <summary>
	/// The priority of this recipe provider, sort sorting purposes.
	/// When handling CraftingRecipe instances, the first provider
	/// to return a result is used.
	/// </summary>
	int RecipePriority { get; }

	/// <summary>
	/// Get an <see cref="IRecipe"/> wrapper for a <see cref="CraftingRecipe"/>.
	/// </summary>
	/// <param name="recipe">The vanilla <c>CraftingRecipe</c> to wrap</param>
	/// <returns>An IRecipe wrapper, or null if this provider does
	/// not handle this recipe.</returns>
	IRecipe? GetRecipe(CraftingRecipe recipe);

	/// <summary>
	/// Whether or not additional recipes from this provider should be
	/// cached. If the list should be updated every time the player
	/// opens the menu, this should return false.
	/// </summary>
	bool CacheAdditionalRecipes { get; }

	/// <summary>
	/// Get any additional recipes in IRecipe form. Additional recipes
	/// are those recipes not included in the <see cref="CraftingRecipe.cookingRecipes"/>
	/// and <see cref="CraftingRecipe.craftingRecipes"/> objects.
	/// </summary>
	/// <param name="cooking">Whether we want cooking recipes or crafting recipes.</param>
	/// <returns>An enumeration of this provider's additional recipes, or null.</returns>
	IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking);
}
