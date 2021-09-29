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
	public abstract class ButtonsChangedEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.ModHelper.Events.Input.ButtonsChanged += OnButtonsChanged;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.Input.ButtonsChanged -= OnButtonsChanged;
		}

		/// <summary>Raised after the player pressed/released any buttons on the keyboard, mouse, or controller.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnButtonsChanged(object sender, ButtonsChangedEventArgs e);
	}
}