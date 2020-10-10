/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Diagnostics;


namespace SplitScreen.Menu
{
	class AffinityButtonsMenu
	{
		PlusOrMinusButton plusButton;
		PlusOrMinusButton minusButton;

		int x, y;

		private readonly int processorCount;
		private static int currentSelectedProcessor = 1 + (int)Math.Log((int)Process.GetCurrentProcess().ProcessorAffinity, 2);//NOT zero based
		private static int CurrentSelectedProcessor
		{
			get => currentSelectedProcessor;
			set
			{
				if (currentSelectedProcessor != value)
				{
					currentSelectedProcessor = value;
					AffinitySetter.SetDesignatedProcessor(value);
				}
			}
		}

		
		public AffinityButtonsMenu(int x, int y)
		{ 
			this.processorCount = Environment.ProcessorCount;

			this.x = x; this.y = y;

			plusButton = new PlusOrMinusButton(x + 60, y, true);
			minusButton = new PlusOrMinusButton(x, y, false);
			plusButton.Clicked += (o, e) => CurrentSelectedProcessor = Math.Min(CurrentSelectedProcessor + 1, processorCount);
			minusButton.Clicked += (o, e) => CurrentSelectedProcessor = Math.Max(CurrentSelectedProcessor - 1, 1);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			plusButton.Draw(spriteBatch, CurrentSelectedProcessor == processorCount);
			minusButton.Draw(spriteBatch, CurrentSelectedProcessor == 1);

			Utility.drawTextWithShadow(spriteBatch, CurrentSelectedProcessor.ToString(), Game1.smallFont, new Vector2(x + 35f, y), Game1.textColor, 1f, 1f, -1, -1, 0f, 3);
			Utility.drawTextWithShadow(spriteBatch, "Designated CPU core", Game1.smallFont, new Vector2(x + 100f, y+3), Game1.textColor, 1f, 1f, -1, -1, 0f, 3);
		}

		public void LeftClickedInMenu (int x, int y)
		{
			if (plusButton.Bounds.Contains(x, y))
				plusButton.OnClicked();
			else if (minusButton.Bounds.Contains(x, y))
				minusButton.OnClicked();
		}
	}

	class PlusOrMinusButton
	{
		public event EventHandler Clicked;

		int x, y;
		float scale = 4f;

		public Rectangle Bounds => new Rectangle(x,y, (int)(OptionsPlusMinus.plusButtonSource.Width*scale), (int)(OptionsPlusMinus.plusButtonSource.Height*scale));

		bool isPlusButton = true;

		public PlusOrMinusButton(int x, int y, bool plusButton)
		{
			this.x = x;
			this.y = y;
			this.isPlusButton = plusButton;
		}

		public void Draw(SpriteBatch spriteBatch, bool greyedOut)
		{
			spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)x, (float)y), this.isPlusButton ? OptionsPlusMinus.plusButtonSource : OptionsPlusMinus.minusButtonSource, Color.White * (greyedOut ? 0.33f : 1f), 0f, Vector2.Zero, scale, SpriteEffects.None, 0.4f);
		}

		public void OnClicked()
		{
			Clicked?.Invoke(this, null);
		}
	}
}
