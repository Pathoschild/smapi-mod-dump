using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class NightEventsExperience : Experience
	{
		public static readonly Dictionary<int, int> AcceptedOfferings =
			new Dictionary<int, int>
		{
			{ 767, 3 }, // Bat Wing
			{ 305, 1 }, // Void Egg
			{ 769, 1 }, // Void Essence
			{ 308, 1 }, // Void Mayonnaise
			{ 795, 1 }, // Void Salmon
		};

		public static readonly Dictionary<string, NightEventType?> Types =
			new Dictionary<string, NightEventType?>
		{
			{ "any", null },
			{ "Fairy", NightEventType.Fairy },
			{ "Witch", NightEventType.Witch },
			{ "Meteorite", NightEventType.Meteorite },
			{ "StrangeCapsule", NightEventType.StrangeCapsule },
			{ "StoneOwl", NightEventType.StoneOwl },
			{ "leave", NightEventType.None }
		};

		protected override bool check ()
		{
			// Consume an appropriate offering.
			if (!checkOffering (accepted: AcceptedOfferings.Keys.ToList ()))
				return false;
			if (offering.Stack < AcceptedOfferings[offering.ParentSheetIndex])
			{
				showRejection ("rejection.insufficient");
				return true;
			}
			consumeOffering (AcceptedOfferings[offering.ParentSheetIndex]);

			// React to the offering, then proceed to run.
			illuminate ();
			playSound ("shadowpeep");
			showAnimation ("TileSheets\\animations",
				new Rectangle (0, 2880, 64, 64), 125f, 10, 1);
			showMessage ("nightEvents.opening", 500);
			Game1.afterDialogues = run;

			return true;
		}

		protected override void doRun ()
		{
			// Show the menu of types.
			List<Response> types = Types.Select ((t) => new Response (t.Key,
				Helper.Translation.Get ($"nightEvents.type.{t.Key}"))).ToList ();
			if (Constants.TargetPlatform == GamePlatform.Android)
				types.RemoveAll ((r) => r.responseKey == "leave");
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("nightEvents.type.question"), types);

			Game1.currentLocation.afterQuestion = (Farmer _who, string type) =>
			{
				Game1.currentLocation.afterQuestion = null;

				// If "leave", we're done.
				if (type == "leave")
				{
					extinguish ();
					return;
				}

				// Gather the appropriate predictions.
				List<NightEventPrediction> predictions =
					NightEvents.ListNextEventsForDate (Utilities.Now (), 3,
						Types[type]);
				if (predictions.Count == 0)
				{
					throw new Exception ($"Could not predict night events of {type} type.");
				}

				// Show a list of the predictions.
				List<string> predictionStrings = predictions.Select ((p) =>
					unbreak (Helper.Translation.Get ($"nightEvents.prediction.{p.type}",
						new { date = p.date.Localize () }).ToString ())).ToList ();
				showDialogues (new List<string>
				{
					string.Join ("^", predictionStrings),
					unbreak (Helper.Translation.Get ("nightEvents.closing")),
				});
				Game1.afterDialogues = extinguish;
			};
		}
	}
}
