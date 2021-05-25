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

namespace TheLion.AwesomeProfessions
{
	internal abstract class DayEndingEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.DayEnding += OnDayEnding;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.DayEnding -= OnDayEnding;
		}

		/// <summary>Raised before the game ends the current day.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnDayEnding(object sender, DayEndingEventArgs e);
	}
}