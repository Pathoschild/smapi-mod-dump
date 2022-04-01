/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nymvaline/StardewValley-AnkiStudyBreak
**
*************************************************/

using System;

namespace AnkiStudyBreak {
	public class ModConfig
	{
		public bool AnkiBreakOnDayStart { get; set; }
		public int NumAnkiCardsForDayStart { get; set; }
		public bool AnkiBreakAtIntervals { get; set; }
		public int NumAnkiCardsForIntervals { get; set; }
		public int NumHoursForIntervals { get; set; }
		public ModConfig()
		{
			this.AnkiBreakOnDayStart = true;
			this.NumAnkiCardsForDayStart = 15;
			this.AnkiBreakAtIntervals = false;
			this.NumAnkiCardsForIntervals = 15;
			this.NumHoursForIntervals = 3;
		}
	}
}