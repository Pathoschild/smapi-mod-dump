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
