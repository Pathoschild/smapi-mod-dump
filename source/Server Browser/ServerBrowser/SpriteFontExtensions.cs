/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

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
