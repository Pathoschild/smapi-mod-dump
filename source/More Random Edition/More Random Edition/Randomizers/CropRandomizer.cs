/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Randomizes crops
    /// </summary>
    public class CropRandomizer
	{
        private static RNG Rng { get; set; }

        public static void Randomize(EditedObjects editedObjectInfo)
		{
            if (!Globals.Config.Crops.Randomize) 
			{
				// Currently this also handles fish dishes... so call this here
				// so we don't accidently skip any renames here
                SetUpCookedFood(editedObjectInfo);
                return;
			}

            Rng = RNG.GetFarmRNG(nameof(CropRandomizer));
			RandomizeCrops(editedObjectInfo);

			WriteToSpoilerLog();
		}

		/// <summary>
		/// Randomizes the crops - currently only does prices, and only for seasonal crops
		/// </summary>
		/// <param name="editedObjectInfo">The edited object information</param>
		private static void RandomizeCrops(EditedObjects editedObjectInfo)
		{
			List<string> regrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && x.RegrowsAfterHarvest && x.ShuffleBetweenSeeds)
				.Select(x => x.Id)
				.ToList();
			List<string> regrowableSeedIdsToRandomizeCopy = new(regrowableSeedIdsToRandomize);

			List<string> nonRegrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && !x.RegrowsAfterHarvest && x.ShuffleBetweenSeeds)
				.Select(x => x.Id)
				.ToList();
			List<string> nonRegrowableSeedIdsToRandomizeCopy = new(nonRegrowableSeedIdsToRandomize);

			List<string> staticSeedsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && !x.ShuffleBetweenSeeds)
				.Select(x => x.Id)
				.ToList();

			// Fill up a dictionary to remap the seed values
			Dictionary<string, string> seedMappings = new(); // Original value, new value

			foreach (string originalRegrowableSeedId in regrowableSeedIdsToRandomize)
			{
				seedMappings.Add(originalRegrowableSeedId, Rng.GetAndRemoveRandomValueFromList(regrowableSeedIdsToRandomizeCopy));
			}

			foreach (string originalNonRegrowableSeedId in nonRegrowableSeedIdsToRandomize)
			{
				seedMappings.Add(originalNonRegrowableSeedId, Rng.GetAndRemoveRandomValueFromList(nonRegrowableSeedIdsToRandomizeCopy));
			}

			foreach(string staticSeedId in staticSeedsToRandomize)
			{
				// These are static, so map it to itself!
				seedMappings.Add(staticSeedId, staticSeedId);
			}

			// Loop through the dictionary and reassign the values, keeping the seasons the same as before
			foreach (KeyValuePair<string, string> seedMapping in seedMappings)
			{
				string originalValue = seedMapping.Key;
				string newValue = seedMapping.Value;

                CropData newCropGrowthInfo = DataLoader.Crops(Game1.content)[newValue.ToString()].DeepClone();
                newCropGrowthInfo.Seasons = DataLoader.Crops(Game1.content)[originalValue.ToString()].DeepClone().Seasons;
				newCropGrowthInfo.DaysInPhase = GetRandomGrowthStages(newCropGrowthInfo.DaysInPhase.Count);
				newCropGrowthInfo.HarvestMethod = Rng.NextBoolean(10)
					? HarvestMethod.Scythe
					: HarvestMethod.Grab;
				newCropGrowthInfo.RegrowDays = newCropGrowthInfo.RegrowDays == -1
					? -1
					: Rng.NextIntWithinRange(1, 7);

				var originalSeedItem = ItemList.Items[originalValue] as SeedItem;
				originalSeedItem.CropGrowthInfo = newCropGrowthInfo;
			}

			// Set the object info
			List<CropItem> randomizedCrops = ItemList.GetCrops(true).Cast<CropItem>()
				.Where(x => nonRegrowableSeedIdsToRandomize
					.Union(regrowableSeedIdsToRandomize)
					.Union(staticSeedsToRandomize).Contains(x.MatchingSeedItem.Id))
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

			SetUpCoffee(editedObjectInfo, vegetableNames[^1]);
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
			List<int> growthStages = new();

			for (int i = 0; i < numberOfStages; i++)
			{
				growthStages.Add(Rng.NextIntWithinRange(1, maxValuePerStage));
			}

			return growthStages;
		}

		/// <summary>
		/// Sets the crop and seed display name, price, and description
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="crops">The crops to set</param>
		/// <param name="randomNames">The random names to give the crops</param>
		private static void SetCropAndSeedInformation(
			EditedObjects editedObjectInfo,
			List<CropItem> crops,
			List<string> randomNames,
			List<string> randomDescriptions)
		{
			for (int i = 0; i < crops.Count; i++)
			{
                CropItem crop = crops[i];
				SeedItem seed = crop.MatchingSeedItem;

				string name = randomNames[i];
                string seasonsString = Globals.GetTranslation(
					"crop-tooltip-seasons", 
					new { seasons = seed.GetSeasonsStringForDisplay() });
				string description = Rng.GetAndRemoveRandomValueFromList(randomDescriptions);
				crop.OverrideName = name;
				crop.Description = $"{description} {seasonsString}";

				seed.OverrideDisplayName = seed.IsTrellisCrop
					? Globals.GetTranslation("trellis-text", new { itemName = name })
					: Globals.GetTranslation("seed-text", new { itemName = name });
				seed.OverrideName = seed.IsTrellisCrop
					? $"{name} Starter"
					: $"{name} Seeds";

				seed.Price = GetRandomSeedPrice();
				crop.Price = CalculateCropPrice(seed);

				ObjectData cropObject = Game1.objectData[crop.Id].DeepClone();
				cropObject.DisplayName = crop.Name;
				cropObject.Description = crop.Description;
				cropObject.Price = crop.Price;

                ObjectData seedObject = Game1.objectData[seed.Id].DeepClone();
                seedObject.DisplayName = seed.Name;
                seedObject.Description = seed.Description;
                seedObject.Price = seed.Price;

                editedObjectInfo.ObjectsReplacements[crop.Id] = cropObject;
				editedObjectInfo.ObjectsReplacements[seed.Id] = seedObject;
			}
		}

		/// <summary>
		/// Sets up the coffee beans and coffee objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="coffeeItemName">The name of the coffee item</param>
		private static void SetUpCoffee(EditedObjects editedObjectInfo, string coffeeItemName)
		{
			Item coffee = ObjectIndexes.Coffee.GetItem();
			coffee.OverrideName = $"Hot {coffeeItemName}";
            coffee.CoffeeIngredient = coffeeItemName; // Used for the description of the coffee bean

			ObjectData coffeeObject = Game1.objectData[coffee.Id].DeepClone();
            coffeeObject.DisplayName = Globals.GetTranslation("item-coffee-name", new { itemName = coffeeItemName });
            editedObjectInfo.ObjectsReplacements[coffee.Id] = coffeeObject;

            SeedItem coffeeBean = ObjectIndexes.CoffeeBean.GetItem() as SeedItem;
			coffeeBean.OverrideName = $"{coffeeItemName} Bean";
			coffeeBean.OverrideDisplayName = Globals.GetTranslation("coffee-bean-name", new { itemName = coffeeItemName });

            ObjectData coffeeBeanObject = Game1.objectData[coffeeBean.Id].DeepClone();
			coffeeBeanObject.DisplayName = coffeeBean.Name;
			coffeeBeanObject.Description = coffeeBean.Description;
            editedObjectInfo.ObjectsReplacements[coffeeBean.Id] = coffeeBeanObject;
		}

		/// <summary>
		/// Sets up the rice objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpRice(EditedObjects editedObjectInfo)
		{
			CropItem unmilledRice = ObjectIndexes.UnmilledRice.GetItem() as CropItem;
			string riceName = unmilledRice.OverrideName;
			unmilledRice.OverrideName = $"Unmilled {riceName}";
			unmilledRice.OverrideDisplayName = Globals.GetTranslation("unmilled-rice-name", new { itemName = riceName });

            ObjectData unmilledRiceObject = Game1.objectData[unmilledRice.Id].DeepClone();
			unmilledRiceObject.DisplayName = unmilledRice.Name;
            editedObjectInfo.ObjectsReplacements[unmilledRice.Id] = unmilledRiceObject;

			Item rice = ObjectIndexes.Rice.GetItem();
            rice.OverrideName = riceName;

            ObjectData riceObject = Game1.objectData[rice.Id].DeepClone();
            riceObject.DisplayName = rice.Name;
            editedObjectInfo.ObjectsReplacements[rice.Id] = riceObject;
        }

		/// <summary>
		/// Changes the names of the cooked food to match those of the objects themselves
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpCookedFood(EditedObjects editedObjectInfo)
		{
			if (Globals.Config.Crops.Randomize)
			{
				CookedItem.GetAllCropDishes().ForEach(cropDish =>
				{
					cropDish.CalculateOverrideName();

                    ObjectData cropDishObject = Game1.objectData[cropDish.Id].DeepClone();
                    cropDishObject.DisplayName = cropDish.Name;
                    editedObjectInfo.ObjectsReplacements[cropDish.Id] = cropDishObject;
                });
			}

			if (Globals.Config.Fish.Randomize)
			{
                CookedItem.GetAllFishDishes().ForEach(fishDish =>
                {
					fishDish.CalculateOverrideName();

                    ObjectData fishDishObject = Game1.objectData[fishDish.Id].DeepClone();
                    fishDishObject.DisplayName = fishDish.Name;
                    editedObjectInfo.ObjectsReplacements[fishDish.Id] = fishDishObject;
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
			int generatedValue = Rng.NextIntWithinRange(1, 100);
			int baseValue;
			if (generatedValue < 41)
			{
				baseValue = Rng.NextIntWithinRange(10, 30);
			}
			else if (generatedValue < 71)
			{
				baseValue = Rng.NextIntWithinRange(31, 60);
			}
			else if (generatedValue < 86)
			{
				baseValue = Rng.NextIntWithinRange(61, 90);
			}
			else if (generatedValue < 96)
			{
				baseValue = Rng.NextIntWithinRange(91, 120);
			}
			else
			{
				baseValue = Rng.NextIntWithinRange(121, 150);
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
			CropData growthInfo = seed.CropGrowthInfo;

			double multiplier;
			if (seedPrice < 31) { multiplier = Rng.NextIntWithinRange(15, 40) / (double)10; }
			else if (seedPrice < 61) { multiplier = Rng.NextIntWithinRange(15, 35) / (double)10; }
			else if (seedPrice < 91) { multiplier = Rng.NextIntWithinRange(15, 30) / (double)10; }
			else if (seedPrice < 121) { multiplier = Rng.NextIntWithinRange(15, 25) / (double)10; }
			else { multiplier = Rng.NextIntWithinRange(15, 20) / (double)10; }

			double regrowthDaysMultiplier;
			switch (growthInfo.RegrowDays)
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
			switch (growthInfo.HarvestMinStack)
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
			if (seed.CanGetExtraCrops && amountPerHarvestMultiplier == 1)
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
			Globals.SpoilerWrite("==== CROPS AND SEEDS ====");
            foreach (SeedItem seedItem in ItemList.GetSeeds().Cast<SeedItem>())
			{
				if (seedItem.ObjectIndex == ObjectIndexes.CoffeeBean || 
					seedItem.ObjectIndex == ObjectIndexes.AncientSeeds) 
				{ 
					continue; 
				}

				CropItem cropItem = (CropItem)ItemList.Items[seedItem.CropId];
				Globals.SpoilerWrite($"{cropItem.Id}: {cropItem.Name} - Seed Buy Price: {seedItem.Price * 2}G - Crop Sell Price: {cropItem.Price}G");
				Globals.SpoilerWrite($"{seedItem.Id}: {seedItem.Description}");
				Globals.SpoilerWrite("---");
			}
			Globals.SpoilerWrite("");
		}
	}
}
