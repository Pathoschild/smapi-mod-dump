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
	public class StaticReturnedToTitleEvent : ReturnedToTitleEvent
	{
		/// <inheritdoc/>
		public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// release mod data
			ModEntry.Data.Unload();

			// reset super mode
			if (ModEntry.SuperModeIndex > 0) ModEntry.SuperModeIndex = -1;

			// unsubscribe player's profession events
			ModEntry.Subscriber.UnsubscribeLocalPlayerEvents();
		}
	}
}