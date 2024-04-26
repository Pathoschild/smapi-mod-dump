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
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictiveCore
{
	public static class Movies
	{
		public struct Prediction
		{
			public SDate effectiveDate;
			public MovieData currentMovie;
			public bool craneGameAvailable;

			public SDate firstDateOfNextMovie;
			public MovieData nextMovie;
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheater") &&
			(Utilities.Config.InaccuratePredictions || Utilities.SupportedVersion);

		// Lists the current and next movie and crane game status as of the
		// given date.
		public static Prediction PredictForDate (SDate date)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("movies");

			Prediction prediction = new ()
			{
				effectiveDate = date,
				currentMovie = MovieTheater.GetMovieForDate (date.ToWorldDate ()),
				firstDateOfNextMovie = Utilities.GetNextSeasonStart (date),
			};
			prediction.nextMovie = MovieTheater.GetMovieForDate
				(prediction.firstDateOfNextMovie.ToWorldDate ());

			// Logic from StardewValley.Locations.MovieTheater.addRandomNPCs()
			// as implemented in Stardew Predictor by MouseyPounds.
			if (Game1.getLocationFromName ("MovieTheater") is MovieTheater theater)
			{
				Random rng = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.Date.TotalDays);
				prediction.craneGameAvailable = !(theater.dayFirstEntered.Value == Game1.Date.TotalDays || rng.NextDouble() < 0.25);
			}

			return prediction;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
				return;
			Utilities.Helper.ConsoleCommands.Add ("predict_movies",
				"Predicts the current and next movie and Crane Game status on a given date, or today by default.\n\nUsage: predict_movies [<year> <season> <day>]\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (args.ToList ()));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				SDate date = Utilities.ArgsToSDate (args);
				Prediction prediction = PredictForDate (date);
				Utilities.Monitor.Log ($"On {prediction.effectiveDate}, the movie showing will be \"{prediction.currentMovie.Title}\". \"{prediction.currentMovie.Description}\"",
					LogLevel.Info);
				Utilities.Monitor.Log ($"The Crane Game {(prediction.craneGameAvailable ? "WILL" : "will NOT")} be available.",
					LogLevel.Info);
				Utilities.Monitor.Log ($"The next movie, \"{prediction.nextMovie.Title}\", will begin showing on {prediction.firstDateOfNextMovie}. \"{prediction.nextMovie.Description}\"",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
