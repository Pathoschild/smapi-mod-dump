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
