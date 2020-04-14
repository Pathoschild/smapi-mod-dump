using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace ScryingOrb
{
	public class LuckyPurpleExperience : Experience
	{
		public static readonly List<int> AcceptedOfferings = new List<int>
		{
			789, // Lucky Purple Shorts
			 71, // Trimmed Lucky Purple Shorts (object version)
		};

		private const string TriedFlag =
			"kdau.ScryingOrb.triedLuckyPurpleShorts";

		protected override bool check ()
		{
			// Only accept the Lucky Purple Shorts or their trimmed version,
			// (only object, as clothing can't be offered). Don't consume them.
			if (!checkOffering (accepted: AcceptedOfferings))
				return false;

			// If the player hasn't tried this before, show the initial warning.
			if (!Game1.player.mailReceived.Contains (TriedFlag))
			{
				Game1.player.mailReceived.Add (TriedFlag);
				playSound ("grunt");
				showMessage ("luckyPurple.initial", 500);
			}
			// The next time, react dramatically and sour their luck for the day.
			else if (Game1.player.team.sharedDailyLuck.Value > -0.12)
			{
				illuminate (255, 0, 0);
				playSound ("death");
				showAnimation ("TileSheets\\animations",
					new Rectangle (0, 1920, 64, 64), 250f, 4, 2);
				showMessage ("luckyPurple.following", 1000);
				Game1.player.team.sharedDailyLuck.Value = -0.12;
				Game1.afterDialogues = extinguish;
			}
			// But only once a day.
			else
			{
				return false;
			}

			return true;
		}

		protected override void doRun ()
		{}

		internal static void Reset ()
		{
			Game1.player.mailReceived.Remove (TriedFlag);
		}
	}
}
