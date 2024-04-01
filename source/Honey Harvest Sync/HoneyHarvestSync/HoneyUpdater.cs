/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/voltaek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyHarvestSync
{
	public static class HoneyUpdater
	{
		/// <summary>Should be set as a refence to the Mod's monitor before anything in here is called (or set as event handlers) so we can do logging.</summary>
		internal static IMonitor Monitor { get; set; }

		/// <summary>Minutes total from when the farmer/player wakes up (6am) until the latest they can be awake (2am).</summary>
		private const int maxMinutesAwake = 1200;

		/// <summary>The max default range a flower can affect a bee house from.</summary>
		private const int flowerRange = 5;

		/// <summary>
		/// Time the farmer wakes up, but in the 24 hour integer form used in the properties of `TimeChangedEventArgs`.
		/// Examples: 600 is 6am and 1300 is 1pm.
		/// </summary>
		private const int startOfDayTime = 600;

		/// <summary>The globally unique identifier for Bee House machines.</summary>
		private const string beeHouseQualifiedItemID = "(BC)10";

		/// <summary>Filter to test locations with to see if they can and do have bee houses in them.</summary>
		private static readonly Func<GameLocation, bool> isLocationWithBeeHouses = (location) => location.IsOutdoors && location.Objects.Values.Any(x => x.QualifiedItemId == beeHouseQualifiedItemID);

		// Tracking lists for bee houses and flowers nearby them that we refresh each day.

		private static readonly Dictionary<GameLocation, List<SObject>> beeHousesReady = new();
		private static readonly Dictionary<GameLocation, List<SObject>> beeHousesReadyToday = new();
		// Do not trust the items in this; they may have become `null`.
		private static readonly HashSet<HoeDirt> nearbyFlowerDirts = new();

		// For debug builds, show log messages as DEBUG so they show in the SMAPI console.
		#if DEBUG
		private const LogLevel logLevel = LogLevel.Debug;
		#else
		private const LogLevel logLevel = LogLevel.Trace;
		#endif

		// Shorthand method for creating a standard log entry.
		private static void Log(string message) => Monitor.Log(message, logLevel);

		// Shorthand property for creating a verbose log entry header.
		// We want to use the verbose log method directly for best performance, both when actually using verbose and not.
		private static string GetVerboseStart
		{
			// Show microsecond, so we can tell if something is slow.
			get { return Monitor.IsVerbose ? DateTime.Now.ToString("ffffff") : String.Empty; }
		}

		/// <summary>Event handler for after a new day starts.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			Monitor.VerboseLog($"{GetVerboseStart} {nameof(OnDayStarted)} - Started");

			// Reset our tracked bee houses and flowers for the new day
			beeHousesReady.Clear();
			beeHousesReadyToday.Clear();
			nearbyFlowerDirts.Clear();

			// Filter to just locations we care about.
			foreach (GameLocation location in Game1.locations.Where(x => isLocationWithBeeHouses(x)))
			{
				AddLocation(location);
			}

			Monitor.VerboseLog($"{GetVerboseStart} {nameof(OnDayStarted)} - Ended");
		}

		/// <summary>Event handler for when the in-game clock changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnTimeChanged(object sender, TimeChangedEventArgs e)
		{
			// We don't need to day anything right when we wake up, since that's handled by `OnDayStarted`,
			// and we don't want to have any race conditions with it, either.
			if (e.NewTime == startOfDayTime)
			{
				return;
			}

			foreach (KeyValuePair<GameLocation, List<SObject>> entry in beeHousesReadyToday)
			{
				List<SObject> newlyReadyBeeHouses = entry.Value.Where(x => x.readyForHarvest.Value).ToList();

				if (newlyReadyBeeHouses.Count == 0)
				{
					continue;
				}

				Log($"{nameof(OnTimeChanged)} - Found {newlyReadyBeeHouses.Count} newly ready bee houses @ {entry.Key.Name} location");

				UpdateLocationBeeHouses(entry.Key, newlyReadyBeeHouses);

				if (!beeHousesReady.ContainsKey(entry.Key))
				{
					beeHousesReady.Add(entry.Key, new List<SObject>());
				}

				beeHousesReady[entry.Key].AddRange(newlyReadyBeeHouses);
				beeHousesReadyToday[entry.Key].RemoveAll(x => newlyReadyBeeHouses.Contains(x));
			}
		}

		/// <summary>Event handler for after the game state is updated, once per second.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
		{
			int dirtRemoved = nearbyFlowerDirts.RemoveWhere(x => x == null);

			if (dirtRemoved > 0)
			{
				Monitor.LogOnce($"{nameof(HoneyUpdater)} {nameof(OnOneSecondUpdateTicked)} Removed {dirtRemoved} `null` flower crop HoeDirt from tracking"
					+ $" (future duplicates of this log message will only appear in the log file itself)", LogLevel.Info);

				Log($"{nameof(HoneyUpdater)} {nameof(OnOneSecondUpdateTicked)} Removed {dirtRemoved} `null` flower crop HoeDirt from tracking");
			}

			// Check if flowers that would affect the honey produced by nearby bee houses have been harvested.
			List<HoeDirt> flowerlessDirts = nearbyFlowerDirts.Where(x => x.crop == null).ToList();

			if (flowerlessDirts.Count == 0)
			{
				return;
			}

			Log($"{nameof(OnOneSecondUpdateTicked)} - Found {flowerlessDirts.Count} harvested flowers.");
			Monitor.VerboseLog($"{GetVerboseStart} {nameof(OnOneSecondUpdateTicked)} - Harvested flower details: {String.Join(" | ", flowerlessDirts.Select(x => $"{x.Location.Name} @ {x.Tile}"))}");

			// Remove the flower tile(s) from being tracked
			nearbyFlowerDirts.RemoveWhere(flowerlessDirts.Contains);

			// Track bee houses we've already updated in this following loop so we only update them each once
			Dictionary<GameLocation, List<SObject>> updatedBeeHouses = new();

			// Update all bee houses near the removed dirt(s)
			foreach (HoeDirt flowerlessDirt in flowerlessDirts)
			{
				if (!beeHousesReady.ContainsKey(flowerlessDirt.Location))
				{
					continue;
				}

				// Collect all the bee houses within the effective range of the removed flower that we haven't updated already.
				List<SObject> beeHousesToUpdate = beeHousesReady[flowerlessDirt.Location].Where(beeHouse =>
					(!updatedBeeHouses.ContainsKey(flowerlessDirt.Location) || !updatedBeeHouses[flowerlessDirt.Location].Contains(beeHouse))
					&& IsWithinFlowerRange(beeHouse.TileLocation, flowerlessDirt.Tile)
				).ToList();
				
				if (beeHousesToUpdate.Count == 0)
				{
					continue;
				}

				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {beeHousesToUpdate.Count} ready bee houses that need updating @ {flowerlessDirt.Location.Name} location.");

				UpdateLocationBeeHouses(flowerlessDirt.Location, beeHousesToUpdate);
				
				if (!updatedBeeHouses.ContainsKey(flowerlessDirt.Location))
				{
					updatedBeeHouses[flowerlessDirt.Location] = new();
				}

				// Track any bee houses we've updated already to prevent duplicate updates
				updatedBeeHouses[flowerlessDirt.Location].AddRange(beeHousesToUpdate);

				Monitor.VerboseLog($"{GetVerboseStart} {nameof(OnOneSecondUpdateTicked)} - Updated bee house details: {String.Join(" | ", beeHousesToUpdate.Select(x => x.TileLocation))}");
			}
		}

		/// <summary>
		/// Event handler for after objects are added/removed in any location (including machines, fences, etc).
		/// This doesn't apply for floating items (see DebrisListChanged) or furniture (see FurnitureListChanged).
		/// This event isn't raised for objects already present when a location is added. If you need to handle those too, use `LocationListChanged` and check `e.Added → objects`.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
		{
			// Check the location and objects similar to how we location-filter elsewhere
			if (!e.Removed.Any()
				|| !e.Location.IsOutdoors
				|| (!beeHousesReady.ContainsKey(e.Location) && !beeHousesReadyToday.ContainsKey(e.Location))
				|| !e.Removed.Any(x => x.Value.QualifiedItemId == beeHouseQualifiedItemID))
			{
				return;
			}

			// Find all removed bee houses so we can remove them from our tracking dictionaries
			IEnumerable<SObject> removedBeeHouses = e.Removed.Select(y => y.Value).Where(z => z.QualifiedItemId == beeHouseQualifiedItemID);
			Log($"{nameof(OnObjectListChanged)} - Found {removedBeeHouses.Count()} bee houses to attempt to remove from tracking");

			if (beeHousesReady.ContainsKey(e.Location) && beeHousesReady[e.Location].Any(x => removedBeeHouses.Contains(x)))
			{
				beeHousesReady[e.Location].RemoveAll(x => removedBeeHouses.Contains(x));
				Monitor.VerboseLog($"{GetVerboseStart} {nameof(OnObjectListChanged)} - {e.Location} location has {beeHousesReady[e.Location].Count} remaining tracked ready bee houses");
			}

			if (beeHousesReadyToday.ContainsKey(e.Location) && beeHousesReadyToday[e.Location].Any(x => removedBeeHouses.Contains(x)))
			{
				beeHousesReadyToday[e.Location].RemoveAll(x => removedBeeHouses.Contains(x));
				Monitor.VerboseLog($"{GetVerboseStart} {nameof(OnObjectListChanged)} - {e.Location} location has {beeHousesReadyToday[e.Location].Count} remaining tracked ready-today bee houses");
			}
		}

		/// <summary>Event handler for after a game location is added or removed (including building interiors).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnLocationListChanged(object sender, LocationListChangedEventArgs e)
		{
			foreach (GameLocation addedLocation in e.Added.Where(x => isLocationWithBeeHouses(x)))
			{
				// If we have the location tracked already, remove all existing tracking before we (re-)add the location
				RemoveLocationFromTracking(addedLocation);

				// Now add the location fresh to our tracking and run any updates we need to do
				AddLocation(addedLocation);
			}

			// Clear any data we are tracking about this location
			foreach (GameLocation removedLocation in e.Removed.Where(x => beeHousesReady.ContainsKey(x) || beeHousesReadyToday.ContainsKey(x)))
			{
				RemoveLocationFromTracking(removedLocation);
			}
		}

		/// <summary>
		/// Remove anything we're tracking at the given location.
		/// </summary>
		/// <param name="location">The location to no longer track anything at.</param>
		private static void RemoveLocationFromTracking(GameLocation location)
		{
			if (beeHousesReady.ContainsKey(location))
			{
				beeHousesReady.Remove(location);
			}

			if (beeHousesReadyToday.ContainsKey(location))
			{
				beeHousesReadyToday.Remove(location);
			}

			int dirtRemoved = nearbyFlowerDirts.RemoveWhere(x => x == null);

			if (dirtRemoved > 0)
			{
				Monitor.LogOnce($"{nameof(HoneyUpdater)} {nameof(RemoveLocationFromTracking)} Removed {dirtRemoved} `null` flower crop HoeDirt from tracking"
					+ $" (future duplicates of this log message will only appear in the log file itself)", LogLevel.Info);

				Log($"{nameof(HoneyUpdater)} {nameof(RemoveLocationFromTracking)} Removed {dirtRemoved} `null` flower crop HoeDirt from tracking");
			}

			nearbyFlowerDirts.RemoveWhere(x => x.Location == location);
		}

		/// <summary>
		/// Refresh the held object in all ready-for-harvest bee houses.
		/// This will refresh the icon shown overtop the bee houses.
		/// This can be used in cases such as if the bee houses should now be showing a different icon
		/// due to a config value that would affect the assigned item being changed.
		/// </summary>
		public static void RefreshBeeHouseHeldObjects()
		{
			Monitor.VerboseLog($"{GetVerboseStart} {nameof(RefreshBeeHouseHeldObjects)} - Started");

			foreach (KeyValuePair<GameLocation, List<SObject>> kvp in beeHousesReady)
			{
				UpdateLocationBeeHouses(kvp.Key, kvp.Value);
			}

			Monitor.VerboseLog($"{GetVerboseStart} {nameof(RefreshBeeHouseHeldObjects)} - Ended");
		}

		/// <summary>
		/// Updates the honey held by the given ready-for-harvest bee houses, which are at the given location.
		/// This also adds any nearby flowers to our tracked list of them.
		/// </summary>
		/// <param name="location">The location of the ready bee houses.</param>
		/// <param name="readyBeeHouses">The bee houses which are ready to be harvested which we should update the honey of.</param>
		private static void UpdateLocationBeeHouses(GameLocation location, List<SObject> readyBeeHouses)
		{
			Monitor.VerboseLog($"{GetVerboseStart} {nameof(UpdateLocationBeeHouses)} - Started");

			ObjectDataDefinition objectData = ItemRegistry.GetObjectTypeDefinition();
			List<SObject> invalidBeeHouses = new();
			int newlyTrackedFlowerDirtCount = 0;

			foreach (SObject beeHouse in readyBeeHouses)
			{
				// If a bee house no longer qualifies in any way, we'll remove it after we go through the list we were given
				if (beeHouse == null || !beeHouse.readyForHarvest.Value || beeHouse.QualifiedItemId != beeHouseQualifiedItemID)
				{
					invalidBeeHouses.Add(beeHouse);

					// If the issue is just that the bee house was harvested, then no log entry should be made
					if (beeHouse?.readyForHarvest.Value == false)
					{
						continue;
					}

					Monitor.Log($"Found an invalid bee house @ {location} location; removing from tracking: "
						+ $"{(beeHouse == null ? "null" : $"Tile {beeHouse.TileLocation}; RFH {(beeHouse.readyForHarvest.Value ? "Yes" : "No")}; QID {beeHouse.QualifiedItemId}")}", LogLevel.Info);

					continue;
				}

				// Same flower check the game uses (see `MachineDataUtility.GetNearbyFlowerItemId()`) when collecting the honey out of the bee house
				Crop closeFlower = Utility.findCloseFlower(location, beeHouse.TileLocation, flowerRange, (Crop crop) => !crop.forageCrop.Value);
				SObject flowerIngredient = null;
				string flowerName = String.Empty;

				// If we found a qualifying flower crop, then get its harvested object form.
				if (closeFlower != null)
				{
					string flowerIngredientID = ItemRegistry.QualifyItemId(closeFlower.indexOfHarvest.Value);

					if (flowerIngredientID == null)
					{
						Monitor.Log($"Failed to get the qualified item ID of a nearby flower from the flower's `indexOfHarvest.Value` value of '{closeFlower.indexOfHarvest.Value}'.", LogLevel.Warn);
					}
					else
					{
						string itemCreationFailureMessage = $"Failed to create an `Item` (and then convert it to `Object`) via `ItemRegistry.Create` using a nearby flower's qualified item ID of '{flowerIngredientID}'.";

						// `StardewValley.Internal.ItemQueryResolver.ItemQueryResolver.DefaultResolvers.FLAVORED_ITEM()` has this in a `try/catch`, so mimicking that here 
						try
						{
							// If this comes back as `null` or the conversion fails (resulting in `null`), that's fine since we'll just get "Wild Honey" back when we attempt to create flavored honey below.
							flowerIngredient = ItemRegistry.Create(flowerIngredientID) as SObject;

							if (flowerIngredient == null)
							{
								Monitor.Log(itemCreationFailureMessage, LogLevel.Warn);
							}
							else
							{
								flowerName = flowerIngredient.Name;
							}
						}
						catch (Exception ex)
						{
							Monitor.Log(itemCreationFailureMessage + $"\n\nException ({ex.GetType().Name}): {ex.Message}", LogLevel.Error);
						}
					}

					if (closeFlower.Dirt == null)
					{
						Monitor.Log($"Flower crop {(String.IsNullOrEmpty(flowerName) ? String.Empty : $"({flowerName}) ")}has a `null` 'Dirt' property. Will be unable to track if this flower gets harvested. "
							+ $"(Bee House Tile {beeHouse.TileLocation} and {location.Name} location)", LogLevel.Info);
					}
					// Track the tile location of the `HoeDirt` that holds the flower's `Crop` object so we can watch for it being harvested later.
					else if (nearbyFlowerDirts.Add(closeFlower.Dirt))
					{
						newlyTrackedFlowerDirtCount += 1;

						Monitor.VerboseLog($"{GetVerboseStart} Now tracking nearby grown flower {(String.IsNullOrEmpty(flowerName) ? String.Empty : $"({flowerName}) ")}"
							+ $"via its dirt with tile {closeFlower.Dirt.Tile} affecting bee house @ {beeHouse.TileLocation} tile @ {location.Name} location");
					}
				}

				/*
				We set the held object to either the default honey item (such as when there are no nearby flowers or all nearby flowers were harvested),
				or to an object that will inform the player about what they'll receive at time of harvest, due to a qualifying full-grown flower being nearby enough.

				If the player's mod config specifies to show the nearby flower (the default), we use the flower ingredient object we created as the held object.
				Otherwise, if they changed their option to show artisan honey, we'll use the flower ingredient to create the flavored honey object.
				Note that the user will need to have another mod to provide custom icons for artisan honey items for this option to display anything besides the base honey item.

				The game will create its own honey object at harvest to return to the farmer, so whatever we have the bee house holding in the meantime won't affect gameplay in any way.
				Previous to SD v1.6, though, the game only updated some of the held object's properties rather than creating a new object,
				so we couldn't use the flower option (without additional programming), then.

				Note that the ingredient passed to `ObjectDataDefinition.CreateFlavoredHoney()` being `null` is fine for honey as it will return the base/default "Wild Honey" object.
				Ref: `Object.CheckForActionOnMachine()`
				*/
				beeHouse.heldObject.Value = flowerIngredient != null && ModEntry.Config.BeeHouseReadyIconEnum == ModConfig.ReadyIcon.Flower
					? flowerIngredient
					: objectData.CreateFlavoredHoney(flowerIngredient);

				Monitor.VerboseLog($"{GetVerboseStart} Assigned {beeHouse.heldObject.Value.Name} to bee house @ {beeHouse.TileLocation} tile @ {location.Name} location");
			}

			// Remove any invalid bee houses from the given list
			readyBeeHouses.RemoveAll(x => invalidBeeHouses.Contains(x));

			Log($"{nameof(UpdateLocationBeeHouses)} - Updated {readyBeeHouses.Count} ready bee houses "
				+ (newlyTrackedFlowerDirtCount > 0 ? $"and now tracking {newlyTrackedFlowerDirtCount} additional nearby flowers" : String.Empty)
				+ $" @ {location.Name} location");

			Monitor.VerboseLog($"{GetVerboseStart} {nameof(UpdateLocationBeeHouses)} - Ended");
		}

		/// <summary>
		/// Adds bee houses in the given location to our lists of bee houses.
		/// For "ready" bee houses, will also update the bee houses, which also adds flowers nearby to the bee houses to our tracked list.
		/// </summary>
		/// <param name="location">The location to add to tracking and immediately start tracking thing at.</param>
		private static void AddLocation(GameLocation location)
		{
			List<SObject> ready = location.Objects.Values.Where(x => x.QualifiedItemId == beeHouseQualifiedItemID && x.readyForHarvest.Value).ToList();
			List<SObject> readyToday = location.Objects.Values.Where(x => x.QualifiedItemId == beeHouseQualifiedItemID
				&& !x.readyForHarvest.Value && x.MinutesUntilReady <= maxMinutesAwake).ToList();

			if (ready.Count > 0)
			{
				Log($"{nameof(AddLocation)} - Found {ready.Count} ready bee houses @ {location.Name} location");

				beeHousesReady.Add(location, ready);
				UpdateLocationBeeHouses(location, ready);
			}

			if (readyToday.Count > 0)
			{
				Log($"{nameof(AddLocation)} - Found {readyToday.Count} bee houses that will be ready today @ {location.Name} location");

				beeHousesReadyToday.Add(location, readyToday);
			}
		}

		/// <summary>Checks if a given location is within the effective range of a flower./// </summary>
		/// <param name="checkLocation">The tile location to check.</param>
		/// <param name="flowerLocation">The location of the flower.</param>
		/// <returns>True if the location is within range, False if not.</returns>
		internal static bool IsWithinFlowerRange(Vector2 checkLocation, Vector2 flowerLocation)
		{
			// Start with a quick check to see if it's in a square of the radius size since that's much faster to check
			if (!(checkLocation.X <= flowerLocation.X + flowerRange && checkLocation.X >= Math.Max(flowerLocation.X - flowerRange, 0)
				&& checkLocation.Y <= flowerLocation.Y + flowerRange && checkLocation.Y >= Math.Max(flowerLocation.Y - flowerRange, 0)))
			{
				return false;
			}

			int yCheck = 0;
			int xCheck = flowerRange;

			// This does kind of "middle out" checking of the diamond shape so we hit the horizontal rows with the most tiles first.
			// We start with the full-width middle row, then check the row above AND below that one at once, but with one less tile on each horizontal side,
			// then continue checking above and below those ones, each time checking less horizontal tiles, until we finish by checking the topmost tile and bottommost tile.
			// In testing, doing it this way takes on average about half the checks versus scanning from topmost tile down each row until bottommost tile.
			while (yCheck <= flowerRange)
			{
				if ((checkLocation.Y == flowerLocation.Y + yCheck || (yCheck != 0 && checkLocation.Y == Math.Max(flowerLocation.Y - yCheck, 0)))
					&& checkLocation.X >= Math.Max(flowerLocation.X - xCheck, 0)
					&& checkLocation.X <= flowerLocation.X + xCheck)
				{
					return true;
				}

				yCheck += 1;
				xCheck -= 1;
			}

			return false;
		}

		internal static void TestIsWithinFlowerRange(bool shouldTestDebugLocations = true, bool shouldTestRandomLocations = false)
		{
			// NOTE - If testing this function elsewhere (such as https://dotnetfiddle.net), will need to include
			// the 'MonoGame.Framework.Gtk' v3.8.0 Nuget package, add `using Microsoft.Xna.Framework;`, and declare the `flowerRange` const int.

			// This location should have at least double the `flowerRange` value for both axis to not break the below debug locations.
			Vector2 flower = new(flowerRange * 2, flowerRange * 2);

			// Debug Locations - these should show whether the algorithm is working or not
			System.Collections.Generic.List<Vector2> insideDiamondLocations = new() {
				new Vector2(flower.X, flower.Y + flowerRange),
				new Vector2(flower.X, flower.Y - flowerRange),
				new Vector2(flower.X - flowerRange, flower.Y),
				new Vector2(flower.X + flowerRange, flower.Y),
				new Vector2(flower.X + flowerRange / 2, flower.Y + flowerRange / 2),
				new Vector2(flower.X - flowerRange / 2, flower.Y + flowerRange / 2),
				new Vector2(flower.X - flowerRange / 2, flower.Y - flowerRange / 2),
				new Vector2(flower.X + flowerRange / 2, flower.Y - flowerRange / 2),
				new Vector2(flower.X - 1, flower.Y + flowerRange - 1),
				new Vector2(flower.X + 1, flower.Y + flowerRange - 1),
				new Vector2(flower.X - 1, flower.Y - flowerRange + 1),
				new Vector2(flower.X + 1, flower.Y - flowerRange + 1),
				new Vector2(flower.X - flowerRange + 1, flower.Y - 1),
				new Vector2(flower.X + flowerRange - 1, flower.Y - 1),
				new Vector2(flower.X - flowerRange + 1, flower.Y + 1),
				new Vector2(flower.X + flowerRange - 1, flower.Y + 1),
			};
			System.Collections.Generic.List<Vector2> outsideDiamondInsideSquareLocations = new() {
				new Vector2(flower.X + flowerRange, flower.Y + flowerRange),
				new Vector2(flower.X - flowerRange, flower.Y + flowerRange),
				new Vector2(flower.X - flowerRange, flower.Y - flowerRange),
				new Vector2(flower.X + flowerRange, flower.Y - flowerRange),
				new Vector2(flower.X + flowerRange, flower.Y + 1),
				new Vector2(flower.X + flowerRange, flower.Y - 1),
				new Vector2(flower.X - flowerRange, flower.Y + 1),
				new Vector2(flower.X - flowerRange, flower.Y - 1),
				new Vector2(flower.X + 1, flower.Y + flowerRange),
				new Vector2(flower.X - 1, flower.Y + flowerRange),
				new Vector2(flower.X + 1, flower.Y - flowerRange),
				new Vector2(flower.X - 1, flower.Y - flowerRange),
			};
			System.Collections.Generic.List<Vector2> outsideSquareLocations = new() {
				new Vector2(flower.X, flower.Y + flowerRange + 1),
				new Vector2(flower.X, flower.Y - flowerRange - 1),
				new Vector2(flower.X - flowerRange - 1, flower.Y),
				new Vector2(flower.X + flowerRange + 1, flower.Y),
				new Vector2(flower.X + flowerRange + 1, flower.Y + flowerRange + 1),
				new Vector2(flower.X - flowerRange - 1, flower.Y + flowerRange + 1),
				new Vector2(flower.X - flowerRange - 1, flower.Y - flowerRange - 1),
				new Vector2(flower.X + flowerRange + 1, flower.Y - flowerRange - 1),
			};

			// Random Locations - these can test real-world speed differences between algorithms
			System.Collections.Generic.List<Vector2> randomLocations = new();

			// Can mess with this to test checking locations at various max distances from the flower location
			int maxDistanceAway = flowerRange * 2;

			int minX = Math.Max(Convert.ToInt32(flower.X) - maxDistanceAway, 0);
			int maxX = Convert.ToInt32(flower.X) + maxDistanceAway;
			int minY = Math.Max(Convert.ToInt32(flower.Y) - maxDistanceAway, 0);
			int maxY = Convert.ToInt32(flower.Y) + maxDistanceAway;
			Random rand = new();

			for (int i = 0; i < 50; i++)
			{
				randomLocations.Add(new Vector2(rand.Next(minX, maxX + 1), rand.Next(minY, maxY + 1)));
			}

			System.Collections.Generic.List<string> fails = new();

			System.Collections.Generic.List<string> ins = new();
			System.Collections.Generic.List<string> outs = new();

			// TESTING STARTS

			System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

			if (shouldTestDebugLocations)
			{
				Console.WriteLine("DEBUG LOCATIONS\n-- Within Diamond --");
				foreach (Vector2 test in insideDiamondLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (!result)
					{
						fails.Add($"{test}");
					}
				}
				if (fails.Count > 0)
				{
					Console.WriteLine($"FAILS: {String.Join(" | ", fails)}");
					fails.Clear();
				}

				Console.WriteLine("\n-- Outside Diamond, but Inside Square --");
				foreach (Vector2 test in outsideDiamondInsideSquareLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (result)
					{
						fails.Add($"{test}");
					}
				}
				if (fails.Count > 0)
				{
					Console.WriteLine($"FAILS: {String.Join(" | ", fails)}");
					fails.Clear();
				}

				Console.WriteLine("\n-- Outside Square --");
				foreach (Vector2 test in outsideSquareLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (result)
					{
						fails.Add($"{test}");
					}
				}
				if (fails.Count > 0)
				{
					Console.WriteLine($"FAILS: {String.Join(" | ", fails)}");
					fails.Clear();
				}

				sw.Stop();
				Console.WriteLine($"\nTested {insideDiamondLocations.Count + outsideDiamondInsideSquareLocations.Count + outsideSquareLocations.Count} locations in {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds}ms)");
			}

			if (shouldTestRandomLocations)
			{
				if (shouldTestDebugLocations)
				{
					sw.Start();
				}

				Console.WriteLine("\n\nRANDOMLY GENERATED LOCATIONS");
				foreach (Vector2 test in randomLocations)
				{
					bool result = IsWithinFlowerRange(test, flower);

					if (result)
					{
						ins.Add($"{test}");
					}
					else
					{
						outs.Add($"{test}");
					}
				}
				sw.Stop();

				Console.WriteLine($"Tested {randomLocations.Count} randomly generated locations in {sw.ElapsedTicks} ticks ({sw.ElapsedMilliseconds}ms)");
				Console.WriteLine($"Ins: {ins.Count} | Outs: {outs.Count}");
			}
		}
	}
}
