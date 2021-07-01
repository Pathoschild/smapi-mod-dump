/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Menus;
using Microsoft.Xna.Framework;

namespace LoveOfCooking.GameObjects
{
	public class CookingManager
	{
		private readonly CookingMenu _cookingMenu;
		private const int DefaultIngredientsSlots = 5;
		internal int FirstEmptySlot => CurrentIngredients.FindIndex(i => i == null);
		internal bool AreAllIngredientSlotsFilled => CurrentIngredients.GetRange(0, MaxIngredients).TrueForAll(i => i != null);
		private int _maxIngredients;
		internal int MaxIngredients
		{
			get => _maxIngredients;
			set
			{
				_maxIngredients = value;
				CurrentIngredients = new List<Ingredient?>(_maxIngredients);
			}
		}
		private List<Ingredient?> _currentIngredients;
		internal List<Ingredient?> CurrentIngredients
		{
			get => _currentIngredients;
			private set
			{
				_currentIngredients = value;
				for (var i = 0; i < Math.Max(DefaultIngredientsSlots, MaxIngredients); ++i)
				{
					_currentIngredients.Add(null);
				}
				_cookingMenu.InitialiseIngredientSlotButtons(buttonsToDisplay: _currentIngredients.Count, usableButtons: MaxIngredients);
			}
		}

		internal struct Ingredient
		{
			public int InventoryId;
			public int ItemIndex;

			public Ingredient(int inventory, int index)
			{
				InventoryId = inventory;
				ItemIndex = index;
			}
		}

		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static Config Config => ModEntry.Instance.Config;

		public CookingManager(CookingMenu cookingMenu)
		{
			_cookingMenu = cookingMenu;
		}

		/// <summary>
		/// Calculates the chance to 'burn' an object when cooking its recipe,
		/// reducing the expected outcome stack, and awarding the player a less-useful consolation object.
		/// </summary>
		public static float GetBurnChance(CraftingRecipe recipe)
		{
			if (!Config.FoodCanBurn || ModEntry.JsonAssets == null)
				return 0f;

			/*if (Config.DebugMode)
			{
				// test values
				List<double> results = new List<double>();
				for (double i = 0d; i < 5d; ++i)
				{
					results.Add(Math.Log(i, Math.E));
				}
			}*/

			int cookingLevel = ModEntry.CookingSkillApi.GetLevel();
			float baseRate = float.Parse(ModEntry.ItemDefinitions["BurnChanceBase"][0]);
			float addedRate = float.Parse(ModEntry.ItemDefinitions["BurnChancePerIngredient"][0]);
			float chance = Math.Max(0f, (baseRate + (addedRate * recipe.getNumberOfIngredients()))
				- cookingLevel * CookingSkill.BurnChanceModifier * CookingSkill.BurnChanceReduction
				- (ModEntry.Instance.States.Value.CookingToolLevel / 2f) * CookingSkill.BurnChanceModifier * CookingSkill.BurnChanceReduction);

			return chance;
		}

		/// <summary>
		/// Checks whether an item can be used in cooking recipes.
		/// Doesn't check for edibility; oil, vinegar, jam, honey, wood, etc are inedible yet used in some recipes.
		/// </summary>
		public static bool CanBeCooked(Item item)
		{
			return !(item == null || item is Tool || item is Furniture || item is Ring || item is Clothing || item is Boots || item is Hat || item is Wallpaper
				|| item.Category < -90 || item.isLostItem || !item.canBeTrashed()
				|| (item is StardewValley.Object o && (o.bigCraftable.Value || o.specialItem)));
		}

		/// <summary>
		/// Checks whether an item can be consumed to improve the quality of cooked recipes.
		/// </summary>
		public static bool IsSeasoning(Item item)
		{
			return item != null && (item.ParentSheetIndex == 917 || item.Name.EndsWith("Oil"));
		}

		/// <summary>
		/// Checks whether an item is equivalent to another for the purposes of being used as an ingredient in a cooking recipe.
		/// </summary>
		/// <param name="id">Identifier for matching, is considered an item category if negative.</param>
		/// <param name="item">Item to compare identifier against.</param>
		public static bool IsMatchingIngredient(int id, Item item)
		{
			return id > 0
				? item.ParentSheetIndex == id
				: item.Category == id;
		}

