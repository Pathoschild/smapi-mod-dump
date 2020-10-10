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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace EastScarpe
{
	public class EastScarpe : GameLocation
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModData Data => ModEntry.Instance.Data;

		public EastScarpe ()
		{}

		public EastScarpe (string mapPath, string name)
		: base (mapPath, name)
		{}

		public override void UpdateWhenCurrentLocation (GameTime time)
		{
			if (wasUpdated)
				return;

			base.UpdateWhenCurrentLocation (time);

			// Very rarely show the Sea Monster.
			if (Game1.eventUp || !(Game1.random.NextDouble () < Data.SeaMonsterChance))
				return;

			// Randomly find a starting position within the range.
			Vector2 position = 64f * new Vector2
				(Game1.random.Next (Data.SeaMonsterRange.Left,
					Data.SeaMonsterRange.Right + 1),
				Game1.random.Next (Data.SeaMonsterRange.Top,
					Data.SeaMonsterRange.Bottom + 1));

			// Confirm the monster can swim to the ocean from there.
			bool foundPosition = true;
			int height = map.Layers[0]?.LayerHeight ?? 0;
			for (int y = (int) position.Y / 64; y < height; ++y)
			{
				if (doesTileHaveProperty ((int) position.X / 64, y, "Water", "Back") == null ||
					doesTileHaveProperty ((int) position.X / 64 - 1, y, "Water", "Back") == null ||
					doesTileHaveProperty ((int) position.X / 64 + 1, y, "Water", "Back") == null)
				{
					foundPosition = false;
					break;
				}
			}

			// Spawn if possible.
			if (foundPosition)
			{
				temporarySprites.Add (new SeaMonsterTemporarySprite
					(250f, 4, Game1.random.Next (7), position));
			}
		}

		public override void checkForMusic (GameTime time)
		{
			// Occasionally play a pelican(-ish) sound on the beach side.
			if (Game1.timeOfDay < 1900 &&
					Game1.random.NextDouble () < Data.SeabirdSoundChance &&
					Game1.player.getTileLocation ().Y >= Data.ForestBeachYLine)
				localSound ("seagulls");

			base.checkForMusic (time);
		}

		protected bool isTidepoolTile (int x, int y)
		{
			return Data.TidepoolTiles.Contains (getTileIndexAt (x, y, "Back")) ||
				Data.TidepoolTiles.Contains (getTileIndexAt (x, y, "Buildings"));
		}

		public override int getFishingLocation (Vector2 tileLocation)
		{
			// Try to use the tile where the line has been cast, instead of
			// where the player is standing.
			if (Game1.player.CurrentTool is FishingRod rod &&
					rod.bobber.Value != Vector2.Zero)
				tileLocation = rod.bobber.Value / 64f;

			// Above the Y demarcation line, assume the pond.
			if (tileLocation.Y < Data.ForestBeachYLine)
				return (int) FishingArea.Pond;

			// If a tidepool tile, it's a tidepool.
			if (isTidepoolTile ((int) tileLocation.X, (int) tileLocation.Y))
				return (int) FishingArea.Tidepool;

			// Otherwise it's the ocean.
			return (int) FishingArea.Ocean;
		}

		protected override void resetSharedState ()
		{
			base.resetSharedState ();

			// Make the water green on Summer 12 through Summer 14.
			if (Game1.IsSummer &&
					Game1.dayOfMonth >= 12 && Game1.dayOfMonth <= 14)
				waterColor.Value = new Color (0, 255, 0) * 0.4f;

			// Make the grass snowy in Winter.
			if (Game1.IsWinter)
			{
				foreach (TerrainFeature tf in terrainFeatures.Values)
				{
					if (tf is Grass grass)
						grass.grassSourceOffset.Value = 80;
				}
			}
		}

		protected override void resetLocalState ()
		{
			base.resetLocalState ();

			// Tidepool water shouldn't shimmer.
			int width = map.Layers[0]?.LayerWidth ?? 0;
			int height = map.Layers[0]?.LayerHeight ?? 0;
			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					if (waterTiles[x, y] && isTidepoolTile (x, y))
						waterTiles[x, y] = false;
				}
			}

			// Spawn pelicans on entry (using Seagull class; reskinned via CP).
			Vector2 center = new Vector2
				(Game1.random.Next (map.DisplayWidth / 64),
				Game1.random.Next (Data.ForestBeachYLine, map.DisplayHeight / 64));
			int count = Game1.random.Next (Data.MaxSeabirdCount + 1);
			foreach (Vector2 pos in Utility.getPositionsInClusterAroundThisTile (center, count))
			{
				if (isTileOnMap (pos) && (isTileLocationTotallyClearAndPlaceable (pos) ||
					doesTileHaveProperty ((int) pos.X, (int) pos.Y, "Water", "Back") != null))
				{
					int startingState = 3;
					if (doesTileHaveProperty ((int) pos.X, (int) pos.Y, "Water", "Back") != null)
					{
						startingState = 2;
						if (Game1.random.NextDouble () < 0.5)
							continue;
					}
					critters.Add (new Seagull (pos * 64f + new Vector2 (32f, 32f), startingState));
				}
			}
		}

		public override void DayUpdate (int dayOfMonth)
		{
			base.DayUpdate (dayOfMonth);

			// Switch tidepool and ocean Crab Pots to saltwater catches.
			foreach (SObject @object in objects.Values)
			{
				// Must be a Crab Pot.
				if (!(@object is CrabPot pot))
					continue;

				// Must not be in a freshwater area.
				if (getFishingLocation (pot.TileLocation) == (int) FishingArea.Pond)
					continue;

				// Must have a non-trash catch already.
				if (pot.heldObject.Value == null ||
						pot.heldObject.Value.Category == SObject.junkCategory)
					continue;

				// Check for the Mariner profession.
				Farmer player = (pot.owner.Value != 0L)
					? Game1.getFarmer (pot.owner.Value) : Game1.player;
				bool mariner = player?.professions?.Contains (Farmer.mariner) ?? false;

				// Seed the RNG.
				Random rng = new Random ((int) Game1.stats.DaysPlayed +
					(int) Game1.uniqueIDForThisGame / 2 +
					(int) pot.tileLocation.X * 1000 +
					(int) pot.tileLocation.Y);

				// Search for suitable fish.
				Dictionary<int, string> fishes =
					Helper.Content.Load<Dictionary<int, string>> ("Data\\Fish",
						ContentSource.GameContent);
				List<int> candidates = new List<int> ();
				foreach (KeyValuePair<int, string> fish in fishes)
				{
					if (!fish.Value.Contains ("trap"))
						continue;

					string[] fields = fish.Value.Split ('/');
					if (fields[4].Equals ("freshwater"))
						continue;

					candidates.Add (fish.Key);

					if (!mariner && rng.NextDouble() < Convert.ToDouble (fields[2]))
					{
						pot.heldObject.Value = new SObject (fish.Key, 1);
						candidates.Clear ();
						break;
					}
				}

				// Provide a fallback.
				if (candidates.Count > 0)
				{
					pot.heldObject.Value = new SObject
						(candidates[rng.Next (candidates.Count)], 1);
				}
			}
		}

		public override void seasonUpdate (string season, bool onLoad = false)
		{
			// Hide the terrain features from the seasonal update so that grass
			// is not removed in winter.
			NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> hold =
				new NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> ();
			hold.MoveFrom (terrainFeatures);
			base.seasonUpdate (season, onLoad);
			terrainFeatures.MoveFrom (hold);

			// Handle the terrain features separately, preserving the grass.
			for (int num = terrainFeatures.Count () - 1; num >= 0; --num)
			{
				TerrainFeature tf = terrainFeatures.Values.ElementAt (num);
				if (tf is Grass)
					tf.loadSprite ();
				else if (tf.seasonUpdate (onLoad))
					terrainFeatures.Remove (terrainFeatures.Keys.ElementAt (num));
			}
		}
	}
}
