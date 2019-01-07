using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace ServerBrowser
{
	class TextMenu : NamingMenu
	{
		private readonly bool passwordInput = false;
		private readonly Action callbackOnEscape;

		public TextMenu(string title, bool passwordInput, doneNamingBehavior b, Action callbackOnEscape = null)
			: base(b, title, "")
		{
			base.textBox.limitWidth = false;
			base.textBox.Width = 512;
			base.textBox.X -= 128;
			base.randomButton.visible = false;
			base.doneNamingButton.bounds.X += 128;
			base.minLength = 0;
			this.passwordInput = passwordInput;
			this.callbackOnEscape = callbackOnEscape;
		}

		public override void update(GameTime time)
		{
			GamePadState pad = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyboard = Game1.GetKeyboardState();
			if (Game1.IsPressEvent(ref pad, Buttons.B) || Game1.IsPressEvent(ref keyboard, Keys.Escape))
			{
				if (Game1.activeClickableMenu is TitleMenu)
				{
					(Game1.activeClickableMenu as TitleMenu).backButtonPressed();
				}
				else
				{
					if (callbackOnEscape == null)
						Game1.exitActiveMenu();
					else
						callbackOnEscape();
				}
			}
			base.update(time);
		}

		public override void draw(SpriteBatch b)
		{
			drawBackground(b);

			string title = ModEntry.ModHelper.Reflection.GetField<string>(this, "title", false)?.GetValue() ?? "<unknown>";

			//base.base.draw(b);
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
			SpriteText.drawStringWithScrollCenteredAt(b, title, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, title, 1f, -1, 0, 0.88f, false);
			
			if (passwordInput)
			{
				var actualText = textBox.Text;
				textBox.Text = new string('#', actualText?.Length ?? 0);
				textBox.Draw(b, true);
				textBox.Text = actualText;
			}
			else
				textBox.Draw(b, true);
			
			doneNamingButton.draw(b);
			randomButton.draw(b);
			base.drawMouse(b);
		}
	}
}
