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
	public class SuperModeOverlayFadeOutUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			ModEntry.SuperModeOverlayAlpha -= 0.01f;
			if (ModEntry.SuperModeOverlayAlpha <= 0)
				ModEntry.Subscriber.Unsubscribe(typeof(SuperModeRenderedWorldEvent), GetType());
		}
	}
}