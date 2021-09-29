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
	public class StaticSaveLoadedEvent : SaveLoadedEvent
	{
		/// <inheritdoc/>
		public override void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// load persisted mod data
			ModEntry.Data.Load();

			// set super mode
			ModEntry.SuperModeIndex = ModEntry.Data.ReadField<int>("SuperModeIndex");

			// subcribe player's profession events
			ModEntry.Subscriber.SubscribeEventsForLocalPlayer();
		}
	}
}