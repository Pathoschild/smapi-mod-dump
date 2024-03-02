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

			// The trees are incremented starting with cherry
			// Note that "treeItems" refers to the item the fruit tree will grow
			for (int i = 0; i < treeItems.Count; i++)
			{
				int price = prices[i];
                int fruitTreeId = fruitTreesIds[i];
                string season = seasons[i];
				string seasonDisplay = Globals.GetTranslation($"seasons-{season}");
				Item treeItem = treeItems[i];

				string newDispName = treeItem.Id == fruitTreesIds[i] ?
					Globals.GetTranslation("item-recursion-sapling-name") :
					Globals.GetTranslation("sapling-text", new { itemName = treeItem.DisplayName });

				// Make the fruit tree grow the correct item
                string fruitTreeValue = $"{i}/{season}/{treeItem.Id}/{price}";
				editedObjectInfo.FruitTreeReplacements[fruitTreeId] = fruitTreeValue;

				// Replace the fruit tree name/price/description
                string[] fruitTreeData = ItemList.OriginalItemList[fruitTreeId].Split("/");
				fruitTreeData[(int)ObjectInformationIndexes.Price] = $"{ price / 2 }";
                fruitTreeData[(int)ObjectInformationIndexes.DisplayName] = newDispName;
				fruitTreeData[(int)ObjectInformationIndexes.Description] = 
					Globals.GetTranslation(
						"sapling-description", 
						new { itemName = newDispName, season = seasonDisplay });

				editedObjectInfo.ObjectInformationReplacements[fruitTreeId] = string.Join("/", fruitTreeData);
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

				if (!Globals.Config.Crops.Randomize) { continue; } // Preserve the original seasons/etc
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

				if (!Globals.Config.Crops.Randomize) { continue; }

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
			if (!Globals.Config.Crops.Randomize) { return; }

			Item coffee = ItemList.Items[ObjectIndexes.Coffee];
			coffee.OverrideName = $"Hot {coffeeName}";
			coffee.CoffeeIngredient = coffeeName;
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.Coffee] = coffee.ToString();

			Item coffeeBean = ItemList.Items[ObjectIndexes.CoffeeBean];
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
			CropItem unmilledRice = (CropItem)ItemList.Items[ObjectIndexes.UnmilledRice];
			string riceName = unmilledRice.OverrideName;
			unmilledRice.OverrideName = $"Unmilled {riceName}";
			unmilledRice.OverrideDisplayName = Globals.GetTranslation("unmilled-rice-name", new { itemName = riceName });
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.UnmilledRice] = unmilledRice.ToString();

			Item rice = ItemList.Items[ObjectIndexes.Rice];
            rice.OverrideName = riceName;

			string[] riceData = ItemList.OriginalItemList[(int)ObjectIndexes.Rice].Split("/");
			riceData[(int)ObjectInformationIndexes.DisplayName] = riceName;
			editedObjectInfo.ObjectInformationReplacements[(int)ObjectIndexes.Rice] = string.Join("/", riceData);
		}

		/// <summary>
		/// Changes the names of the cooked food to match those of the objects themselves
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpCookedFood(EditedObjectInformation editedObjectInfo)
		{
			if (Globals.Config.Crops.Randomize)
			{
				CookedItem.GetAllCropDishes().ForEach(cropDish =>
				{
					cropDish.CalculateOverrideName();
                    editedObjectInfo.ObjectInformationReplacements[cropDish.Id] = cropDish.ToString();
                });
			}

			if (Globals.Config.Fish.Randomize)
			{
                CookedItem.GetAllFishDishes().ForEach(fishDish =>
                {
					fishDish.CalculateOverrideName();
                    editedObjectInfo.ObjectInformationReplacements[fishDish.Id] = fishDish.ToString();
                });
			}
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
			int baseValue;
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

			if (Globals.Config.Crops.Randomize)
			{
				Globals.SpoilerWrite("==== CROPS AND SEEDS ====");
				foreach (SeedItem seedItem in ItemList.GetSeeds())
				{
					if (seedItem.Id == (int)ObjectIndexes.CoffeeBean || seedItem.Id == (int)ObjectIndexes.AncientSeeds) { continue; }
					CropItem cropItem = (CropItem)ItemList.Items[(ObjectIndexes)seedItem.CropGrowthInfo.CropId];
					Globals.SpoilerWrite($"{cropItem.Id}: {cropItem.Name} - Seed Buy Price: {seedItem.Price * 2}G - Crop Sell Price: {cropItem.Price}G");
					Globals.SpoilerWrite($"{seedItem.Id}: {seedItem.Description}");
					Globals.SpoilerWrite("---");
				}
				Globals.SpoilerWrite("");
			}

			if (Globals.Config.RandomizeFruitTrees)
			{
				Globals.SpoilerWrite("==== FRUIT TREES ====");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.CherrySapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.AppleSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.OrangeSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.PeachSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.PomegranateSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.ApricotSapling)}");
				Globals.SpoilerWrite("");
			}
		}
	}
}
