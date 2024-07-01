/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LonerAxl/Stardew_HarvestCalendar
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarvestCalendar.Framework;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HarvestCalendar.Menu
{
    /// <summary>
    /// Modified from https://github.com/Hedgehog-Technologies/StardewMods/blob/main/src/AutoTrasher/Components/TrashListMenu.cs
    /// </summary>
    internal abstract class ScrollableMenu : IClickableMenu
    {
        //private readonly Configuration _config;


        private ClickableComponent _title;
        private string title;
        private ClickableTextureComponent _upArrow;
        private ClickableTextureComponent _downArrow;
        private ClickableTextureComponent _scrollbar;
        private Rectangle _scrollbarRunner;
        private int _currentItemIndex;
        private bool _isScrolling;

        protected List<ClickableComponent> _optionSlots;
        protected List<OptionsElement> _options;

        private const int ItemsPerPage = 10;
        private int _optionsSlotHeld;

        private const int ButtonBorderWidth = 4 * Game1.pixelZoom;



        public ScrollableMenu(string title)
        {
            //_config = config;
            _options = new();
            _optionSlots = new();
            this.title = title;

            _optionsSlotHeld = -1;
            _currentItemIndex = 0;

            Game1.playSound("bigSelect");

            ResetComponents();
            //SetOptions();
        }

        public void ExitIfValid()
        {
            if (readyToClose() && !GameMenu.forcePreventClose)
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        protected void ResetComponents()
        {
            width = 800 + borderWidth * 2;
            height = 600 + borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width - (int)(Game1.tileSize / 2.4f)) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

            // TITLE
            _title = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2, yPositionOnScreen, Game1.tileSize * 4, Game1.tileSize), title);

            // SCROLL UI
            var scrollbarOffset = Game1.tileSize * 4 / 16;
            _upArrow = new ClickableTextureComponent("up-arrow", new Rectangle(xPositionOnScreen + width + scrollbarOffset, yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            _downArrow = new ClickableTextureComponent("down-arrow", new Rectangle(xPositionOnScreen + width + scrollbarOffset, yPositionOnScreen + height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            _scrollbar = new ClickableTextureComponent("scrollbar", new Rectangle(_upArrow.bounds.X + Game1.pixelZoom * 3, _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            _scrollbarRunner = new Rectangle(_scrollbar.bounds.X, _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom, _scrollbar.bounds.Width, height - Game1.tileSize * 2 - _upArrow.bounds.Height - Game1.pixelZoom * 2);
            SetScrollbarToCurrentIndex();

            // OPTION SLOTS
            _optionSlots.Clear();
            for (int i = 0; i < ItemsPerPage; i++)
            {
                _optionSlots.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * ((height - Game1.tileSize * 2) / ItemsPerPage), width - Game1.tileSize / 2, (height - Game1.tileSize * 2) / ItemsPerPage + Game1.pixelZoom), string.Concat(i)));
            }
        }

        protected abstract void SetOptions();



        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            }

            base.draw(b);

            // Draw Menu Box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // Draw Title Box
            DrawTab(_title.bounds.X, _title.bounds.Y, Game1.dialogueFont, _title.name, 1);

            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

            // Draw options
            for (int idx = 0; idx < _optionSlots.Count; idx++)
            {
                if (_currentItemIndex >= 0 && _currentItemIndex + idx < _options.Count)
                {
                    _options[_currentItemIndex + idx].draw(b, _optionSlots[idx].bounds.X, _optionSlots[idx].bounds.Y + 5);
                }
            }

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (!GameMenu.forcePreventClose)
            {

                // Draw Scrollbar when scrolling can happen
                if (_options.Count > ItemsPerPage)
                {
                    _upArrow.draw(b);
                    _downArrow.draw(b);
                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), _scrollbarRunner.X, _scrollbarRunner.Y, _scrollbarRunner.Width, _scrollbarRunner.Height, Color.White, Game1.pixelZoom, false);
                    _scrollbar.draw(b);
                }
            }

            // Draw mouse / cursor
            if (!Game1.options.hardwareCursor)
            {
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }

        private static void DrawTab(int x, int y, SpriteFont font, string text, int align = 0, float alpha = 1, bool drawShadow = true)
        {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);

            DrawTab(x, y, (int)bounds.X, (int)bounds.Y, out Vector2 drawPos, align, alpha, drawShadow);
            Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
        }

        private static void DrawTab(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition, int align = 0, float alpha = 1, bool drawShadow = true)
        {
            var spriteBatch = Game1.spriteBatch;

            // calculate outer coordinates
            var outerWidth = innerWidth + ButtonBorderWidth * 2;
            var outerHeight = innerHeight + Game1.tileSize / 3;
            var offsetX = align switch
            {
                1 => -outerWidth / 2,
                2 => -outerWidth,
                _ => 0
            };

            drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
            innerDrawPosition = new Vector2(x + ButtonBorderWidth + offsetX, y + ButtonBorderWidth);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose) return;

            base.leftClickHeld(x, y);

            if (_isScrolling)
            {
                var num = _scrollbar.bounds.Y;

                _scrollbar.bounds.Y = Math.Min(
                    yPositionOnScreen + height - Game1.tileSize - Game1.pixelZoom * 3 - _scrollbar.bounds.Height,
                    Math.Max(y, yPositionOnScreen + _upArrow.bounds.Height + Game1.pixelZoom * 5));

                _currentItemIndex = Math.Min(
                    _options.Count - ItemsPerPage,
                    Math.Max(0, (int)Math.Round((_options.Count - ItemsPerPage) * (double)((y - _scrollbarRunner.Y) / (float)_scrollbarRunner.Height))));

                SetScrollbarToCurrentIndex();

                if (num == _scrollbar.bounds.Y) return;

                Game1.playSound("shiny4");
            }
            else
            {
                if (_optionsSlotHeld == -1 || _optionsSlotHeld + _currentItemIndex >= _options.Count) return;

                _options[_currentItemIndex + _optionsSlotHeld].leftClickHeld(x - _optionSlots[_optionsSlotHeld].bounds.X, y - _optionSlots[_optionsSlotHeld].bounds.Y);
            }
        }



        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose) return;

            base.releaseLeftClick(x, y);

            if (_optionsSlotHeld != -1 && _optionsSlotHeld + _currentItemIndex < _options.Count)
            {
                _options[_currentItemIndex + _optionsSlotHeld].leftClickReleased(x - _optionSlots[_optionsSlotHeld].bounds.X, y - _optionSlots[_optionsSlotHeld].bounds.Y);
            }

            _optionsSlotHeld = -1;
            _isScrolling = false;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.menuButton.Contains(new InputButton(key)))
            {
                ExitIfValid();
            }
            else
            {
                GetActiveOption()?.receiveKeyPress(key);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose) return;

            base.receiveLeftClick(x, y, playSound);

            if (_downArrow.containsPoint(x, y) && _currentItemIndex < Math.Max(0, _options.Count - ItemsPerPage))
            {
                DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_upArrow.containsPoint(x, y) && _currentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_scrollbar.containsPoint(x, y))
            {
                _isScrolling = true;
            }
            else if (!_downArrow.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + Game1.tileSize * 2 && y > yPositionOnScreen && y < yPositionOnScreen + height)
            {
                _isScrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }

            _currentItemIndex = Math.Max(0, Math.Min(_options.Count - ItemsPerPage, _currentItemIndex));

            for (int idx = 0; idx < _optionSlots.Count; ++idx)
            {
                
                if (_optionSlots[idx].bounds.Contains(x, y) && _currentItemIndex + idx < _options.Count)
                {
                    _options[_currentItemIndex + idx].receiveLeftClick(x - _optionSlots[idx].bounds.X, y - _optionSlots[idx].bounds.Y + 5);
                    _optionsSlotHeld = idx;
                    break;
                }
            }


        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose) return;

            base.receiveScrollWheelAction(direction);

            if (direction > 0 && _currentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }
            else
            {
                if (direction >= 0 || _currentItemIndex >= Math.Max(0, _options.Count - ItemsPerPage)) return;

                DownArrowPressed();
                Game1.playSound("shwip");
            }
        }

        private OptionsElement? GetActiveOption()
        {
            if (_optionsSlotHeld == -1) return null;

            var idx = _currentItemIndex + _optionsSlotHeld;

            return idx < _options.Count
                ? _options[idx]
                : null;
        }
        private void SetScrollbarToCurrentIndex()
        {
            if (!_options.Any()) return;

            if (_options.Count > ItemsPerPage)
            {
                if (_currentItemIndex + ItemsPerPage > _options.Count)
                {
                    _currentItemIndex = _options.Count - ItemsPerPage;
                }
            }
            else
            {
                _currentItemIndex = 0;
            }

            _scrollbar.bounds.Y = _scrollbarRunner.Height / Math.Max(1, _options.Count - ItemsPerPage + 1) * _currentItemIndex + _upArrow.bounds.Bottom + Game1.pixelZoom;

            if (_currentItemIndex != _options.Count - ItemsPerPage) return;

            _scrollbar.bounds.Y = _downArrow.bounds.Y - _scrollbar.bounds.Height - Game1.pixelZoom;
        }


        private void DownArrowPressed()
        {
            _downArrow.scale = _downArrow.baseScale;
            ++_currentItemIndex;
            SetScrollbarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            _upArrow.scale = _upArrow.baseScale;
            --_currentItemIndex;
            SetScrollbarToCurrentIndex();
        }

        public override void performHoverAction(int x, int y)
        {
            if (GameMenu.forcePreventClose) return;

            _upArrow.tryHover(x, y);
            _downArrow.tryHover(x, y);
            _scrollbar.tryHover(x, y);
        }

    }
}
