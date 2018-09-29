using System.Windows.Forms;

namespace SplitScreen
{
	public static class Utils
	{
		public static bool TrueIsWindowActive() => Form.ActiveForm == Control.FromHandle(StardewValley.Game1.game1.Window.Handle) as Form;
		public static bool IsMouseLocked() => Cursor.Position.X == 0 && Cursor.Position.Y == 0;
	}
}
