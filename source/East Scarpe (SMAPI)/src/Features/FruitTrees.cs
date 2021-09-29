/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Layers;

namespace EastScarp
{
	public static class FruitTrees
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		private static readonly string SpawnedFlag = "kdau.OrchardAnywhere/spawned";

		public static void DayUpdate ()
		{
			if (!Context.IsMainPlayer)
				return;

			foreach (var location in GetLocations ())
			{
				if (!location.modData.ContainsKey (SpawnedFlag))
					SpawnTrees (location);
				else
					UpdateTrees (location);
			}
		}

		public static void Reset ()
		{
			if (!Context.IsMainPlayer)
				throw new Exception ("Cannot reset fruit trees without an active save as host.");

			foreach (var location in GetLocations ())
			{
				ClearTrees (location);
				SpawnTrees (location);
			}
		}

		public static IList<GameLocation> GetLocations ()
		{
			return Data.FruitTreeLocations
			.Select ((locationName) =>
			{
				var location = Game1.getLocationFromName (locationName);
				if (location == null)
				{
					Monitor.Log ($"Fruit trees skipping unknown location '{locationName}'.",
						LogLevel.Warn);
					return null;
				}
				if (location.Map.GetLayer ("Back") == null)
				{
					Monitor.Log ($"Fruit trees skipping location '{locationName}' with no Back layer.",
						LogLevel.Warn);
					return null;
				}
				return location;
			})
			.Where ((location) => location != null)
			.ToList ();
		}

		private static void SpawnTrees (GameLocation location)
		{
			Monitor.Log ($"Spawning fruit trees in location '{location.Name}'...",
				LogLevel.Debug);
			Layer back = location.Map.GetLayer ("Back");
			for (int x = 0; x < back.LayerWidth; ++x)
			{
				for (int y = 0; y < back.LayerHeight; ++y)
				{
					string value = location.doesTileHaveProperty (x, y,
						"FruitTree", "Back");
					if (value == null || !int.TryParse (value, out int saplingIndex))
						continue;

					Vector2 tile = new (x, y);
					if (location.terrainFeatures.ContainsKey (tile))
						continue;

					FruitTree tree = new (saplingIndex, FruitTree.treeStage);
					tree.daysUntilMature.Value = 0;
					if (tree.fruitSeason.Value == Game1.GetSeasonForLocation (location))
						tree.fruitsOnTree.Value = 1;
					location.terrainFeatures.Add (tile, tree);
					Monitor.Log ($"...spawned fruit tree of type {tree.treeType.Value} at ({x},{y})",
						LogLevel.Trace);
				}
			}
			location.modDataForSerialization[SpawnedFlag] = "true";
		}

		private static void UpdateTrees (GameLocation location)
		{
			Monitor.Log ($"Updating fruit trees in location '{location.Name}'...",
				LogLevel.Debug);
			Layer back = location.Map.GetLayer ("Back");
			for (int x = 0; x < back.LayerWidth; ++x)
			{
				for (int y = 0; y < back.LayerHeight; ++y)
				{
					string value = location.doesTileHaveProperty (x, y,
						"FruitLimit", "Back");
					if (value == null || !int.TryParse (value, out int fruitLimit))
						continue;

					Vector2 tile = new (x, y);
					if (location.terrainFeatures.TryGetValue (tile, out TerrainFeature feature) &&
						feature is FruitTree tree &&
						tree.fruitsOnTree.Value > fruitLimit)
					{
						tree.fruitsOnTree.Value = fruitLimit;
						Monitor.Log ($"...limited fruit count to {fruitLimit} on tree of type {tree.treeType.Value} at ({x},{y})",
							LogLevel.Trace);
					}
				}
			}
		}

		private static void ClearTrees (GameLocation location)
		{
			Monitor.Log ($"Clearing fruit trees in location '{location.Name}'...",
				LogLevel.Debug);
			Layer back = location.Map.GetLayer ("Back");
			for (int x = 0; x < back.LayerWidth; ++x)
			{
				for (int y = 0; y < back.LayerHeight; ++y)
				{
					Vector2 tile = new (x, y);
					if (location.terrainFeatures.TryGetValue (tile, out TerrainFeature feature) &&
						feature is FruitTree tree)
					{
						location.terrainFeatures.Remove (tile);
						Monitor.Log ($"...cleared fruit tree of type {tree.treeType.Value} at ({x},{y})",
							LogLevel.Trace);
					}
				}
			}
			location.modDataForSerialization.Remove (SpawnedFlag);
		}
	}
}
