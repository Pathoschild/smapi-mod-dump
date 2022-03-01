/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Common.UI {

	public static class SimpleHelper {

		public static SimpleBuilder Builder(LayoutDirection dir = LayoutDirection.Vertical, int margin = 0, Vector2? minSize = null, Alignment align = Alignment.None) {
			return new(null, new LayoutNode(dir, null, margin, minSize, align));
		}

		public static void DrawHover(this ISimpleNode node, SpriteBatch batch, SpriteFont defaultFont, Color? defaultColor = null, Color? defaultShadowColor = null, int offsetX = 0, int offsetY = 0, int overrideX = -1, int overrideY = -1, float alpha = 1f, bool drawBG = true, Texture2D bgTexture = null, Rectangle? bgSource = null, float bgScale = 1) {
			// Get the node's size.
			Vector2 size = node.GetSize(defaultFont, Vector2.Zero);

			// If we have no size, we have nothing to draw.
			if (size.X <= 0 || size.Y <= 0)
				return;

			// Add padding around the menu.
			int width = (int) size.X + 32;
			int height = (int) size.Y + 32;

			int x = overrideX < 0 ? Game1.getOldMouseX() + 32 + offsetX : overrideX;
			int y = overrideY < 0 ? Game1.getOldMouseY() + 32 + offsetY : overrideY;

			Rectangle safeArea = Utility.getSafeArea();

			// TODO: Refactor logic to allow flipping to left positioned
			// tooltips.

			if (x + width > safeArea.Right) {
				x = safeArea.Right - width;
				y += 16;
			}

			if (y + height > safeArea.Bottom) {
				y = safeArea.Bottom - height;
				x += 16;

				if (x + width > safeArea.Right)
					 x = safeArea.Right - width;
			}

			if (x < safeArea.Left)
				x = safeArea.Left;

			if (y < safeArea.Top) {
				y = safeArea.Top;
				x += 16 + 32;

				if (x + width > safeArea.Right) {
					x = safeArea.Right - width;
					y += 16 + 32;
				}
			}

			// Draw the background first.
			if (drawBG)
				IClickableMenu.drawTextureBox(
					batch,
					texture: bgTexture ?? Game1.menuTexture,
					sourceRect: bgSource ?? new Rectangle(0, 256, 60, 60),
					x: x,
					y: y,
					width: width,
					height: height,
					color: Color.White * alpha,
					scale: bgScale
				);

			x += 16;
			y += 16;

			node.Draw(batch, new Vector2(x, y), size, size, alpha, defaultFont, defaultColor, defaultShadowColor);
		}

	}
}
