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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SplitScreen.Menu
{
	abstract class BaseTextButton : BaseButton
	{
		protected string text = "";

		public static int Height => 55;

		public BaseTextButton(int x, int y, string text, Texture2D sourceTexture = null, Rectangle? sourceRectangle = null)
			: base(new Rectangle(), null, null)
		{
			this.text = text;

			base.bounds = new Rectangle(x, y, (int)Game1.smallFont.MeasureString(text).X + 20, Height);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			Utility.drawTextWithShadow(spriteBatch, this.text, Game1.smallFont, new Vector2(base.bounds.X + 10f, base.bounds.Y + 10f), Game1.textColor, 1f, 1f, -1, -1, 0f, 3);
		}
	}
}