		/// <summary>
		/// Find all items in some lists that work as an ingredient or substitute in a cooking recipe for some given requirement.
		/// </summary>
		/// <param name="id">The required item's identifier for the recipe, given as an index or category.</param>
		/// <param name="sourceItems">Container of items in which to seek a match.</param>
		/// <param name="required">Stack quantity required to fulfil this ingredient in its recipe.</param>
		/// <param name="limit">Maximum number of matching ingredients to return.</param>
		internal static List<Ingredient> GetMatchingIngredients(int id, List<IList<Item>> sourceItems, int required, int limit = DefaultIngredientsSlots)
		{
			List<Ingredient> foundIngredients = new List<Ingredient>();
			int ingredientsFulfilled = 0;
			int ingredientsRequired = required;
			for (int i = 0; i < sourceItems.Count; ++i)
			{
				for (int j = 0; j < sourceItems[i].Count && ingredientsFulfilled < limit; ++j)
				{
					if (CanBeCooked(sourceItems[i][j])
						&& (IsMatchingIngredient(id: id, item: sourceItems[i][j])
							|| CraftingRecipe.isThereSpecialIngredientRule((StardewValley.Object)sourceItems[i][j], id)))
					{
						// Mark ingredient as matched
						Ingredient ingredient = new Ingredient(inventory: i, index: j);
						foundIngredients.Add(ingredient);

						// Count up number of fulfilled ingredients
						// Ingredients may require multiple items each if stacks are small enough, or requirement is large enough
						ingredientsRequired -= sourceItems[i][j].Stack;
						if (ingredientsRequired < 0)
						{
							++ingredientsFulfilled;
							ingredientsRequired = required;
						}
					}
				}
			}
			return foundIngredients;
		}

		public int GetAmountCraftable(CraftingRecipe recipe, List<IList<Item>> sourceItems, bool limitToCurrentIngredients)
		{
			int count = -1;
			if (recipe == null)
				return 0;
			foreach (KeyValuePair<int, int> itemAndQuantity in recipe.recipeList)
			{
				int countForThisIngredient = 0;
				int requiredToCook = itemAndQuantity.Value;
				if (limitToCurrentIngredients)
				{
					// Check amount craftable considering current ingredients
					for (int i = 0; i < CurrentIngredients.Count; ++i)
					{
						bool hasValue = CurrentIngredients[i].HasValue;
						Item item = hasValue ? this.GetItemForIngredient(index: i, sourceItems: sourceItems) : null;
						bool isMatch = item != null && IsMatchingIngredient(id: itemAndQuantity.Key, item: item);
						if (hasValue && item != null && isMatch)
						{
							countForThisIngredient += item.Stack / requiredToCook;
						}
						else
						{
							continue;
						}
					}
				}
				else
				{
					// Check amount craftable regardless of current ingredients
					if (CookingManager.GetMatchingIngredients(id: itemAndQuantity.Key, sourceItems: sourceItems, required: itemAndQuantity.Value)
							is List<Ingredient> ingredients
						&& ingredients != null && ingredients.Count > 0)
					{
						countForThisIngredient = ingredients.Sum(
							i => this.GetItemForIngredient(ingredient: i, sourceItems: sourceItems).Stack) / requiredToCook;
					}
				}
				if (countForThisIngredient < 1)
				{
					return 0;
				}
				count = count == -1 ? countForThisIngredient : Math.Min(count, countForThisIngredient);
			}
			return count;
		}

