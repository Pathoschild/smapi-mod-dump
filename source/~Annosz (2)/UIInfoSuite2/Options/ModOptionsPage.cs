/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace UIInfoSuite2.Options;

/// <summary>Our mod options made page to be added to <see cref="GameMenu.pages" /></summary>
public class ModOptionsPage : IClickableMenu
{
  private const int visibleSlots = 7;
  private const int Width = 880;
  private readonly ClickableTextureComponent _downArrow;
  private readonly List<ModOptionsElement> _options;
  private readonly ClickableTextureComponent _scrollBar;
  private readonly ClickableTextureComponent _upArrow;
  private int _currentItemIndex;
  private string _hoverText;
  private bool _isScrolling;

  /// <summary>
  ///   The visible option slots
  ///   <para>
  ///     Must be public so
  ///     <see cref="IClickableMenu.populateClickableComponentList" /> can find it.
  ///   </para>
  /// </summary>
  public List<ClickableComponent> _optionSlots = new();

  private int _optionsSlotHeld = -1;
  private Rectangle _scrollBarRunner;

  public ModOptionsPage(List<ModOptionsElement> options, IModEvents events) : base(
    Game1.activeClickableMenu.xPositionOnScreen,
    Game1.activeClickableMenu.yPositionOnScreen + 10,
    Width,
    Game1.activeClickableMenu.height
  )
  {
    _options = options;
    _upArrow = new ClickableTextureComponent(
      new Rectangle(
        xPositionOnScreen + width + Game1.tileSize / 4,
        yPositionOnScreen + Game1.tileSize,
        11 * Game1.pixelZoom,
        12 * Game1.pixelZoom
      ),
      Game1.mouseCursors,
      new Rectangle(421, 459, 11, 12),
      Game1.pixelZoom
    );

    _downArrow = new ClickableTextureComponent(
      new Rectangle(
        _upArrow.bounds.X,
        yPositionOnScreen + height - Game1.tileSize,
        _upArrow.bounds.Width,
        _upArrow.bounds.Height
      ),
      Game1.mouseCursors,
      new Rectangle(421, 472, 11, 12),
      Game1.pixelZoom
    );

    _scrollBar = new ClickableTextureComponent(
      new Rectangle(
        _upArrow.bounds.X + Game1.pixelZoom * 3,
        _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom,
        6 * Game1.pixelZoom,
        10 * Game1.pixelZoom
      ),
      Game1.mouseCursors,
      new Rectangle(435, 463, 6, 10),
      Game1.pixelZoom
    );

    _scrollBarRunner = new Rectangle(
      _scrollBar.bounds.X,
      _scrollBar.bounds.Y,
      _scrollBar.bounds.Width,
      height - Game1.tileSize * 2 - _upArrow.bounds.Height - Game1.pixelZoom * 2
    );

    for (var i = 0; i < visibleSlots; ++i)
    {
      // tqdv: I'm not sure where Game1.tileSize and Game1.pixelZoom come from
      var component = new ClickableComponent(
        new Rectangle(
          xPositionOnScreen + Game1.tileSize / 4,
          yPositionOnScreen +
          Game1.tileSize * 5 / 4 +
          Game1.pixelZoom +
          i * (height - Game1.tileSize * 2) / visibleSlots,
          width - Game1.tileSize / 2,
          (height - Game1.tileSize * 2) / visibleSlots + Game1.pixelZoom
        ),
        i.ToString()
      )
      {
        myID = i,
        downNeighborID = i + 1 < visibleSlots ? i + 1 : ClickableComponent.CUSTOM_SNAP_BEHAVIOR,
        upNeighborID = i - 1 >= 0 ? i - 1 : ClickableComponent.CUSTOM_SNAP_BEHAVIOR,
        fullyImmutable = true
      };
      _optionSlots.Add(component);
    }

    events.Display.MenuChanged += OnMenuChanged;
  }

