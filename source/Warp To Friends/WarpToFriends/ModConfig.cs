using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace WarpToFriends
{
	public class ModConfig
	{

		[OptionDisplay("Open Menu Key")]
		public SButton OpenMenuKey { get; set; } = SButton.J;

		[OptionDisplay("Allow Console Warp")]
		public bool canConsoleWarp { get; set; } = true;
	}
}
