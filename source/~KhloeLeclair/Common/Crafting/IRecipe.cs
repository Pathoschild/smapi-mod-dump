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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.Crafting;

// Remember to update IBetterCrafting whenever this changes!

/// <summary>
/// An <c>IRecipe</c> represents a single crafting recipe, though it need not
/// be associated with a vanilla <see cref="StardewValley.CraftingRecipe"/>.
/// Recipes usually produce <see cref="Item"/>s, but they are not required
/// to do so.
/// </summary>
public interface IRecipe {

	#region Identity

	/// <summary>
	/// An addditional sorting value to apply to recipes in the Better Crafting
	/// menu. Applied before other forms of sorting.
	/// </summary>
	int SortValue { get; }

	/// <summary>
	/// The internal name of the recipe. For standard recipes, this matches the
	/// name of the recipe used in the player's cookingRecipes / craftingRecipes
	/// dictionaries. For non-standard recipes, this can be anything as long as
	/// it's unique, and it's recommended to prefix the names with your mod's
	/// unique ID to ensure uniqueness.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// A name displayed to the user.
	/// </summary>
	string DisplayName { get; }

	/// <summary>
	/// An optional description of the recipe displayed on its tooltip.
	/// </summary>
	string? Description { get; }

	/// <summary>
	/// Whether or not the player knows this recipe.
	/// </summary>
	/// <param name="who">The player we're asking about</param>
	bool HasRecipe(Farmer who);

	/// <summary>
	/// How many times the player has crafted this recipe. If advanced crafting
	/// information is enabled, and this value is non-zero, it will be
	/// displayed on recipe tooltips.
	/// </summary>
	/// <param name="who">The player we're asking about.</param>
	int GetTimesCrafted(Farmer who);

	/// <summary>
	/// The vanilla <c>CraftingRecipe</c> instance for this recipe, if one
	/// exists. This may be used for interoperability with some other
	/// mods, but is not required.
	/// </summary>
	CraftingRecipe? CraftingRecipe { get; }

	#endregion

	#region Display

	/// <summary>
	/// The texture to use when drawing this recipe in the menu.
	/// </summary>
	Texture2D Texture { get; }

	/// <summary>
	/// The source rectangle to use when drawing this recipe in the menu.
	/// </summary>
	Rectangle SourceRectangle { get; }

	/// <summary>
	/// How tall this recipe should appear in the menu, in grid squares.
	/// </summary>
	int GridHeight { get; }

	/// <summary>
	/// How wide this recipe should appear in the menu, in grid squares.
	/// </summary>
	int GridWidth { get; }

	#endregion

	#region Cost and Quantity

	/// <summary>
	/// The quantity of item produced every time this recipe is crafted.
	/// </summary>
	int QuantityPerCraft { get; }

	/// <summary>
	/// The ingredients used by this recipe.
	/// </summary>
	IIngredient[]? Ingredients { get; }

	#endregion

	#region Creation

	/// <summary>
	/// Whether or not the item created by this recipe is stackable, and thus
	/// eligible for bulk crafting.
	/// </summary>
	bool Stackable { get; }

	/// <summary>
	/// Check to see if the given player can currently craft this recipe. This
	/// method is suitable for checking external conditions. For example, the
	/// add-on for crafting buildings from the crafting menu uses this to check
	/// that the current <see cref="GameLocation"/> allows building.
	/// </summary>
	/// <param name="who">The player we're asking about.</param>
	bool CanCraft(Farmer who);

	/// <summary>
	/// An optional, extra string to appear on item tooltips. This can be used
	/// for displaying error messages to the user, or anything else that would
	/// be relevant. For example, the add-on for crafting buildings uses this
	/// to display error messages telling users why they are unable to craft
	/// a building, if they cannot.
	/// </summary>
	/// <param name="who">The player we're asking about.</param>
	string? GetTooltipExtra(Farmer who);

	/// <summary>
	/// Create an instance of the Item this recipe crafts, if this recipe
	/// crafts an item. Returning null is perfectly acceptable.
	/// </summary>
	Item? CreateItem();

	/// <summary>
	/// This method is called when performing a craft, and can be used to
	/// perform asynchronous actions or other additional logic as required.
	/// While crafting is taking place, Better Crafting will hold locks on
	/// every inventory involved. You should ideally do as little work
	/// here as possible.
	/// </summary>
	/// <param name="evt">Details about the event, and methods for telling
	/// Better Crafting when the craft has succeeded or failed.</param>
	void PerformCraft(IPerformCraftEvent evt) {
		evt.Complete();
	}

	#endregion
}
