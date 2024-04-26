/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
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
			MovieTheater, // (only when replacing JojaMart)
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
			Game1.stats.Get("trashCansChecked") > 0 &&
			(Utilities.Config.InaccuratePredictions ||
				(Utilities.SupportedVersion &&
				!Utilities.Helper.ModRegistry.IsLoaded ("AairTheGreat.BetterGarbageCans")));

		// Whether future progress by the player could alter the loot found in
		// cans. This intentionally disregards the crafting recipes, cooking
		// recipes, mine progress and vault bundle completion considered by
		// Utility.getRandomItemFromSeason, as the combination of those would
		// keep this confusingly true well into the midgame.
		public static bool IsProgressDependent =>
			Game1.stats.Get("trashCansChecked") + 1 <= 20;

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
			Dictionary<Can, string> canIDs = new()
			{
				{ Can.SamHouse, "JodiAndKent" },
				{ Can.HaleyHouse, "EmilyAndHaley" },
				{ Can.ManorHouse, "Mayor" },
				{ Can.ArchaeologyHouse, "Museum" },
				{ Can.Blacksmith, "Blacksmith" },
				{ Can.Saloon, "Saloon" },
				{ Can.JoshHouse, "Evelyn" },
				{ Can.JojaMart, "JojaMart" },
				{ Can.MovieTheater, "JojaMart" }
			};
			var canID = canIDs[standardCan];

			// Skip either JojaMart or (non-Joja) Movie Theater, depending on whether JojaMart has been replaced.
			var ccMovieTheater = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater");
            if (
				(standardCan == Can.JojaMart && ccMovieTheater)
					|| (standardCan == Can.MovieTheater && !ccMovieTheater)
			) {
				return null;
			}

            // Use today's luck for today, else a liquidated value.
            bool today = date == SDate.Now();
            double dailyLuck = today ? Game1.player.DailyLuck
                : Game1.player.hasSpecialCharm ? 0.125 : 0.1;

            var town = Game1.getLocationFromName("Town");
			town.TryGetGarbageItem(
				canID,
				dailyLuck,
				out var item,
				out var selected,
				out var garbageRandom,
				logError: null
			);

			if (item != null)
			{
				switch (item.QualifiedItemId)
				{
					case "(O)170": // Broken Glasses
					case "(O)171": // Broken CD
					case "(O)172": // Soggy Newspaper
					case "(O)216": // Bread
					case "(O)403": // Field Snack
					case "(O)309": // Acorn
					case "(O)310": // Maple Seed
					case "(O)311": // Pine Cone
					case "(O)153": // Green Algae
					case "(O)168": // Trash
						break;
                    case "(O)167": // Joja Cola
                        special = (standardCan == Can.JojaMart && !Utility.HasAnyPlayerSeenEvent("191393")); // Stardew Hero
                        break;
                    default:
						special = true;
						break;
				}
			}

            return item;
		}
	}
}
