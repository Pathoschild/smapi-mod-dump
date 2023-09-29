/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamescodesthings/smapi-better-sprinklers
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using BetterSprinklersPlus.Framework.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace BetterSprinklersPlus.Framework
{
  internal enum TileState
  {
    Off = 0,
    On = 1,
    DefaultTile = 2
  }

  internal class CoverageEditMenu : IClickableMenu
  {
    private readonly IModHelper _helper;
    private readonly Texture2D _whitePixel;
    private readonly List<ClickableComponent> _tabs;
    private readonly ClickableTextureComponent _okButton;
    private readonly ClickableTextureComponent _resetButton;

    private const int DefaultTileSize = 32;
    private const int Padding = 2;
    private const int MinLeftMargin = 32;
    private const int MinTopMargin = 32;
    private const int TabDistanceFromMenu = -32;
    private const int TabItemWidth = 64;
    private const int TabItemHeight = 64;
    private const int TabDistanceVerticalBetweenTabs = 16;
    private const int TabLeftMargin = 16;
    private const int TabVerticalMargins = 16;
    private const int TabRightMargin = 32;
    private const int TextSize = 64;

    private int _arraySize;
    private int _centerTile;
    private int _tileSize;

    /**
     * Hovered Item x in tiles
     */
    private int _hoveredItemX;
    /**
     * Hovered Item y in tiles
     */
    private int _hoveredItemY;

    private readonly Color[] _colors = { Color.Tomato, Color.ForestGreen };

    private int _leftMargin;
    private int _topMargin;

    private int[,] _sprinklerGrid;

    private int _activeSprinklerSheet;

    private TileState? _toggling;

    private int _cost;
    private int _countCovered;
    private float _costPerTile;


    /// <summary>Constructor</summary>
    public CoverageEditMenu(IModHelper helper)
    {
      Logger.Verbose($"new CoverageEditMenu()");
      _helper = helper;

      var menuWidth = BetterSprinklersPlusConfig.Active.MaxGridSize * DefaultTileSize + MinLeftMargin * 2;
      var menuHeight = BetterSprinklersPlusConfig.Active.MaxGridSize * DefaultTileSize + MinTopMargin * 2 + TextSize;

      var menuX = Game1.uiViewport.Width / 2 - menuWidth / 2;
      var menuY = Game1.uiViewport.Height / 2 - menuHeight / 2;
        
      Logger.Verbose($"CoverageEditMenu.initialize()");
      initialize(menuX, menuY, menuWidth, menuHeight, true);

      _tabs = new List<ClickableComponent>();
      const int tabWidth = TabItemWidth + TabLeftMargin + TabRightMargin;
      const int tabHeight = TabItemHeight + TabVerticalMargins * 2;
      _tabs.Add(new ClickableComponent(
        new Rectangle(menuX - TabDistanceFromMenu - tabWidth,
          menuY + TabDistanceVerticalBetweenTabs, tabWidth, tabHeight),
        new Object(Vector2.Zero, 599)));
      _tabs.Add(new ClickableComponent(
        new Rectangle(menuX - TabDistanceFromMenu - tabWidth,
          _tabs[0].bounds.Y + tabHeight + TabDistanceVerticalBetweenTabs, tabWidth, tabHeight),
        new Object(Vector2.Zero, 621)));
      _tabs.Add(new ClickableComponent(
        new Rectangle(menuX - TabDistanceFromMenu - tabWidth,
          _tabs[1].bounds.Y + tabHeight + TabDistanceVerticalBetweenTabs, tabWidth, tabHeight),
        new Object(Vector2.Zero, 645)));

      _okButton = new ClickableTextureComponent("save-changes",
        new Rectangle(xPositionOnScreen + width - Game1.tileSize / 2,
          yPositionOnScreen + height - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize), "",
        "Save Changes", Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f);
      
      Logger.Verbose($"Creating Reset Button");
      _resetButton = new ClickableTextureComponent("reset-changes",
        new Rectangle(
          xPositionOnScreen + width - (Game1.tileSize / 2) * 4,
          yPositionOnScreen + height - Game1.tileSize / 2, 
          Game1.tileSize, 
          Game1.tileSize
        ), 
        "",
        "Reset Changes", 
        Game1.mouseCursors, 
        new Rectangle(192, 256, 64, 64), 
        1f);

      _whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
      _whitePixel.SetData(new[] { Color.White });

      Logger.Verbose($"Setting Active Sprinkler Sheet Index");
      SetActiveSprinklerSheetIndex(599);
    }

    private Color GetColor(int value)
    {
      try
      {
        return _colors[value];
      }
      catch (Exception e)
      {
        Logger.Error($"Error getting color {value}: {e.Message}");
        return _colors[0];
      }
    }

    public override void draw(SpriteBatch b)
    {
      foreach (var tab in _tabs)
      {
        drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), tab.bounds.X,
          tab.bounds.Y, tab.bounds.Width, tab.bounds.Height, Color.White);
        Game1.spriteBatch.Draw(Game1.objectSpriteSheet,
          new Rectangle(tab.bounds.X + TabLeftMargin, tab.bounds.Y + TabVerticalMargins, TabItemWidth,
            TabItemHeight),
          Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tab.item.ParentSheetIndex, 16, 16),
          Color.White);
      }

      drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
        xPositionOnScreen, yPositionOnScreen, width, height, Color.White);


      int x;
      int y;
        
      if (_hoveredItemX > -1 && _hoveredItemY > -1)
      {
        // If not Center Tile, draw hover effect
        if (_hoveredItemX != _centerTile || _hoveredItemY != _centerTile) {
          x = xPositionOnScreen + _leftMargin + _hoveredItemX * _tileSize;
          y = yPositionOnScreen + _topMargin + _hoveredItemY * _tileSize;
        
          Game1.spriteBatch.Draw(_whitePixel, new Rectangle(x, y, _tileSize, _tileSize), Color.AntiqueWhite);
        }
      }

      for (var countX = 0; countX < _arraySize; countX++)
      {
        for (var countY = 0; countY < _arraySize; countY++)
        {
          // Don't draw center tile
          if (countX == _centerTile && countY == _centerTile)
          {
            continue;
          }

          x = xPositionOnScreen + _leftMargin + Padding + countX * _tileSize;
          y = yPositionOnScreen + _topMargin + Padding + countY * _tileSize;
            
          Game1.spriteBatch.Draw(_whitePixel,
            new Rectangle(
              x,
              y, 
              _tileSize - Padding * 2,
              _tileSize - Padding * 2
            ),
            GetColor(_sprinklerGrid[countX, countY]));
        }
      }

      x = xPositionOnScreen + _leftMargin + Padding + _centerTile * _tileSize;
      y = yPositionOnScreen + _topMargin + Padding + _centerTile * _tileSize;
      
      // Draw the center sprinkler?
      Game1.spriteBatch.Draw(Game1.objectSpriteSheet,
        new Rectangle(x, y, _tileSize - Padding * 2, _tileSize - Padding * 2),
        Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, _activeSprinklerSheet, 16, 16),
        Color.White);
      
      // Draw the ok button
      _okButton.draw(Game1.spriteBatch);
      _resetButton.draw(Game1.spriteBatch);

      // Draw Cost
      var font = Game1.smallFont;
      var defaultMessage = BetterSprinklersPlusConfig.Active.DefaultTiles != (int)BetterSprinklersPlusConfig.DefaultTilesOptions.CostMoney ? "extra " : "";
      Utility.drawTextWithShadow(b, $"{_cost}G per day ({_countCovered} {defaultMessage}tiles x {_costPerTile}G per day)", font,
        new Vector2(xPositionOnScreen + 18, yPositionOnScreen + height - TextSize), Game1.textColor);

      Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
        4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);

      base.draw(b);
    }

    public override void update(GameTime time)
    {
      base.update(time);

      var mouseGridRelX = Game1.getOldMouseX() - xPositionOnScreen - _leftMargin - Padding;
      var mouseGridRelY = Game1.getOldMouseY() - yPositionOnScreen - _topMargin - Padding;

      if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < _arraySize * _tileSize - Padding &&
          mouseGridRelY < _arraySize * _tileSize - Padding)
      {
        _hoveredItemX = mouseGridRelX / _tileSize;
        _hoveredItemY = mouseGridRelY / _tileSize;
      }
      else
      {
        _hoveredItemX = -1;
        _hoveredItemY = -1;
      }

      // here, if mouse is down and hoveredItem is a tile, toggle the tile.
      var isLeftMousePressed = _helper.Input.IsDown(SButton.MouseLeft);

      if (!isLeftMousePressed)
      {
        ResetToggle();
      }

      if (isLeftMousePressed && _hoveredItemX != -1 && _hoveredItemY != -1 && (_hoveredItemX != _centerTile || _hoveredItemY != _centerTile))
      {
        Logger.Verbose($"Left mouse is pressed over hovered item");
        Toggle();
      }


      _okButton.tryHover(Game1.getOldMouseX(), Game1.getOldMouseY());
      _resetButton.tryHover(Game1.getOldMouseX(), Game1.getOldMouseY());
    }

    private void Toggle()
    {
      if (_sprinklerGrid[_hoveredItemX, _hoveredItemY] == (int)TileState.DefaultTile) return;

      if (_toggling == null)
      {
        var current = (TileState)_sprinklerGrid[_hoveredItemX, _hoveredItemY];
        var updated = current == TileState.Off ? TileState.On : TileState.Off;
        _toggling = updated;
      }

      _sprinklerGrid[_hoveredItemX, _hoveredItemY] = (int)_toggling;
      RecalculateCost();
    }


    private void ResetToggle()
    {
      _toggling = null;
    }


    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);

      foreach (var tab in _tabs.Where(tab => tab.containsPoint(x, y)))
      {
        Game1.playSound("select");
        SetActiveSprinklerSheetIndex(tab.item.ParentSheetIndex);
      }
      
      if (_resetButton.containsPoint(x, y)) Reset();

      if (_okButton.containsPoint(x, y)) Save();
    }

    private void SetActiveSprinklerSheetIndex(int type)
    {
      Logger.Verbose($"SetActiveSprinklerSheetIndex({SprinklerHelper.SprinklerTypes[type]})");
      _activeSprinklerSheet = type;
      _sprinklerGrid = BetterSprinklersPlusConfig.Active.SprinklerShapes[type];

      _hoveredItemX = -1;
      _hoveredItemY = -1;
        
      _arraySize = BetterSprinklersPlusConfig.Active.Range[type];
      _centerTile = _arraySize / 2;
      _tileSize = _arraySize <= 7 ? 64 : DefaultTileSize;

      Logger.Verbose($"Type: {type} ArraySize: {_arraySize}, CenterTile: {_centerTile}, TileSize {_tileSize}");

      _leftMargin = (width - (_arraySize * _tileSize)) / 2;
      _topMargin = (height - (_arraySize * _tileSize)) / 2;

      RecalculateCost();
    }

    private void Save()
    {
      Game1.playSound("select");
      BetterSprinklersPlusConfig.SaveChanges();
    }

    /**
     * Resets the shape on the active tab
     */
    private void Reset()
    {
      var type = _activeSprinklerSheet;

      // Zero all tiles
      for (var r = 0; r < _arraySize; r++)
      {
        for (var c = 0; c < _arraySize; c++)
        {
          _sprinklerGrid[r, c] = 0;
        }
      }
        
      // Set defaults
      switch (type)
      {
        case 599:
          _sprinklerGrid[_centerTile, _centerTile] = 0;
          _sprinklerGrid[_centerTile - 1, _centerTile] = 1;
          _sprinklerGrid[_centerTile + 1, _centerTile] = 1;
          _sprinklerGrid[_centerTile, _centerTile - 1] = 1;
          _sprinklerGrid[_centerTile, _centerTile + 1] = 1;
          break;
        case 621:
          for (var x = _centerTile - 1; x < _centerTile + 2; x++)
          {
            for (var y = _centerTile - 1; y < _centerTile + 2; y++)
            {
              // Do not draw center tile
              if (x == _centerTile && y == _centerTile) continue;
              
              _sprinklerGrid[x, y] = 1;
            }
          }

          break;
        case 645:
          for (var x = _centerTile - 2; x < _centerTile + 3; x++)
          {
            for (var y = _centerTile - 2; y < _centerTile + 3; y++)
            {
              // Do not draw center tile
              if (x == _centerTile && y == _centerTile) continue;
              
              _sprinklerGrid[x, y] = 1;
            }
          }

          break;
      }
    }

    private void RecalculateCost()
    {
      Logger.Verbose($"RecalculateCost()");
      _costPerTile = _activeSprinklerSheet.GetCostPerTile();
      Logger.Verbose($"RecalculateCost(): costPerTile: {_costPerTile}");
      _countCovered = _activeSprinklerSheet.CountCoveredTiles();
      _cost = (int)Math.Round(_sprinklerGrid.CalculateCostForSprinkler(_activeSprinklerSheet));
      Logger.Verbose($"RecalculateCost(): _cost: {_cost}");
      
      Logger.Verbose($"Type {this._activeSprinklerSheet} will cost {_cost}G per sprinkler, per day.");
    }
  }
}