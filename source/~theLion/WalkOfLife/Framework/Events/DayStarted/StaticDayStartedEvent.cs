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
using StardewValley;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class StaticDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			AwesomeProfessions.EventManager.SubscribeMissingEvents();
			AwesomeProfessions.EventManager.CleanUpRogueEvents();
			LevelUpMenu.RevalidateHealth(Game1.player);
		}
	}
}