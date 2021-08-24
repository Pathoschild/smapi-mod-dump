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
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeDisabledEventHandler();

	public class SuperModeDisabledEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeDisabled += OnSuperModeDisabled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeDisabled -= OnSuperModeDisabled;
		}

		/// <summary>Raised when IsSuperModeActive is set to false.</summary>
		public void OnSuperModeDisabled()
		{
			Game1.player.stopGlowing();
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarFlashUpdateTickedEvent), typeof(SuperModeCountdownUpdateTickedEvent));

			var buffID = (ModEntry.UniqueID + ModEntry.SuperModeIndex).Hash();
			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == ++buffID);
			if (buff != null) Game1.buffsDisplay.removeOtherBuff(buffID);

			ModEntry.Multiplayer.SendMessage(message: ModEntry.SuperModeIndex, messageType: "SuperModeDectivated", modIDs: new[] { ModEntry.UniqueID });
		}
	}
}