  /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
  /// <param name="sender">The event sender.</param>
  /// <param name="e">The event arguments.</param>
  private void OnMenuChanged(object sender, MenuChangedEventArgs e)
  {
    if (e.NewMenu is GameMenu)
    {
      xPositionOnScreen = Game1.activeClickableMenu.xPositionOnScreen;
      yPositionOnScreen = Game1.activeClickableMenu.yPositionOnScreen + 10;
      height = Game1.activeClickableMenu.height;

      for (var i = 0; i < _optionSlots.Count; ++i)
      {
        ClickableComponent next = _optionSlots[i];
        next.bounds.X = xPositionOnScreen + Game1.tileSize / 4;
        next.bounds.Y = yPositionOnScreen +
                        Game1.tileSize * 5 / 4 +
                        Game1.pixelZoom +
                        i * (height - Game1.tileSize * 2) / 7;
        next.bounds.Width = width - Game1.tileSize / 2;
        next.bounds.Height = (height - Game1.tileSize * 2) / 7 + Game1.pixelZoom;
      }

      _upArrow.bounds.X = xPositionOnScreen + width + Game1.tileSize / 4;
      _upArrow.bounds.Y = yPositionOnScreen + Game1.tileSize;
      _upArrow.bounds.Width = 11 * Game1.pixelZoom;
      _upArrow.bounds.Height = 12 * Game1.pixelZoom;

      _downArrow.bounds.X = _upArrow.bounds.X;
      _downArrow.bounds.Y = yPositionOnScreen + height - Game1.tileSize;
      _downArrow.bounds.Width = _upArrow.bounds.Width;
      _downArrow.bounds.Height = _upArrow.bounds.Height;

      _scrollBar.bounds.X = _upArrow.bounds.X + Game1.pixelZoom * 3;
      _scrollBar.bounds.Y = _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom;
      _scrollBar.bounds.Width = 6 * Game1.pixelZoom;
      _scrollBar.bounds.Height = 10 * Game1.pixelZoom;

      _scrollBarRunner.X = _scrollBar.bounds.X;
      _scrollBarRunner.Y = _scrollBar.bounds.Y;
      _scrollBarRunner.Width = _scrollBar.bounds.Width;
      _scrollBarRunner.Height = height - Game1.tileSize * 2 - _upArrow.bounds.Height - Game1.pixelZoom * 2;
    }
  }

  public override void snapToDefaultClickableComponent()
  {
    currentlySnappedComponent = getComponentWithID(1);
    snapCursorToCurrentSnappedComponent();
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    if (oldID == visibleSlots - 1 && direction == Game1.down)
    {
      if (_currentItemIndex + visibleSlots < _options.Count)
      {
        DownArrowPressed();
        Game1.playSound("shiny4");
      }
    }
    else if (oldID == 0 && direction == Game1.up)
    {
      if (_currentItemIndex > 0)
      {
        UpArrowPressed();
        Game1.playSound("shiny4");
      }
      else
      {
        // Already at the top, move to the menu tab
        currentlySnappedComponent = getComponentWithID(12348);
        if (currentlySnappedComponent != null)
        {
          // Set the down neighbor of the tab to the first slot, instead of the default (which is the second slot)
          currentlySnappedComponent.downNeighborID = 0;
        }

        snapCursorToCurrentSnappedComponent();
      }
    }
  }

  public override void snapCursorToCurrentSnappedComponent()
  {
    if (currentlySnappedComponent != null)
    {
      ModOptionsElement? snappedElement = GetVisibleOption(currentlySnappedComponent.myID);
      if (snappedElement != null)
      {
        Point? maybePos = snappedElement.GetRelativeSnapPoint(currentlySnappedComponent.bounds);
        if (maybePos is Point pos) // if it's not null
        {
          Game1.setMousePosition(
            currentlySnappedComponent.bounds.X + pos.X,
            currentlySnappedComponent.bounds.Y + pos.Y
          );
          return;
        }
      }

      if (currentlySnappedComponent.myID < visibleSlots)
      {
        ModEntry.MonitorObject.Log($"{GetType().Name}: Using default snap position for a slot");

        // Positioning taken from OptionsPage.snapCursorToCurrentSnappedComponent
        Game1.setMousePosition(
          currentlySnappedComponent.bounds.Left + 48,
          currentlySnappedComponent.bounds.Center.Y - 12
        );
      }
      else
      {
        base.snapCursorToCurrentSnappedComponent();
      }
    }
  }

