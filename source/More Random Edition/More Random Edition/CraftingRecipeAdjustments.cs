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
		/// </summary>
		private static Dictionary<string, int> CropDishesMap = new Dictionary<string, int>
		{
			{ "Cheese Cauli.", (int)ObjectIndexes.CheeseCauliflower },
			{ "Parsnip Soup", (int)ObjectIndexes.ParsnipSoup },
			{ "Bean Hotpot", (int)ObjectIndexes.BeanHotpot },
			{ "Glazed Yams", (int)ObjectIndexes.GlazedYams },
			{ "Pepper Poppers", (int)ObjectIndexes.PepperPoppers },
			{ "Rhubarb Pie", (int)ObjectIndexes.RhubarbPie },
			{ "Eggplant Parm.", (int)ObjectIndexes.EggplantParmesan },
			{ "Blueberry Tart", (int)ObjectIndexes.BlueberryTart },
			{ "Pumpkin Soup", (int)ObjectIndexes.PumpkinSoup },
			{ "Cran. Sauce", (int)ObjectIndexes.CranberrySauce },
			{ "Pumpkin Pie", (int)ObjectIndexes.PumpkinPie },
			{ "Radish Salad", (int)ObjectIndexes.RadishSalad },
			{ "Cranberry Candy", (int)ObjectIndexes.CranberryCandy },
			{ "Artichoke Dip", (int)ObjectIndexes.ArtichokeDip },
			{ "Rice Pudding", (int)ObjectIndexes.RicePudding },
			{ "Fruit Salad", (int)ObjectIndexes.FruitSalad },
			{ "Poppyseed Muffin", (int)ObjectIndexes.PoppyseedMuffin }
		};

		/// <summary>
		/// A mapping of cooking recipes that include fish names to the id of that fish
		/// </summary>
		private static Dictionary<string, int> FishDishesMap = new Dictionary<string, int>
		{
			{ "Carp Surprise", (int)ObjectIndexes.CarpSurprise },
			{ "Salmon Dinner", (int)ObjectIndexes.SalmonDinner },
			{ "Crispy Bass", (int)ObjectIndexes.CrispyBass },
			{ "Trout Soup", (int)ObjectIndexes.TroutSoup },
			{ "Fried Eel", (int)ObjectIndexes.FriedEel },
			{ "Spicy Eel", (int)ObjectIndexes.SpicyEel }
		};

		/// <summary>
		/// Fixes the cooking recipe names and the crab pot recipe for the tapper profession if the
		/// appropriate things are randomized
		/// </summary>
		public static void HandleCraftingMenus()
		{
			IClickableMenu genericMenu = Game1.activeClickableMenu;
			if (genericMenu is null) { return; }

			if (genericMenu is CraftingPage)
			{
				if (!Globals.Config.RandomizeFish && !Globals.Config.RandomizeCrops) { return; }
				FixCookingRecipeHoverText((CraftingPage)genericMenu);
			}

			else if (genericMenu is GameMenu && Game1.player.professions.Contains(TapperProfession))
			{
				if (!Globals.Config.RandomizeCraftingRecipes) { return; }
				ReduceCrabPotCost((GameMenu)genericMenu);
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
						CraftableItem crabPot = (CraftableItem)ItemList.Items[(int)ObjectIndexes.CrabPot];
						Dictionary<int, int> randomizedRecipe = crabPot.LastRecipeGenerated;
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
		private static void ReduceRecipeCost(CraftingRecipe inGameRecipe, Dictionary<int, int> randomizedRecipe)
		{
			Dictionary<int, int> recipeList = (Dictionary<int, int>)GetInstanceField(inGameRecipe, "recipeList");
			recipeList.Clear();

			List<int> itemIds = randomizedRecipe.Keys.ToList();
			if (randomizedRecipe.Values.All(x => x < 2))
			{
				int firstKeyOfEasiestItem = randomizedRecipe.Keys
					.Select(x => ItemList.Items[x])
					.OrderBy(x => x.DifficultyToObtain)
					.Select(x => x.Id)
					.First();

				foreach (int id in randomizedRecipe.Keys.Where(x => x != firstKeyOfEasiestItem))
				{
					recipeList.Add(id, 1);
				}
			}
			else
			{
				foreach (int id in randomizedRecipe.Keys)
				{
					int value = randomizedRecipe[id];
					recipeList.Add(id, Math.Max(value / 2, 1));
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
			if (!Globals.Config.RandomizeFish) { return; }

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
			if (!Globals.Config.RandomizeCrops) { return; }

			if (CropDishesMap.ContainsKey(recipe.name))
			{
				recipe.DisplayName = GetDishName(CropDishesMap[recipe.name]);
			}
		}

		/// <summary>
		/// Gets the dish name based on the id
		/// </summary>
		/// <param name="id"></param>
		private static string GetDishName(int id)
		{
			CookedItem item = (CookedItem)ItemList.Items[id];
			string nameAndDescription = Globals.GetTranslation($"item-{id}-name-and-description", new { itemName = item.IngredientName });
			return nameAndDescription.Split('/')[0];
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