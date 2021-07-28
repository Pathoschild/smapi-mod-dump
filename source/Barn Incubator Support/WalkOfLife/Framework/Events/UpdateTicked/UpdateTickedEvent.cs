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
	internal abstract class UpdateTickedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
		}

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnUpdateTicked(object sender, UpdateTickedEventArgs e);
	}
}