  private void SetScrollBarToCurrentItem()
  {
    if (_options.Count > 0)
    {
      _scrollBar.bounds.Y = _scrollBarRunner.Height / Math.Max(1, _options.Count - 7 + 1) * _currentItemIndex +
                            _upArrow.bounds.Bottom +
                            Game1.pixelZoom;

      if (_currentItemIndex == _options.Count - 7)
      {
        _scrollBar.bounds.Y = _downArrow.bounds.Y - _scrollBar.bounds.Height - Game1.pixelZoom;
      }
    }
  }

  public override void leftClickHeld(int x, int y)
  {
    if (!GameMenu.forcePreventClose)
    {
      base.leftClickHeld(x, y);

      if (_isScrolling)
      {
        int yBefore = _scrollBar.bounds.Y;

        _scrollBar.bounds.Y = Math.Min(
          yPositionOnScreen + height - Game1.tileSize - Game1.pixelZoom * 3 - _scrollBar.bounds.Height,
          Math.Max(y, yPositionOnScreen + _upArrow.bounds.Height + Game1.pixelZoom * 5)
        );

        _currentItemIndex = Math.Max(
          0,
          Math.Min(_options.Count - visibleSlots, _options.Count * (y - _scrollBarRunner.Y) / _scrollBarRunner.Height)
        );

        SetScrollBarToCurrentItem();

        if (yBefore != _scrollBar.bounds.Y)
        {
          Game1.playSound("shiny4");
        }
      }
      else if (_optionsSlotHeld > -1 && _optionsSlotHeld + _currentItemIndex < _options.Count)
      {
        _options[_currentItemIndex + _optionsSlotHeld]
          .LeftClickHeld(x - _optionSlots[_optionsSlotHeld].bounds.X, y - _optionSlots[_optionsSlotHeld].bounds.Y);
      }
    }
  }

  public override void receiveKeyPress(Keys key)
  {
    if (_optionsSlotHeld > -1 && _optionsSlotHeld + _currentItemIndex < _options.Count)
    {
      _options[_currentItemIndex + _optionsSlotHeld].ReceiveKeyPress(key);
    }
    else
    {
      // The base implementation handles gamepad movement
      base.receiveKeyPress(key);
    }
  }

  public override void receiveScrollWheelAction(int direction)
  {
    if (!GameMenu.forcePreventClose)
    {
      base.receiveScrollWheelAction(direction);

      if (direction > 0 && _currentItemIndex > 0)
      {
        UpArrowPressed();
        Game1.playSound("shiny4");
      }
      else if (direction < 0 && _currentItemIndex + visibleSlots < _options.Count)
      {
        DownArrowPressed();
        Game1.playSound("shiny4");
      }
    }
  }

  public override void releaseLeftClick(int x, int y)
  {
    if (!GameMenu.forcePreventClose)
    {
      base.releaseLeftClick(x, y);

      if (_optionsSlotHeld > -1 && _optionsSlotHeld + _currentItemIndex < _options.Count)
      {
        ClickableComponent optionSlot = _optionSlots[_optionsSlotHeld];
        _options[_currentItemIndex + _optionsSlotHeld]
          .LeftClickReleased(x - optionSlot.bounds.X, y - optionSlot.bounds.Y);
      }

      _optionsSlotHeld = -1;
      _isScrolling = false;
    }
  }

  private void DownArrowPressed()
  {
    _downArrow.scale = _downArrow.baseScale;
    ++_currentItemIndex;
    SetScrollBarToCurrentItem();
  }

