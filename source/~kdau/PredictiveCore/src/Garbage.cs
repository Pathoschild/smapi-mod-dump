/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace PredictiveCore
{
	public static class Garbage
	{
		public enum Can
		{
			SamHouse, // Jodi, Kent*, Sam, Vincent
			HaleyHouse, // Emily, Haley
			ManorHouse, // Lewis
			ArchaeologyHouse, // Gunther
			Blacksmith, // Clint
			Saloon, // Gus
			JoshHouse, // Alex, Evelyn, George
			JojaMart, // Morris
			MovieTheater, // (only when replaing JojaMart)
			Max,
			// alternate can sequence for SVE
			SVE_SamHouse = 100, // Jodi, Kent*, Sam, Vincent
			SVE_HaleyHouse, // Emily, Haley
			SVE_ArchaeologyHouse, // Gunther
			SVE_JoshHouse, // Alex, Evelyn, George
			SVE_Saloon, // Gus
			SVE_JenkinsHouse, // Olivia, Victor
			SVE_ManorHouse, // Lewis
			SVE_Blacksmith, // Clint
			SVE_Max,
		}

		public struct Prediction
		{
			public SDate date;
			public Can can;
			public Item loot;
			public bool special;
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Game1.stats.getStat ("trashCansChecked") > 0 &&
			(Utilities.Config.InaccuratePredictions ||
				(Utilities.SupportedVersion &&
				!Utilities.Helper.ModRegistry.IsLoaded ("AairTheGreat.BetterGarbageCans")));

		// Whether future progress by the player could alter the loot found in
		// cans. This intentionally disregards the crafting recipes, cooking
		// recipes, mine progress and vault bundle completion considered by
		// Utility.getRandomItemFromSeason, as the combination of those would
		// keep this confusingly true well into the midgame.
		public static bool IsProgressDependent =>
			Game1.stats.getStat ("trashCansChecked") + 1 <= 20;

		// Lists the loot to be found in Garbage Cans on the given date.
		public static List<Prediction> ListLootForDate (SDate date,
			bool hatOnly = false)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("garbage");

			List<Prediction> predictions = new ();

			bool sve = Utilities.Helper.ModRegistry.IsLoaded
				("FlashShifter.StardewValleyExpandedCP");
			for (int can = sve ? 100 : 0;
				can < (sve ? (int) Can.SVE_Max : (int) Can.Max);
				++can)
			{
				Item loot = GetLootForDateAndCan (date, (Can) can,
					hatOnly, out bool special);
				if (loot != null)
				{
					predictions.Add (new Prediction
					{
						date = date,
						can = (Can) can,
						loot = loot,
						special = special
					});
				}
			}

			return predictions;
		}

		// Finds the next Garbage Hat to be available on or after the given date.
		public static Prediction? FindGarbageHat (SDate fromDate)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("garbage");

			for (int days = fromDate.DaysSinceStart;
				days < fromDate.DaysSinceStart + Utilities.MaxHorizon;
				++days)
			{
				List<Prediction> predictions =
					ListLootForDate (SDate.FromDaysSinceStart (days), true)
					.Where ((p) => p.loot is Hat).ToList ();
				if (predictions.Count > 0)
					return predictions[0];
			}
			return null;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
			{
				return;
			}
			Utilities.Helper.ConsoleCommands.Add ("predict_garbage",
				"Predicts the loot to be found in Garbage Cans on a given date, or today by default.\n\nUsage: predict_garbage [<year> <season> <day>]\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (args.ToList ()));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				SDate date = Utilities.ArgsToSDate (args);

				List<Prediction> predictions = ListLootForDate (date);
				Utilities.Monitor.Log ($"Loot in Garbage Cans on {date}:",
					LogLevel.Info);
				foreach (Prediction prediction in predictions)
				{
					string name = (prediction.loot.ParentSheetIndex == 217)
						? "(dish of the day)"
						: prediction.loot.Name;
					string stars = (prediction.loot is Hat)
						? " ***"
							: prediction.special
								? " *"
								: "";
					Utilities.Monitor.Log ($"- {prediction.can}: {name}{stars}",
						LogLevel.Info);
				}
				if (predictions.Count == 0)
				{
					Utilities.Monitor.Log ("  (none)", LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}

		public static readonly Dictionary<Can, Location> CanLocations = new ()
		{
			{ Can.SamHouse, new Location (13, 86) },
			{ Can.HaleyHouse, new Location (19, 89) },
			{ Can.ManorHouse, new Location (56, 85) },
			{ Can.ArchaeologyHouse, new Location (108, 91) },
			{ Can.Blacksmith, new Location (97, 80) },
			{ Can.Saloon, new Location (47, 70) },
			{ Can.JoshHouse, new Location (52, 63) },
			{ Can.JojaMart, new Location (110, 56) },
			{ Can.MovieTheater, new Location (110, 56) },
			// alternate can locations for SVE
			{ Can.SVE_SamHouse, new Location (5, 89) },
			{ Can.SVE_HaleyHouse, new Location (27, 84) },
			{ Can.SVE_ArchaeologyHouse, new Location (108, 91) },
			{ Can.SVE_JoshHouse, new Location (52, 63) },
			{ Can.SVE_Saloon, new Location (47, 70) },
			{ Can.SVE_JenkinsHouse, new Location (66, 52) },
			{ Can.SVE_ManorHouse, new Location (56, 85) },
			{ Can.SVE_Blacksmith, new Location (96, 81) },
		};

		private static Item GetLootForDateAndCan (SDate date, Can can,
			bool hatOnly, out bool special)
		{
			// Logic from StardewValley.Locations.Town.checkAction()
			// as implemented in Stardew Predictor by MouseyPounds.

			special = false;

			// Handle the presence of SVE's altered town map.
			Can standardCan = (Can) ((int) can % 100);
			int canValue = (int) standardCan;

			// Handle the special case of JojaMart/MovieTheater.
			bool hasTheater = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheater") &&
				!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheaterJoja");
			if ((hasTheater && can == Can.JojaMart) ||
				(!hasTheater && can == Can.MovieTheater))
				return null;
			if (can == Can.MovieTheater)
				canValue = (int) Can.JojaMart;

			// Create and prewarm the random generator.
			Random rng = new ((int) Game1.uniqueIDForThisGame / 2 +
				date.DaysSinceStart + 777 + canValue * 77);
			int prewarm = rng.Next (0, 100);
			for (int i = 0; i < prewarm; i++)
				rng.NextDouble ();
			prewarm = rng.Next (0, 100);
			for (int j = 0; j < prewarm; j++)
				rng.NextDouble ();

			// Roll for regular items.
			uint trashCansChecked = Game1.stats.getStat ("trashCansChecked") + 1;
			bool regular = trashCansChecked > 20 && rng.NextDouble () < 0.01;

			// Roll for the Garbage Hat.
			if (trashCansChecked > 20 && rng.NextDouble () < 0.002)
				return new Hat (66);
			else if (hatOnly)
				return null;

			// If the regular roll failed, roll for luck and then give up.
			// Use today's luck for today, else a liquidated value.
			bool today = date == SDate.Now ();
			double dailyLuck = today ? Game1.player.DailyLuck
				: Game1.player.hasSpecialCharm ? 0.125 : 0.1;
			if (!regular && !(rng.NextDouble () < 0.2 + dailyLuck))
				return null;

			// Roll for a generic or seasonal item.
			int itemID = rng.Next (10) switch
			{
				1 => 167, // Joja Cola
				2 => 170, // Broken Glasses
				3 => 171, // Broken CD
				4 => 172, // Soggy Newspaper
				5 => 216, // Bread
				6 => -1, // seasonal item
				7 => 403, // Field Snack
				8 => 309 + rng.Next (3), // Acorn, Maple Seed, Pine Cone
				9 => 153, // Green Algae
				_ => 168, // Trash
			};
			bool seasonal = false;
			if (itemID == -1)
			{
				seasonal = true;
				itemID = Utility.getRandomItemFromSeason (date.Season,
					(CanLocations[can].X * 653) + (CanLocations[can].Y * 777) +
					date.DaysSinceStart, forQuest: false, changeDaily: false);
			}

			// Roll for location-specific overrides. These do not care about
			// SVE, so take the standard can identity.
			bool locationSpecific = false;
			switch (standardCan)
			{
			case Can.ArchaeologyHouse:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					if (rng.NextDouble () < 0.05)
						itemID = 749; // Omni Geode
					else
						itemID = 535; // Geode
				}
				break;
			case Can.Blacksmith:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					itemID = 378 + (rng.Next (3) * 2); // Copper Ore, Iron Ore, Coal
					rng.Next (1, 5); // unused
				}
				break;
			case Can.Saloon:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					if (!today)
						itemID = 217; // placeholder for dish of the day
					else if (Game1.dishOfTheDay != null)
						itemID = Game1.dishOfTheDay.ParentSheetIndex;
				}
				break;
			case Can.JoshHouse:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					itemID = 223; // Cookie
				}
				break;
			case Can.JojaMart:
				if (rng.NextDouble () < 0.2 &&
					!Utility.HasAnyPlayerSeenEvent (191393))
				{
					locationSpecific = true;
					itemID = 167; // Joja Cola
				}
				break;
			case Can.MovieTheater:
				if (rng.NextDouble () < 0.2)
				{
					locationSpecific = true;
					itemID = (rng.NextDouble () < 0.25)
						? 809 : 270; // Movie Ticket, Corn
				}
				break;
			}

			// Ignoring the chance of a Qi Bean here since it uses the main RNG.

			special = seasonal || locationSpecific;
			return new SObject (itemID, 1);
		}
	}
}
