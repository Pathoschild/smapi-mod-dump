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
using System.Reflection;

namespace Randomizer
{
    public class CraftingRecipeAdjustments
	{
		private const int TapperProfession = 7;

		/// <summary>
		/// A mapping of cooking recipes that include crop names to the id of that crop
		/// These ARE needed because the cooking recipe is slightly different than the item name in some cases
		/// </summary>
		public readonly static Dictionary<string, ObjectIndexes> CropDishesMap = new()
		{
			{ "Cheese Cauli.", ObjectIndexes.CheeseCauliflower },
			{ "Parsnip Soup", ObjectIndexes.ParsnipSoup },
			{ "Bean Hotpot", ObjectIndexes.BeanHotpot },
			{ "Glazed Yams", ObjectIndexes.GlazedYams },
			{ "Pepper Poppers", ObjectIndexes.PepperPoppers },
			{ "Rhubarb Pie", ObjectIndexes.RhubarbPie },
			{ "Eggplant Parm.", ObjectIndexes.EggplantParmesan },
			{ "Blueberry Tart", ObjectIndexes.BlueberryTart },
			{ "Pumpkin Soup", ObjectIndexes.PumpkinSoup },
			{ "Cran. Sauce", ObjectIndexes.CranberrySauce },
			{ "Pumpkin Pie", ObjectIndexes.PumpkinPie },
			{ "Radish Salad", ObjectIndexes.RadishSalad },
			{ "Cranberry Candy", ObjectIndexes.CranberryCandy },
			{ "Artichoke Dip", ObjectIndexes.ArtichokeDip },
			{ "Rice Pudding", ObjectIndexes.RicePudding },
			{ "Fruit Salad", ObjectIndexes.FruitSalad },
			{ "Poppyseed Muffin", ObjectIndexes.PoppyseedMuffin }
		};

        /// <summary>
        /// A mapping of cooking recipes that include fish names to the id of that fish
		/// These ARE needed because the cooking recipe is slightly different than the item name in some cases
        /// </summary>
        public readonly static Dictionary<string, ObjectIndexes> FishDishesMap = new()
		{
			{ "Carp Surprise", ObjectIndexes.CarpSurprise },
			{ "Salmon Dinner", ObjectIndexes.SalmonDinner },
			{ "Crispy Bass", ObjectIndexes.CrispyBass },
			{ "Trout Soup", ObjectIndexes.TroutSoup },
			{ "Fried Eel", ObjectIndexes.FriedEel },
			{ "Spicy Eel", ObjectIndexes.SpicyEel }
		};

        /// <summary>
        /// Fix the cooking recipe display names so that the queen of sauce shows
        /// can actually display the correct thing
        /// </summary>
        public static void FixCookingRecipeDisplayNames()
		{
			foreach (KeyValuePair<string, ObjectIndexes> entry in CropDishesMap)
			{
				ObjectIndexes id = entry.Value;
				CookedItem item = (CookedItem)ItemList.Items[id];
				item.OverrideDisplayName = GetDishName(entry.Value);
			}

			foreach (KeyValuePair<string, ObjectIndexes> entry in FishDishesMap)
			{
                ObjectIndexes id = entry.Value;
				CookedItem item = (CookedItem)ItemList.Items[id];
				item.OverrideDisplayName = GetDishName(entry.Value);
			}
		}

		/// <summary>
		/// Fixes the cooking recipe names and the crab pot recipe for the tapper profession if the
		/// appropriate things are randomized
		/// </summary>
		public static void HandleCraftingMenus()
		{
			IClickableMenu genericMenu = Game1.activeClickableMenu;
			if (genericMenu is null) { return; }

			if (genericMenu is CraftingPage craftingPage)
			{
				if (!Globals.Config.Fish.Randomize && !Globals.Config.Crops.Randomize) { return; }
				FixCookingRecipeHoverText(craftingPage);
			}

			else if (genericMenu is GameMenu gameMenu && Game1.player.professions.Contains(TapperProfession))
			{
				if (!Globals.Config.CraftingRecipes.Randomize) { return; }
				ReduceCrabPotCost(gameMenu);
			}
		}

