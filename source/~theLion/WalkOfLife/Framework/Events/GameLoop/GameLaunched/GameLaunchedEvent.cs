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
	internal abstract class GameLaunchedEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.GameLoop.GameLaunched -= OnGameLaunched;
		}

		/// <summary>Raised after the game is launched, right before the first update tick.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public abstract void OnGameLaunched(object sender, GameLaunchedEventArgs e);
	}
}