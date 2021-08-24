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
using System;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ScavengerHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (ModEntry.ScavengerHunt.TreasureTile == null) return;

			// track and reveal treasure hunt target
			Util.HUD.DrawTrackingArrowPointer(ModEntry.ScavengerHunt.TreasureTile.Value, Color.Violet);
			var distanceSquared = (Game1.player.getTileLocation() - ModEntry.ScavengerHunt.TreasureTile.Value).LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				Util.HUD.DrawArrowPointerOverTarget(ModEntry.ScavengerHunt.TreasureTile.Value, Color.Violet);
		}
	}
}