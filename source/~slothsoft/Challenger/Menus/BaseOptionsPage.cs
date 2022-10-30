/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace Slothsoft.Challenger.Menus;

/// <summary>
/// A menu page that displays a selection of options of some kind.
/// This is a more general class to reuse (maybe). After extending this class you need to call
/// <see cref="AddOption"/> to add your option entries.
/// </summary>
internal abstract class BaseOptionsPage : IClickableMenu {
    public const int ItemsPerPage = 7;

    private readonly List<ClickableComponent> _optionSlots = new();
    private readonly List<OptionsElement> _options = new(ItemsPerPage);

    private readonly ClickableTextureComponent _upArrow;
    private readonly ClickableTextureComponent _downArrow;
    private readonly ClickableTextureComponent _scrollBar;

    private int _currentItemIndex;
    private bool _scrolling;
    private Rectangle _scrollBarRunner;

    private HeldOption? _heldOption;

    public BaseOptionsPage()
        : this(new Rectangle(
            Game1.viewport.Width / 2 - (800 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2,
            800 + borderWidth * 2, 
            600 + borderWidth * 2
            )) {
    }

    public BaseOptionsPage(Rectangle bounds) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height) {
        _upArrow = new ClickableTextureComponent(
            new Rectangle(
                xPositionOnScreen + width + 16,
                yPositionOnScreen + 64,
                44,
                48
            ), Game1.mouseCursors,
            new Rectangle(421, 459, 11, 12), 4f);
        _downArrow = new ClickableTextureComponent(
            new Rectangle(
                xPositionOnScreen + width + 16,
                yPositionOnScreen + height - 64,
                44,
                48
            ), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
        _scrollBar = new ClickableTextureComponent(
            new Rectangle(
                _upArrow.bounds.X + 12,
                _upArrow.bounds.Y + _upArrow.bounds.Height + 4,
                24,
                40
            ), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
        _scrollBarRunner = new Rectangle(
            _scrollBar.bounds.X,
            _upArrow.bounds.Y + _upArrow.bounds.Height + 4,
            _scrollBar.bounds.Width,
            height - 128 - _upArrow.bounds.Height - 8
        );

        const int padding = 16;
        const int tabsHeight = 80;
        var slotHeight = (height - 128) / 7;

        for (var i = 0; i < ItemsPerPage; ++i) {
            _optionSlots.Add(
                new ClickableComponent(
                    new Rectangle(
                        xPositionOnScreen + padding,
                        yPositionOnScreen + tabsHeight + i * slotHeight + padding,
                        width - 2 * padding,
                        slotHeight),
                    i.ToString()
                ) {
                    myID = i,
                    downNeighborID = i < 6 ? i + 1 : -7777,
                    upNeighborID = i > 0 ? i - 1 : -7777,
                    fullyImmutable = true
                });
        }
    }

    public void AddOption(OptionsElement option) {
        _options.Add(option);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true) {
        if (GameMenu.forcePreventClose) {
            return;
        }

        if (_downArrow.containsPoint(x, y) && _currentItemIndex < Math.Max(0, _options.Count - 7)) {
            DownArrowPressed();
            Game1.playSound("shwip");
        } else if (_upArrow.containsPoint(x, y) && _currentItemIndex > 0) {
            UpArrowPressed();
            Game1.playSound("shwip");
        } else if (_scrollBar.containsPoint(x, y)) {
            _scrolling = true;
        } else if (!_downArrow.containsPoint(x, y)
                   && x > xPositionOnScreen + width
                   && x < xPositionOnScreen + width + 128
                   && y > yPositionOnScreen
                   && y < yPositionOnScreen + height) {
            _scrolling = true;
            leftClickHeld(x, y);
            releaseLeftClick(x, y);
        }

        _currentItemIndex = Math.Max(0, Math.Min(_options.Count - ItemsPerPage, _currentItemIndex));
        for (var index = 0; index < _optionSlots.Count; ++index) {
            if (_optionSlots[index].bounds.Contains(x, y) && _currentItemIndex + index < _options.Count) {
                _heldOption = new HeldOption(_optionSlots[index], _options[_currentItemIndex + index]);
                _heldOption.Element.receiveLeftClick(x - _optionSlots[index].bounds.X, y - _optionSlots[index].bounds.Y);
                break;
            }
        }
    }

    public override void releaseLeftClick(int x, int y) {
        if (GameMenu.forcePreventClose)
            return;
        base.releaseLeftClick(x, y);
        
        _heldOption?.Element.leftClickReleased(x - _heldOption.Slot.bounds.X, y - _heldOption.Slot.bounds.Y);
        _heldOption = null;
        _scrolling = false;
    }

    public override void snapToDefaultClickableComponent() {
        base.snapToDefaultClickableComponent();
        currentlySnappedComponent = getComponentWithID(1);
        snapCursorToCurrentSnappedComponent();
    }

    public override void applyMovementKey(int direction) {
        if (!HasActiveOverlay()) {
            base.applyMovementKey(direction);
        }
    }

    private bool HasActiveOverlay() {
        return _heldOption is {Element: OptionsDropDown};
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldId) {
        base.customSnapBehavior(direction, oldRegion, oldId);

        if (oldId == ItemsPerPage - 1 && direction == 2 && _currentItemIndex < Math.Max(0, _options.Count - ItemsPerPage)) {
            DownArrowPressed();
            Game1.playSound("shiny4");
        } else {
            if (oldId != 0 || direction != 0)
                return;
            if (_currentItemIndex > 0) {
                UpArrowPressed();
                Game1.playSound("shiny4");
            } else {
                currentlySnappedComponent = getComponentWithID(12346);
                if (currentlySnappedComponent != null)
                    currentlySnappedComponent.downNeighborID = 0;
                snapCursorToCurrentSnappedComponent();
            }
        }
    }

    private void SetScrollBarToCurrentIndex() {
        if (_options.Count <= 0)
            return;
        _scrollBar.bounds.Y = _scrollBarRunner.Height / Math.Max(1, _options.Count - ItemsPerPage + 1) * _currentItemIndex + _upArrow.bounds.Bottom + 4;

        if (_scrollBar.bounds.Y <= _downArrow.bounds.Y - _scrollBar.bounds.Height - 4)
            return;
        _scrollBar.bounds.Y = _downArrow.bounds.Y - _scrollBar.bounds.Height - 4;
    }

    public override void leftClickHeld(int x, int y) {
        if (GameMenu.forcePreventClose)
            return;
        base.leftClickHeld(x, y);

        if (_scrolling) {
            var y1 = _scrollBar.bounds.Y;
            _scrollBar.bounds.Y = Math.Min(
                yPositionOnScreen + height - 64 - 12 - _scrollBar.bounds.Height,
                Math.Max(y, yPositionOnScreen + _upArrow.bounds.Height + 20)
            );
            _currentItemIndex = Math.Min(
                _options.Count - 7,
                Math.Max(0, (int) (_options.Count * (double) ((y - _scrollBarRunner.Y) / (float) _scrollBarRunner.Height)))
            );
            SetScrollBarToCurrentIndex();
            var y2 = _scrollBar.bounds.Y;
            if (y1 == y2) {
                return;
            }

            Game1.playSound("shiny4");
        } else {
            if (_heldOption == null) {
                return;
            }

            _heldOption.Element.leftClickHeld(x - _heldOption.Slot.bounds.X, y - _heldOption.Slot.bounds.Y);
        }
    }

    public override void setCurrentlySnappedComponentTo(int id) {
        base.setCurrentlySnappedComponentTo(id);
        snapCursorToCurrentSnappedComponent();
    }

    public override void receiveKeyPress(Keys key) {
        if (_heldOption != null || Game1.options.snappyMenus && Game1.options.gamepadControls) {
            if (currentlySnappedComponent != null && Game1.options.snappyMenus && Game1.options.gamepadControls &&
                _options.Count > _currentItemIndex + currentlySnappedComponent.myID &&
                _currentItemIndex + currentlySnappedComponent.myID >= 0) {
                _options[_currentItemIndex + currentlySnappedComponent.myID].receiveKeyPress(key);
            } else {
                _heldOption?.Element.receiveKeyPress(key);
            }
        }

        base.receiveKeyPress(key);
    }

    public override void receiveScrollWheelAction(int direction) {
        if (GameMenu.forcePreventClose || HasActiveOverlay())
            return;
        base.receiveScrollWheelAction(direction);
        if (direction > 0 && _currentItemIndex > 0) {
            UpArrowPressed();
            Game1.playSound("shiny4");
        } else if (direction < 0 && _currentItemIndex < Math.Max(0, _options.Count - 7)) {
            DownArrowPressed();
            Game1.playSound("shiny4");
        }

        if (!Game1.options.SnappyMenus)
            return;
        snapCursorToCurrentSnappedComponent();
    }


    private void DownArrowPressed() {
        if (HasActiveOverlay())
            return;
        _downArrow.scale = _downArrow.baseScale;
        _currentItemIndex ++;
        SetScrollBarToCurrentIndex();
    }

    private void UpArrowPressed() {
        if (HasActiveOverlay())
            return;
        _upArrow.scale = _upArrow.baseScale;
        _currentItemIndex--;
        SetScrollBarToCurrentIndex();
    }


    public override void receiveRightClick(int x, int y, bool playSound = true) {
    }

    public override void performHoverAction(int x, int y) {
        if (_scrollBarRunner.Contains(x, y))
            Game1.SetFreeCursorDrag();
        if (GameMenu.forcePreventClose)
            return;
        _upArrow.tryHover(x, y);
        _downArrow.tryHover(x, y);
        _scrollBar.tryHover(x, y);
    }

    public override void draw(SpriteBatch b) {
        b.End();
        b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

        for (var index = 0; index < _optionSlots.Count; ++index) {
            if (_currentItemIndex + index >= _options.Count) {
                continue;
            }
            var currentOption = _options[_currentItemIndex + index];
            currentOption.draw(b, _optionSlots[index].bounds.X, _optionSlots[index].bounds.Y,this);
        }

        b.End();
        b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        if (!GameMenu.forcePreventClose) {
            _upArrow.draw(b);
            _downArrow.draw(b);
            if (_options.Count > ItemsPerPage) {
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6),
                    _scrollBarRunner.X, _scrollBarRunner.Y, _scrollBarRunner.Width,
                    _scrollBarRunner.Height, Color.White, 4f, false);
                _scrollBar.draw(b);
            }
        }
    }
    
    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
        var xDiff = xPositionOnScreen;
        var yDiff = yPositionOnScreen;
        base.gameWindowSizeChanged(oldBounds, newBounds);
        xDiff -= xPositionOnScreen;
        yDiff -= yPositionOnScreen;
            
        // move all items on the page the same amount as the entire page
        _upArrow.bounds.X -= xDiff;
        _upArrow.bounds.Y -= yDiff;
        _downArrow.bounds.X -= xDiff;
        _downArrow.bounds.Y -= yDiff;
        _scrollBar.bounds.X -= xDiff;
        _scrollBar.bounds.Y -= yDiff;
        foreach (var optionSlot in _optionSlots) {
            optionSlot.bounds.X -= xDiff;
            optionSlot.bounds.Y -= yDiff;
        }
    }
}

internal record HeldOption(ClickableComponent Slot, OptionsElement Element);