		/// <summary>
		/// Reduces the cost of the crab pot - intended to be used if the player has the Tapper profession
		/// </summary>
		/// <param name="gameMenu">The game menu that needs its cost adjusted</param>
		private static void ReduceCrabPotCost(GameMenu gameMenu)
		{
			CraftingPage craftingPage = (CraftingPage)gameMenu.pages[GameMenu.craftingTab];

			List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes =
				(List<Dictionary<ClickableTextureComponent, CraftingRecipe>>)GetInstanceField(craftingPage, "pagesOfCraftingRecipes");

			foreach (Dictionary<ClickableTextureComponent, CraftingRecipe> page in pagesOfCraftingRecipes)
			{
				foreach (ClickableTextureComponent key in page.Keys)
				{
					CraftingRecipe recipe = page[key];
					if (recipe.name == "Crab Pot")
					{
						CraftableItem crabPot = (CraftableItem)ItemList.Items[ObjectIndexes.CrabPot];
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
			Dictionary<int, int> recipeList = (Dictionary<int, int>)GetInstanceField(inGameRecipe, "recipeList");
			recipeList.Clear();

			List<ObjectIndexes> itemIds = randomizedRecipe.Keys.ToList();
			if (randomizedRecipe.Values.All(x => x < 2))
			{
				ObjectIndexes firstKeyOfEasiestItem = randomizedRecipe.Keys
					.Select(x => ItemList.Items[x])
					.OrderBy(x => x.DifficultyToObtain)
					.Select(x => (ObjectIndexes)x.Id)
					.First();

				foreach (ObjectIndexes id in randomizedRecipe.Keys.Where(x => x != firstKeyOfEasiestItem))
				{
					recipeList.Add((int)id, 1);
				}
			}
			else
			{
				foreach (ObjectIndexes id in randomizedRecipe.Keys)
				{
					int value = randomizedRecipe[id];
					recipeList.Add((int)id, Math.Max(value / 2, 1));
				}
			}
		}

		/// <summary>
		/// Fixes the cooking recipe values if they were changed
		/// </summary>
		private static void FixCookingRecipeHoverText(CraftingPage craftingMenu)
		{
			if (!(bool)GetInstanceField(craftingMenu, "cooking")) { return; }

			List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes =
				(List<Dictionary<ClickableTextureComponent, CraftingRecipe>>)GetInstanceField(craftingMenu, "pagesOfCraftingRecipes");

			foreach (Dictionary<ClickableTextureComponent, CraftingRecipe> page in pagesOfCraftingRecipes)
			{
				foreach (ClickableTextureComponent key in page.Keys)
				{
					CraftingRecipe recipe = page[key];
					FixFishDish(recipe);
					FixCropDish(recipe);
				}
			}
		}

		/// <summary>
		/// Replaces the display name of the given recipe with the renamed fish dish
		/// </summary>
		/// <param name="recipe">The recipe</param>
		private static void FixFishDish(CraftingRecipe recipe)
		{
			if (!Globals.Config.Fish.Randomize) { return; }

			if (FishDishesMap.ContainsKey(recipe.name))
			{
				recipe.DisplayName = GetDishName(FishDishesMap[recipe.name]);
			}
		}

		/// <summary>
		/// Replaces the display name of the given recipe with the renamed crop dish
		/// </summary>
		/// <param name="recipe">The recipe</param>
		private static void FixCropDish(CraftingRecipe recipe)
		{
			if (!Globals.Config.Crops.Randomize) { return; }

			if (CropDishesMap.ContainsKey(recipe.name))
			{
				recipe.DisplayName = GetDishName(CropDishesMap[recipe.name]);
			}
		}

		/// <summary>
		/// Gets the dish name based on the id
		/// </summary>
		/// <param name="id"></param>
		private static string GetDishName(ObjectIndexes id)
		{
			CookedItem item = ItemList.Items[id] as CookedItem;
			return item.DisplayName;
		}

		/// <summary>
		/// Gets the instance value of a field, even if it's private
		/// USE SPARANGLY, THIS KIND OF PRACTICE IS EXTREMELY FROWNED UPON
		/// Credit goes here: https://stackoverflow.com/questions/3303126/how-to-get-the-value-of-private-field-in-c
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="instance">The instance</param>
		/// <param name="fieldName">The field name to get</param>
		/// <returns>The retrieved field</returns>
		private static object GetInstanceField<T>(T instance, string fieldName)
		{
			BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			FieldInfo field = typeof(T).GetField(fieldName, bindFlags);
			return field.GetValue(instance);
		}
	}
}