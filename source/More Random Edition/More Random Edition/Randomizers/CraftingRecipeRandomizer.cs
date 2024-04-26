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
            Dictionary<string, string> replacements = new();
            if (!Globals.Config.CraftingRecipes.Randomize)
            {
				return replacements;
            }

			Globals.SpoilerWrite($"==== CRAFTING RECIPES ===="); 

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
			const string TransmuteIronName = "Transmute (Fe)";
            const string TransmuteGoldName = "Transmute (Au)";
			const string FakeId = "-1000";
            replacements[TransmuteIronName] = new CraftableItem(
				FakeId, 
				CraftableCategories.Moderate, 
				dataKey: TransmuteIronName).GetCraftingString(TransmuteIronName);
            replacements[TransmuteGoldName] = new CraftableItem(
                FakeId, 
				CraftableCategories.Moderate, 
				dataKey: TransmuteGoldName).GetCraftingString(TransmuteGoldName);

			Globals.SpoilerWrite("");

			return replacements;
		}
	}
}
