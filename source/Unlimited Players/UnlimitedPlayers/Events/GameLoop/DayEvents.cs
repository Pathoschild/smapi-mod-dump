/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Armitxes/StardewValley_UnlimitedPlayers
**
*************************************************/

using StardewModdingAPI.Events;

namespace UnlimitedPlayers.Events.GameLoop
{
	class DayEvents
	{
		public void DayStarted(object sender, DayStartedEventArgs e)
		{
			LazyHelper.OverwritePlayerLimit();
		}
	}
}
