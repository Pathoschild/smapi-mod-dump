using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public class GarbageExperience : Experience
	{
		public override bool isAvailable =>
			base.isAvailable && Garbage.IsAvailable;

		protected override bool check ()
		{
			// Require an appropriate offering.
			if (!checkOffering (category: SObject.junkCategory))
				return false;
			
			// Consume a total of 3 trash, combining across stacks in inventory.
			Queue<Item> offerings = new Queue<Item> ();
			offerings.Enqueue (offering);
			int stack = Math.Min (3, offering.Stack);
			foreach (Item item in Game1.player.Items)
			{
				if (stack == 3)
					break;
				if (object.ReferenceEquals (item, offering))
					continue;
				if (!checkItem (item, category: SObject.junkCategory))
					continue;
				offerings.Enqueue (item);
				stack += Math.Min (3 - stack, item.Stack);
			}
			if (stack < 3)
			{
				showRejection ("rejection.insufficient");
				return true;
			}
			while (stack > 0 && offerings.Count > 0)
			{
				Item offering = offerings.Dequeue ();
				int count = Math.Min (stack, offering.Stack);
				consumeItem (offering, count);
				stack -= count;
			}

			// React to the offering, then proceed to run.
			illuminate ();
			playSound ("trashcan");
			showAnimation ("TileSheets\\animations",
				new Rectangle (256, 1856, 64, 128), 150f, 6, 1);
			showMessage ("garbage.opening", 500);
			Game1.afterDialogues = run;

			return true;
		}

		private static readonly string[] Modes =
			new string[] { "today", "later", "hat", "leave" };

		protected override void doRun ()
		{
			// Show the menu of modes.
			List<Response> modes = Modes.Select ((mode) => new Response (mode,
				Helper.Translation.Get ($"garbage.mode.{mode}"))).ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("garbage.mode.question"), modes);

			Game1.currentLocation.afterQuestion = (Farmer _who, string mode) =>
			{
				Game1.currentLocation.afterQuestion = null;
				WorldDate today = Utilities.Now ();

				switch (mode)
				{
				case "today":
					showPredictions (today, Garbage.ListLootForDate (today), mode);
					break;
				case "later":
					Game1.activeClickableMenu = new DatePicker (today,
						Helper.Translation.Get ("garbage.date.question"), (date) =>
							showPredictions (date, Garbage.ListLootForDate (date), mode));
					break;
				case "hat":
					GarbagePrediction? hat = Garbage.FindGarbageHat (today);
					List<GarbagePrediction> predictions =
						new List<GarbagePrediction> ();
					if (hat.HasValue)
						predictions.Add (hat.Value);
					showPredictions (hat.HasValue ? hat.Value.date : today,
						predictions, mode);
					break;
				case "leave":
				default:
					extinguish ();
					break;
				}
			};
		}

		private void showPredictions (WorldDate date,
			List<GarbagePrediction> predictions, string mode)
		{
			bool today = date == Utilities.Now ();
			List<string> pages = new List<string> ();

			// Show a special message for all cans being empty.
			if (predictions.Count == 0)
			{
				pages.Add (Helper.Translation.Get ($"garbage.none.{mode}", new
				{
					date = date.Localize (),
				}));
			}
			else
			{
				// Build the list of predictions.
				List<string> lines = new List<string>
				{
					Helper.Translation.Get ($"garbage.header.{(today ? "today" : "later")}", new
					{
						date = date.Localize (),
					})
				};

				// Randomize the order of predictions for variety.
				Random rng = new Random ((int) Game1.uniqueIDForThisGame +
					date.TotalDays);

				foreach (GarbagePrediction prediction in
					predictions.OrderBy ((GarbagePrediction a) => rng.Next ()))
				{
					string can = prediction.can.ToString ().Replace ("SVE_", "");
					lines.Add (Helper.Translation.Get ($"garbage.prediction.{can}", new
					{
						itemName = (prediction.loot is Hat)
							? Helper.Translation.Get ("garbage.item.hat")
							: (prediction.loot.ParentSheetIndex == 217)
								? Helper.Translation.Get ("garbage.item.dishOfTheDay")
								: prediction.loot.DisplayName,
					}));
				}
				if (Constants.TargetPlatform != GamePlatform.Android)
					lines.Add (""); // padding for occasional display issues
				pages.Add (string.Join ("^", lines));
			}

			// If checking more cans could alter the results, add an
			// appropriate closing.
			if (Garbage.IsProgressDependent)
			{
				pages.Add (Helper.Translation.Get ("garbage.closing.progress"));
			}

			// Show the predictions.
			showDialogues (pages);
			Game1.afterDialogues = extinguish;
		}
	}
}
