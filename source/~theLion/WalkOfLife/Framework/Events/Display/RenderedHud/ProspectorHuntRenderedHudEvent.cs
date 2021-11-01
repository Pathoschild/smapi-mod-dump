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
using TheLion.Stardew.Professions.Framework.Util;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc />
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (ModEntry.ProspectorHunt.TreasureTile is null) return;

			// reveal treasure hunt target
			var distanceSquared = (Game1.player.getTileLocation() - ModEntry.ProspectorHunt.TreasureTile.Value)
				.LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				HUD.DrawArrowPointerOverTarget(ModEntry.ProspectorHunt.TreasureTile.Value, Color.Violet);
		}
	}
}