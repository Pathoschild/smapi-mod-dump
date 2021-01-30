/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shalankwa/SDV_Mods
**
*************************************************/

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
