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
				return Game1.player.activeDialogueEvents.TryGetValue (Topic, out int days)
					? days : -1;
			}

			private set
			{
				Game1.player.activeDialogueEvents.Remove (Topic);
				if (value >= 0)
					Game1.player.activeDialogueEvents.Add (Topic, value);
			}
		}

		public static bool IsActive => Config.UnlimitedUse || DaysRemaining >= 0;

		public static readonly List<int> AcceptedOfferings = new ()
		{
			373, // Golden Pumpkin
			279, // Magic Rock Candy
			797, // Pearl
			74, // Prismatic Shard
			166, // Treasure Chest
		};

		protected override bool check ()
		{
			// In order for EnchantmentsExperience to work, tools have to be
			// skipped here.
			if (Enchantments.IsAvailable && Game1.player.CurrentTool != null)
				return false;

			// If using the cheat, proceed directly to run.
			if (Config.UnlimitedUse)
			{
				run ();
				return true;
			}

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
			Dictionary<string, Experience> experiences = new ()
			{
				{ "mining", new MiningExperience { orb = orb } },
				{ "geodes", new GeodesExperience { orb = orb } },
				{ "enchantments", new EnchantmentsExperience { orb = orb } },
				{ "nightEvents", new NightEventsExperience { orb = orb } },
				{ "garbage", new GarbageExperience { orb = orb } },
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

				// For EnchantmentsExperience, direct the player to offer the tool.
				if (response == "enchantments")
				{
					showMessage ("unlimited.enchantments");
					Game1.afterDialogues = extinguish;
					return;
				}

				Experience experience = experiences[response];
				if (experience != null)
				{
					experience.transferIllumination (this);
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
	}
}
