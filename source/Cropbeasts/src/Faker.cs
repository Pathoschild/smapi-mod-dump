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
using StardewValley.TerrainFeatures;
using System;
using System.Linq;

namespace Cropbeasts
{
	internal static class Faker
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		// private static ModConfig Config => ModConfig.Instance;

		public static void Fake (string harvestName, bool giantCrop, uint count,
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
						FakeOne (mapping.harvestIndex, mapping.giantCrop, near);
				}
				return;
			}

			if (TryParseHarvestItem (harvestName, out int harvestIndex) &&
				Mappings.Get (harvestIndex, giantCrop) != null)
			{
				for (uint i = 0; i < count; ++i)
					FakeOne (harvestIndex, giantCrop, near);
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
						FakeOne (mapping.harvestIndex, mapping.giantCrop, near);
				}
				else
				{
					for (uint i = 0; i < count; ++i)
					{
						var mapping = mappings[i % mappings.Length];
						FakeOne (mapping.harvestIndex, mapping.giantCrop, near);
					}
				}
				return;
			}

			Monitor.Log ($"Harvest \"{harvestName}\" not recognized for fake_cropbeast.", LogLevel.Error);
		}

		private static void FakeOne (int harvestIndex, bool giantCrop,
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
				feature = new GiantCrop (harvestIndex, tileLocation);
				location.resourceClumps.Add (feature as ResourceClump);
			}
			else
			{
				feature = new HoeDirt (0, crop);
				location.terrainFeatures.Add (tileLocation, feature);
			}

			CropTile cropTile = new (feature, crop, giantCrop, location,
				Utility.Vector2ToPoint (tileLocation), fake: true);
			if (cropTile.state != CropTile.State.Available)
			{
				cropTile.cleanUpFake ();
				throw new Exception ($"Faked a {cropTile.logDescription} but it had no available cropbeast mapping.");
			}

			cropTile.choose ();
			DelayedAction.functionAfterDelay (() => Spawner.Spawn (cropTile), 0);
		}

		private static bool TryParseHarvestItem (string harvest, out int harvestIndex)
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
}
