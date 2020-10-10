/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SplitScreen.Patchers
{
	public class FakeMouse
	{
		public static int X { set; get; }
		public static int Y { set; get; }

		public static Point GetPoint() => new Point(X, Y);
	}
}
