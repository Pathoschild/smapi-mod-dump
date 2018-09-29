using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SplitScreen.Menu
{
	class InputDeviceMenu : IClickableMenu
	{
		Keyboards.MultipleKeyboardManager keyboardManager;
		Mice.MultipleMiceManager miceManager;

		private SpriteFont textFont = Game1.smallFont;

		private readonly Rectangle textureSegment = new Rectangle(384, 373, 18, 18);

		private Vector2 playerIndexTextPosition;
		private Vector2 keyboardPointerTextPosition;
		private Vector2 mousePointerTextPosition;

		private Rectangle ahkNullWarningTextRect;

		AttachMouseButton attachMouseButton;
		DetachMouseButton detachMouseButton;
		AttachKeyboardButton attachKeyboardButton;
		DetachKeyboardButton detachKeyboardButton;

		AffinityButtonsMenu affinityDropdownMenu;

		ToggleBordersButton toggleBordersButton;

		public InputDeviceMenu(Keyboards.MultipleKeyboardManager keyboardManager, Mice.MultipleMiceManager miceManager)
		{
			this.keyboardManager = keyboardManager;
			this.miceManager = miceManager;

			base.width = 900;
			base.height = 370;

			CalculateElementPositions();
		}

		private void CalculateElementPositions()
		{
			Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height, 0, 0);
			base.xPositionOnScreen = (int)topLeft.X;
			base.yPositionOnScreen = (int)topLeft.Y + 32;

			playerIndexTextPosition = new Vector2(base.xPositionOnScreen + 32, base.yPositionOnScreen + 32);
			keyboardPointerTextPosition = new Vector2(base.xPositionOnScreen + 32, base.yPositionOnScreen + 32*2);
			mousePointerTextPosition = new Vector2(base.xPositionOnScreen + 32, base.yPositionOnScreen + 32*3);
			ahkNullWarningTextRect = new Rectangle(base.xPositionOnScreen, base.yPositionOnScreen + base.width + 35, base.width, 70);

			initializeUpperRightCloseButton();

			

			//Lower left corner
			attachMouseButton = new AttachMouseButton(base.xPositionOnScreen + 32, base.yPositionOnScreen + base.height - AttachMouseButton.Height - 25, miceManager);
			detachMouseButton = new DetachMouseButton(attachMouseButton.bounds.X + attachMouseButton.bounds.Width + 15, attachMouseButton.bounds.Y, miceManager);
			attachKeyboardButton = new AttachKeyboardButton(detachMouseButton.bounds.X + detachMouseButton.bounds.Width + 15, detachMouseButton.bounds.Y, keyboardManager);
			detachKeyboardButton = new DetachKeyboardButton(attachKeyboardButton.bounds.X + attachKeyboardButton.bounds.Width + 15, attachKeyboardButton.bounds.Y, keyboardManager);

			toggleBordersButton = new ToggleBordersButton(base.xPositionOnScreen + 32, base.yPositionOnScreen + base.height - 150);

			affinityDropdownMenu = new AffinityButtonsMenu(base.xPositionOnScreen + 32, base.yPositionOnScreen + base.height - 200);	
		}
		
		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			CalculateElementPositions();
		}

		#region Drawing
		public override void draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

			#region Primary box
			//Boundary box
			IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, textureSegment, 
				base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f, true);

			//Top texts
			spriteBatch.DrawString(textFont, $"Gamepad player index: {PlayerIndexController.GetIndexAsString()}", playerIndexTextPosition, Game1.textColor);
			spriteBatch.DrawString(textFont, $"Attached keyboard: { Keyboards.MultipleKeyboardManager.AttachedKeyboardID }", keyboardPointerTextPosition, Game1.textColor);
			spriteBatch.DrawString(textFont, $"Attached mouse: { Mice.MultipleMiceManager.AttachedMouseID }", mousePointerTextPosition, Game1.textColor);

			//Mouse/Keyboard buttons
			attachMouseButton.Draw(spriteBatch);//must be Draw not draw
			detachMouseButton.Draw(spriteBatch);
			attachKeyboardButton.Draw(spriteBatch);
			detachKeyboardButton.Draw(spriteBatch);

			toggleBordersButton.Draw(spriteBatch);

			affinityDropdownMenu.Draw(spriteBatch);
			#endregion

			#region AHK == null display box
			IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, textureSegment, base.xPositionOnScreen, base.yPositionOnScreen + base.height + 13, base.width, 80, Color.White, 3f, true);

			Game1.drawWithBorder(Mice.MouseDisabler.IsAutoHotKeyNull ? "Warning! This instance can't lock mouse" : "This instance can lock the mouse", 
				Color.White, Mice.MouseDisabler.IsAutoHotKeyNull ? Color.OrangeRed : Color.ForestGreen, 
				new Vector2(base.xPositionOnScreen + 25, base.yPositionOnScreen + base.height + 32));
			#endregion

			//Base draws the exit button
			base.draw(spriteBatch);
			Game1.mouseCursorTransparency = 1f;
			base.drawMouse(spriteBatch);
		}

		public override void performHoverAction(int x, int y)
		{
			attachMouseButton.TryHover(x, y);
			detachMouseButton.TryHover(x, y);
			attachKeyboardButton.TryHover(x, y);
			detachKeyboardButton.TryHover(x, y);
			toggleBordersButton.TryHover(x, y);
			base.performHoverAction(x, y);
		}
		#endregion

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (attachMouseButton.bounds.Contains(x, y) && !attachMouseButton.IsDisabled)
				attachMouseButton.OnClicked();
			else if (detachMouseButton.bounds.Contains(x, y) && !detachMouseButton.IsDisabled)
				detachMouseButton.OnClicked();
			else if (attachKeyboardButton.bounds.Contains(x, y) && !attachKeyboardButton.IsDisabled)
				attachKeyboardButton.OnClicked();
			else if (detachKeyboardButton.bounds.Contains(x, y) && !detachKeyboardButton.IsDisabled)
				detachKeyboardButton.OnClicked();
			else if (toggleBordersButton.bounds.Contains(x, y) && !toggleBordersButton.IsDisabled)
				toggleBordersButton.OnClicked();

			affinityDropdownMenu.LeftClickedInMenu(x, y);

			base.receiveLeftClick(x, y, playSound);
		}

		public override void update(GameTime time)
		{
			attachMouseButton.IsDisabled = PlayerIndexController._PlayerIndex != null || !Keyboards.MultipleKeyboardManager.HasAttachedKeyboard();
			detachMouseButton.IsDisabled = PlayerIndexController._PlayerIndex != null || !Mice.MultipleMiceManager.HasAttachedMouse();

			detachKeyboardButton.IsDisabled = PlayerIndexController._PlayerIndex != null || !Keyboards.MultipleKeyboardManager.HasAttachedKeyboard() || (Mice.MultipleMiceManager.HasAttachedMouse() && !Game1.game1.IsActive);

			
			attachKeyboardButton.Update();
			base.update(time);
		}
	}
}
