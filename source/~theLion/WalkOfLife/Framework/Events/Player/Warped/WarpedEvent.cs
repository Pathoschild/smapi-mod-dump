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
	internal abstract class WarpedEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.Player.Warped += OnWarped;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.Player.Warped -= OnWarped;
		}

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnWarped(object sender, WarpedEventArgs e);
	}
}