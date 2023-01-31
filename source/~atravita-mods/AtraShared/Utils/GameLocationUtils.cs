/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Buffers;
using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace AtraShared.Utils;

// TODO: Remove checks for BuildableGameLocation in 1.6.

/// <summary>
/// Utility for gamelocations.
/// </summary>
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Reviewed")]
public static class GameLocationUtils
{
    #region Animations

    /// <summary>
    /// The code in this function is effectively copied from the game, and explodes a bomb on this tile.
    /// </summary>
    /// <param name="loc">Location to explode bomb.</param>
    /// <param name="whichBomb">Which bomb to explode.</param>
    /// <param name="tileloc">Tile to explode bomb on.</param>
    /// <param name="mp">Multiplayer instance - used to broadcast sprites.</param>
    public static void ExplodeBomb(GameLocation loc, int whichBomb, Vector2 tileloc, Multiplayer mp)
    {
        int bombID = Game1.random.Next();
        loc.playSound("thudStep");
        TemporaryAnimatedSprite tas_bomb = new(
            initialParentTileIndex: whichBomb,
            animationInterval: 100f,
            animationLength: 1,
            numberOfLoops: 24,
            position: tileloc,
            flicker: true,
            flipped: false,
            parent: loc,
            owner: Game1.player)
        {
            shakeIntensity = 0.5f,
            shakeIntensityChange = 0.002f,
            extraInfoForEndBehavior = bombID,
            endFunction = loc.removeTemporarySpritesWithID,
        };
        mp.broadcastSprites(loc, tas_bomb);
        TemporaryAnimatedSprite tas_yellow = new(
            textureName: Game1.mouseCursorsName,
            sourceRect: new Rectangle(598, 1279, 3, 4),
            animationInterval: 53f,
            animationLength: 5,
            numberOfLoops: 9,
            position: tileloc + (new Vector2(5f, 3f) * 4f),
            flicker: true,
            flipped: false,
            layerDepth: (float)(tileloc.Y + 7) / 10000f,
            alphaFade: 0f,
            color: Color.Yellow,
            scale: 4f,
            scaleChange: 0f,
            rotation: 0f,
            rotationChange: 0f)
        {
            id = bombID,
        };
        mp.broadcastSprites(loc, tas_yellow);
        TemporaryAnimatedSprite tas_orange = new(
            textureName: Game1.mouseCursorsName,
            sourceRect: new Rectangle(598, 1279, 3, 4),
            animationInterval: 53f,
            animationLength: 5,
            numberOfLoops: 9,
            position: tileloc + (new Vector2(5f, 3f) * 4f),
            flicker: true,
            flipped: false,
            layerDepth: (float)(tileloc.Y + 7) / 10000f,
            alphaFade: 0f,
            color: Color.Orange,
            scale: 4f,
            scaleChange: 0f,
            rotation: 0f,
            rotationChange: 0f)
        {
            delayBeforeAnimationStart = 100,
            id = bombID,
        };
        mp.broadcastSprites(loc, tas_orange);
        TemporaryAnimatedSprite tas_white = new(
            textureName: Game1.mouseCursorsName,
            sourceRect: new Rectangle(598, 1279, 3, 4),
            animationInterval: 53f,
            animationLength: 5,
            numberOfLoops: 9,
            position: tileloc + (new Vector2(5f, 3f) * 4f),
            flicker: true,
            flipped: false,
            layerDepth: (float)(tileloc.Y + 7) / 10000f,
            alphaFade: 0f,
            color: Color.White,
            scale: 4f,
            scaleChange: 0f,
            rotation: 0f,
            rotationChange: 0f)
        {
            delayBeforeAnimationStart = 200,
            id = bombID,
        };
        mp.broadcastSprites(loc, tas_white);
        loc.netAudio.StartPlaying("fuse");
    }

