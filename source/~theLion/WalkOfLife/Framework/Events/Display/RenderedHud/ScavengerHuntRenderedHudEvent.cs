/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ScavengerHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc />
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (ModState.ScavengerHunt.TreasureTile is null) return;

			// track and reveal treasure hunt target
			HUD.DrawTrackingArrowPointer(ModState.ScavengerHunt.TreasureTile.Value, Color.Violet);
			var distanceSquared = (Game1.player.getTileLocation() - ModState.ScavengerHunt.TreasureTile.Value)
				.LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				HUD.DrawArrowPointerOverTarget(ModState.ScavengerHunt.TreasureTile.Value, Color.Violet);
		}
	}
}