/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			// reveal on-sreen trackable objects
			foreach (var pair in Game1.currentLocation.Objects.Pairs.Where(p => Utility.ShouldPlayerTrackObject(p.Value)))
				Utility.DrawArrowPointerOverTarget(pair.Key, Color.Yellow);

			if (!Utility.LocalPlayerHasProfession("Prospector") || Game1.currentLocation is not MineShaft shaft) return;

			// reveal on-screen ladders and shafts
			foreach (var tile in Utility.GetLadderTiles(shaft)) Utility.DrawTrackingArrowPointer(tile, Color.Lime);
		}
	}
}