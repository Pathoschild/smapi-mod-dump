using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ServerBrowser
{
	static class SpriteFontExtensions
	{
		public static Rectangle GetBounds(this SpriteFont font, string text, int x, int y)
		{
			var a = font.MeasureString(text);
			return new Rectangle(x , y, (int)a.X, (int)a.Y);
		}
	}
}
