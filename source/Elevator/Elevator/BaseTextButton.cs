using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Elevator
{
	abstract class BaseTextButton : BaseButton
	{
		protected string text = "";

		public static int Height => 55;

		public BaseTextButton(int x, int y, string text, Texture2D sourceTexture = null, Rectangle? sourceRectangle = null)
			: base(new Rectangle(), null, null)
		{
			this.text = text;

			base.bounds = new Rectangle(x, y, (int)Game1.smallFont.MeasureString(text).X + 20, Height);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			Utility.drawTextWithShadow(spriteBatch, this.text, Game1.smallFont, new Vector2(base.bounds.X + 10f, base.bounds.Y + 10f), Game1.textColor, 1f, 1f, -1, -1, 0f, 3);
		}
	}
}
