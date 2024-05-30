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
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using Object = StardewValley.Object;

namespace UIInfoSuite2.UIElements;

internal class ShowItemEffectRanges : IDisposable
{
#region Properties
  private readonly PerScreen<HashSet<Point>> _effectiveArea = new(() => new HashSet<Point>());

  private readonly Mutex _mutex = new();

  private readonly IModHelper _helper;
#endregion


#region Lifecycle
  public ShowItemEffectRanges(IModHelper helper)
  {
    _helper = helper;
  }

  public void Dispose()
  {
    ToggleOption(false);
  }

  public void ToggleOption(bool showItemEffectRanges)
  {
    _helper.Events.Display.RenderingHud -= OnRenderingHud;
    _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;

    if (showItemEffectRanges)
    {
      _helper.Events.Display.RenderingHud += OnRenderingHud;
      _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }
  }
#endregion


#region Event subscriptions
  private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
  {
    if (!e.IsMultipleOf(4))
    {
      return;
    }

    // Ticks can happen when the player reverts to the loading screen; defend against that.
    if (Game1.currentLocation is null)
    {
      return;
    }

    if (_mutex.WaitOne())
    {
      try
      {
        _effectiveArea.Value.Clear();
      }
      finally
      {
        _mutex.ReleaseMutex();
      }
    }

    if (Game1.activeClickableMenu == null && UIElementUtils.IsRenderingNormally())
    {
      UpdateEffectiveArea();
    }
  }


