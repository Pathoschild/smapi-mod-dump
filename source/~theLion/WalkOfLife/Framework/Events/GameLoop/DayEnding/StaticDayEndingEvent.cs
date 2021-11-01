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
	public class StaticDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc />
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			// fix dumb shit with other mods
			ModEntry.Subscriber.CleanUpRogueEvents();
			ModEntry.Data.CleanUpRogueDataFields();
			ModEntry.Subscriber.SubscribeEventsForLocalPlayer();
			ModEntry.Data.InitializeDataFieldsForLocalPlayer();
		}
	}
}