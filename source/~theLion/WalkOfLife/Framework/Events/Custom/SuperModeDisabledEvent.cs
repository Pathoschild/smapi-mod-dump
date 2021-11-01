/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;

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
			// remove countdown and fade out overlay
			ModEntry.Subscriber.Subscribe(new SuperModeOverlayFadeOutUpdateTickedEvent());
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCountdownUpdateTickedEvent));

			// notify peers
			ModEntry.ModHelper.Multiplayer.SendMessage(ModEntry.SuperModeIndex, "SuperModeDectivated",
				new[] { ModEntry.UniqueID });

			// remove permanent effects
			if (ModEntry.SuperModeIndex != Util.Professions.IndexOf("Piper")) return;
			
			// depower
			foreach (var slime in ModEntry.PipedSlimeScales.Keys)
				slime.DamageToFarmer = (int) Math.Round(slime.DamageToFarmer / slime.Scale);

			// degorge
			ModEntry.Subscriber.Subscribe(new SlimeDeflationUpdateTickedEvent());
		}
	}
}