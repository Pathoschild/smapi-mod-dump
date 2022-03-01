/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Inventory;

using StardewValley;

namespace Leclair.Stardew.Common {
	public static class CraftingHelper {

		public static bool HasIngredients(IIngredient[] ingredients, Farmer who, IList<Item> items, IList<IInventory> inventories, int max_quality) {
			foreach (var entry in ingredients)
				if (entry.GetAvailableQuantity(who, items, inventories, max_quality) < entry.Quantity)
					return false;

			return true;
		}

		public static bool HasIngredients(this IRecipe recipe, Farmer who, IList<Item> items, IList<IInventory> inventories, int max_quality) {
			return HasIngredients(recipe.Ingredients, who, items, inventories, max_quality);
		}

		public static void ConsumeIngredients(IIngredient[] ingredients, Farmer who, IList<IInventory> inventories, int max_quality, bool low_quality_first) {
			foreach (var entry in ingredients)
				entry.Consume(who, inventories, max_quality, low_quality_first);
		}

		public static void Consume(this IRecipe recipe, Farmer who, IList<IInventory> inventories, int max_quality, bool low_quality_first) {
			ConsumeIngredients(recipe.Ingredients, who, inventories, max_quality, low_quality_first);
		}

	}
}
