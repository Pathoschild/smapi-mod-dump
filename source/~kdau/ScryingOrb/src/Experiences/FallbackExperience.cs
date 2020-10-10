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
	public class FallbackExperience : Experience
	{
		protected override bool check ()
		{
			return true;
		}

		protected override void doRun ()
		{
			showRejection ("rejection.unrecognized");
		}
	}
}
