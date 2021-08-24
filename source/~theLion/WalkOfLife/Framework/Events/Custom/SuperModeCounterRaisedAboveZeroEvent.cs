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
	public delegate void SuperModeCounterRaisedAboveZeroEventHandler();

	public class SuperModeCounterRaisedAboveZeroEvent : BaseEvent
	{
		private readonly SuperModeBarRenderedHudEvent _superModeBarRenderedHudEvent = new();
		private readonly SuperModeBuffsDisplayUpdateTickedEvent _superModeBuffsDisplayUpdateTickedEvent = new();

		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeCounterRaisedAboveZero += OnSuperModeCounterRaisedAboveZero;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeCounterRaisedAboveZero -= OnSuperModeCounterRaisedAboveZero;
		}

		/// <summary>Raised when the SuperModeCounter is raised from zero to any value greater than zero.</summary>
		public void OnSuperModeCounterRaisedAboveZero()
		{
			ModEntry.Subscriber.Subscribe(_superModeBarRenderedHudEvent, _superModeBuffsDisplayUpdateTickedEvent);
		}
	}
}
