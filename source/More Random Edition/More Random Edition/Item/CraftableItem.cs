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
using System;
using System.Collections.Generic;

namespace Randomizer
{
    public class CraftableItem : Item
	{
		public RNG Rng { get; set; }
		public string SkillString { get; set; } = "";
		/// <summary>
		/// We use Name by default, but some recipes use a different name than that
		/// </summary>
		public string CraftingRecipeKey { get; set; }
		public int OriginalLevelLearnedAt { get; set; }
		public int BaseLevelLearnedAt { get; set; }
		public bool IsLearnedOnLevelup
		{
			get { return SkillString.Length > 0; }
		}
		public CraftableCategories CraftableCategory { get; set; }
		public Dictionary<ObjectIndexes, int> LastRecipeGenerated { get; set; } = new(); // item id to amount needed

		private readonly static Dictionary<string, string> CraftingRecipeData =
			DataLoader.CraftingRecipes(Game1.content);

		/// <summary>
		/// The original data taken from Data/CraftingRecipes.xnb
		/// Will be modified as it is randomzied
		/// </summary>
		public string[] CraftingData { get; private set; }


		public CraftableItem(
			ObjectIndexes id,
            CraftableCategories category,
            int overrideBaseLevelLearnedAt = -1,
            int bigCraftablePrice = 1000,
            string dataKey = null) 
			: this(id.GetId(), category, overrideBaseLevelLearnedAt, isBigCraftable: false, bigCraftablePrice, dataKey)
		{ }

        public CraftableItem(
			BigCraftableIndexes id,
			CraftableCategories category,
			int overrideBaseLevelLearnedAt = -1,
			int bigCraftablePrice = 1000,
			string dataKey = null)
			: this(id.GetId(), category, overrideBaseLevelLearnedAt, isBigCraftable: true, bigCraftablePrice, dataKey)
				{ }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The id of the item</param>
		/// <param name="category">The category of the item - defines how hard it is to craft</param>
        /// <param name="overrideBaseLevelLearnedAt">Will override the base level learned at with this value - set to -1 to not use it</param>
		/// <param name="isBigCraftable">Whether this is a BigCraftable object</param>
		/// <param name="bigCraftablePrice">The price to use for BigCrafables - 1000 is default</param>
		/// <param name="dataKey">
		/// Normally linked to the English name in Object Information - but if there's an exception,
		/// this paramter should be passed in
		/// </param>
        public CraftableItem(
			string id,
			CraftableCategories category, 
			int overrideBaseLevelLearnedAt = -1,
			bool isBigCraftable = false,
			int bigCraftablePrice = 1000,
			string dataKey = null) : base(id)
		{
			// Create a unique RNG for each item so that adding new craftables won't impact the RNG
			// also so that the different items won't always generate linked values
			Rng = RNG.GetFarmRNG($"{nameof(CraftableItem)}.{id}.{isBigCraftable}");

            IsBigCraftable = isBigCraftable; // Put this first so the value is set for EnglishName
			CraftingRecipeKey = dataKey ?? EnglishName;
            CraftingData = CraftingRecipeData[CraftingRecipeKey].Split("/");
            IsCraftable = true;
			BigCraftablePrice = bigCraftablePrice;
			CraftableCategory = category;
			DifficultyToObtain = ObtainingDifficulties.NonCraftingItem; // By default, craftable items won't be materials for other craftable items

			try
			{
				if (isBigCraftable)
				{
                    _ = BigCraftableIndexesExtentions.GetBigCraftableIndex(id);
                }
            }
            catch (Exception)
			{
				Globals.ConsoleWarn($"Craftable item marked as big craftable without a matching BigCraftableIndex: {id}");
			}

			// The skill and level learned at are in a space-delimited string
			// not all crafting recipes have this, though, so check the exceptions first
			string unlockConditionsString = CraftingData[(int)CraftingRecipeIndexes.UnlockConditions];
			if (unlockConditionsString != "null" && 
				unlockConditionsString != "default" &&
                unlockConditionsString != "l 0")
			{
                string[] unlockConditions = unlockConditionsString.Split(" ");
                SkillString = unlockConditions[^2].Trim();
                OriginalLevelLearnedAt = int.Parse(unlockConditions[^1].Trim());
            }

			BaseLevelLearnedAt = overrideBaseLevelLearnedAt == -1
				? OriginalLevelLearnedAt
				: overrideBaseLevelLearnedAt;
		}

