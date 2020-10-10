/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
	class Spawns
	{
		private static List<TileLocation> spawnLocations;

		public static void LoadSpawnLocations(IModHelper helper)
		{
			var data = helper.Data.ReadJsonFile<SpawnLocationsDataModel>("spawns.json");
			if (data == null)
			{
				data = new SpawnLocationsDataModel();
				helper.Data.WriteJsonFile("spawns.json", data);
			}
			spawnLocations = data.Spawns;

			foreach (var spawn in spawnLocations)
			{
				Console.WriteLine($"Loaded spawn ({spawn.locationName}, {spawn.tileX}, {spawn.tileY})");
			}
		}

		public Dictionary<Farmer, TileLocation> ScatterPlayers(IEnumerable<Farmer> players)
		{
			var random = new Random();
			var viableSpawns = spawnLocations.ToList().OrderBy(x => random.NextDouble());

			var enumerator = players.GetEnumerator();

			var chosenSpawns = new Dictionary<Farmer, TileLocation>();

			foreach (TileLocation spawnLocation in viableSpawns)
			{
				if (enumerator.MoveNext())
					chosenSpawns.Add(enumerator.Current, spawnLocation);
				else
				{
					Console.WriteLine("Successfully warped each player to a unique spawn");
					return chosenSpawns;
				}
			}

			//There are more players than spawn locations. We will have to spawn players on top of eachother
			Console.WriteLine("There are more players than spawn locations");
			while (enumerator.MoveNext())
				chosenSpawns.Add(enumerator.Current, spawnLocations[random.Next(spawnLocations.Count)]);

			return chosenSpawns;
		}
	}

	class SpawnLocationsDataModel
	{
		public List<TileLocation> Spawns { get; set; } = new List<TileLocation>()
		{
			new TileLocation("Town", 16, 14),
			new TileLocation("Town", 63, 17),
			new TileLocation("Town", 60, 65),
			new TileLocation("Town", 46, 92),
			new TileLocation("Town", 92, 85),
			new TileLocation("ArchaeologyHouse", 11, 11),
			new TileLocation("SamHouse", 4, 18),
			new TileLocation("Beach", 29, 35),
			new TileLocation("Mountain", 32, 9),
			new TileLocation("Mountain", 56, 32),
			new TileLocation("Mountain", 64, 10),
			new TileLocation("Railroad", 7, 43),
			new TileLocation("Railroad", 62, 44),
			new TileLocation("Backwoods", 20, 10),
			new TileLocation("FarmCave", 6, 7),
			new TileLocation("Forest", 79, 16),
			new TileLocation("Forest", 5, 29),
			new TileLocation("Forest", 8, 80),
			new TileLocation("Forest", 41, 75),
			new TileLocation("Forest", 99, 92),
			new TileLocation("Forest", 84, 50),
			new TileLocation("Farm", 28, 16),
			new TileLocation("Desert", 37, 11),
			new TileLocation("Desert", 2, 51),
			new TileLocation("Backwoods", 39, 30),
			new TileLocation("BusStop", 32, 6),
			new TileLocation("Hospital", 4, 5),
			new TileLocation("SeedShop", 20, 13),
			new TileLocation("JoshHouse", 3, 19),
			new TileLocation("CommunityCenter", 33, 13),
			new TileLocation("Mine", 13, 10),
			new TileLocation("Sewer", 16, 21),
			new TileLocation("WizardHouse", 7, 17),
			new TileLocation("Farm", 73, 14),
			new TileLocation("JojaMart", 14, 7),
			new TileLocation("SebastianRoom", 6, 7)
		};
	}
}
