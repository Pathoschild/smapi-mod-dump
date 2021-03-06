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
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace LoveOfCooking.GameObjects
{
	public class NotificationMenu : IClickableMenu
	{
		private const int RootId = 6830 * 10000;

		// Notifications from modmail
		private List<ModEntry.Notification> _notifications;
		private List<List<ModEntry.Notification>> _pages;
		private List<ClickableComponent> _notificationButtons;
		private ModEntry.Notification? _currentNotification;

		// Main menu buttons
		private Point _closeButtonDefaultPosition;
		private ClickableTextureComponent _nextButton;
		private ClickableTextureComponent _prevButton;
		private ClickableTextureComponent _letter;
		private ClickableTextureComponent _acceptButton;
		private ClickableTextureComponent _declineButton;
		private readonly Rectangle _acceptIconSourceRect = new Rectangle(128, 256, 64, 64);
		private readonly Rectangle _declineIconSourceRect = new Rectangle(192, 256, 64, 64);

		// Menu control
		private const int NotificationsPerPage = 3;
		private int _currentPage;
		private readonly bool _showMailbox = true;
		private bool _showNotification;
		private bool _showReplyPrompt;

		// Menu dimensions
		private const int MenuWidth = 680;
		private const int MenuHeight = 480;
		private Point _centre;

		// Letter dimensions
		private readonly Texture2D _letterTexture;
		private readonly Rectangle _letterIconSourceRect = new Rectangle(188, 422, 16, 13);
		private const float LetterMaxScale = 1f;
		private float _letterScale;
		private int _mailboxButtonYPos;

		// Strings
		private ITranslationHelper i18n => ModEntry.Instance.i18n;
		private string _hoverText = "";

		public NotificationMenu()
			: base(0, 0, 0, 0, true)
		{
			_letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");

			Game1.playSound("bigSelect");
			ModEntry.HasUnreadNotifications = false;

			// Menu dimensions
			var scale = 4f;
			width = MenuWidth;
			height = MenuHeight;
			_centre = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Center;
			var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
			xPositionOnScreen = (int)topLeft.X;
			yPositionOnScreen = (int)topLeft.Y;

			// Populate all notifications the player currently has
			_notificationButtons = new List<ClickableComponent>();
			for (var i = 0; i < NotificationsPerPage; i++)
			{
				_notificationButtons.Add(new ClickableComponent(
					Rectangle.Empty,
					string.Concat(i))
				{
					myID = RootId + i,
					downNeighborID = -7777,
					upNeighborID = i > 0
						? i - 1
						: -1,
					rightNeighborID = -7777,
					leftNeighborID = -7777,
					fullyImmutable = true
				});
			}

			PaginateNotifications();

			// Other button alignments
			_mailboxButtonYPos = yPositionOnScreen + height / 2 - 48 / 2;

			// Buttons in the menu and letter views
			_letter = new ClickableTextureComponent(
				new Rectangle(_centre.X - MenuHeight / 2, _centre.Y - MenuWidth / 2, MenuHeight, MenuWidth),
				_letterTexture,
				new Rectangle(0, 0, 320, 180),
				0f,
				true)
			{
				myID = RootId + 2001
			};
			_acceptButton = new ClickableTextureComponent(
				Rectangle.Empty,
				Game1.mouseCursors,
				Rectangle.Empty,
				1f)
			{
				myID = RootId + 2002
			};
			_declineButton = new ClickableTextureComponent(
				Rectangle.Empty,
				Game1.mouseCursors,
				Rectangle.Empty,
				1f)
			{
				myID = RootId + 2003
			};

			_closeButtonDefaultPosition = new Point(xPositionOnScreen + width - 20, yPositionOnScreen - 8);
			upperRightCloseButton = new ClickableTextureComponent(
				new Rectangle(_closeButtonDefaultPosition.X, _closeButtonDefaultPosition.Y, 48, 48),
				Game1.mouseCursors,
				new Rectangle(337, 494, 12, 12),
				scale);

			_prevButton = new ClickableTextureComponent(
				Rectangle.Empty,
				Game1.mouseCursors,
				new Rectangle(352, 495, 12, 11),
				scale)
			{
				myID = RootId + 1002,
				rightNeighborID = -7777
			};
			_nextButton = new ClickableTextureComponent(
				Rectangle.Empty,
				Game1.mouseCursors,
				new Rectangle(365, 495, 12, 11),
				scale)
			{
				myID = RootId + 1001
			};

			ChangeButtonFormatting();

			if (Game1.options.SnappyMenus)
			{
				populateClickableComponentList();
				snapToDefaultClickableComponent();
			}
		}

		private void PaginateNotifications()
		{
			_notifications = ModEntry.PendingNotifications;
			_pages = new List<List<ModEntry.Notification>>();

			for (var i = 0; i < _notifications.Count; ++i)
			{
				if (_pages.Count <= i / NotificationsPerPage)
					_pages.Add(new List<ModEntry.Notification>());
				_pages[i / NotificationsPerPage].Add(_notifications[i]);
			}

			if (_pages.Count == 0)
				_pages.Add(new List<ModEntry.Notification>());

			_currentPage = Math.Min(Math.Max(_currentPage, 0), _pages.Count - 1);
			Log.D($"Pagination - notifications: {_notifications.Count}, pages: {_pages.Count}, currentPage: {_currentPage}",
				ModEntry.Instance.Config.DebugMode);
		}

		private void ChangeButtonFormatting()
		{
			if (_showNotification)
			{
				upperRightCloseButton.bounds.X = _letter.bounds.Right - 8;
			}
			else
			{
				upperRightCloseButton.bounds.X = _closeButtonDefaultPosition.X;
			}

			// Accept/decline notification buttons
			_declineButton.sourceRect = _declineIconSourceRect;
			_acceptButton.sourceRect = _acceptIconSourceRect;

			if (_currentNotification == ModEntry.Notification.BundleMultiplayerWarning)
			{
				_declineButton.visible = true;

				_declineButton.bounds = new Rectangle(
					_letter.bounds.Center.X + 8,
					_letter.bounds.Y + _letter.bounds.Height - 128,
					64, 64);
				_acceptButton.bounds = new Rectangle(
					_letter.bounds.Center.X - 64 - 8,
					_declineButton.bounds.Y,
					64, 64);
			}
			else
			{
				_declineButton.visible = false;

				_acceptButton.bounds = new Rectangle(
					_letter.bounds.Center.X - 32,
					_letter.bounds.Y + _letter.bounds.Height - 128,
					64, 64);
			}

			// Move prev/next buttons back into mailbox positions
			_prevButton.bounds.X = xPositionOnScreen - 64;
			_nextButton.bounds.X = xPositionOnScreen + width + 64 - 48;
			_prevButton.bounds.Y = _nextButton.bounds.Y = _mailboxButtonYPos;
			_prevButton.bounds.Width = _nextButton.bounds.Width = 48;
			_prevButton.bounds.Height = _nextButton.bounds.Height = 44;

			// Notification buttons, used for mail elements and options in the reply prompt
			for (var i = 0; i < _notificationButtons.Count; ++i)
			{
				var numBtns = NotificationsPerPage;
				if (!_showReplyPrompt)
				{
					// Arrange vertically for the mailbox elements
					_notificationButtons[i].bounds = new Rectangle(
						xPositionOnScreen + 16,
						yPositionOnScreen + 16 + i * ((height - 32) / numBtns),
						width - 32,
						(height - 32) / numBtns + 4);
				}
				else
				{
					// Arrange horizontally for reply options
					numBtns = 2;
					_notificationButtons[i].bounds = new Rectangle(
						xPositionOnScreen + 16 + i * ((width - 32) / numBtns),
						yPositionOnScreen + 16 + height / 3 + 68,
						(width - 32) / numBtns,
						(height - 32) / 3 + 8 - 32);
				}
			}
		}

		private void PressNextPageButton()
		{
			++_currentPage;
			Game1.playSound("shwip");

			if (!Game1.options.SnappyMenus || _currentPage != _pages.Count - 1)
				return;

			currentlySnappedComponent = getComponentWithID(0);
			snapCursorToCurrentSnappedComponent();
		}

		private void PressPreviousPageButton()
		{
			--_currentPage;
			Game1.playSound("shwip");

			if (!Game1.options.SnappyMenus || _currentPage != 0)
				return;

			currentlySnappedComponent = getComponentWithID(0);
			snapCursorToCurrentSnappedComponent();
		}

		private void OpenNotification(bool playSound, ModEntry.Notification notification)
		{
			if (playSound)
				Game1.playSound("shwip");

			_currentNotification = notification;
			_showNotification = true;

			ChangeButtonFormatting();
		}

		private void CloseNotification(bool playSound)
		{
			if (playSound)
				Game1.playSound("shwip");

			_currentNotification = null;
			_showReplyPrompt = false;
			_showNotification = false;
			_letterScale = 0f;

			if (ModEntry.PendingNotifications.Count == 0)
			{
				ModEntry.RemoveNotificationButton();
			}

			if (!_showMailbox)
			{
				exitThisMenuNoSound();
			}
		}

		private void TrashNotification(bool playSound)
		{
			if (_currentNotification == null)
				return;

			Log.D($"Trashed notification {_currentNotification}",
				ModEntry.Instance.Config.DebugMode);

			if (playSound)
				Game1.playSound("throwDownITem"); // [sic]
			_notifications.Remove(_currentNotification.Value);
			PaginateNotifications();
			CloseNotification(playSound: false);
		}

		public override void snapToDefaultClickableComponent()
		{
			currentlySnappedComponent = getComponentWithID(0);
			snapCursorToCurrentSnappedComponent();
		}

		public override void performHoverAction(int x, int y)
		{
			_hoverText = "";

			if (_showNotification)
			{
				_acceptButton.tryHover(x, y, 0.1f);
				_declineButton.tryHover(x, y, 0.1f);
			}

			base.performHoverAction(x, y);

			if (!_showNotification || _showReplyPrompt)
			{
				for (var i = 0; i < _notificationButtons.Count; i++)
				{
					if (_pages.Count > 0
						&& _pages[0].Count > i
						&& _notificationButtons[i].containsPoint(x, y)
						&& !_notificationButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
					{
						Game1.playSound("Cowboy_gunshot");
					}
				}
			}

			_nextButton.tryHover(x, y, 0.2f);
			_prevButton.tryHover(x, y, 0.2f);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);

			if (Game1.activeClickableMenu == null)
				return;

			if (_showNotification && Math.Abs(LetterMaxScale - _letterScale) < 0.001f)
			{
				if (_acceptButton.containsPoint(x, y))
				{
					switch (_currentNotification)
					{
						case ModEntry.Notification.BundleMultiplayerWarning:
							Log.I("Enabled custom bundle data in a possible multiplayer game."
								+ "\nReload the save if you need bundles disabled again for playing with friends.");
							ModEntry.Instance.SetCommunityCentreKitchenForThisSession(true);
							ModEntry.Instance.LoadBundleData();
							break;
					}
					TrashNotification(playSound: false);
				}
				else if (_declineButton.containsPoint(x, y))
				{
					switch (_currentNotification)
					{
						case ModEntry.Notification.BundleMultiplayerWarning:
							Log.I("Declined to load bundle data in a possible multiplayer game."
								+ "\nReload the save if you need bundles enabled for playing singleplayer.");
							break;
					}
					TrashNotification(playSound: false);
				}
			}
			else
			{
				if (_currentPage < _pages.Count - 1 && _nextButton.containsPoint(x, y))
				{
					PressNextPageButton();
				}
				else if (_currentPage > 0 && _prevButton.containsPoint(x, y))
				{
					PressPreviousPageButton();
				}
				else
				{
					for (var i = 0; i < _notificationButtons.Count; i++)
					{
						var actualIndex = i * _currentPage + i;

						if (_pages.Count <= 0 || _pages[_currentPage].Count <= i || !_notificationButtons[i].containsPoint(x, y))
							continue;

						Log.D($"Opened notification ({_notifications[actualIndex]}) at i: {i} actual i: {actualIndex}",
							ModEntry.Instance.Config.DebugMode);
						OpenNotification(playSound: true, notification: _notifications[actualIndex]);
						return;
					}
				}
			}

			ChangeButtonFormatting();
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (_showNotification)
				CloseNotification(playSound: true);
		}

		public override void receiveGamePadButton(Buttons b)
		{
			if (b == Buttons.RightTrigger && _currentPage < _pages.Count - 1)
				PressNextPageButton();
			else if (b == Buttons.LeftTrigger && _currentPage > 0)
				PressPreviousPageButton();

			ChangeButtonFormatting();
		}

		public override void receiveKeyPress(Keys key)
		{
			base.receiveKeyPress(key);

			if ((readyToClose()
				 || Game1.options.doesInputListContain(Game1.options.menuButton, key))
				&& Game1.options.doesInputListContain(Game1.options.journalButton, key))
			{
				Game1.exitActiveMenu();
				Game1.playSound("bigDeSelect");
				if (ModEntry.PendingNotifications.Count == 0)
				{
					ModEntry.RemoveNotificationButton();
				}
			}

			ChangeButtonFormatting();
		}

		public override void update(GameTime time)
		{
			// Open up the notification
			if (!_showNotification)
				return;

			if (!(_letterScale < LetterMaxScale))
				return;

			_letterScale += time.ElapsedGameTime.Milliseconds * 0.003f * LetterMaxScale;
			if (_letterScale >= LetterMaxScale)
				_letterScale = LetterMaxScale;
		}

		public override void draw(SpriteBatch b)
		{
			var scale = 4f;

			// Screen blackout
			b.Draw(
				Game1.fadeToBlackRect,
				Game1.graphics.GraphicsDevice.Viewport.Bounds,
				Color.Black * 0.75f);

			// Mailbox menu
			if (_showMailbox && !_showNotification)
			{
				// Journal heading with paper scroll
				SpriteText.drawStringWithScrollCenteredAt(b,
					i18n.Get("notification.icon.inspect"),
					xPositionOnScreen + width / 2,
					yPositionOnScreen - 64);

				// Menu container
				drawTextureBox(b,
					Game1.mouseCursors,
					new Rectangle(384, 373, 18, 18),
					xPositionOnScreen,
					yPositionOnScreen,
					width,
					height,
					Color.White,
					scale * (1f - _letterScale));

				// Notification elements
				for (var i = 0; i < _notificationButtons.Count; i++)
				{
					if (_pages.Count <= 0 || _pages[_currentPage].Count <= i)
						continue;

					// Button container
					drawTextureBox(b,
						Game1.mouseCursors,
						new Rectangle(384, 396, 15, 15),
						_notificationButtons[i].bounds.X,
						_notificationButtons[i].bounds.Y,
						_notificationButtons[i].bounds.Width,
						_notificationButtons[i].bounds.Height,
						_notificationButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY())
							? Color.Wheat
							: Color.White,
						scale,
						false);

					// Icon
					Utility.drawWithShadow(b,
						Game1.mouseCursors,
						new Vector2(
							_notificationButtons[i].bounds.X + 16,
							_notificationButtons[i].bounds.Y + _notificationButtons[i].bounds.Height / 2 - 13 * 2 - 4),
						_letterIconSourceRect,
						Color.White,
						0f,
						Vector2.Zero,
						scale,
						false,
						0.99f);

					// Summary
					var text = i18n.Get($"notification.{_notifications[i].ToString().ToLower()}.inspect");
					var width = _notificationButtons[i].bounds.Width - 96 - 4;
					SpriteText.drawString(b,
						text,
						_notificationButtons[i].bounds.X + 96 + 4,
						_notificationButtons[i].bounds.Y + (_notificationButtons[i].bounds.Height - SpriteText.getHeightOfString(text, width)) / 2,
						999999,
						width,
						_notificationButtons[i].bounds.Height - 84 * 2 - 4 * 2 - 24 * 2);
				}

				if (_notifications.Count == 0)
				{
					// No mail
					SpriteText.drawStringHorizontallyCenteredAt(b,
						i18n.Get("notification.empty.inspect"),
						xPositionOnScreen + width / 2,
						yPositionOnScreen - 24 + height / 2 - SpriteText.characterHeight,
						999999,
						MenuWidth - 64,
						_notificationButtons[0].bounds.Height);

				}
			}

			// Letter background and notification contents
			if (_showNotification)
			{
				b.Draw(
					_letterTexture,
					new Rectangle(
						(int)(_centre.X + MenuHeight / 2f * _letterScale),
						(int)((_centre.Y * 2f - MenuWidth) / 2f * _letterScale),
						(int)(MenuWidth * _letterScale),
						(int)(MenuHeight * _letterScale)),
					_letter.sourceRect,
					Color.White,
					1.5708f,
					Vector2.Zero,
					SpriteEffects.None,
					0.88f);

				// Letter popout is ready
				if (Math.Abs(LetterMaxScale - _letterScale) < 0.001f)
				{
					var text = i18n.Get($"notification.{_currentNotification.ToString().ToLower()}.message").ToString();
					var margin = new Point(32, 48);

					// Notification details
					Utility.drawTextWithShadow(b,
						Game1.parseText(text, Game1.smallFont, _letter.bounds.Width - margin.X * 2), Game1.smallFont,
						new Vector2(_letter.bounds.Left + margin.X, _letter.bounds.Top + margin.Y), Game1.textColor, 1f);

					// Accept/decline notification buttons
					_acceptButton.draw(b);
					if (_currentNotification == ModEntry.Notification.BundleMultiplayerWarning)
					{
						_declineButton.draw(b);
					}
				}
			}
			else
			{
				// Nav left/right buttons
				//if (_currentPage < _pages.Count - 1)
				_nextButton.draw(b);
				//if (_currentPage > 0)
				_prevButton.draw(b);
			}

			// Upper right close button
			base.draw(b);

			// Hover text
			if (_hoverText.Length > 0)
				drawHoverText(b, _hoverText, Game1.dialogueFont);

			// Cursor
			Game1.mouseCursorTransparency = 1f;
			drawMouse(b);
		}
	}
}
