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
using System.Collections.Generic;

namespace Randomizer
{
    public class FishData
	{
		/// <summary>
		/// The default fish data from Data/Fish.xnb
		/// </summary>
		public readonly static Dictionary<string, string> DefaultStringData =
			DataLoader.Fish(Game1.content);

		/// <summary>
		/// Indexes for the Data/Fish.xnb fields
		/// </summary>
		public enum FishFields
		{
			Name,
			DartChance,
			BehaviorType,
			MinSize,
			MaxSize,
			Times,
			Seasons,
			Weather,
			Unused,
			MinWaterDepth,
			SpawnMultiplier,
			DepthMultiplier,
			MinFishingLevel,
			IsValidTutorialFish
		}

        /// <summary>
        /// Populates the given fish with the default info
        /// </summary>
        /// <param name="fish">The fish</param>
        public static void FillDefaultFishInfo(FishItem fish)
		{
			string input = DefaultStringData[fish.Id.ToString()];

			string[] fields = input.Split('/');;
            if (fields.Length < 14)
			{
				Globals.ConsoleError($"Incorrect number of fields when parsing fish with input: {input}");
				return;
			}

			// Name
			fish.OriginalName = fields[(int)FishFields.Name];

            // Dart Chance
            if (!int.TryParse(fields[(int)FishFields.DartChance], out int dartChance))
			{
				Globals.ConsoleError($"Could not parse the dart chance when parsing fish with input: {input}");
				return;
			}
			fish.DartChance = dartChance;

			// Behavior type
			string behaviorTypeString = fields[(int)FishFields.BehaviorType];
			switch (behaviorTypeString)
			{
				case "mixed":
					fish.BehaviorType = FishBehaviorType.Mixed;
					break;
				case "dart":
					fish.BehaviorType = FishBehaviorType.Dart;
					break;
				case "smooth":
					fish.BehaviorType = FishBehaviorType.Smooth;
					break;
				case "floater":
					fish.BehaviorType = FishBehaviorType.Floater;
					break;
				case "sinker":
					fish.BehaviorType = FishBehaviorType.Sinker;
					break;
				default:
					Globals.ConsoleError($"Fish behavior type {behaviorTypeString} not found when parsing fish with input: {input}");
					return;
			}

			// Min Size
			if (!int.TryParse(fields[(int)FishFields.MinSize], out int minSize))
			{
				Globals.ConsoleError($"Could not parse the min size when parsing fish with input: {input}");
				return;
			}
			fish.MinSize = minSize;

			// Max Size
			if (!int.TryParse(fields[(int)FishFields.MaxSize], out int maxSize))
			{
				Globals.ConsoleError($"Could not parse the max size when parsing fish with input: {input}");
				return;
			}
			fish.MaxSize = maxSize;

			// Times
			List<int> times = ParseTimes(fields[(int)FishFields.Times]);
			if (times.Count == 2)
			{
				fish.Times = new Range(times[0], times[1]);
			}
			else if (times.Count == 4)
			{
				if (times[0] < times[1] && times[1] < times[2] && times[2] < times[3])
				{
					fish.Times = new Range(times[0], times[3]);
					fish.ExcludedTimes = new Range(times[1], times[2]);
				}
				else
				{
					Globals.ConsoleError($"Times are not in chronological order when parsing fish with input: {input}");
				}
			}

			// Seasons done during initialization (after the ItemList is initialized)

			// Weather
			string weather = fields[(int)FishFields.Weather];
			switch (weather)
			{
				case "sunny":
					fish.Weathers.Add(Weather.Sunny);
					break;
				case "rainy":
					fish.Weathers.Add(Weather.Rainy);
					break;
				case "both":
					fish.Weathers.Add(Weather.Sunny);
					fish.Weathers.Add(Weather.Rainy);
					break;
				default:
					Globals.ConsoleError($"Unexpected weather string when parsing fish with input: {input}");
					break;
			}

			// Unused
			fish.UnusedData = fields[(int)FishFields.Unused];

			// Min Water Depth,
			if (!int.TryParse(fields[(int)FishFields.MinWaterDepth], out int minWaterDepth))
			{
				Globals.ConsoleError($"Could not parse the min water depth when parsing fish with input: {input}");
				return;
			}
			fish.MinWaterDepth = minWaterDepth;

			// Spawn Multiplier
			if (!double.TryParse(fields[(int)FishFields.SpawnMultiplier], out double spawnMultiplier))
			{
				Globals.ConsoleError($"Could not parse the spawn multiplier when parsing fish with input: {input}");
				return;
			}
			fish.SpawnMultiplier = spawnMultiplier;

			// Depth Multiplier
			if (!double.TryParse(fields[(int)FishFields.DepthMultiplier], out double depthMultiplier))
			{
				Globals.ConsoleError($"Could not parse the depth multiplier when parsing fish with input: {input}");
				return;
			}
			fish.DepthMultiplier = depthMultiplier;

			// Min Fishing Level
			if (!int.TryParse(fields[(int)FishFields.MinFishingLevel], out int minFishingLevel))
			{
				Globals.ConsoleError($"Could not parse the min fishing level when parsing fish with input: {input}");
				return;
			}
			fish.MinFishingLevel = minFishingLevel;

			// Is Valid Tutorial Fish
			if (!bool.TryParse(fields[(int)FishFields.IsValidTutorialFish], out bool isValidTutorialFish))
			{
                Globals.ConsoleError($"Could not parse whether the fish is a valid tutorial fish when parsing fish with input: {input}");
                return;
            }
            fish.IsValidTutorialFish = isValidTutorialFish;
        } 

		/// <summary>
		/// Parses the given time string into a list of integers
		/// </summary>
		/// <param name="timeString">The time string</param>
		/// <returns />
		private static List<int> ParseTimes(string timeString)
		{
			string[] timeStringParts = timeString.Split(' ');
			List<int> times = new();

			foreach (string time in timeStringParts)
			{
				if (!int.TryParse(time, out int intTime))
				{
					Globals.ConsoleError($"Could not convert time to integer in {timeString}");
					return null;
				}
				times.Add(intTime);
			}

			return times;
		}
	}
}
