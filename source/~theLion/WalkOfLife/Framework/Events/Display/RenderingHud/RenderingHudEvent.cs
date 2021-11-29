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

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal abstract class RenderingHudEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.Display.RenderingHud += OnRenderingHud;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.Display.RenderingHud -= OnRenderingHud;
		}

		/// <summary>Raised before the game draws anything to the screen in a draw tick, as soon as the sprite batch is opened.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderingHud(object sender, RenderingHudEventArgs e);
	}
}