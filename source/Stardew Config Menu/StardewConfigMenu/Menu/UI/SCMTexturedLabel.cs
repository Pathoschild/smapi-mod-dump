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

namespace StardewConfigMenu.UI {
	public class SCMTexturedLabel {
		public SCMTexturedLabel(string text, SpriteFont font, int x = 0, int y = 0, int minWidth = 0) {
			MinWidth = minWidth;
			label = new SCMLabel("", font);
			background = new SCMTextureBox(Game1.menuTexture, new Rectangle(0, 256, 60, 60));

			Text = text;
			X = x;
			Y = y;
		}

		int MinWidth;
		SCMLabel label;
		SCMTextureBox background;

		public string Text {
			get => label.Text;
			set {
				label.Text = value;
				var width = Math.Max(label.Width + Game1.pixelZoom * 12, MinWidth);
				Width = width;
				background.Height = label.Height + Game1.pixelZoom * 6;
			}
		}

		public float Transparency { get => background.Transparency; set => background.Transparency = value; }

		public int X {
			get => background.X;
			set {
				background.X = value;
				label.X = value + Game1.pixelZoom * 6;
			}
		}
		public int Y {
			get => background.Y;
			set {
				background.Y = value;
				label.Y = value + Game1.pixelZoom * 4;
			}
		}
		public int Width {
			get => background.Width;
			set {
				if (value < MinWidth || value < (label.Width + Game1.pixelZoom * 12))
					return;

				background.Width = value;
			}
		}
		public int Height { get => background.Height; }

		public Rectangle Bounds => background.Bounds;

		public void DrawAt(SpriteBatch b, int x, int y) {
			X = x;
			Y = y;
			Draw(b);
		}

		public void Draw(SpriteBatch b) {
			background.Draw(b);
			label.Draw(b);
		}
	}
}
