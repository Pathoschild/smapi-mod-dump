/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Leclair.Stardew.Common.Enums;

namespace Leclair.Stardew.Almanac.Models {

	public class LocationOverride {
		public string Map;
		public int Zone { get; set; } = -1;
		public Season Season { get; set; } = Season.All;

		public string[] AddFish { get; set; } = null;
		public string[] RemoveFish { get; set; } = null;
	}
}
