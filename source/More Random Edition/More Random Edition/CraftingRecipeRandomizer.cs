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
		public static Dictionary<string, string> Randomize()
		{
			if (Globals.Config.CraftingRecipies.Randomize) { Globals.SpoilerWrite($"==== CRAFTING RECIPES ===="); }
			Dictionary<string, string> replacements = new Dictionary<string, string>();
			foreach (CraftableItem item in ItemList.Items.Values.Where(x => x.IsCraftable))
			{
				replacements[item.Name] = item.GetCraftingString();
			}
			if (Globals.Config.CraftingRecipies.Randomize) { Globals.SpoilerWrite(""); }
			return replacements;
		}
	}
}