		/// <summary>
		/// Gets the level you learn this skill at
		/// </summary>
		/// <returns>
		/// Any value in the given range. Excludes 0, 5, and 10.
		/// Returns 9 if it's 10; returns 1 if it's 0; returns 4 or 6 if it's 5
		/// </returns>
		public int GetLevelLearnedAt()
		{
            if (!Globals.Config.CraftingRecipes.RandomizeLevels)
            {
                return OriginalLevelLearnedAt;
            }

            Range levelRange = new(BaseLevelLearnedAt - 3, BaseLevelLearnedAt + 3);
			int generatedLevel = levelRange.GetRandomValue(Rng);
			if (generatedLevel > 8) { return 9; }
			if (generatedLevel < 1) { return 1; }
			if (generatedLevel == 5)
			{
				generatedLevel = Rng.NextBoolean() ? 4 : 6;
			}

			return generatedLevel;
		}

		/// <summary>
		/// Gets the string to be used for the crafting recipe
		/// Also writes to the spoiler log
		/// </summary>
		/// <param name="overrideRecipeName">
		/// The recipe name to use if not an item in ItemList
		/// Currently used by the two transmute recipes
		/// </param>
		/// <returns>The data in the xnb format</returns>
		public string GetCraftingString(string overrideRecipeName = null)
		{
			string itemsRequiredString = GetItemsRequired();
			string unlockConditions = IsLearnedOnLevelup ? $"{SkillString} {GetLevelLearnedAt()}" : "";
			CraftingData[(int)CraftingRecipeIndexes.Ingredients] = itemsRequiredString;
			CraftingData[(int)CraftingRecipeIndexes.UnlockConditions] = unlockConditions;

            string requiredItemsSpoilerString = "";
			string[] requiredItemsTokens = itemsRequiredString.Split(' ');
			for (int i = 0; i < requiredItemsTokens.Length; i += 2)
			{
				string itemId = requiredItemsTokens[i];
				string itemName;

				// Grab the item or category name for the spoiler log
				// A negative number string is a category
				if (itemId.StartsWith("-"))
				{
                    int categoryId = int.Parse(itemId);
					itemName = $"[{((ItemCategories)categoryId).GetTranslation()}]";
                }
				else
				{
					itemName = ItemList.GetItemName(
						ObjectIndexesExtentions.GetObjectIndex(itemId));
                }

                string amount = requiredItemsTokens[i + 1];
				requiredItemsSpoilerString += $" - {itemName}: {amount}";
			}

			string displayName = overrideRecipeName ?? Name;
            Globals.SpoilerWrite($"{displayName} - {unlockConditions}");
			Globals.SpoilerWrite(requiredItemsSpoilerString);
			Globals.SpoilerWrite("---");

			return string.Join("/", CraftingData);
		}

