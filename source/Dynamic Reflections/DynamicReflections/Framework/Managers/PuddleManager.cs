/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;
using xTile.Tiles;

namespace DynamicReflections.Framework.Managers
{
    internal class PuddleManager
    {
        private const int PUDDLES_POOL = 16;
        internal const int DEFAULT_PUDDLE_INDEX = -1;

        internal List<TemporaryAnimatedSprite> puddleRippleSprites = new List<TemporaryAnimatedSprite>();
        private Dictionary<GameLocation, bool[,]> _locationToPuddleTiles;

        public void Reset()
        {
            _locationToPuddleTiles = new Dictionary<GameLocation, bool[,]>();
        }

        public void Generate(GameLocation location, int percentOfDiggableTiles = 10, bool force = false)
        {
            puddleRippleSprites = new List<TemporaryAnimatedSprite>();
            if (location is null || location.Map is null)
            {
                return;
            }

            if (_locationToPuddleTiles is null)
            {
                Reset();
            }
            else if (force is false && _locationToPuddleTiles.ContainsKey(location) is true && _locationToPuddleTiles[location] is not null)
            {
                return;
            }

            GenerateByPercentage(location, percentOfDiggableTiles);
        }

        private void GenerateByPercentage(GameLocation location, int percentOfDiggableTiles = 10)
        {
            var random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
            if (location.Map.GetLayer("Back") is var backLayer && backLayer is not null)
            {
                _locationToPuddleTiles[location] = new bool[backLayer.LayerWidth, backLayer.LayerHeight];

                List<Point> diggableTiles = new List<Point>();
                for (int x = 0; x < backLayer.LayerWidth; x++)
                {
                    for (int y = 0; y < backLayer.LayerHeight; y++)
                    {
                        if (backLayer.Tiles[x, y] is not null)
                        {
                            backLayer.Tiles[x, y].Properties["PuddleIndex"] = DEFAULT_PUDDLE_INDEX;
                            backLayer.Tiles[x, y].Properties["BigPuddleIndex"] = DEFAULT_PUDDLE_INDEX;

                            if (location.isTileLocationTotallyClearAndPlaceable(x, y) is false)
                            {
                                continue;
                            }

                            if (String.IsNullOrEmpty(location.doesTileHaveProperty(x, y, "Diggable", "Back")) is false && location.isTileHoeDirt(new Microsoft.Xna.Framework.Vector2(x, y)) is false)
                            {
                                diggableTiles.Add(new Point(x, y));
                            }
                            else
                            {
                                string stepType = location.doesTileHaveProperty(x, y, "Type", "Buildings");
                                if (stepType == null || stepType.Length < 1)
                                {
                                    stepType = location.doesTileHaveProperty(x, y, "Type", "Back");
                                }

                                if (stepType == "Dirt" || stepType == "Stone")
                                {
                                    diggableTiles.Add(new Point(x, y));
                                }
                            }
                        }
                    }
                }

                if (DynamicReflections.currentPuddleSettings.ShouldGeneratePuddles is false || percentOfDiggableTiles == 0)
                {
                    return;
                }

                for (int i = 0; i < diggableTiles.Count / percentOfDiggableTiles; i++)
                {
                    var tilePosition = GetRandomTile(random, diggableTiles);
                    if (IsBigPuddleTile(backLayer, tilePosition))
                    {
                        continue;
                    }

                    var puddleIndex = random.Next(DEFAULT_PUDDLE_INDEX, PUDDLES_POOL);
                    backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["PuddleIndex"] = puddleIndex;
                    backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["PuddleEffect"] = random.Next(0, 4);
                    backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["PuddleRotation"] = MathHelper.ToRadians(90 * random.Next(0, 4));

                    bool shouldAttemptBigPuddle = random.NextDouble() < DynamicReflections.currentPuddleSettings.BigPuddleChance / 100f;
                    if (shouldAttemptBigPuddle)
                    {
                        bool canMakeBigPuddle = true;
                        if (diggableTiles.FirstOrDefault(d => d.X == tilePosition.X + 1 && d.Y == tilePosition.Y) is var xOffsetPoint && (xOffsetPoint == default(Point) || IsBigPuddleTile(backLayer, xOffsetPoint)))
                        {
                            canMakeBigPuddle = false;
                        }
                        if (diggableTiles.FirstOrDefault(d => d.X == tilePosition.X + 1 && d.Y == tilePosition.Y + 1) is var yOffsetPoint && (yOffsetPoint == default(Point) || IsBigPuddleTile(backLayer, yOffsetPoint)))
                        {
                            canMakeBigPuddle = false;
                        }
                        if (diggableTiles.FirstOrDefault(d => d.X == tilePosition.X && d.Y == tilePosition.Y + 1) is var xyOffsetPoint && (xyOffsetPoint == default(Point) || IsBigPuddleTile(backLayer, xyOffsetPoint)))
                        {
                            canMakeBigPuddle = false;
                        }

                        if (canMakeBigPuddle)
                        {
                            var puddleEffect = random.Next(0, 4);
                            var puddleRotation = 0;
                            var adjustedPuddleIndex = random.Next(0, PUDDLES_POOL);
                            if (adjustedPuddleIndex % 2 != 0)
                            {
                                adjustedPuddleIndex -= 1;
                            }

                            backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["PuddleIndex"] = puddleEffect is (0 or 1) ? adjustedPuddleIndex : adjustedPuddleIndex + 1;
                            backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["BigPuddleIndex"] = puddleEffect is (0 or 2) ? 1 : 2;
                            backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["PuddleEffect"] = puddleEffect;
                            backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["PuddleRotation"] = puddleRotation;
                            _locationToPuddleTiles[location][tilePosition.X, tilePosition.Y] = true;

                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y].Properties["PuddleIndex"] = puddleEffect is (0 or 1) ? adjustedPuddleIndex : adjustedPuddleIndex + 1;
                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y].Properties["BigPuddleIndex"] = puddleEffect is (0 or 2) ? 2 : 1;
                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y].Properties["PuddleEffect"] = puddleEffect;
                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y].Properties["PuddleRotation"] = puddleRotation;
                            _locationToPuddleTiles[location][tilePosition.X + 1, tilePosition.Y] = true;

                            backLayer.Tiles[tilePosition.X, tilePosition.Y + 1].Properties["PuddleIndex"] = puddleEffect is (0 or 1) ? adjustedPuddleIndex + 1 : adjustedPuddleIndex;
                            backLayer.Tiles[tilePosition.X, tilePosition.Y + 1].Properties["BigPuddleIndex"] = puddleEffect is (0 or 2) ? 1 : 2;
                            backLayer.Tiles[tilePosition.X, tilePosition.Y + 1].Properties["PuddleEffect"] = puddleEffect;
                            backLayer.Tiles[tilePosition.X, tilePosition.Y + 1].Properties["PuddleRotation"] = puddleRotation;
                            _locationToPuddleTiles[location][tilePosition.X, tilePosition.Y + 1] = true;

                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y + 1].Properties["PuddleIndex"] = puddleEffect is (0 or 1) ? adjustedPuddleIndex + 1 : adjustedPuddleIndex;
                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y + 1].Properties["BigPuddleIndex"] = puddleEffect is (0 or 2) ? 2 : 1;
                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y + 1].Properties["PuddleEffect"] = puddleEffect;
                            backLayer.Tiles[tilePosition.X + 1, tilePosition.Y + 1].Properties["PuddleRotation"] = puddleRotation;
                            _locationToPuddleTiles[location][tilePosition.X + 1, tilePosition.Y + 1] = true;
                        }
                        else
                        {
                            _locationToPuddleTiles[location][tilePosition.X, tilePosition.Y] = puddleIndex != DEFAULT_PUDDLE_INDEX;
                        }
                    }
                    else
                    {
                        _locationToPuddleTiles[location][tilePosition.X, tilePosition.Y] = puddleIndex != DEFAULT_PUDDLE_INDEX;
                    }
                }
            }
        }

        private bool IsBigPuddleTile(Layer backLayer, Point tilePosition)
        {
            if (backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties.ContainsKey("BigPuddleIndex") && Int32.TryParse(backLayer.Tiles[tilePosition.X, tilePosition.Y].Properties["BigPuddleIndex"], out int value) && value != DEFAULT_PUDDLE_INDEX)
            {
                return true;
            }

            return false;
        }

        private Point GetRandomTile(Random random, List<Point> tilePoints)
        {
            return tilePoints[random.Next(tilePoints.Count)];
        }

        private void GeneratePerTile(GameLocation location)
        {
            var random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
            if (location.Map.GetLayer("Back") is var backLayer && backLayer is not null)
            {
                _locationToPuddleTiles[location] = new bool[backLayer.LayerWidth, backLayer.LayerHeight];

                for (int x = 0; x < backLayer.LayerWidth; x++)
                {
                    for (int y = 0; y < backLayer.LayerHeight; y++)
                    {
                        var point = new Point(x, y);
                        if (backLayer.Tiles[x, y] is not null)
                        {
                            var puddleIndex = DEFAULT_PUDDLE_INDEX;
                            if (backLayer.Tiles[x, y].TileIndexProperties.TryGetValue("Diggable", out _) && DoesPointHaveNeighbor(location, point) is false && location.isTileLocationTotallyClearAndPlaceable(x, y))
                            {
                                puddleIndex = random.NextDouble() < 0.95 ? DEFAULT_PUDDLE_INDEX : random.Next(DEFAULT_PUDDLE_INDEX, PUDDLES_POOL);
                                _locationToPuddleTiles[location][x, y] = puddleIndex != DEFAULT_PUDDLE_INDEX;
                            }

                            backLayer.Tiles[x, y].Properties["PuddleIndex"] = puddleIndex;
                        }
                    }
                }
            }
        }

        public bool DoesPointHaveNeighbor(GameLocation location, Point point)
        {
            var offsetPoint = new Point(point.X - 1, point.Y);
            if (IsTilePuddle(location, offsetPoint.X, offsetPoint.Y) is true)
            {
                return true;
            }

            offsetPoint = new Point(point.X + 1, point.Y);
            if (IsTilePuddle(location, offsetPoint.X, offsetPoint.Y) is true)
            {
                return true;
            }

            offsetPoint = new Point(point.X, point.Y - 1);
            if (IsTilePuddle(location, offsetPoint.X, offsetPoint.Y) is true)
            {
                return true;
            }

            offsetPoint = new Point(point.X, point.Y + 1);
            if (IsTilePuddle(location, offsetPoint.X, offsetPoint.Y) is true)
            {
                return true;
            }

            return false;
        }

        public bool IsTilePuddle(GameLocation location, int x, int y)
        {
            if (_locationToPuddleTiles.ContainsKey(location) is false || _locationToPuddleTiles[location] is null || x < 0 || y < 0 || _locationToPuddleTiles[location].GetLength(0) <= x || _locationToPuddleTiles[location].GetLength(1) <= y)
            {
                return false;
            }

            return _locationToPuddleTiles[location][x, y];
        }

        public List<Point> GetPuddleTiles(GameLocation location, bool limitToView = false)
        {
            var puddles = new List<Point>();
            if (_locationToPuddleTiles.ContainsKey(location) is false)
            {
                return puddles;
            }

            int tileWidth = Game1.pixelZoom * 16;
            int tileHeight = Game1.pixelZoom * 16;
            int tileXMin = ((Game1.viewport.X >= 0) ? (Game1.viewport.X / tileWidth) : ((Game1.viewport.X - tileWidth + 1) / tileWidth));
            int tileYMin = ((Game1.viewport.Y >= 0) ? (Game1.viewport.Y / tileHeight) : ((Game1.viewport.Y - tileHeight + 1) / tileHeight));
            if (tileXMin < 0)
            {
                tileXMin = 0;
            }
            if (tileYMin < 0)
            {
                tileYMin = 0;
            }
            int tileColumns = 1 + (Game1.viewport.Size.Width - 1) / tileWidth;
            int tileRows = 1 + (Game1.viewport.Size.Height - 1) / tileHeight;
            int tileXMax = tileXMin + tileColumns;
            int tileYMax = tileYMin + tileRows;

            for (int x = 0; x < _locationToPuddleTiles[location].GetLength(0); x++)
            {
                for (int y = 0; y < _locationToPuddleTiles[location].GetLength(1); y++)
                {
                    if (_locationToPuddleTiles[location][x, y] is true)
                    {
                        if (limitToView is false || (limitToView is true && x >= tileXMin && x < tileXMax && y >= tileYMin && y < tileYMax))
                        {
                            puddles.Add(new Point(x, y));
                        }
                    }
                }
            }

            return puddles;
        }
    }
}
