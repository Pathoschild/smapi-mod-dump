/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rupak0577/FishDex
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishDex
{
	/// <summary>Parses the raw game data into usable models. These may be expensive operations and should be cached.</summary>
	internal class DataParser
	{
		private List<FishInfo> Fishes = new List<FishInfo>();

		public DataParser()
		{
			ParseFishData();
		}

		/// <summary>Read parsed data about the fishes.</summary>
		public void ParseFishData()
		{
			IDictionary<int, string> internalFishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
			IDictionary<string, string> internalLocData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

			foreach (var fish in internalFishData)
			{
				// parse fish info

				// fields info - https://stardewvalleywiki.com/Modding:Fish_data
				string[] fields = fish.Value.Split('/');
				if (fields[1] == "trap" || fields[0] == "Seaweed" || fields[0] == "Green Algae" || fields[0] == "White Algae")
					continue;

				string tod = parseTimeField(fields[5]);

				// find spawn locations and seasons
				Tuple<String, String> parsedTuple = parseLocationsAndSeasons(fish, internalLocData);

				// add fish info object
				Fishes.Add(new FishInfo(fish.Key, fields[0], false, tod, parsedTuple.Item1, parsedTuple.Item2, 
					(fish.Key == 775) ? "both" : fields[7]));
			}
		}

		private Tuple<String, String> parseLocationsAndSeasons(KeyValuePair<int, string> fish, IDictionary<string, string> internalLocData)
		{
			StringBuilder locations = new StringBuilder();
			StringBuilder seasons = new StringBuilder();

			string fishId = fish.Key.ToString();
			Boolean foundInSpring = false;
			Boolean foundInSummer = false;
			Boolean foundInFall = false;
			Boolean foundInWinter = false;
			foreach (var location in internalLocData)
			{
				if (location.Key == "Farm" || location.Key == "fishingGame" || location.Key == "Temp")
					continue;

				// Locations
				string[] values = location.Value.Split('/');
				if (values[4].Contains(fishId) || values[5].Contains(fishId) || values[6].Contains(fishId) ||
					values[7].Contains(fishId))
				{
					locations.Append(location.Key);
					locations.Append(" | ");
				}

				// Seasons
				// Index 4 == Spring, Index 5 == Summer, Index 6 == Fall, Index 7 == Winter
				if (values[4].Contains(fishId))
					foundInSpring = true;
				if (values[5].Contains(fishId))
					foundInSummer = true;
				if (values[6].Contains(fishId))
					foundInFall = true;
				if (values[7].Contains(fishId))
					foundInWinter = true;
			}

			// Fix missing or incorrect location data for some fishes
			if (fish.Key == 158) // stonefish
				locations.Append("Mines, Level 20 | ");
			else if (fish.Key == 161) // ice pip
				locations.Append("Mines, Level 60 | ");
			else if (fish.Key == 162) // lava eel
				locations.Append("Mines, Level 100 | ");
			else if (fish.Key == 798 || fish.Key == 799 || fish.Key == 800 || fish.Key == 149 || 
				fish.Key == 154 || fish.Key == 155) // midnight squid, spookfish, blobfish, octopus, sea and super cucumber
				locations.Append("Submarine at Night Market | ");

			if (locations.Length == 0 || fish.Key == 163) // location field still empty => legendary fishes
			{
				// Location data from stardewvalleywiki.com
				switch (fish.Key)
				{
					case 159: // Crimsonfish
						locations.Append("East Pier on The Beach. Deep water.");
						break;
					case 160: // Angler
						locations.Append("North of JojaMart on the wooden plank bridge.");
						break;
					case 163: // Legend
						locations.Clear(); // The game data contains the wrong location info for the Legend
						locations.Append("The Mountain Lake near the log.");
						break;
					case 775: // Glacierfish
						locations.Append("South end of Arrowhead Island in Cindersap Forest. Deep water.");
						break;
					case 682: // Mutant Carp
						locations.Append("Sewers");
						break;
				}
			}
			else
				locations.Length -= 3; // exclude the last " | " separator

			// Determine seasons for the fish
			if ((foundInSpring && foundInSummer && foundInFall && foundInWinter) || fish.Key == 682 ||
				fish.Key == 158 || fish.Key == 161 || fish.Key == 162)
			{
				seasons.Append("all");
			}
			else
			{
				if (foundInSpring || fish.Key == 163)
					seasons.Append("spring").Append(" | ");
				if (foundInSummer || fish.Key == 159)
					seasons.Append("summer").Append(" | ");
				if (foundInFall || fish.Key == 160)
					seasons.Append("fall").Append(" | ");
				if (foundInWinter || fish.Key == 775 || fish.Key == 798 || fish.Key == 799 || fish.Key == 800)
					seasons.Append("winter").Append(" | ");

				// Add season data for some fishes available in the Night Market in winter
				if (fish.Key == 149 || fish.Key == 155) // octopus, super cucumber (sea cucumber's data does not need correction)
					seasons.Append("winter").Append(" | ");

				if (seasons.Length != 0)
					seasons.Length -= 3; // exclude the last " | " separator
			}

			return new Tuple<string, string>(locations.ToString(), seasons.ToString());
		}

		private string parseTimeField(string timeField)
		{
			char[] timeFieldChars = timeField.ToCharArray();
			int firstBlankIndex = timeField.IndexOf(' ');
			int lastBlankIndex = timeField.LastIndexOf(' ');
			if (firstBlankIndex == lastBlankIndex)
				return new string(timeFieldChars).Replace(" ", "-");
			else
			{
				timeFieldChars[firstBlankIndex] = '-';
				timeFieldChars[lastBlankIndex] = '-';
				return new string(timeFieldChars).Replace(" ", " | ");
			}
		}

		public IEnumerable<FishInfo> GetFishData()
		{
			return Fishes;
		}

		/// <summary>Set caught data for the fishes. To be called only after the world has been loaded.</summary>
		public void SetCaughtData()
		{
			foreach (var fish in Fishes)
			{
				if (Game1.player.fishCaught.ContainsKey(fish.Id))
				{
					fish.Caught = true;
				}
				else
				{
					fish.Caught = false;
				}
			}
		}

		/// <summary>A fish entry parsed from the game's data files.</summary>
		internal class FishInfo
		{
			private string[] KEYS = { "Time of Day", "Locations", "Season", "Weather" };

			/*********
			** Accessors
			*********/
			/// <summary>The fish id.</summary>
			public int Id { get; }

			/// <summary>The fish name.</summary>
			public string Name { get; }

			/// <summary>Whether the fish has been caught before.</summary>
			public Boolean Caught { get; set; }

			/// <summary>The fish data.</summary>
			public IDictionary<string, string> Data { get; }

			/*********
			** Public methods
			*********/
			/// <summary>Construct an instance.</summary>
			/// <param name="id">The fish's internal id.</param>
			/// <param name="name">The fish name.</param>
			/// <param name="caught">Whether the fish has been caught before.</param>
			/// <param name="tod">The time of day when the fish spawns.</param>
			/// <param name="locations">The locations where the fish spawns.</param>
			/// <param name="season">The season when the fish spawns.</param>
			/// <param name="weather">The weather when the fish spawns.</param>
			public FishInfo(int id, string name, Boolean caught, string tod, string locations, string season, string weather)
			{
				this.Id = id;
				this.Name = name;
				this.Caught = caught;
				this.Data = new Dictionary<string, string>
				{
					{ KEYS[0], tod },
					{ KEYS[1], locations },
					{ KEYS[2], season },
					{ KEYS[3], weather }
				};
			}

			public String GetTod()
			{
				return this.Data[KEYS[0]];
			}

			public String GetLocation()
			{
				return this.Data[KEYS[1]];
			}

			public String GetSeason()
			{
				return this.Data[KEYS[2]];
			}

			public String GetWeather()
			{
				return this.Data[KEYS[3]];
			}
		}
	}
}
