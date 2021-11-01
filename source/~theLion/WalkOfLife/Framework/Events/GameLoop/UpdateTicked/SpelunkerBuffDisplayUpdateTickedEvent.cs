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
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
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

		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.currentLocation is not MineShaft) return;

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffID);
			if (buff is not null) return;
			
			var bonusLadderChance = ModEntry.SpelunkerLadderStreak;
			var bonusSpeed = Math.Min(ModEntry.SpelunkerLadderStreak / 5 + 1, 10);
			Game1.buffsDisplay.addOtherBuff(
				new(0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					bonusSpeed,
					0,
					0,
					1,
					"Spelunker",
					ModEntry.ModHelper.Translation.Get("spelunker.name." + (Game1.player.IsMale ? "male" : "female")))
				{
					which = _buffID,
					sheetIndex = SHEET_INDEX,
					millisecondsDuration = 0,
					description =
						ModEntry.ModHelper.Translation.Get("spelunker.buffdesc", new {bonusLadderChance, bonusSpeed})
				}
			);
		}
	}
}