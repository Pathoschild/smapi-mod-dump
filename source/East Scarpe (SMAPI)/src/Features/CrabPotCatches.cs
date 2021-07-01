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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace EastScarpe
{
	public static class CrabPotCatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;

		public static void DayUpdate ()
		{
			if (!Context.IsMainPlayer)
				return;

			foreach (var @catch in Data.CrabPotCatches)
				UpdateCatch (@catch);
		}

		private static bool UpdateCatch (CrabPotCatch @catch)
		{
			// Must be a valid location.
			var location = Game1.getLocationFromName (@catch.Location);
			if (location == null)
				return false;

			// Conditions must hold.
			if (!@catch.Conditions.check ())
				return false;

			// Update the Crab Pots.
			Rectangle area = @catch.adjustArea (location);
			Monitor.Log ($"Updating Crab Pot catches in area {area} (fishing area {@catch.FishingArea}) of location '{location.Name}'.",
				LogLevel.Trace);
			foreach (SObject @object in location.objects.Values)
			{
				// Must be a Crab Pot.
				if (@object is not CrabPot pot)
					continue;

				// Must be in the right area and fishing area.
				if (!area.Contains (Utility.Vector2ToPoint (pot.TileLocation)) ||
					(@catch.FishingArea != -1 &&
						location.getFishingLocation (pot.TileLocation) != @catch.FishingArea))
					continue;

				UpdatePot (@catch, pot);
			}
			return true;
		}

		private static void UpdatePot (CrabPotCatch @catch, CrabPot pot)
		{
			// Check for the Mariner and Luremaster professions.
			var owner = Game1.getFarmer (pot.owner.Value);
			bool luremaster = owner != null && owner.professions.Contains (11);
			bool mariner = (owner != null && owner.professions.Contains (10)) ||
				(pot.owner.Value == 0L && Game1.player.professions.Contains (11));

			// Don't proceed unless the pot is baited or Luremaster applies.
			if (pot.bait.Value == null && !luremaster)
				return;

			// The game stops here if the pot has an item, but we must continue.

			// Set the pot as ready for harvest. This is probably redundant.
			pot.tileIndexToShow = 714;
			pot.readyForHarvest.Value = true;

			// Seed the RNG.
			Random random = new ((int) Game1.stats.DaysPlayed +
				(int) Game1.uniqueIDForThisGame / 2 +
				(int) pot.TileLocation.X * 1000 +
				(int) pot.TileLocation.Y);

			// Maybe catch trash.
			double trashChance = 0.2 + @catch.ExtraTrashChance;
			if (!mariner && !(random.NextDouble() > trashChance))
			{
				pot.heldObject.Value = new SObject (random.Next (168, 173), 1);
				return;
			}

			// Search for a suitable fish.
			var fishes = Helper.Content.Load<Dictionary<int, string>> ("Data\\Fish",
				ContentSource.GameContent);
			List<int> candidates = new ();
			foreach (var fish in fishes)
			{
				string[] fields = fish.Value.Split ('/');

				// Must be a Crab Pot fish.
				if (!fish.Value.Contains ("trap"))
					continue;

				// Must be the right catch type.
				if (fields[4] == (@catch.OceanCatches ? "freshwater" : "ocean"))
					continue;

				// Mariners get all fish with equal chance.
				if (mariner)
				{
					candidates.Add (fish.Key);
					continue;
				}

				// Roll for getting the fish immediately.
				double chance = Convert.ToDouble (fields[2]);
				if (random.NextDouble() < chance)
				{
					pot.heldObject.Value = new SObject (fish.Key, 1);
					return;
				}
			}

			// Fall back to a random selection of the candidates.
			if (candidates.Count == 0)
				candidates.AddRange (new int[] { 168, 169, 170, 171, 172 });
			pot.heldObject.Value = new SObject(candidates[random.Next (candidates.Count)], 1);
		}
	}
}
