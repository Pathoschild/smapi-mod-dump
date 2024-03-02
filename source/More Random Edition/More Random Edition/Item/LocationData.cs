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
        /// The default location data from Data/Locations.xnb
		/// It is keyed by the string version of the enum string of the Locations enum
        /// </summary>
        public readonly static Dictionary<string, string> DefaultLocationData =
            Globals.ModRef.Helper.GameContent.Load<Dictionary<string, string>>("Data/Locations");

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
			string foragableString;
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
				// Load the defaults
                string[] locData = DefaultLocationData[Location.ToString()].Split("/");
				string spring = locData[(int)LocationDataIndexes.SpringForage];
                string summer = locData[(int)LocationDataIndexes.SummerForage];
                string fall = locData[(int)LocationDataIndexes.FallForage];
                string winter = locData[(int)LocationDataIndexes.WinterForage];
                foragableString = $"{spring}/{summer}/{fall}/{winter}";
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
		private static string GetStringForSeason(List<ForagableData> foragableList)
		{
			if (foragableList.Count == 0) { return "-1"; }
			string output = "";

			foreach (ForagableData data in foragableList)
			{
				output += $"{data} ";
			}
			return output.Trim();
		}

		/// <summary>
		/// Gets the hard-coded string of location data for the current location name
		/// </summary>
		/// <returns />
		private string GetDefaultFishAndDiggingLocationString()
		{
            // Load the artifact data
            string[] locData = DefaultLocationData[Location.ToString()].Split("/");
            string artifactString = locData[(int)LocationDataIndexes.ArtifactData];
         
            if (ExtraDiggingItem != null)
			{
				string extraDiggingItemString = $"{ExtraDiggingItem.Id} {ExtraDiggingItemRarity}";
				string clayItemString = $"{(int)ObjectIndexes.Clay} 1";

				// Always place the clay item string at the end, since it's guaranteed to roll that one and
				// we will never see our custom item in that case
				artifactString = artifactString.EndsWith(clayItemString)
					? $"{artifactString[..^clayItemString.Length].Trim()} {extraDiggingItemString} {clayItemString}"
					: $"{artifactString} {clayItemString}";

            }

			return $"{GetFishLocationData()}/{artifactString}";
		}

		/// <summary>
		/// Gets the fish location data string for all the seasons
		/// </summary>
		/// <returns />
		private string GetFishLocationData()
		{
			Locations location = (Location == Locations.Backwoods) ? Locations.Mountain : Location;

			string[] locData = DefaultLocationData[location.ToString()].Split("/");

			string springDefault = locData[(int)LocationDataIndexes.SpringFish];
			string summerDefault = locData[(int)LocationDataIndexes.SummerFish];
			string fallDefault = locData[(int)LocationDataIndexes.FallFish];
			string winterDefault = locData[(int)LocationDataIndexes.WinterFish];

			string spring = GetFishLocationDataForSeason(Seasons.Spring, springDefault);
            string summer = GetFishLocationDataForSeason(Seasons.Summer, summerDefault);
            string fall = GetFishLocationDataForSeason(Seasons.Fall, fallDefault);
            string winter = GetFishLocationDataForSeason(Seasons.Winter, winterDefault);
            return $"{spring}/{summer}/{fall}/{winter}";
        }

		/// <summary>
		/// Gets the fish location data string for the given season
		/// </summary>
		/// <param name="season">The season</param>
		/// <returns />
		private string GetFishLocationDataForSeason(Seasons season, string defaultString)
		{
			if (!Globals.Config.Fish.Randomize) 
			{ 
				return defaultString; 
			}

			// This location thing is just how the game did it... probably don't need fish locations
			// in the backwoods, but doing this just to be safe
			Locations location = (Location == Locations.Backwoods) ? Locations.Mountain : Location;

			// Legendaries not included in FishItem.Get by default, so we're good there
			List<int> validFish = FishItem.Get(location, season)
				.Cast<FishItem>()
				.Where(fishItem =>
					// Don't include the mines fish here, as the are hard-coded
					!(location == Locations.UndergroundMine && fishItem.IsMinesFish)
				 )
				.Select(item => item.Id)
				.ToList();

			// Get all the junk items out of the string and patch in the new fish
			List<int> fishingList = ItemList.GetItemListFromString(defaultString)
				.Where(item => item is not FishItem)
				.Select(item => item.Id)
				.Concat(validFish)
				.ToList();

			// Build the output string from the list we just built
            return $"{string.Join(" -1 ", fishingList)} -1";
		}
	}
}
