/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class CraftableItem : Item
	{
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
		public CraftableCategories Category { get; set; }
		public Dictionary<ObjectIndexes, int> LastRecipeGenerated { get; set; } = new(); // item id to amount needed

		private readonly static Dictionary<string, string> CraftingRecipeData = 
			Globals.ModRef.Helper.GameContent.Load<Dictionary<string, string>>("Data/CraftingRecipes");

		/// <summary>
		/// The original data taken from Data/CraftingRecipes.xnb
		/// Will be modified as it is randomzied
		/// </summary>
		public string[] CraftingData { get; private set; }

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
			int id,
			CraftableCategories category, 
			int overrideBaseLevelLearnedAt = -1,
			bool isBigCraftable = false,
			int bigCraftablePrice = 1000,
			string dataKey = null) : base(id)
		{
            IsBigCraftable = isBigCraftable; // Put this first so the value is set for EnglishName
			CraftingRecipeKey = dataKey ?? EnglishName;
            CraftingData = CraftingRecipeData[CraftingRecipeKey].Split("/");
            IsCraftable = true;
			BigCraftablePrice = bigCraftablePrice;
			Category = category;
			DifficultyToObtain = ObtainingDifficulties.NonCraftingItem; // By default, craftable items won't be materials for other craftable items

            if (isBigCraftable && !Enum.IsDefined(typeof(BigCraftableIndexes), id))
			{
				Globals.ConsoleWarn($"Craftable item marked as big craftable without a matching BigCraftableIndex: {id}");
			}

			// The skill and level learned at are in a space-delimited string
			// not all crafting recipes have this, though, so check the exceptions first
			string unlockConditionsString = CraftingData[(int)CraftingRecipeIndexes.UnlockConditions];
			if (unlockConditionsString != "null" && unlockConditionsString != "l 0")
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
			Range levelRange = new(BaseLevelLearnedAt - 3, BaseLevelLearnedAt + 3);
			int generatedLevel = levelRange.GetRandomValue();
			if (generatedLevel > 8) { return 9; }
			if (generatedLevel < 1) { return 1; }
			if (generatedLevel == 5)
			{
				generatedLevel = Globals.RNGGetNextBoolean() ? 4 : 6;
			}

			if (!Globals.Config.CraftingRecipes.Randomize || 
				!Globals.Config.CraftingRecipes.RandomizeLevels) 
			{ 
				return OriginalLevelLearnedAt; 
			}

			return generatedLevel;
		}

		/// <summary>
		/// Gets the string to be used for the crafting recipe
		/// </summary>
		/// <returns>The data in the xnb format</returns>
		public string GetCraftingString()
		{
			string itemsRequiredString = GetItemsRequired();
			string unlockConditions = IsLearnedOnLevelup ? $"{SkillString} {GetLevelLearnedAt()}" : "";
			CraftingData[(int)CraftingRecipeIndexes.Ingredients] = itemsRequiredString;
			CraftingData[(int)CraftingRecipeIndexes.UnlockConditions] = unlockConditions;

            string requiredItemsSpoilerString = "";
			string[] requiredItemsTokens = itemsRequiredString.Split(' ');
			for (int i = 0; i < requiredItemsTokens.Length; i += 2)
			{
				string itemName = ItemList.GetItemName((ObjectIndexes)int.Parse(requiredItemsTokens[i]));
				string amount = requiredItemsTokens[i + 1];
				requiredItemsSpoilerString += $" - {itemName}: {amount}";
			}

			if (Globals.Config.CraftingRecipes.Randomize)
			{
				Globals.SpoilerWrite($"{Name} - {unlockConditions}");
				Globals.SpoilerWrite(requiredItemsSpoilerString);
				Globals.SpoilerWrite("---");
			}

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
			string craftingString;
			switch (Category)
			{
				case CraftableCategories.EasyAndNeedMany:
					craftingString = GetStringForEasyAndNeedMany();
					break;
				case CraftableCategories.Easy:
					craftingString = GetStringForEasy();
					break;
				case CraftableCategories.ModerateAndNeedMany:
					craftingString = GetStringForModerateAndNeedMany();
					break;
				case CraftableCategories.Moderate:
					craftingString = GetStringForModerate();
					break;
				case CraftableCategories.DifficultAndNeedMany:
					craftingString = GetStringForDifficultAndNeedMany();
					break;
				case CraftableCategories.Difficult:
					craftingString = GetStringForDifficult();
					break;
				case CraftableCategories.Endgame:
					craftingString = GetStringForEndgame();
					break;
				case CraftableCategories.Foragables:
					craftingString = GetStringForForagables();
					break;
				default:
					Globals.ConsoleError($"Invalid category when generating recipe for {Name}!");
					craftingString = "18 9"; // just a random value for now
					break;
			}

			PopulateLastRecipeGenerated(craftingString);
			return craftingString;
		}

		/// <summary>
		/// Fills the LastRecipeGenerated dictionary with the new recipe
		/// This is a dictionary with item ids mapped to amounts
		/// </summary>
		/// <param name="craftingString">The crafting string to parse</param>
		private void PopulateLastRecipeGenerated(string craftingString)
		{
			LastRecipeGenerated.Clear();
			string[] tokens = craftingString.Split(' ');
			for (int i = 0; i + 1 < tokens.Length; i += 2)
			{
				ObjectIndexes id = (ObjectIndexes)int.Parse(tokens[i]);
				int amount = int.Parse(tokens[i + 1]);

				if (!LastRecipeGenerated.ContainsKey(id))
				{
					LastRecipeGenerated.Add(id, amount);
				}
			}
		}

		/// <summary>
		/// Gets the crafting string for an item that is easy to get and that you need to craft many of
		/// Consists of 1 or 2 items that have no reqiurements to obtain
		/// </summary>
		/// <returns></returns>
		private string GetStringForEasyAndNeedMany()
		{
			Item item = ItemList.GetRandomCraftableItem(
				new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
				this,
				null,
				true
			);

			int numberRequired = Globals.RNGGetNextBoolean() ? 1 : 2;
			return $"{item.Id} {numberRequired}";
		}

		/// <summary>
		/// Uses either two really easy to get items (one being a resource), or one slightly harder to get item		
		/// /// </summary>
		/// <returns>The item string</returns>
		private string GetStringForEasy()
		{
			bool useHarderItem = Globals.RNGGetNextBoolean();
			if (useHarderItem)
			{
				return ItemList.GetRandomCraftableItem(
					new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
					this
				).GetStringForCrafting();
			}

			Item resourceItem = ItemList.GetRandomCraftableItem(
				new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
				this,
				null,
				true
			);
			Item otherItem = ItemList.GetRandomCraftableItem(
				new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
				this,
				new List<int> { Id, resourceItem.Id }
			);

			return $"{resourceItem.GetStringForCrafting()} {otherItem.GetStringForCrafting()}";
		}

		/// <summary>
		/// One of the following, limited to one item needed
		/// - Three sets of SmallTime
		/// - One MediumTime, one SmallTime/No, one No
		/// - One MediumTime, one SmallTime
		/// </summary>
		/// <returns>The item string</returns>
		private string GetStringForModerateAndNeedMany()
		{
			string output = string.Empty;
			foreach (Item item in GetListOfItemsForModerate())
			{
				output += $"{item.Id} 1 ";
			}
			return output.Trim();
		}

		/// <summary>
		/// One of the following
		/// - Three sets of SmallTime
		/// - One MediumTime, one SmallTime/No, one No
		/// - One MediumTime, one SmallTime
		/// </summary>
		/// <returns>The item string</returns>
		private string GetStringForModerate()
		{
			string output = string.Empty;
			foreach (Item item in GetListOfItemsForModerate())
			{
				output += $"{item.GetStringForCrafting()} ";
			}
			return output.Trim();
		}

		/// <summary>
		/// Gets the list of items for any of the moderate cases
		/// </summary>
		/// <returns />
		private List<Item> GetListOfItemsForModerate()
		{
			Item item1, item2, item3;
			switch (Globals.RNG.Next(0, 3))
			{
				case 0:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<int> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<int> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				case 1:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements },
						this,
						new List<int> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.NoRequirements },
						this,
						new List<int> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				default:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<int> { item1.Id }
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
		private string GetStringForDifficultAndNeedMany()
		{
			string output = string.Empty;
			foreach (Item item in GetListOfItemsForDifficult())
			{
				output += $"{item.Id} 1 ";
			}
			return output.Trim();
		}

		/// <summary>
		/// One of the following, limited to one item needed
		/// - Three sets of MediumTime
		/// - One LongTime, one MediumTime/SmallTime, MediumTime/SmallTime/No
		/// - Two sets of LongTime
		/// </summary>
		/// <returns>The item string</returns>
		private string GetStringForDifficult()
		{
			string output = string.Empty;
			foreach (Item item in GetListOfItemsForDifficult())
			{
				output += $"{item.GetStringForCrafting()} ";
			}
			return output.Trim();
		}

		/// <summary>
		/// Gets the list of items for any of the moderate cases
		/// </summary>
		/// <returns />
		private List<Item> GetListOfItemsForDifficult()
		{
			List<Item> possibleItems = ItemList.Items.Values.ToList();
			Item item1, item2, item3;
			switch (Globals.RNG.Next(0, 3))
			{
				case 0:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this,
						new List<int> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements },
						this,
						new List<int> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				case 1:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements, ObtainingDifficulties.SmallTimeRequirements },
						this,
						new List<int> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.MediumTimeRequirements, ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements },
						this,
						new List<int> { item1.Id, item2.Id }
					);
					return new List<Item> { item1, item2, item3 };
				default:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<int> { item1.Id }
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
		private string GetStringForEndgame()
		{
			List<Item> possibleItems = ItemList.Items.Values.ToList();
			Item item1, item2, item3;
			switch (Globals.RNG.Next(0, 3))
			{
				case 0:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<int> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<int> { item1.Id, item2.Id }
					);
					break;
				case 1:
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this,
						new List<int> { item1.Id }
					);
					item3 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements },
						this,
						new List<int> { item1.Id, item2.Id }
					);
					break;
				default:
					var mediumOrLess = new List<ObtainingDifficulties> {
						ObtainingDifficulties.MediumTimeRequirements, ObtainingDifficulties.SmallTimeRequirements, ObtainingDifficulties.NoRequirements
					};
					item1 = ItemList.GetRandomCraftableItem(
						new List<ObtainingDifficulties> { ObtainingDifficulties.LargeTimeRequirements },
						this
					);
					item2 = ItemList.GetRandomCraftableItem(mediumOrLess, this, new List<int> { item1.Id });
					item3 = ItemList.GetRandomCraftableItem(mediumOrLess, this, new List<int> { item1.Id, item2.Id });
					break;
			}

			return $"{item1.GetStringForCrafting()} {item2.GetStringForCrafting()} {item3.GetStringForCrafting()}";
		}

		/// <summary>
		/// Gets the string for the Foragable type
		/// This will be 4 of any of the foragables of the appropriate season
		/// </summary>
		/// <returns />
		private string GetStringForForagables()
		{
			Seasons season = Seasons.Spring;
			switch (Id)
			{
				case (int)ObjectIndexes.SpringSeeds:
					season = Seasons.Spring;
					break;
				case (int)ObjectIndexes.SummerSeeds:
					season = Seasons.Summer;
					break;
				case (int)ObjectIndexes.FallSeeds:
					season = Seasons.Fall;
					break;
				case (int)ObjectIndexes.WinterSeeds:
					season = Seasons.Winter;
					break;
				default:
					Globals.ConsoleError("Generated string for Foragable type for a non-wild seed! Using 1 wood instead...");
					return $"{(int)ObjectIndexes.Wood} 1";
			}

			Dictionary<int, int> foragablesToUse = new Dictionary<int, int>();
			for (int i = 0; i < 4; i++)
			{
				int foragableId = Globals.RNGGetRandomValueFromList(ItemList.GetForagables(season)).Id;
				if (foragablesToUse.ContainsKey(foragableId))
				{
					foragablesToUse[foragableId]++;
				}
				else
				{
					foragablesToUse.Add(foragableId, 1);
				}
			}

			string craftingString = "";
			foreach (int id in foragablesToUse.Keys)
			{
				craftingString += $"{id} {foragablesToUse[id]} ";
			}

			return craftingString.Trim();
		}
	}
}
