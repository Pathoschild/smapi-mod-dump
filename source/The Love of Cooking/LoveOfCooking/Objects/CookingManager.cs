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

namespace LoveOfCooking.Objects
{
	public class CookingManager
	{
		private readonly CookingMenu _cookingMenu;
		private const int DefaultIngredientsSlots = 5;
		internal int FirstEmptySlot => this.CurrentIngredients
			.FindIndex(i => i is null);
		internal bool AreAllIngredientSlotsFilled => this.CurrentIngredients
			.GetRange(0, this.MaxIngredients)
			.TrueForAll(i => i is not null);
		private int _maxIngredients;
		internal int MaxIngredients
		{
			get => this._maxIngredients;
			set
			{
				this._maxIngredients = value;
				this.CurrentIngredients = new List<Ingredient?>(this._maxIngredients);
			}
		}
		private List<Ingredient?> _currentIngredients;
		internal List<Ingredient?> CurrentIngredients
		{
			get => _currentIngredients;
			private set
			{
				this._currentIngredients = value;
				for (var i = 0; i < Math.Max(CookingManager.DefaultIngredientsSlots, this.MaxIngredients); ++i)
				{
					_currentIngredients.Add(null);
				}
				this._cookingMenu.InitialiseIngredientSlotButtons(
					buttonsToDisplay: this._currentIngredients.Count,
					usableButtons: this.MaxIngredients);
			}
		}

		internal struct Ingredient
		{
			public int WhichInventory;
			public int WhichItem;
			public int ItemId;

			public Ingredient(int whichInventory, int whichItem, int itemId)
			{
				WhichInventory = whichInventory;
				WhichItem = whichItem;
				ItemId = itemId;
			}

			public static bool operator ==(Ingredient obj1, object obj2)
			{
				return obj2 is Ingredient other
					&& obj1.WhichInventory == other.WhichInventory && obj1.WhichItem == other.WhichItem && obj1.ItemId == other.ItemId;
			}

			public static bool operator !=(Ingredient obj1, object obj2)
			{
				return !(obj1 == obj2);
			}

			public override bool Equals(object obj)
			{
				return this == obj;
			}
		}


		public CookingManager(CookingMenu cookingMenu)
		{
			this._cookingMenu = cookingMenu;
		}

		/// <summary>
		/// Calculates the chance to 'burn' an object when cooking its recipe,
		/// reducing the expected outcome stack, and awarding the player a less-useful consolation object.
		/// </summary>
		public static float GetBurnChance(CraftingRecipe recipe)
		{
			float minimumChance = 0f;
			if (!ModEntry.Config.FoodCanBurn || Interface.Interfaces.JsonAssets is null)
				return minimumChance;

			int cookingLevel = ModEntry.CookingSkillApi.GetLevel();
			float baseRate = float.Parse(ModEntry.ItemDefinitions["BurnChanceBase"][0]);
			float addedRate = float.Parse(ModEntry.ItemDefinitions["BurnChancePerIngredient"][0]);
			float chance = Math.Max(minimumChance, (baseRate + (addedRate * recipe.getNumberOfIngredients()))
				- cookingLevel * CookingSkill.BurnChanceModifier * CookingSkill.BurnChanceReduction
				- (Objects.CookingTool.GetEffectiveGlobalToolUpgradeLevel() / 2f) * CookingSkill.BurnChanceModifier * CookingSkill.BurnChanceReduction);

			return chance;
		}

		/// <summary>
		/// Checks whether an item can be used in cooking recipes.
		/// Doesn't check for edibility; oil, vinegar, jam, honey, wood, etc are inedible yet used in some recipes.
		/// </summary>
		public static bool CanBeCooked(Item item)
		{
			return !(item is null or Tool or Furniture or Ring or Clothing or Boots or Hat or Wallpaper
				|| item.Category < -90 || item.isLostItem || !item.canBeTrashed()
				|| (item is StardewValley.Object o && (o.bigCraftable.Value || o.specialItem)));
		}

