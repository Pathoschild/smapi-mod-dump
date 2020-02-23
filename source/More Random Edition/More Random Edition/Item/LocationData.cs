using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Contains information about foragable items and fish per season, as well as diggable items
	/// </summary>
	public class LocationData
	{
		public Locations Location { get; set; }
		public string LocationName { get { return Location.ToString(); } }
		public List<ForagableData> SpringForagables { get; } = new List<ForagableData>();
		public List<ForagableData> SummerForagables { get; } = new List<ForagableData>();
		public List<ForagableData> FallForagables { get; } = new List<ForagableData>();
		public List<ForagableData> WinterForagables { get; } = new List<ForagableData>();

		public Item ExtraDiggingItem { get; set; }
		public double ExtraDiggingItemRarity { get; set; }

		/// <summary>
		/// Returns whether there's any foragable location data
		/// </summary>
		/// <returns />
		public bool HasData()
		{
			return SpringForagables.Count + SummerForagables.Count + FallForagables.Count + WinterForagables.Count > 0;
		}

		/// <summary>
		/// Gets the string for this set of location data
		/// </summary>
		/// <returns>
		/// A string in the following format:
		/// springForagables/summerForagables/fallForagables/winterForagables/springFishing/summerFishing/fallFishing/winterFishing/dirtFindings
		/// </returns>
		public override string ToString()
		{
			string foragableString = "";
			if (Globals.Config.RandomizeForagables)
			{
				string springForagables = GetStringForSeason(SpringForagables);
				string summerForagables = GetStringForSeason(SummerForagables);
				string fallForagables = GetStringForSeason(FallForagables);
				string winterForagables = GetStringForSeason(WinterForagables);
				foragableString = $"{springForagables}/{summerForagables}/{fallForagables}/{winterForagables}";
			}

			else
			{
				// These are the default values
				switch (Location)
				{
					case Locations.Desert:
						foragableString = "88 .5 90 .5/88 .5 90 .5/88 .5 90 .5/88 .5 90 .5";
						break;
					case Locations.BusStop:
						foragableString = "18 .9 20 .4 22 .7/396 .4 398 .4 402 .7/406 .6 408 .4/414 .33 418 .6 283 .5";
						break;
					case Locations.Forest:
						foragableString = "16 .9 22 .9/396 .6 402 .9/404 .9 410 .9/418 .9 414 .9 283 .5";
						break;
					case Locations.Town:
						foragableString = "18 .9/402 .9/410 .6/418 .7 414 .1 283 .5";
						break;
					case Locations.Mountain:
						foragableString = "20 .7 16 .5/396 .5 398 .8/404 .4 406 .4 408 .9/414 .85 418 .9 283 .5";
						break;
					case Locations.Backwoods:
						foragableString = "20 .7 16 .5/396 .5 398 .8/404 .4 406 .4 408 .9/414 .25 418 .4 283 .5";
						break;
					case Locations.Railroad:
						foragableString = "18 .9 20 .4 22 .7/396 .4 398 .4 402 .7/406 .6 408 .4 410 .6/414 .8 418 .8";
						break;
					case Locations.Beach:
						foragableString = "372 .9 718 .1 719 .3 723 .3/372 .9 394 .5 718 .1 719 .3 723 .3/372 .9 718 .1 719 .3 723 .3/372 .4 392 .8 718 .05 719 .2 723 .2";
						break;
					case Locations.Woods:
						foragableString = "257 .5 404 .25 16 .8/259 .9 420 .25/281 .5 404 .6 420 .2/283 .9";
						break;
					default:
						foragableString = "-1/-1/-1/-1";
						break;
				}
			}


			return $"{foragableString}/{GetDefaultFishAndDiggingLocationString()}";
		}

		/// <summary>
		/// Gets the string for the given season of data
		/// </summary>
		/// <param name="data">The season of data</param>
		/// <returns>
		/// -1 if there's no data; the string in the following format otherwise:
		///   {itemId} {itemRarity} ...
		/// </returns>
		private string GetStringForSeason(List<ForagableData> foragableList)
		{
			if (foragableList.Count == 0) { return "-1"; }
			string output = "";

			foreach (ForagableData data in foragableList)
			{
				output += $"{data.ToString()} ";
			}
			return output.Trim();
		}

		/// <summary>
		/// Gets the hard-coded string of location data for the current location name
		/// </summary>
		/// <returns />
		private string GetDefaultFishAndDiggingLocationString()
		{
			string locationString;
			switch (Location)
			{
				case Locations.Desert:
					locationString = $"{GetFishLocationData()}/390 .25";
					break;
				case Locations.BusStop:
					locationString = $"{GetFishLocationData()}/584 .08 378 .15 102 .15 390 .25";
					break;
				case Locations.Forest:
					locationString = $"{GetFishLocationData()}/378 .08 579 .1 588 .1 102 .15 390 .25";
					break;
				case Locations.Town:
					locationString = $"{GetFishLocationData()}/378 .2 110 .2 583 .1 102 .2 390 .25";
					break;
				case Locations.Mountain:
					locationString = $"{GetFishLocationData()}/382 .06 581 .1 378 .1 102 .15 390 .25";
					break;
				case Locations.Backwoods:
					locationString = $"{GetFishLocationData()}/382 .06 582 .1 378 .1 102 .15 390 .25";
					break;
				case Locations.Railroad:
					locationString = $"{GetFishLocationData()}/580 .1 378 .15 102 .19 390 .25";
					break;
				case Locations.Beach:
					locationString = $"{GetFishLocationData()}/384 .08 589 .09 102 .15 390 .25";
					break;
				case Locations.Woods:
					locationString = $"{GetFishLocationData()}/390 .25";
					break;
				case Locations.UndergroundMine:
					locationString = $"{GetFishLocationData()}/107 .01";
					break;
				default:
					Globals.ConsoleError($"No location data found for {LocationName}!");
					return "-1/-1/-1/-1/-1";
			}

			if (ExtraDiggingItem != null)
			{
				locationString += $" {ExtraDiggingItem.Id} {ExtraDiggingItemRarity}";
			}

			// Put the clay at the end, since having a probability of 1 will block the possibly of getting 
			// an item listed off of it
			if (Location != Locations.UndergroundMine)
			{
				locationString += $" {(int)ObjectIndexes.Clay} 1";
			}

			return locationString;
		}

		/// <summary>
		/// Gets the fish location data string for all the seasons
		/// Note that the original behaviors for carps and catfish were changed here for ease
		/// of ensuring that all locations have all the fish they need
		/// </summary>
		/// <returns />
		private string GetFishLocationData()
		{
			Locations location = (Location == Locations.Backwoods) ? Locations.Mountain : Location;

			switch (location)
			{
				case Locations.Desert:
					string desertDefaultString = "153 -1 164 -1 165 -1";
					string desertSpringData = GetFishLocationDataForSeason(Seasons.Spring, desertDefaultString);
					string desertSummerData = GetFishLocationDataForSeason(Seasons.Summer, desertDefaultString);
					string desertFallData = GetFishLocationDataForSeason(Seasons.Fall, desertDefaultString);
					string desertWinterData = GetFishLocationDataForSeason(Seasons.Winter, desertDefaultString);
					return $"{desertSpringData}/{desertSummerData}/{desertFallData}/{desertWinterData}";
				case Locations.BusStop:
					return "-1/-1/-1/-1";
				case Locations.Forest:
					string forestSpringData = GetFishLocationDataForSeason(Seasons.Spring, "153 -1 145 0 143 0 137 1 132 0 706 0 702 0");
					string forestSummerData = GetFishLocationDataForSeason(Seasons.Summer, "153 -1 145 0 144 -1 138 0 132 0 706 0 704 0 702 0");
					string forestFallData = GetFishLocationDataForSeason(Seasons.Fall, "143 0 153 -1 140 -1 139 0 137 1 132 0 706 0 702 0 699 0");
					string forestWinterData = GetFishLocationDataForSeason(Seasons.Winter, "699 0 143 0 153 -1 144 -1 141 -1 140 -1 132 0 707 0 702 0");
					return $"{forestSpringData}/{forestSummerData}/{forestFallData}/{forestWinterData}";
				case Locations.Town:
					string townSpringData = GetFishLocationDataForSeason(Seasons.Spring, "137 -1 132 -1 143 -1 145 -1 153 -1 706 -1");
					string townSummerData = GetFishLocationDataForSeason(Seasons.Summer, "138 -1 132 -1 144 -1 145 -1 153 -1 706 -1");
					string townFallData = GetFishLocationDataForSeason(Seasons.Fall, "139 -1 137 -1 132 -1 140 -1 143 -1 153 -1 706 -1 699 -1");
					string townWinterData = GetFishLocationDataForSeason(Seasons.Winter, "132 -1 140 -1 141 -1 143 -1 144 -1 153 -1 707 -1 699 -1");
					return $"{townSpringData}/{townSummerData}/{townFallData}/{townWinterData}";
				case Locations.Mountain:
					string mountainSpringData = GetFishLocationDataForSeason(Seasons.Spring, "136 -1 142 -1 153 -1 702 -1 700 -1 163 -1");
					string mountainSummerData = GetFishLocationDataForSeason(Seasons.Summer, "136 -1 142 -1 153 -1 138 -1 702 -1 700 -1 698 -1");
					string mountainFallData = GetFishLocationDataForSeason(Seasons.Fall, "136 -1 140 -1 142 -1 153 -1 702 -1 700 -1");
					string mountainWinterData = GetFishLocationDataForSeason(Seasons.Winter, "136 -1 140 -1 141 -1 153 -1 707 -1 702 -1 700 -1 698 -1");
					return $"{mountainSpringData}/{mountainSummerData}/{mountainFallData}/{mountainWinterData}";
				case Locations.Railroad:
					return $"-1/-1/-1/-1";
				case Locations.Beach:
					string beachSpringData = GetFishLocationDataForSeason(Seasons.Spring, "129 -1 131 -1 147 -1 148 -1 152 -1 708 -1");
					string beachSummerData = GetFishLocationDataForSeason(Seasons.Summer, "128 -1 130 -1 146 -1 149 -1 150 -1 152 -1 155 -1 708 -1 701 -1");
					string beachFallData = GetFishLocationDataForSeason(Seasons.Fall, "129 -1 131 -1 148 -1 150 -1 152 -1 154 -1 155 -1 705 -1 701 -1");
					string beachWinterData = GetFishLocationDataForSeason(Seasons.Winter, "708 -1 130 -1 131 -1 146 -1 147 -1 150 -1 151 -1 152 -1 154 -1 705 -1");
					return $"{beachSpringData}/{beachSummerData}/{beachFallData}/{beachWinterData}";
				case Locations.Woods:
					string woodsDefaultString = "734 -1 142 -1 143 -1";
					string woodsSpringData = GetFishLocationDataForSeason(Seasons.Spring, woodsDefaultString);
					string woodsSummerData = GetFishLocationDataForSeason(Seasons.Summer, "734 -1 142 -1 157 -1");
					string woodsFallData = GetFishLocationDataForSeason(Seasons.Fall, woodsDefaultString);
					string woodsWinterData = GetFishLocationDataForSeason(Seasons.Winter, "734 -1 157 -1 143 -1");
					return $"{woodsSpringData}/{woodsSummerData}/{woodsFallData}/{woodsWinterData}";
				case Locations.UndergroundMine:
					string fishString = "153 -1 157 -1 ";
					List<int> undergroundFish = FishItem.Get(Locations.UndergroundMine)
						.Where(x => !new int[] { 158, 161, 162 }.Contains(x.Id)) // The three mines fish that are not ghost fish
						.Select(x => x.Id)
						.ToList();
					fishString += $"{string.Join(" -1 ", undergroundFish)} -1";
					return $"{fishString}/{fishString}/{fishString}/{fishString}";
				default:
					Globals.ConsoleError($"No location data found for {LocationName}!");
					return "-1/-1/-1/-1/-1";
			}
		}

		/// <summary>
		/// Gets the fish location data string for the given season
		/// </summary>
		/// <param name="season">The season</param>
		/// <returns />
		private string GetFishLocationDataForSeason(Seasons season, string defaultString)
		{
			// This location thing is just how the game did it... probably don't need fish locations
			// in the backwoods, but doing this just to be safe
			Locations location = (Location == Locations.Backwoods) ? Locations.Mountain : Location;

			if (!Globals.Config.RandomizeFish) { return defaultString; }

			List<int> allFishIds = FishItem.Get().Select(x => x.Id).ToList();
			List<int> fishIds = FishItem.Get(location, season).Select(x => x.Id).ToList();

			string[] stringParts = defaultString.Split(' ');
			int fishIdIndex = 0;
			for (int i = 0; i < stringParts.Length; i += 2)
			{
				// Skips over algae, etc.
				if (allFishIds.Contains(int.Parse(stringParts[i])))
				{
					stringParts[i] = fishIds[fishIdIndex].ToString();
					fishIdIndex++;
				}
			}
			if (fishIdIndex != fishIds.Count)
			{
				Globals.ConsoleError($"Didn't assign all the fish to {Location.ToString()} in the {season.ToString()}! Assigned {fishIdIndex} out of {fishIds.Count}.");
			}

			return string.Join(" ", stringParts);
		}
	}
}
