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
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class SpelunkerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLocation is not MineShaft shaft) return;

			var currentMineLevel = (uint)(shaft).mineLevel;
			if (currentMineLevel > AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", uint.Parse))
				AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", currentMineLevel.ToString());
		}
	}
}