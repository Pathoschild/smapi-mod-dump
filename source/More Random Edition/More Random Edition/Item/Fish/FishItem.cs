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
using System.Globalization;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Represents a fish
	/// </summary>
	public class FishItem : Item
	{
		/// <summary>
		/// TODO: remove this in the next major update
		///   This is used to put the tooltip info in for the fish so we don't change the RNG by
		///   populating the seasons/locations
		/// </summary>
		public bool IsNewMinesFish { get; set; }

		public string OriginalName { get; set; }
		public List<Seasons> AvailableSeasons { get; set; } = new List<Seasons>();
		public List<Weather> Weathers { get; set; } = new List<Weather>();
		public List<Locations> AvailableLocations { get; set; } = new List<Locations>();
		public string MineFloorString { get; set; }
		public Range Times { get; set; } = new Range(600, 2600); // That's anytime in the day
		public Range ExcludedTimes { get; set; } = new Range(0, 0);

		public bool IsNightFish { get { return Times.MaxValue >= 1800; } }
		public bool IsRainFish { get { return Weathers.Contains(Weather.Rainy); } }
		public bool IsSunFish { get { return Weathers.Contains(Weather.Sunny); } }

		public bool IsSpringFish { get { return AvailableSeasons.Contains(Seasons.Spring) && !IsSubmarineOnlyFish; } }
		public bool IsSummerFish { get { return AvailableSeasons.Contains(Seasons.Summer) && !IsSubmarineOnlyFish; } }
		public bool IsFallFish { get { return AvailableSeasons.Contains(Seasons.Fall) && !IsSubmarineOnlyFish; } }
		public bool IsWinterFish { get { return AvailableSeasons.Contains(Seasons.Winter); } }

		public int DartChance { get; set; }
		public FishBehaviorType BehaviorType { get; set; }
		public int MinSize { get; set; }
		public int MaxSize { get; set; }
		public string UnusedData { get; set; }
		public int MinWaterDepth { get; set; }
		public double SpawnMultiplier { get; set; }
		public double DepthMultiplier { get; set; }
		public int MinFishingLevel { get; set; }
		public bool IsValidTutorialFish { get; set; }

		/// <summary>
		/// Returns whether this fish is a mines fish
		/// These are the three hard-coded mines-specific fish
		/// </summary>
		public bool IsMinesFish
		{
			get
			{
				return new string[] { 
					ObjectIndexes.Stonefish.GetId(), 
					ObjectIndexes.IcePip.GetId(), 
					ObjectIndexes.LavaEel.GetId() }.Contains(Id);
			}
		}

		/// <summary>
		/// Returns whether this is one of the 5 legendary fish
		/// </summary>
		public bool IsLegendaryFish
		{
			get
			{
				return IsLegendary(Id);
			}
		}

		/// <summary>
		/// Returns whether the given ID maps to one of the legendary fish
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool IsLegendary(string id)
		{
			return new string[] {
				ObjectIndexes.Crimsonfish.GetId(),
				ObjectIndexes.Angler.GetId(),
				ObjectIndexes.Legend.GetId(),
				ObjectIndexes.Glacierfish.GetId(),
				ObjectIndexes.MutantCarp.GetId()
			}.Contains(id);
		}

		/// <summary>
		/// Returns whether this fish is a submarine only fish
		/// This will let us know whether it's actually a winter only fish as well
		/// Used with retrieving season-specific fish and descriptions
		/// </summary>
		public bool IsSubmarineOnlyFish
		{
			get
			{
				return AvailableLocations.Count == 1 && AvailableLocations.Contains(Locations.Submarine);
			}
		}

		/// <summary>
		/// The description - used in the tooltip
		/// </summary>
		public string Description
		{
			get
			{
				string seasonsString = GetStringForSeasons();
				string locationString = GetStringForLocations();

				if (IsSubmarineOnlyFish)
				{
					return $"{seasonsString} {locationString}";
				}

				string timesString = GetTimesString();
				string weatherString = GetStringForWeather();
				string initialDescription = $"{timesString} {seasonsString} {locationString} {weatherString}";

				string legendaryString = GetStringForLegendary();
				return $"{initialDescription.Trim()} {legendaryString}";
			}
		}

		public FishItem(ObjectIndexes index, 
			ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.LargeTimeRequirements) 
			: base(index)
		{
			DifficultyToObtain = difficultyToObtain;
			IsFish = true;
		}

		public FishItem(ObjectIndexes index, bool boop) : base(index)
		{
		}

		/// <summary>
		/// Gets the string to be used for the time part of the description
		/// </summary>
		/// <returns />
		private string GetTimesString()
		{
			string timesString;
			if (ExcludedTimes.MinValue > 0 && ExcludedTimes.MaxValue > 0)
			{
				string fromTime = GetStringForTime(ExcludedTimes.MinValue);
				string toTime = GetStringForTime(ExcludedTimes.MaxValue);
				timesString = Globals.GetTranslation("fish-tooltip-excluded-times", new { fromTime, toTime });
			}
			else if (Times.MinValue == 600 && Times.MaxValue == 2600)
			{
				timesString = Globals.GetTranslation("fish-tooltip-all-day");
			}
			else
			{
				string fromTime = GetStringForTime(Times.MinValue);
				string toTime = GetStringForTime(Times.MaxValue);
				timesString = Globals.GetTranslation("fish-tooltip-time-range", new { fromTime, toTime });
			}
			return timesString;
		}

		/// <summary>
		/// Converts the given time to a 12-hour time,
		/// e.g. 1400 - 2:00pm
		/// </summary>
		/// <param name="timeRange"></param>
		/// <return />
		private static string GetStringForTime(int time)
		{
			if (time > 2359)
			{
				time -= 2400;
			}
			string timeString = time.ToString("D4");
			DateTime dateTime = DateTime.ParseExact(timeString, "HHmm", CultureInfo.InvariantCulture);
			CultureInfo culture = CultureInfo.CreateSpecificCulture(Globals.ModRef.Helper.Translation.Locale);
			return dateTime.ToString(Globals.GetTranslation("time-format"), culture).ToLower();
		}

		/// <summary>
		/// Gets the string to be used in the description for locations
		/// </summary>
		/// <returns>A string in the following format: Lives in the [loc1], [loc2], and [loc3].</returns>
		private string GetStringForLocations()
		{
			// TODO: remove this in the next major update
			if (IsNewMinesFish)
			{
				var minesLoc = $"{Globals.GetTranslation($"fish-undergroundmine-location")} {MineFloorString}";
				return Globals.GetTranslation("fish-tooltip-locations", new { locations = minesLoc });
			}

			if (AvailableLocations.Count == 0) 
			{ 
				return ""; 
			}

			List<string> locationStrings = GetLocationStrings();
			string locations = string.Join(", ", locationStrings);
			return Globals.GetTranslation("fish-tooltip-locations", new { locations });
		}

		/// <summary>
		/// Gets the string to be used in the description for seasons
		/// </summary>
		/// <return />
		private string GetStringForSeasons()
		{
			// TODO: remove this in the next major update
			if (IsNewMinesFish)
			{
				return Globals.GetTranslation("fish-tooltip-seasons-all");
			}

			if (AvailableSeasons.Count == 0) 
			{ 
				return ""; 
			}

			if (IsSubmarineOnlyFish)
			{
				string winterSeason = Globals.GetTranslation($"seasons-winter");
				return Globals.GetTranslation("fish-tooltip-seasons", new { seasons = winterSeason });
			}
			else if (AvailableSeasons.Count == 4)
			{
				return Globals.GetTranslation("fish-tooltip-seasons-all");
			}

			string[] seasonStrings = AvailableSeasons
				.Select(x => x.ToString().ToLower())
				.Select(x => Globals.GetTranslation($"seasons-{x}"))
				.ToArray();

			string seasons = string.Join(", ", seasonStrings);
			return Globals.GetTranslation("fish-tooltip-seasons", new { seasons });
		}

		/// <summary>
		/// Gets a list of strings for the locations to be used in the description
		/// </summary>
		/// <returns></returns>
		private List<string> GetLocationStrings()
		{
			List<string> output = new();
			foreach (Locations location in AvailableLocations)
			{
				string translation = Globals.GetTranslation($"fish-{location.ToString().ToLower()}-location");
				if (location == Locations.UndergroundMine)
				{
					translation += $" {ComputeMineFloorString(QualifiedId)}";
				}
				output.Add(translation);
			}
			return output;
		}

		/// <summary>
		/// Gets the string used for the weather part of the tooltip
		/// </summary>
		/// <returns />
		public string GetStringForWeather()
		{
			if (Weathers.Count != 1) { return ""; }

			string weather = Globals.GetTranslation($"fish-weather-{Weathers[0].ToString().ToLower()}");
			return Globals.GetTranslation("fish-tooltip-weather", new { weather });
		}

		/// <summary>
		/// Gets the string to use for when the fish is a legendary fish
		/// </summary>
		/// <returns />
		public string GetStringForLegendary()
		{
			if (!IsLegendaryFish)
			{
				return "";
			}

			return Globals.GetTranslation("fish-tooltip-legendary");
		}

		/// <summary>
		/// Gets all the fish
		/// </summary>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<Item> Get(bool includeLegendaries = false)
		{
			return ItemList.Items.Values.Where(x =>
				x.IsFish &&
				x.DifficultyToObtain != ObtainingDifficulties.Impossible &&
				(includeLegendaries || (!includeLegendaries && !IsLegendary(x.Id)))
			).ToList();
		}

		/// <summary>
		/// Gets all the fish
		/// </summary>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<FishItem> GetListAsFishItem(bool includeLegendaries = false)
		{
			return Get(includeLegendaries).Cast<FishItem>().ToList();
		}

		/// <summary>
		/// Gets all the fish that can be caught during the given season
		/// </summary>
		/// <param name="season">The season</param>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<Item> Get(Seasons season, bool includeLegendaries = false)
		{
			List<FishItem> allFish = GetListAsFishItem(includeLegendaries);
			switch (season)
			{
				case Seasons.Spring:
					return allFish.Where(x => x.IsSpringFish).Cast<Item>().ToList();
				case Seasons.Summer:
					return allFish.Where(x => x.IsSummerFish).Cast<Item>().ToList();
				case Seasons.Fall:
					return allFish.Where(x => x.IsFallFish).Cast<Item>().ToList();
				case Seasons.Winter:
					return allFish.Where(x => x.IsWinterFish).Cast<Item>().ToList();
			}

			Globals.ConsoleError($"Tried to get fish belonging to the non-existent season: {season}");
			return new List<Item>();
		}

		/// <summary>
		/// Gets all the fish that can be caught during the given weather type
		/// </summary>
		/// <param name="weather">The weather type</param>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<Item> Get(Weather weather, bool includeLegendaries = false)
		{
			List<FishItem> allFish = GetListAsFishItem(includeLegendaries);
			switch (weather)
			{
				case Weather.Any:
					return allFish.Cast<Item>().ToList();
				case Weather.Sunny:
					return allFish.Where(x => x.IsSunFish).Cast<Item>().ToList();
				case Weather.Rainy:
					return allFish.Where(x => x.IsRainFish).Cast<Item>().ToList();
			}

			Globals.ConsoleError($"Tried to get fish belonging to the non-existent weather: {weather}");
			return new List<Item>();
		}

		/// <summary>
		/// Gets all the fish that can be caught at a given location
		/// </summary>
		/// <param name="location">The location</param>
		/// <param name="season">The season</param>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<Item> Get(Locations location, Seasons season, bool includeLegendaries = false)
		{
			List<FishItem> fishFromSeason = Get(season, includeLegendaries).Cast<FishItem>().ToList();
			return fishFromSeason.Where(x => x.AvailableLocations.Contains(location)).Cast<Item>().ToList();
		}

		/// <summary>
		/// Gets all the fish that can be caught at a given location
		/// </summary>
		/// <param name="location">The location</param>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<Item> Get(Locations location, bool includeLegendaries = false)
		{
			List<FishItem> allFish = GetListAsFishItem(includeLegendaries);
			return allFish.Where(x => x.AvailableLocations.Contains(location)).Cast<Item>().ToList();
		}

		/// <summary>
		/// Gets the fish that can be caught at 2am that can't be caught in the morning
		/// OR that have exclusions and can be caught at night
		/// </summary>
		/// <param name="startingTime">The time</param>
		/// <param name="includeLegendaries">Include the legendary fish</param>
		/// <returns />
		public static List<Item> GetNightFish(bool includeLegendaries = false)
		{
			var test = GetListAsFishItem(includeLegendaries);
			return GetListAsFishItem(includeLegendaries).Cast<FishItem>().Where(x =>
				(x.Times.MinValue >= 1200 && x.Times.MaxValue >= 2600) ||
				(x.ExcludedTimes.MaxValue >= 600 && x.ExcludedTimes.MaxValue < 2600)
			).Cast<Item>().ToList();
		}

		/// <summary>
		/// Gets all the legendary fish
		/// </summary>
		/// <returns />
		public static List<Item> GetLegendaries()
		{
			return ItemList.Items.Values.Where(x => x.IsFish && IsLegendary(x.Id)).ToList();
		}

		/// <summary>
		/// Returns the matching floor for the given normal fish id
		/// If it's not a normal mines fish, will return 20 and 60 (the normal ghostfish results)
		/// </summary>
		/// <returns></returns>
		public static string ComputeMineFloorString(string fishId = null)
		{
			if (fishId == null)
			{
				return "20, 60";
			}

			if (fishId == ItemList.GetQualifiedId(ObjectIndexes.Stonefish))
			{
				return "20";
			}

			if (fishId == ItemList.GetQualifiedId(ObjectIndexes.IcePip))
			{
				return "60";
			}

			if (fishId == ItemList.GetQualifiedId(ObjectIndexes.LavaEel))
			{
				return "100";
			}

			return "20, 60";
		}

        /// <summary>
        /// Returns the ToString representation to be used for the Fish asset
        /// </summary>
        /// <returns />
        public override string ToString()
        {
            string timeString;
            if (ExcludedTimes.MinValue < 600 || ExcludedTimes.MaxValue < 600)
            {
                timeString = $"{Times.MinValue} {Times.MaxValue}";
            }
            else
            {
                timeString = $"{Times.MinValue} {ExcludedTimes.MinValue} {ExcludedTimes.MaxValue} {Times.MaxValue}";
            }

            string seasonsString = "";
            foreach (Seasons season in AvailableSeasons)
            {
                seasonsString += $"{season.ToString().ToLower()} ";
            }
            seasonsString = seasonsString.Trim();

            string weatherString;
            if (Weathers.Count >= 2) { weatherString = "both"; }
            else { weatherString = Weathers[0].ToString().ToLower(); }

            string spawnMultiplierString = (SpawnMultiplier == 0) ? "0" : SpawnMultiplier.ToString().TrimStart(new char[] { '0' });
            string depthMultiplierString = (DepthMultiplier == 0) ? "0" : DepthMultiplier.ToString().TrimStart(new char[] { '0' });

            return $"{OriginalName}/{DartChance}/{BehaviorType.ToString().ToLower()}/{MinSize}/{MaxSize}/{timeString}/{seasonsString}/{weatherString}/{UnusedData}/{MinWaterDepth}/{spawnMultiplierString}/{depthMultiplierString}/{MinFishingLevel}/{IsValidTutorialFish}";
        }
    }
}