  private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
  {
    if (_mutex.WaitOne(0))
    {
      try
      {
        foreach (Point point in _effectiveArea.Value)
        {
          var position = new Vector2(
            point.X * Utility.ModifyCoordinateFromUIScale(Game1.tileSize),
            point.Y * Utility.ModifyCoordinateFromUIScale(Game1.tileSize)
          );
          e.SpriteBatch.Draw(
            Game1.mouseCursors,
            Utility.ModifyCoordinatesForUIScale(Game1.GlobalToLocal(Utility.ModifyCoordinatesForUIScale(position))),
            new Rectangle(194, 388, 16, 16),
            Color.White * 0.7f,
            0.0f,
            Vector2.Zero,
            Utility.ModifyCoordinateForUIScale(Game1.pixelZoom),
            SpriteEffects.None,
            0.01f
          );
        }
      }
      finally
      {
        _mutex.ReleaseMutex();
      }
    }
  }
#endregion


#region Logic
  private void UpdateEffectiveArea()
  {
    int[][] arrayToUse;
    List<Object> similarObjects;

    // Junimo Hut is handled differently, because it is a building
    Building building = Game1.currentLocation.getBuildingAt(Game1.GetPlacementGrabTile());

    if (building is JunimoHut)
    {
      arrayToUse = GetDistanceArray(ObjectsWithDistance.JunimoHut);
      foreach (Building? nextBuilding in Game1.currentLocation.buildings)
      {
        if (nextBuilding is JunimoHut nextHut)
        {
          AddTilesToHighlightedArea(arrayToUse, nextHut.tileX.Value + 1, nextHut.tileY.Value + 1);
        }
      }
    }

    // Every other item is here
    if (Game1.player.CurrentItem is Object currentItem && currentItem.isPlaceable())
    {
      string itemName = currentItem.Name;

      Vector2 currentTile = Game1.GetPlacementGrabTile();
      Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
      Vector2 validTile = Utility.snapToInt(
                            Utility.GetNearbyValidPlacementPosition(
                              Game1.player,
                              Game1.currentLocation,
                              currentItem,
                              (int)currentTile.X * Game1.tileSize,
                              (int)currentTile.Y * Game1.tileSize
                            )
                          ) /
                          Game1.tileSize;
      Game1.isCheckingNonMousePlacement = false;

      if (itemName.IndexOf("arecrow", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        arrayToUse = itemName.Contains("eluxe")
          ? GetDistanceArray(ObjectsWithDistance.DeluxeScarecrow, false, currentItem)
          : GetDistanceArray(ObjectsWithDistance.Scarecrow, false, currentItem);
        AddTilesToHighlightedArea(arrayToUse, (int)validTile.X, (int)validTile.Y);

        similarObjects = GetSimilarObjectsInLocation("arecrow");
        foreach (Object next in similarObjects)
        {
          arrayToUse = next.Name.IndexOf("eluxe", StringComparison.OrdinalIgnoreCase) >= 0
            ? GetDistanceArray(ObjectsWithDistance.DeluxeScarecrow, false, next)
            : GetDistanceArray(ObjectsWithDistance.Scarecrow, false, next);
          AddTilesToHighlightedArea(arrayToUse, (int)next.TileLocation.X, (int)next.TileLocation.Y);
        }
      }
      else if (itemName.IndexOf("sprinkler", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        // Relative tile positions to the placable items locations - need to pass coordinates

        /*
         * @NermNermNerm:
         * This change is a little bit worrisome because Object.GetSprinklerTiles didn't semantically change in 1.6...
         * But it did change. Somebody noodled over it and changed a variable name.
         * However, its behavior got changed by something else -
         * in the past it was zero-based - as Object.tileLocation used to be 0.0 for unplaced objects,
         * and now it's the tile that's being hovered over.
         * That new behavior might not be intended and might get rolled back.
         */

        // Move tiles to 0, 0 and then offset by the correct tile.
        IEnumerable<Vector2> unplacedSprinklerTiles = currentItem.GetSprinklerTiles();
        if (currentItem.TileLocation != validTile)
        {
          unplacedSprinklerTiles = unplacedSprinklerTiles.Select(tile => tile - currentItem.TileLocation + validTile);
        }

        AddTilesToHighlightedArea(unplacedSprinklerTiles);

        similarObjects = GetSimilarObjectsInLocation("sprinkler");
        foreach (Object next in similarObjects)
        {
          // Absolute tile positions
          AddTilesToHighlightedArea(next.GetSprinklerTiles());
        }
      }
      else if (itemName.IndexOf("bee house", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        arrayToUse = GetDistanceArray(ObjectsWithDistance.Beehouse);
        AddTilesToHighlightedArea(arrayToUse, (int)validTile.X, (int)validTile.Y);
      }
      else if (itemName.IndexOf("mushroom log", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        arrayToUse = GetDistanceArray(ObjectsWithDistance.MushroomLog);
        AddTilesToHighlightedArea(arrayToUse, (int)validTile.X, (int)validTile.Y);
      }
      else if (itemName.IndexOf("mossy seed", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        arrayToUse = GetDistanceArray(ObjectsWithDistance.MossySeed);
        AddTilesToHighlightedArea(arrayToUse, (int)validTile.X, (int)validTile.Y);
      }
    }
  }

  private void AddTilesToHighlightedArea(IEnumerable<Vector2> tiles, int xPos = 0, int yPos = 0)
  {
    if (_mutex.WaitOne())
    {
      try
      {
        foreach (Vector2 tile in tiles)
        {
          var point = tile.ToPoint();
          point.X += xPos;
          point.Y += yPos;
          _effectiveArea.Value.Add(point);
        }
      }
      finally
      {
        _mutex.ReleaseMutex();
      }
    }
  }

  private void AddTilesToHighlightedArea(int[][] tileMap, int xPos = 0, int yPos = 0)
  {
    int xOffset = tileMap.Length / 2;

    if (_mutex.WaitOne())
    {
      try
      {
        for (var i = 0; i < tileMap.Length; ++i)
        {
          int yOffset = tileMap[i].Length / 2;
          for (var j = 0; j < tileMap[i].Length; ++j)
          {
            if (tileMap[i][j] == 1)
            {
              _effectiveArea.Value.Add(new Point(xPos + i - xOffset, yPos + j - yOffset));
            }
          }
        }
      }
      finally
      {
        _mutex.ReleaseMutex();
      }
    }
  }

  private List<Object> GetSimilarObjectsInLocation(string nameContains)
  {
    var result = new List<Object>();

    if (!string.IsNullOrEmpty(nameContains))
    {
      nameContains = nameContains.ToLower();
      OverlaidDictionary? objects = Game1.currentLocation.Objects;

      foreach (Object? nextThing in objects.Values)
      {
        if (nextThing.name.ToLower().Contains(nameContains))
        {
          result.Add(nextThing);
        }
      }
    }

    return result;
  }

#region Distance map
  private enum ObjectsWithDistance
  {
    JunimoHut,
    Beehouse,
    Scarecrow,
    DeluxeScarecrow,
    Sprinkler,
    QualitySprinkler,
    IridiumSprinkler,
    PrismaticSprinkler,
    MushroomLog,
    MossySeed
  }

  private int[][] GetDistanceArray(ObjectsWithDistance type, bool hasPressureNozzle = false, Object? instance = null)
  {
    switch (type)
    {
      case ObjectsWithDistance.JunimoHut:
        return GetCircularMask(100, maxDisplaySquareRadius: 8);
      case ObjectsWithDistance.Beehouse:
        return GetCircularMask(4.19, 5, true);
      case ObjectsWithDistance.Scarecrow:
        return GetCircularMask((instance?.GetRadiusForScarecrow() ?? 9) - 0.01);
      case ObjectsWithDistance.DeluxeScarecrow:
        return GetCircularMask((instance?.GetRadiusForScarecrow() ?? 17) - 0.01);
      case ObjectsWithDistance.Sprinkler:
        return hasPressureNozzle ? GetCircularMask(100, maxDisplaySquareRadius: 1) : GetCircularMask(1);
      case ObjectsWithDistance.QualitySprinkler:
        return hasPressureNozzle
          ? GetCircularMask(100, maxDisplaySquareRadius: 2)
          : GetCircularMask(100, maxDisplaySquareRadius: 1);
      case ObjectsWithDistance.IridiumSprinkler:
        return hasPressureNozzle
          ? GetCircularMask(100, maxDisplaySquareRadius: 3)
          : GetCircularMask(100, maxDisplaySquareRadius: 2);
      case ObjectsWithDistance.PrismaticSprinkler:
        return GetCircularMask(3.69, Math.Sqrt(18), false);
      case ObjectsWithDistance.MushroomLog:
        return GetCircularMask(100, maxDisplaySquareRadius: 7);
      case ObjectsWithDistance.MossySeed:
        return GetCircularMask(100, maxDisplaySquareRadius: 5);
      default:
        return null;
    }
  }

  private static int[][] GetCircularMask(
    double maxDistance,
    double? exceptionalDistance = null,
    bool? onlyClearExceptions = null,
    int? maxDisplaySquareRadius = null
  )
  {
    int radius = Math.Max(
      (int)Math.Ceiling(maxDistance),
      exceptionalDistance.HasValue ? (int)Math.Ceiling(exceptionalDistance.Value) : 0
    );
    radius = Math.Min(radius, maxDisplaySquareRadius.HasValue ? maxDisplaySquareRadius.Value : radius);
    int size = 2 * radius + 1;

    var result = new int[size][];
    for (var i = 0; i < size; i++)
    {
      result[i] = new int[size];
      for (var j = 0; j < size; j++)
      {
        double distance = GetDistance(i, j, radius);
        int val = IsInDistance(maxDistance, distance) ||
                  (IsDistanceDirectionOK(i, j, radius, onlyClearExceptions) &&
                   IsExceptionalDistanceOK(exceptionalDistance, distance))
          ? 1
          : 0;
        result[i][j] = val;
      }
    }

    return result;
  }

  private static bool IsDistanceDirectionOK(int i, int j, int radius, bool? onlyClearExceptions)
  {
    return onlyClearExceptions.HasValue && onlyClearExceptions.Value ? radius - j == 0 || radius - i == 0 : true;
  }

  private static bool IsExceptionalDistanceOK(double? exceptionalDistance, double distance)
  {
    return exceptionalDistance.HasValue && exceptionalDistance.Value == distance;
  }

  private static bool IsInDistance(double maxDistance, double distance)
  {
    return distance <= maxDistance;
  }

  private static double GetDistance(int i, int j, int radius)
  {
    return Math.Sqrt((radius - i) * (radius - i) + (radius - j) * (radius - j));
  }
#endregion
#endregion
}
