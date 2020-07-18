using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Linq;

namespace StardewConfigMenu {
	public class Utilities {

		// Adds newlines into string
		// Useful for splitting long descriptions in hoverTextDictionary
		public static string GetWordWrappedString(string input, int charCount = 40) {

			// early out if les than 1 line length
			if (input.Length <= charCount) {
				return string.Copy(input);
			}

			string output = string.Empty;
			var count = 0;

			foreach (char ch in input) {
				if ((count > charCount && (ch == ' ' || ch == '\t')) || ch == '\n') {
					output += '\n';
					count = 0;
				} else if (ch == '\t') {
					output += ' '; // remove tabs, might mess stuff up
					count++;
				} else {
					output += ch;
					count++;
				}
			}

			return output;
		}


		// Ripped from assembly, and stripped of unnecesary logic. Used for recreating menu Hovered Text over original
		public static void drawHoverTextWithoutShadow(SpriteBatch b, string text, SpriteFont font, int xOffset = 0, int yOffset = 0) {
			if (text == null || text.Length == 0) {
				return;
			}
			var alpha = 1f;
			int num = 20;
			int num2 = Math.Max(0, Math.Max((int) font.MeasureString(text).X, 0)) + Game1.tileSize / 2;
			int num3 = Math.Max(num * 3, (int) font.MeasureString(text).Y + Game1.tileSize / 2);

			int num7 = Game1.getOldMouseX() + Game1.tileSize / 2 + xOffset;
			int num8 = Game1.getOldMouseY() + Game1.tileSize / 2 + yOffset;

			if (num7 + num2 > Utility.getSafeArea().Right) {
				num7 = Utility.getSafeArea().Right - num2;
				num8 += Game1.tileSize / 4;
			}
			if (num8 + num3 > Utility.getSafeArea().Bottom) {
				num7 += Game1.tileSize / 4;
				if (num7 + num2 > Utility.getSafeArea().Right) {
					num7 = Utility.getSafeArea().Right - num2;
				}
				num8 = Utility.getSafeArea().Bottom - num3;
			}
			IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), num7, num8, num2, num3, Color.White * alpha, 1f, false);

			if (!string.IsNullOrEmpty(text) && text != " ") {
				b.DrawString(font, text, new Vector2(num7 + Game1.tileSize / 4, num8 + Game1.tileSize / 4 + 4) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2(num7 + Game1.tileSize / 4, num8 + Game1.tileSize / 4 + 4) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2(num7 + Game1.tileSize / 4, num8 + Game1.tileSize / 4 + 4) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2(num7 + Game1.tileSize / 4, num8 + Game1.tileSize / 4 + 4), Game1.textColor * 0.9f * alpha);
			}
		}
	}
}
