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
			if (Globals.Config.RandomizeCraftingRecipes) { Globals.SpoilerWrite($"==== CRAFTING RECIPES ===="); }
			Dictionary<string, string> replacements = new Dictionary<string, string>();
			foreach (CraftableItem item in ItemList.Items.Values.Where(x => x.IsCraftable))
			{
				replacements[item.Name] = item.GetCraftingString();
			}
			if (Globals.Config.RandomizeCraftingRecipes) { Globals.SpoilerWrite(""); }
			return replacements;
		}
	}
}
