/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.UI {
	public class SCMTextureBox {
		public static SCMTextureBox SliderBackground => new SCMTextureBox(Game1.mouseCursors, OptionsSlider.sliderBGSource, Rectangle.Empty, Color.White, 1f, Game1.pixelZoom, false);

		public static SCMTextureBox DropdownBackground => new SCMTextureBox(Game1.mouseCursors, OptionsDropDown.dropDownBGSource, Rectangle.Empty, Color.White, 1f, Game1.pixelZoom, false);

		public SCMTextureBox(Texture2D texture, Rectangle sourceRect, Rectangle bounds = new Rectangle(), Color color = default(Color), float transparency = 1f, float scale = 1f, bool drawShadow = true) {
			_Bounds = bounds;
			Texture = texture;
			SourceRect = sourceRect;
			Color = color == default(Color) ? Color.White : color;
			Scale = scale;
			DrawShadow = drawShadow;
			Transparency = transparency;
		}

		public int X { get => _Bounds.X; set => _Bounds.X = value; }
		public int Y { get => _Bounds.Y; set => _Bounds.Y = value; }
		public int Width { get => _Bounds.Width; set => _Bounds.Width = value; }
		public int Height { get => _Bounds.Height; set => _Bounds.Height = value; }

		public Rectangle Bounds => _Bounds;

		Rectangle _Bounds;

		public readonly Texture2D Texture;
		public readonly Rectangle SourceRect;
		public Color Color;
		public float Scale;
		public bool DrawShadow;
		public float Transparency;

		public void DrawAt(SpriteBatch b, int x, int y) {
			X = x;
			Y = y;
			Draw(b);
		}

		public void Draw(SpriteBatch b) {
			IClickableMenu.drawTextureBox(b, Texture, SourceRect, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Color * Transparency, Scale, DrawShadow);
		}
	}
}