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
	internal abstract class ButtonsChangedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.Input.ButtonsChanged += OnButtonsChanged;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.Input.ButtonsChanged -= OnButtonsChanged;
		}

		/// <summary>Raised after the player released a keyboard, mouse, or controller button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnButtonsChanged(object sender, ButtonsChangedEventArgs e);
	}
}