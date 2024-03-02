/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the crafting recipes
	/// </summary>
	public class CraftingRecipeRandomizer
	{
		/// <summary>
		/// Randomizes the crafting recipes
		/// Includes two non-item recipes to transmute metals
		/// </summary>
		/// <returns>The dictionary of changes to make to the asset</returns>
		public static Dictionary<string, string> Randomize()
		{
			if (Globals.Config.CraftingRecipes.Randomize)  
			{ 
				Globals.SpoilerWrite($"==== CRAFTING RECIPES ===="); 
			}

			Dictionary<string, string> replacements = new();
			var allCraftableItems = ItemList.Items.Values
				.Concat(ItemList.BigCraftableItems.Values)
				.Where(x => x.IsCraftable)
				.Cast<CraftableItem>()
				.ToList();
			foreach (CraftableItem item in allCraftableItems)
			{
                replacements[item.CraftingRecipeKey] = item.GetCraftingString();
			}

			// These two are not actually items, but we want to randomize their recipes anwyway
			// The IDs passed in don't really matter
			replacements["Transmute (Fe)"] = new CraftableItem(
				-1000, CraftableCategories.Moderate, dataKey: "Transmute (Fe)").GetCraftingString();
            replacements["Transmute (Au)"] = new CraftableItem(
				-1000, CraftableCategories.Moderate, dataKey: "Transmute (Au)").GetCraftingString();

            if (Globals.Config.CraftingRecipes.Randomize) 
			{ 
				Globals.SpoilerWrite(""); 
			}

			return replacements;
		}
	}
}
