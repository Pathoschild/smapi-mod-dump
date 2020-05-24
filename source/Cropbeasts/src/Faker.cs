using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;

namespace Cropbeasts
{
#if DEBUG
	internal static class Faker
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;

		public static void fake (string harvestName, bool giantCrop, uint count,
			Farmer near = null)
		{
			// Check preconditions.
			if (!Context.IsWorldReady)
				throw new Exception ("The world is not ready.");
			if (!Context.IsMainPlayer)
				throw new InvalidOperationException ("Only the host can do that.");

			if (harvestName == "all")
			{
				// Necessarily ignore the count for "all".
				foreach (Mapping mapping in Mappings.GetAll ())
				{
					if (mapping.available)
						fakeOne (mapping.harvestIndex, mapping.giantCrop, near);
				}
				return;
			}

			int harvestIndex;
			if (tryParseHarvestItem (harvestName, out harvestIndex) &&
				Mappings.Get (harvestIndex, giantCrop) != null)
			{
				for (uint i = 0; i < count; ++i)
					fakeOne (harvestIndex, giantCrop, near);
				return;
			}

			string beastName =
				Utility.fuzzySearch (harvestName, Assets.MonsterEditor.List ());
			if (beastName != null)
			{
				var mappings = Mappings.GetForBeast (beastName)
					.Where ((mapping) => mapping.available).ToArray ();
				if (count == 1u)
				{
					foreach (Mapping mapping in mappings)
						fakeOne (mapping.harvestIndex, mapping.giantCrop, near);
				}
				else
				{
					for (uint i = 0; i < count; ++i)
					{
						var mapping = mappings[i % mappings.Length];
						fakeOne (mapping.harvestIndex, mapping.giantCrop, near);
					}
				}
				return;
			}

			Monitor.Log ($"Harvest \"{harvestName}\" not recognized for fake_cropbeast.", LogLevel.Error);
		}

		private static void fakeOne (int harvestIndex, bool giantCrop,
			Farmer near = null)
		{
			near ??= Game1.player;
			GameLocation location = near.currentLocation;

			var tiles = Utility.recursiveFindOpenTiles (location,
				near.getTileLocation (), 1, 100);
			if (tiles.Count == 0)
				throw new Exception ($"Could not find an open tile in {location.Name}.");
			Vector2 tileLocation = tiles[0];

			Crop crop = Utilities.MakeNonceCrop (harvestIndex,
				Utility.Vector2ToPoint (tileLocation));

			TerrainFeature feature;
			if (giantCrop)
			{
				if (location is Farm farm)
				{
					feature = new GiantCrop (harvestIndex, tileLocation);
					farm.resourceClumps.Add (feature as ResourceClump);
				}
				else
				{
					throw new Exception ($"Cannot fake a Giant Cropbeast in {location.Name}.");
				}
			}
			else
			{
				feature = new HoeDirt (0, crop);
				location.terrainFeatures.Add (tileLocation, feature);
			}

			CropTile cropTile = new CropTile (feature, crop, giantCrop, location,
				Utility.Vector2ToPoint (tileLocation), fake: true);
			if (cropTile.state != CropTile.State.Available)
			{
				cropTile.cleanUpFake ();
				throw new Exception ($"Faked a {cropTile.logDescription} but it had no available cropbeast mapping.");
			}

			cropTile.choose ();
			DelayedAction.functionAfterDelay (() => Spawner.Spawn (cropTile), 0);
		}

		private static bool tryParseHarvestItem (string harvest, out int harvestIndex)
		{
			Item harvestItem = Utility.fuzzyItemSearch
				(harvest.Replace ('_', ' '));
			if (harvestItem != null)
			{
				harvestIndex = harvestItem.ParentSheetIndex;
				return true;
			}
			else
			{
				harvestIndex = -1;
				return false;
			}
		}
	}
#endif
}
