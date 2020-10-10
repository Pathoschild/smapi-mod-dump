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
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Cropbeasts
{
	public static class Utilities
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModConfig Config => ModConfig.Instance;

		// Accesses Game1.multiplayer.
		public static Multiplayer MP => Helper.Reflection.GetField<Multiplayer>
			(typeof (Game1), "multiplayer").GetValue ();

		// Gets the parentSheetIndex of the seed object corresponding to the
		// given crop harvest object.
		public static Crop MakeNonceCrop (int harvestIndex, Point tileLocation)
		{
			Crop seedLookup = new Crop ();
			seedLookup.indexOfHarvest.Value = harvestIndex;
			seedLookup.InferSeedIndex ();
			Crop crop = new Crop (seedLookup.netSeedIndex.Value,
				tileLocation.X, tileLocation.Y);
			crop.growCompletely ();
			return crop;
		}

		// Lists the Wicked Statues in the given location.
		public static List<SObject> FindWickedStatues (GameLocation location)
		{
			return location.objects.Values
				.Where ((obj) => obj.bigCraftable.Value &&
					obj.ParentSheetIndex == 83)
				.ToList ();
		}

		// Lists the Junimo Huts in the given location that are active today
		// (not turned off, not rainy/stormy, not winter).
		public static List<JunimoHut> FindActiveJunimoHuts (GameLocation location)
		{
			if (Game1.IsWinter || Game1.isRaining)
				return new List<JunimoHut> ();

			if (!(location is Farm farm))
				return new List<JunimoHut> ();

			return farm.buildings
				.Where ((bldg) => bldg is JunimoHut jh && !jh.noHarvest.Value)
				.Select ((bldg => bldg as JunimoHut))
				.ToList ();
		}

		// Finds all farmers within the given threshold of distance to the given
		// tile in the given location, sorted by distance.
		public static SortedList<float, Farmer> FindNearbyFarmers
			(GameLocation location, Point tileLocation, float threshold = -1f)
		{
			var farmers = new SortedList<float, Farmer> ();
			foreach (Farmer farmer in location.farmers)
			{
				float distance = Vector2.Distance
					(Utility.PointToVector2 (tileLocation),
					farmer.getTileLocation ());
				if (distance < threshold || threshold < 0f)
					farmers[distance] = farmer;
			}
			return farmers;
		}

		// Finds the farmer nearest to the given tile in the given location,
		// optionally constrained to a given threshold of distance.
		public static Farmer FindNearestFarmer (GameLocation location,
			Point tileLocation, out float distance, float threshold = -1f)
		{
			try
			{
				var farmers = FindNearbyFarmers (location, tileLocation,
					threshold);
				distance = farmers.First ().Key;
				return farmers.First ().Value;
			}
			catch (InvalidOperationException)
			{
				distance = -1f;
				return null;
			}
		}

		public static void MagicPoof (GameLocation location, Vector2 center,
			float radius, int count, Color color, string soundCue = null)
		{
			if ((soundCue ?? "") != "")
				location.playSound (soundCue);
			for (int i = 0; i < count; ++i)
			{
				Vector2 position = center + radius * 2f * new Vector2
					((float) Game1.random.NextDouble () - 0.5f,
					(float) Game1.random.NextDouble () - 0.5f);
				Utilities.MP.broadcastSprites (location,
					new TemporaryAnimatedSprite (Game1.objectSpriteSheetName,
					Game1.getSourceRectForStandardTileSheet
						(Game1.objectSpriteSheet, 354, 16, 16),
					Game1.random.Next (25, 75), 6, 2, position,
					flicker: false, flipped: Game1.random.NextDouble () < 0.5,
					1f, 0f, color, 3f, 0f, 0f, 0f));
			}
		}

		// Tries to play a sound cue, but intercepts and discards
		// InvalidOperationException in case it was already playing but somehow
		// didn't register as IsPlaying, which can apparently happen.
		public static void TryPlayCue (ICue cue)
		{
			try
			{
				cue.Play ();
			}
			catch (InvalidOperationException)
			{}
		}

		// Tries to stop a sound cue, but uses reflection and ignores the
		// nonexistence of the method since some players have a version of the
		// game and/or XNA that doesn't like this.
		public static void TryStopCue (ICue cue)
		{
			IReflectedMethod stop =
				Helper.Reflection.GetMethod (cue, "Stop", false);
			if (stop != null)
				stop.Invoke (AudioStopOptions.AsAuthored);
		}
	}
}
