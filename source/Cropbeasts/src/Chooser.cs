/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Cropbeasts
{
	public class Chooser
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public int outdoorSpawnCount { get; protected set; }
		public int indoorSpawnCount { get; protected set; }
		public int islandSpawnCount { get; protected set; }
		public int spawnCount => outdoorSpawnCount + indoorSpawnCount + islandSpawnCount;

		public bool witchFlyoverShown { get; set; }

		protected readonly Dictionary<Point, CropTile> chosenTiles = new ();

		public Chooser ()
		{ }

		public void reset ()
		{
			outdoorSpawnCount = 0;
			indoorSpawnCount = 0;
			islandSpawnCount = 0;
			witchFlyoverShown = false;
			chosenTiles.Clear ();
		}

		// Chooses a crop tile to become a cropbeast.
		public CropTile choose (bool console = false,
			bool force = false, string filter = null)
		{
			// Use a predictable RNG seeded by game, day and time.
			Random rng = new ((int) Game1.uniqueIDForThisGame / 2 +
				(int) Game1.stats.DaysPlayed + Game1.timeOfDay);

			// Choose the location to evaluate.
			var location = Game1.currentLocation;
			if (!IsSuitableLocation (location))
			{
				var farm = Game1.getLocationFromName ("Farm");
				var greenhouse = Game1.getLocationFromName ("Greenhouse");
				var islandFarm = Game1.getLocationFromName ("IslandWest");
				if (farm != null && farm.farmers.Count > 0)
					location = farm;
				else if (greenhouse != null && greenhouse.farmers.Count > 0)
					location = greenhouse;
				else if (islandFarm != null && islandFarm.farmers.Count > 0)
					location = islandFarm;
			}

			// Check preconditions unless forced.
			if (!force && !shouldChoose (rng, location, console: console))
				return null;

			// Get the maximum distance on the current map for weighting use.
			var mapLayer = location.map.Layers[0];
			float mapDiagonal = Vector2.Distance (new Vector2 (0, 0),
				new Vector2 (mapLayer.LayerWidth, mapLayer.LayerHeight));

			// Find all candidate crop tiles, sort pseudorandomly with
			// weighting based on type and towards those closer to a farmer.
			SortedList<double, CropTile> candidates =
				new (findCandidateCropTiles (location, filter, console)
						.ToDictionary ((tile) =>
				{
					Utilities.FindNearestFarmer (tile.location, tile.tileLocation,
						out float distance);
					return rng.NextDouble () * 1.5
						- tile.mapping.choiceWeight
						+ distance / mapDiagonal * 2.0;
				}));

			// If the list is empty, give up.
			if (candidates.Count == 0)
			{
				Monitor.Log ($"At {Game1.getTimeOfDayString (Game1.timeOfDay)}, {location.Name} met preconditions but had no candidate crops to become cropbeasts.",
					console ? LogLevel.Warn : LogLevel.Trace);
				return null;
			}

			// Choose the top of the list as the winner.
			CropTile winner = candidates.First ().Value;
			Monitor.Log ($"At {Game1.getTimeOfDayString (Game1.timeOfDay)}, chose {winner.logDescription} to become {winner.mapping.beastName}.",
				console ? LogLevel.Info : LogLevel.Debug);

			// Record progress towards the daily limit for the location type.
			if (location is IslandLocation)
				++islandSpawnCount;
			else if (location.IsOutdoors)
				++outdoorSpawnCount;
			else
				++indoorSpawnCount;

			// Make sure this tile isn't chosen again.
			chosenTiles[winner.tileLocation] = winner;
			winner.choose ();

			return winner;
		}

		// Alters records as if the given crop tile had never been chosen.
		public void unchoose (CropTile tile)
		{
			if (!chosenTiles.ContainsKey (tile.tileLocation))
				return;

			if (tile.location is IslandLocation)
				--islandSpawnCount;
			else if (tile.location.IsOutdoors)
				--outdoorSpawnCount;
			else
				--indoorSpawnCount;

			chosenTiles.Remove (tile.tileLocation);
		}

		protected static bool IsSuitableLocation (GameLocation location)
		{
			return (location.IsFarm && location.IsOutdoors) ||
				location.IsGreenhouse ||
				// NOTE: Might need to accommodate more island maps eventually.
				(location is IslandLocation && location.Name == "IslandWest");
		}

		// Checks whether an attempt to spawn a cropbeast should be made now.
		protected bool shouldChoose (Random rng, GameLocation location,
			bool console = false)
		{
			// Only spawn if the daily chance roll succeeds. This uses a separate
			// RNG seeded by the game and day only.
			Random dayRNG = new ((int) Game1.uniqueIDForThisGame / 2 +
				(int) Game1.stats.DaysPlayed);
			if (!(dayRNG.NextDouble () < Config.DailyChance))
			{
				if (console)
					Monitor.Log ($"If not by console command, wouldn't have spawned a cropbeast because the daily chance roll failed.", LogLevel.Info);
				else
					return false;
			}

			// Only spawn in daytime. Nighttime is for the regular Wilderness
			// Farm monsters. The dusk hour is left as a buffer period.
			if (Game1.timeOfDay >= 1800)
			{
				if (console)
					Monitor.Log ($"Not spawning a cropbeast because the current time {Game1.timeOfDay} is after 1800.", LogLevel.Info);
				return false;
			}

			// Don't spawn on a festival or wedding day.
			if (Utility.isFestivalDay (Game1.Date.DayOfMonth, Game1.Date.Season) ||
					Game1.weddingToday)
			{
				if (console)
					Monitor.Log ($"Not spawning a cropbeast because it is a festival or wedding day.", LogLevel.Info);
				return false;
			}

			// Only spawn if all previous cropbeasts have been slain,
			// unless otherwise configured.
			if (ModEntry.Instance.currentBeastCount > 0 && !Config.AllowSimultaneous)
			{
				if (console)
					Monitor.Log ($"Not spawning a cropbeast because there are already {ModEntry.Instance.currentBeastCount} active and we are not configured for simultaneous spawns.", LogLevel.Info);
				return false;
			}

			// Only spawn on farms with monsters, unless otherwise configured.
			if (!Game1.spawnMonstersAtNight && !Config.SpawnOnAnyFarm)
			{
				if (console)
					Monitor.Log ($"Not spawning a cropbeast because farm monsters are deactivated and we are not configured to ignore that.", LogLevel.Info);
				return false;
			}

			// Only spawn outdoors on a farm or indoors in a greenhouse.
			if (!IsSuitableLocation (location))
			{
				if (console)
					Monitor.Log ($"Not spawning a cropbeast because {location.Name} is not outdoors on a farm or indoors in a greenhouse.", LogLevel.Info);
				return false;
			}

			// Check for location type.
			bool outdoors = location.IsOutdoors;
			bool island = location is IslandLocation;

			// Only spawn if there are a minimum number of crops in the location.
			int totalCount = CropTile.CountAll (location);
			int minTotalCount = island ? 8 : outdoors ? 16 : 4;
			if (totalCount < minTotalCount)
			{
				if (console)
					Monitor.Log ($"Not spawning a cropbeast because there are only {totalCount} crop(s) on {location.Name} (minimum {minTotalCount}).", LogLevel.Info);
				return false;
			}

			// Only spawn if the applicable daily limit has not been hit.
			int limit = outdoors
				? Config.OutdoorSpawnLimit
				: Config.IndoorSpawnLimit;
			int count = island
				? islandSpawnCount
				: outdoors
					? outdoorSpawnCount
					: indoorSpawnCount;
			if (limit >= 0 && count >= limit)
			{
				if (console)
				{
					string locationType = island
						? "on the island"
						: outdoors
							? "outdoors"
							: "indoors";
					Monitor.Log ($"Not spawning a cropbeast because we have already spawned {count} out of a limit of {limit} {locationType}.", LogLevel.Info);
				}
				return false;
			}

			// Give a small fixed chance of a cropbeast spawning at each clock
			// tick, scaled based on the total number of spawns expected so that
			// the desired limit fits within the day comfortably.
			double chance = 0.01 * limit;

			// However, don't let too much time pass without a spawn lest the
			// count fall too far short of the desired limit.
			double dayProgress = (Game1.timeOfDay / 100 - 6) / 12.0;
			double spawnProgress = (count + 1) / (double) limit;
			if (dayProgress > spawnProgress)
				chance *= 5.0;

			// Roll for chance. Always succeed when no RNG or for the console command.
			if (rng == null)
				return true;
			else if (rng.NextDouble () < chance)
				return true;
			else if (console)
			{
				Monitor.Log ($"If not by console command, wouldn't have spawned a cropbeast because the individual chance roll failed.", LogLevel.Info);
				return true;
			}
			else
				return false;
		}

		// Lists all the tiles in the given location with crops
		// on them that are candidates for becoming cropbeasts.
		protected List<CropTile> findCandidateCropTiles (GameLocation location,
			string filter = null, bool console = false)
		{
			List<SObject> wickedStatues = Utilities.FindWickedStatues (location);
			List<JunimoHut> junimoHuts = Utilities.FindActiveJunimoHuts (location);

			// For IF2R, avoid cropbeast-spawning special giant crops in an area
			// of the map that is initially inaccessible.
			List<Point> exemptTiles = new ();
			if (location is Farm && Game1.whichFarm == Farm.default_layout &&
					Helper.ModRegistry.IsLoaded ("flashshifter.immersivefarm2remastered"))
				exemptTiles.Add (new Point (79, 99));

			return CropTile.FindAll (location).Where ((tile) =>
			{
				// The crop must be available.
				if (tile.state != CropTile.State.Available)
					return false;

				// If a filter was supplied, it must match.
				if (filter != null && !tile.mapping.matchesFilter (filter))
				{
					if (console)
						Monitor.Log ($"Excluded a {tile.logDescription} because it does not match the filter \"{filter}\".");
					return false;
				}

				// The crop's tile must not be on the exempt list for the map.
				Point tileLoc = tile.tileLocation;
				if (exemptTiles.Contains (tileLoc))
				{
					if (console)
						Monitor.Log ($"Excluded a {tile.logDescription} because its tile is exempt on this map.");
					return false;
				}

				// The crop must not have already been chosen by another
				// in-progress cropbeast spawn.
				if (chosenTiles.ContainsKey (tileLoc))
				{
					if (console)
						Monitor.Log ($"Excluded a {tile.logDescription} because its tile has already been chosen.");
					return false;
				}

				// A non-giant crop must not be in range of an active Junimo Hut.
				if (!tile.giantCrop)
				{
					foreach (JunimoHut junimoHut in junimoHuts)
					{
						if (tileLoc.X >= junimoHut.tileX.Value + 1 - 8 &&
							tileLoc.X < junimoHut.tileX.Value + 2 + 8 &&
							tileLoc.Y >= junimoHut.tileY.Value - 8 + 1 &&
							tileLoc.Y < junimoHut.tileY.Value + 2 + 8)
						{
							if (console)
								Monitor.Log ($"Excluded a {tile.logDescription} because it is in range of an active Junimo Hut at ({junimoHut.tileX.Value},{junimoHut.tileY.Value}).");
							return false;
						}
					}
				}

				// The crop must not be in range of a Wicked Statue.
				foreach (SObject wickedStatue in wickedStatues)
				{
					if (Config.WickedStatueRange == -1 || // infinite range
						Vector2.Distance (Utility.PointToVector2 (tileLoc),
							wickedStatue.TileLocation) < (float) Config.WickedStatueRange)
					{
						if (console)
							Monitor.Log ($"Excluded a {tile.logDescription} because it is in range of a Wicked Statue at ({(int) wickedStatue.TileLocation.X},{(int) wickedStatue.TileLocation.Y}).");
						return false;
					}
				}

				return true;
			}).ToList ();
		}
	}
}
