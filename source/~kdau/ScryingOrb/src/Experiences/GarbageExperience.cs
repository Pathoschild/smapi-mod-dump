/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
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
		public override bool isAvailable => Garbage.IsAvailable;

		protected override bool check ()
		{
			// Require an appropriate offering.
			if (!checkOffering (category: SObject.junkCategory))
				return false;

			// Consume a total of 3 trash, combining across stacks in inventory.
			Queue<Item> offerings = new ();
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
				SDate today = SDate.Now ();

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
					Garbage.Prediction? hat = Garbage.FindGarbageHat (today);
					List<Garbage.Prediction> predictions = new ();
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

		private void showPredictions (SDate date,
			List<Garbage.Prediction> predictions, string mode)
		{
			bool today = date == SDate.Now ();
			List<string> pages = new ();

			// Show a special message for all cans being empty.
			if (predictions.Count == 0)
			{
				pages.Add (Helper.Translation.Get ($"garbage.none.{mode}", new
				{
					date = date.ToLocaleString (),
				}));
			}
			else
			{
				// Build the list of predictions.
				List<string> lines = new ()
				{
					Helper.Translation.Get ($"garbage.header.{(today ? "today" : "later")}", new
					{
						date = date.ToLocaleString (),
					})
				};

				// Randomize the order of predictions for variety.
				Random rng = new ((int) Game1.uniqueIDForThisGame + date.DaysSinceStart);

				foreach (Garbage.Prediction prediction in
					predictions.OrderBy ((Garbage.Prediction a) => rng.Next ()))
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
