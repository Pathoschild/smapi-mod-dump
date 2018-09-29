using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpToFriends
{
	class UtilityPlus : Utility
	{

		public static void drawButtonWithText(SpriteBatch b, Rectangle bounds, Color boxColor, string text, SpriteFont font, Color color)
		{
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9),
				bounds.X, bounds.Y, bounds.Width, bounds.Height, boxColor, Game1.pixelZoom, true);

			float textWidth = font.MeasureString(text).X;
			float textHeight = font.MeasureString(text).Y;

			Vector2 center = new Vector2(bounds.Width / 2, bounds.Height / 2);
			Vector2 textStart = new Vector2(bounds.X + center.X - textWidth / 2, bounds.Y + center.Y - textHeight / 2);

			Utility.drawBoldText(b, text, font, textStart, color);

		}

		public static void drawTextBox(SpriteBatch b, Rectangle bounds, Color boxColor, string text, SpriteFont font, Color color)
		{
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9),
				bounds.X, bounds.Y, bounds.Width, bounds.Height, boxColor, Game1.pixelZoom, true);
		}


	}
}
