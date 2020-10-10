/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		public static readonly int Radius = 7;

		public static void DetonateAll ()
		{
			foreach (GameLocation location in Game1.locations)
			{
				foreach (var kvp in location.objects.Pairs.ToArray ())
				{
					if (kvp.Value is FlowerBomb bomb)
						bomb.detonate (location, kvp.Key);
				}
			}
		}

		public void detonate (GameLocation location, Vector2 center)
		{
			Monitor.Log ($"Flower Bomb with {heldObject.Value?.Name ?? "no seeds"} detonating at ({center.X},{center.Y}) on {location.Name}",
				LogLevel.Debug);
			location.removeObject (center, true);

			Random mainRandom = RandomForTile (center);

			Queue<Vector2> tiles = new Queue<Vector2> (
				FindTargetTiles (location, center).OrderBy ((tile) =>
					Vector2.Distance (center, tile) +
					1.5 * mainRandom.NextDouble ()));

			int plantings = RollForCount (tiles.Count, mainRandom,
				0.5, 0.2, 0.1, 0.1);

			if (heldObject.Value != null)
			{
				Crop testCrop = new Crop (heldObject.Value.ParentSheetIndex, 0, 0);
				if (testCrop.seasonsToGrowIn.Contains (Game1.currentSeason))
				{
					// Place a medium quantity of the specific flower, if any.
					if (testCrop.whichForageCrop.Value == 0)
					{
						int specifics = RollForCount (plantings, mainRandom,
							0.3, 0.2, 0.2);
						plantings -= PlaceObjects (location, tiles, specifics,
							(_tile) => testCrop.indexOfHarvest.Value,
							isForage: false,
							programColorCrop: testCrop.programColored.Value
								? heldObject.Value.ParentSheetIndex : -1);
					}

					// Fill most of the remainder with seasonal wildflowers.
					int wildflowers = RollForCount (plantings, mainRandom,
						0.8, 0.1, 0.1);
					plantings -= PlaceObjects (location, tiles, wildflowers,
						(tile) => GetWildflowerForSeason (RandomForTile (tile)));
				}
			}

			// Fill the remainder with weeds.
			PlaceObjects (location, tiles, plantings,
				(tile) => GetWeedsForSeason (RandomForTile (tile)),
				grabbable: false);
		}

		// Use the same seed that Crop uses for tint colors.
		private static Random RandomForTile (Vector2 tile) =>
			new Random (Game1.dayOfMonth + 1000 * (int) tile.X + (int) tile.Y);

		private static int RollForCount (int total, Random random,
			double baseChance, double randomChance, double luckMultiplier = 0.0,
			double rainChance = 0.0)
		{
			return (int) (total * Math.Min (1.0, baseChance +
				randomChance * random.NextDouble () +
				luckMultiplier * Game1.player.team.sharedDailyLuck.Value +
				(Game1.isRaining ? rainChance : 0.0)));
		}

		private static int GetWildflowerForSeason (Random random)
		{
			bool heads = random.Next (1) == 1;
			return Game1.currentSeason switch
			{
				// Dandelion, Daffodil
				"spring" => (random.NextDouble () < 0.25) ? 22 : 18,
				// Fiddlehead Fern, Sweet Pea
				"summer" => (random.NextDouble () < 0.25) ? 259 : 402,
				// Hazelnut, Common Mushroom
				"fall" => (random.NextDouble () < 0.25) ? 408 : 404,
				// Crocus
				"winter" => 418,
				_ => GameLocation.getWeedForSeason (random, Game1.currentSeason),
			};
		}

		private static int GetWeedsForSeason (Random random)
		{
			string season = Game1.currentSeason;
			switch (season)
			{
			case "summer":
				// Work around a bug in GameLocation.getWeedForSeason.
				season = " summer";
				break;
			case "winter":
				// Choose one of the ice crystals from the Mines.
				return random.Next (319, 322);
			}
			return (random.NextDouble () < 0.1)
				// Small chance of the seasonal special weed.
				? 792 + Utility.getSeasonNumber (Game1.currentSeason)
				// Otherwise proceed to the base game code.
				: GameLocation.getWeedForSeason (random, season);
		}

		private static int PlaceObjects (GameLocation location,
			Queue<Vector2> tiles, int count, Func<Vector2, int> parentSheetIndex,
			bool grabbable = true, bool isForage = true, int programColorCrop = -1)
		{
			int placed = 0;
			while (tiles.Count > 0 && placed < count)
			{
				Vector2 tile = tiles.Dequeue ();
				if (!IsSuitableTile (location, tile, clearGrass: true))
					continue;
				Color color = (programColorCrop != -1)
					? new Crop (programColorCrop, (int) tile.X, (int) tile.Y)
						.tintColor.Value
					: Color.White;
				SObject @object = isForage
					? new SObject (parentSheetIndex (tile), 1)
					: new Flower (parentSheetIndex (tile), 1, color);
				@object.TileLocation = tile;
				@object.IsSpawnedObject = grabbable;
				@object.CanBeGrabbed = grabbable;
				location.objects[tile] = @object;
				++placed;
			}
			return placed;
		}

		// Based on GameLocation.explode
		private static List<Vector2> FindTargetTiles (GameLocation location,
			Vector2 center)
		{
			bool[,] circleOutlineGrid = Game1.getCircleOutlineGrid (Radius);
			Vector2 tile = new Vector2 (center.X - Radius, center.Y - Radius);

			int width = location.map.Layers[0].LayerWidth;
			int height = location.map.Layers[0].LayerHeight;

			bool withinRadius = false;
			List<Vector2> targetTiles = new List<Vector2> ();

			for (int x = 0; x < Radius * 2 + 1; ++x)
			{
				for (int y = 0; y < Radius * 2 + 1; ++y)
				{
					if (x == 0 || y == 0 || x == Radius * 2 || y == Radius * 2)
					{
						withinRadius = circleOutlineGrid[x, y];
					}
					else if (circleOutlineGrid[x, y])
					{
						withinRadius = !withinRadius;
						if (!withinRadius)
							targetTiles.Add (tile);
					}
					if (withinRadius)
						targetTiles.Add (tile);
					tile.Y = Math.Min (height - 1, Math.Max (0, tile.Y + 1));
				}
				tile.X = Math.Min (width - 1f, Math.Max (0, tile.X + 1));
				tile.Y = Math.Min (height - 1f, Math.Max (0, center.Y - Radius));
			}

			return targetTiles;
		}
	}
}