		/// <summary>
		/// Checks whether an item can be consumed to improve the quality of cooked recipes.
		/// </summary>
		public static bool IsSeasoning(Item item)
		{
			return item is not null && (item.ParentSheetIndex == 917 || item.Name.EndsWith("Oil"));
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
					if (CookingManager.CanBeCooked(sourceItems[i][j])
						&& (CookingManager.IsMatchingIngredient(id: id, item: sourceItems[i][j])
							|| CraftingRecipe.isThereSpecialIngredientRule((StardewValley.Object)sourceItems[i][j], id)))
					{
						// Mark ingredient as matched
						Ingredient ingredient = new Ingredient(whichInventory: i, whichItem: j, itemId: sourceItems[i][j].ParentSheetIndex);
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

		private int GetAmountCraftable(CraftingRecipe recipe, List<IList<Item>> sourceItems, List<Ingredient> ingredients)
        {
			List<IList<Item>> ingredientsItems = new List<IList<Item>>
			{
				ingredients.Select(i => GetItemForIngredient(ingredient: i, sourceItems: sourceItems)).ToList()
			};
			return this.GetAmountCraftable(recipe: recipe, sourceItems: ingredientsItems, limitToCurrentIngredients: true);
        }

		public int GetAmountCraftable(CraftingRecipe recipe, List<IList<Item>> sourceItems, bool limitToCurrentIngredients)
		{
			int count = -1;
			if (recipe is null)
				return 0;
			foreach (KeyValuePair<int, int> itemAndQuantity in recipe.recipeList)
			{
				int countForThisIngredient = 0;
				int requiredToCook = itemAndQuantity.Value;
				if (limitToCurrentIngredients)
				{
					// Check amount craftable considering current ingredients
					for (int i = 0; i < this.CurrentIngredients.Count; ++i)
					{
						bool hasValue = this.CurrentIngredients[i].HasValue;
						Item item = hasValue ? this.GetItemForIngredient(index: i, sourceItems: sourceItems) : null;
						bool isMatch = item is not null && CookingManager.IsMatchingIngredient(id: itemAndQuantity.Key, item: item);
						if (hasValue && item is not null && isMatch)
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
						&& ingredients is not null && ingredients.Count > 0)
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
				for (int i = 0; i < this.CurrentIngredients.Count && remainingRequired > 0; ++i)
				{
					if (this.CurrentIngredients[i] is null)
						continue;

					Item item = this.GetItemForIngredient(index: i, sourceItems: sourceItems);
					if (item is null)
					{
						this.CurrentIngredients[i] = null; // No items were found for this ingredient, prevent it being checked later
						continue;
					}
					if (CookingManager.IsMatchingIngredient(id: itemAndQuantity.Key, item: item))
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
			{
				string msg1 = $"Cooking {recipe.name} x{quantity}";
				string msg2 = recipe.recipeList.Aggregate("Requires: ", (str, pair) => $"{str} ({pair.Key} x{pair.Value})");
				string msg3 = sourceItems.Aggregate("Sources: ", (str, list) => $"{str} ({sourceItems.IndexOf(list)} x{list.Count})");
				string msg4 = this.CurrentIngredients.Aggregate("Current: ", (str, i) => $"{str} [{(i.HasValue ? i.Value.WhichInventory + ", " + i.Value.WhichItem : "null")}]");
				Log.D(string.Join(Environment.NewLine, new[] { msg1, msg2, msg3, msg4 }),
					ModEntry.Config.DebugMode);
			}

			// Identify items to be consumed from inventory to fulfil ingredients requirements
			Dictionary<int, int> ingredientsToConsume = this.ChooseIngredientsForCrafting(recipe: recipe, sourceItems: sourceItems);
			// Set up dictionary for populating with quantities of different quality levels
			Dictionary<int, int> qualityStacks = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 4, 0 } };
			int numPerCraft = recipe.numberProducedPerCraft;

			{
				string msg1 = "Indices: " + (ingredientsToConsume is not null
					? ingredientsToConsume.Aggregate("", (str, pair) => $"{str} ({pair.Key} {pair.Value})")
					: "null");
				Log.D($"{msg1}",
					ModEntry.Config.DebugMode);
			}

			for (int i = 0; i < quantity && ingredientsToConsume is not null; ++i)
			{
				// Consume ingredients from source lists
				foreach (KeyValuePair<int, int> indexAndQuantity in ingredientsToConsume.ToList())
				{
					Ingredient ingredient = CurrentIngredients[indexAndQuantity.Key].Value;
					if ((sourceItems[ingredient.WhichInventory][ingredient.WhichItem].Stack -= indexAndQuantity.Value) < 1)
					{
						if (ingredient.WhichInventory == CookingMenu.BackpackInventoryId)
						{
							// Clear item slot in player's inventory
							sourceItems[ingredient.WhichInventory][ingredient.WhichItem] = null;
						}
						else
						{
							// Clear item and ensure no gaps are left in inventory for fridges and chests
							sourceItems[ingredient.WhichInventory].RemoveAt(ingredient.WhichItem);
							// Adjust other ingredients accordingly
							for (int j = 0; j < this.CurrentIngredients.Count; ++j)
							{
								if (this.CurrentIngredients[j].HasValue
									&& this.CurrentIngredients[j].Value.WhichInventory == ingredient.WhichInventory
									&& this.CurrentIngredients[j].Value.WhichItem > ingredient.WhichItem)
								{
									this.CurrentIngredients[j] = new Ingredient(
										whichInventory: this.CurrentIngredients[j].Value.WhichInventory,
										whichItem: this.CurrentIngredients[j].Value.WhichItem - 1,
										itemId: this.CurrentIngredients[j].Value.ItemId);
								}
							}
						}
					}
				}

				// Add to stack
				qualityStacks[0] += numPerCraft;

				// Apply extra portion bonuses to the amount cooked
				if (ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.ExtraPortion)
					&& ModEntry.CookingSkillApi.RollForExtraPortion())
				{
					qualityStacks[0] += numPerCraft;
				}

				// Choose new ingredients until none are found
				ingredientsToConsume = this.ChooseIngredientsForCrafting(recipe: recipe, sourceItems: sourceItems);
			}

			// Apply oil quality bonuses to the stack choices
			bool hasOilPerk = ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.ImprovedOil);

			// Gather seasonings as ingredients to use for improving cooked item qualities
			bool consumeSeasoningsFromInventory = bool.Parse(ModEntry.ItemDefinitions["AutoConsumeOilAndSeasoning"][0]);
			List<Ingredient> ingredientsForSeasonings = new List<Ingredient>();
			if (consumeSeasoningsFromInventory)
			{
				for (int i = 0; i < sourceItems.Count; ++i)
				{
					for (int j = 0; j < sourceItems[i].Count; ++j)
					{
						if (!CookingManager.IsSeasoning(item: sourceItems[i][j]))
							continue;
						Ingredient ingredient = new Ingredient(whichInventory: i, whichItem: j, itemId: sourceItems[i][j].ParentSheetIndex);
						ingredientsForSeasonings.Add(ingredient);
					}
				}
			}
			else
			{
				ingredientsForSeasonings = this.CurrentIngredients
					.Where(i => i.HasValue)
					.Select(i => i.Value)
					.Where(i => CookingManager.IsSeasoning(item: sourceItems[i.WhichInventory][i.WhichItem]))
					.ToList();
			}

			// Consume best seasonings first, assuming seasonings follow the trend of higher-index = higher-quality
			ingredientsForSeasonings = ingredientsForSeasonings
				.OrderByDescending(i => sourceItems[i.WhichInventory][i.WhichItem].ParentSheetIndex)
				.ToList();

			// Consume seasoning items from ingredients to improve the recipe output item qualities, rebalancing the stack numbers per quality item
			foreach (Ingredient ingredient in ingredientsForSeasonings)
			{
				while (qualityStacks[0] > 0) // Stop iterating when we've run out of standard quality ingredients
				{
					// Reduce the base quality stack
					qualityStacks[0] -= numPerCraft;

					// Increase higher quality stacks
					switch (sourceItems[ingredient.WhichInventory][ingredient.WhichItem].ParentSheetIndex)
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
					if (--sourceItems[ingredient.WhichInventory][ingredient.WhichItem].Stack < 1)
					{
						if (ingredient.WhichInventory == CookingMenu.BackpackInventoryId)
						{
							// Clear item slot in player's inventory
							sourceItems[ingredient.WhichInventory][ingredient.WhichItem] = null;
						}
						else
						{
							// Clear item and ensure no gaps are left in inventory for fridges and chests
							sourceItems[ingredient.WhichInventory].RemoveAt(ingredient.WhichItem);
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
					if (CookingManager.GetBurnChance(recipe) > Game1.random.NextDouble())
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
			if (ModEntry.Config.AddCookingSkillAndRecipes)
			{
				if (!ModEntry.Instance.States.Value.FoodCookedToday.ContainsKey(recipe.name))
					ModEntry.Instance.States.Value.FoodCookedToday[recipe.name] = 0;
				ModEntry.Instance.States.Value.FoodCookedToday[recipe.name] += quantity;

				ModEntry.CookingSkillApi.CalculateExperienceGainedFromCookingItem(
					item: item,
					recipe.getNumberOfIngredients(),
					quantityCooked,
					applyExperience: true);
				Game1.player.cookedRecipe(item.ParentSheetIndex);

				// Update game stats
				Game1.stats.ItemsCooked += (uint)quantityCooked;
				Game1.player.checkForQuestComplete(null, -1, -1, item, null, 2);
				Game1.stats.checkForCookingAchievements();
			}

			// Add cooked items to inventory if possible
			foreach (StardewValley.Object cookedItem in itemsCooked)
			{
				Utils.AddOrDropItem(cookedItem);
			}

			// Add burnt items
			if (CookingMenu.LastBurntCount > 0)
			{
				Item burntItem = new StardewValley.Object(
					Interface.Interfaces.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "burntfood"),
					CookingMenu.LastBurntCount);
				Utils.AddOrDropItem(burntItem);
			}

			return CookingMenu.LastBurntCount;
		}

		internal bool AddToIngredients(int whichInventory, int whichItem, int itemId)
		{
			Ingredient ingredient = new Ingredient(whichInventory: whichInventory, whichItem: whichItem, itemId: itemId);
			return this.AddToIngredients(ingredient: ingredient);
		}

		internal bool AddToIngredients(Ingredient ingredient)
		{
			if (this.FirstEmptySlot < 0 || this.FirstEmptySlot >= this.MaxIngredients)
				return false;
			this.CurrentIngredients[this.FirstEmptySlot] = ingredient;
			return true;
		}

		internal bool RemoveFromIngredients(int inventoryId, int itemIndex)
		{
			int index = this.CurrentIngredients.FindIndex(i => i.HasValue && i.Value.WhichInventory == inventoryId && i.Value.WhichItem == itemIndex);
			if (index < 0)
				return false;
			this.CurrentIngredients[index] = null;
			return true;
		}

		internal bool RemoveFromIngredients(int ingredientsIndex)
		{
			if (ingredientsIndex < 0 || ingredientsIndex >= this.CurrentIngredients.Count || this.CurrentIngredients[ingredientsIndex] is null)
				return false;
			this.CurrentIngredients[ingredientsIndex] = null;
			return true;
		}

		internal void AutoFillIngredients(CraftingRecipe recipe, List<IList<Item>> sourceItems)
		{
			// Don't fill slots if the player isn't able to cook the recipe
			if (recipe is null || this.MaxIngredients < recipe.recipeList.Count
				|| 1 > this.GetAmountCraftable(recipe: recipe, sourceItems: sourceItems, limitToCurrentIngredients: false))
				return;

			// Get all matching ingredients for recipe items
			List<Ingredient> ingredients = recipe.recipeList
				.SelectMany(itemAndQuantity => CookingManager.GetMatchingIngredients(
					id: itemAndQuantity.Key, sourceItems: sourceItems, required: itemAndQuantity.Value))
				.ToList();

			// Skip if no matching ingredients are found
			if (ingredients is null || ingredients.Count == 0)
				return;

			// Reduce ingredients to try and complete the recipe in as many slots as we have,
			// sorting by stack counts to maximise the amount craftable
			List<List<int>> matchingItemIndexes = recipe.recipeList.Keys
				.Select(id => ingredients
					.Where(i => CookingManager.IsMatchingIngredient(id: id, item: this.GetItemForIngredient(ingredient: i, sourceItems: sourceItems)))
					.Select(i => ingredients.IndexOf(i))
					.OrderByDescending(i => this.GetItemForIngredient(ingredients[i], sourceItems: sourceItems)?.Stack)
					.ToList())
				.ToList();

			// Add items from each list of matching ingredients in turn
			// This should create a mixed list where each required item has an ingredient represented
			List<Ingredient> ingredientsToUse = new List<Ingredient>();
			int maxItems = matchingItemIndexes.Max(list => list.Count);
			int maxLists = matchingItemIndexes.Count;
			for (int whichItem = 0; whichItem < maxItems; ++whichItem)
				for (int whichList = 0; whichList < maxLists; ++whichList)
					if (whichItem < matchingItemIndexes[whichList].Count)
						ingredientsToUse.Add(ingredients[matchingItemIndexes[whichList][whichItem]]);

			// Fill slots with select ingredients
			foreach (Ingredient ingredient in ingredientsToUse.Take(this.MaxIngredients))
			{
				this.AddToIngredients(ingredient);
			}
		}

		internal void ClearCurrentIngredients()
		{
			for (int i = 0; i < this.CurrentIngredients.Count; ++i)
			{
				this.CurrentIngredients[i] = null;
			}
		}

		internal Item GetItemForIngredient(int index, List<IList<Item>> sourceItems)
		{
			Item item = this.CurrentIngredients.Count > index && this.CurrentIngredients[index].HasValue
				? this.GetItemForIngredient(ingredient: this.CurrentIngredients[index].Value, sourceItems: sourceItems)
				: null;
			return item;
		}

		internal Item GetItemForIngredient(Ingredient ingredient, List<IList<Item>> sourceItems)
		{
			Item item = ingredient.WhichInventory >= 0 && sourceItems.Count > ingredient.WhichInventory 
					&& ingredient.WhichItem >= 0 && sourceItems[ingredient.WhichInventory].Count > ingredient.WhichItem
				? sourceItems[ingredient.WhichInventory][ingredient.WhichItem]
				: null;
			return item;
		}

		internal bool IsInventoryItemInCurrentIngredients(int inventoryIndex, int itemIndex)
		{
			return this.CurrentIngredients
				.Any(i => i.HasValue && i.Value.WhichInventory == inventoryIndex && i.Value.WhichItem == itemIndex);
		}
	}
}
