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
			List<FishItem> normalFishCopy = new List<FishItem>();
			foreach (FishItem fish in normalFish)
			{
				FishItem fishInfo = new FishItem(fish.Id, true);
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

				if (!Globals.Config.RandomizeFish) { continue; }

				CopyFishInfo(fishToReplace, fish);
				fish.DartChance = newDartChance;
				fish.BehaviorType = newBehaviorType;
				fish.OverrideName = newName;

				if (new int[] { 158, 161, 162 }.Contains(fish.Id)) // The three hard-coded mines fish
				{
					if (!fish.AvailableLocations.Contains(Locations.UndergroundMine))
					{
						fish.AvailableLocations.Add(Locations.UndergroundMine);
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

				if (!Globals.Config.RandomizeFish) { continue; }

				fish.BehaviorType = newBehaviorType;
				fish.OverrideName = newName;

				editedObjectInfo.FishReplacements.Add(fish.Id, fish.ToString());
				editedObjectInfo.ObjectInformationReplacements.Add(fish.Id, GetFishObjectInformation(fish));
			}

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
			string defaultObjectInfo = FishData.DefaultObjectInformation[fish.Id];
			string[] objectInfoParts = defaultObjectInfo.Split('/');

			objectInfoParts[0] = fish.OverrideName;
			objectInfoParts[4] = fish.OverrideName;
			objectInfoParts[5] = fish.Description;
			objectInfoParts[6] = fish.ObjectInformationSuffix;

			return string.Join("/", objectInfoParts);
		}

		/// <summary>
		/// Writes the relevant changes to the spoiler log
		/// </summary>
		public static void WriteToSpoilerLog()
		{
			if (!Globals.Config.RandomizeFish) { return; }

			List<FishItem> allRandomizedFish = FishItem.GetListAsFishItem();

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
