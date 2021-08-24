/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterFilledEventHandler();

	public class SuperModeCounterFilledEvent : BaseEvent
	{
		private readonly SuperModeBarFlashUpdateTickedEvent _superModeBarFlashUpdateTickedEvent = new();

		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeCounterFilled += OnSuperModeCounterFilled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeCounterFilled -= OnSuperModeCounterFilled;
		}

		/// <summary>Raised when SuperModeCounter is set to zero.</summary>
		public void OnSuperModeCounterFilled()
		{
			ModEntry.Subscriber.Subscribe(_superModeBarFlashUpdateTickedEvent);
		}
	}
}
