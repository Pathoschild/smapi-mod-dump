using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class UnlimitedExperience : Experience
	{
		private const string Topic = "kdau.ScryingOrb.unlimited";
		public static int DaysRemaining
		{
			get
			{
				return Game1.player.activeDialogueEvents.TryGetValue
					(Topic, out int days)
						? days
						: -1;
			}

			private set
			{
				Game1.player.activeDialogueEvents.Remove (Topic);
				if (value >= 0)
					Game1.player.activeDialogueEvents.Add (Topic, value);
			}
		}

		public static readonly List<int> AcceptedOfferings = new List<int>
		{
			373, // Golden Pumpkin
			279, // Magic Rock Candy
			797, // Pearl
			 74, // Prismatic Shard
			166, // Treasure Chest
		};

		protected override bool check ()
		{
			// If currently in an unlimited period, ignore the offering, react
			// to the ongoing period, then proceed to run.
			if (DaysRemaining >= 0)
			{
				illuminate ();
				playSound ("yoba");
				showMessage ((DaysRemaining == 0)
					? "unlimited.lastDay" : "unlimited.following", 250);
				Game1.afterDialogues = run;
				return true;
			}

			// Consume an appropriate offering.
			if (!checkOffering (accepted: AcceptedOfferings))
				return false;
			consumeOffering ();

			// Start an unlimited period and increase luck for the day.
			DaysRemaining = 7;
			Game1.player.team.sharedDailyLuck.Value = 0.12;

			// React to the offering dramatically, then proceed to run.
			illuminate ();
			playSound ("reward");
			showAnimation ("TileSheets\\animations",
				new Rectangle (0, 192, 64, 64), 125f, 8, 1);
			showMessage ($"unlimited.initial.main", 1000);
			Game1.afterDialogues = run;

			return true;
		}

		protected override void doRun ()
		{
			// In case we were called directly by ModEntry.
			illuminate ();

			// Show the menu of experiences.
			Dictionary<string, Experience> experiences =
				new Dictionary<string, Experience>
			{
				{ "mining", new MiningExperience { orb = orb } },
				{ "geodes", new GeodesExperience { orb = orb } },
				{ "nightEvents", new NightEventsExperience { orb = orb } },
				// TODO: { "shopping", new ShoppingExperience { Orb = Orb } },
				{ "garbage", new GarbageExperience { orb = orb } },
				// TODO: { "itemFinder", new ItemFinderExperience { Orb = Orb } },
				{ "leave", null }
			};
			List<Response> choices = experiences
				.Where ((e) => e.Value == null || e.Value.isAvailable)
				.Select ((e) => new Response (e.Key,
					Helper.Translation.Get ($"unlimited.experience.{e.Key}")))
				.ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("unlimited.experience.question"), choices);

			// Hand over control to the selected experience. Since that class
			// may also use afterQuestion and uses of it can't be synchronously
			// nested, use a nominal DelayedAction to break out of it.
			Game1.currentLocation.afterQuestion = (Farmer _who, string response) =>
				DelayedAction.functionAfterDelay (() =>
			{
				Game1.currentLocation.afterQuestion = null;
				Experience experience = experiences[response];
				if (experience != null)
				{
					experience.run ();
				}
				else
				{
					extinguish ();
				}
			}, 1);
		}

		internal static void Reset ()
		{
			DaysRemaining = -1;
		}

		internal static void MigrateData ()
		{
			// Carry over any unlimited period from the old persistence format.
			if (Context.IsMainPlayer && DaysRemaining == -1)
			{
				var old = Helper.Data.ReadSaveData<OldData> ("Unlimited");
				if (old != null)
				{
					int days = old.ExpirationDay - Utilities.Now ().TotalDays;
					if (days >= 0)
						DaysRemaining = days;
					Helper.Data.WriteSaveData<OldData> ("Unlimited", null);
				}
			}
		}

		private class OldData
		{
			public int ExpirationDay { get; set; } = -1;
		}
	}
}