    /// <summary>
    /// Adds a splash at the indicated location.
    /// </summary>
    /// <param name="loc">Map.</param>
    /// <param name="nonTileLocation">Location in pixels.</param>
    /// <param name="mp">Multiplayer instance.</param>
    /// <param name="delayTime">How long to wait before splashing.</param>
    /// <remarks>Mostly copied from FishPond.showObjectThrownIntoPondAnimation.</remarks>
    public static void DrawWaterSplash(GameLocation loc, Vector2 nonTileLocation, Multiplayer mp, int delayTime)
    {
        mp.broadcastSprites(
            loc,
            new TemporaryAnimatedSprite(
                initialParentTileIndex: 28,
                animationInterval: 100f,
                animationLength: 2,
                numberOfLoops: 1,
                position: nonTileLocation,
                flicker: false,
                flipped: false)
                {
                    delayBeforeAnimationStart = delayTime,
                    layerDepth = (nonTileLocation.Y + 2f) / 10000f,
                });

        mp.broadcastSprites(
            loc,
            new TemporaryAnimatedSprite(
                textureName: Game1.animationsName,
                sourceRect: new Rectangle(0, 0, 64, 64),
                animationInterval: 55f,
                animationLength: 8,
                numberOfLoops: 0,
                position: nonTileLocation,
                flicker: false,
                flipped: Game1.random.NextDouble() < 0.5,
                layerDepth: (nonTileLocation.Y + 1f) / 10000f,
                alphaFade: 0.01f,
                color: Color.White,
                scale: 0.75f,
                scaleChange: 0.003f,
                rotation: 0f,
                rotationChange: 0f)
                {
                    delayBeforeAnimationStart = delayTime,
                });

        mp.broadcastSprites(
            loc,
            new TemporaryAnimatedSprite(
                textureName: Game1.animationsName,
                sourceRect: new Rectangle(0, 0, 64, 64),
                animationInterval: 65f,
                animationLength: 8,
                numberOfLoops: 0,
                position: nonTileLocation + new Vector2(Game1.random.Next(-32, 32), Game1.random.Next(-16, 32)),
                flicker: false,
                flipped: Game1.random.NextDouble() < 0.5,
                layerDepth: (nonTileLocation.Y + 1f) / 10000f,
                alphaFade: 0.01f,
                color: Color.White,
                scale: 0.75f,
                scaleChange: 0.003f,
                rotation: 0f,
                rotationChange: 0f)
                {
                    delayBeforeAnimationStart = delayTime,
                });

        mp.broadcastSprites(
            loc,
            new TemporaryAnimatedSprite(
                textureName: Game1.animationsName,
                sourceRect: new Rectangle(0, 0, 64, 64),
                animationInterval: 75f,
                animationLength: 8,
                numberOfLoops: 0,
                position: nonTileLocation + new Vector2(Game1.random.Next(-32, 32), Game1.random.Next(-16, 32)),
                flicker: false,
                flipped: Game1.random.NextDouble() < 0.5,
                layerDepth: (nonTileLocation.Y + 1f) / 10000f,
                alphaFade: 0.01f,
                color: Color.White,
                scale: 0.75f,
                scaleChange: 0.003f,
                rotation: 0f,
                rotationChange: 0f)
                {
                    delayBeforeAnimationStart = delayTime,
                });
    }

    #endregion

    #region AllLocations

    /// <summary>
    /// Gets all the tiles of a location within specified bounds.
    /// </summary>
    /// <param name="location">The location to get the tiles from.</param>
    /// <param name="xstart">Start for x.</param>
    /// <param name="xend">End for x.</param>
    /// <param name="ystart">Start for y.</param>
    /// <param name="yend">End for y.</param>
    /// <returns>A rented array of tiles, plus the total count.</returns>
    /// <remarks>The array is rented, do remember to return it.</remarks>
    public static (Vector2[] tiles, int count) GetTiles(
        this GameLocation location,
        int xstart = 1,
        int xend = int.MaxValue,
        int ystart = 1,
        int yend = int.MaxValue)
    {
        Guard.IsNotNull(location);

        // sanity
        xstart = Math.Max(xstart, 1);
        ystart = Math.Max(ystart, 1);

        xend = Math.Clamp(xend, xstart, location.Map.Layers[0].LayerWidth - 2);
        yend = Math.Clamp(yend, ystart, location.Map.Layers[0].LayerHeight - 2);

        int count = (xend - xstart) * (yend - ystart);

        if (count == 0)
        {
            return (Array.Empty<Vector2>(), 0);
        }

        Vector2[]? buffer = ArrayPool<Vector2>.Shared.Rent(count);
        int yLength = yend - ystart + 1;
        for (int x = xstart; x <= xend; x++)
        {
            for (int y = ystart; y <= yend; y++)
            {
                buffer[((x - xstart) * yLength) + (y - ystart)] = new Vector2(x, y);
            }
        }
        return (buffer, count);
    }

    /// <summary>
    /// Yields all game locations.
    /// </summary>
    /// <returns>IEnumerable of all game locations.</returns>
    public static IEnumerable<GameLocation> YieldAllLocations()
    {
        foreach (GameLocation location in Game1.locations)
        {
            yield return location;
            if (location is BuildableGameLocation buildableloc)
            {
                foreach (GameLocation loc in YieldInteriorLocations(buildableloc))
                {
                    yield return loc;
                }
            }
        }
    }

    /// <summary>
    /// Gets all the buildings.
    /// </summary>
    /// <returns>IEnumerable of all buildings.</returns>
    public static IEnumerable<Building> GetBuildings()
    {
        foreach (GameLocation? loc in Game1.locations)
        {
            if (loc is BuildableGameLocation buildable)
            {
                foreach (Building? building in GetBuildings(buildable))
                {
                    yield return building;
                }
            }
        }
    }

    private static IEnumerable<GameLocation> YieldInteriorLocations(BuildableGameLocation loc)
    {
        foreach (Building building in loc.buildings)
        {
            if (building.indoors?.Value is GameLocation indoorloc)
            {
                yield return indoorloc;
                if (indoorloc is BuildableGameLocation buildableIndoorLoc)
                {
                    foreach (GameLocation nestedLocation in YieldInteriorLocations(buildableIndoorLoc))
                    {
                        yield return nestedLocation;
                    }
                }
            }
        }
    }

    private static IEnumerable<Building> GetBuildings(BuildableGameLocation loc)
    {
        foreach (Building building in loc.buildings)
        {
            yield return building;
            if (building.indoors?.Value is BuildableGameLocation buildable)
            {
                foreach (Building interiorBuilding in GetBuildings(buildable))
                {
                    yield return interiorBuilding;
                }
            }
        }
    }

    #endregion
}