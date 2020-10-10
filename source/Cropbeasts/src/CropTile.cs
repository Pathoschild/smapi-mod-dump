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
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cropbeasts
{
	public class CropTile : GuestCropTile
	{
		public enum State
		{
			Unavailable,
			Available,
			Chosen,
			Spawned,
			PlantRestored,
			FullyRestored,
		}

		public readonly TerrainFeature feature;
		public readonly GameLocation location;
		public readonly int fertilizer;

		public readonly int baseQuality;
		public readonly int baseQuantity;

		internal readonly Random rng;

		public State state { get; protected set; }
		internal readonly bool fake;

		internal CropTile (TerrainFeature feature, Crop crop,
			bool giantCrop = false, GameLocation location = null,
			Point? tileLocation = null, bool fake = false)
		: base (crop, giantCrop,
			tileLocation ?? Utility.Vector2ToPoint (feature.currentTileLocation))
		{
			this.feature = feature;
			this.location = location ?? feature.currentLocation;
			this.fake = fake;

			if (feature is HoeDirt hd)
				fertilizer = hd.fertilizer.Value;

			// Seed the RNG and calculate the base quality and quantity.
			rng = createRandom ();
			baseQuality = calculateQuality ();
			baseQuantity = calculateQuantity ();

			// Set the initial state depending on suitability.
			if (readyForHarvest && (mapping?.available ?? false))
				state = State.Available;
			else
				state = State.Unavailable;
		}

		public bool readyForHarvest =>
			giantCrop || (feature is HoeDirt hd && hd.readyForHarvest ());

		public override string logDescription =>
			$"{mapping?.harvestName ?? feature.GetType ().Name} (#{harvestIndex}) at ({tileLocation.X},{tileLocation.Y}) on {location.Name}";

		public void choose ()
		{
			if (state != State.Available)
				throw new InvalidOperationException ($"Cannot choose a CropTile in {state} state.");
			state = State.Chosen;
			mapping.choose ();
		}

		public void spawn ()
		{
			if (state != State.Chosen)
				throw new InvalidOperationException ($"Cannot spawn a CropTile in {state} state.");
			state = State.Spawned;

			// Load now before these changes make it incorrect.
			if (!fake)
				loadCropTexture ();

			if (giantCrop)
			{
				(location as Farm).resourceClumps.Remove
					(feature as ResourceClump);
			}
			else
			{
				(feature as HoeDirt).destroyCrop
					(Utility.PointToVector2 (tileLocation), false, location);

				if (fake)
				{
					location.terrainFeatures.Remove
						(Utility.PointToVector2 (tileLocation));
				}

				if (repeatingCrop)
				{
					crop.fullyGrown.Value = true;
					crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
				}
			}
		}

		public void restorePlant ()
		{
			if (state == State.PlantRestored || state == State.FullyRestored)
				return;
			if (state != State.Spawned)
				throw new InvalidOperationException ($"Cannot restore the plant for a CropTile in {state} state.");
			if (!repeatingCrop)
				throw new InvalidOperationException ($"Cannot restore the plant for a non-repeating CropTile.");
			state = State.PlantRestored;

			if (fake)
				return;

			(feature as HoeDirt).crop = crop;
			(feature as HoeDirt).paddyWaterCheck
				(location, Utility.PointToVector2 (tileLocation));
			crop.updateDrawMath (Utility.PointToVector2 (tileLocation));
		}

		public void restoreFully ()
		{
			if (state == State.FullyRestored)
				return;
			if (state != State.Spawned && state != State.PlantRestored)
				throw new InvalidOperationException ($"Cannot fully restore a CropTile in {state} state.");
			state = State.FullyRestored;

			if (fake)
				return;

			if (giantCrop)
			{
				GiantCrop gc = feature as GiantCrop;
				Helper.Reflection.GetField<NetVector2> (gc, "tile").SetValue
					(new NetVector2 (Utility.PointToVector2 (tileLocation)));
				(location as Farm).resourceClumps.Add (gc);
			}
			else
			{
				(feature as HoeDirt).crop = crop;
				(feature as HoeDirt).paddyWaterCheck
					(location, Utility.PointToVector2 (tileLocation));
				if (repeatingCrop)
				{
					crop.fullyGrown.Value = false;
					crop.dayOfCurrentPhase.Value = 0;
				}
				crop.updateDrawMath (Utility.PointToVector2 (tileLocation));
			}
		}

		public void cleanUpFake ()
		{
			if (!fake)
				throw new InvalidOperationException ($"Cannot clean up a non-fake CropTile.");
			if (state != State.Unavailable)
				throw new InvalidOperationException ($"Cannot clean up a CropTile in {state} state.");
			state = State.Chosen;
			spawn ();
			state = State.Unavailable;
		}

		// Lists all the tiles in the given location with crops on them,
		// whether or not they are cropbeast candidates.
		public static List<CropTile> FindAll (GameLocation location)
		{
			List<CropTile> tiles = new List<CropTile> ();

			foreach (TerrainFeature feature in location.terrainFeatures.Values)
			{
				if (feature is HoeDirt hd && hd.crop != null)
					tiles.Add (new CropTile (feature, hd.crop));
			}

			if (location is Farm farm)
			{
				foreach (ResourceClump clump in farm.resourceClumps)
				{
					if (clump is GiantCrop gc)
					{
						Point tileLocation = Utility.Vector2ToPoint (gc.tile.Value);
						Crop nonce = Utilities.MakeNonceCrop
							(gc.parentSheetIndex.Value, tileLocation);
						tiles.Add (new CropTile (gc, nonce, true, farm,
							tileLocation));
					}
				}
			}

			return tiles;
		}

		// Counts the crops in the given (else current) location, whether or not
		// they are cropbeast candidates. Giant crops count for nine regular crops.
		public static int CountAll (GameLocation location)
		{
			List<CropTile> crops = FindAll (location);
			return crops.Count + 8 * crops.Where ((tile) => tile.giantCrop).Count ();
		}

		internal static readonly Dictionary<int, double> Fertilizers =
			new Dictionary<int, double> { { 368, 1.0 }, { 369, 2.0 } };

		private Random createRandom ()
		{
			int gameDay = (int) Game1.uniqueIDForThisGame +
				(int) Game1.stats.DaysPlayed;
			return new Random (tileLocation.X * 7 + tileLocation.Y * 11 + gameDay);
		}

		private int calculateQuality ()
		{
			// Use the average farming skill of all players present, since no
			// specific player is performing a harvest.
			double farmingLevel = Game1.player.team.AverageSkillLevel
				(Farmer.farmingSkill, location);

			Fertilizers.TryGetValue (fertilizer, out double fertilizerBoost);

			double goldChance = 0.2 * (farmingLevel / 10.0) +
				0.2 * fertilizerBoost * ((farmingLevel + 2.0) / 12.0) + 0.01;
			double silverChance = Math.Min (0.75, goldChance * 2.0);

			if (rng.NextDouble () < goldChance)
				return 2;
			else if (rng.NextDouble () < silverChance)
				return 1;
			else
				return 0;
		}

		private int calculateQuantity ()
		{
			if (giantCrop)
				return rng.Next (15, 22);

			if (crop == null)
				return 1;

			int quantity = 1;

			// Crop-specific quantity range, including farming skill bonus.
			if (crop.minHarvest.Value > 1 || crop.maxHarvest.Value > 1)
			{
				int skillBonus = 0;
				if (crop.maxHarvestIncreasePerFarmingLevel.Value > 0)
				{
					double farmingLevel = Game1.player.team.AverageSkillLevel
						(Farmer.farmingSkill, location);
					skillBonus = (int) farmingLevel /
						crop.maxHarvestIncreasePerFarmingLevel.Value;
				}
				quantity = rng.Next (crop.minHarvest.Value,
					Math.Max (crop.minHarvest.Value + 1,
						crop.maxHarvest.Value + 1 + skillBonus));
			}

			// Crop-specific chance for extra crops.
			if (crop.chanceForExtraCrops.Value > 0.0)
			{
				while (rng.NextDouble () < Math.Min (0.9,
						crop.chanceForExtraCrops.Value))
					++quantity;
			}

			// Low luck-based chance of double harvest, buffed here by the
			// average combat skill of players present.
			if (crop.harvestMethod.Value != 1 && rng.NextDouble () <
					Game1.player.team.AverageLuckLevel () / 1500.0 +
					Game1.player.team.AverageDailyLuck () / 1200.0 +
					Game1.player.team.AverageSkillLevel (Farmer.combatSkill,
						location) / 1000.0 +
					0.001)
				quantity *= 2;

			return quantity;
		}
	}
}
