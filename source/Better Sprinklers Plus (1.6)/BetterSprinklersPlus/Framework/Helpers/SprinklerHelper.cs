/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gingajamie/smapi-better-sprinklers-plus-encore
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace BetterSprinklersPlus.Framework.Helpers
{
  /**
   * Helps us get from GameLocations to Sprinkler objects and tiles
   */
  public static class SprinklerHelper
  {
    public static readonly List<int> SprinklerObjectIds = new()
    {
      599,
      621,
      645,
    };

    public static readonly int PressureNozzleId = 915;
    public static readonly Dictionary<int, string> SprinklerTypes = new()
    {
      [599] = "Sprinkler",
      [621] = "Quality Sprinkler",
      [645] = "Iridium Sprinkler",
    };
    
    public static readonly Dictionary<int, int> DefaultTileCountWithoutPressureNozzle = new()
    {
      [599] = 4,
      [621] = 8,
      [645] = 24,
    };
    
    public static readonly Dictionary<int, int> DefaultTileCountWithPressureNozzle = new()
    {
      [599] = 8,
      [621] = 24,
      [645] = 48,
    };

    public static readonly Dictionary<int, int[,]> DefaultGrids = new()
    {
      [599] = new[,]
      {
        { 0, 1, 0 },
        { 1, 0, 1 },
        { 0, 1, 0 },
      },
      [621] = new[,]
      {
        { 1, 1, 1 },
        { 1, 0, 1 },
        { 1, 1, 1 }
      },
      [645] = new[,]
      {
        { 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1 },
        { 1, 1, 0, 1, 1 },
        { 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1 },
      }
    };

    public static readonly Dictionary<int, int[,]> DefaultGridsWithPressureNozzle = new()
    {
      [599] = new[,]
      {
        { 1, 1, 1 },
        { 1, 0, 1 },
        { 1, 1, 1 },
      },
      [621] = new[,]
      {
        { 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1 },
        { 1, 1, 0, 1, 1 },
        { 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1 },
      },
      [645] = new[,]
      {
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 0, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
      }
    };

    public static bool IsSprinkler(this Object obj)
    {
      return SprinklerObjectIds.Contains(obj.ParentSheetIndex);
    }

    private static int DefaultTileCount(this int type, bool hasPressureNozzle = false)
    {
      var count = 0;
      try
      {
        if (hasPressureNozzle)
        {
          DefaultTileCountWithPressureNozzle.TryGetValue(type, out count);
        }
        else
        {
          DefaultTileCountWithoutPressureNozzle.TryGetValue(type, out count);
        }
      }
      catch (Exception e)
      {
        Logger.Error($"Could not get default count for type {type}, returning 0: {e.Message}");
      }

      return count;
    }

    public static IEnumerable<KeyValuePair<Vector2, Object>> AllSprinklers(this GameLocation location)
    {
      return location.objects.Pairs
        .Where(obj => SprinklerObjectIds.Contains(obj.Value.ParentSheetIndex));
    }

    public static int CountCoveredTiles(this int type)
    {
      BetterSprinklersPlusConfig.Active.SprinklerShapes.TryGetValue(type, out var grid);
      if (grid == null) return 0;

      if (BetterSprinklersPlusConfig.Active.DefaultTiles == (int)BetterSprinklersPlusConfig.DefaultTilesOptions.AreFree)
      {
        grid = grid.UnsetDefaultTiles(type);
      }

      var countCoveredTiles = grid.CountCoveredTiles();
      Logger.Verbose($"Count Covered Tiles: {countCoveredTiles}");
      if (BetterSprinklersPlusConfig.Active.DefaultTiles ==
          (int)BetterSprinklersPlusConfig.DefaultTilesOptions.SameNumberAreFree)
      {
        var defaultForType = type.DefaultTileCount();

        countCoveredTiles = countCoveredTiles < defaultForType ? 0 : countCoveredTiles - defaultForType;
        
        Logger.Verbose($"Default count is free, count without default: {countCoveredTiles}");
      }

      return countCoveredTiles;
    }
    
    public static int CountCoveredTiles(this int[,] grid)
    {
      Logger.Verbose($"CountCoveredTiles(int[,] grid)");
      if (grid == null)
      {
        Logger.Warn($"CountCoveredTiles: Grid was null, returning 0");
        return 0;
      }

      var count = grid.Cast<int>().Count(cell => cell > 0);

      Logger.Verbose($"Count of covered tiles: {count}");
      return count;
    }

    public class SprinklerTile
    {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsCovered { get; set; }

      public SprinklerTile(int x, int y, bool isCovered)
      {
        X = x;
        Y = y;
        IsCovered = isCovered;
      }

      public Vector2 ToVector2()
      {
        return new Vector2(X, Y);
      }
    }

    public static void ForAllTiles(this Object sprinkler, Vector2 tile, Action<SprinklerTile> perform)
    {
      Logger.Verbose($"ForAllTiles(sprinkler, {tile.X}x{tile.Y}, perform)");
      BetterSprinklersPlusConfig.Active.SprinklerShapes.TryGetValue(sprinkler.ParentSheetIndex, out var grid);
      foreach (var coveredTile in GridHelper.GetAllTiles(tile , grid))
      {
        perform(coveredTile);
      }
    }

    public static void ForDefaultTiles(this Object sprinkler, Vector2 tile, Action<SprinklerTile> perform)
    {
      Logger.Verbose($"ForDefaultTiles(sprinkler, {tile.X}x{tile.Y}, perform)");
      var hasPressureNozzle = sprinkler.HasPressureNozzle();
      int[,] grid;
      if (hasPressureNozzle)
      {
        DefaultGridsWithPressureNozzle.TryGetValue(sprinkler.ParentSheetIndex, out grid);
      }
      else
      {
        DefaultGrids.TryGetValue(sprinkler.ParentSheetIndex, out grid);
      }

      foreach (var coveredTile in GridHelper.GetAllTiles(tile , grid))
      {
        perform(coveredTile);
      }
    }

    public static bool IsDirt(this TerrainFeature terrainFeature)
    {
      return terrainFeature is HoeDirt;
    }

    public static bool HasPressureNozzle(this Object sprinkler)
    {
#pragma warning disable AvoidImplicitNetFieldCast
      if (sprinkler == null || sprinkler.heldObject == null) return false;
#pragma warning restore AvoidImplicitNetFieldCast

#pragma warning disable AvoidImplicitNetFieldCast
      var sprinklerHeldObj = sprinkler.heldObject.Get();
      return sprinklerHeldObj != null && sprinklerHeldObj.QualifiedItemId == "(O)915";
#pragma warning restore AvoidImplicitNetFieldCast
    }

    public static int[,] GetGrid(this int type)
    {
      BetterSprinklersPlusConfig.Active.SprinklerShapes.TryGetValue(type, out var grid);
      return grid;
    }

    public static float CalculateCostForSprinkler(this int type, bool hasPressureNozzle = false)
    {
      var grid = type.GetGrid();
      return grid.CalculateCostForSprinkler(type, hasPressureNozzle);
    }


    public static float CalculateCostForSprinkler(this int[,] grid, int type, bool hasPressureNozzle = false)
    {
      if (BetterSprinklersPlusConfig.Active.DefaultTiles == (int)BetterSprinklersPlusConfig.DefaultTilesOptions.AreFree)
      {
        Logger.Verbose("Defaults are free, removing tiles in default position");
        // grid.VerboseLog();
        grid = grid.UnsetDefaultTiles(type, hasPressureNozzle);
        // grid.VerboseLog();
      }

      Logger.Verbose($"CalculateCostForSprinkler(int[,] grid, {SprinklerTypes[type]})");
      var count = grid.CountCoveredTiles();
      
      Logger.Verbose($"Count of covered tiles: {count}");

      if (BetterSprinklersPlusConfig.Active.DefaultTiles ==
          (int)BetterSprinklersPlusConfig.DefaultTilesOptions.SameNumberAreFree)
      {
        var defaultForType = type.DefaultTileCount();

        count = count < defaultForType ? 0 : count - defaultForType;
        
        
        Logger.Verbose($"Default count is free, Count of covered tiles without default: {count}");
      }

      var costPerTile = type.GetCostPerTile(hasPressureNozzle);
      Logger.Verbose($"Cost Per Tile: {costPerTile}G");

      var costForSprinkler = count * costPerTile;
      Logger.Verbose($"Cost for sprinkler: {costForSprinkler}G");
      return costForSprinkler;
    }

    private static int[,] UnsetDefaultTiles(this int[,] grid, int type, bool hasPressureNozzle = false)
    {
      Logger.Verbose($"UnsetDefaultTiles({type}, {hasPressureNozzle})");
      if (hasPressureNozzle)
      {
        return grid.UnsetDefaultTilesWithPressureNozzle(type);
      }

      return grid.UnsetDefaultTilesWithoutPressureNozzle(type);
    }

    private static void VerboseLog(this int[,] grid)
    {
      var cols = grid.GetLength(0);
      var rows = grid.GetLength(1);
      for (var row = 0; row < rows; row++)
      {
        var r = "";
        for (var col = 0; col < cols; col++)
        {
          r += $"{grid[col, row]}";
        }
        Logger.Verbose($"{r}");
      }
    }

    private static int[,] UnsetDefaultTilesWithPressureNozzle(this int[,] grid, int type)
    {
      var newGrid = (int[,])grid.Clone();
      var centerTile = newGrid.GetLength(0) / 2;
      
      switch (type)
      {
        case 599:
          for (var x = centerTile - 1; x < centerTile + 2; x++)
          {
            for (var y = centerTile - 1; y < centerTile + 2; y++)
            { 
              newGrid[x, y] = 0;
            }
          }
          
          break;
        case 621:
          for (var x = centerTile - 2; x < centerTile + 3; x++)
          {
            for (var y = centerTile - 2; y < centerTile + 3; y++)
            { 
              newGrid[x, y] = 0;
            }
          }

          break;
        case 645:
          for (var x = centerTile - 3; x < centerTile + 4; x++)
          {
            for (var y = centerTile - 3; y < centerTile + 4; y++)
            { 
              newGrid[x, y] = 0;
            }
          }
          break;
      }

      return newGrid;
    }

    private static int[,] UnsetDefaultTilesWithoutPressureNozzle(this int[,] grid, int type)
    {
      var newGrid = (int[,])grid.Clone();
      var centerTile = newGrid.GetLength(0) / 2;
      
      switch (type)
      {
        case 599:
          newGrid[centerTile, centerTile] = 0;
          newGrid[centerTile - 1, centerTile] = 0;
          newGrid[centerTile + 1, centerTile] = 0;
          newGrid[centerTile, centerTile - 1] = 0;
          newGrid[centerTile, centerTile + 1] = 0;
          break;
        case 621:
          for (var x = centerTile - 1; x < centerTile + 2; x++)
          {
            for (var y = centerTile - 1; y < centerTile + 2; y++)
            { 
              newGrid[x, y] = 0;
            }
          }

          break;
        case 645:
          for (var x = centerTile - 2; x < centerTile + 3; x++)
          {
            for (var y = centerTile - 2; y < centerTile + 3; y++)
            { 
              newGrid[x, y] = 0;
            }
          }
          break;
      }

      return newGrid;
    }

    public static float GetCostPerTile(this int type, bool hasPressureNozzle = false)
    {
      Logger.Verbose($"GetCostPerTile({type}, {hasPressureNozzle})");
      var baseCost = GetCostPerTile();
      float multiplier;

      try
      {
        multiplier = BetterSprinklersPlusConfig.Active.CostMultiplier[type];
      }
      catch (Exception)
      {
        multiplier = 1;
      }

      var costAfterMultiplier = baseCost * multiplier;
      Logger.Verbose($"cost after type multiplier: {multiplier}");

      if (hasPressureNozzle)
      {
        var pressureNozzleMultiplier = BetterSprinklersPlusConfig.Active.PressureNozzleMultiplier;
        Logger.Verbose($"cost after has pressure nozzle (true): {costAfterMultiplier * pressureNozzleMultiplier}");
        return costAfterMultiplier * pressureNozzleMultiplier;
      }

      Logger.Verbose($"cost after has pressure nozzle (false): {costAfterMultiplier}");
      return costAfterMultiplier;
    }

    /// <summary>
    /// Gets the cost per tile in .Gs
    /// </summary>
    /// <returns>The cost of watering one tile (as a fraction of a G)</returns>
    public static float GetCostPerTile()
    {
      try
      {
        var costPerTile = BetterSprinklersPlusConfig.BalancedModeOptionsMultipliers[BetterSprinklersPlusConfig.Active.BalancedMode];
        Logger.Verbose($"GetCostPerTile(): {costPerTile}G");
        return costPerTile;
      }
      catch (Exception e)
      {
        Logger.Error($"GetCostPerTile(): {e.Message}, returning 0G");
        return 0f;
      }
    }
  }
}
