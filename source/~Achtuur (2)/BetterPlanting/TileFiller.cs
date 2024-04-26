/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework;
using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace BetterPlanting;

/// <summary>
/// Modes that describe what way the fill should work
/// </summary>
internal enum FillMode
{
    /// <summary>
    /// Single tile, equal to no fill
    /// </summary>
    Disabled,
    /// <summary>
    /// Three tiles horizontally/vertically, depending on the Farmers orientation w.r.t. the cursor
    /// </summary>
    ThreeInARow,
    /// <summary>
    /// Five tiles horizontally/vertically, depending on the Farmers orientation w.r.t. the cursor
    /// </summary>
    FiveInARow,
    /// <summary>
    /// A square with sides of length 3
    /// </summary>
    ThreeSquare,
    /// <summary>
    /// A square with sides of length 5
    /// </summary>
    FiveSquare,
    /// <summary>
    /// A square with sides of length 7
    /// </summary>
    SevenSquare,
    /// <summary>
    /// Every possible adjacent tile to the selected one.
    /// </summary>
    All,
}

internal enum PlantTarget
{
    HoeDirt,
    GardenPot,
}


/// <summary>
/// Todo: change name
/// 
/// Calculates which tiles to fill based on current fill setting.
/// </summary>
internal class TileFiller
{
    /// <summary>
    /// Placement range where tile filler starts doing work. Set to 1.5f (should be sqrt(2) technically) so the 8 squares around the player are counted
    /// </summary>
    internal const float PlacementRange = 1.5f;

    /// <summary>
    /// Amount of fill modes available
    /// </summary>
    internal const int FillModeNumber = 7;
    /// <summary>
    /// Maxmimum number of tiles this filler is allowed to use
    /// </summary>
    internal int TileLimit { get; set; }

    private int fillModePointer;

    internal FillMode FillMode { get; set; }

    // caching
    private Vector2 previousFarmerTile;
    private Vector2 previousCursorTile;
    private IEnumerable<FillTile> fillTiles;


    internal TileFiller()
    {
        this.TileLimit = 500;
        this.fillModePointer = 0;
        this.FillMode = FillMode.Disabled;

        this.previousCursorTile = Vector2.Zero;
        this.previousFarmerTile = Vector2.Zero;
    }

    internal void SetFillMode(FillMode fillMode)
    {
        this.FillMode = fillMode;
        this.fillModePointer = (int)fillMode;
    }

    internal void ResetFillMode()
    {
        this.FillMode = ModEntry.Instance.Config.DefaultFillMode;
        this.fillModePointer = (int)this.FillMode;
    }

