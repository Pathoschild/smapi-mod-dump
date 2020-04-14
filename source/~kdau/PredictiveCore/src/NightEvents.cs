using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace PredictiveCore
{
	public enum NightEventType
	{
		None,
		Earthquake,
		Fairy,
		Witch,
		Meteorite,
		StrangeCapsule,
		StoneOwl,
		NewYear, // used by PublicAccessTV only
	}

	public struct NightEventPrediction
	{
		public WorldDate date;
		public NightEventType type;
	}

	public static class NightEvents
	{
		// Whether this module should be available for player use.
		public static bool IsAvailable => true;

		// Lists the next several night events to occur on or after the given
		// date, up to the given limit, optionally of a given type.
		public static List<NightEventPrediction> ListNextEventsForDate
			(WorldDate fromDate, uint limit, NightEventType? onlyType = null)
		{
			Utilities.CheckWorldReady ();

			// Logic from StardewValley.Utility.<>c.<pickFarmEvent>b__146_0()
			// as implemented in Stardew Predictor by MouseyPounds.

			List<NightEventPrediction> predictions =
				new List<NightEventPrediction> ();

			for (int days = fromDate.TotalDays;
				predictions.Count < limit &&
					days < fromDate.TotalDays + Utilities.MaxHorizon;
				++days)
			{
				WorldDate tonight = Utilities.TotalDaysToWorldDate (days);
				WorldDate tomorrow = Utilities.TotalDaysToWorldDate (days + 1);

				// No event if there is a wedding tomorrow.
				foreach (Farmer farmer in Game1.getAllFarmers ())
				{
					Friendship spouse = farmer.GetSpouseFriendship ();
					if (spouse != null && spouse.WeddingDate == tomorrow)
						continue;
				}

				NightEventType type = NightEventType.None;
				Random rng = new Random (((int) Game1.uniqueIDForThisGame / 2) +
					days + 2);
				if (days == 29)
					type = NightEventType.Earthquake;
				// Ignoring the possibility of bundle completion here.
				else if (rng.NextDouble () < 0.01 && tomorrow.Season != "winter")
					type = NightEventType.Fairy;
				else if (rng.NextDouble () < 0.01)
					type = NightEventType.Witch;
				else if (rng.NextDouble () < 0.01)
					type = NightEventType.Meteorite;
				else if (rng.NextDouble () < 0.01 && tomorrow.Year > 1)
					type = NightEventType.StrangeCapsule;
				else if (rng.NextDouble () < 0.01)
					type = NightEventType.StoneOwl;

				if (type == NightEventType.None ||
						(onlyType != null && type != onlyType))
					continue;

				predictions.Add (new NightEventPrediction
					{ date = tonight, type = type });
			}

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
				return;
			Utilities.Helper.ConsoleCommands.Add ("predict_night_events",
				"Predicts the next several night events to occur on or after a given date, or tonight by default.\n\nUsage: predict_night_events [<limit> [<year> <season> <day>]]\n- limit: number of events to predict (default 20)\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (new List<string> (args)));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				uint limit = 20;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out limit) || limit < 1)
						throw new ArgumentException ($"Invalid limit '{args[0]}', must be a number 1 or higher.");
					args.RemoveAt (0);
				}
				WorldDate date = Utilities.ArgsToWorldDate (args);

				List<NightEventPrediction> predictions = ListNextEventsForDate (date, limit);
				Utilities.Monitor.Log ($"Next {limit} night event(s) occurring on or after {date}:",
					LogLevel.Info);
				foreach (NightEventPrediction prediction in predictions)
				{
					Utilities.Monitor.Log ($"- {prediction.date}: {prediction.type}",
						LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
