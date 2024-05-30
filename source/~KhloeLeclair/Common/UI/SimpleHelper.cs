/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_SIMPLELAYOUT

using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.UI;

public static class SimpleHelper {

	public static ISimpleBuilder Builder(LayoutDirection dir = LayoutDirection.Vertical, int margin = 0, Vector2? minSize = null, Alignment align = Alignment.None) {
		return new SimpleBuilder(null, new LayoutNode(dir, null, margin, minSize, align));
	}

	public static void DrawHover(
		this ISimpleNode node,
		SpriteBatch batch,
		SpriteFont defaultFont,
		Color? defaultColor = null,
		Color? defaultShadowColor = null,
		int offsetX = 0,
		int offsetY = 0,
		int overrideX = -1,
		int overrideY = -1,
		float alpha = 1f,
		bool drawBG = true,
		Texture2D? bgTexture = null,
		Rectangle? bgSource = null,
		float bgScale = 1,
		Texture2D? divTexture = null,
		Rectangle? divHSource = null,
		Rectangle? divVSource = null,
		Vector2? minSize = null
	) {
		// Get the node's size.
		Vector2 size = node.GetSize(defaultFont, minSize ?? Vector2.Zero);

		Vector2 contSize = size;

		if (minSize.HasValue) {
			if (contSize.X < minSize.Value.X)
				contSize.X = minSize.Value.X;
			if (contSize.Y < minSize.Value.Y)
				contSize.Y = minSize.Value.Y;
		}

		// If we have no size, we have nothing to draw.
		if (contSize.X <= 0 || contSize.Y <= 0)
			return;

		// Add padding around the menu.
		int width = (int) contSize.X + 32;
		int height = (int) contSize.Y + 32;

		int mx = Game1.getOldMouseX();
		int my = Game1.getOldMouseY();

		int x = overrideX == -1 ? mx + 32 + offsetX : overrideX;
		int y = overrideY == -1 ? my + 32 + offsetY : overrideY;

		Rectangle safeArea = Utility.getSafeArea();

		// Make sure we're in the safe area.
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

		// Flip to the other side?
		// Don't flip if we have override coordinates.
		if (overrideX < 0 && overrideY < 0 && x < mx + 32 + offsetX && y < my + 32 + offsetY) {
			int tx = mx - width - 16;
			bool moved = false;
			if (tx < safeArea.Left) {
				if (safeArea.Left + width < mx) {
					x = safeArea.Left;
					moved = true;
				}
			} else {
				x = tx;
				moved = true;
			}

			if (!moved && y < my + 32 + offsetY) {
				int ty = my - height - 16;
				if (ty < safeArea.Top) {
					if (safeArea.Top + height < my)
						y = safeArea.Top;
				} else
					y = ty;
			}
		}


		// Draw the background first.
		if (drawBG)
			RenderHelper.DrawBox(
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

		DividerNode.DefaultTexture = divTexture;
		DividerNode.HorizontalSource = divHSource;
		DividerNode.VerticalSource = divVSource;
		DividerNode.Scale = divHSource.HasValue ? 4f : 1f;

		node.Draw(batch, new Vector2(x, y), size, contSize, alpha, defaultFont, defaultColor, defaultShadowColor);
	}

}

#endif
