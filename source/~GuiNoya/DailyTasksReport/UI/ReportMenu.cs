using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace DailyTasksReport.UI
{
    public sealed class ReportMenu : IClickableMenu
    {
        private readonly Texture2D _letterTexture = Game1.temporaryContent.Load<Texture2D>(@"LooseSprites\letterBG");
        private ClickableTextureComponent _backButton;
        private ClickableTextureComponent _forwardButton;
        private ClickableTextureComponent _settingsButton;

        private readonly ModEntry _parent;
        private readonly List<string> _mailMessage;
        private bool _firstKeyEvent;
        private int _page;
        private float _scale;

        public ReportMenu(ModEntry parent, string text, bool skipAnimation = false) : base(
            (int)Utility.getTopLeftPositionForCenteringOnScreen(320 * Game1.pixelZoom, 180 * Game1.pixelZoom).X,
            (int)Utility.getTopLeftPositionForCenteringOnScreen(320 * Game1.pixelZoom, 180 * Game1.pixelZoom).Y,
            320 * Game1.pixelZoom, 180 * Game1.pixelZoom, true)
        {
            _parent = parent;
            _firstKeyEvent = true;
            _scale = skipAnimation ? 1.0f : 0.0f;

            if (!skipAnimation)
                Game1.playSound("shwip");

            _backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + Game1.tileSize / 2,
                    yPositionOnScreen + height - Game1.tileSize / 2 - 16 * Game1.pixelZoom, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom)
            {
                myID = 101,
                upNeighborID = 103,
                rightNeighborID = 102
            };

            _forwardButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - Game1.tileSize / 2 - 12 * Game1.pixelZoom,
                    yPositionOnScreen + height - Game1.tileSize / 2 - 16 * Game1.pixelZoom, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), Game1.pixelZoom)
            {
                myID = 102,
                upNeighborID = 103,
                leftNeighborID = 101
            };

            _settingsButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 9 * Game1.pixelZoom, yPositionOnScreen + Game1.pixelZoom * 14,
                    12 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(366, 372, 17, 17),
                (float)(Game1.pixelZoom * 0.85))
            {
                myID = 103,
                upNeighborID = 100,
                downNeighborID = 102
            };

            _mailMessage =
                SpriteText.getStringBrokenIntoSectionsOfHeight(text, width - Game1.tileSize / 2,
                    height - Game1.tileSize * 2);

            upperRightCloseButton.myID = 100;
            upperRightCloseButton.downNeighborID = 103;

            if (!Game1.options.SnappyMenus || !Game1.options.gamepadControls) return;
            allClickableComponents =
                new List<ClickableComponent> { upperRightCloseButton, _backButton, _forwardButton, _settingsButton };
            snapToDefaultClickableComponent();
        }

        public sealed override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(102);
            snapCursorToCurrentSnappedComponent();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            xPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(320 * Game1.pixelZoom,
                                                                                     180 * Game1.pixelZoom).X;
            yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(320 * Game1.pixelZoom,
                                                                                     180 * Game1.pixelZoom).Y;

            _backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + Game1.tileSize / 2,
                    yPositionOnScreen + height - Game1.tileSize / 2 - 16 * Game1.pixelZoom, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom)
            {
                myID = 101,
                upNeighborID = 103,
                rightNeighborID = 102
            };

            _forwardButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - Game1.tileSize / 2 - 12 * Game1.pixelZoom,
                    yPositionOnScreen + height - Game1.tileSize / 2 - 16 * Game1.pixelZoom, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), Game1.pixelZoom)
            {
                myID = 102,
                upNeighborID = 103,
                leftNeighborID = 101
            };

            _settingsButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 9 * Game1.pixelZoom, yPositionOnScreen + Game1.pixelZoom * 14,
                    12 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(366, 372, 17, 17),
                (float)(Game1.pixelZoom * 0.85))
            {
                myID = 103,
                upNeighborID = 100,
                downNeighborID = 102
            };

            initializeUpperRightCloseButton();

            upperRightCloseButton.myID = 100;
            upperRightCloseButton.downNeighborID = 103;

            if (!Game1.options.SnappyMenus) return;

            allClickableComponents =
                new List<ClickableComponent> { upperRightCloseButton, _backButton, _forwardButton, _settingsButton };

            switch (currentlySnappedComponent.myID)
            {
                case 100:
                    currentlySnappedComponent = upperRightCloseButton;
                    break;

                case 101:
                    currentlySnappedComponent = _backButton;
                    break;

                case 102:
                    currentlySnappedComponent = _forwardButton;
                    break;

                case 103:
                    currentlySnappedComponent = _settingsButton;
                    break;

                default:
                    snapToDefaultClickableComponent();
                    break;
            }
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            switch (b)
            {
                case Buttons.LeftTrigger when _page > 0:
                    --_page;
                    Game1.playSound("shwip");
                    break;

                case Buttons.RightTrigger when _page < _mailMessage.Count - 1:
                    ++_page;
                    Game1.playSound("shwip");
                    break;

                default:
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (_backButton.containsPoint(x, y) && _page > 0)
            {
                --_page;
                Game1.playSound("shwip");
            }
            else if (_forwardButton.containsPoint(x, y) && _page < _mailMessage.Count - 1)
            {
                ++_page;
                Game1.playSound("shwip");
            }
            else if (_settingsButton.containsPoint(x, y))
            {
                SettingsMenu.OpenMenu(_parent);
            }
            else if (isWithinBounds(x, y))
            {
                if (_page < _mailMessage.Count - 1)
                {
                    ++_page;
                    Game1.playSound("shwip");
                }
                else if (_page == _mailMessage.Count - 1 && _mailMessage.Count > 1)
                {
                    _page = 0;
                    Game1.playSound("shwip");
                }
                else if (_mailMessage.Count == 1)
                {
                    exitThisMenuNoSound();
                    Game1.playSound("shwip");
                }
            }
            else
            {
                exitThisMenuNoSound();
                Game1.playSound("shwip");
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _settingsButton.tryHover(x, y, 0.5f);
            _backButton.tryHover(x, y, 0.6f);
            _forwardButton.tryHover(x, y, 0.6f);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (_scale < 1.0f)
            {
                _scale = (float)(_scale + time.ElapsedGameTime.Milliseconds * (3.0 / 1000.0));
                if (_scale >= 1.0f)
                    _scale = 1.0f;
            }
            if (_page >= _mailMessage.Count - 1 ||
                _forwardButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                return;
            _forwardButton.scale = (float)(4.0 + Math.Sin(time.TotalGameTime.Milliseconds / (64.0 * Math.PI)) / 1.5);
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);

            if ((SButton)key == _parent.Config.OpenReportKey && readyToClose())
            {
                if (_firstKeyEvent)
                {
                    _firstKeyEvent = false;
                    return;
                }
                exitThisMenu();
            }
            else if (key == Keys.Right && _page < _mailMessage.Count - 1)
            {
                ++_page;
                Game1.playSound("shwip");
            }
            else if (key == Keys.Left && _page > 0)
            {
                --_page;
                Game1.playSound("shwip");
            }
            else if ((SButton)key == _parent.Config.OpenSettings)
            {
                SettingsMenu.OpenMenu(_parent);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            b.Draw(_letterTexture, new Vector2(xPositionOnScreen + width / 2, yPositionOnScreen + height / 2),
                new Rectangle(0, 0, 320, 180), Color.White, 0.0f, new Vector2(160f, 90f), Game1.pixelZoom * _scale,
                SpriteEffects.None, 0.86f);

            if (_scale >= 1.0f)
            {
                SpriteText.drawString(b, _mailMessage[_page], xPositionOnScreen + Game1.tileSize / 2,
                    yPositionOnScreen + Game1.tileSize / 2, 999999, width - Game1.tileSize, 999999, 0.75f, 0.865f);
                base.draw(b);
                _settingsButton.draw(b);

                if (_page < _mailMessage.Count - 1)
                    _forwardButton.draw(b);
                if (_page > 0)
                    _backButton.draw(b);
            }

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0.0f,
                    Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }
    }
}