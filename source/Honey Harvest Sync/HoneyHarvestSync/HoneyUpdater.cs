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
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyHarvestSync
{
	public static class HoneyUpdater
	{
		// Tracking collections for bee houses and flowers (or flower equivalents) nearby them that we refresh each day.

		private static readonly Dictionary<string, HashSet<SObject>> beeHousesReady = new();
		private static readonly Dictionary<string, HashSet<SObject>> beeHousesReadyToday = new();

		private static readonly Dictionary<string, HashSet<HoeDirt>> nearbyFlowerDirt = new();
		
		// For tracking modded honey-flavor sources
		private static readonly Dictionary<string, HashSet<FruitTree>> nearbyFruitTrees = new();
		private static readonly Dictionary<string, HashSet<Bush>> nearbyBushes = new();
		private static readonly Dictionary<string, HashSet<IndoorPot>> nearbyBushIndoorPots = new();
		private static readonly Dictionary<string, HashSet<IndoorPot>> nearbyForageIndoorPots = new();
		private static readonly Dictionary<string, HashSet<SObject>> nearbyForageObjects = new();
		private static readonly Dictionary<string, HashSet<GiantCrop>> nearbyGiantCrops = new();

		// NOTE ON THINGS IN LOCATIONS' LISTS
		// `GameLocation.OnTerrainFeatureRemoved` sets the feature's Location property to `null`, so unless we track the location ourselves
		// or get it from a SMAPI event, we won't know the location it was removed from.
		// `GameLocation.OnResourceClumpRemoved` sets the clump's Location property to `null`, so same issue, except that SMAPI doesn't have an event
		// for `GameLocation.resourceClumps` being modified, so we must track it ourselves.

		/// <summary>Shorthand for the main logger instance.</summary>
		private static IMonitor Logger
		{
			get { return ModEntry.Logger; }
		}

		// Shorthand method for creating a standard log entry.
		private static void Log(string message) => Logger.Log(message, Constants.buildLogLevel);

		// Shorthand property for creating a verbose log entry header.
		// We want to use the verbose log method directly for best performance, both when actually using verbose and not.
		private static string VerboseStart
		{
			// Show microsecond, so we can tell if something is slow.
			get { return Logger.IsVerbose ? DateTime.Now.ToString("ffffff") : String.Empty; }
		}

		internal static string ModDataKey_BeeHouseReadyTempDisplayObject
		{
			get { return $"{ModEntry.Context.ModManifest.UniqueID}_BeeHouseReadyTempDisplayObject"; }
		}

		/// <summary>Event handler for after a new day starts.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			Logger.VerboseLog($"{VerboseStart} {nameof(OnDayStarted)} - Started");

			// Refresh everything - our tracked bee houses and our honey-flavor sources - for the new day
			RefreshAll();			

			Logger.VerboseLog($"{VerboseStart} {nameof(OnDayStarted)} - Ended");
		}

		/// <summary>Event handler for when the in-game clock changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnTimeChanged(object sender, TimeChangedEventArgs e)
		{
			// We don't need to day anything right when we wake up, since that's handled by `OnDayStarted`,
			// and we don't want to have any race conditions with it, either.
			if (e.NewTime == Constants.startOfDayTime)
			{
				return;
			}

			foreach (KeyValuePair<string, HashSet<SObject>> entry in beeHousesReadyToday)
			{
				HashSet<SObject> newlyReadyBeeHouses = entry.Value.Where(x => x.readyForHarvest.Value).ToHashSet();

				if (newlyReadyBeeHouses.Count == 0)
				{
					continue;
				}

				Log($"{nameof(OnTimeChanged)} - Found {newlyReadyBeeHouses.Count} newly ready bee houses @ {entry.Key} location");

				GameLocation location = FetchLocationByName(entry.Key);

				if (location != null)
				{
					UpdateLocationBeeHouses(location, newlyReadyBeeHouses);

					if (!beeHousesReady.ContainsKey(entry.Key))
					{
						beeHousesReady.Add(entry.Key, new HashSet<SObject>());
					}

					beeHousesReady[entry.Key].AddRange(newlyReadyBeeHouses);
				}

				beeHousesReadyToday[entry.Key].RemoveWhere(newlyReadyBeeHouses.Contains);
			}
		}

		/// <summary>Event handler for after the game state is updated, once per second.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
		{
			// Every X seconds, refresh everything if we found that another mod has changed settings we care about.
			if (e.IsMultipleOf(5 * 60) && ModEntry.Compat.DidCompatModConfigChange())
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Doing a full refresh because another mod has updated config values we care about.");

				RefreshAll();

				return;
			}

			// Collect all tiles to update around for each location
			Dictionary<string, HashSet<Vector2>> locationTilesToUpdateAround = new();
						
			int croplessFlowerDirtCount = 0;
			
			foreach (KeyValuePair<string, HashSet<HoeDirt>> nearbyDirtEntry in nearbyFlowerDirt)
			{
				// Check if flowers that would affect the honey produced by nearby bee houses have been harvested.
				HashSet<HoeDirt> cropless = nearbyDirtEntry.Value.Where(x => x.crop == null).ToHashSet();

				if (cropless.Count > 0)
				{
					croplessFlowerDirtCount += cropless.Count;

					if (!locationTilesToUpdateAround.ContainsKey(nearbyDirtEntry.Key))
					{
						locationTilesToUpdateAround.Add(nearbyDirtEntry.Key, new());
					}

					// Hold onto where in the GameLocation we need to update near
					locationTilesToUpdateAround[nearbyDirtEntry.Key].AddRange(cropless.Select(x => x.Tile));

					// Remove the flower dirt(s) from being tracked
					nearbyDirtEntry.Value.RemoveWhere(cropless.Contains);

					Logger.VerboseLog($"{VerboseStart} {nameof(OnOneSecondUpdateTicked)} - "
						+ $"Harvested flowers:\n\t{nearbyDirtEntry.Key} @ [{String.Join(", ", cropless.Select(y => y.Tile))}]");
				}
			}
			
			if (croplessFlowerDirtCount > 0)
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {croplessFlowerDirtCount} harvested flowers.");
			}
			else if (croplessFlowerDirtCount == 0 && !ModEntry.Compat.ShouldTrackNonDirtCrops)
			{
				return;
			}

			int fruitlessFruitTreesCount = 0;

			foreach (KeyValuePair<string, HashSet<FruitTree>> nearbyFruitTreeEntry in nearbyFruitTrees)
			{
				// Check if fruit trees that would affect the honey produced by nearby bee houses have been harvested of all their fruit.
				HashSet<FruitTree> fruitless = nearbyFruitTreeEntry.Value.Where(x => (x.fruit?.Count ?? 0) == 0).ToHashSet();

				if (fruitless.Count > 0)
				{
					fruitlessFruitTreesCount += fruitless.Count;

					if (!locationTilesToUpdateAround.ContainsKey(nearbyFruitTreeEntry.Key))
					{
						locationTilesToUpdateAround.Add(nearbyFruitTreeEntry.Key, new());
					}

					// Hold onto where in the GameLocation we need to update near
					locationTilesToUpdateAround[nearbyFruitTreeEntry.Key].AddRange(fruitless.Select(x => x.Tile));

					// Remove the fruit tree(s) from being tracked
					nearbyFruitTreeEntry.Value.RemoveWhere(fruitless.Contains);

					Logger.VerboseLog($"{VerboseStart} {nameof(OnOneSecondUpdateTicked)} - "
						+ $"Harvested fruit trees:\n\t{nearbyFruitTreeEntry.Key} @ [{String.Join(", ", fruitless.Select(y => y.Tile))}]");
				}
			}
			
			if (fruitlessFruitTreesCount > 0)
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {fruitlessFruitTreesCount} harvested fruit trees.");
			}


			// TODO LATER - After SD v1.6.9+ vv


			/*
			 * TEMP - Since `Bush.inBloom()` doesn't change throughout the day, we don't need to run this until BB and us switch to SD v1.6.9+ `Bush.canHarvest`.
			 *		  See all the notes around the 3 variants of initializeding `emptyBushes` in the `foreach` loop for details.
			 *


			int emptyBushesCount = 0;

			foreach (KeyValuePair<string, HashSet<Bush>> nearbyBushEntry in nearbyBushes)
			{
				// Check if bushes that would affect the honey produced by nearby bee houses have been shaken of all their harvestables.
				// In `Bush.shake`, an item debris (AKA a shaken-off item) is only allowed to be created when `this.tileSheetOffset.Value == 1` (AKA the bush is showing
				// its "has fruit/flowers/etc to harvest" sprite), and `this.tileSheetOffset.Value` is then set to `0` upon being shaken/harvested.
				// In the 'Custom Bush' framework mod (which is likely what is being used for any custom bushes), it appears the section of `Bush.shake` that checks
				// and then assigns to `this.tileSheetOffset.Value` is not altered by any of its patches.
				// Also, `__instance.tileSheetOffset.Value` is referenced in its `Bush_setUpSourceRect_postfix` patch, so it appears the value is still used in a "vanilla" way
				// for determining whether the bush is harvestable or not.
				// Ref: https://github.com/LeFauxMatt/StardewMods/blob/develop/CustomBush/Framework/Services/ModPatches.cs
				//HashSet<Bush> emptyBushes = nearbyBushEntry.Value.Where(x => x.tileSheetOffset.Value == 0).ToHashSet();
				// NOTE - The above is not what BB is currently using to determine if a bush is a honey source or not, so replacing for now with the below.

				// Better Beehouses v2.1.1 only checks if the bush is "in season to produce items", rather than whether or not it currently has any.
				// NOTE - For testing this in-game, just don't shake any bushes, since it won't change anything in terms of flavoring nearby bee houses (unless
				// it's a new month [and Matt has merged my PR to fix EOM cached-item-clearing]).
				HashSet<Bush> emptyBushes = nearbyBushEntry.Value.Where(x => x.inBloom()).ToHashSet();

				//HashSet<Bush> emptyBushes = nearbyBushEntry.Value.Where(x => !x.canHarvest).ToHashSet();
				// NOTE - Stardew Valley v1.6.9 will support `Bush.canHarvest` (thanks Pathoschild!), and Better Beehouses will switch to referencing it to calc eligible bushes once it's out.

				if (emptyBushes.Count > 0)
				{
					emptyBushesCount += emptyBushes.Count;

					if (!locationTilesToUpdateAround.ContainsKey(nearbyBushEntry.Key))
					{
						locationTilesToUpdateAround.Add(nearbyBushEntry.Key, new());
					}

					// Hold onto where in the GameLocation we need to update near
					locationTilesToUpdateAround[nearbyBushEntry.Key].AddRange(emptyBushes.Select(x => x.Tile));

					// Remove the bush(es) from being tracked
					nearbyBushEntry.Value.RemoveWhere(emptyBushes.Contains);

					Logger.VerboseLog($"{GetVerboseStart} {nameof(OnOneSecondUpdateTicked)} - "
						+ $"Harvested bushes:\n\t{nearbyBushEntry.Key} @ [{String.Join(", ", emptyBushes.Select(y => y.Tile))}]");
				}
			}

			if (emptyBushesCount > 0)
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {emptyBushesCount} harvested bushes.");
			}


			*/


			int invalidBushPotsCount = 0;

			foreach (KeyValuePair<string, HashSet<IndoorPot>> nearbyBushPotEntry in nearbyBushIndoorPots)
			{
				HashSet<IndoorPot> invalidBushPots = nearbyBushPotEntry.Value.Where(x => x.bush.Value == null).ToHashSet();
				//HashSet<IndoorPot> invalidBushPots = nearbyBushPotEntry.Value.Where(x => x.bush.Value == null || !x.bush.Value.canHarvest).ToHashSet();
				// NOTE - Stardew Valley v1.6.9 will support `Bush.canHarvest` (thanks Pathoschild!), and Better Beehouses will switch to referencing it
				// to calc eligible bushes once it's out, so at that point we'll start checking that the bush hasn't been harvested, too.


				// TODO LATER - After SD v1.6.9+ ^^


				if (invalidBushPots.Count > 0)
				{
					invalidBushPotsCount += invalidBushPots.Count;

					if (!locationTilesToUpdateAround.ContainsKey(nearbyBushPotEntry.Key))
					{
						locationTilesToUpdateAround.Add(nearbyBushPotEntry.Key, new());
					}

					// Hold onto where in the GameLocation we need to update near
					locationTilesToUpdateAround[nearbyBushPotEntry.Key].AddRange(invalidBushPots.Select(x => x.TileLocation));

					// Remove the bush pot(s) from being tracked
					nearbyBushPotEntry.Value.RemoveWhere(invalidBushPots.Contains);

					Logger.VerboseLog($"{VerboseStart} {nameof(OnOneSecondUpdateTicked)} - "
						+ $"Harvested or removed garden pot bushes:\n\t{nearbyBushPotEntry.Key} @ [{String.Join(", ", invalidBushPots.Select(y => y.TileLocation))}]");
				}
			}

			if (invalidBushPotsCount > 0)
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {invalidBushPotsCount} harvested or removed garden pot bushes.");
			}

			int foragelessIndoorPotsCount = 0;

			foreach (KeyValuePair<string, HashSet<IndoorPot>> nearbyPotEntry in nearbyForageIndoorPots)
			{
				// Check if indoor pot has had its held forage item that would affect the honey produced by nearby bee houses collected.
				HashSet<IndoorPot> forageless = nearbyPotEntry.Value.Where(x => x.heldObject.Value == null).ToHashSet();

				if (forageless.Count > 0)
				{
					foragelessIndoorPotsCount += forageless.Count;

					if (!locationTilesToUpdateAround.ContainsKey(nearbyPotEntry.Key))
					{
						locationTilesToUpdateAround.Add(nearbyPotEntry.Key, new());
					}

					// Hold onto where in the GameLocation we need to update near
					locationTilesToUpdateAround[nearbyPotEntry.Key].AddRange(forageless.Select(x => x.TileLocation));

					// Remove the indoor pot(s) from being tracked
					nearbyPotEntry.Value.RemoveWhere(forageless.Contains);

					Logger.VerboseLog($"{VerboseStart} {nameof(OnOneSecondUpdateTicked)} - "
						+ $"Harvested (of forage) indoor pots:\n\t{nearbyPotEntry.Key} @ [{String.Join(", ", forageless.Select(y => y.TileLocation))}]");
				}
			}

			if (foragelessIndoorPotsCount > 0)
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {foragelessIndoorPotsCount} pots with no forage item in them.");
			}

			int goneGiantCropsCount = 0;

			foreach (KeyValuePair<string, HashSet<GiantCrop>> nearbyGiantCropEntry in nearbyGiantCrops)
			{
				GameLocation giantCropLocation = FetchLocationByName(nearbyGiantCropEntry.Key);

				if (giantCropLocation == null)
				{
					Logger.LogOnce($"Unable to check if any of {nearbyGiantCropEntry.Value.Count} giant crops were harvested at this location.", LogLevel.Info);

					nearbyGiantCrops.Remove(nearbyGiantCropEntry.Key);

					continue;
				}

				// Check if giant crop that would affect the honey produced by nearby bee houses is still in its location's list of resource clumps. If not, it's been removed.
				HashSet<GiantCrop> gone = nearbyGiantCropEntry.Value.Where(x => !giantCropLocation.resourceClumps.Contains(x)).ToHashSet();

				if (gone.Count > 0)
				{
					goneGiantCropsCount += gone.Count;

					if (!locationTilesToUpdateAround.ContainsKey(nearbyGiantCropEntry.Key))
					{
						locationTilesToUpdateAround.Add(nearbyGiantCropEntry.Key, new());
					}

					// Hold onto where in the GameLocation we need to update near
					locationTilesToUpdateAround[nearbyGiantCropEntry.Key].AddRange(gone.Select(x => x.Tile));

					// Remove the giant crop(s) from being tracked
					nearbyGiantCropEntry.Value.RemoveWhere(gone.Contains);

					Logger.VerboseLog($"{VerboseStart} {nameof(OnOneSecondUpdateTicked)} - "
						+ $"Harvested giant crops:\n\t{nearbyGiantCropEntry.Key} @ [{String.Join(", ", gone.Select(y => y.Tile))}]");
				}
			}

			if (goneGiantCropsCount > 0)
			{
				Log($"{nameof(OnOneSecondUpdateTicked)} - Found {goneGiantCropsCount} giant crops that are gone.");
			}

			// Now make a single pass through each location to update all the tiles we collected by collecting all the bee houses near all the tiles before processing a location.
			UpdateBeeHousesNearLocationTiles(locationTilesToUpdateAround);
		}

		/// <summary>
		/// Event handler for after objects are added/removed in any location (including machines, fences, etc).
		/// This doesn't apply for floating items (see `DebrisListChanged`) or furniture (see `FurnitureListChanged`).
		/// This event isn't raised for objects already present when a location is added. If you need to handle those too, use `LocationListChanged` and check `e.Added → objects`.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
		{
			string locationName = e.Location.NameOrUniqueName;

			// Check the removed objects for bee houses
			if (e.Removed.Any(x => x.Value.QualifiedItemId == Constants.beeHouseQualifiedItemID))
			{
				// Find all removed bee houses so we can remove them from our tracking dictionaries
				IEnumerable<SObject> removedBeeHouses = e.Removed.Select(y => y.Value).Where(z => z.QualifiedItemId == Constants.beeHouseQualifiedItemID);

				Log($"{nameof(OnObjectListChanged)} - Found {removedBeeHouses.Count()} bee houses to attempt to remove from tracking at '{locationName}' location");

				if (beeHousesReady.ContainsKey(locationName) && beeHousesReady[locationName].Any(removedBeeHouses.Contains))
				{
					beeHousesReady[locationName].RemoveWhere(removedBeeHouses.Contains);
					Logger.VerboseLog($"{VerboseStart} {nameof(OnObjectListChanged)} - {e.Location} location has {beeHousesReady[locationName].Count} remaining tracked ready bee houses");
				}

				if (beeHousesReadyToday.ContainsKey(locationName) && beeHousesReadyToday[locationName].Any(removedBeeHouses.Contains))
				{
					beeHousesReadyToday[locationName].RemoveWhere(removedBeeHouses.Contains);
					Logger.VerboseLog($"{VerboseStart} {nameof(OnObjectListChanged)} - {e.Location} location has {beeHousesReadyToday[locationName].Count} remaining tracked ready-today bee houses");
				}
			}

			if (!ModEntry.Compat.ShouldTrackNonDirtCrops)
			{
				return;
			}

			// Collect all tiles to update around for this location
			HashSet<Vector2> updateNearTiles = new();

			// When BB installed - Have to check our list of forage-holding and bush-hosting pots to see if one of those was removed
			if (e.Removed.Any(x => x.Value.QualifiedItemId == Constants.gardenPotQualifiedItemID)
				&& (nearbyForageIndoorPots.ContainsKey(locationName) || nearbyBushIndoorPots.ContainsKey(locationName)))
			{
				IEnumerable<IndoorPot> removedIndoorPots = e.Removed.Select(x => x.Value as IndoorPot).Where(x => x is not null && x.QualifiedItemId == Constants.gardenPotQualifiedItemID);

				Log($"{nameof(OnObjectListChanged)} - Found {removedIndoorPots.Count()} garden pots to attempt to remove from tracking at '{locationName}' location");

				if (removedIndoorPots.Any())
				{
					IEnumerable<IndoorPot> removedLocationForagePots = nearbyForageIndoorPots[locationName].Where(removedIndoorPots.Contains);
					IEnumerable<IndoorPot> removedLocationBushPots = nearbyBushIndoorPots[locationName].Where(removedIndoorPots.Contains);

					if (removedLocationForagePots.Any())
					{
						Log($"{nameof(OnObjectListChanged)} - Found {removedLocationForagePots.Count()} removed forage-holding indoor pots at {locationName} to update bee houses near.");
						Logger.VerboseLog($"{VerboseStart} [{String.Join(", ", removedLocationForagePots.Select(y => y.TileLocation))}]");

						// Hold onto where in the GameLocation we need to update near
						updateNearTiles.AddRange(removedLocationForagePots.Select(x => x.TileLocation));

						// Remove the indoor pot(s) from being tracked
						nearbyForageIndoorPots[locationName].RemoveWhere(removedLocationForagePots.Contains);
					}

					if (removedLocationBushPots.Any())
					{
						Log($"{nameof(OnObjectListChanged)} - Found {removedLocationBushPots.Count()} removed bush-hosting indoor pots at {locationName} to update bee houses near.");
						Logger.VerboseLog($"{VerboseStart} [{String.Join(", ", removedLocationBushPots.Select(y => y.TileLocation))}]");

						// Hold onto where in the GameLocation we need to update near
						updateNearTiles.AddRange(removedLocationBushPots.Select(x => x.TileLocation));

						// Remove the indoor pot(s) from being tracked
						nearbyBushIndoorPots[locationName].RemoveWhere(removedLocationBushPots.Contains);
					}
				}
			}

			// When BB installed - Check our list of bare forage to see if any were removed
			if (e.Removed.Any(x => x.Value.CanBeGrabbed && Utilities.IsHoneyFlavorSource(x.Value)) && nearbyForageObjects.ContainsKey(locationName))
			{
				IEnumerable<SObject> removedForageObjects = e.Removed.Select(x => x.Value).Where(obj => obj.CanBeGrabbed && Utilities.IsHoneyFlavorSource(obj));

				Log($"{nameof(OnObjectListChanged)} - Found {removedForageObjects.Count()} forage objects to attempt to remove from tracking at '{locationName}' location");

				if (removedForageObjects.Any())
				{
					IEnumerable<SObject> removedLocationForage = nearbyForageObjects[locationName].Where(removedForageObjects.Contains);

					Log($"{nameof(OnObjectListChanged)} - Found {removedLocationForage.Count()} harvested bare forage at {locationName} to update bee houses near.");
					Logger.VerboseLog($"{VerboseStart} [{String.Join(", ", removedLocationForage.Select(y => y.TileLocation))}]");

					// Hold onto where in the GameLocation we need to update near
					updateNearTiles.AddRange(removedLocationForage.Select(x => x.TileLocation));

					// Remove the forage(s) from being tracked
					nearbyForageObjects[locationName].RemoveWhere(removedLocationForage.Contains);
				}
			}

			if (updateNearTiles.Count > 0)
			{
				// Now make a single pass through all the tiles we collected by collecting all the bee houses near all the tiles before processing the location.
				UpdateBeeHousesNearLocationTiles(new Dictionary<string, HashSet<Vector2>>() { { locationName, updateNearTiles } });
			}
		}

		/// <summary>Event handler for after a game location is added or removed (including building interiors).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		internal static void OnLocationListChanged(object sender, LocationListChangedEventArgs e)
		{
			foreach (GameLocation addedLocation in e.Added.Where(Utilities.IsLocationWithBeeHouses))
			{
				// If we have the location tracked already, remove all existing tracking before we (re-)add the location
				RemoveLocationFromTracking(addedLocation);

				// Now add the location fresh to our tracking and run any updates we need to do
				AddLocation(addedLocation);
			}

			// Clear any data we are tracking about this location
			foreach (GameLocation removedLocation in e.Removed.Where(x => beeHousesReady.ContainsKey(x.NameOrUniqueName) || beeHousesReadyToday.ContainsKey(x.NameOrUniqueName)))
			{
				RemoveLocationFromTracking(removedLocation);
			}
		}

		/// <summary>
		/// Adds bee houses in the given location to our lists of bee houses.
		/// For "ready" bee houses, will also update the bee houses, which also adds flowers nearby to the bee houses to our tracked list.
		/// </summary>
		/// <param name="location">The location to add to tracking and immediately start tracking thing at.</param>
		private static void AddLocation(GameLocation location)
		{
			HashSet<SObject> ready = location.Objects.Values.Where(x => x.QualifiedItemId == Constants.beeHouseQualifiedItemID && x.readyForHarvest.Value).ToHashSet();
			HashSet<SObject> readyToday = location.Objects.Values.Where(x => x.QualifiedItemId == Constants.beeHouseQualifiedItemID
				&& !x.readyForHarvest.Value && x.MinutesUntilReady <= Constants.maxMinutesAwake).ToHashSet();

			if (ready.Count > 0)
			{
				Log($"{nameof(AddLocation)} - Found {ready.Count} ready bee houses @ {location.NameOrUniqueName} location");

				beeHousesReady.Add(location.NameOrUniqueName, ready);
				UpdateLocationBeeHouses(location, ready);
			}

			if (readyToday.Count > 0)
			{
				Log($"{nameof(AddLocation)} - Found {readyToday.Count} bee houses that will be ready today @ {location.NameOrUniqueName} location");

				beeHousesReadyToday.Add(location.NameOrUniqueName, readyToday);
			}
		}

		/// <summary>Remove anything we're tracking at the given location.</summary>
		/// <param name="location">The location to no longer track anything at.</param>
		private static void RemoveLocationFromTracking(GameLocation location)
		{
			beeHousesReady.Remove(location.NameOrUniqueName);
			beeHousesReadyToday.Remove(location.NameOrUniqueName);
			nearbyFlowerDirt.Remove(location.NameOrUniqueName);

			if (!ModEntry.Compat.ShouldTrackNonDirtCrops)
			{
				return;
			}

			nearbyFruitTrees.Remove(location.NameOrUniqueName);
			nearbyBushes.Remove(location.NameOrUniqueName);
			nearbyBushIndoorPots.Remove(location.NameOrUniqueName);
			nearbyForageIndoorPots.Remove(location.NameOrUniqueName);
			nearbyForageObjects.Remove(location.NameOrUniqueName);
			nearbyGiantCrops.Remove(location.NameOrUniqueName);
		}

		/// <summary>
		/// Refresh the "held object" in all tracked, ready-for-harvest bee houses.
		/// This will refresh the icon shown overtop those bee houses.
		/// This can be used in cases where the bee houses should now be showing a different icon above them
		/// due to another mod's config value being changed, which could/would affect the assigned/shown item.
		/// </summary>
		public static void RefreshBeeHouseHeldObjects()
		{
			Logger.VerboseLog($"{VerboseStart} {nameof(RefreshBeeHouseHeldObjects)} - Started");

			foreach (KeyValuePair<string, HashSet<SObject>> kvp in beeHousesReady)
			{
				GameLocation location = FetchLocationByName(kvp.Key);

				if (location == null)
				{
					continue;
				}

				UpdateLocationBeeHouses(location, kvp.Value);
			}

			Logger.VerboseLog($"{VerboseStart} {nameof(RefreshBeeHouseHeldObjects)} - Ended");
		}

		/// <summary>
		/// This will refresh all tracking - bee houses being tracked as well as their honey flavor sources - across all locations.
		/// This is what runs at the start of each day and should ideally only be run then,
		/// but if everything should be thrown out and re-evaluated for some reason, this will do that.
		/// </summary>
		public static void RefreshAll()
		{
			// Reset our tracked bee houses and honey-flavor sources
			beeHousesReady.Clear();
			beeHousesReadyToday.Clear();
			nearbyFlowerDirt.Clear();

			if (ModEntry.Compat.ShouldTrackNonDirtCrops)
			{
				nearbyFruitTrees.Clear();
				nearbyBushes.Clear();
				nearbyBushIndoorPots.Clear();
				nearbyForageIndoorPots.Clear();
				nearbyForageObjects.Clear();
				nearbyGiantCrops.Clear();
			}

			// Get just locations we care about. Include indoor locations only when needed for mod compatability.
			Utility.ForEachLocation((GameLocation location) => {
				if (Utilities.IsLocationWithBeeHouses(location))
				{
					AddLocation(location);
				}

				return true;
			}, ModEntry.Compat.SyncIndoorBeeHouses);
		}

		/// <summary>
		/// This uses a base game method that handles all of our needs (caching + inside locs), plus will do a `LogOnce` for a location if it can't be found.
		/// We can't really trust a Location property on - for example - a TerrainFeature or ResourceClump since it gets set to `null` when they're removed by the game
		/// from its location's list of them, so we fetch location instances ourselves instead of trying to use an instance's location property.
		/// </summary>
		/// <param name="locationName">The game's name for a location</param>
		/// <returns>The `GameLocation` object if found; `null` if not.</returns>
		private static GameLocation FetchLocationByName(string locationName)
		{
			// This base game method will get from cache where possible and handles locations which are buildings.
			GameLocation location = Game1.getLocationFromName(locationName);

			if (location == null)
			{
				Logger.LogOnce($"Failed to get GameLocation with name '{locationName}'. Will be unable to refresh bee houses in this location.", LogLevel.Warn);
			}

			return location;
		}

		/// <summary>Updates any bee houses nearby each of the tiles in the given location tiles collections.</summary>
		/// <param name="locationTilesToUpdateAround">A collection of tiles that need nearby bee houses to be updated, grouped by their location.</param>
		private static void UpdateBeeHousesNearLocationTiles(Dictionary<string, HashSet<Vector2>> locationTilesToUpdateAround)
		{
			foreach (KeyValuePair<string, HashSet<Vector2>> locationWithTiles in locationTilesToUpdateAround)
			{
				string updateLocationName = locationWithTiles.Key;

				if (!beeHousesReady.ContainsKey(updateLocationName))
				{
					continue;
				}

				HashSet<SObject> beeHousesToUpdate = beeHousesReady[updateLocationName]
					.Where(beeHouse => locationWithTiles.Value.Any(updateAroundTile => Utilities.IsWithinFlowerRange(beeHouse.TileLocation, updateAroundTile)))
					.ToHashSet();

				if (beeHousesToUpdate.Count == 0)
				{
					continue;
				}

				Log($"{nameof(UpdateBeeHousesNearLocationTiles)} - Found {beeHousesToUpdate.Count} ready bee houses that need updating @ {updateLocationName} location.");

				GameLocation updateLocation = FetchLocationByName(updateLocationName);

				if (updateLocation == null)
				{
					Logger.LogOnce($"Unable to update the bee houses that need refreshed at this location.", LogLevel.Info);

					locationTilesToUpdateAround.Remove(updateLocationName);

					continue;
				}

				UpdateLocationBeeHouses(updateLocation, beeHousesToUpdate);

				Logger.VerboseLog($"{VerboseStart} {nameof(UpdateBeeHousesNearLocationTiles)} - Updated bee house details: {String.Join(" | ", beeHousesToUpdate.Select(x => x.TileLocation))}");
			}
		}

		/// <summary>
		/// Updates the honey held by the given ready-for-harvest bee houses, which are at the given location.
		/// This also adds any nearby flowers to our tracked list of them.
		/// </summary>
		/// <param name="location">The location of the ready bee houses.</param>
		/// <param name="readyBeeHouses">The bee houses which are ready to be harvested which we should update the honey of.</param>
		private static void UpdateLocationBeeHouses(GameLocation location, HashSet<SObject> readyBeeHouses)
		{
			Logger.VerboseLog($"{VerboseStart} {nameof(UpdateLocationBeeHouses)} - Started");

			ObjectDataDefinition objectData = ItemRegistry.GetObjectTypeDefinition();
			int flowerRange = ModEntry.Compat.FlowerRange;

			List<SObject> invalidBeeHouses = new();
			int newlyTrackedHoneyFlavorSourceCount = 0;

			foreach (SObject beeHouse in readyBeeHouses)
			{
				// If a bee house no longer qualifies in any way, we'll remove it after we go through the list we were given
				if (beeHouse == null || !beeHouse.readyForHarvest.Value || beeHouse.QualifiedItemId != Constants.beeHouseQualifiedItemID)
				{
					invalidBeeHouses.Add(beeHouse);

					// If the issue is just that the bee house was harvested, then no log entry should be made
					if (beeHouse?.readyForHarvest.Value == false)
					{
						continue;
					}

					Logger.Log($"Found an invalid bee house @ {location} location; removing from tracking: "
						+ $"{(beeHouse == null ? "null" : $"Tile {beeHouse.TileLocation}; RFH {(beeHouse.readyForHarvest.Value ? "Yes" : "No")}; QID {beeHouse.QualifiedItemId}")}", LogLevel.Info);

					continue;
				}

				// Same flower check the game uses (see `MachineDataUtility.GetNearbyFlowerItemId()`) when collecting the honey out of the bee house.
				// Note that if another mod patches this method - such as 'Better Beehouses' - we'll still get a `Crop` back, but it might not be a flower,
				// and/or it might not be in a standard HoeDirt instance.
				Crop closeFlower = Utility.findCloseFlower(location, beeHouse.TileLocation, flowerRange, (Crop crop) => !crop.forageCrop.Value);
				SObject flowerIngredient = null;
				string flowerHarvestName = String.Empty;

				// If we found a qualifying flower crop with an assigned harvest item, then get its harvested object form.
				if (closeFlower?.indexOfHarvest?.Value != null)
				{
					string flowerIngredientID = ItemRegistry.QualifyItemId(closeFlower.indexOfHarvest.Value);

					if (flowerIngredientID == null)
					{
						Logger.Log($"Failed to get the qualified item ID of a nearby flower from the flower's `indexOfHarvest.Value` value of '{closeFlower.indexOfHarvest.Value}'.", LogLevel.Warn);
					}
					else
					{
						string itemCreationFailureMessage = $"Failed to create an `Item` (and then convert it to `Object`) via `ItemRegistry.Create` "
							+ $"using a nearby flower's qualified item ID of '{flowerIngredientID}'.";

						// `StardewValley.Internal.ItemQueryResolver.ItemQueryResolver.DefaultResolvers.FLAVORED_ITEM()` has this in a `try/catch`, so mimicking that here 
						try
						{
							// If this comes back as `null` or the conversion fails (resulting in `null`), that's fine since we'll just get "Wild Honey" back
							// when we attempt to create flavored honey below.
							flowerIngredient = ItemRegistry.Create(flowerIngredientID, allowNull: true) as SObject;

							if (flowerIngredient == null)
							{
								Logger.Log(itemCreationFailureMessage, LogLevel.Warn);
							}
							else
							{
								flowerHarvestName = flowerIngredient.Name;
							}
						}
						catch (Exception ex)
						{
							Logger.Log(itemCreationFailureMessage + $"\n\nException ({ex.GetType().Name}): {ex.Message}", LogLevel.Error);
						}
					}

					newlyTrackedHoneyFlavorSourceCount += TrackHoneyFlavorSource(closeFlower, location, beeHouse, flowerHarvestName) ? 1 : 0;
				}
				else if (closeFlower != null && closeFlower.indexOfHarvest?.Value == null)
				{
					Logger.Log($"The nearby {(ModEntry.Compat.ShouldTrackNonDirtCrops ? "honey flavor source" : "flower")} "
						+ $"has no harvest item (`indexOfHarvest.Value`) value assigned, which is probably incorrect.", LogLevel.Info);
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

				// Add modData to this item to indicate that it's from this mod and it's just for display
				beeHouse.heldObject.Value.modData[ModDataKey_BeeHouseReadyTempDisplayObject] = "1";

				Logger.VerboseLog($"{VerboseStart} Assigned {beeHouse.heldObject.Value.Name} to bee house @ {beeHouse.TileLocation} tile @ {location.Name} location");
			}

			// Remove any invalid bee houses from the given list
			readyBeeHouses.RemoveWhere(invalidBeeHouses.Contains);

			Log($"{nameof(UpdateLocationBeeHouses)} - Updated {readyBeeHouses.Count} ready bee houses "
				+ (newlyTrackedHoneyFlavorSourceCount > 0 ? $"and now tracking {newlyTrackedHoneyFlavorSourceCount} additional nearby {(ModEntry.Compat.ShouldTrackNonDirtCrops ? "honey flavor sources" : "flowers")}" : String.Empty)
				+ $" @ {location.Name} location");

			Logger.VerboseLog($"{VerboseStart} {nameof(UpdateLocationBeeHouses)} - Ended");
		}

		/// <summary>
		/// Adds the given crop (typically a flower) to our tracking so we can keep bee houses showing their current honey-flavor source (when ready).
		/// </summary>
		/// <param name="crop">The crop to track. Not necessarily a flower when other mods are involved.</param>
		/// <param name="location">The map that the crop is on.</param>
		/// <param name="beeHouse">The bee house the crop is affecting the honey of. Currently only used for logging purposes.</param>
		/// <param name="honeyFlavorSourceHarvestName">The name of the harvest item of the crop, if it was able to be determined. Should be an empty string if not.</param>
		/// <returns>True if we added the crop to tracking. False if the crop was A) already being tracked or B) we couldn't add it to be tracked.</returns>
		private static bool TrackHoneyFlavorSource(Crop crop, GameLocation location, SObject beeHouse, string honeyFlavorSourceHarvestName)
		{
			// Make sure the `Dirt` property is set. Mods that support more than just crops as honey "flavor" sources can return `Crop` instances with minimal properties set on them.
			// Mods like "Better Beehouses" create `Crop` objects for all the non-`Crop` things that they let bee houses use for flavoring honey,
			// and for that mod, they don't set the `Dirt` property (since many of them don't/can't have an associated one).
			// A crop with dirt associated to it can be either in the ground or in an "Garden Pot" AKA `IndoorPot`, now that vanilla SD v1.6.6+ supports flowers in pots flavoring honey, too.
			if (crop.Dirt != null)
			{
				if (!nearbyFlowerDirt.ContainsKey(location.NameOrUniqueName))
				{
					nearbyFlowerDirt.Add(location.NameOrUniqueName, new());
				}

				// Track the tile location of the `HoeDirt` that holds the flower's `Crop` object so we can watch for it being harvested later.
				if (nearbyFlowerDirt[location.NameOrUniqueName].Add(crop.Dirt))
				{
					Logger.VerboseLog($"{VerboseStart} Now tracking nearby grown flower '{honeyFlavorSourceHarvestName}' "
						+ $"via its Dirt with tile {crop.Dirt?.Tile.ToString() ?? "[Dirt has `null` Tile]"}. (Bee House Tile {beeHouse.TileLocation} and {location.NameOrUniqueName} location)");

					return true;
				}

				return false;
			}

			// Check that we should attempt to track modded honey flavor sources, and ensure we have a location to check at.
			if (!ModEntry.Compat.ShouldTrackNonDirtCrops || crop.tilePosition.Equals(default) || crop.tilePosition.Equals(Vector2.Zero))
			{
				Logger.Log($"`Crop` object '{honeyFlavorSourceHarvestName}' is missing required data. "
					+ $"Will be unable to track if it gets harvested. (Bee House Tile {beeHouse.TileLocation} and {location.Name} location)", LogLevel.Debug);

				return false;
			}

			// If we can't track the dirt for when its crop is harvested, we'll have to try to determine what to even track by what's at this "crop" (which isn't a normal crop) location.
			Vector2 searchPosition = crop.tilePosition;

			// Better Beehouses labels the source type so we know what any of its minimally filled-in `Crop` instances represent.
			// BB source types: Crop, Forage, FruitTree, Bush, and GiantCrop
			bool hasSourceType_BB = crop.modData.TryGetValue(ModCompat.betterBeehousesModDataSourceTypeKey, out string sourceType_BB);

			// This key is only set when the thing is in a pot, so we can assume if the key exists that its value is the equivalent of `true` (they set it to "T").
			bool isInPot_BB = crop.modData.ContainsKey(ModCompat.betterBeehousesModDataFromPotKey);

			bool wasFound = false;
			bool wasAdded = false;

			// First we'll track for normal dirt with a crop at the location, in case the `Dirt` property just wasn't set on our copy for whatever reason.
			if (location.terrainFeatures.TryGetValue(searchPosition, out TerrainFeature terrainFeature))
			{
				// If we can get dirt with a crop in it, we can track this like normal crop flowers
				if (terrainFeature is HoeDirt tfDirt && tfDirt.crop != null)
				{
					wasFound = true;

					if (!nearbyFlowerDirt.ContainsKey(location.NameOrUniqueName))
					{
						nearbyFlowerDirt.Add(location.NameOrUniqueName, new());
					}

					// Track the tile location of the `HoeDirt` that holds the flower's `Crop` object so we can watch for it being harvested later.
					if (nearbyFlowerDirt[location.NameOrUniqueName].Add(tfDirt))
					{
						wasAdded = true;
					}
				}
				// Note that BB will provide either fruit trees with flowers as the "fruit" on them
				// or *any* fruit tree if its 'UseAnyFruitTrees' setting is enabled.
				else if (terrainFeature is FruitTree tfFruitTree)
				{
					wasFound = true;

					if (!nearbyFruitTrees.ContainsKey(location.NameOrUniqueName))
					{
						nearbyFruitTrees.Add(location.NameOrUniqueName, new());
					}

					if (nearbyFruitTrees[location.NameOrUniqueName].Add(tfFruitTree))
					{
						wasAdded = true;
					}
				}
				else if (terrainFeature is Bush tfBush)
				{
					wasFound = true;

					if (!nearbyBushes.ContainsKey(location.NameOrUniqueName))
					{
						nearbyBushes.Add(location.NameOrUniqueName, new());
					}

					if (nearbyBushes[location.NameOrUniqueName].Add(tfBush))
					{
						wasAdded = true;
					}
				}
			}

			// Only check the objects list if BB marked the item as in a pot or noted its "type".
			// Note that in the future if we need to support other mods, we could either remove the BB-specific checks in this `if` or add to them.
			// In the meantime we'll prefer to not search the (potentially large) objects list if possible.
			if (!wasFound && (isInPot_BB || hasSourceType_BB) && location.Objects.TryGetValue(searchPosition, out SObject locationObject))
			{
				if (locationObject is IndoorPot objPot)
				{
					// For Better Beehouses, we should get a crop with its dirt associated with it back even for crops in pots,
					// which we would have been handled above already, so this shouldn't be necessary.
					// But it's best to cover all bases, especially if we add support/compat for other mods in the future, so we'll double check here.
					if (objPot.hoeDirt?.Value?.crop != null)
					{
						wasFound = true;

						if (!nearbyFlowerDirt.ContainsKey(location.NameOrUniqueName))
						{
							nearbyFlowerDirt.Add(location.NameOrUniqueName, new());
						}

						if (nearbyFlowerDirt[location.NameOrUniqueName].Add(objPot.hoeDirt.Value))
						{
							wasAdded = true;
						}
					}
					// Check if the pot has a qualifying item in it. Non-crop items can be grabbed, whereas crops stay crops until harvested.
					// For Better Beehouses, this would likely be a forage item, but could be anything with its 'AnythingHoney' config enabled.
					else if (objPot.heldObject.Value?.CanBeGrabbed ?? false)
					{
						wasFound = true;

						if (!nearbyForageIndoorPots.ContainsKey(location.NameOrUniqueName))
						{
							nearbyForageIndoorPots.Add(location.NameOrUniqueName, new());
						}

						if (nearbyForageIndoorPots[location.NameOrUniqueName].Add(objPot))
						{
							wasAdded = true;
						}
					}
					else if (objPot.bush?.Value != null)
					{
						wasFound = true;

						if (!nearbyBushIndoorPots.ContainsKey(location.NameOrUniqueName))
						{
							nearbyBushIndoorPots.Add(location.NameOrUniqueName, new());
						}

						if (nearbyBushIndoorPots[location.NameOrUniqueName].Add(objPot))
						{
							wasAdded = true;
						}
					}
				}
				// Check if it's an item that's just on the ground/floor, i.e. not in a pot.
				// For Better Beehouses, this would likely be a forage item, but could be anything with its 'AnythingHoney' config enabled.
				else if (locationObject.CanBeGrabbed)
				{
					wasFound = true;

					if (!nearbyForageObjects.ContainsKey(location.NameOrUniqueName))
					{
						nearbyForageObjects.Add(location.NameOrUniqueName, new());
					}

					if (nearbyForageObjects[location.NameOrUniqueName].Add(locationObject))
					{
						wasAdded = true;
					}
				}
			}

			// Only do this check if BB marked it as a giant crop.
			if (!wasFound && sourceType_BB == "GiantCrop")
			{
				GiantCrop giantCrop = location.resourceClumps.FirstOrDefault(x => x is GiantCrop && x.Tile == searchPosition) as GiantCrop;

				if (giantCrop != null)
				{
					wasFound = true;

					if (!nearbyGiantCrops.ContainsKey(location.NameOrUniqueName))
					{
						nearbyGiantCrops.Add(location.NameOrUniqueName, new());
					}

					if (nearbyGiantCrops[location.NameOrUniqueName].Add(giantCrop))
					{
						wasAdded = true;
					}
				}
			}

			if (wasAdded)
			{
				Logger.VerboseLog($"{VerboseStart} Now tracking nearby honey-flavor source '{honeyFlavorSourceHarvestName}' "
					+ $"{(hasSourceType_BB ? $"(BB | source type: {sourceType_BB} | harvest ID: {crop.indexOfHarvest.Value}) " : String.Empty)}"
					+ $"{(isInPot_BB ? $"(BB in-pot item) " : String.Empty)}at {searchPosition} tile position. "
					+ $"(Bee House Tile {beeHouse.TileLocation} and {location.Name} location)");
			}
			else if (!wasFound)
			{
				Logger.Log($"`Crop` object '{honeyFlavorSourceHarvestName}' "
					+ $"{(hasSourceType_BB ? $"(BB | source type: {sourceType_BB} | harvest ID: {crop.indexOfHarvest.Value}) " : String.Empty)}"
					+ $"{(isInPot_BB ? $"(BB in-pot item) " : String.Empty)}at {searchPosition} tile position didn't match any known trackable honey-flavoring source. "
					+ $"Will be unable to track if it gets harvested. (Bee House Tile {beeHouse.TileLocation} and {location.Name} location)", LogLevel.Debug);
			}

			return wasAdded;
		}
	}
}
