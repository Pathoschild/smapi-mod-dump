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

#if COMMON_CRAFTING

using System.Collections.Generic;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.Crafting;

// Remember to update IBetterCrafting whenever this changes!

/// <summary>
/// This event is dispatched by Better Crafting whenever a player performs a
/// craft, and may be fired multiple times in quick succession if a player is
/// performing bulk crafting.
/// </summary>
public interface IPerformCraftEvent {

	/// <summary>
	/// The player performing the craft.
	/// </summary>
	Farmer Player { get; }

	/// <summary>
	/// The item being crafted, may be null depending on the recipe.
	/// </summary>
	Item? Item { get; set; }

	/// <summary>
	/// The <c>BetterCraftingPage</c> menu instance that the player is
	/// crafting from.
	/// </summary>
	IClickableMenu Menu { get; }

	/// <summary>
	/// Cancel the craft, marking it as a failure. The ingredients will not
	/// be consumed and the player will not receive the item.
	/// </summary>
	void Cancel();

	/// <summary>
	/// Complete the craft, marking it as a success. The ingredients will be
	/// consumed and the player will receive the item, if there is one.
	/// </summary>
	void Complete();
}


/// <summary>
/// An extended IPerformCraftEvent subclass that also includes a
/// reference to the recipe being used. This is necessary because
/// adding this to the existing model would break Pintail proxying,
/// for some reason.
/// </summary>
public interface IGlobalPerformCraftEvent : IPerformCraftEvent {

	/// <summary>
	/// The recipe being crafted.
	/// </summary>
	IRecipe Recipe { get; }

}

/// <summary>
/// A further extended IGlobalPerformCraftEvent subclass that includes
/// a list of the matching ingredients being consumed. This is necessary
/// because adding this to the existing model would break Pintail's
/// proxying, for some reason.
/// </summary>
public interface IGlobalPerformCraftEventV2 : IGlobalPerformCraftEvent {

	/// <summary>
	/// A dictionary for looking up matching ingredient items.
	/// If an <see cref="IIngredient"/> does not have an entry in this
	/// list, then that ingredient does not support the ingredient
	/// matching feature.
	///
	/// You may remove an item from the list to prevent it from
	/// being consumed.
	/// </summary>
	IReadOnlyDictionary<IIngredient, List<Item>> MatchingItems { get; }

}


#endif
