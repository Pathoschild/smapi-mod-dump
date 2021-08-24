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
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SpelunkerWarpedEvent : WarpedEvent
	{
		private static readonly SpelunkerBuffDisplayUpdateTickedEvent SpelunkerUpdateTickedEvent = new();

		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			if (e.NewLocation is MineShaft)
			{
				++ModEntry.SpelunkerLadderStreak;
				ModEntry.Subscriber.Subscribe(SpelunkerUpdateTickedEvent);
			}
			else
			{
				ModEntry.SpelunkerLadderStreak = 0;
				ModEntry.Subscriber.Unsubscribe(SpelunkerUpdateTickedEvent.GetType());
			}

			//e.Player.health = Math.Min(e.Player.health + 11, e.Player.maxHealth);
			//e.Player.Stamina = Math.Min(e.Player.Stamina + 25f, e.Player.MaxStamina);
		}
	}
}