    internal void IncrementFillMode(int amount)
    {
        if (!ModEntry.PlayerIsHoldingPlantableObject())
            return;

        this.fillModePointer = (this.fillModePointer + amount) % FillModeNumber;
        if (this.fillModePointer < 0)
            this.fillModePointer += FillModeNumber;

        this.FillMode = (FillMode)this.fillModePointer;

        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"FillMode: {this.FillMode}");
        ModEntry.Instance.UIOverlay.ModeSwitchText = new DecayingText(ModEntry.Instance.TileFiller.GetFillModeAsString(), TilePlaceOverlay.DecayingTextLifeSpan);
    }

    internal string GetFillModeAsString()
    {
        switch (this.FillMode)
        {
            case FillMode.Disabled:
                return I18n.FillModeDisabled();
            case FillMode.ThreeInARow:
                return I18n.FillModeInArow(3);
            case FillMode.FiveInARow:
                return I18n.FillModeInArow(5);
            case FillMode.ThreeSquare:
                return I18n.FillModeSquare(3);
            case FillMode.FiveSquare:
                return I18n.FillModeSquare(5);
            case FillMode.SevenSquare:
                return I18n.FillModeSquare(7);
            case FillMode.All:
                return I18n.FillModeAll();
            default:
                return "";
        }
    }

    internal IEnumerable<FillTile> GetFillTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition)
    {
        // if farmer and cursor not position or fillTiles has not been filled yet, recalculate tiles
        if (this.fillTiles is null || FarmerTilePosition != previousFarmerTile || CursorTilePosition != previousCursorTile)
        {
            this.previousCursorTile = CursorTilePosition;
            this.previousFarmerTile = FarmerTilePosition;
            this.fillTiles = RecalculateFillTiles(FarmerTilePosition, CursorTilePosition);
        }

        foreach (FillTile tile in fillTiles)
            yield return tile;
    }

    private IEnumerable<FillTile> RecalculateFillTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition)
    {
        // Dont return any tiles when out of range
        if ((CursorTilePosition - FarmerTilePosition).LengthSquared() > PlacementRange * PlacementRange)
            yield break;

        IEnumerable<FillTile> fillModeTiles;

        switch (this.FillMode)
        {
            case FillMode.Disabled:
                fillModeTiles = new List<FillTile>();
                break;
            case FillMode.ThreeInARow:
                fillModeTiles = GetRowModeTiles(FarmerTilePosition, CursorTilePosition, 3);
                break;
            case FillMode.FiveInARow:
                fillModeTiles = GetRowModeTiles(FarmerTilePosition, CursorTilePosition, 5);
                break;
            case FillMode.ThreeSquare:
                fillModeTiles = GetSquareModeTiles(FarmerTilePosition, CursorTilePosition, 3);
                break;
            case FillMode.FiveSquare:
                fillModeTiles = GetSquareModeTiles(FarmerTilePosition, CursorTilePosition, 5);
                break;
            case FillMode.SevenSquare:
                fillModeTiles = GetSquareModeTiles(FarmerTilePosition, CursorTilePosition, 7);
                break;
            case FillMode.All:
                fillModeTiles = GetAllModeTiles(FarmerTilePosition, CursorTilePosition);
                break;
            default:
                fillModeTiles = new List<FillTile>();
                break;
        }

        // Sort tiles based on approximity to player, useful when number of seeds < tiles in fill mode
        fillModeTiles = fillModeTiles
            .Where(tile => tile.Location != Game1.currentCursorTile)
            .OrderByDescending(t => t.Priority).ThenBy(t => t.State == TileState.Plantable);


        // Track number of seeds farmer has
        int tile_count = 0;
        int? heldSeeds = null;
        if (ModEntry.PlayerIsHoldingPlantableObject())
        {
            heldSeeds = Game1.player.ActiveObject.Stack;
            if (ModEntry.IsCursorTilePlantable())
                heldSeeds--;
        }

        foreach (FillTile tile in fillModeTiles)
        {
            if (tile.IsPlantable())
                tile_count++;

            // Quit early if more tiles than seeds in hand
            if (heldSeeds != null && tile_count > heldSeeds.Value)
            {
                if (FillMode == FillMode.All)
                    break;
                else
                    tile.State = TileState.NotEnoughSeeds;
            }

            yield return tile;
        }
    }

    /// <summary>
    /// Get <paramref name="nTiles"/> tiles in the direction of <paramref name="FarmerTilePosition"/> - <paramref name="CursorTilePosition"/>
    /// </summary>
    /// <param name="FarmerTilePosition"></param>
    /// <param name="CursorTilePosition"></param>
    /// <returns></returns>
    private IEnumerable<FillTile> GetRowModeTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition, int nTiles)
    {
        // get vector2 in a cardinal direction
        Vector2 dir = GetCardinalDirection(CursorTilePosition - FarmerTilePosition);
        Vector2 startTile = FarmerTilePosition + dir;

        // dir = 0 (cursor is on the farmers position)
        if (dir.LengthSquared() < 1f)
            dir = VectorHelper.GetFaceDirectionUnitVector(Game1.player.FacingDirection);

        for (int t = 0; t < nTiles; t++)
        {
            Vector2 tile = startTile + t * dir;
            yield return new FillTile(tile, priority: nTiles-t);
        }
    }

    /// <summary>
    /// Get rows in a square in direction <paramref name="FarmerTilePosition"/> - <paramref name="CursorTilePosition"/> with sides <paramref name="sideLength"/>
    /// </summary>
    /// <param name="FarmerTilePosition"></param>
    /// <param name="CursorTilePosition"></param>
    /// <param name="sideLength"></param>
    /// <returns></returns>
    private IEnumerable<FillTile> GetSquareModeTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition, int sideLength)
    {
        Vector2 dir = GetCardinalDirection(CursorTilePosition - FarmerTilePosition);
        Vector2 center = FarmerTilePosition + dir * (sideLength / 2 + 1);

        yield return new FillTile(center, priority: 10 * sideLength);
        // iterate outwards in rings from center
        // Start at 3 since s=1 will never return anything
        for (int s = 3; s <= sideLength; s += 2)
        {
            // prioritise tiles closer to center of square
            int base_priority = sideLength - s;

            // iterate over a single side (except last square)
            // Last square is skipped because mirroring 4 sides takes care of that
            // (the first square of the next mirror is the last one of the previous side)
            for (int i = -s / 2; i < s / 2; i++)
            {
                // Top
                Vector2 top = center + new Vector2(i, -s / 2);
                yield return new FillTile(top, priority: 10 * base_priority - i - s);

                // Right
                Vector2 right = center + new Vector2(s / 2, i);
                yield return new FillTile(right, priority: 10 * base_priority - i - 2 * s);

                // Bottom (starts at bottom right)
                Vector2 bot = center + new Vector2(-i, s / 2);
                yield return new FillTile(bot, priority: 10 * base_priority - i - 3 * s);

                // Left (starts at bottom left)
                Vector2 left = center + new Vector2(-s / 2, -i);
                yield return new FillTile(left, priority: 10 * base_priority - i - 4 * s);

            }
        }
    }


    private IEnumerable<FillTile> GetAllModeTiles(Vector2 FarmerTilePosition, Vector2 CursorTilePosition)
    {
        Vector2 dir = GetCardinalDirection(CursorTilePosition - FarmerTilePosition);
        Vector2 startTile = FarmerTilePosition + dir;

        PlantTarget plantTarget = GetPlantTarget(new FillTile(startTile));

        List<Vector2> tileQueue = new() { startTile };
        int backPointer = 0;

        Vector2[] card_dir = new[]
        {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0),
        };

        // Find all adjacent tiles to startTile that are HoeDirt (and thus possible plantable)
        while (backPointer < tileQueue.Count && tileQueue.Count <= ModEntry.Instance.Config.TileFillLimit)
        {
            Vector2 currentTile = tileQueue[backPointer];
            backPointer++;

            foreach (Vector2 d in card_dir)
            {
                Vector2 newTile = currentTile + d;
                if ((ModEntry.CanPlantHeldObject(newTile) || ModEntry.TileContainsAliveCrop(newTile))
                    && !tileQueue.Contains(newTile)
                    && TileIsPlantTarget(newTile, plantTarget))
                {
                    tileQueue.Add(newTile);
                }
            }
        }

        foreach (Vector2 tile in tileQueue)
            yield return new FillTile(tile);
    }


    private Vector2 GetCardinalDirection(Vector2 vec)
    {
        vec = vec.toCardinal();

        // if can place diagonally, or vector is not diagonal, return vec as is
        if (ModEntry.Instance.Config.CanPlaceDiagonally || vec.LengthSquared() <= 1f)
            return vec;

        Vector2 faceDirection = VectorHelper.GetFaceDirectionUnitVector(Game1.player.FacingDirection);

        float LenfaceDirMinVec = (faceDirection - vec).Length();

        // Cursor is either diagonally above/to the side of where player is looking
        if (LenfaceDirMinVec <= 1f)
            return faceDirection;
        // In other case, cursor is diagonally behind player, so invert faceDirection
        else
            return -faceDirection;
    }

    private PlantTarget GetPlantTarget(FillTile first_tile)
    {
        if (first_tile.IsHoeDirt())
            return PlantTarget.HoeDirt;
        else if (first_tile.IsGardenPot())
            return PlantTarget.GardenPot;

        return PlantTarget.HoeDirt;
    }

    private bool TileIsPlantTarget(Vector2 tileLocation, PlantTarget plantTarget)
    {
        switch (plantTarget)
        {
            case PlantTarget.HoeDirt:
                return FillTile.IsHoeDirt(tileLocation);
            case PlantTarget.GardenPot:
                return FillTile.IsGardenPot(tileLocation);
            default:
                return false;
        }
    }
}

