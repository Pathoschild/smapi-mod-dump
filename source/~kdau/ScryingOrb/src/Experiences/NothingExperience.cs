/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using StardewValley;

namespace ScryingOrb
{
	public class NothingExperience : Experience
	{
		protected override bool check ()
		{
			if (!isAvailable)
				return false;
			
			if (Game1.player.CurrentItem != null &&
					!(Game1.player.CurrentItem is Tool))
				return false;

			showMessage ("rejection.nothing");
			return true;
		}

		protected override void doRun ()
		{}
	}
}