		/// <summary>
		/// Identify inventory items tracked in <see cref="CurrentIngredients"/> required to craft a recipe.
		/// Recipes may require more than one inventory item to fulfill one ingredient,
		/// for example, recipes requiring multiple of a category ingredient.
		/// </summary>
		/// <param name="recipe">Recipe we would like to craft.</param>
		/// <param name="sourceItems">Container of items referenced by <see cref="CurrentIngredients"/>.</param>
		/// <returns>
		/// List of <see cref="Ingredient"/> references tracking indexes and quantities of items to consume when crafting.
		/// Null if not all required items were found.
		/// </returns>
		internal Dictionary<int, int> ChooseIngredientsForCrafting(CraftingRecipe recipe, List<IList<Item>> sourceItems)
		{
			Dictionary<int, int> ingredientsToConsume = new Dictionary<int, int>();
			foreach (KeyValuePair<int, int> itemAndQuantity in recipe.recipeList)
			{
				int remainingRequired = itemAndQuantity.Value;
				for (int i = 0; i < CurrentIngredients.Count && remainingRequired > 0; ++i)
				{
					Item item = this.GetItemForIngredient(index: i, sourceItems: sourceItems);
					if (item == null)
					{
						CurrentIngredients[i] = null; // No items were found for this ingredient, prevent it being checked later
						return null;
					}
					if (IsMatchingIngredient(id: itemAndQuantity.Key, item: item))
					{
						// Mark ingredient for consumption and check remaining count before consuming other ingredients
						int quantityToConsume = Math.Min(remainingRequired, item.Stack);
						remainingRequired -= quantityToConsume;
						ingredientsToConsume.Add(i, quantityToConsume);
					}
				}
				if (remainingRequired > 0)
				{
					// Abort search if any required ingredients aren't fulfilled
					return null;
				}
			}

			return ingredientsToConsume;
		}

		internal List<StardewValley.Object> CraftItemAndConsumeIngredients(CraftingRecipe recipe, List<IList<Item>> sourceItems, int quantity)
		{
			Dictionary<int, int> ingredientsToConsume = this.ChooseIngredientsForCrafting(recipe: recipe, sourceItems: sourceItems);
			Dictionary<int, int> qualityStacks = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 4, 0 } };
			int numPerCraft = recipe.numberProducedPerCraft;

			for (int i = 0; i < quantity && ingredientsToConsume != null; ++i)
			{
				// Consume ingredients from source lists
				foreach (KeyValuePair<int, int> indexAndQuantity in ingredientsToConsume.ToList())
				{
					Ingredient ingredient = CurrentIngredients[indexAndQuantity.Key].Value;
					if ((sourceItems[ingredient.InventoryId][ingredient.ItemIndex].Stack -= indexAndQuantity.Value) < 1)
					{
						if (ingredient.InventoryId == CookingMenu.BackpackInventoryId)
						{
							// Clear item slot in player's inventory
							sourceItems[ingredient.InventoryId][ingredient.ItemIndex] = null;
						}
						else
						{
							// Clear item and ensure no gaps are left in inventory for fridges and chests
							sourceItems[ingredient.InventoryId].RemoveAt(ingredient.ItemIndex);
							// Adjust other ingredients accordingly
							for (int j = 0; j < CurrentIngredients.Count; ++j)
							{
								if (CurrentIngredients[j].HasValue
									&& CurrentIngredients[j].Value.InventoryId == ingredient.InventoryId
									&& CurrentIngredients[j].Value.ItemIndex > ingredient.ItemIndex)
								{
									CurrentIngredients[j] = new Ingredient(CurrentIngredients[j].Value.InventoryId, CurrentIngredients[j].Value.ItemIndex - 1);
								}
							}
						}
					}
				}

				// Add to stack
				qualityStacks[0] += numPerCraft;

				// Apply extra portion bonuses to the amount cooked
				if (ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.ExtraPortion) && ModEntry.CookingSkillApi.RollForExtraPortion())
				{
					qualityStacks[0] += numPerCraft;
				}

