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
	/// <summary>
	/// Randomizes fish
	/// </summary>
	public class FishRandomizer
	{
		public static void Randomize(EditedObjectInformation editedObjectInfo)
		{
			List<FishItem> legendaryFish = FishItem.GetLegendaries().Cast<FishItem>().ToList();
			List<FishItem> normalFish = FishItem.Get().Cast<FishItem>().ToList();
			List<FishItem> normalFishCopy = new();
			foreach (FishItem fish in normalFish)
			{
				FishItem fishInfo = new(fish.Id, true); // A constructor that does nothing
				CopyFishInfo(fish, fishInfo);
				normalFishCopy.Add(fishInfo);
			}

			List<string> fishNames = NameAndDescriptionRandomizer.GenerateFishNames(normalFish.Count + legendaryFish.Count);
			foreach (FishItem fish in normalFish)
			{
				FishItem fishToReplace = Globals.RNGGetAndRemoveRandomValueFromList(normalFishCopy);
				int newDartChance = GenerateRandomFishDifficulty();
				FishBehaviorType newBehaviorType = Globals.RNGGetRandomValueFromList(
					Enum.GetValues(typeof(FishBehaviorType)).Cast<FishBehaviorType>().ToList());
				string newName = Globals.RNGGetAndRemoveRandomValueFromList(fishNames);

				if (!Globals.Config.Fish.Randomize) { continue; }

				CopyFishInfo(fishToReplace, fish);
				fish.DartChance = newDartChance;
				fish.BehaviorType = newBehaviorType;
				fish.OverrideName = newName;

				// Locations are copied over, but these specific IDs
				// will ALWAYS spawn here, so add the location to them
				if (fish.IsMinesFish)
				{
					if (!fish.AvailableLocations.Contains(Locations.UndergroundMine))
					{
						fish.AvailableLocations.Add(Locations.UndergroundMine);
					}
				}

				if (Globals.Config.Fish.Randomize)
				{
					if (fish.IsSubmarineOnlyFish)
					{
						fish.DifficultyToObtain = ObtainingDifficulties.RareItem;
					}
					else
					{
						fish.DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
					}
				}

				editedObjectInfo.FishReplacements.Add(fish.Id, fish.ToString());
				editedObjectInfo.ObjectInformationReplacements.Add(fish.Id, GetFishObjectInformation(fish));
			}

			foreach (FishItem fish in legendaryFish)
			{
				FishBehaviorType newBehaviorType = Globals.RNGGetRandomValueFromList(
					Enum.GetValues(typeof(FishBehaviorType)).Cast<FishBehaviorType>().ToList());

				string newName = Globals.RNGGetAndRemoveRandomValueFromList(fishNames);

				if (!Globals.Config.Fish.Randomize) { continue; }

				fish.BehaviorType = newBehaviorType;
				fish.OverrideName = newName;

				editedObjectInfo.FishReplacements.Add(fish.Id, fish.ToString());
				editedObjectInfo.ObjectInformationReplacements.Add(fish.Id, GetFishObjectInformation(fish));
			}

            // Keeping this here for debugging purposes
			// Uncomment to print out all randomized seasons and locations
            //List<int> sortedFishIds = ItemList.Items.Values
            //    .Where(x => x is FishItem)
            //    .Select(x => x.Id).ToList();
            //sortedFishIds.Sort();
            //foreach (int fishId in sortedFishIds)
            //{
            //    List<Seasons> seasons = (ItemList.Items[(ObjectIndexes)fishId] as FishItem).AvailableSeasons;
            //    List<Locations> locs = (ItemList.Items[(ObjectIndexes)fishId] as FishItem).AvailableLocations;
            //    string itemName = ItemList.Items[(ObjectIndexes)fishId].Name;
            //    Globals.ConsoleWarn($"{fishId} {itemName}: {string.Join(" ", seasons.Select(x => x.ToString().ToLower()))}");
            //    Globals.ConsoleWarn($"{fishId} {itemName}: {string.Join(" ", locs.Select(x => x.ToString().ToLower()))}");
            //}

            WriteToSpoilerLog();
		}

		/// <summary>
		/// Copies a select set of info from one fish to another
		/// </summary>
		/// <param name="fromFish">The fish to copy from</param>
		/// <param name="toFish">The fish to copy to</param>
		private static void CopyFishInfo(FishItem fromFish, FishItem toFish)
		{
			toFish.Times = new Range(fromFish.Times.MinValue, fromFish.Times.MaxValue);
			toFish.ExcludedTimes = new Range(fromFish.ExcludedTimes.MinValue, fromFish.ExcludedTimes.MaxValue);
			toFish.Weathers = new List<Weather>(fromFish.Weathers);
			toFish.MinWaterDepth = fromFish.MinWaterDepth;
			toFish.DepthMultiplier = fromFish.DepthMultiplier;
			toFish.AvailableLocations = new List<Locations>(fromFish.AvailableLocations);
			toFish.AvailableSeasons = new List<Seasons>(fromFish.AvailableSeasons);
		}

		/// <summary>
		/// Gets a random fish difficulty value
		/// </summary>
		/// <returns>
		/// 25% Easy: 15 - 30
		/// 45% Moderate: 31 - 50
		/// 25% Difficult: 51 - 75
		/// 5% WTF: 76 - 95
		/// 0% Legendary: 96 - 110
		/// </returns>
		private static int GenerateRandomFishDifficulty()
		{
			int difficultyRange = Range.GetRandomValue(1, 100);
			if (difficultyRange < 26)
			{
				return Range.GetRandomValue(15, 30);
			}
			else if (difficultyRange < 71)
			{
				return Range.GetRandomValue(31, 50);
			}
			else if (difficultyRange < 96)
			{
				return Range.GetRandomValue(51, 75);
			}
			else
			{
				return Range.GetRandomValue(76, 95);
			}
		}

		/// <summary>
		/// Gets the fish object info string
		/// </summary>
		/// <param name="fish">The fish</param>
		/// <returns />
		private static string GetFishObjectInformation(FishItem fish)
		{
            string defaultObjectInfo = ItemList.OriginalItemList[fish.Id];
            string[] objectInfoParts = defaultObjectInfo.Split('/');

            objectInfoParts[(int)ObjectInformationIndexes.DisplayName] = fish.OverrideName;
			objectInfoParts[(int)ObjectInformationIndexes.Description] = fish.Description;
			objectInfoParts[(int)ObjectInformationIndexes.AdditionalFishInfo] = fish.ObjectInformationSuffix;

			return string.Join("/", objectInfoParts);
		}

		/// <summary>
		/// Writes the relevant changes to the spoiler log
		/// </summary>
		public static void WriteToSpoilerLog()
		{
			if (!Globals.Config.Fish.Randomize) { return; }

			List<FishItem> allRandomizedFish = FishItem.GetListAsFishItem(true);

			Globals.SpoilerWrite("==== FISH ====");
			foreach (FishItem fish in allRandomizedFish)
			{
				Globals.SpoilerWrite($"{fish.Id}: {fish.Name}");
				Globals.SpoilerWrite($"Difficulty: {fish.DartChance} - Level Req: {fish.MinFishingLevel} - Water depth: {fish.MinWaterDepth}");
				Globals.SpoilerWrite(fish.Description);
				Globals.SpoilerWrite("---");
			}
			Globals.SpoilerWrite("");
		}
	}
}
