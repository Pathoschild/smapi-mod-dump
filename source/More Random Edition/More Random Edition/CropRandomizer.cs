using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Randomizes fruit trees
	/// </summary>
	public class CropRandomizer
	{
		public static void Randomize(EditedObjectInformation editedObjectInfo)
		{
			RandomizeCrops(editedObjectInfo);
			RandomizeFruitTrees(editedObjectInfo);
			WriteToSpoilerLog();
		}

		/// <summary>
		/// Randomize fruit tree information
		/// </summary>
		/// <param name="editedObjectInfo">The edited object information</param>
		private static void RandomizeFruitTrees(EditedObjectInformation editedObjectInfo)
		{
			int[] fruitTreesIds = new int[]
			{
				(int)ObjectIndexes.CherrySapling,
				(int)ObjectIndexes.ApricotSapling,
				(int)ObjectIndexes.OrangeSapling,
				(int)ObjectIndexes.PeachSapling,
				(int)ObjectIndexes.PomegranateSapling,
				(int)ObjectIndexes.AppleSapling
			};
			List<Item> allPotentialTreesItems = ItemList.Items.Values.Where(x =>
				fruitTreesIds.Contains(x.Id) || x.DifficultyToObtain < ObtainingDifficulties.Impossible
			).ToList();

			List<Item> treeItems = Globals.RNGGetRandomValuesFromList(allPotentialTreesItems, 6);

			string[] seasons = { "spring", "spring", "summer", "summer", "fall", "fall" };
			seasons[Globals.RNG.Next(0, 6)] = "winter";

			int[] prices = treeItems.Select(x => x.GetPriceForObtainingDifficulty(0.2)).ToArray();
			if (!Globals.Config.RandomizeFruitTrees) { return; }

			// Fruit tree asset replacements
			var fruitTreeReplacements = new Dictionary<int, string>();

			// The Trees are incremented starting with cherry
			for (int i = 0; i < treeItems.Count; i++)
			{
				int price = prices[i];
				string season = seasons[i];
				string seasonDisplay = Globals.GetTranslation($"seasons-{season}");
				Item treeItem = treeItems[i];
				string treeItemName = treeItem.DisplayName;
				string fruitTreeName = treeItem.Id == fruitTreesIds[i] ?
					Globals.GetTranslation("item-recursion-sapling-name") :
					Globals.GetTranslation("sapling-text", new { itemName = treeItemName });
				string fruitTreeEnglishName = treeItem.Id == fruitTreesIds[i] ?
					"Recursion Sapling" :
					$"{treeItem.Name} Sapling";

				int fruitTreeId = fruitTreesIds[i];

				string fruitTreeValue = $"{i}/{season}/{treeItem.Id}/{price}";
				editedObjectInfo.FruitTreeReplacements[fruitTreeId] = fruitTreeValue;

				ItemList.Items[fruitTreeId].OverrideName = fruitTreeEnglishName;
				string fruitTreeObjectValue = $"{fruitTreeName}/{price / 2}/-300/Basic -74/{fruitTreeName}/{Globals.GetTranslation("sapling-description", new { itemName = treeItemName, season = seasonDisplay })}";
				editedObjectInfo.ObjectInformationReplacements[fruitTreeId] = fruitTreeObjectValue;
			}
		}

		/// <summary>
		/// Randomizes the crops - currently only does prices, and only for seasonal crops
		/// </summary>
		/// <param name="editedObjectInfo">The edited object information</param>
		/// crop format: name/price/-300/Seeds -74/name/tooltip
		private static void RandomizeCrops(EditedObjectInformation editedObjectInfo)
		{
			List<int> regrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && x.CropGrowthInfo.RegrowsAfterHarvest)
				.Select(x => x.Id)
				.ToList();
			List<int> regrowableSeedIdsToRandomizeCopy = new List<int>(regrowableSeedIdsToRandomize);

			List<int> nonRegrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && !x.CropGrowthInfo.RegrowsAfterHarvest)
				.Select(x => x.Id)
				.ToList();
			List<int> nonRegrowableSeedIdsToRandomizeCopy = new List<int>(nonRegrowableSeedIdsToRandomize);

			// Fill up a dictionary to remap the seed values
			Dictionary<int, int> seedMappings = new Dictionary<int, int>(); // Original value, new value

			foreach (int originalRegrowableSeedId in regrowableSeedIdsToRandomize)
			{
				seedMappings.Add(originalRegrowableSeedId, Globals.RNGGetAndRemoveRandomValueFromList(regrowableSeedIdsToRandomizeCopy));
			}

			foreach (int originalNonRegrowableSeedId in nonRegrowableSeedIdsToRandomize)
			{
				seedMappings.Add(originalNonRegrowableSeedId, Globals.RNGGetAndRemoveRandomValueFromList(nonRegrowableSeedIdsToRandomizeCopy));
			}

			// Loop through the dictionary and reassign the values, keeping the seasons the same as before
			foreach (KeyValuePair<int, int> seedMapping in seedMappings)
			{
				int originalValue = seedMapping.Key;
				int newValue = seedMapping.Value;

				CropGrowthInformation cropInfoToAdd = CropGrowthInformation.ParseString(CropGrowthInformation.DefaultStringData[newValue]);
				cropInfoToAdd.GrowingSeasons = CropGrowthInformation.ParseString(CropGrowthInformation.DefaultStringData[originalValue]).GrowingSeasons;
				cropInfoToAdd.GrowthStages = GetRandomGrowthStages(cropInfoToAdd.GrowthStages.Count);
				cropInfoToAdd.CanScythe = Globals.RNGGetNextBoolean(10);
				cropInfoToAdd.DaysToRegrow = cropInfoToAdd.RegrowsAfterHarvest ? Range.GetRandomValue(1, 7) : -1;

				if (!Globals.Config.RandomizeCrops) { continue; } // Preserve the original seasons/etc
				CropGrowthInformation.CropIdsToInfo[originalValue] = cropInfoToAdd;
			}

			// Set the object info
			List<CropItem> randomizedCrops = ItemList.GetCrops(true).Cast<CropItem>()
				.Where(x => nonRegrowableSeedIdsToRandomize.Union(regrowableSeedIdsToRandomize).Contains(x.MatchingSeedItem.Id))
				.ToList();
			List<CropItem> vegetables = randomizedCrops.Where(x => !x.IsFlower).ToList();
			List<CropItem> flowers = randomizedCrops.Where(x => x.IsFlower).ToList();

			List<string> vegetableNames = NameAndDescriptionRandomizer.GenerateVegetableNames(vegetables.Count + 1);
			List<string> cropDescriptions = NameAndDescriptionRandomizer.GenerateCropDescriptions(randomizedCrops.Count);
			SetCropAndSeedInformation(
				editedObjectInfo,
				vegetables,
				vegetableNames,
				cropDescriptions); // Note: It removes the descriptions it uses from the list after assigning them- may want to edit later

			SetUpCoffee(editedObjectInfo, vegetableNames[vegetableNames.Count - 1]);
			SetUpRice(editedObjectInfo);

			SetCropAndSeedInformation(
				editedObjectInfo,
				flowers,
				NameAndDescriptionRandomizer.GenerateFlowerNames(flowers.Count),
				cropDescriptions); // Note: It removes the descriptions it uses from the list after assigning them- may want to edit later

			SetUpCookedFood(editedObjectInfo);
		}

		/// <summary>
		/// Gets a list of randomly generated growth stages
		/// </summary>
		/// <param name="numberOfStages"></param>
		/// <returns>A list of integers, totaling up to a max of 12</returns>
		private static List<int> GetRandomGrowthStages(int numberOfStages)
		{
			if (numberOfStages <= 0)
			{
				Globals.ConsoleError("Tried to pass an invalid number of growth stages when randomizing crops.");
				return new List<int>();
			}

			int maxValuePerStage = 12 / numberOfStages;
			List<int> growthStages = new List<int>();

			for (int i = 0; i < numberOfStages; i++)
			{
				growthStages.Add(Range.GetRandomValue(1, maxValuePerStage));
			}

			return growthStages;
		}

		/// <summary>
		/// Sets the ToString information for the given crops
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="crops">The crops to set</param>
		/// <param name="randomNames">The random names to give the crops</param>
		private static void SetCropAndSeedInformation(
			EditedObjectInformation editedObjectInfo,
			List<CropItem> crops,
			List<string> randomNames,
			List<string> randomDescriptions)
		{
			for (int i = 0; i < crops.Count; i++)
			{
				CropItem crop = crops[i];
				string name = randomNames[i];
				string description = Globals.RNGGetAndRemoveRandomValueFromList(randomDescriptions);
				crop.OverrideName = name;
				crop.Description = description;

				SeedItem seed = ItemList.GetSeedFromCrop(crop);
				seed.OverrideDisplayName = seed.CropGrowthInfo.IsTrellisCrop ?
					Globals.GetTranslation("trellis-text", new { itemName = name }) :
					Globals.GetTranslation("seed-text", new { itemName = name });
				seed.OverrideName = seed.CropGrowthInfo.IsTrellisCrop ?
					$"{name} Starter" :
					$"{name} Seeds";

				seed.Price = GetRandomSeedPrice();
				crop.Price = CalculateCropPrice(seed);

				if (!Globals.Config.RandomizeCrops) { continue; }

				editedObjectInfo.ObjectInformationReplacements[crop.Id] = crop.ToString();
				editedObjectInfo.ObjectInformationReplacements[seed.Id] = seed.ToString();
			}
		}

		/// <summary>
		/// Sets up the coffee beans and coffee objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="coffeeName">The name of the coffee item</param>
		private static void SetUpCoffee(EditedObjectInformation editedObjectInfo, string coffeeName)
		{
			if (!Globals.Config.RandomizeCrops) { return; }

			Item coffee = ItemList.Items[(int)ObjectIndexes.Coffee];
			coffee.OverrideName = $"Hot {coffeeName}";
			coffee.CoffeeIngredient = coffeeName;
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.Coffee] = coffee.ToString();

			Item coffeeBean = ItemList.Items[(int)ObjectIndexes.CoffeeBean];
			coffeeBean.OverrideName = $"{coffeeName} Bean";
			coffeeBean.OverrideDisplayName = Globals.GetTranslation("coffee-bean-name", new { itemName = coffeeName });
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.CoffeeBean] = coffeeBean.ToString();
		}

		/// <summary>
		/// Sets up the rice objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		public static void SetUpRice(EditedObjectInformation editedObjectInfo)
		{
			CropItem unmilledRice = (CropItem)ItemList.Items[(int)ObjectIndexes.UnmilledRice];
			string riceName = unmilledRice.OverrideName;
			unmilledRice.OverrideName = $"Unmilled {riceName}";
			unmilledRice.OverrideDisplayName = Globals.GetTranslation("unmilled-rice-name", new { itemName = riceName });
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.UnmilledRice] = unmilledRice.ToString();

			Item rice = ItemList.Items[(int)ObjectIndexes.Rice];
			rice.OverrideName = riceName;
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.Rice] =
				$"{riceName}/100/5/Basic/{riceName}/{Globals.GetTranslation("item-rice-description")}";
		}

		/// <summary>
		/// Changes the names of the cooked food to match those of the objects themselves
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpCookedFood(EditedObjectInformation editedObjectInfo)
		{
			if (!Globals.Config.RandomizeCrops) { return; }

			string cauliflower = ItemList.Items[(int)ObjectIndexes.Cauliflower].Name;
			string parsnip = ItemList.Items[(int)ObjectIndexes.Parsnip].Name;
			string greenbean = ItemList.Items[(int)ObjectIndexes.GreenBean].Name;
			string yam = ItemList.Items[(int)ObjectIndexes.Yam].Name;
			string hotpepper = ItemList.Items[(int)ObjectIndexes.HotPepper].Name;
			string rhubarb = ItemList.Items[(int)ObjectIndexes.Rhubarb].Name;
			string eggplant = ItemList.Items[(int)ObjectIndexes.Eggplant].Name;
			string blueberry = ItemList.Items[(int)ObjectIndexes.Blueberry].Name;
			string pumpkin = ItemList.Items[(int)ObjectIndexes.Pumpkin].Name;
			string cranberry = ItemList.Items[(int)ObjectIndexes.Cranberries].Name;
			string radish = ItemList.Items[(int)ObjectIndexes.Radish].Name;
			string poppyseed = ItemList.Items[(int)ObjectIndexes.Poppy].Name;
			string artichoke = ItemList.Items[(int)ObjectIndexes.Artichoke].Name;
			string rice = ItemList.Items[(int)ObjectIndexes.Rice].Name;

			string carp = ItemList.Items[(int)ObjectIndexes.Carp].Name;
			string salmon = ItemList.Items[(int)ObjectIndexes.Salmon].Name;
			string bass = ItemList.Items[(int)ObjectIndexes.LargemouthBass].Name.Split(' ').Last();
			string trout = ItemList.Items[(int)ObjectIndexes.RainbowTrout].Name;
			string eel = ItemList.Items[(int)ObjectIndexes.Eel].Name;

			SetIngredientAndOverrideName((int)ObjectIndexes.CheeseCauliflower, cauliflower, "Cheese {0}");
			SetIngredientAndOverrideName((int)ObjectIndexes.ParsnipSoup, parsnip, "{0} Soup");
			SetIngredientAndOverrideName((int)ObjectIndexes.BeanHotpot, greenbean, "{0} Hotpot");
			SetIngredientAndOverrideName((int)ObjectIndexes.GlazedYams, yam, "Glazed {0} Platter");
			SetIngredientAndOverrideName((int)ObjectIndexes.PepperPoppers, hotpepper, "{0} Poppers");
			SetIngredientAndOverrideName((int)ObjectIndexes.RhubarbPie, rhubarb, "{0} Pie");
			SetIngredientAndOverrideName((int)ObjectIndexes.EggplantParmesan, eggplant, "{0} Parmesan");
			SetIngredientAndOverrideName((int)ObjectIndexes.BlueberryTart, blueberry, "{0} Tart");
			SetIngredientAndOverrideName((int)ObjectIndexes.PumpkinSoup, pumpkin, "{0} Soup");
			SetIngredientAndOverrideName((int)ObjectIndexes.CranberrySauce, cranberry, "{0} Sauce");
			SetIngredientAndOverrideName((int)ObjectIndexes.PumpkinPie, pumpkin, "{0} Pie");
			SetIngredientAndOverrideName((int)ObjectIndexes.RadishSalad, radish, "{0} Salad");
			SetIngredientAndOverrideName((int)ObjectIndexes.CranberryCandy, cranberry, "{0} Candy");
			SetIngredientAndOverrideName((int)ObjectIndexes.PoppyseedMuffin, poppyseed, "{0} Muffin");
			SetIngredientAndOverrideName((int)ObjectIndexes.ArtichokeDip, artichoke, "{0} Dip");
			SetIngredientAndOverrideName((int)ObjectIndexes.FruitSalad, "", "{0} Candy");
			SetIngredientAndOverrideName((int)ObjectIndexes.RicePudding, rice, "{0} Pudding");

			SetIngredientAndOverrideName((int)ObjectIndexes.CarpSurprise, carp, "{0} Surprise");
			SetIngredientAndOverrideName((int)ObjectIndexes.SalmonDinner, salmon, "{0} Dinner");
			SetIngredientAndOverrideName((int)ObjectIndexes.CrispyBass, bass, "Crispy {0}");
			SetIngredientAndOverrideName((int)ObjectIndexes.TroutSoup, trout, "{0} Soup");
			SetIngredientAndOverrideName((int)ObjectIndexes.FriedEel, eel, "Fried {0}");
			SetIngredientAndOverrideName((int)ObjectIndexes.SpicyEel, eel, "Spicy {0}");

			var cropItemReplacements = new Dictionary<int, string>
			{
				{ (int)ObjectIndexes.CheeseCauliflower, $"Cheese {cauliflower}/300/55/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.CheeseCauliflower, cauliflower)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.ParsnipSoup, $"{parsnip} Soup/120/34/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.ParsnipSoup, parsnip)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.BeanHotpot, $"{greenbean} Hotpot/100/50/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.BeanHotpot, greenbean)}/food/0 0 0 0 0 0 2 0 0 0 0/600" },
				{ (int)ObjectIndexes.GlazedYams, $"Glazed {yam} Platter/200/80/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.GlazedYams, yam)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.PepperPoppers, $"{hotpepper} Poppers/200/52/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.PepperPoppers, hotpepper)}/food/2 0 0 0 0 0 0 0 0 1 0/600" },
				{ (int)ObjectIndexes.RhubarbPie, $"{rhubarb} Pie/400/86/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.RhubarbPie, rhubarb)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.EggplantParmesan, $"{eggplant} Parmesan/200/70/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.EggplantParmesan, eggplant)}/food/0 0 1 0 0 0 0 0 0 0 3/400" },
				{ (int)ObjectIndexes.RicePudding, $"{rice} Pudding/260/46/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.RicePudding, rice)}/food/0 0 0 0 0 0 0 0 0 0 0/0"},
				{ (int)ObjectIndexes.BlueberryTart, $"{blueberry} Tart/150/50/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.BlueberryTart, blueberry)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.PumpkinSoup, $"{pumpkin} Soup/300/80/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.PumpkinSoup, pumpkin)}/food/0 0 0 0 2 0 0 0 0 0 2/660" },
				{ (int)ObjectIndexes.CranberrySauce, $"{cranberry} Sauce/120/50/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.CranberrySauce, cranberry)}/food/0 0 2 0 0 0 0 0 0 0 0/300" },
				{ (int)ObjectIndexes.ArtichokeDip, $"{artichoke} Dip/210/40/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.ArtichokeDip, artichoke)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.PumpkinPie, $"{pumpkin} Pie/385/90/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.PumpkinPie, pumpkin)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.RadishSalad, $"{radish} Salad/300/80/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.RadishSalad, radish)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.CranberryCandy, $"{cranberry} Candy/175/50/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.CranberryCandy, cranberry)}/drink/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.PoppyseedMuffin, $"{poppyseed} Muffin/250/60/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.PoppyseedMuffin, poppyseed)}/food/0 0 0 0 0 0 0 0 0 0 0/0" },
				{ (int)ObjectIndexes.FruitSalad, $"Harvest Salad/450/105/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.FruitSalad, "")}/food/0 0 0 0 0 0 0 0 0 0 0/0" }
			};

			var fishItemReplacements = new Dictionary<int, string>
			{
				{ (int)ObjectIndexes.CarpSurprise, $"{carp} Surprise/150/36/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.CarpSurprise, carp)}/food/0 0 0 0 0 0 0 0 0 0 0/0"},
				{ (int)ObjectIndexes.SalmonDinner, $"{salmon} Dinner/300/50/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.SalmonDinner, salmon)}/food/0 0 0 0 0 0 0 0 0 0 0/0"},
				{ (int)ObjectIndexes.CrispyBass, $"Crispy {bass}/150/36/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.CrispyBass, bass)}/food/0 0 0 0 0 0 0 0 64 0 0/600"},
				{ (int)ObjectIndexes.TroutSoup, $"{trout} Soup/100/40/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.TroutSoup, trout)}/food/0 1 0 0 0 0 0 0 0 0 0/400"},
				{ (int)ObjectIndexes.FriedEel, $"Fried {eel}/120/30/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.FriedEel, eel)}/food/0 0 0 0 1 0 0 0 0 0 0/600"},
				{ (int)ObjectIndexes.SpicyEel, $"Spicy {eel}/175/46/Cooking -7/{GetRecipeNameAndDescription((int)ObjectIndexes.SpicyEel, eel)}/food/0 0 0 0 1 0 0 0 0 1 0/600"}
			};

			if (Globals.Config.RandomizeCrops)
			{
				foreach (KeyValuePair<int, string> pair in cropItemReplacements)
				{
					editedObjectInfo.ObjectInformationReplacements[pair.Key] = pair.Value;
				}
			}

			if (Globals.Config.RandomizeFish)
			{
				foreach (KeyValuePair<int, string> pair in fishItemReplacements)
				{
					editedObjectInfo.ObjectInformationReplacements[pair.Key] = pair.Value;
				}
			}
		}

		/// <summary>
		/// Sets the ingredient and override name for the given cooked item id
		/// </summary>
		/// <param name="id">The id of the cooked item</param>
		/// <param name="ingredient">The name of the ingredient</param>
		/// <param name="overrideNameTemplate">The template for the override name</param>
		/// <returns />
		private static void SetIngredientAndOverrideName(int id, string ingredient, string overrideNameTemplate)
		{
			CookedItem item = (CookedItem)ItemList.Items[id];
			item.OverrideName = string.Format(overrideNameTemplate, ingredient);
			item.IngredientName = ingredient;
		}

		/// <summary>
		/// Gets the recipe name and description
		/// </summary>
		/// <param name="id">The ID of the recipe</param>
		/// <param name="itemName">The item that is in this recipe</param>
		/// <returns>The internationalized string</returns>
		private static string GetRecipeNameAndDescription(int id, string itemName)
		{
			return Globals.GetTranslation($"item-{id}-name-and-description", new { itemName });
		}

		/// <summary>
		/// Get a random seed price - the weighted values are as follows:
		/// 10 - 30: 40%
		/// 30 - 60: 30%
		/// 60 - 90: 15%
		/// 90 - 120: 10%
		/// 120 - 150: 5%
		/// </summary>
		/// <returns>
		/// The generated number - this will be an even number because the seed price that
		/// we need to report is actually the sell value, which is half of the price we will
		/// generate here
		/// </returns>
		private static int GetRandomSeedPrice()
		{
			int generatedValue = Range.GetRandomValue(1, 100);
			int baseValue = 0;
			if (generatedValue < 41)
			{
				baseValue = Range.GetRandomValue(10, 30);
			}
			else if (generatedValue < 71)
			{
				baseValue = Range.GetRandomValue(31, 60);
			}
			else if (generatedValue < 86)
			{
				baseValue = Range.GetRandomValue(61, 90);
			}
			else if (generatedValue < 96)
			{
				baseValue = Range.GetRandomValue(91, 120);
			}
			else
			{
				baseValue = Range.GetRandomValue(121, 150);
			}

			return baseValue / 2; // We need to store the sell price, not the buy price
		}

		/// <summary>
		/// Calculates the seed price based on the seed growth info and price
		/// </summary>
		/// <param name="seed">The seed</param>
		/// <returns>
		/// Returns a value based on a random multiplier, regrowth days, and
		/// potential amount per harvest
		/// </returns>
		private static int CalculateCropPrice(SeedItem seed)
		{
			int seedPrice = seed.Price * 2; // The amount we store here is half of what we want to base this off of
			CropGrowthInformation growthInfo = seed.CropGrowthInfo;

			double multiplier = 1;
			if (seedPrice < 31) { multiplier = Range.GetRandomValue(15, 40) / (double)10; }
			else if (seedPrice < 61) { multiplier = Range.GetRandomValue(15, 35) / (double)10; }
			else if (seedPrice < 91) { multiplier = Range.GetRandomValue(15, 30) / (double)10; }
			else if (seedPrice < 121) { multiplier = Range.GetRandomValue(15, 25) / (double)10; }
			else { multiplier = Range.GetRandomValue(15, 20) / (double)10; }

			double regrowthDaysMultiplier = 1;
			switch (growthInfo.DaysToRegrow)
			{
				case 1: regrowthDaysMultiplier = 0.3; break;
				case 2: regrowthDaysMultiplier = 0.4; break;
				case 3: regrowthDaysMultiplier = 0.5; break;
				case 4: regrowthDaysMultiplier = 0.6; break;
				case 5: regrowthDaysMultiplier = 0.7; break;
				case 6: regrowthDaysMultiplier = 0.8; break;
				case 7: regrowthDaysMultiplier = 0.9; break;
				default: regrowthDaysMultiplier = 1; break;
			}

			double amountPerHarvestMultiplier = 1;
			switch (growthInfo.ExtraCropInfo.MinExtra)
			{
				case 0: break;
				case 1: break;
				case 2: amountPerHarvestMultiplier = 0.6; break;
				case 3: amountPerHarvestMultiplier = 0.45; break;
				case 4: amountPerHarvestMultiplier = 0.3; break;
				default:
					Globals.ConsoleError($"Unexpected seed with more than 4 minimum extra crops: {seed.Id}");
					break;
			}
			if (growthInfo.ExtraCropInfo.CanGetExtraCrops && amountPerHarvestMultiplier == 1)
			{
				amountPerHarvestMultiplier = 0.9;
			}

			return (int)(seedPrice * multiplier * regrowthDaysMultiplier * amountPerHarvestMultiplier);
		}

		/// <summary>
		/// Writes relevant crop changes to the spoiler log
		/// </summary>
		private static void WriteToSpoilerLog()
		{

			if (Globals.Config.RandomizeCrops)
			{
				Globals.SpoilerWrite("==== CROPS AND SEEDS ====");
				foreach (SeedItem seedItem in ItemList.GetSeeds())
				{
					if (seedItem.Id == (int)ObjectIndexes.CoffeeBean || seedItem.Id == (int)ObjectIndexes.AncientSeeds) { continue; }
					CropItem cropItem = (CropItem)ItemList.Items[seedItem.CropGrowthInfo.CropId];
					Globals.SpoilerWrite($"{cropItem.Id}: {cropItem.Name} - Seed Buy Price: {seedItem.Price * 2}G - Crop Sell Price: {cropItem.Price}G");
					Globals.SpoilerWrite($"{seedItem.Id}: {seedItem.Description}");
					Globals.SpoilerWrite("---");
				}
				Globals.SpoilerWrite("");
			}

			if (Globals.Config.RandomizeFruitTrees)
			{
				Globals.SpoilerWrite("==== FRUIT TREES ====");
				Globals.SpoilerWrite($"{ItemList.GetItemName((int)ObjectIndexes.CherrySapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName((int)ObjectIndexes.AppleSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName((int)ObjectIndexes.OrangeSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName((int)ObjectIndexes.PeachSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName((int)ObjectIndexes.PomegranateSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName((int)ObjectIndexes.ApricotSapling)}");
				Globals.SpoilerWrite("");
			}
		}
	}
}
