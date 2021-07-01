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
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictiveCore
{
	public static class NightEvents
	{
		public enum Event
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

		public struct Prediction
		{
			public SDate date;
			public Event @event;
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Utilities.Config.InaccuratePredictions || Utilities.SupportedVersion;

		// Lists the next several night events to occur on or after the given
		// date, up to the given limit, optionally of a given type.
		public static List<Prediction> ListNextEventsFromDate (SDate fromDate,
			uint limit, Event? onlyType = null)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new UnavailableException ("night events");

			// Logic from StardewValley.Utility.pickFarmEvent
			// as implemented in Stardew Predictor by MouseyPounds.

			List<Prediction> predictions = new ();

			for (int days = fromDate.DaysSinceStart;
				predictions.Count < limit &&
					days < fromDate.DaysSinceStart + Utilities.MaxHorizon;
				++days)
			{
				SDate tonight = SDate.FromDaysSinceStart (days);
				SDate tomorrow = SDate.FromDaysSinceStart (days + 1);

				// No event if there is a wedding tomorrow.
				foreach (Farmer farmer in Game1.getAllFarmers ())
				{
					Friendship spouse = farmer.GetSpouseFriendship ();
					if (spouse != null &&
							spouse.WeddingDate == tomorrow.ToWorldDate ())
						continue;
				}

				Event @event = Event.None;
				Random rng = new (((int) Game1.uniqueIDForThisGame / 2) + days + 1);
				if (days == 29)
					@event = Event.Earthquake;
				// Ignoring the possibility of a WorldChangeEvent here.
				else if (rng.NextDouble () < 0.01 && tomorrow.Season != "winter")
					@event = Event.Fairy;
				else if (rng.NextDouble () < 0.01)
					@event = Event.Witch;
				else if (rng.NextDouble () < 0.01)
					@event = Event.Meteorite;
				else if (rng.NextDouble () < 0.005)
					@event = Event.StoneOwl;
				else if (rng.NextDouble () < 0.008 && tomorrow.Year > 1 &&
						!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("Got_Capsule"))
					@event = Event.StrangeCapsule;

				if (@event == Event.None ||
						(onlyType != null && @event != onlyType))
					continue;

				predictions.Add (new Prediction { date = tonight, @event = @event });
			}

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
				return;
			Utilities.Helper.ConsoleCommands.Add ("predict_night_events",
				"Predicts the next several night events to occur on or after a given date, or tonight by default.\n\nUsage: predict_night_events [<limit> [<year> <season> <day>]]\n- limit: number of events to predict (default 20)\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (args.ToList ()));
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
				SDate date = Utilities.ArgsToSDate (args);

				List<Prediction> predictions = ListNextEventsFromDate (date, limit);
				Utilities.Monitor.Log ($"Next {limit} night event(s) occurring on or after {date}:",
					LogLevel.Info);
				foreach (Prediction prediction in predictions)
				{
					Utilities.Monitor.Log ($"- {prediction.date}: {prediction.@event}",
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
