/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    public class CraftingMenuAdjustments
	{
		/// <summary>
		/// Reduces the cost of the crab pot
		/// Intended to be used if the player has the Trapper profession
		/// </summary>
		/// <param name="gameMenu">The game menu that needs its cost adjusted</param>
		public static void ReduceCrabPotCost(GameMenu gameMenu)
		{
            const int TrapperProfession = 7;
            if (!Globals.Config.CraftingRecipes.Randomize || 
				!Game1.player.professions.Contains(TrapperProfession)) 
			{ 
				return; 
			}

			CraftingPage craftingPage = (CraftingPage)gameMenu.pages[GameMenu.craftingTab];
			foreach (var page in craftingPage.pagesOfCraftingRecipes)
			{
				foreach (ClickableTextureComponent key in page.Keys)
				{
					CraftingRecipe recipe = page[key];
					if (recipe.name == "Crab Pot")
					{
						CraftableItem crabPot = (CraftableItem)ObjectIndexes.CrabPot.GetItem();
						Dictionary<ObjectIndexes, int> randomizedRecipe = crabPot.LastRecipeGenerated;
						ReduceRecipeCost(page[key], randomizedRecipe);
					}
				}
			}
		}

		/// <summary>
		/// Reduces a recipe's cost
		/// - if everything only needs one of each item, remove the cheapest item
		/// - otherwise - halve the amounts of all items, rounding down (with a min of 1 required)
		/// </summary>
		/// <param name="inGameRecipe">The recipe as stored by Stardew Valley</param>
		/// <param name="randomizedRecipe">The recipe as stored by this mod</param>
		private static void ReduceRecipeCost(
			CraftingRecipe inGameRecipe, 
			Dictionary<ObjectIndexes, int> randomizedRecipe)
		{
			Dictionary<string, int> modifiedRecipe = 
				CraftableItem.ConvertRecipeToUseCategories(randomizedRecipe);
            Dictionary<string, int> recipeList = inGameRecipe.recipeList;
			recipeList.Clear();
			if (modifiedRecipe.Values.All(x => x < 2))
			{
				string firstKeyOfEasiestItem = modifiedRecipe.Keys
					.Where(id => !id.StartsWith("-")) // Removes categories - these will never be the cheapest item in this case
					.Select(id => ItemList.Items[id])
					.OrderBy(item => item.DifficultyToObtain)
					.Select(item => item.Id)
					.First();

				foreach (string index in modifiedRecipe.Keys.Where(x => x != firstKeyOfEasiestItem))
				{
					recipeList.Add(index, 1);
				}
			}
			else
			{
				foreach (string index in modifiedRecipe.Keys)
				{
                    int numberRequired = modifiedRecipe[index];
					recipeList.Add(index.ToString(), Math.Max(numberRequired / 2, 1));
				}
			}
		}
	}
}