		/// <summary>
		/// Generates a string consisting of the items required to craft this item
		/// This will NOT return the same value each time it's called!
		/// </summary>
		/// <returns>
		/// A string consisting of the following format:
		/// itemId numberOfItemsRequired (repeat this x times)
		/// </returns>
		private string GetItemsRequired()
		{
			Dictionary<ObjectIndexes, int> craftingRecipe;
			switch (CraftableCategory)
			{
				case CraftableCategories.EasyAndNeedMany:
                    craftingRecipe = GetRecipeForEasyAndNeedMany();
					break;
				case CraftableCategories.Easy:
                    craftingRecipe = GetRecipeForEasy();
					break;
				case CraftableCategories.ModerateAndNeedMany:
                    craftingRecipe = GetRecipeForModerateAndNeedMany();
					break;
				case CraftableCategories.Moderate:
                    craftingRecipe = GetRecipeForModerate();
					break;
				case CraftableCategories.DifficultAndNeedMany:
                    craftingRecipe = GetRecipeForDifficultAndNeedMany();
					break;
				case CraftableCategories.Difficult:
                    craftingRecipe = GetRecipeForDifficult();
					break;
				case CraftableCategories.Endgame:
                    craftingRecipe = GetRecipeForEndgame();
					break;
				case CraftableCategories.Foragables:
                    craftingRecipe = GetRecipeForForagables();
					break;
				default:
					Globals.ConsoleError($"Invalid category when generating recipe for {Name}!");
                    craftingRecipe = new Dictionary<ObjectIndexes, int>() { [ObjectIndexes.Acorn] = 1 }; 
					break;
			}

            // Do this before we change any egg/milk to their categories, as:
            // - Crab Pot uses this for the Trapper profession
            // - Tappers use this to find what items to have available at Robin's
            LastRecipeGenerated = craftingRecipe;
			return RecipeToString(craftingRecipe);
		}

		/// <summary>
		/// If necessary, converts the crafting recipe to use the appropriate category instead
		/// Combines together items so that there are no dups as well (as that causes the menu to not open)
		/// </summary>
		/// <returns>The new crafting string</returns>
		private static string RecipeToString(Dictionary<ObjectIndexes, int> recipe)
		{
			Dictionary<string, int> itemsToAmounts = ConvertRecipeToUseCategories(recipe);

			string newCraftingString = string.Empty;
			foreach (KeyValuePair<string, int> kv in itemsToAmounts)
			{
				newCraftingString += $"{kv.Key} {kv.Value} ";
			}
			return newCraftingString.Trim();
		}

		/// <summary>
		/// Takes a recipe and converts it to a dictionary mapping the item or category to the amount
		/// The Trapper menu hack uses this to get its list of items to check
		/// </summary>
		/// <param name="recipe">The recipe to convert</param>
		/// <returns>The converted dictionary</returns>
		public static Dictionary<string, int> ConvertRecipeToUseCategories(
			Dictionary<ObjectIndexes, int> recipe)
		{
            Dictionary<string, int> itemsToAmounts = new();
            foreach (KeyValuePair<ObjectIndexes, int> kv in recipe)
            {
                string itemOrCraftingCategoryId = ConvertItemIdToCraftingItemId(kv.Key.GetId());
                int amount = kv.Value;

                if (itemsToAmounts.ContainsKey(itemOrCraftingCategoryId))
                {
                    itemsToAmounts[itemOrCraftingCategoryId] += amount;
                }
                else
                {
                    itemsToAmounts[itemOrCraftingCategoryId] = amount;
                }
            }
			return itemsToAmounts;
        }

		/// <summary>
		/// If necessary, converts the crafting item id to use the appropriate category instead
		/// so that all items of that particular category work for the recipe
		/// </summary>
		/// <returns>The new id for the crafting item id</returns>
		private static string ConvertItemIdToCraftingItemId(string id)
		{
			// Skipping void eggs explicitely since they are MEANT to be
			// harder to get than normal eggs
			// If there are too many of these cases, we'll consider putting
			// a property on the item itself to check if we should skip using its category
			if (id == ObjectIndexes.VoidEgg.GetId())
			{
				return id;
			}

            Item item = ItemList.Items[id];
            if (item.IsEgg || item.IsMilk)
            {
				return ((int)item.Category).ToString();
            }

			return id;
        }

		/// <summary>
		/// Gets the crafting recipe for an item that is easy to get and that you need to craft many of
		/// Consists of 1 or 2 items that have no reqiurements to obtain
		/// </summary>
		/// <returns></returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForEasyAndNeedMany()
		{
			Item item = ItemList.GetRandomCraftableItem(
				Rng,
				new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
				this,
				null,
				true
			);

			int numberRequired = Rng.NextBoolean() ? 1 : 2;
			return new Dictionary<ObjectIndexes, int>() 
			{ 
				[item.ObjectIndex] = numberRequired 
			};
		}

