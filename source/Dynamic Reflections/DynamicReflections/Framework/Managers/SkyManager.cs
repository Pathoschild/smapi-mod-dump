/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Utilities.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace DynamicReflections.Framework.Managers
{
    internal class SkyManager
    {
        internal const int SKY_TILES_X = 4;
        internal const int SKY_TILES_Y = 5;
        internal const int DEFAULT_SKY_INDEX = 0;

        internal List<TemporaryAnimatedSprite> skyEffectSprites = new List<TemporaryAnimatedSprite>();
        private Dictionary<GameLocation, bool[,]> _locationToSkyTiles;
        private Dictionary<GameLocation, List<Point>> _locationToSkyPoints;

        public void Reset()
        {
            _locationToSkyTiles = new Dictionary<GameLocation, bool[,]>();
            _locationToSkyPoints = new Dictionary<GameLocation, List<Point>>();
        }

        public void Generate(GameLocation location, bool force = false)
        {
            skyEffectSprites = new List<TemporaryAnimatedSprite>();
            if (location is null || location.Map is null)
            {
                return;
            }

            if (_locationToSkyTiles is null)
            {
                Reset();
            }
            else if (force is false && _locationToSkyTiles.ContainsKey(location) is true && _locationToSkyTiles[location] is not null)
            {
                return;
            }

            GeneratePerTile(location);
        }

        private void GeneratePerTile(GameLocation location)
        {
            var random = new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
            if (location.Map.GetLayer("Back") is var backLayer && backLayer is not null)
            {
                _locationToSkyTiles[location] = new bool[backLayer.LayerWidth, backLayer.LayerHeight];
                _locationToSkyPoints[location] = new List<Point>();

                double starDensityPercentage = DynamicReflections.currentSkySettings.StarDensityPercentage / 100f;
                for (int x = 0; x < backLayer.LayerWidth; x++)
                {
                    for (int y = 0; y < backLayer.LayerHeight; y++)
                    {
                        if (location.isWaterTile(x, y) || (DynamicReflections.puddleManager is not null && DynamicReflections.puddleManager.IsTilePuddle(location, x, y)))
                        {
                            _locationToSkyTiles[location][x, y] = true;
                            _locationToSkyPoints[location].Add(new Point(x, y));
                            backLayer.Tiles[x, y].Properties["SkyIndex"] = random.NextDouble() > starDensityPercentage ? DEFAULT_SKY_INDEX : random.Next(DEFAULT_SKY_INDEX, SKY_TILES_X * SKY_TILES_Y);
                            backLayer.Tiles[x, y].Properties["SkyEffect"] = random.Next(0, 4);
                            backLayer.Tiles[x, y].Properties["SkyAlpha"] = random.NextDouble() < 0.55 ? (float)random.NextDouble() : 1f;
                        }
                    }
                }
            }
        }

        internal static Point GetTilePoint(int tileIndex)
        {
            return new Point(tileIndex % SKY_TILES_X, tileIndex / SKY_TILES_Y);
        }

        private static Point GetRandomTile(Random random, List<Point> tilePoints)
        {
            return tilePoints[random.Next(tilePoints.Count)];
        }

        internal void AttemptEffects(GameLocation location)
        {
            if (_locationToSkyPoints is null || _locationToSkyPoints.ContainsKey(location) is false)
            {
                return;
            }

            if (location.Map.GetLayer("Back") is var backLayer && backLayer is not null)
            {
                var skyTiles = GetSkyTiles(location, true);
                if (skyTiles.Count == 0)
                {
                    return;
                }
                var randomWaterTilePoint = GetRandomTile(Game1.random, GetSkyTiles(location, true));
                double cometChance = DynamicReflections.currentSkySettings.CometChance / 100f;

                var leftTile = backLayer.PickTile(new Location((randomWaterTilePoint.X - 1) * 64, randomWaterTilePoint.Y * 64), Game1.viewport.Size);
                var topTile = backLayer.PickTile(new Location(randomWaterTilePoint.X * 64, (randomWaterTilePoint.Y - 1) * 64), Game1.viewport.Size);
                if (leftTile is not null && location.isWaterTile(randomWaterTilePoint.X - 1, randomWaterTilePoint.Y) is false)
                {
                    // Trigger event with this tile as starting point
                    if (Game1.random.NextDouble() < cometChance)
                    {
                        skyEffectSprites.AddRange(GenerateComet(new Point(randomWaterTilePoint.X - 1, randomWaterTilePoint.Y)));
                    }
                    else
                    {
                        skyEffectSprites.Add(GenerateShootingStar(new Point(randomWaterTilePoint.X - 1, randomWaterTilePoint.Y)));
                    }
                }
                else if (topTile is not null && location.isWaterTile(randomWaterTilePoint.X, randomWaterTilePoint.Y - 1) is false)
                {
                    // Trigger event with this tile as starting point
                    if (Game1.random.NextDouble() < cometChance)
                    {
                        skyEffectSprites.AddRange(GenerateComet(new Point(randomWaterTilePoint.X, randomWaterTilePoint.Y - 1)));
                    }
                    else
                    {
                        skyEffectSprites.Add(GenerateShootingStar(new Point(randomWaterTilePoint.X, randomWaterTilePoint.Y - 1)));
                    }
                }
                else if (Game1.random.NextDouble() < 0.075)
                {
                    // Trigger event with this tile as starting point
                    if (Game1.random.NextDouble() < 0.0001)
                    {
                        skyEffectSprites.Add(GenerateEasterEgg(new Point(randomWaterTilePoint.X, randomWaterTilePoint.Y)));
                    }
                    else
                    {
                        skyEffectSprites.Add(GenerateShootingStar(new Point(randomWaterTilePoint.X, randomWaterTilePoint.Y)));
                    }
                }
            }
        }

        private TemporaryAnimatedSprite GenerateShootingStar(Point point)
        {
            var shootingStar = new TemporaryAnimatedSprite(DynamicReflections.assetManager.SkyEffectsTileSheetTexturePath, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), Game1.random.Next(150, 300), 3, 12, new Vector2(point.X, point.Y) * 64f, flicker: false, flipped: false, 0f, 0f, Color.White, (float)(Game1.random.Next(1, 4) + Game1.random.NextDouble()), 0f, 0f, 0f);

            var speed = (float)(Game1.random.NextDouble(DynamicReflections.currentSkySettings.ShootingStarMinSpeed, DynamicReflections.currentSkySettings.ShootingStarMaxSpeed));
            shootingStar.acceleration = new Vector2(speed, speed);

            return shootingStar;
        }

        private TemporaryAnimatedSprite GenerateEasterEgg(Point point)
        {
            var easterEgg = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 784, 16, 16), Game1.random.Next(150, 300), 4, 12, new Vector2(point.X, point.Y) * 64f, flicker: false, flipped: false, 0f, 0f, Color.Gray, 3f, 0f, 0f, 0f);

            var speed = (float)(Game1.random.NextDouble(DynamicReflections.currentSkySettings.ShootingStarMinSpeed, DynamicReflections.currentSkySettings.ShootingStarMaxSpeed));
            easterEgg.acceleration = new Vector2(speed, speed);

            return easterEgg;
        }

        private List<TemporaryAnimatedSprite> GenerateComet(Point point)
        {
            float speed = (float)(Game1.random.NextDouble(DynamicReflections.currentSkySettings.CometMinSpeed, DynamicReflections.currentSkySettings.CometMaxSpeed));
            float scale = (float)(Game1.random.Next(1, 3) + Game1.random.NextDouble());

            var segments = new List<TemporaryAnimatedSprite>();
            int minSegments = DynamicReflections.currentSkySettings.CometSegmentMin;
            int maxSegments = DynamicReflections.currentSkySettings.CometSegmentMax < minSegments ? minSegments + 1 : DynamicReflections.currentSkySettings.CometSegmentMax;
            int totalSegments = Game1.random.Next(minSegments, minSegments + 1);
            for (int i = 0; i < totalSegments; i++)
            {
                var offset = 0.1f * i;
                int numberOfLoops = i == 0 ? totalSegments * 36 : 72;

                var cometSegment = new TemporaryAnimatedSprite(DynamicReflections.assetManager.SkyEffectsTileSheetTexturePath, new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), Game1.random.Next(30, 150), i == 0 ? 1 : 3, numberOfLoops, new Vector2(point.X - offset, point.Y - offset) * 64f, flicker: false, flipped: false, 0f, 0f, Color.White, scale - offset * 2, 0f, 0f, 0f);

                cometSegment.acceleration = new Vector2(speed, speed);
                segments.Add(cometSegment);
            }

            return segments;
        }

        private List<Point> GetSkyTiles(GameLocation location, bool limitToView = false)
        {
            var tiles = new List<Point>();
            if (_locationToSkyPoints.ContainsKey(location) is false)
            {
                return tiles;
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

            foreach (var point in _locationToSkyPoints[location])
            {
                if (limitToView is false || (limitToView is true && point.X >= tileXMin && point.X < tileXMax && point.Y >= tileYMin && point.Y < tileYMax))
                {
                    tiles.Add(new Point(point.X, point.Y));
                }
            }

            if (DynamicReflections.puddleManager is not null)
            {
                tiles.AddRange(DynamicReflections.puddleManager.GetPuddleTiles(location, true));
            }

            return tiles;
        }
    }
}
