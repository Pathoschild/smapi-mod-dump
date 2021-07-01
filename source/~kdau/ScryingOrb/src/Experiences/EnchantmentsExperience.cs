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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class EnchantmentsExperience : Experience
	{
		public override bool isAvailable => Enchantments.IsAvailable;

		protected override bool check ()
		{
			// Check that the current item is a tool.
			if (!isAvailable || Game1.player.CurrentTool == null)
				return false;

			// Find and consume 5 Cinder Shard as the offering (unless unlimited).
			if (!UnlimitedExperience.IsActive)
			{
				foreach (Item item in Game1.player.Items)
				{
					if (checkItem (item, accepted: new List<int> { 848 }))
					{
						offering = item;
						break;
					}
				}
				if (offering == null || offering.Stack < 5)
				{
					showRejection ("rejection.insufficient");
					return true;
				}
				consumeOffering (5);
			}

			// React to the offerings, then proceed to run.
			illuminate ();
			playSound ("crafting");
			showAnimation ("TileSheets\\animations",
				new Rectangle (0, 256, 64, 64), 125f, 8, 1);
			showMessage ("enchantments.opening", 500);
			Game1.afterDialogues = run;

			return true;
		}

		protected override void doRun ()
		{
			// Show the question about cumulative enchantment.
			List<Response> responses = new ()
			{
				new Response ("yes", Helper.Translation.Get ($"enchantments.cumulative.yes")),
				new Response ("no", Helper.Translation.Get ($"enchantments.cumulative.no")),
			};
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("enchantments.cumulative.question"), responses);

			Game1.currentLocation.afterQuestion = (Farmer _who, string response) =>
			{
				Game1.currentLocation.afterQuestion = null;
				bool concurrent = response == "yes";

				// Gather the appropriate predictions.
				List<Enchantments.Prediction> predictions =
					Enchantments.ListForTool (Game1.player.CurrentTool, 8u, concurrent);
				if (predictions.Count == 0)
				{
					throw new Exception ("Could not predict enhancements.");
				}

				// Build the list of predictions.
				List<string> lines = new ()
				{
					Helper.Translation.Get ($"enchantments.header.{response}", new
					{
						toolName = Game1.player.CurrentTool.DisplayName,
					})
				};
				uint firstNumber = predictions.First ().number;
				foreach (var prediction in predictions)
				{
					lines.Add (unbreak (Helper.Translation.Get ($"enchantments.prediction", new
					{
						num = prediction.number - firstNumber + 1u,
						name = prediction.enchantment?.GetDisplayName () ?? "-",
					}).ToString ()));
				}

				// Show a list of the predictions.
				showDialogues (new List<string> { string.Join ("^", lines) });
				Game1.afterDialogues = extinguish;
			};
		}
	}
}
