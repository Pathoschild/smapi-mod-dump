/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using System;

using StardewValley;
using StardewValley.Menus;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace LoveOfCooking.GameObjects
{
	/// <summary>
	/// This is a big stupid class copied from blueberry.SecretBase.
	/// 
	/// It's a pulsing icon of a blueberry patched into an empty space in Content/LooseSprites/Cursors from WorldEditor
	/// that shows up when there are pending notifications.
	/// 
	/// It copies the dimensions and position of the DayTimeMoneyBox class that
	/// makes up most of the Stardew Valley overworld HUD to have a quick-fix easy-fit
	/// solution to squeezing the clickable Button in beside the Journal button that it mimics.
	/// </summary>
	internal class NotificationButton : IClickableMenu
	{
		private Vector2 _position;
		internal ClickableTextureComponent Button;

		private string _hoverText = "";
		private int _pulseTimer;
		private int _whenToPulseTimer;

		public NotificationButton()
			: base(Game1.viewport.Width - 300 + 32, 8, 300, 284)
		{
			var sourceRect = new Rectangle(AssetManager.IconPosition.X, AssetManager.IconPosition.Y, 11, 14);
			_position = new Vector2(xPositionOnScreen, yPositionOnScreen);
			Button = new ClickableTextureComponent(
				new Rectangle(xPositionOnScreen + 220, yPositionOnScreen + 240, 44, 46),
				Game1.mouseCursors,
				sourceRect,
				4f)
			{
				visible = true,
				myID = 53050001
			};
		}

		public override bool isWithinBounds(int x, int y)
		{
			return Button.containsPoint(x, y) && ModEntry.PendingNotifications.Count > 0;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			UpdatePosition();
			if (isWithinBounds(x, y))
				Game1.activeClickableMenu = new NotificationMenu();
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			UpdatePosition();
		}

		public override void performHoverAction(int x, int y)
		{
			UpdatePosition();
			_hoverText = "";
			if (isWithinBounds(x, y))
				_hoverText = ModEntry.Instance.i18n.Get("notification.icon.inspect");
		}

		private void UpdatePosition()
		{
			_position = new Vector2(Game1.viewport.Width - 300, 8f);
			if (Game1.isOutdoorMapSmallerThanViewport())
			{
				_position = new Vector2(Math.Min(_position.X, -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 300), 8f);
			}
			Utility.makeSafe(ref _position, 300, 284);
			xPositionOnScreen = (int)_position.X;
			yPositionOnScreen = (int)_position.Y;
			Button.bounds = new Rectangle(xPositionOnScreen + 160, yPositionOnScreen + 240, 44, 46);
		}

		public override void draw(SpriteBatch b)
		{
			UpdatePosition();

			if (!_hoverText.Equals("") && isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
				drawHoverText(b, _hoverText, Game1.dialogueFont);

			if (_pulseTimer > 0)
				_pulseTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;

			if (_whenToPulseTimer >= 0)
			{
				_whenToPulseTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
				if (_whenToPulseTimer <= 0)
				{
					_whenToPulseTimer = 3000;
					_pulseTimer = 1000;
				}
			}

			if (ModEntry.PendingNotifications.Count <= 0)
				return;

			b.Draw(Game1.mouseCursors,
				Button.getVector2(),
				Button.sourceRect,
				Color.White,
				0f,
				Vector2.Zero,
				Button.scale,
				SpriteEffects.None,
				0.99f - 1f / 10000f);

			if (_pulseTimer <= 0 || !ModEntry.HasUnreadNotifications)
				return;

			var scaleMult = 1f / (Math.Max(300f, Math.Abs(_pulseTimer % 1000 - 500)) / 500f);
			b.Draw(Game1.mouseCursors,
				new Vector2(Button.bounds.X + 24, Button.bounds.Y + 32)
				+ (scaleMult > 1f
					? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2))
					: Vector2.Zero),
				new Rectangle(
					Button.sourceRect.X - Button.sourceRect.Width,
					Button.sourceRect.Y,
					Button.sourceRect.Width,
					Button.sourceRect.Height),
				Color.White,
				0f,
				new Vector2(Button.sourceRect.Width / 2f, Button.sourceRect.Height / 2f),
				4f * scaleMult,
				SpriteEffects.None,
				0.99f);
		}
	}
}
