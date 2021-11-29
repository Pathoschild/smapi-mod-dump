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

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class SuperModeRenderedWorldEvent : RenderedWorldEvent
	{
		/// <inheritdoc />
		public override void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
		{
			// draw color tint overlay
			e.SpriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds,
				ModState.SuperModeOverlayColor * ModState.SuperModeOverlayAlpha);
		}
	}
}