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
    public class FishData
	{
		/// <summary>
		/// The default fish data from Data/Fish.xnb
		/// </summary>
		public readonly static Dictionary<int, string> DefaultStringData =
			Globals.ModRef.Helper.GameContent.Load<Dictionary<int, string>>("Data/Fish");

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
			MinFishingLevel
		}

		/// <summary>
		/// We will use the location data to initialize all fish's locations and seasons
		/// as the fish data is unreliable - the locations are ACTUALLY what are used
		/// </summary>
		public static void InitializeFishToLocations()
		{
			InitializeNightMarketFish();
			InitializeMinesFish();
			InializeLegendaryFish();

            // Go through each location and add to the location dictionary
            foreach (var locData in LocationData.DefaultLocationData)
			{
				string locationName = locData.Key;
				if (!Enum.TryParse(locationName, out Locations location))
				{
					// This is okay, we don't have every location mapped
					continue; 
				}

				var locDataParts = locData.Value.Split("/");
				var springFish = locDataParts[(int)LocationDataIndexes.SpringFish];
                var summerFish = locDataParts[(int)LocationDataIndexes.SummerFish];
                var fallFish = locDataParts[(int)LocationDataIndexes.FallFish];
                var winterFish = locDataParts[(int)LocationDataIndexes.WinterFish];

				AssignFishDataFromStringList(springFish, location, Seasons.Spring);
                AssignFishDataFromStringList(summerFish, location, Seasons.Summer);
                AssignFishDataFromStringList(fallFish, location, Seasons.Fall);
                AssignFishDataFromStringList(winterFish, location, Seasons.Winter);
            }
        }

		/// <summary>
		/// Takes in a string of location fish data (the space delimited string of ids and -1's)
		/// and assigns the given location and season to the fish in the list
		/// </summary>
		/// <param name="fishData">The fish data string</param>
		/// <param name="location">The location of the fish to assign</param>
		/// <param name="season">The season to assign</param>
		private static void AssignFishDataFromStringList(
			string fishData, Locations location, Seasons season)
		{
            List<FishItem> fishItems = ItemList
                .GetItemListFromString(fishData)
                .Distinct()
                .Where(item => item is FishItem)
                .Cast<FishItem>()
                .ToList();

            foreach (FishItem fishItem in fishItems)
            {
                int fishId = fishItem.Id;
				fishItem.AvailableLocations ??= new List<Locations>();
                fishItem.AvailableSeasons ??= new List<Seasons>();

                // For some reason, Legend is marked as backwoods, and
                // the rando won't work properly if it's NOT assigned to Mountain
				// In all other cases, backwoods is equivalent, so this logic should be fine
                Locations locToAdd = location == Locations.Backwoods
                    ? locToAdd = Locations.Mountain
                    : location;
                if (!fishItem.AvailableLocations.Contains(locToAdd))
                {
                    fishItem.AvailableLocations.Add(locToAdd);
                }
                if (!fishItem.AvailableSeasons.Contains(season))
                {
                    fishItem.AvailableSeasons.Add(season);
                }
            }
        }

        /// <summary>
        /// These fish are underground but are NOT in Data/Locations
        /// since they are hard-coded to be caught in the game
        /// </summary>
        private static void InitializeMinesFish()
		{
            Item[] minesFishList = {
                ItemList.Items[ObjectIndexes.Stonefish],
                ItemList.Items[ObjectIndexes.IcePip],
                ItemList.Items[ObjectIndexes.LavaEel]
            };
            foreach (FishItem minesFish in minesFishList.Cast<FishItem>())
            {
                minesFish.AvailableLocations = new List<Locations>()
                    { Locations.UndergroundMine };
                minesFish.AvailableSeasons = new List<Seasons>()
                {
                    Seasons.Spring,
                    Seasons.Summer,
                    Seasons.Fall,
                    Seasons.Winter
                };
            }
        }

		/// <summary>
		/// The night market is not defined in Data/Locations, but we use it
		/// internally - we need to manually set this data accordingly
		/// </summary>
		private static void InitializeNightMarketFish()
		{
            // The night market is NOT a "real" location, so we will add these manually
            Item[] nightMarketFishList = {
                ItemList.Items[ObjectIndexes.MidnightSquid],
                ItemList.Items[ObjectIndexes.SpookFish],
                ItemList.Items[ObjectIndexes.Blobfish]
            };
            foreach (FishItem nightMarketFish in nightMarketFishList.Cast<FishItem>())
            {
                nightMarketFish.AvailableLocations = new List<Locations>()
                    { Locations.NightMarket };
                nightMarketFish.AvailableSeasons = new List<Seasons>()
                {
                    Seasons.Spring,
                    Seasons.Summer,
                    Seasons.Fall,
                    Seasons.Winter
                };
            }
        }

		/// <summary>
		/// The legendary fish are not defined in Data/Locations,
		/// as they are hard-coded
		/// </summary>
		private static void InializeLegendaryFish()
		{
			FishItem crimsonFish = ItemList.Items[ObjectIndexes.Crimsonfish] as FishItem;
			crimsonFish.AvailableLocations = new List<Locations> { Locations.Beach };
			crimsonFish.AvailableSeasons = new List<Seasons>() { Seasons.Summer };

            FishItem angler = ItemList.Items[ObjectIndexes.Angler] as FishItem;
            angler.AvailableLocations = new List<Locations> { Locations.Town };
            angler.AvailableSeasons = new List<Seasons>() { Seasons.Fall };

			FishItem mutantCarp = ItemList.Items[ObjectIndexes.MutantCarp] as FishItem;
            mutantCarp.AvailableLocations = new List<Locations> { Locations.Sewer };
			mutantCarp.AvailableSeasons = new List<Seasons>()
            {
                Seasons.Spring,
                Seasons.Summer,
                Seasons.Fall,
                Seasons.Winter
            };

            FishItem glacierfish = ItemList.Items[ObjectIndexes.Glacierfish] as FishItem;
            glacierfish.AvailableLocations = new List<Locations> { Locations.Forest };
            glacierfish.AvailableSeasons = new List<Seasons>() { Seasons.Winter };
        }

        /// <summary>
        /// Populates the given fish with the default info
        /// </summary>
        /// <param name="fish">The fish</param>
        public static void FillDefaultFishInfo(FishItem fish)
		{
			string input = DefaultStringData[fish.Id];

			string[] fields = input.Split('/');;
            if (fields.Length < 13)
			{
				Globals.ConsoleError($"Incorrect number of fields when parsing fish with input: {input}");
				return;
			}

            // Name
            // Skipped because it's computed from the id

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
