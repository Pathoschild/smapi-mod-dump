/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace AutoTrasher.Components
{
	internal static class UIHelpers
	{
		public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

		public static void DrawTab(int x, int y, SpriteFont font, string text, int align = 0, float alpha = 1, bool drawShadow = true)
		{
			var spriteBatch = Game1.spriteBatch;
			var bounds = font.MeasureString(text);

			DrawTab(x, y, (int)bounds.X, (int)bounds.Y, out Vector2 drawPos, align, alpha, drawShadow);
			Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
		}

		public static void DrawTab(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition, int align = 0, float alpha = 1, bool drawShadow = true)
		{
			var spriteBatch = Game1.spriteBatch;

			// calculate outer coordinates
			var outerWidth = innerWidth + ButtonBorderWidth * 2;
			var outerHeight = innerHeight + Game1.tileSize / 3;
			var offsetX = align switch
			{
				1 => -outerWidth / 2,
				2 => -outerWidth,
				_ => 0
			};

			IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
			innerDrawPosition = new Vector2(x + ButtonBorderWidth + offsetX, y + ButtonBorderWidth);
		}
	}
}
