using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ServerBrowser
{
	class Checkbox
	{
		const int pixelsWide = 9;
		const int scale = 4;
		public bool IsChecked { get; set; }

		static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);
		static Rectangle sourceRectChecked = new Rectangle(236, 425, 9, 9);

		public int x, y;
		public Rectangle Bounds => new Rectangle(x, y, pixelsWide * scale, pixelsWide * scale);
		public Rectangle BoundsPlusText(SpriteFont font = null) => new Rectangle(x, y, pixelsWide * scale + 8 + (int)(font ?? Game1.dialogueFont).MeasureString(label).X, pixelsWide * scale);

		readonly string label;

		public Checkbox(int x, int y, string label, bool defaultState = false)
		{
			this.x = x;
			this.y = y;
			this.label = label;
			this.IsChecked = defaultState;
		}

		public void Clicked(int x, int y)
		{
			Game1.playSound("drumkit6");
			IsChecked = !IsChecked;
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font = null)
		{
			bool greyedOut = false;
			spriteBatch.Draw(Game1.mouseCursors, new Vector2(x,  y), IsChecked ? sourceRectChecked : sourceRectUnchecked, Color.White * (greyedOut ? 0.33f : 1f), 0f, Vector2.Zero, (float)scale, SpriteEffects.None, 0.4f);
			Utility.drawTextWithShadow(spriteBatch, label, font ?? Game1.dialogueFont, new Vector2((float)(x + pixelsWide*scale + 8), (float)(y)), greyedOut ? (Game1.textColor * 0.33f) : Game1.textColor, 1f, 0.1f, -1, -1, 1f, 3);
		}
	}
}
