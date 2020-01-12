using StardewModdingAPI;

namespace SailorStyles_Clothing
{
	class Config
	{
		public SButton debugWarpKey { get; set; }
		public bool debugMode { get; set; }
		public bool debugCate { get; set; }

		public Config()
		{
			debugMode = false;
			debugWarpKey = SButton.U;
			debugCate = true;
		}
	}
}
