/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableSpecialOrdersUnlock
**
*************************************************/

// Configurable Special Orders Unlock mod for Stardew Valley
// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

namespace ConfigurableSpecialOrdersUnlock
{
	/// <summary>
	/// Utilized by SMAPI to store configurable values. Can be modified by hand or by use of GMCM.
	/// </summary>
	public class ModConfig
	{
		public int unlockYear = 1;
		public string unlockSeason = "Fall";
		public int unlockDay = 2;

		public bool skipCutscene = false;

		/// <summary>
		/// Converts the provided year, season and day provided by the config file to an <c>Int</c> number of days played.
		/// </summary>
		public int GetUnlockDaysPlayed()
		{
			return ((Clamp(unlockYear, 1, 10) - 1) * 4 + GetSeasonIndex(unlockSeason)) * 28 + Clamp(unlockDay, 1, 28);
		}

		public int GetSeasonIndex(string season)
		{
			switch (season.ToLower())
			{
				case "spring": return 0;
				case "summer": return 1;
				case "fall": return 2;
				case "winter": return 3;
				default: return 0;
			}
		}

		/// <summary>
		/// Clamps the passed in value to be within <c>min</c>(inclusive) and <c>max</c>(inclusive).
		/// </summary>
		public int Clamp(int num, int min, int max)
		{
			if (num < min) return min;
			else if (num > max) return max;
			else return num;
		}
	}
}