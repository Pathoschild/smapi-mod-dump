using StardewModdingAPI;

namespace VersatileGrabber
{
	class Config
	{
		public SButton debugKey { get; set; }

		public Config()
		{
			debugKey = SButton.J;
		}
	}
}
