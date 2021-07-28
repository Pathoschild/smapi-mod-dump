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
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		public static readonly int Radius = 7;

		private bool germinating = false;

		public static void GerminateAll (bool force = false)
		{
			foreach (GameLocation location in Game1.locations)
			{
				if (!force && Config.WaterToGerminate &&
						!Game1.IsRainingHere (location) &&
						!Game1.IsSnowingHere (location))
					continue;

				foreach (var pair in location.objects.Pairs.ToArray ())
				{
					if (TryGetLinked (pair.Value, out FlowerBomb bomb))
						bomb.germinate (location, pair.Key);
				}
			}
		}

		private static void ApplySprinkler_Postfix (GameLocation location, Vector2 tile)
		{
			try
			{
				if (location.doesTileHavePropertyNoNull ((int) tile.X, (int) tile.Y,
						"NoSprinklers", "Back") == "T")
					return;
				if (location.objects.TryGetValue (tile, out SObject @object) &&
						TryGetLinked (@object, out FlowerBomb bomb))
					bomb.germinate (location, tile);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (ApplySprinkler_Postfix)}:\n{e}",
					LogLevel.Error);
			}
		}

		public void germinateLive (GameLocation location, Vector2 center)
		{
			// Only germinate once.
			if (germinating)
				return;

			// Choose a random ID for sprites.
			int id = Game1.random.Next ();

			// Play the fuse sound.
			location.netAudio.StartPlaying ("fuse");

			// Render the stray particles.
			MP.broadcastSprites (location, new TemporaryAnimatedSprite ("LooseSprites\\Cursors",
				new Rectangle (598, 1279, 3, 4), 53f, 5, 9, center * 64f + new Vector2 (5f, 3f) * 4f,
				flicker: true, flipped: false, (center.Y * 64 + 7) / 10000f, 0f, Color.Brown, 4f, 0f, 0f, 0f)
			{
				id = id,
			});
			MP.broadcastSprites (location, new TemporaryAnimatedSprite ("LooseSprites\\Cursors",
				new Rectangle (598, 1279, 3, 4), 53f, 5, 9, center * 64f + new Vector2 (5f, 3f) * 4f,
				flicker: true, flipped: true, (center.Y * 64 + 7) / 10000f, 0f, Color.Green, 4f, 0f, 0f, 0f)
			{
				delayBeforeAnimationStart = 100,
				id = id,
			});
			MP.broadcastSprites (location, new TemporaryAnimatedSprite ("LooseSprites\\Cursors",
				new Rectangle (598, 1279, 3, 4), 53f, 5, 9, center * 64f + new Vector2 (5f, 3f) * 4f,
				flicker: true, flipped: false, (center.Y * 64 + 7) / 10000f, 0f, Color.Yellow, 3f, 0f, 0f, 0f)
			{
				delayBeforeAnimationStart = 200,
				id = id,
			});

			// Render the shaking bomb, timing the next stage.
			bool ending = false;
			Base.isTemporarilyInvisible = true;
			MP.broadcastSprites (location, new TemporaryAnimatedSprite (
				(seed != null) ? FullSaveIndex.Index : TileIndex,
				100f, 1, 12, center * 64f, flicker: true, flipped: false, location, null)
			{
				shakeIntensity = 0.5f,
				shakeIntensityChange = 0.002f,
				endFunction = (int _) =>
				{
					// Only end once.
					if (ending)
						return;
					ending = true;

					// Remove the sprites.
					location.removeTemporarySpritesWithID (id);

					// Show a mild flash.
					if (Game1.currentLocation == location)
						Game1.flashAlpha = 0.5f;

					if (Context.IsMainPlayer)
					{
						// Stop the fuse sound and play the thud.
						location.netAudio.StopPlaying ("fuse");
						location.playSound ("treethud");

						// Actually germinate.
						germinate (location, center);
					}
				},
			});
		}

		public void germinate (GameLocation location, Vector2 center)
		{
			// Only germinate once.
			if (germinating)
				return;
			germinating = true;

			Monitor.Log ($"Flower Bomb with {seed?.DisplayName ?? "no seeds"} germinating at ({center.X},{center.Y}) on {location.Name}",
				LogLevel.Debug);

			// Remove the bomb itself.
			location.objects.Remove (center);

			// Roll for the overall shape and density of the growth.
			Random mainRandom = RandomForTile (center);
			Queue<Vector2> tiles = new (FindTargetTiles (location, center).OrderBy ((tile) =>
				Vector2.Distance (center, tile) + 1.5 * mainRandom.NextDouble ()));
			int plantings = RollForCount (tiles.Count, mainRandom, location,
				0.5, 0.2, 0.1, 0.1);

			if (seed != null)
			{
				Crop testCrop = new (seed.ParentSheetIndex, 0, 0);
				if (testCrop.seasonsToGrowIn.Contains
					(Game1.GetSeasonForLocation (location)))
				{
					// Place a medium quantity of the specific flower, if any.
					if (testCrop.whichForageCrop.Value == 0)
					{
						int specifics = RollForCount (plantings, mainRandom,
							location, 0.3, 0.2, 0.2);
						plantings -= PlaceObjects (location, tiles, specifics,
							(_tile) => testCrop.indexOfHarvest.Value,
							isForage: false,
							programColorCrop: testCrop.programColored.Value
								? seed.ParentSheetIndex : -1);
					}

					// Fill most of the remainder with seasonal wildflowers.
					int wildflowers = RollForCount (plantings, mainRandom,
						location, 0.8, 0.1, 0.1);
					plantings -= PlaceObjects (location, tiles, wildflowers,
						(tile) => GetWildflowerForSeason (RandomForTile (tile), location));
				}
			}

			// Fill the remainder with weeds.
			PlaceObjects (location, tiles, plantings,
				(tile) => GetWeedsForSeason (RandomForTile (tile), location),
				grabbable: false);
		}

		// Use the same seed that Crop uses for tint colors.
		private static Random RandomForTile (Vector2 tile) =>
			new (Game1.dayOfMonth + 1000 * (int) tile.X + (int) tile.Y);

		private static int RollForCount (int total, Random random, GameLocation location,
			double baseChance, double randomChance, double luckMultiplier = 0.0,
			double rainChance = 0.0)
		{
			return (int) (total * Math.Min (1.0, baseChance +
				randomChance * random.NextDouble () +
				luckMultiplier * Game1.player.team.sharedDailyLuck.Value +
				(Game1.IsRainingHere (location) ? rainChance : 0.0)));
		}

		private static int GetWildflowerForSeason (Random random, GameLocation location)
		{
			return Game1.GetSeasonForLocation (location) switch
			{
				// Dandelion, Daffodil
				"spring" => (random.NextDouble () < 0.25) ? 22 : 18,
				// Fiddlehead Fern, Sweet Pea
				"summer" => (random.NextDouble () < 0.25) ? 259 : 402,
				// Hazelnut, Common Mushroom
				"fall" => (random.NextDouble () < 0.25) ? 408 : 404,
				// Crocus
				"winter" => 418,
				// fallback
				_ => GetWeedsForSeason (random, location),
			};
		}

		private static int GetWeedsForSeason (Random random, GameLocation location)
		{
			string season = Game1.GetSeasonForLocation (location);

			// For winter, choose one of the ice crystals from the Mines.
			if (season == "winter")
				return random.Next (319, 322);

			// Small chance of the seasonal special weed.
			if (random.NextDouble () < 0.1)
				return 792 + Utility.getSeasonNumber (season);

			// Work around an oddity in GameLocation.getWeedForSeason.
			if (season == "summer")
				season = " summer";

			// Otherwise proceed to the base game code.
			return GameLocation.getWeedForSeason (random, season);
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
					: Wildflower.GetNew (parentSheetIndex (tile), color);
				@object.IsSpawnedObject = grabbable;
				@object.CanBeGrabbed = grabbable;
				@object.TileLocation = tile;
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
			Vector2 tile = new (center.X - Radius, center.Y - Radius);

			int mapWidth = location.map.Layers[0].LayerWidth;
			int mapHeight = location.map.Layers[0].LayerHeight;

			bool withinRadius = false;
			List<Vector2> targetTiles = new ();

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
					tile.Y = Math.Min (mapHeight - 1, Math.Max (0, tile.Y + 1));
				}
				tile.X = Math.Min (mapWidth - 1f, Math.Max (0, tile.X + 1));
				tile.Y = Math.Min (mapHeight - 1f, Math.Max (0, center.Y - Radius));
			}

			return targetTiles;
		}
	}
}