		/// <summary>
		/// Uses either two really easy to get items (one being a resource), or one slightly harder to get item		
		/// </summary>
		/// <returns>The item string</returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForEasy()
		{
			Dictionary<ObjectIndexes, int> recipe = new();

            bool useHarderItem = Rng.NextBoolean();
			if (useHarderItem)
			{
				Item harderItem = ItemList.GetRandomCraftableItem(
					Rng,
					new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
					this
				);

				AddItemToRecipe(recipe, harderItem, Rng);
				return recipe;
			}

			Item resourceItem = ItemList.GetRandomCraftableItem(
                Rng,
                new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
				this,
				null,
				true
			);
			Item otherItem = ItemList.GetRandomCraftableItem(
                Rng,
                new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
				this,
				new List<string> { Id, resourceItem.Id }
			);

			AddItemToRecipe(recipe, resourceItem, Rng);
			AddItemToRecipe(recipe, otherItem, Rng);
			return recipe;
        }

		/// <summary>
		/// One of the following, limited to one item needed
		/// - Three sets of SmallTime
		/// - One MediumTime, one SmallTime/No, one No
		/// - One MediumTime, one SmallTime
		/// </summary>
		/// <returns>The item string</returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForModerateAndNeedMany()
		{
			Dictionary<ObjectIndexes, int> recipe = new();
			GetListOfItemsForModerate().ForEach(item =>
				AddItemToRecipe(recipe, 
					item.ObjectIndex, 
					amount: 1));
			return recipe;
		}

		/// <summary>
		/// One of the following
		/// - Three sets of SmallTime
		/// - One MediumTime, one SmallTime/No, one No
		/// - One MediumTime, one SmallTime
		/// </summary>
		/// <returns>The item string</returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForModerate()
		{
            Dictionary<ObjectIndexes, int> recipe = new();
			GetListOfItemsForModerate().ForEach(item =>
                AddItemToRecipe(recipe, item, Rng));
			return recipe;
		}

		/// <summary>
		/// Gets the list of items for any of the moderate cases
		/// </summary>
		/// <returns />
		private List<Item> GetListOfItemsForModerate()
		{
			Item item1, item2, item3;
			switch (Rng.NextIntWithinRange(0, 2))
			{
				case 0:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<string> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				case 1:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements },
						this,
						new List<string> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
						this,
						new List<string> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				default:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					return new List<Item> { item1, item2 };
			}
		}

		/// <summary>
		/// One of the following
		/// - Three sets of MediumTime
		/// - One LongTime, one MediumTime/SmallTime, MediumTime/SmallTime/No
		/// - Two sets of LongTime
		/// </summary>
		/// <returns>The item string</returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForDifficultAndNeedMany()
		{
            Dictionary<ObjectIndexes, int> recipe = new();
            GetListOfItemsForDifficult().ForEach(item =>
                AddItemToRecipe(recipe, 
					item.ObjectIndex, 
					amount: 1));
			return recipe;
		}

		/// <summary>
		/// One of the following, limited to one item needed
		/// - Three sets of MediumTime
		/// - One LongTime, one MediumTime/SmallTime, MediumTime/SmallTime/No
		/// - Two sets of LongTime
		/// </summary>
		/// <returns>The item string</returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForDifficult()
		{
            Dictionary<ObjectIndexes, int> recipe = new();
            GetListOfItemsForDifficult().ForEach(item =>
                AddItemToRecipe(recipe, item, Rng));
            return recipe;
		}

