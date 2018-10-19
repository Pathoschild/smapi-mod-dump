using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace Elevator
{
	abstract class BaseButton : ClickableComponent
	{
		private const float baseScale = 4f;

		protected Texture2D sourceTexture;
		protected Rectangle sourceRectangle;

		protected bool isDisabled = false;
		public bool IsDisabled { get => isDisabled; set => isDisabled = value; }

		protected Color baseColor = Color.White;

		public BaseButton(Rectangle bounds, Texture2D sourceTexture = null, Rectangle? sourceRectangle = null)
			: base(bounds, "", "")
		{
			this.sourceTexture = sourceTexture ?? Game1.mouseCursors;
			this.sourceRectangle = sourceRectangle ?? new Rectangle(432, 439, 9, 9);

			base.scale = baseScale;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			int x = base.bounds.X + base.bounds.Width / 2;
			int y = base.bounds.Y + base.bounds.Height / 2;
			int width = (int)(base.bounds.Width * base.scale / baseScale);
			int height = (int)(base.bounds.Height * base.scale / baseScale);
			IClickableMenu.drawTextureBox(spriteBatch, sourceTexture, sourceRectangle, x - width / 2, y - height / 2, width, height, isDisabled ? Color.SlateGray : baseColor, base.scale, true);
		}

		public void TryHover(int x, int y, float maxScaleIncrease = 0.1f)
		{
			if (isDisabled) maxScaleIncrease = 0f;

			if (bounds.Contains(x, y))
			{
				base.scale = Math.Min(base.scale + 0.04f, baseScale + maxScaleIncrease);
				Game1.SetFreeCursorDrag();
			}
			else
			{
				base.scale = Math.Max(base.scale - 0.04f, baseScale);
			}
		}

		public abstract void OnClicked();
	}
}
