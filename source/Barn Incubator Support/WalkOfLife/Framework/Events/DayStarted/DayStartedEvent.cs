/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class DayStartedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.DayStarted += OnDayStarted;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.DayStarted -= OnDayStarted;
		}

		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnDayStarted(object sender, DayStartedEventArgs e);
	}
}