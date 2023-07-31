/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Extensions;
using BetterPlanting.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterPlanting;

/// <summary>
/// State of a tile
/// 
/// <remark>IMPORTANT: keep Plantable as the first tilestate, since TileFiller sorts based on state</remark>
/// </summary>
public enum TileState
{
    Plantable,
    NotEnoughSeeds,
    AlreadyPlanted,
    NonPlantable,
}

internal class FillTile
{
    /// <summary>
    /// Tile location of this <see cref="FillTile"/> in <see cref="Game1.currentLocation"/>
    /// </summary>
    public Vector2 Location { get; set; }

    /// <summary>
    /// <see cref="TileState"/> of this tile. Used to determine overlay color and whether it should be taken into account for planting
    /// </summary>
    public TileState State { get; set; }

    /// <summary>
    /// Priority this tile has. Tiles with higher priority will be chosen over tiles with lower priority if there are not enough seeds for the total shape.
    /// </summary>
    public int Priority { get; set; }

    public FillTile(Vector2 location, int priority = 0)
    {
        this.Location = location;
        this.Priority = priority;
        this.State = DetermineTileState(location);
    }

    private static TileState DetermineTileState(Vector2 location)
    {
        // Doing this so if statements are more readable
        bool canPlant = ModEntry.CanPlantHeldObject(location);
        bool hasObject = Game1.currentLocation.isObjectAtTile((int)location.X, (int)location.Y);
        bool hasGardenPot = ModEntry.IsObjectAtTileGardenPot(location);
        bool hasAliveCrop = ModEntry.TileContainsAliveCrop(location);
        bool isHoldingSeed = Game1.player.IsHoldingCategory(SObject.SeedsCategory);

        if (canPlant && !(hasGardenPot ^ hasObject))
            return TileState.Plantable;

        if (hasAliveCrop && isHoldingSeed)
            return TileState.AlreadyPlanted;

        return TileState.NonPlantable;
    }
    
    public static bool IsPlantable(Vector2 tile)
    {
        return DetermineTileState(tile) == TileState.Plantable;
    }

    public bool IsPlantable()
    {
        return this.State == TileState.Plantable;
    }

    public static bool IsHoeDirt(Vector2 tile)
    {
        return Game1.currentLocation.terrainFeatures.ContainsKey(tile)
            && Game1.currentLocation.terrainFeatures[tile] is HoeDirt
            && !Game1.currentLocation.isObjectAtTile(tile);
    }

    public bool IsHoeDirt()
    {
        return IsHoeDirt(this.Location);
    }

    public static bool IsGardenPot(Vector2 tile)
    {
        return ModEntry.IsObjectAtTileGardenPot(tile);
    }

    public bool IsGardenPot()
    {
        return IsGardenPot(this.Location);
    }

    public Color GetColor()
    {
        // keep in mind the green selection texture is used, meaning that blue/red colors will look kind of murky
        switch (State)
        {
            case TileState.Plantable:
                return Color.White;

            case TileState.AlreadyPlanted:
                return Color.Yellow;

            case TileState.NotEnoughSeeds:
                return Color.Red;

            case TileState.NonPlantable:
                return Color.Transparent;

            default:
                return Color.Transparent;
        }
    }

    public bool UseGreenPlacementTexture()
    {
        switch (State)
        {
            case TileState.Plantable:
                return true;
            default: 
                return false;
        }
    }
}
