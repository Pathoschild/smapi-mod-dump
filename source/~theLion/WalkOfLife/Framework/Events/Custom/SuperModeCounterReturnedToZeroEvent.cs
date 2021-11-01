/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterReturnedToZeroEventHandler();

	public class SuperModeCounterReturnedToZeroEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeCounterReturnedToZero += OnSuperModeCounterReturnedToZero;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeCounterReturnedToZero -= OnSuperModeCounterReturnedToZero;
		}

		/// <summary>Raised when SuperModeCounter is set to zero.</summary>
		public void OnSuperModeCounterReturnedToZero()
		{
			if (!ModEntry.IsSuperModeActive) return;
			ModEntry.IsSuperModeActive = false;

			if (!Game1.currentLocation.IsCombatZone())
				ModEntry.Subscriber.Subscribe(new SuperModeBarFadeOutUpdateTickedEvent());
		}
	}
}