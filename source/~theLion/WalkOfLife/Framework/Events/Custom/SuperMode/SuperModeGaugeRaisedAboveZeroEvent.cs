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
	public delegate void SuperModeGaugeRaisedAboveZeroEventHandler();

	internal class SuperModeGaugeRaisedAboveZeroEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModState.SuperModeGaugeRaisedAboveZero += OnSuperModeGaugeRaisedAboveZero;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModState.SuperModeGaugeRaisedAboveZero -= OnSuperModeGaugeRaisedAboveZero;
		}

		/// <summary>Raised when SuperModeGauge is raised from zero to any value greater than zero.</summary>
		public void OnSuperModeGaugeRaisedAboveZero()
		{
			// stop waiting for gauge to return to zero and start waiting for it to fill up
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeGaugeReturnedToZeroEvent));
			ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent(),
				new SuperModeBuffDisplayUpdateTickedEvent(), new SuperModeGaugeFilledEvent());
		}
	}
}