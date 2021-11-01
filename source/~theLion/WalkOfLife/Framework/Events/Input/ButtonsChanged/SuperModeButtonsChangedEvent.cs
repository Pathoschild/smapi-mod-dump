/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeButtonsChangedEvent : ButtonsChangedEvent
	{
		/// <inheritdoc />
		public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (ModEntry.Config.SuperModeKey.JustPressed() && !ModEntry.IsSuperModeActive &&
				ModEntry.SuperModeCounter >= ModEntry.SuperModeCounterMax)
			{
				if (ModEntry.Config.HoldKeyToActivateSuperMode)
					ModEntry.Subscriber.Subscribe(new SuperModeActivationTimerUpdateTickedEvent());
				else
					ModEntry.IsSuperModeActive = true;
			}
			else if (ModEntry.Config.SuperModeKey.GetState() == SButtonState.Released)
			{
				ModEntry.Subscriber.Unsubscribe(typeof(SuperModeActivationTimerUpdateTickedEvent));
			}
		}
	}
}