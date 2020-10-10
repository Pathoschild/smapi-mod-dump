/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using StardewValley;

namespace SplitScreen.Menu
{
	class ToggleBordersButton : BaseTextButton
	{
		private static bool isBorderless = false;

		public ToggleBordersButton(int x, int y) : base (x,y, "Toggle borders")
		{

		}

		public override void OnClicked()
		{
			var form = System.Windows.Forms.Control.FromHandle(Game1.game1.Window.Handle).FindForm();
			form.Bounds = new System.Drawing.Rectangle(form.Bounds.X + (isBorderless ? -7 : 7), form.Bounds.Y, form.Bounds.Width, form.Bounds.Height);
			form.FormBorderStyle = isBorderless ? System.Windows.Forms.FormBorderStyle.Sizable : System.Windows.Forms.FormBorderStyle.None;
			isBorderless = !isBorderless;
		}
	}
}
