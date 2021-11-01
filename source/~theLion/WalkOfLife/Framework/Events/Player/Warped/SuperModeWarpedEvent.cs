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
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

			if (e.NewLocation.IsCombatZone())
			{
				ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent());
			}
			else
			{
				ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarFadeOutUpdateTickedEvent),
					typeof(SuperModeBarShakeTimerUpdateTickedEvent), typeof(SuperModeBarRenderingHudEvent));

				ModEntry.SuperModeCounter = 0;
				ModEntry.SuperModeBarAlpha = 1f;
				ModEntry.ShouldShakeSuperModeBar = false;
			}
		}
	}
}