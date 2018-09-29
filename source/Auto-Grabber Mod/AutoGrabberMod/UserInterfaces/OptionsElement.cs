using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace AutoGrabberMod.UserInterfaces
{
    public class OptionsElement
	{
		public const int defaultX = 8;

		public const int defaultY = 4;

		public const int defaultPixelWidth = 9;

		public Rectangle bounds;

		public string label;        

		public bool greyedOut;

		public OptionsElement(string label)
		{
			this.label = label;
			this.bounds = new Rectangle(32, 16, 36, 36);
		}

		public OptionsElement(string label, int x, int y, int width, int height)
		{
			if (x == -1)
			{
				x = 32;
			}
			if (y == -1)
			{
				y = 16;
			}
			this.bounds = new Rectangle(x, y, width, height);
			this.label = label;
		}

		public OptionsElement(string label, Rectangle bounds)
		{			
			this.label = label;
			this.bounds = bounds;
		}

		public virtual void receiveLeftClick(int x, int y)
		{
		}

		public virtual void leftClickHeld(int x, int y)
		{
		}

		public virtual void leftClickReleased(int x, int y)
		{
		}

		public virtual void receiveKeyPress(Keys key)
		{
		}

		public virtual void draw(SpriteBatch b, int slotX, int slotY)
		{
			Utility.drawTextWithShadow(
				b,
				this.label,
				Game1.dialogueFont,
				new Vector2((float)(slotX + this.bounds.X + this.bounds.Width + 10), (float)(slotY + this.bounds.Y)),
				this.greyedOut ? (Game1.textColor * 0.33f) : Game1.textColor, 
                1f, 
                0.1f, 
                -1, 
                -1, 
                1f, 
				3
			);
		}
	}
}
