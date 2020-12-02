/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace SailorStyles
{
	internal class Config
	{
		public bool EnableHairstyles { get; set; } = true;
		public List<string> ExtraContentPacksToSellInTheShop { get; set; } = new List<string>
		{
			"JA.kisekaeskirts",
			"new.pleatedskirtja",
		};
		public bool DebugCaturday { get; set; } = false;
		public bool DebugMode { get; set; } = false;
	}
}
