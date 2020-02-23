using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModSettingsTab.Framework.Components;
using StardewValley;
using StardewValley.Menus;
using OptionsElement = ModSettingsTab.Framework.Components.OptionsElement;
using TextBox = ModSettingsTab.Framework.Components.TextBox;

namespace ModSettingsTab.Menu
{
    public abstract class BaseOptionsModPage : IClickableMenu
    {
        protected string HoverTitle = "";
        protected string HoverText = "";
        private readonly List<ClickableComponent> _optionSlots = new List<ClickableComponent>();

        public List<OptionsElement> Options = new List<OptionsElement>();

        private int _optionsSlotHeld = -1;
        private const int ItemsPerPage = 7;
        private const int IndexOfGraphicsPage = 6;
        private int _currentItemIndex;
        private readonly ClickableTextureComponent _upArrow;
        private readonly ClickableTextureComponent _downArrow;
        private readonly ClickableTextureComponent _scrollBar;
        private bool _scrolling;
        private Rectangle _scrollBarRunner;
        public static Point SlotSize;
        protected FilterTextBox FilterTextBox;

        static BaseOptionsModPage()
        {
            SlotSize = new Point( 800 + borderWidth * 2- 32 + ModData.Offset, (600 + borderWidth * 2 - 128) / ItemsPerPage);
        }
        protected BaseOptionsModPage(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            _upArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48),
                Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            _downArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48),
                Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            _scrollBar = new ClickableTextureComponent(
                new Rectangle(_upArrow.bounds.X + 12, _upArrow.bounds.Y + _upArrow.bounds.Height + 4, 24,
                    40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            _scrollBarRunner = new Rectangle(_scrollBar.bounds.X,
                _upArrow.bounds.Y + _upArrow.bounds.Height + 4, _scrollBar.bounds.Width,
                height - 128 - _upArrow.bounds.Height - 8);
            for (var index = 0; index < ItemsPerPage; ++index)
                _optionSlots.Add(
                    new ClickableComponent(
                        new Rectangle(xPositionOnScreen + 16,
                            yPositionOnScreen + 80 + 4 + index * SlotSize.Y, SlotSize.X,
                            SlotSize.Y + 4), string.Concat(index))
                    {
                        myID = index,
                        downNeighborID = index < IndexOfGraphicsPage ? index + 1 : -7777,
                        upNeighborID = index > 0 ? index - 1 : -7777,
                        fullyImmutable = true
                    });
            
        }

        public override void snapToDefaultClickableComponent()
        {
            _currentItemIndex = 0;
            base.snapToDefaultClickableComponent();
            currentlySnappedComponent = getComponentWithID(1);
            snapCursorToCurrentSnappedComponent();
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldId)
        {
            base.customSnapBehavior(direction, oldRegion, oldId);
            if (oldId == 6 && direction == 2 && _currentItemIndex < Math.Max(0, Options.Count - ItemsPerPage))
            {
                DownArrowPressed();
                Game1.playSound("shiny4");
            }
            else
            {
                if (oldId != 0 || (uint) direction > 0U)
                    return;
                if (_currentItemIndex > 0)
                {
                    UpArrowPressed();
                    Game1.playSound("shiny4");
                }
                else
                {
                    currentlySnappedComponent = getComponentWithID(12346);
                    if (currentlySnappedComponent != null)
                        currentlySnappedComponent.downNeighborID = 0;
                    snapCursorToCurrentSnappedComponent();
                }
            }
        }

        public void SetScrollBarToCurrentIndex()
        {
            if (Options.Count <= 0)
                return;
            _scrollBar.bounds.Y =
                _scrollBarRunner.Height / Math.Max(1, Options.Count - ItemsPerPage + 1) * _currentItemIndex +
                _upArrow.bounds.Bottom + 4;
            if (_currentItemIndex != Options.Count - ItemsPerPage)
                return;
            _scrollBar.bounds.Y = _downArrow.bounds.Y - _scrollBar.bounds.Height - 4;
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (currentlySnappedComponent != null && currentlySnappedComponent.myID < Options.Count)
            {
                Game1.setMousePosition(currentlySnappedComponent.bounds.Left + 48,
                    currentlySnappedComponent.bounds.Center.Y - 12);
            }
            else
            {
                if (currentlySnappedComponent == null)
                    return;
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x, y);
            if (_scrolling)
            {
                var y1 = _scrollBar.bounds.Y;
                _scrollBar.bounds.Y =
                    Math.Min(yPositionOnScreen + height - 64 - 12 - _scrollBar.bounds.Height,
                        Math.Max(y, yPositionOnScreen + _upArrow.bounds.Height + 20));
                _currentItemIndex = Math.Min(Options.Count - ItemsPerPage,
                    Math.Max(0,
                        (int) (Options.Count *
                               ((y - _scrollBarRunner.Y) / (double) _scrollBarRunner.Height))));
                SetScrollBarToCurrentIndex();
                var y2 = _scrollBar.bounds.Y;
                if (y1 == y2)
                    return;
                Game1.playSound("shiny4");
            }
            else
            {
                if (_optionsSlotHeld == -1 || _optionsSlotHeld + _currentItemIndex >= Options.Count)
                    return;
                Options[_currentItemIndex + _optionsSlotHeld].LeftClickHeld(
                    x - _optionSlots[_optionsSlotHeld].bounds.X,
                    y - _optionSlots[_optionsSlotHeld].bounds.Y);
            }
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            return currentlySnappedComponent;
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            currentlySnappedComponent = getComponentWithID(id);
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (_optionsSlotHeld != -1 && _optionsSlotHeld + _currentItemIndex < Options.Count ||
                Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                if (currentlySnappedComponent != null && Game1.options.snappyMenus &&
                    (Game1.options.gamepadControls &&
                     Options.Count > _currentItemIndex + currentlySnappedComponent.myID) &&
                    _currentItemIndex + currentlySnappedComponent.myID >= 0)
                    Options[_currentItemIndex + currentlySnappedComponent.myID].ReceiveKeyPress(key);
                else if (Options.Count > _currentItemIndex + _optionsSlotHeld &&
                         _currentItemIndex + _optionsSlotHeld >= 0)
                    Options[_currentItemIndex + _optionsSlotHeld].ReceiveKeyPress(key);
            }

            base.receiveKeyPress(key);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && _currentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shiny4");
            }
            else
            {
                if (direction >= 0 || _currentItemIndex >= Math.Max(0, Options.Count - ItemsPerPage))
                    return;
                DownArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            FilterTextBox?.Update();
            if (_optionsSlotHeld != -1 && _optionsSlotHeld + _currentItemIndex < Options.Count)
                Options[_currentItemIndex + _optionsSlotHeld].LeftClickReleased(
                    x - _optionSlots[_optionsSlotHeld].bounds.X,
                    y - _optionSlots[_optionsSlotHeld].bounds.Y);
            _optionsSlotHeld = -1;
            _scrolling = false;
        }

        private void DownArrowPressed()
        {
            _downArrow.scale = _downArrow.baseScale;
            ++_currentItemIndex;
            SetScrollBarToCurrentIndex();
            TextBox.GlobalUpdate();
        }

        private void UpArrowPressed()
        {
            _upArrow.scale = _upArrow.baseScale;
            --_currentItemIndex;
            SetScrollBarToCurrentIndex();
            TextBox.GlobalUpdate();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            if (_downArrow.containsPoint(x, y) && _currentItemIndex < Math.Max(0, Options.Count - ItemsPerPage))
            {
                DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_upArrow.containsPoint(x, y) && _currentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_scrollBar.containsPoint(x, y))
                _scrolling = true;
            else if (!_downArrow.containsPoint(x, y) && x > xPositionOnScreen + width &&
                     (x < xPositionOnScreen + width + 128 && y > yPositionOnScreen) &&
                     y < yPositionOnScreen + height)
            {
                _scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }

            _currentItemIndex = Math.Max(0, Math.Min(Options.Count - 7, _currentItemIndex));
            for (var index = 0; index < _optionSlots.Count; ++index)
            {
                if (_optionSlots[index].bounds.Contains(x, y) &&
                    _currentItemIndex + index < Options.Count && Options[_currentItemIndex + index]
                        .Bounds.Contains(x - _optionSlots[index].bounds.X, y - _optionSlots[index].bounds.Y))
                {
                    Options[_currentItemIndex + index].ReceiveLeftClick(x - _optionSlots[index].bounds.X,
                        y - _optionSlots[index].bounds.Y);
                    _optionsSlotHeld = index;
                    break;
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            if (_optionSlots.Where((t, index) =>
            {
                if (_currentItemIndex < 0 || _currentItemIndex + index >= Options.Count) return false;
                if (!Options[_currentItemIndex + index].PerformHoverAction(x - t.bounds.X, y - t.bounds.Y))
                    return false;
                HoverTitle = Options[_currentItemIndex + index].HoverTitle;
                HoverText = Options[_currentItemIndex + index].HoverText;
                return true;
            }).Any())
                Game1.SetFreeCursorDrag();
            else HoverText = "";
            if (_scrollBarRunner.Contains(x, y))
                Game1.SetFreeCursorDrag();
            if (GameMenu.forcePreventClose)
                return;
            _upArrow.tryHover(x, y);
            _downArrow.tryHover(x, y);
            _scrollBar.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            for (var index = 0; index < _optionSlots.Count; ++index)
            {
                if (_currentItemIndex >= 0 && _currentItemIndex + index < Options.Count)
                    Options[_currentItemIndex + index].Draw(b, _optionSlots[index].bounds.X,
                        _optionSlots[index].bounds.Y);
            }

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null,null,null,
                Game1.gameMode == 0 ? Matrix.CreateScale(0.9f) : new Matrix?() );
            if (!GameMenu.forcePreventClose)
            {
                _upArrow.draw(b);
                FilterTextBox?.Draw(b);
                _downArrow.draw(b);
                if (Options.Count > ItemsPerPage)
                {
                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6),
                        _scrollBarRunner.X, _scrollBarRunner.Y, _scrollBarRunner.Width,
                        _scrollBarRunner.Height, Color.White, 4f, false);
                    _scrollBar.draw(b);
                }
            }

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (HoverText.Equals(""))
                return;
            if (HoverTitle.Equals(""))
                drawHoverText(b, HoverText, Game1.smallFont);
            else
                drawHoverText(b, HoverText, Game1.smallFont, boldTitleText: HoverTitle);
        }
    }
}