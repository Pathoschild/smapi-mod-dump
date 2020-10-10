/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using System.Windows.Forms;

namespace SplitScreen
{
	public static class Utils
	{
		public static bool TrueIsWindowActive() => Form.ActiveForm == Control.FromHandle(StardewValley.Game1.game1.Window.Handle) as Form;
		public static bool IsMouseLocked() => Cursor.Position.X == 0 && Cursor.Position.Y == 0;
	}
}
