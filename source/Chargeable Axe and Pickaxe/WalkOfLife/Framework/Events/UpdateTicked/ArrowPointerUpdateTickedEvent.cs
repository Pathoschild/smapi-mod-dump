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
	internal class ArrowPointerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal ArrowPointerUpdateTickedEvent() { }

		/// <summary>Raised after the game state is updated. Update tracking pointer offset for Prospector and Scavenger.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (e.Ticks % 4 == 0) Utility.ArrowPointer.Bob();
		}
	}
}
