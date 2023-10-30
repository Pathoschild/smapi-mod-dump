/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using StardewValley;
using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Hooks
{
	internal static class DayStarted
	{
		/// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, DayStartedEventArgs e)
		{
			if (Game1.IsMasterGame)
				BuildingHelper.Update();
		}
	}
}
