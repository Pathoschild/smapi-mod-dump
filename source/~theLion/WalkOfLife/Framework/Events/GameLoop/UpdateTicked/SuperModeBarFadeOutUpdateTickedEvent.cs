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
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarFadeOutUpdateTickedEvent : UpdateTickedEvent
	{
		private const int FADE_OUT_DELAY_I = 120, FADE_OUT_DURATION_I = 30;

		private int _fadeOutTimer = FADE_OUT_DELAY_I + FADE_OUT_DURATION_I;

		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			--_fadeOutTimer;
			if (_fadeOutTimer >= FADE_OUT_DURATION_I) return;

			var ratio = (float) _fadeOutTimer / FADE_OUT_DURATION_I;
			ModEntry.SuperModeBarAlpha = (float) (-1.0 / (1.0 + Math.Exp(12.0 * ratio - 6.0)) + 1.0);

			if (_fadeOutTimer > 0) return;

			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarRenderingHudEvent), GetType());
			ModEntry.SuperModeBarAlpha = 1f;
		}
	}
}