		/// <summary>
		/// Gets the list of items for any of the moderate cases
		/// </summary>
		/// <returns />
		private List<Item> GetListOfItemsForDifficult()
		{
			Item item1, item2, item3;
			switch (Rng.NextIntWithinRange(0, 2))
			{
				case 0:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this,
						new List<string> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				case 1:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements, ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements, ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements },
						this,
						new List<string> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				default:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					return new List<Item> { item1, item2 };
			}
		}

		/// <summary>
		/// - Three sets of LongTime
		/// - Two sets of LongTime, one SmallTime or less
		/// - One set of Longtime, two MediumTime or less
		/// </summary>
		/// <returns>The item string</returns>
		private Dictionary<ObjectIndexes, int> GetRecipeForEndgame()
		{
            Dictionary<ObjectIndexes, int> recipe = new();
			Item item1, item2, item3;
			switch (Rng.NextIntWithinRange(0, 2))
			{
				case 0:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<string> { item1.Id, item2.Id }
					);
					break;
				case 1:
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<string> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements },
						this,
						new List<string> { item1.Id, item2.Id }
					);
					break;
				default:
					var mediumOrLess = new List<ObtainingDifficulties> {
						ObtainingDifficulties.MediumTimeRequirements, ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements
					};
					item1 = ItemList.GetRandomCraftableItem(
                        Rng,
                        new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(Rng, mediumOrLess, this, new List<string> { item1.Id });
					item3 = ItemList.GetRandomCraftableItem(Rng, mediumOrLess, this, new List<string> { item1.Id, item2.Id });
					break;
			}

			AddItemToRecipe(recipe, item1, Rng);
            AddItemToRecipe(recipe, item2, Rng);
            AddItemToRecipe(recipe, item3, Rng);
            return recipe;
		}

		/// <summary>
		/// Gets the recipe for the Foragable type
		/// This will be 4 of any of the foragables of the appropriate season
		/// </summary>
		/// <returns />
		private Dictionary<ObjectIndexes, int> GetRecipeForForagables()
		{
            Dictionary<ObjectIndexes, int> recipe = new();
            Seasons season;

            switch (ObjectIndex)
			{
				case ObjectIndexes.SpringSeeds:
					season = Seasons.Spring;
					break;
				case ObjectIndexes.SummerSeeds:
					season = Seasons.Summer;
					break;
				case ObjectIndexes.FallSeeds:
					season = Seasons.Fall;
					break;
				case ObjectIndexes.WinterSeeds:
					season = Seasons.Winter;
					break;
				default:
					Globals.ConsoleError("Generated string for Foragable type for a non-wild seed! Using 1 wood instead...");
					AddItemToRecipe(recipe, ObjectIndexes.Wood, amount: 1);
					return recipe;
			}

			for (int i = 0; i < 4; i++)
			{
                ObjectIndexes foragableIndex = 
					Rng.GetRandomValueFromList(ItemList.GetForagables(season)).ObjectIndex;
                AddItemToRecipe(recipe, foragableIndex, amount: 1);
            }

			return recipe;
		}

		/// <summary>
		/// Adds an item to the recipe
		/// If it's a dup, will increment the amount accordingly
		/// </summary>
		/// <param name="recipe">The recipe to modif</param>
		/// <param name="itemIndex">The item to add</param>
		/// <param name="amount">The amount to add</param>
		private static void AddItemToRecipe(
			Dictionary<ObjectIndexes, int> recipe, 
			ObjectIndexes itemIndex,
			int amount)
		{
			if (recipe.ContainsKey(itemIndex))
			{
				recipe[itemIndex] += amount;
			}
			else
			{
                recipe[itemIndex] = amount;
            }
        }

        /// <summary>
        /// Adds an item to the recipe
        /// If it's a dup, will increment the amount accordingly
        /// </summary>
        /// <param name="recipe">The recipe to modif</param>
        /// <param name="item">The item to add</param>
        /// <param name="rng">The RNG to use to get the amount</param>
        private static void AddItemToRecipe(
			Dictionary<ObjectIndexes, int> recipe, 
			Item item, 
			RNG rng)
        {
			AddItemToRecipe(recipe, 
				item.ObjectIndex, 
				item.GetAmountRequiredForCrafting(rng));
        }
    }
}
