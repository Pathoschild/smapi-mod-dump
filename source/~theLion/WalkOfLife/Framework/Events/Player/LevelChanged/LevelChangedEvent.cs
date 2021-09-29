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
	public abstract class LevelChangedEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.ModHelper.Events.Player.LevelChanged += OnLevelChanged;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.Player.LevelChanged -= OnLevelChanged;
		}

		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnLevelChanged(object sender, LevelChangedEventArgs e);
	}
}