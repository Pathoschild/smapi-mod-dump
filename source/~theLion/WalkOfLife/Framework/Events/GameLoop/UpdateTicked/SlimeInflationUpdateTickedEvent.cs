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
using StardewValley;
using System;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			var uninflatedSlimes = ModEntry.PipedSlimesScales.Keys.ToList();
			while (uninflatedSlimes.Any())
			{
				for (int i = uninflatedSlimes.Count - 1; i >= 0; --i)
				{
					uninflatedSlimes[i].Scale = Math.Min(uninflatedSlimes[i].Scale * 1.1f, Math.Min(ModEntry.PipedSlimesScales[uninflatedSlimes[i]] * 2f, 2f));

					if (uninflatedSlimes[i].Scale >= 1.8f) uninflatedSlimes[i].willDestroyObjectsUnderfoot = true;

					if ((uninflatedSlimes[i].Scale >= 1f && Game1.random.NextDouble() < 0.1 + Game1.player.DailyLuck / 2 + Game1.player.LuckLevel * 0.01) || uninflatedSlimes[i].Scale >= ModEntry.PipedSlimesScales[uninflatedSlimes[i]] * 2f)
						uninflatedSlimes.RemoveAt(i);
				}
			}

			ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}