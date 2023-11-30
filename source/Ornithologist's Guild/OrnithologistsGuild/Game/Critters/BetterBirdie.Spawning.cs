/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using OrnithologistsGuild.Content;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace OrnithologistsGuild.Game.Critters
{
    public enum SpawnType
    {
        Land,
        Water,
        Perch
    }

    public partial class BetterBirdie : StardewValley.BellsAndWhistles.Critter
	{
        private const int Trials = 50;

        private bool CheckRelocationDistance(Vector2 relocateToTile)
        {
            var currentTile = position / Game1.tileSize;

            var distance = Vector2.Distance(currentTile, relocateToTile);
            if (distance < 10) return false; // Too close

            var distanceX = MathF.Abs(currentTile.X - relocateToTile.X);
            var distanceY = MathF.Abs(currentTile.Y - relocateToTile.Y);
            if (distanceX < 6 || distanceY < 4) return false; // Too straight (lol)

            return true;
        }

        public static SpawnType GetRandomSpawnType(BirdieDef birdieDef)
        {
            return Utilities.WeightedRandom(
                new List<SpawnType>() { SpawnType.Land, SpawnType.Water, SpawnType.Perch },
                spawnType => spawnType switch
                {
                    SpawnType.Land => birdieDef.LandPreference,
                    SpawnType.Water => birdieDef.WaterPreference,
                    SpawnType.Perch => birdieDef.PerchPreference,
                    _ => 0
                }
            );
        }

        private static bool CheckArea(Rectangle area, Func<Vector2, bool> predicate)
        {
            for (int left = area.Left; left < area.Right; ++left)
            {
                for (int top = area.Top; top < area.Bottom; ++top)
                {
                    var tile = new Vector2(left, top);

                    if (!predicate(tile)) return false;
                }
            }

            return true;
        }

        private static bool CheckCorners(Rectangle area, Func<Vector2, bool> predicate)
        {
            Vector2[] corners = new Vector2[]
            {
                new Vector2(area.Left, area.Top),
                new Vector2(area.Right, area.Top),
                new Vector2(area.Left, area.Bottom),
                new Vector2(area.Right, area.Bottom)
            };

            foreach (var corner in corners)
            {
                if (!predicate(corner)) return false;
            }

            return true;
        }

        public Tuple<Vector3, Perch> GetRandomPositionOrPerch(SpawnType? spawnType = null)
        {
            return GetRandomPositionsOrPerchesFor(Environment, BirdieDef, mustBeOffscreen: false, birdie: this, flockSize: 1, spawnType).FirstOrDefault();
        }

        public static IEnumerable<Tuple<Vector3, Perch>> GetRandomPositionsOrPerchesFor(GameLocation location, BirdieDef birdieDef, bool mustBeOffscreen, BetterBirdie birdie = null, int? flockSize = null, SpawnType? spawnType = null, Rectangle? tileAreaBound = null)
        {
            if (!spawnType.HasValue) spawnType = ModEntry.debug_PerchType == null ? GetRandomSpawnType(birdieDef) : SpawnType.Perch;
            if (!flockSize.HasValue) flockSize = Game1.random.Next(1, birdieDef.MaxFlockSize + 1);

            ModEntry.Instance.Monitor.Log($"GetRandomPositionsOrPerchesFor {flockSize} {spawnType} {birdieDef.ID}");

            IEnumerable<Perch> availablePerches = Enumerable.Empty<Perch>();
            if (spawnType == SpawnType.Perch)
            {
                if (ModEntry.debug_PerchType == null)
                {
                    availablePerches = Utilities.Randomize(
                        Perch.GetAllAvailablePerches(location,
                            mapTile: true,
                            tree: true,
                            feeder: birdieDef.FeederBaseWts.Any(),
                            bath: birdieDef.CanUseBaths));
                }
                else
                {
                    availablePerches = Utilities.Randomize(
                        Perch.GetAllAvailablePerches(location,
                            mapTile: ModEntry.debug_PerchType == PerchType.MapTile,
                            tree: ModEntry.debug_PerchType == PerchType.Tree,
                            feeder: ModEntry.debug_PerchType == PerchType.Feeder,
                            bath: ModEntry.debug_PerchType == PerchType.Bath));
                }

                if (!availablePerches.Any()) return Enumerable.Empty<Tuple<Vector3, Perch>>();
            }

            for (var i = 0; i < Trials; i++)
            {
                if (spawnType == SpawnType.Land || spawnType == SpawnType.Water)
                {
                    // Try to find a clear area
                    var randomTile = tileAreaBound.HasValue ?
                        Utility.getRandomPositionInThisRectangle(tileAreaBound.Value, Game1.random) :
                        location.getRandomTile();

                    var tileArea = Utility.getRectangleCenteredAt(randomTile, flockSize == 1 ? 1 : 3);

                    if (CheckArea(tileArea, tile =>
                    {
                        // Tile onscreen
                        if (mustBeOffscreen && Utility.isOnScreen(tile * Game1.tileSize, Game1.tileSize)) return false;

                        // Tile not on map
                        if (!location.isTileOnMap(tile)) return false;

                        // Tile not a valid land tile
                        if (spawnType == SpawnType.Land &&
                            !(location.isTilePassable(new xTile.Dimensions.Location((int)tile.X, (int)tile.Y), Game1.viewport) &&
                            !location.isWaterTile((int)tile.X, (int)tile.Y) &&
                            !location.isTileOccupied(tile))) return false;

                        // Tile not a valid water tile
                        if (spawnType == SpawnType.Water &&
                            !location.isOpenWater((int)tile.X, (int)tile.Y)) return false;

                        // Too close/straight to existing birdie
                        if (birdie != null && !birdie.CheckRelocationDistance(tile)) return false;

                        // Too close to character
                        if (Utility.isThereAFarmerOrCharacterWithinDistance(tile, birdieDef.GetContextualCautiousness(), location) != null) return false;

                        return true;
                    }))
                    {
                        // Suitable area found, distribute birds throughout it
                        var positionArea = new Rectangle(
                            tileArea.X * Game1.tileSize,
                            tileArea.Y * Game1.tileSize,
                            tileArea.Width * Game1.tileSize,
                            tileArea.Height * Game1.tileSize);

                        return Enumerable
                            .Repeat(0, flockSize.Value)
                            .Select(_ => new Tuple<Vector3, Perch>(
                                new Vector3(Utility.getRandomPositionInThisRectangle(positionArea, Game1.random), 0),
                                null)
                            );
                    }
                }
                else if (spawnType == SpawnType.Perch)
                {
                    if (flockSize > 1)
                    {
                        // Try to find a clear area
                        var randomTile = tileAreaBound.HasValue ?
                            Utility.getRandomPositionInThisRectangle(tileAreaBound.Value, Game1.random) :
                            location.getRandomTile();

                        var tileArea = Utility.getRectangleCenteredAt(randomTile, 17);

                        if (CheckCorners(tileArea, tile =>
                        {
                            // Tile onscreen
                            if (mustBeOffscreen && Utility.isOnScreen(tile * Game1.tileSize, Game1.tileSize)) return false;

                            // Tile not on map
                            if (!location.isTileOnMap(tile)) return false;

                            // Too close/straight to existing birdie
                            if (birdie != null && !birdie.CheckRelocationDistance(tile)) return false;

                            // Too close to character
                            if (Utility.isThereAFarmerOrCharacterWithinDistance(tile, birdieDef.GetContextualCautiousness(), location) != null) return false;

                            return true;
                        }))
                        {
                            return availablePerches
                            .Where(perch => tileArea.Contains(Utilities.XY(perch.Position) / Game1.tileSize) && birdieDef.CanPerchAt(perch))
                            .Take(flockSize.Value)
                            .Select(perch => new Tuple<Vector3, Perch>(perch.Position, perch));
                        }
                    } else
                    {
                        // Take at most 3 of each type of perch (this will
                        // favor feeders, baths, and map tiles for relocation)
                        return Utilities.Randomize(
                            availablePerches
                                .GroupBy(perch => perch.Type)
                                .SelectMany(group => group.Take(3)))
                            .Select(perch => new Tuple<Vector3, Perch>(perch.Position, perch))
                            .Take(1);
                    }
                }
            }

            return Enumerable.Empty<Tuple<Vector3, Perch>>();
        }
    }
}
