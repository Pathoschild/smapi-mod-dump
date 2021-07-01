/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ScryingOrb
{
	public class MetaExperience : Experience
	{
		private const string TriedFlag =
			"kdau.ScryingOrb.triedMetaOffering";

		protected override bool check ()
		{
			// Only accept a Scrying Orb. Don't consume it.
			List<int> accepted = new () { ModEntry.Instance.parentSheetIndex };
			if (!checkOffering (accepted: accepted, bigCraftable: true))
				return false;

			// If the player has tried this before, react nonchalantly.
			if (Game1.player.mailReceived.Contains (TriedFlag))
			{
				playSound ("clank");
				showMessage ("meta.following", 500);
			}
			// Otherwise show the initial joke.
			else
			{
				Game1.player.mailReceived.Add (TriedFlag);
				playSound ("clank");
				showMessage ("meta.initial", 500);
			}

			return true;
		}

		protected override void doRun ()
		{ }

		internal static void Reset ()
		{
			Game1.player.mailReceived.Remove (TriedFlag);
		}
	}
}
