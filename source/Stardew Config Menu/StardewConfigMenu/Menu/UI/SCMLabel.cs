using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.UI {
	public class SCMLabel {
		public SCMLabel(string text, SpriteFont font, int x = 0, int y = 0) {
			_Text = "";
			_Bounds = new Rectangle(x, y, 0, 0);
			Font = font;
			Text = text;
		}

		public int X { get => _Bounds.X; set => _Bounds.X = value; }
		public int Y { get => _Bounds.Y; set => _Bounds.Y = value; }
		public int Width { get => _Bounds.Width; }
		public int Height { get => _Bounds.Height; }

		public Rectangle Bounds => _Bounds;

		Rectangle _Bounds;

		public string Text {
			get => _Text;
			set {
				var size = Font.MeasureString(value);
				_Bounds.Width = (int) Math.Ceiling(size.X);
				_Bounds.Height = (int) Math.Ceiling(size.Y);
				_Text = value;
			}
		}
		string _Text;
		public SpriteFont Font;

		public void DrawAt(SpriteBatch b, int x, int y) {
			X = x;
			Y = y;
			Draw(b);
		}

		public void Draw(SpriteBatch b) {
			var origin = new Vector2(Bounds.X, Bounds.Y);
			b.DrawString(Font, Text, origin, Color.Black);
		}
	}
}