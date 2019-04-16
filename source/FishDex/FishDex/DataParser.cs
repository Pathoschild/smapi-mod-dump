using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishDex
{
	/// <summary>Parses the raw game data into usable models. These may be expensive operations and should be cached.</summary>
	internal class DataParser
	{
		private static IDictionary<int, string> internalFishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
		private static IDictionary<string, string> internalLocData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

		private List<FishInfo> Fishes;

		public DataParser()
		{
			Fishes = new List<FishInfo>();
			ParseFishData();
		}

		/// <summary>Read parsed data about the fishes.</summary>
		public void ParseFishData()
		{
			foreach (var fish in internalFishData)
			{
				// parse fish info
				string[] fields = fish.Value.Split('/');
				if (fields[1] == "trap" || fields[0] == "Seaweed" || fields[0] == "Green Algae" || fields[0] == "White Algae")
					continue;

				char[] temp = fields[5].ToCharArray();
				temp[fields[5].IndexOf(' ')] = '-';
				temp[fields[5].LastIndexOf(' ')] = '-';
				string tod = new string(temp).Replace(" ", " | ");

				// find spawn locations
				StringBuilder locations = new StringBuilder();
				string fishId = fish.Key.ToString();
				foreach (var location in internalLocData)
				{
					if (location.Key == "Farm" || location.Key == "fishingGame" || location.Key == "Temp")
						continue;

					string[] values = location.Value.Split('/');
					if (values[4].Contains(fishId) || values[5].Contains(fishId) || values[6].Contains(fishId) ||
						values[7].Contains(fishId))
					{
						locations.Append(location.Key);
						locations.Append(" | ");
					}
				}
				if (locations.Length == 0)
					locations.Append("-");
				else
					locations.Length -= 3;

				// add fish info object
				Fishes.Add(new FishInfo(fish.Key, fields[0], false, tod, locations.ToString(), fields[6].Replace(" ", " | "), 
					fields[7] == "both" ? "sunny | rainy" : fields[7].Replace(" ", " | ")));
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
			public static string[] LABELS = { "Time of Day", "Locations", "Season", "Weather" };

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
					{ LABELS[0], tod },
					{ LABELS[1], locations },
					{ LABELS[2], season },
					{ LABELS[3], weather }
				};
			}
		}
	}
}
