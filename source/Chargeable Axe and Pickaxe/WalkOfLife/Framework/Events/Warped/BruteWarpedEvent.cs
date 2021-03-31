/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class BruteWarpedEvent : WarpedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal BruteWarpedEvent() { }

		/// <summary>Raised after the current player moves to a new location. Reset Brute buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && AwesomeProfessions.bruteKillStreak > 0 && e.NewLocation.GetType() != e.OldLocation.GetType())
				AwesomeProfessions.bruteKillStreak = 0;
		}
	}
}
