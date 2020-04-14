using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace PredictiveCore
{
	public enum GarbageCan
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
		SVE_AdventureGuild, // Marlon
		SVE_JoshHouse, // Alex, Evelyn, George
		SVE_Saloon, // Gus
		SVE_JenkinsHouse, // Olivia, Victor
		SVE_ManorHouse, // Lewis
		SVE_Max,
	}

	public struct GarbagePrediction
	{
		public WorldDate date;
		public GarbageCan can;
		public Item loot;
		public bool special;
	}

	public static class Garbage
	{
		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Game1.stats.getStat ("trashCansChecked") > 0 &&
			(Utilities.Config.InaccuratePredictions ||
				!Utilities.Helper.ModRegistry.IsLoaded ("AairTheGreat.BetterGarbageCans"));

		// Whether future progress by the player could alter the loot found in
		// cans. This intentionally disregards the crafting recipes, cooking
		// recipes, mine progress and vault bundle completion considered by
		// Utility.getRandomItemFromSeason, as the combination of those would
		// keep this confusingly true well into the midgame.
		public static bool IsProgressDependent
		{
			get
			{
				Utilities.CheckWorldReady ();
				return Game1.stats.getStat ("trashCansChecked") + 1 <= 20;
			}
		}

		// Lists the loot to be found in Garbage Cans on the given date.
		public static List<GarbagePrediction> ListLootForDate (WorldDate date,
			bool hatOnly = false)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new InvalidOperationException ("No garbage cans have been checked.");

			List<GarbagePrediction> predictions = new List<GarbagePrediction> ();

			bool sve = Utilities.Helper.ModRegistry.IsLoaded
				("FlashShifter.StardewValleyExpandedALL");
			for (int can = sve ? 100 : 0;
				can < (sve ? (int) GarbageCan.SVE_Max : (int) GarbageCan.Max);
				++can)
			{
				Item loot = GetLootForDateAndCan (date, (GarbageCan) can,
					hatOnly, out bool special);
				if (loot != null)
				{
					predictions.Add (new GarbagePrediction
					{
						date = date,
						can = (GarbageCan) can,
						loot = loot,
						special = special
					});
				}
			}

			return predictions;
		}

		// Finds the next Garbage Hat to be available on or after the given date.
		public static GarbagePrediction? FindGarbageHat (WorldDate fromDate)
		{
			for (int days = fromDate.TotalDays;
				days < fromDate.TotalDays + Utilities.MaxHorizon;
				++days)
			{
				List<GarbagePrediction> predictions =
					ListLootForDate (Utilities.TotalDaysToWorldDate (days), true)
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
				(_command, args) => ConsoleCommand (new List<string> (args)));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				WorldDate date = Utilities.ArgsToWorldDate (args);

				List<GarbagePrediction> predictions = ListLootForDate (date);
				Utilities.Monitor.Log ($"Loot in Garbage Cans on {date}:",
					LogLevel.Info);
				foreach (GarbagePrediction prediction in predictions)
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

		public static readonly Dictionary<GarbageCan, Location> CanLocations =
			new Dictionary<GarbageCan, Location>
		{
			{ GarbageCan.SamHouse, new Location (13, 86) },
			{ GarbageCan.HaleyHouse, new Location (19, 89) },
			{ GarbageCan.ManorHouse, new Location (56, 85) },
			{ GarbageCan.ArchaeologyHouse, new Location (108, 91) },
			{ GarbageCan.Blacksmith, new Location (97, 80) },
			{ GarbageCan.Saloon, new Location (47, 70) },
			{ GarbageCan.JoshHouse, new Location (52, 63) },
			{ GarbageCan.JojaMart, new Location (110, 56) },
			{ GarbageCan.MovieTheater, new Location (110, 56) },
			// alternate can locations for SVE
			{ GarbageCan.SVE_SamHouse, new Location (5, 89) },
			{ GarbageCan.SVE_HaleyHouse, new Location (27, 84) },
			{ GarbageCan.SVE_AdventureGuild, new Location (28, 98) },
			{ GarbageCan.SVE_JoshHouse, new Location (52, 63) },
			{ GarbageCan.SVE_Saloon, new Location (47, 70) },
			{ GarbageCan.SVE_JenkinsHouse, new Location (66, 52) },
			{ GarbageCan.SVE_ManorHouse, new Location (56, 85) },
		};

		private static Item GetLootForDateAndCan (WorldDate date, GarbageCan can,
			bool hatOnly, out bool special)
		{
			// Logic from StardewValley.Locations.Town.checkAction()
			// as implemented in Stardew Predictor by MouseyPounds.

			special = false;
			
			// Handle the presence of SVE's altered town map.
			GarbageCan standardCan = (GarbageCan) ((int) can % 100);
			int canValue = (int) standardCan;

			// Handle the special case of JojaMart/MovieTheater.
			bool hasTheater = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheater") &&
				!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheaterJoja");
			if ((hasTheater && can == GarbageCan.JojaMart) ||
				(!hasTheater && can == GarbageCan.MovieTheater))
				return null;
			if (can == GarbageCan.MovieTheater)
				canValue = (int) GarbageCan.JojaMart;

			// Create and prewarm the random generator.
			int daysPlayed = date.TotalDays + 1;
			Random rng = new Random ((int) Game1.uniqueIDForThisGame / 2 +
				daysPlayed + 777 + canValue * 77);
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
			bool today = date == Utilities.Now ();
			double dailyLuck = today ? Game1.player.DailyLuck
				: Game1.player.hasSpecialCharm ? 0.125 : 0.1;
			if (!regular && !(rng.NextDouble () < 0.2 + dailyLuck))
				return null;

			// Roll for a generic or seasonal item.
			int itemID;
			bool seasonal = false;
			switch (rng.Next (10))
			{
			case 1:
				itemID = 167; // Joja Cola
				break;
			case 2:
				itemID = 170; // Broken Glasses
				break;
			case 3:
				itemID = 171; // Broken CD
				break;
			case 4:
				itemID = 172; // Soggy Newspaper
				break;
			case 5:
				itemID = 216; // Bread
				break;
			case 6:
				seasonal = true;
				Location location = CanLocations[can];
				itemID = Utility.getRandomItemFromSeason (date.Season,
					(location.X * 653) + (location.Y * 777) + daysPlayed,
					false, false);
				break;
			case 7:
				itemID = 403; // Field Snack
				break;
			case 8:
				itemID = 309 + rng.Next (3); // Acorn, Maple Seed, Pine Cone
				break;
			case 9:
				itemID = 153; // Green Algae
				break;
			default:
				itemID = 168; // Trash
				break;
			}

			// Roll for location-specific overrides. These do not care about
			// SVE, so take the standard can identity.
			bool locationSpecific = false;
			switch (standardCan)
			{
			case GarbageCan.ArchaeologyHouse:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					if (rng.NextDouble () < 0.05)
						itemID = 749; // Omni Geode
					else
						itemID = 535; // Geode
				}
				break;
			case GarbageCan.Blacksmith:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					itemID = 378 + (rng.Next (3) * 2); // Copper Ore, Iron Ore, Coal
					rng.Next (1, 5); // unused
				}
				break;
			case GarbageCan.Saloon:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					if (!today)
						itemID = 217; // placeholder for dish of the day
					else if (Game1.dishOfTheDay != null)
						itemID = Game1.dishOfTheDay.ParentSheetIndex;
				}
				break;
			case GarbageCan.JoshHouse:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					itemID = 223; // Cookie
				}
				break;
			case GarbageCan.JojaMart:
				if (rng.NextDouble () < 0.2 &&
					!Utility.HasAnyPlayerSeenEvent (191393))
				{
					locationSpecific = true;
					itemID = 167; // Joja Cola
				}
				break;
			case GarbageCan.MovieTheater:
				if (rng.NextDouble () < 0.2)
				{
					locationSpecific = true;
					itemID = (rng.NextDouble () < 0.25)
						? 809 : 270; // Movie Ticket, Corn
				}
				break;
			}

			special = seasonal || locationSpecific;
			return new SObject (itemID, 1);
		}
	}
}
