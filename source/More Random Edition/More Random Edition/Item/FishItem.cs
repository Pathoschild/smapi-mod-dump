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
		public List<Seasons> AvailableSeasons { get; set; } = new List<Seasons>();
		public List<Seasons> WoodsOnlySeasons { get; set; } = new List<Seasons>();
		public List<Weather> Weathers { get; set; } = new List<Weather>();
		public List<Locations> AvailableLocations { get; set; } = new List<Locations>();
		public Range Times { get; set; } = new Range(600, 2600); // That's anytime in the day
		public Range ExcludedTimes { get; set; } = new Range(0, 0);

		public bool IsNightFish { get { return Times.MaxValue >= 1800; } }
		public bool IsRainFish { get { return Weathers.Contains(Weather.Rainy); } }
		public bool IsSunFish { get { return Weathers.Contains(Weather.Sunny); } }

		public bool IsSpringFish { get { return AvailableSeasons.Contains(Seasons.Spring); } }
		public bool IsSummerFish { get { return AvailableSeasons.Contains(Seasons.Summer); } }
		public bool IsFallFish { get { return AvailableSeasons.Contains(Seasons.Fall); } }
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

		/// <summary>
		/// The description - used in the tooltip
		/// </summary>
		public string Description
		{
			get
			{
				string timesString = GetTimesString();
				string seasonsString = GetStringForSeasons();
				string locationString = GetStringForLocations();
				string weatherString = GetStringForWeather();
				return $"{timesString} {seasonsString} {locationString} {weatherString}";
			}
		}

		/// <summary>
		/// This is at the end of the object information - unsure if it's actually used anywhere
		/// and also unsure what the requirements for the "Day/Night" strings are
		/// </summary>
		public string ObjectInformationSuffix
		{
			get
			{
				string dayString = Times.MinValue < 500 ? "Day" : "";
				string nightString = Times.MinValue > 500 || Times.MaxValue > 900 ? "Night" : "";
				string dayListString = string.Join(" ", new List<string> { dayString, nightString });

				string seasonString = string.Join(" ", AvailableSeasons.Select(x => x.ToString()));

				return $"{dayListString}^{seasonString}";
			}
		}

		public FishItem(int id, ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.LargeTimeRequirements) : base(id)
		{
			DifficultyToObtain = difficultyToObtain;
			IsFish = true;

			if (id != (int)ObjectIndexes.AnyFish)
			{
				FishData.FillDefaultFishInfo(this);
			}
		}

		public FishItem(int id, bool boop) : base(id)
		{
		}

		/// <summary>
		/// Gets the string to be used for the time part of the description
		/// </summary>
		/// <returns />
		private string GetTimesString()
		{
			string timesString = "";
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
			if (AvailableLocations.Count == 0) { return ""; }
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
			if (AvailableSeasons.Count == 0) { return ""; }
			if (AvailableSeasons.Count == 4)
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
			List<string> output = new List<string>();
			foreach (Locations location in AvailableLocations)
			{
				output.Add(Globals.GetTranslation($"fish-{location.ToString().ToLower()}-location"));
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
		/// Returns the ToString representation to be used for the Fish asset
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			if (Id == -4) { return ""; }

			string timeString = "";
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

			string weatherString = "";
			if (Weathers.Count >= 2) { weatherString = "both"; }
			else { weatherString = Weathers[0].ToString().ToLower(); }

			string spawnMultiplierString = (SpawnMultiplier == 0) ? "0" : SpawnMultiplier.ToString().TrimStart(new char[] { '0' });
			string depthMultiplierString = (DepthMultiplier == 0) ? "0" : DepthMultiplier.ToString().TrimStart(new char[] { '0' });

			return $"{Name}/{DartChance}/{BehaviorType.ToString().ToLower()}/{MinSize}/{MaxSize}/{timeString}/{seasonsString}/{weatherString}/{UnusedData}/{MinWaterDepth}/{spawnMultiplierString}/{depthMultiplierString}/{MinFishingLevel}";
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
				x.Id != (int)ObjectIndexes.AnyFish &&
				x.DifficultyToObtain != ObtainingDifficulties.Impossible &&
				(includeLegendaries || (!includeLegendaries && x.DifficultyToObtain < ObtainingDifficulties.EndgameItem))
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

			Globals.ConsoleError($"Tried to get fish belonging to the non-existent season: {season.ToString()}");
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

			Globals.ConsoleError($"Tried to get fish belonging to the non-existent weather: {weather.ToString()}");
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
		/// Gets all the fish that can be caught at a given location and season
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
			return ItemList.Items.Values.Where(x =>
				x.IsFish &&
				x.DifficultyToObtain == ObtainingDifficulties.EndgameItem
			).ToList();
		}
	}
}
