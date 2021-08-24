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
using StardewValley.Locations;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SpelunkerBuffDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int SHEET_INDEX = 40;

		private readonly int _buffID;

		/// <summary>Construct an instance.</summary>
		internal SpelunkerBuffDisplayUpdateTickedEvent()
		{
			_buffID = (ModEntry.UniqueID + Util.Professions.IndexOf("Spelunker")).Hash();
		}

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.currentLocation is not MineShaft) return;

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffID);
			if (buff != null) return;

			var bonusLadderChance = ModEntry.SpelunkerLadderStreak;
			var bonusSpeed = bonusLadderChance / 10 + 1;
			Game1.buffsDisplay.addOtherBuff(
				new Buff(0,
						 0,
						 0,
						 0,
						 0,
						 0,
						 0,
						 0,
						 0,
						 speed: bonusSpeed,
						 0,
						 0,
						 minutesDuration: 1,
						 source: "Spelunker",
						 displaySource: ModEntry.I18n.Get("spelunker.buffname"))
				{
					which = _buffID,
					sheetIndex = SHEET_INDEX,
					millisecondsDuration = 49,
					description = ModEntry.I18n.Get("spelunker.buffdescription", new { bonusLadderChance, bonusSpeed })
				}
			);
		}
	}
}