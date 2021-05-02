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

namespace TheLion.AwesomeProfessions
{
	internal class BruteUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.bruteKillStreak <= 0) return;

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.BruteBuffID);
			if (buff != null) Game1.buffsDisplay.removeOtherBuff(Utility.BruteBuffID);

			Game1.buffsDisplay.addOtherBuff(
				new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "brute", displaySource: AwesomeProfessions.I18n.Get("brute.name"))
				{
					which = Utility.BruteBuffID,
					sheetIndex = 20,
					millisecondsDuration = 50,
					description = AwesomeProfessions.I18n.Get("brute.buffdescription", new { buffMagnitude = Math.Truncate(AwesomeProfessions.bruteKillStreak * 5.0) / 10 })
				}
			);
		}
	}
}