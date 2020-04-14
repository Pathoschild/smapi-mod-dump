using StardewModdingAPI;

namespace SailorStyles_Clothing
{
	internal class Config
	{
		public SButton DebugWarpKey { get; set; }
		public bool DebugMode { get; set; }
		public bool DebugAlwaysCaturday { get; set; }

		public Config()
		{
			DebugAlwaysCaturday = false;
			DebugMode = false;
			DebugWarpKey = SButton.U;
		}
	}
}
