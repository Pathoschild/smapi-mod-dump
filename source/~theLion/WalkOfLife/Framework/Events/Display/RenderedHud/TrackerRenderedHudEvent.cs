/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class TrackerRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			// reveal on-sreen trackable objects
			foreach (var pair in Game1.currentLocation.Objects.Pairs.Where(p => p.Value.ShouldBeTracked()))
				Util.HUD.DrawArrowPointerOverTarget(pair.Key, Color.Yellow);

			if (!Game1.player.HasProfession("Prospector") || Game1.currentLocation is not MineShaft shaft) return;

			// reveal on-screen ladders and shafts
			foreach (var tile in Util.Tiles.GetLadderTiles(shaft)) Util.HUD.DrawTrackingArrowPointer(tile, Color.Lime);
		}
	}
}