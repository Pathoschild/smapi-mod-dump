/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;

namespace Shockah.CommonModCode.Stardew
{
	public static class WorldDateExt
	{
		public static int DaysPerSeason { get; set; } = 28;

		public static WorldDate GetByAddingDays(this WorldDate self, int days)
		{
			if (days == 0)
				return self;

			var day = self.DayOfMonth;
			var season = self.SeasonIndex;
			var year = self.Year;

			day += days;
			while (day > DaysPerSeason)
			{
				day -= DaysPerSeason;
				season++;
			}
			while (day < 0)
			{
				day += DaysPerSeason;
				season--;
			}
			while (season >= 4)
			{
				season -= 4;
				year++;
			}
			while (season < 0)
			{
				season += 4;
				year--;
			}

			return New(year, season, day);
		}

		public static WorldDate New(int year, int seasonIndex, int dayOfMonth)
			=> new(year, Enum.GetName((Season)seasonIndex)!.ToLower(), dayOfMonth);
	}
}
