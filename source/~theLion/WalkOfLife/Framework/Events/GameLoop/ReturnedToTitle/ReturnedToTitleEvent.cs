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
	public abstract class ReturnedToTitleEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle -= OnReturnedToTitle;
		}

		/// <summary>Raised after the game returns to the title screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public abstract void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e);
	}
}