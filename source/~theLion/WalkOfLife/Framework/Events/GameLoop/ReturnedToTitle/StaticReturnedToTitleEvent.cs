/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	[UsedImplicitly]
	internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
	{
		/// <inheritdoc />
		public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// release mod data
			ModEntry.Data.Unload();

			// unsubscribe events
			ModEntry.Subscriber.UnsubscribeLocalPlayerEvents();

			// reset Super Mode
			if (ModState.SuperModeIndex > 0) ModState.SuperModeIndex = -1;
		}
	}
}