  private void UpArrowPressed()
  {
    _upArrow.scale = _upArrow.baseScale;
    --_currentItemIndex;
    SetScrollBarToCurrentItem();
  }

  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (!GameMenu.forcePreventClose)
    {
      if (_downArrow.containsPoint(x, y) && _currentItemIndex < Math.Max(0, _options.Count - 7))
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
      {
        _isScrolling = true;
      }
      else if (!_downArrow.containsPoint(x, y) &&
               x > xPositionOnScreen + width &&
               x < xPositionOnScreen + width + Game1.tileSize * 2 &&
               y > yPositionOnScreen &&
               y < yPositionOnScreen + height)
      {
        // Handle scrollbar click even if the player clicked right next to it, but do not enable scrollbar dragging
        // NB the leniency area is based on the option page's, so it's too large
        _isScrolling = true;
        base.leftClickHeld(x, y);
        base.releaseLeftClick(x, y);
      }

      _currentItemIndex = Math.Max(0, Math.Min(_options.Count - visibleSlots, _currentItemIndex));
      for (var i = 0; i < _optionSlots.Count; ++i)
      {
        if (_optionSlots[i].bounds.Contains(x, y) &&
            _currentItemIndex + i < _options.Count &&
            _options[_currentItemIndex + i].Bounds.Contains(x - _optionSlots[i].bounds.X, y - _optionSlots[i].bounds.Y))
        {
          _options[_currentItemIndex + i].ReceiveLeftClick(x - _optionSlots[i].bounds.X, y - _optionSlots[i].bounds.Y);
          _optionsSlotHeld = i;
          break;
        }
      }
    }
  }


  public override void receiveRightClick(int x, int y, bool playSound = true) { }

  public override void performHoverAction(int x, int y)
  {
    if (!GameMenu.forcePreventClose)
    {
      _hoverText = "";
      _upArrow.tryHover(x, y);
      _downArrow.tryHover(x, y);
      _scrollBar.tryHover(x, y);
    }
  }

  public override void draw(SpriteBatch batch)
  {
    Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen - 10, width, height, false, true);
    batch.End();
    batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp);
    for (var i = 0; i < _optionSlots.Count; ++i)
    {
      if (_currentItemIndex >= 0 && _currentItemIndex + i < _options.Count)
      {
        _options[_currentItemIndex + i].Draw(batch, _optionSlots[i].bounds.X, _optionSlots[i].bounds.Y);
      }
    }

    batch.End();
    batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
    if (!GameMenu.forcePreventClose)
    {
      _upArrow.draw(batch);
      _downArrow.draw(batch);
      if (_options.Count > 7)
      {
        drawTextureBox(
          batch,
          Game1.mouseCursors,
          new Rectangle(403, 383, 6, 6),
          _scrollBarRunner.X,
          _scrollBarRunner.Y,
          _scrollBarRunner.Width,
          _scrollBarRunner.Height,
          Color.White,
          Game1.pixelZoom,
          false
        );
        _scrollBar.draw(batch);
      }
    }

    if (_hoverText != "")
    {
      drawHoverText(batch, _hoverText, Game1.smallFont);
    }
  }

  /// <summary>Returns the <see cref="ModOptionsElement" /> that corresponds to the component ID</summary>
  /// <returns>the mod options element, or null if it is invalid</returns>
  private ModOptionsElement? GetVisibleOption(int componentId)
  {
    if (componentId >= visibleSlots)
    {
      return null;
    }

    int index = _currentItemIndex + componentId;
    if (0 <= index && index < _options.Count)
    {
      return _options[index];
    }

    return null;
  }

  internal void SaveState(ModOptionsPageState state)
  {
    state.currentIndex = _currentItemIndex;
    state.currentComponent = currentlySnappedComponent?.myID;
  }

  internal void LoadState(ModOptionsPageState state)
  {
    if (state.currentIndex is int index)
    {
      _currentItemIndex = index;
    }

    if (state.currentComponent is int componentID)
    {
      ClickableComponent? component = getComponentWithID(componentID);
      if (component != null)
      {
        currentlySnappedComponent = component;
        snapCursorToCurrentSnappedComponent();
      }
    }
  }
}
