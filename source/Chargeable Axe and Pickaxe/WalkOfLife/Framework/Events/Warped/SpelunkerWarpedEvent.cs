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
using StardewValley.Locations;

namespace TheLion.AwesomeProfessions
{
	internal class SpelunkerWarpedEvent : WarpedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal SpelunkerWarpedEvent() { }

		/// <summary>Raised after the current player moves to a new location. Record Spelunker lowest level reached.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && e.NewLocation is MineShaft)
			{
				uint currentMineLevel = (uint)(e.NewLocation as MineShaft).mineLevel;
				if (currentMineLevel > Data.LowestMineLevelReached) Data.LowestMineLevelReached = currentMineLevel;
			}
		}
	}
}
