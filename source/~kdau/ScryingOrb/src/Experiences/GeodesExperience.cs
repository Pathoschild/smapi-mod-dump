using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public class GeodesExperience : Experience
	{
		public static readonly Dictionary<int, int> StackedOfferings =
			new Dictionary<int, int>
		{
			{ 571, 3 }, // Limestone
			{ 574, 2 }, // Mudstone
		};

		public static readonly Dictionary<string, Geodes.GeodeType?> Types =
			new Dictionary<string, Geodes.GeodeType?>
		{
			{ "any", null },
			{ "Regular", Geodes.GeodeType.Regular },
			{ "Frozen", Geodes.GeodeType.Frozen },
			{ "Magma", Geodes.GeodeType.Magma },
			{ "Omni", Geodes.GeodeType.Omni },
			{ "Trove", Geodes.GeodeType.Trove },
			{ "leave", null }
		};

		public static readonly uint TypedCount =
			(Constants.TargetPlatform == GamePlatform.Android) ? 8u : 10u;

		public override bool isAvailable =>
			base.isAvailable && Geodes.IsAvailable;

		protected override bool check ()
		{
			// Consume an appropriate offering.
			if (!checkOffering (category: SObject.mineralsCategory))
				return false;
			if (!StackedOfferings.TryGetValue (offering.ParentSheetIndex,
					out int count))
				count = 1;
			if (offering.Stack < count)
			{
				showRejection ("rejection.insufficient");
				return true;
			}
			consumeOffering (count);

			// React to the offering, then proceed to run.
			illuminate ();
			playSound ("discoverMineral");
			showAnimation ("TileSheets\\animations",
				new Rectangle (0, 512, 64, 64), 125f, 8, 1);
			showMessage ("geodes.opening", 500);
			Game1.afterDialogues = run;

			return true;
		}

		protected override void doRun ()
		{
			// Show the menu of types.
			List<Response> types = Types.Select ((t) => new Response (t.Key,
				Helper.Translation.Get ($"geodes.type.{t.Key}",
					new { count = TypedCount }))).ToList ();
			if (Constants.TargetPlatform == GamePlatform.Android)
				types.RemoveAll ((r) => r.responseKey == "leave");
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("geodes.type.question"), types);

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
				List<Geodes.Prediction> predictions =
					Geodes.ListTreasures (Game1.player.stats.GeodesCracked + 1,
						(type == "any") ? 3u : TypedCount);
				if (predictions.Count == 0)
				{
					throw new Exception ("Could not predict geode treasures.");
				}

				List<string> pages = new List<string> ();
				string footer = unbreak (Helper.Translation.Get ("geodes.footer"));

				// For the next geode of any type, build a page for each geode
				// with a list of types.
				if (type == "any")
				{
					foreach (Geodes.Prediction p in predictions)
					{
						uint num = p.number - Game1.player.stats.GeodesCracked;
						string header = unbreak (Helper.Translation.Get ($"geodes.header.any{num}"));

						List<string> lines = p.treasures.Select ((tt) =>
						{
							Geodes.Treasure t = tt.Value;
							return string.Join (" ", new string[]
							{
								">",
								t.geodeObject.DisplayName + ":",
								(t.stack > 1) ? t.stack.ToString () : null,
								t.displayName,
								t.valuable ? "$" : null,
								t.needDonation ? "=" : null
							}.Where ((s) => s != null));
						}).ToList ();

						lines.Insert (0, header);
						lines.Add ("");
						lines.Add (footer);
						pages.Add (string.Join ("^", lines));
					}
				}
				// For specific types, build a list of geodes.
				else
				{
					string header = unbreak (Helper.Translation.Get ($"geodes.header.{type}",
						new { count = TypedCount }));

					List<string> lines = predictions.Select ((p) =>
					{
						Geodes.Treasure t =
							p.treasures[Types[type] ?? Geodes.GeodeType.Regular];
						uint num = p.number - Game1.player.stats.GeodesCracked;
						return string.Join (" ", new string[]
						{
							string.Format ("{0,2:D}.", num),
							(t.stack > 1) ? t.stack.ToString () : null,
							t.displayName,
							t.valuable ? "$" : null,
							t.needDonation ? "=" : null
						}.Where ((s) => s != null));
					}).ToList ();

					lines.Insert (0, header);
					if (Constants.TargetPlatform != GamePlatform.Android)
						lines.Add ("");
					lines.Add (footer);
					pages.Add (string.Join ("^", lines));
				}

				// If deeper mine level could alter the results, add an
				// appropriate closing.
				if (Geodes.IsProgressDependent)
				{
					pages.Add (unbreak (Helper.Translation.Get ("geodes.closing.progress")));
				}

				// Show the predictions.
				showDialogues (pages);
				Game1.afterDialogues = extinguish;
			};
		}
	}
}
