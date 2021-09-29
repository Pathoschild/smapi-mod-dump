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
	public abstract class SavingEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.ModHelper.Events.GameLoop.Saving += OnSaving;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.GameLoop.Saving -= OnSaving;
		}

		/// <summary>Raised before the game writes data to save file.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public abstract void OnSaving(object sender, SavingEventArgs e);
	}
}