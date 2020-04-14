using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ScryingOrb
{
	public class MiningExperience : Experience
	{
		public static readonly Dictionary<int, int> AcceptedOfferings =
			new Dictionary<int, int>
		{
			{ 378, 5 }, // Copper Ore
			{ 380, 3 }, // Iron Ore
			{ 384, 1 }, // Gold Ore
			{ 386, 1 }, // Iridium Ore
			{ 382, 2 }, // Coal
		};

		public override bool isAvailable =>
			base.isAvailable && Mining.IsAvailable;

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
			playSound ("hammer");
			playSound ("stoneCrack");
			showAnimation ("TileSheets\\animations",
				new Rectangle (0, 3072, 128, 128), 150f, 5, 1);
			showMessage ("mining.opening", 500);
			Game1.afterDialogues = run;

			return true;
		}

		protected override void doRun ()
		{
			Game1.activeClickableMenu = new DatePicker (Utilities.Now (),
				Helper.Translation.Get ("mining.date.question"), onDateChosen);
		}
		
		private void onDateChosen (WorldDate date)
		{
			// Gather the appropriate predictions.
			List<MiningPrediction> predictions =
				Mining.ListFloorsForDate (date);

			bool today = date == Utilities.Now ();
			List<string> pages = new List<string> ();

			// Build the list of predictions.
			List<string> lines = new List<string>
			{
				Helper.Translation.Get ($"mining.header.{(today ? "today" : "later")}", new
				{
					date = date.Localize (),
				})
			};

			string joiner = CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
			foreach (MineFloorType type in predictions
				.Select ((p) => p.type).Distinct ().ToList ())
			{
				List<int> floors = predictions
					.Where ((p) => p.type == type)
					.Select ((p) => p.floor)
					.ToList ();
				string floorsText;
				if (floors.Count == 1)
				{
					floorsText = Helper.Translation.Get ("mining.floor",
						new { num = floors[0] });
				}
				else
				{
					int lastNum = floors[floors.Count - 1];
					floors.RemoveAt (floors.Count - 1);
					floorsText = unbreak (Helper.Translation.Get ("mining.floors",
						new { nums = string.Join (joiner, floors), lastNum = lastNum }));
				}

				lines.Add (Helper.Translation.Get ($"mining.prediction.{type}",
					new { floors = floorsText }));
			}
			if (predictions.Count == 0)
				lines.Add (Helper.Translation.Get ("mining.prediction.none"));
			pages.Add (string.Join ("^", lines));

			// If going deeper in the mines could alter the results, add an
			// appropriate closing.
			if (Mining.IsProgressDependent)
			{
				pages.Add (Helper.Translation.Get ("mining.closing.progress"));
			}

			// Show the predictions.
			showDialogues (pages);
			Game1.afterDialogues = extinguish;
		}
	}
}
