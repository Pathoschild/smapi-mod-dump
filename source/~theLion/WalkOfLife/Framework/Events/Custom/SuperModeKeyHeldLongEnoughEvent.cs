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
	public delegate void SuperModeKeyHeldLongEnoughEventHandler();

	public class SuperModeKeyHeldLongEnoughEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeKeyHeldLongEnough += OnSuperModeKeyHeldLongEnough;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeKeyHeldLongEnough -= OnSuperModeKeyHeldLongEnough;
		}

		/// <summary>Raised when SuperModeKeyTimer is set to zero.</summary>
		public void OnSuperModeKeyHeldLongEnough()
		{
			ModEntry.IsSuperModeActive = true;
		}
	}
}
