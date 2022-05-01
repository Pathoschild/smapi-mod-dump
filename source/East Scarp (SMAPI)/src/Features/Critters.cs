/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarp
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace EastScarp
{
	public static class Critters
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void CheckSpawns (bool onEntry)
		{
			// World must be ready without an event active.
			if (!Context.IsWorldReady || Game1.eventUp)
				return;

			foreach (var spawn in Data.CritterSpawns)
				CheckSpawn (spawn, onEntry);
		}

		private static bool CheckSpawn (CritterSpawn spawn, bool onEntry)
		{
			// Must be in the right location.
			var location = Game1.player.currentLocation;
			if (location.Name != spawn.Location)
				return false;

			// Random roll must succeed.
			if (!(Game1.random.NextDouble () <
					(onEntry ? spawn.ChanceOnEntry : spawn.ChanceOnTick)))
				return false;

			// Conditions must hold.
			if (!spawn.Conditions.check ())
				return false;

			// Roll for number of clusters to spawn.
			int clusters = Game1.random.Next (spawn.MinClusters, spawn.MaxClusters + 1);
			if (clusters < 1)
				return false;

			// Spawn the clusters.
			for (int i = 0; i < clusters; ++i)
				SpawnCluster (spawn, location);
			return true;
		}

		private static void SpawnCluster (CritterSpawn spawn, GameLocation location)
		{
			// Randomly find a center position within the area.
			Rectangle area = spawn.adjustArea (location);
			int x = Game1.random.Next (area.Left, area.Right + 1);
			int y = Game1.random.Next (area.Top, area.Bottom + 1);
			Vector2 center = new (x, y);

			// Roll for number of critters to spawn.
			int count = Game1.random.Next (spawn.MinPerCluster, spawn.MaxPerCluster + 1);
			if (count < 1)
				return;

			// Spawn the critters.
			Monitor.Log ($"Spawning cluster of up to {count} critter(s) in location '{location.Name}' around ({x},{y}).",
				LogLevel.Trace);
			foreach (Vector2 tile in Utility.getPositionsInClusterAroundThisTile (center, count))
			{
				// Don't veer off the map.
				if (!location.isTileOnMap (tile))
					continue;

				// Tile must be clear, except allow water tiles with specified chance.
				bool onWater = location.doesTileHaveProperty ((int) tile.X, (int) tile.Y, "Water", "Back") != null;
				if (!location.isTileLocationTotallyClearAndPlaceable (tile))
				{
					if (!onWater || !(Game1.random.NextDouble () < spawn.ChanceOnWater))
						continue;
				}

				// Spawn the critter.
				Critter critter = spawn.Type switch
				{
					CritterType.BrownBird =>
						new Birdie ((int) tile.X, (int) tile.Y, Birdie.brownBird),
					CritterType.BlueBird =>
						new Birdie ((int) tile.X, (int) tile.Y, Birdie.blueBird),
					CritterType.SpecialBlueBird =>
						new Birdie ((int) tile.X, (int) tile.Y, 125),
					CritterType.SpecialRedBird =>
						new Birdie ((int) tile.X, (int) tile.Y, 135),
					CritterType.Butterfly =>
						new Butterfly (tile),
					CritterType.IslandButterfly =>
						new Butterfly (tile, true),
					CritterType.CalderaMonkey =>
						new CalderaMonkey (tile * 64f),
					CritterType.Cloud =>
						new Cloud (tile),
					CritterType.Crab =>
						new CrabCritter (tile * 64f),
					CritterType.Crow =>
						new Crow ((int) tile.X, (int) tile.Y),
					CritterType.Firefly =>
						new Firefly (tile),
					CritterType.Frog =>
						new Frog (tile, waterLeaper: onWater, forceFlip: Game1.random.NextDouble () < 0.5),
					CritterType.OverheadParrot =>
						new OverheadParrot (tile * 64f),
					CritterType.Owl =>
						new Owl (tile * 64f),
					CritterType.Rabbit =>
						new Rabbit (tile, flip: Game1.random.NextDouble () < 0.5),
					CritterType.Seagull =>
						new Seagull (tile * 64f + new Vector2 (32f, 32f), startingState: onWater ? 2 : 3),
					CritterType.Squirrel =>
						new Squirrel (tile, flip: Game1.random.NextDouble () < 0.5),
					_ => null,
				};
				if (critter != null)
					location.critters.Add (critter);
			}
		}
	}
}
