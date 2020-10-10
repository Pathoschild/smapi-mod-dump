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
using System.Linq;
using xTile.Layers;

namespace EastScarpe
{
	public static class ESOrchard
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.Data;

		private static readonly string TreesSpawnedFlag =
			"LemurKat.EastScarpe.OrchardTreesSpawned";

		public static GameLocation location =>
			Game1.getLocationFromName ("ESOrchard");

		public static void DayUpdate ()
		{
			if (!Context.IsMainPlayer || location == null)
				return;

			if (!Game1.player.mailReceived.Contains (TreesSpawnedFlag))
				SpawnTrees ();
			else
				UpdateTrees ();
		}

		public static void Reset ()
		{
			if (!Context.IsMainPlayer)
				throw new Exception ("Cannot reset the East Scarpe orchard without an active save as host.");
			if (location == null)
				throw new Exception ("Cannot locate the East Scarpe orchard.");

			ClearTrees ();
			SpawnTrees ();
		}

		private static void SpawnTrees ()
		{
			GameLocation loc = location;
			Layer back = loc.Map.GetLayer ("Back");
			if (back == null)
				throw new Exception ("Back layer missing from orchard map.");

			Monitor.Log ("Spawning fruit trees in orchard", LogLevel.Debug);
			for (int x = 0; x < back.LayerWidth; ++x)
			{
				for (int y = 0; y < back.LayerHeight; ++y)
				{
					string value = loc.doesTileHaveProperty (x, y,
						"FruitTree", "Back");
					if (value == null || !Int32.TryParse (value,
							out int saplingIndex))
						continue;

					FruitTree tree = new FruitTree (saplingIndex,
						FruitTree.treeStage);
					tree.daysUntilMature.Value = 0;
					if (tree.fruitSeason.Value == Game1.currentSeason)
						tree.fruitsOnTree.Value = 1;
					loc.terrainFeatures.Add (new Vector2 (x, y), tree);
					Monitor.Log ($"Spawned fruit tree type {tree.treeType.Value} at ({x},{y})",
						LogLevel.Trace);
				}
			}
			Game1.player.mailReceived.Add (TreesSpawnedFlag);
		}

		private static void UpdateTrees ()
		{
			Monitor.Log ("Updating fruit trees in orchard", LogLevel.Trace);
			foreach (FruitTree tree in location.terrainFeatures.Values.OfType<FruitTree> ())
			{
				if (tree.fruitsOnTree.Value > 1)
					tree.fruitsOnTree.Value = 1;
			}
		}

		private static void ClearTrees ()
		{
			GameLocation loc = location;
			foreach (var kvp in loc.terrainFeatures.Pairs.ToArray ())
			{
				if (kvp.Value is FruitTree)
					loc.terrainFeatures.Remove (kvp.Key);
			}
			Game1.player.mailReceived.Remove (TreesSpawnedFlag);
		}
	}
}