				// Choose new ingredients until none are found
				ingredientsToConsume = this.ChooseIngredientsForCrafting(recipe: recipe, sourceItems: sourceItems);
			}

			// Apply oil quality bonuses to the stack choices
			bool hasOilPerk = ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.ImprovedOil);

			// Gather seasonings as ingredients to use for improving cooked item qualities
			bool limitToCurrentIngredients = !bool.Parse(ModEntry.ItemDefinitions["AutoConsumeOilAndSeasoning"][0]);
			List<Ingredient> ingredientsForSeasonings = new List<Ingredient>();
			if (limitToCurrentIngredients)
			{
				ingredientsForSeasonings = CurrentIngredients.Where(i => i.HasValue).Select(i => i.Value)
					.Where(i => IsSeasoning(item: sourceItems[i.InventoryId][i.ItemIndex])).ToList();
			}
			else
			{
				for (int i = 0; i < sourceItems.Count; ++i)
				{
					for (int j = 0; j < sourceItems[i].Count; ++j)
					{
						if (!IsSeasoning(item: sourceItems[i][j]))
							continue;
						Ingredient ingredient = new Ingredient(inventory: i, index: j);
						ingredientsForSeasonings.Add(ingredient);
					}
				}
			}

			// Consume best seasonings first, assuming seasonings follow the trend of higher-index = higher-quality
			ingredientsForSeasonings = ingredientsForSeasonings.OrderByDescending(i => sourceItems[i.InventoryId][i.ItemIndex].ParentSheetIndex).ToList();

			// Consume seasoning items from ingredients to improve the recipe output item qualities, rebalancing the stack numbers per quality item
			foreach (Ingredient ingredient in ingredientsForSeasonings)
			{
				while (qualityStacks[0] > 0) // Stop iterating when we've run out of standard quality ingredients
				{
					// Reduce the base quality stack
					qualityStacks[0] -= numPerCraft;

					// Increase higher quality stacks
					switch (sourceItems[ingredient.InventoryId][ingredient.ItemIndex].ParentSheetIndex)
					{
						case 917: // Qi Seasoning
							qualityStacks[2] += numPerCraft;
							break;
						case 432: // Truffle Oil
							qualityStacks[hasOilPerk ? 4 : 2] += numPerCraft;
							break;
						case 247: // Oil
							qualityStacks[hasOilPerk ? 2 : 1] += numPerCraft;
							break;
						default: // Seasonings not yet discovered by science
							qualityStacks[hasOilPerk ? 4 : 2] += numPerCraft;
							break;
					}

					// Remove consumed seasonings
					if (--sourceItems[ingredient.InventoryId][ingredient.ItemIndex].Stack < 1)
					{
						if (ingredient.InventoryId == CookingMenu.BackpackInventoryId)
						{
							// Clear item slot in player's inventory
							sourceItems[ingredient.InventoryId][ingredient.ItemIndex] = null;
						}
						else
						{
							// Clear item and ensure no gaps are left in inventory for fridges and chests
							sourceItems[ingredient.InventoryId].RemoveAt(ingredient.ItemIndex);
						}
						// Stop iterating when we've run out of this seasoning item
						break;
					}
				}
			}

			// Apply burn chance to destroy cooked food at random
			int burntCount = 0;
			List<int> qualities = qualityStacks.Keys.ToList();
			foreach (int quality in qualities)
			{
				for (int i = qualityStacks[quality] - 1; i >= 0; i -= numPerCraft)
				{
					if (GetBurnChance(recipe) > Game1.random.NextDouble())
					{
						qualityStacks[quality] -= numPerCraft;
						++burntCount;
					}
				}
			}

			// Create item list from quality stacks
			List<StardewValley.Object> itemsCooked = new List<StardewValley.Object>();
			foreach (KeyValuePair<int, int> pair in qualityStacks.Where(pair => pair.Value > 0))
			{
				var item = recipe.createItem() as StardewValley.Object;
				item.Quality = pair.Key;
				item.Stack = pair.Value;
				itemsCooked.Add(item);
			}

			CookingMenu.LastBurntCount = burntCount;
			return itemsCooked;
		}

		internal int CookRecipe(CraftingRecipe recipe, List<IList<Item>> sourceItems, int quantity)
		{
			// Craft items to be cooked from recipe
			List<StardewValley.Object> itemsCooked = this.CraftItemAndConsumeIngredients(recipe, sourceItems, quantity);
			int quantityCooked = Math.Max(0, (itemsCooked.Sum(item => item.Stack) / recipe.numberProducedPerCraft) - CookingMenu.LastBurntCount);
			Item item = recipe.createItem();

			// Track experience for items cooked
			if (Config.AddCookingSkillAndRecipes)
			{
				if (!ModEntry.Instance.States.Value.FoodCookedToday.ContainsKey(recipe.name))
					ModEntry.Instance.States.Value.FoodCookedToday[recipe.name] = 0;
				ModEntry.Instance.States.Value.FoodCookedToday[recipe.name] += quantity;

				ModEntry.CookingSkillApi.CalculateExperienceGainedFromCookingItem(item: item, recipe.getNumberOfIngredients(), quantityCooked, applyExperience: true);
				Game1.player.cookedRecipe(item.ParentSheetIndex);

				// Update game stats
				Game1.stats.ItemsCooked += (uint)quantityCooked;
				Game1.player.checkForQuestComplete(null, -1, -1, item, null, 2);
				Game1.stats.checkForCookingAchievements();
			}

			// Add cooked items to inventory if possible
			foreach (StardewValley.Object cookedItem in itemsCooked)
			{
				ModEntry.AddOrDropItem(cookedItem);
			}

			// Add burnt items
			if (CookingMenu.LastBurntCount > 0)
			{
				Item burntItem = new StardewValley.Object(ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "burntfood"), CookingMenu.LastBurntCount);
				ModEntry.AddOrDropItem(burntItem);
			}

			return CookingMenu.LastBurntCount;
		}

		internal bool AddToIngredients(int inventoryId, int itemIndex)
		{
			Ingredient ingredient = new Ingredient(inventory: inventoryId, index: itemIndex);
			return this.AddToIngredients(ingredient: ingredient);
		}

		internal bool AddToIngredients(Ingredient ingredient)
		{
			if (FirstEmptySlot < 0 || FirstEmptySlot >= MaxIngredients)
				return false;
			CurrentIngredients[FirstEmptySlot] = ingredient;
			return true;
		}

		internal bool RemoveFromIngredients(int inventoryId, int itemIndex)
		{
			int index = CurrentIngredients.FindIndex(i => i.HasValue && i.Value.InventoryId == inventoryId && i.Value.ItemIndex == itemIndex);
			if (index < 0)
				return false;
			CurrentIngredients[index] = null;
			return true;
		}

		internal bool RemoveFromIngredients(int ingredientsIndex)
		{
			if (ingredientsIndex < 0 || ingredientsIndex >= CurrentIngredients.Count || CurrentIngredients[ingredientsIndex] == null)
				return false;
			CurrentIngredients[ingredientsIndex] = null;
			return true;
		}

		internal void AutoFillIngredients(CraftingRecipe recipe, List<IList<Item>> sourceItems)
		{
			// Don't fill slots if the player isn't able to cook the recipe
			if (recipe == null || MaxIngredients < recipe.recipeList.Count || this.GetAmountCraftable(recipe: recipe, sourceItems: sourceItems, limitToCurrentIngredients: false) < 1)
				return;

			// Fill slots with ingredients
			List<Ingredient> ingredients = recipe.recipeList.SelectMany(itemAndQuantity => GetMatchingIngredients(
					id: itemAndQuantity.Key, sourceItems: sourceItems, required: itemAndQuantity.Value)).ToList();
			if (ingredients == null || ingredients.Count == 0)
				return;
			ingredients = ingredients.OrderByDescending(i => i.InventoryId).ThenByDescending(i => i.ItemIndex).ToList();
			foreach (Ingredient ingredient in ingredients)
			{
				this.AddToIngredients(ingredient);
			}
		}

		internal void ClearCurrentIngredients()
		{
			for (int i = 0; i < CurrentIngredients.Count; ++i)
			{
				CurrentIngredients[i] = null;
			}
		}

		internal Item GetItemForIngredient(int index, List<IList<Item>> sourceItems)
		{
			Item item = CurrentIngredients.Count > index && CurrentIngredients[index].HasValue
				? this.GetItemForIngredient(ingredient: CurrentIngredients[index].Value, sourceItems: sourceItems)
				: null;
			return item;
		}

		internal Item GetItemForIngredient(Ingredient ingredient, List<IList<Item>> sourceItems)
		{
			Item item = ingredient.InventoryId >= 0 && sourceItems.Count > ingredient.InventoryId 
					&& ingredient.ItemIndex >= 0 && sourceItems[ingredient.InventoryId].Count > ingredient.ItemIndex
				? sourceItems[ingredient.InventoryId][ingredient.ItemIndex]
				: null;
			return item;
		}

		internal bool IsInventoryItemInCurrentIngredients(int inventoryIndex, int itemIndex)
		{
			return CurrentIngredients.Any(i => i.HasValue && i.Value.InventoryId == inventoryIndex && i.Value.ItemIndex == itemIndex);
		}
	}
}
