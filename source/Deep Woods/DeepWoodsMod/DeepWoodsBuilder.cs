using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using static DeepWoodsMod.DeepWoodsEnterExit;
using static DeepWoodsMod.DeepWoodsRandom;
using static DeepWoodsMod.DeepWoodsTileDefinitions;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;

namespace DeepWoodsMod
{
    class DeepWoodsBuilder
    {
        private enum GrassType
        {
            BLACK,
            DARK,
            NORMAL,
            BRIGHT
        }

        private enum PlacingDirection
        {
            DOWNWARDS,
            UPWARDS,
            LEFTWARDS,
            RIGHTWARDS
        }

        private class Placing
        {
            public readonly Location location;
            public readonly PlacingDirection dir;
            public readonly PlacingDirection dirInward;

            public int XDir
            {
                get
                {
                    switch (dir)
                    {
                        case PlacingDirection.LEFTWARDS:
                            return -1;
                        case PlacingDirection.RIGHTWARDS:
                            return 1;
                        case PlacingDirection.DOWNWARDS:
                        case PlacingDirection.UPWARDS:
                            return 0;
                        default:
                            throw new InvalidOperationException("Invalid placing direction: " + this.dir);
                    }
                }
            }

            public int YDir
            {
                get
                {
                    switch (dir)
                    {
                        case PlacingDirection.LEFTWARDS:
                        case PlacingDirection.RIGHTWARDS:
                            return 0;
                        case PlacingDirection.DOWNWARDS:
                            return 1;
                        case PlacingDirection.UPWARDS:
                            return -1;
                        default:
                            throw new InvalidOperationException("Invalid placing direction: " + this.dir);
                    }
                }
            }

            public int XDirInward
            {
                get
                {
                    switch (dirInward)
                    {
                        case PlacingDirection.LEFTWARDS:
                            return -1;
                        case PlacingDirection.RIGHTWARDS:
                            return 1;
                        case PlacingDirection.DOWNWARDS:
                        case PlacingDirection.UPWARDS:
                            return 0;
                        default:
                            throw new InvalidOperationException("Invalid placing direction: " + this.dirInward);
                    }
                }
            }

            public int YDirInward
            {
                get
                {
                    switch (dirInward)
                    {
                        case PlacingDirection.LEFTWARDS:
                        case PlacingDirection.RIGHTWARDS:
                            return 0;
                        case PlacingDirection.DOWNWARDS:
                            return 1;
                        case PlacingDirection.UPWARDS:
                            return -1;
                        default:
                            throw new InvalidOperationException("Invalid placing direction: " + this.dirInward);
                    }
                }
            }

            public int DistanceTo(Location location)
            {
                switch(dir)
                {
                    case PlacingDirection.LEFTWARDS:
                    case PlacingDirection.RIGHTWARDS:
                        return Math.Abs(location.X - this.location.X);
                    case PlacingDirection.DOWNWARDS:
                    case PlacingDirection.UPWARDS:
                        return Math.Abs(location.Y - this.location.Y);
                    default:
                        throw new InvalidOperationException("Invalid placing direction: " + this.dirInward);
                }
            }

            public Placing Replace(Location location)
            {
                return new Placing(location, this.dir, this.dirInward);
            }

            public Placing(Location location, PlacingDirection dir, PlacingDirection dirInward)
            {
                this.location = location;
                this.dir = dir;
                this.dirInward = dirInward;
            }
            public Placing(Placing placing, int steps, int stepsInward)
            {
                this.location.X = placing.location.X + (placing.XDir * steps) + (placing.XDirInward * stepsInward);
                this.location.Y = placing.location.Y + (placing.YDir * steps) + (placing.YDirInward * stepsInward);
                this.dir = placing.dir;
                this.dirInward = placing.dirInward;
            }
        }

        private enum PlaceMode
        {
            DONT_OVERRIDE,
            OVERRIDE
        }

        private DeepWoods deepWoods;
        private DeepWoodsRandom random;
        private DeepWoodsSpaceManager spaceManager;
        private Map map;
        private Dictionary<ExitDirection, Location> exitLocations;
        private TileSheet defaultOutdoorTileSheet;
        private TileSheet lakeTileSheet;
        private Layer backLayer;
        private Layer buildingsLayer;
        private Layer frontLayer;
        private Layer alwaysFrontLayer;


        public DeepWoodsBuilder(DeepWoods deepWoods, DeepWoodsRandom random, DeepWoodsSpaceManager spaceManager, Map map, Dictionary<ExitDirection, Location> exitLocations)
        {
            this.deepWoods = deepWoods;
            this.random = random;
            this.spaceManager = spaceManager;
            this.map = map;
            this.exitLocations = exitLocations;
            this.defaultOutdoorTileSheet = map.GetTileSheet(DEFAULT_OUTDOOR_TILESHEET_ID);
            this.lakeTileSheet = map.GetTileSheet(LAKE_TILESHEET_ID);
            this.backLayer = map.GetLayer("Back");
            this.buildingsLayer = map.GetLayer("Buildings");
            this.frontLayer = map.GetLayer("Front");
            this.alwaysFrontLayer = map.GetLayer("AlwaysFront");
        }

        public static DeepWoodsBuilder Build(DeepWoods deepWoods, Map map, Dictionary<ExitDirection, Location> exitLocations)
        {
            DeepWoodsBuilder deepWoodsBuilder = new DeepWoodsBuilder(deepWoods, new DeepWoodsRandom(deepWoods, deepWoods.Seed), new DeepWoodsSpaceManager(deepWoods.mapWidth.Value, deepWoods.mapHeight.Value), map, exitLocations);
            deepWoodsBuilder.Build();
            return deepWoodsBuilder;
        }

        private void Build()
        {
            GenerateForestBorder();

            if (deepWoods.isLichtung.Value)
                GenerateLichtung();
            else
                GenerateForestPatches();

            GenerateGround();

            if (deepWoods.lichtungHasLake.Value)
                AddLakeToLichtung();
        }

        private int GetRandomWaterTileIndex()
        {
            return this.random.GetRandomValue(WATER_TILES);
        }

        private int GetRandomGrassTileIndex(GrassType grassType)
        {
            switch (grassType)
            {
                case GrassType.BLACK:
                    return GrassTiles.BLACK;
                case GrassType.DARK:
                    return this.random.GetRandomValue(GrassTiles.DARK);
                case GrassType.BRIGHT:
                    return this.random.GetRandomValue(GrassTiles.BRIGHT);
                case GrassType.NORMAL:
                    return this.random.GetRandomValue(GrassTiles.NORMAL);
            }

            throw new ArgumentException("Unknown GrassType: " + grassType);
        }

        private bool IsTileBrightGrass(Location tileLocation, bool includeDarkPatches = false)
        {
            return IsTileBrightGrass(tileLocation.X, tileLocation.Y, includeDarkPatches);
        }

        private bool IsTileBrightGrass(int x, int y, bool includeDarkPatches = false)
        {
            return DeepWoodsBuilder.IsTileIndexBrightGrass(backLayer.Tiles[x, y]?.TileIndex ?? 0, includeDarkPatches);
        }

        public static bool IsTileIndexBrightGrass(int tileIndex, bool includeDarkPatches = false)
        {
            if (includeDarkPatches)
            {
                return tileIndex == 175
                    || tileIndex == 275
                    || tileIndex == 402
                    || tileIndex == 401
                    || tileIndex == 400
                    || tileIndex == 150
                    || tileIndex == 254
                    || tileIndex == 255
                    || tileIndex == 256;
            }
            else
            {
                return tileIndex == 175 || tileIndex == 275 || tileIndex == 402 || tileIndex == 150;
            }
        }

        private void GenerateGround()
        {
            int mapWidth = this.spaceManager.GetMapWidth();
            int mapHeight = this.spaceManager.GetMapHeight();

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.NORMAL), x, y);
                }
            }
        }

        private void GenerateForestBorder()
        {
            int mapWidth = this.spaceManager.GetMapWidth();
            int mapHeight = this.spaceManager.GetMapHeight();

            Size topLeftCornerSize = GenerateForestCorner(0, 0, 1, 1, DeepWoodsCornerTileMatrix.TOP_LEFT);
            Size topRightCornerSize = GenerateForestCorner(mapWidth - 1, 0, -1, 1, DeepWoodsCornerTileMatrix.TOP_RIGHT);
            Size bottomLeftCornerSize = GenerateForestCorner(0, mapHeight - 1, 1, -1, DeepWoodsCornerTileMatrix.BOTTOM_LEFT);
            Size bottomRightCornerSize = GenerateForestCorner(mapWidth - 1, mapHeight - 1, -1, -1, DeepWoodsCornerTileMatrix.BOTTOM_RIGHT);

            GenerateForestRow(
                new Placing(new Location(topLeftCornerSize.Width, 0), PlacingDirection.RIGHTWARDS, PlacingDirection.DOWNWARDS),
                mapWidth - (topLeftCornerSize.Width + topRightCornerSize.Width),
                ExitDirection.TOP,
                DeepWoodsRowTileMatrix.TOP);

            GenerateForestRow(
                new Placing(new Location(bottomLeftCornerSize.Width, mapHeight - 1), PlacingDirection.RIGHTWARDS, PlacingDirection.UPWARDS),
                mapWidth - (bottomLeftCornerSize.Width + bottomRightCornerSize.Width),
                ExitDirection.BOTTOM,
                DeepWoodsRowTileMatrix.BOTTOM);

            GenerateForestRow(
                new Placing(new Location(0, topLeftCornerSize.Height), PlacingDirection.DOWNWARDS, PlacingDirection.RIGHTWARDS),
                mapHeight - (topLeftCornerSize.Height + bottomLeftCornerSize.Height),
                ExitDirection.LEFT,
                DeepWoodsRowTileMatrix.LEFT);

            GenerateForestRow(
                new Placing(new Location(mapWidth - 1, topRightCornerSize.Height), PlacingDirection.DOWNWARDS, PlacingDirection.LEFTWARDS),
                mapHeight - (topRightCornerSize.Height + bottomRightCornerSize.Height),
                ExitDirection.RIGHT,
                DeepWoodsRowTileMatrix.RIGHT);

            GenerateExits();
        }

        private void GenerateForestRow(
            Placing placing,
            int numTiles,
            ExitDirection exitDir,
            DeepWoodsRowTileMatrix matrix)
        {
            if (this.exitLocations.ContainsKey(exitDir))
            {
                Location exitPosition = this.exitLocations[exitDir];

                int numTilesFromStartToExit = placing.DistanceTo(exitPosition);
                int numTilesFromExitToEnd = numTiles - numTilesFromStartToExit - 1;
                
                GenerateForestRow(placing, numTilesFromStartToExit - Settings.Map.ExitRadius, matrix);
                GenerateForestRow(new Placing(placing.Replace(exitPosition), Settings.Map.ExitRadius + 1, 0), numTilesFromExitToEnd - Settings.Map.ExitRadius, matrix);
            }
            else
            {
                GenerateForestRow(placing, numTiles, matrix);
            }
        }

        private void GenerateForestRow(Placing placing, int numTiles, DeepWoodsRowTileMatrix matrix, int y = 0, bool noBlackGrass = false)
        {
            // Out of range check
            if (numTiles <= 0)
            {
                ModEntry.Log("GenerateForestRow out of range! dir: " + placing.dir + ", numTiles: " + numTiles, StardewModdingAPI.LogLevel.Warn);
                return;
            }

            bool lastStepWasBumpOut = false;
            for (int x = 0; x < numTiles; x++)
            {
                if (y > 0 && (x >= (numTiles - Math.Abs(y)) || (!lastStepWasBumpOut && this.random.CheckChance(Chance.FIFTY_FIFTY))))
                {
                    // Bump back!
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, placing, x, y - 1);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_LEFT_CORNER_BACK, placing, x, y - 1);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_LEFT_CONCAVE_CORNER, placing, x, y + 0);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_LEFT_CONVEX_CORNER, placing, x, y + 1);
                    if (!noBlackGrass && matrix.HAS_BLACK_GRASS)
                    {
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BLACK), placing, x, y + 0, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.BLACK_GRASS_LEFT_CONCAVE_CORNER, placing, x, y + 1, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.BLACK_GRASS_LEFT_CONVEX_CORNER, placing, x, y + 2, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_LEFT_CONVEX_CORNER, placing, x, y + 3, PlaceMode.OVERRIDE);
                    }
                    else
                    {
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.DARK), placing, x, y + 0, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_LEFT_CONCAVE_CORNER, placing, x, y + 1, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_LEFT_CONVEX_CORNER, placing, x, y + 2, PlaceMode.OVERRIDE);
                    }
                    y--;
                    lastStepWasBumpOut = false;
                }
                else if (x < (numTiles - (2 + Math.Abs(y))) && y < Settings.Map.MaxBumpSizeForForestBorder && this.random.CheckChance(Chance.FIFTY_FIFTY))
                {
                    // Bump out!
                    y++;
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, placing, x, y - 1);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_RIGHT_CORNER_BACK, placing, x, y - 1);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_RIGHT_CONCAVE_CORNER, placing, x, y + 0);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_RIGHT_CONVEX_CORNER, placing, x, y + 1);
                    if (!noBlackGrass && matrix.HAS_BLACK_GRASS)
                    {
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BLACK), placing, x, y + 0, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.BLACK_GRASS_RIGHT_CONCAVE_CORNER, placing, x, y + 1, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.BLACK_GRASS_RIGHT_CONVEX_CORNER, placing, x, y + 2, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT_CONVEX_CORNER, placing, x, y + 3, PlaceMode.OVERRIDE);
                    }
                    else
                    {
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.DARK), placing, x, y + 0, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT_CONCAVE_CORNER, placing, x, y + 1, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT_CONVEX_CORNER, placing, x, y + 2, PlaceMode.OVERRIDE);
                    }
                    lastStepWasBumpOut = true;
                }
                else
                {
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, placing, x, y + 0);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_BACK, placing, x, y + 0);
                    PlaceTile(alwaysFrontLayer, matrix.FOREST_FRONT, placing, x, y + 1);
                    if (!noBlackGrass && matrix.HAS_BLACK_GRASS)
                    {
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BLACK), placing, x, y + 0, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BLACK), placing, x, y + 1, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.BLACK_GRASS_FRONT, placing, x, y + 2, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_FRONT, placing, x, y + 3, PlaceMode.OVERRIDE);
                        if (!lastStepWasBumpOut && x > 1 && x < numTiles - 2 && x % 2 == 0 && this.random.CheckChance(Chance.FIFTY_FIFTY))
                        {
                            PlaceTile(buildingsLayer, FOREST_ROW_TREESTUMP_LEFT, placing, x - 1, y + 1);
                            PlaceTile(buildingsLayer, FOREST_ROW_TREESTUMP_RIGHT, placing, x, y + 1);
                        }
                    }
                    else
                    {
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.DARK), placing, x, y + 0, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.DARK), placing, x, y + 1, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_FRONT, placing, x, y + 2, PlaceMode.OVERRIDE);
                    }
                    lastStepWasBumpOut = false;
                }

                // Fill tiles behind "bumped out" row
                for (int yy = y; yy >= 0; yy--)
                {
                    if (PlaceTile(alwaysFrontLayer, GetRandomForestFillerTileIndex(), placing, x, yy))
                    {
                        PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, placing, x, yy);
                        PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BLACK), placing, x, yy, PlaceMode.OVERRIDE);
                    }
                }
            }
        }

        private Size GenerateForestCorner(int startXPos, int startYPos, int xDir, int yDir, DeepWoodsCornerTileMatrix matrix)
        {
            int width = Settings.Map.MaxTilesForCorner; // this.random.GetRandomValue(DeepWoodsSpaceManager.Settings.Map.MinTilesForCorner, Settings.Map.MaxTilesForCorner);
            int height = Settings.Map.MaxTilesForCorner; //this.random.GetRandomValue(DeepWoodsSpaceManager.Settings.Map.MinTilesForCorner, Settings.Map.MaxTilesForCorner);

            float ratio = (float)height / (float)width;
            int chanceValue = (int)((100f / (ratio + 1f)) * ratio);
            Chance chance = new Chance(chanceValue, 100);

            int endXPos = startXPos + ((width - 1) * xDir);
            int endYPos = startYPos + ((height - 1) * yDir);

            int curXPos = endXPos;
            int curYPos = startYPos;

            while (curXPos != startXPos)
            {
                int deltaX = Math.Abs(curXPos - startXPos);
                int deltaY = Math.Abs(curYPos - endYPos);
                if (deltaX > 1 && deltaY > 2 && this.random.CheckChance(chance))
                {
                    // go vertical
                    for (int y = startYPos; y != curYPos; y += yDir)
                    {
                        FillForestTile(curXPos, y);
                    }
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, curXPos, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONCAVE_CORNER_HORIZONTAL_BACK, curXPos, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONCAVE_CORNER, curXPos, curYPos + 1 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONVEX_CORNER, curXPos, curYPos + 2 * yDir, PlaceMode.OVERRIDE);

                    PlaceTile(backLayer, GetRandomGrassTileIndex(matrix.HAS_BLACK_GRASS ? GrassType.BLACK : GrassType.DARK), curXPos, curYPos + 1 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(backLayer, GetRandomGrassTileIndex(matrix.HAS_BLACK_GRASS ? GrassType.BLACK : GrassType.DARK), curXPos, curYPos + 2 * yDir, PlaceMode.OVERRIDE);

                    PlaceTile(backLayer, matrix.HAS_BLACK_GRASS ? matrix.BLACK_GRASS_CONCAVE_CORNER : matrix.DARK_GRASS_CONCAVE_CORNER, curXPos + 1 * xDir, curYPos + 1 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(backLayer, matrix.HAS_BLACK_GRASS ? matrix.BLACK_GRASS_CONVEX_CORNER : matrix.DARK_GRASS_CONVEX_CORNER, curXPos + 1 * xDir, curYPos + 2 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(backLayer, matrix.HAS_BLACK_GRASS ? matrix.BLACK_GRASS_HORIZONTAL : matrix.DARK_GRASS_HORIZONTAL, curXPos + 0 * xDir, curYPos + 2 * yDir, PlaceMode.OVERRIDE);

                    if (matrix.HAS_BLACK_GRASS)
                    {
                        // Add dark grass
                        PlaceTile(backLayer, matrix.DARK_GRASS_CONCAVE_CORNER, curXPos + 2 * xDir, curYPos + 2 * yDir, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_CONVEX_CORNER, curXPos + 2 * xDir, curYPos + 3 * yDir, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_HORIZONTAL, curXPos + 1 * xDir, curYPos + 3 * yDir, PlaceMode.OVERRIDE);
                        PlaceTile(backLayer, matrix.DARK_GRASS_HORIZONTAL, curXPos + 0 * xDir, curYPos + 3 * yDir, PlaceMode.OVERRIDE);
                    }

                    curXPos -= xDir;
                    curYPos += yDir;
                }
                else if (deltaX > 1)
                {
                    // go horizontal
                    for (int y = startYPos; y != curYPos; y += yDir)
                    {
                        FillForestTile(curXPos, y);
                    }
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, curXPos, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, this.random.GetRandomValue(matrix.HORIZONTAL_BACK), curXPos, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, this.random.GetRandomValue(matrix.HORIZONTAL_FRONT), curXPos, curYPos + 1 * yDir, PlaceMode.OVERRIDE);

                    PlaceTile(backLayer, matrix.HAS_BLACK_GRASS ? matrix.BLACK_GRASS_HORIZONTAL : matrix.DARK_GRASS_HORIZONTAL, curXPos, curYPos + 1 * yDir, PlaceMode.OVERRIDE);

                    if (matrix.HAS_BLACK_GRASS)
                    {
                        // Add dark grass
                        PlaceTile(backLayer, matrix.DARK_GRASS_HORIZONTAL, curXPos, curYPos + 2 * yDir, PlaceMode.OVERRIDE);
                    }

                    curXPos -= xDir;
                }
                else
                {
                    // fill last corner
                    for (int y = startYPos; y != curYPos; y += yDir)
                    {
                        FillForestTile(curXPos - 1 * xDir, y);
                        FillForestTile(curXPos - 0 * xDir, y);
                    }
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, curXPos - 1 * xDir, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, curXPos - 0 * xDir, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, curXPos - 1 * xDir, curYPos + 1 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONCAVE_CORNER_DIAGONAL_BACK, curXPos - 1 * xDir, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONCAVE_CORNER_HORIZONTAL_BACK, curXPos - 0 * xDir, curYPos + 0 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONCAVE_CORNER_VERTICAL_BACK, curXPos - 1 * xDir, curYPos + 1 * yDir, PlaceMode.OVERRIDE);
                    PlaceTile(alwaysFrontLayer, matrix.CONCAVE_CORNER, curXPos - 0 * xDir, curYPos + 1 * yDir, PlaceMode.OVERRIDE);
                    curYPos += yDir;
                    if (curYPos != endYPos)
                    {
                        height -= Math.Abs(curYPos - endYPos);
                    }
                    curXPos -= xDir;
                }
            }

            deepWoods.lightSources.Add(new Vector2(startXPos, startYPos));

            return new Size(width, height);
        }

        private void GenerateExits()
        {
            foreach (var exit in this.exitLocations)
            {
                switch (exit.Key)
                {
                    case ExitDirection.TOP:
                        GenerateExit(new Placing(exit.Value, PlacingDirection.RIGHTWARDS, PlacingDirection.DOWNWARDS), DeepWoodsRowTileMatrix.TOP);
                        break;
                    case ExitDirection.BOTTOM:
                        GenerateExit(new Placing(exit.Value, PlacingDirection.RIGHTWARDS, PlacingDirection.UPWARDS), DeepWoodsRowTileMatrix.BOTTOM);
                        break;
                    case ExitDirection.LEFT:
                        GenerateExit(new Placing(exit.Value, PlacingDirection.DOWNWARDS, PlacingDirection.RIGHTWARDS), DeepWoodsRowTileMatrix.LEFT);
                        break;
                    case ExitDirection.RIGHT:
                        GenerateExit(new Placing(exit.Value, PlacingDirection.DOWNWARDS, PlacingDirection.LEFTWARDS), DeepWoodsRowTileMatrix.RIGHT);
                        break;
                }
                deepWoods.lightSources.Add(new Vector2(exit.Value.X, exit.Value.Y));
            }
        }

        private void GenerateExit(Placing placing, DeepWoodsRowTileMatrix matrix)
        {
            // Add forest pieces left and right
            PlaceTile(alwaysFrontLayer, matrix.FOREST_LEFT_FRONT, placing, -Settings.Map.ExitRadius, 0);
            PlaceTile(alwaysFrontLayer, matrix.FOREST_LEFT_CONVEX_CORNER, placing, -Settings.Map.ExitRadius, 1);
            PlaceTile(alwaysFrontLayer, matrix.FOREST_RIGHT_FRONT, placing, Settings.Map.ExitRadius, 0);
            PlaceTile(alwaysFrontLayer, matrix.FOREST_RIGHT_CONVEX_CORNER, placing, Settings.Map.ExitRadius, 1);

            // Add forest "shadow" (dark grass) left and right
            PlaceTile(backLayer, matrix.DARK_GRASS_LEFT, placing, -Settings.Map.ExitRadius, 0);
            PlaceTile(backLayer, matrix.DARK_GRASS_LEFT, placing, -Settings.Map.ExitRadius, 1);
            PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT, placing, Settings.Map.ExitRadius, 0);
            PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT, placing, Settings.Map.ExitRadius, 1);

            if (matrix.HAS_BLACK_GRASS)
            {
                // longer dark grass to fit row with black grass
                PlaceTile(backLayer, matrix.DARK_GRASS_LEFT, placing, -Settings.Map.ExitRadius, 2);
                PlaceTile(backLayer, matrix.DARK_GRASS_LEFT_CONVEX_CORNER, placing, -Settings.Map.ExitRadius, 3);
                PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT, placing, Settings.Map.ExitRadius, 2);
                PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT_CONVEX_CORNER, placing, Settings.Map.ExitRadius, 3);

                // Override black shadows from row with corner tiles
                PlaceTile(backLayer, matrix.BLACK_GRASS_LEFT, placing, -(Settings.Map.ExitRadius + 1), 1, PlaceMode.OVERRIDE);
                PlaceTile(backLayer, matrix.BLACK_GRASS_LEFT_CONVEX_CORNER, placing, -(Settings.Map.ExitRadius + 1), 2, PlaceMode.OVERRIDE);
                PlaceTile(backLayer, matrix.BLACK_GRASS_RIGHT, placing, Settings.Map.ExitRadius + 1, 1, PlaceMode.OVERRIDE);
                PlaceTile(backLayer, matrix.BLACK_GRASS_RIGHT_CONVEX_CORNER, placing, Settings.Map.ExitRadius + 1, 2, PlaceMode.OVERRIDE);
            }
            else
            {
                // simple dark grass corner to fit row shadow without black grass
                PlaceTile(backLayer, matrix.DARK_GRASS_LEFT_CONVEX_CORNER, placing, -Settings.Map.ExitRadius, 2);
                PlaceTile(backLayer, matrix.DARK_GRASS_RIGHT_CONVEX_CORNER, placing, Settings.Map.ExitRadius, 2);
            }

            // Add bright grass entrance tiles
            PlaceTile(backLayer, matrix.BRIGHT_GRASS_RIGHT_CONCAVE_CORNER, placing, -1, 0);
            PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BRIGHT), placing, 0, 0);
            PlaceTile(backLayer, matrix.BRIGHT_GRASS_LEFT_CONCAVE_CORNER, placing, 1, 0);

            // If this level is a lichtung, GenerateLichtung() will generate bright grass growing out from each exit,
            // so we need to generate Settings.Map.ExitLength tiles here, that are "open ended".
            // In normal forest levels, we generate a random amount of tiles with variable end pieces.
            int brightGrassPacesInwards = deepWoods.isLichtung ? Settings.Map.ExitLength : this.random.GetRandomValue(2, 4);

            // Add bright grass some paces inwards
            for (int i = 1; i < brightGrassPacesInwards; i++)
            {
                PlaceTile(backLayer, matrix.BRIGHT_GRASS_RIGHT, placing, -1, i);
                PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BRIGHT), placing, 0, i);
                PlaceTile(backLayer, matrix.BRIGHT_GRASS_LEFT, placing, 1, i);
            }

            // Add one of 4 possible bright grass ends
            if (!deepWoods.isLichtung)
            {
                switch (this.random.GetRandomValue(0, 4))
                {
                    case 0:
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_RIGHT_CONVEX_CORNER, placing, -1, brightGrassPacesInwards);
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_FRONT_RIGHT_STEEP_CORNER, placing, 0, brightGrassPacesInwards);    // 301
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_LEFT_STEEP_CORNER, placing, 1, brightGrassPacesInwards - 1, PlaceMode.OVERRIDE);   // 328 -> 261
                        break;
                    case 1:
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_RIGHT_STEEP_CORNER, placing, -1, brightGrassPacesInwards - 1, PlaceMode.OVERRIDE); // 378 -> 281
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_FRONT_LEFT_STEEP_CORNER, placing, 0, brightGrassPacesInwards); // 303
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_LEFT_CONVEX_CORNER, placing, 1, brightGrassPacesInwards);
                        break;
                    case 2:
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_RIGHT_STEEP_CORNER, placing, -1, brightGrassPacesInwards - 1, PlaceMode.OVERRIDE); // 378 -> 281
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_TINY_FRONT, placing, 0, brightGrassPacesInwards);  // 326 -> 302
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_LEFT_STEEP_CORNER, placing, 1, brightGrassPacesInwards - 1, PlaceMode.OVERRIDE);   // 328 -> 261
                        break;
                    case 3:
                    default:
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_RIGHT_CONVEX_CORNER, placing, -1, brightGrassPacesInwards);
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_FRONT, placing, 0, brightGrassPacesInwards);
                        PlaceTile(backLayer, matrix.BRIGHT_GRASS_LEFT_CONVEX_CORNER, placing, 1, brightGrassPacesInwards);
                        break;
                }
            }
        }

        private Location GetExitLocationForLichtung(ExitDirection exitDir)
        {
            if (this.exitLocations.ContainsKey(exitDir))
            {
                return this.exitLocations[exitDir];
            }
            else
            {
                switch(exitDir)
                {
                    case ExitDirection.LEFT:
                        return new Location(0, this.spaceManager.GetMapHeight() / 2);
                    case ExitDirection.RIGHT:
                        return new Location(this.spaceManager.GetMapWidth() - 1, this.spaceManager.GetMapHeight() / 2);
                    case ExitDirection.TOP:
                        return new Location(this.spaceManager.GetMapWidth() / 2, 0);
                    case ExitDirection.BOTTOM:
                        return new Location(this.spaceManager.GetMapWidth() / 2, this.spaceManager.GetMapHeight() - 1);
                }
            }
            throw new ArgumentException("Invalid ExitDirection: " + exitDir);
        }

        // Called by DeepWoodsStuffCreator
        public void AddLakeToLichtung()
        {
            Location leftPos = GetExitLocationForLichtung(ExitDirection.LEFT);
            Location rightPos = GetExitLocationForLichtung(ExitDirection.RIGHT);
            Location topPos = GetExitLocationForLichtung(ExitDirection.TOP);
            Location bottomPos = GetExitLocationForLichtung(ExitDirection.BOTTOM);

            GenerateLichtungLakeCorner(leftPos, topPos, 1, 0, 0, -1, DeepWoodsLichtungTileMatrix.LEFT_TO_TOP);
            GenerateLichtungLakeCorner(topPos, rightPos, 0, 1, 1, 0, DeepWoodsLichtungTileMatrix.TOP_TO_RIGHT);
            GenerateLichtungLakeCorner(rightPos, bottomPos, -1, 0, 0, 1, DeepWoodsLichtungTileMatrix.RIGHT_TO_BOTTOM);
            GenerateLichtungLakeCorner(bottomPos, leftPos, 0, -1, -1, 0, DeepWoodsLichtungTileMatrix.BOTTOM_TO_LEFT);

            deepWoods.waterTiles = new bool[this.spaceManager.GetMapWidth(), this.spaceManager.GetMapHeight()];

            int minX = leftPos.X + Settings.Map.ExitLength;
            int maxX = rightPos.X - Settings.Map.ExitLength;
            int minY = topPos.Y + Settings.Map.ExitLength;
            int maxY = bottomPos.Y - Settings.Map.ExitLength;

            while (minX < maxX && buildingsLayer.Tiles[minX, leftPos.Y] == null)
                minX++;

            while (maxX > minX && buildingsLayer.Tiles[maxX, rightPos.Y] == null)
                maxX--;

            while (minY < maxY && buildingsLayer.Tiles[topPos.X, minY] == null)
                minY++;

            while (maxY > minY && buildingsLayer.Tiles[bottomPos.X, maxY] == null)
                maxY--;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (IsTileBrightGrass(x, y, true))
                    {
                        if (deepWoods.doesTileHaveProperty(x, y - 1, "Water", "Back") != null
                            && buildingsLayer.Tiles[x, y - 1] == null
                            && this.random.CheckChance(CHANCE_FOR_WATER_LILY))
                        {
                            if (this.random.CheckChance(CHANCE_FOR_BLOSSOM_ON_WATER_LILY))
                                PlaceAnimatedTile(buildingsLayer, defaultOutdoorTileSheet, WATER_LILY_WITH_BLOSSOM, x, y - 1, PlaceMode.DONT_OVERRIDE, this.random.GetRandomValue(WATER_LILY_FRAMERATES));
                            else
                                PlaceAnimatedTile(buildingsLayer, defaultOutdoorTileSheet, WATER_LILY, x, y - 1, PlaceMode.DONT_OVERRIDE, this.random.GetRandomValue(WATER_LILY_FRAMERATES));
                            PlaceTile(backLayer, WATER_LILY_SHADOW, x, y, PlaceMode.OVERRIDE);
                        }
                        else
                        {
                            PlaceTile(backLayer, GetRandomWaterTileIndex(), x, y, PlaceMode.OVERRIDE);
                        }
                        backLayer.Tiles[x, y].Properties.Add("Water", "T");
                        deepWoods.waterTiles[x, y] = true;
                    }
                }
            }
        }

        private void GenerateLichtungLakeCorner(Location startLocation, Location stopLocation, int xDir, int yDir, int xDirInwards, int yDirInwards, DeepWoodsLichtungTileMatrix matrix)
        {
            startLocation.X += xDir * Settings.Map.ExitLength;
            startLocation.Y += yDir * Settings.Map.ExitLength;
            stopLocation.X -= xDirInwards * Settings.Map.ExitLength;
            stopLocation.Y -= yDirInwards * Settings.Map.ExitLength;

            if (startLocation.X == stopLocation.X || startLocation.Y == stopLocation.Y)
                throw new ArgumentException($"Invalid arguments: startLocation and stopLocation must not be equal, nor lie in the same column or row! ({startLocation}, {stopLocation}, {xDir}, {yDir}, {xDirInwards}, {yDirInwards})");

            if (stopLocation.X > startLocation.X && (xDir < 0 || xDirInwards < 0))
                throw new ArgumentException($"Invalid arguments: stopLocation lies before startLocation in X direction! ({startLocation}, {stopLocation}, {xDir}, {yDir}, {xDirInwards}, {yDirInwards})");

            if (stopLocation.Y > startLocation.Y && (yDir < 0 || yDirInwards < 0))
                throw new ArgumentException($"Invalid arguments: stopLocation lies before startLocation in Y direction! ({startLocation}, {stopLocation}, {xDir}, {yDir}, {xDirInwards}, {yDirInwards})");

            while (!IsTileBrightGrass(stopLocation + new Location(xDir, yDir), true))
            {
                stopLocation.X -= xDirInwards;
                stopLocation.Y -= yDirInwards;
            }

            while (!IsTileBrightGrass(startLocation + new Location(xDirInwards, yDirInwards), true))
            {
                startLocation.X += xDir;
                startLocation.Y += yDir;
            }

            bool xDeltaIsNegative = (startLocation.X - stopLocation.X) < 0;
            bool yDeltaIsNegative = (startLocation.Y - stopLocation.Y) < 0;

            Location curLocation;
            if (IsTileBrightGrass(startLocation - new Location(xDirInwards, yDirInwards), true))
            {
                PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_VERTICAL), startLocation.X, startLocation.Y);
                Location nextLocation = startLocation - new Location(xDirInwards, yDirInwards);
                while (IsTileBrightGrass(nextLocation, true))
                {
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_VERTICAL), nextLocation.X, nextLocation.Y);
                    nextLocation -= new Location(xDirInwards, yDirInwards);
                }
                curLocation = startLocation + new Location(xDirInwards, yDirInwards);
            }
            else
            {
                PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_CONCAVE_CORNER), startLocation.X, startLocation.Y);
                curLocation = startLocation + new Location(xDirInwards, yDirInwards);
            }

            bool left = false;
            bool top = false;
            Location lastLocation = curLocation;
            while ((curLocation.X == stopLocation.X || xDeltaIsNegative == ((curLocation.X - stopLocation.X) < 0))
                && (curLocation.Y == stopLocation.Y || yDeltaIsNegative == ((curLocation.Y - stopLocation.Y) < 0)))
            {
                lastLocation = curLocation;
                left = !IsTileBrightGrass(curLocation - new Location(xDir, yDir), true);
                top = !IsTileBrightGrass(curLocation + new Location(xDirInwards, yDirInwards), true);
                if (left && top)
                {
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_CONCAVE_CORNER), curLocation.X, curLocation.Y);
                    curLocation.X += xDir;
                    curLocation.Y += yDir;
                }
                else if (top)
                {
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_HORIZONTAL), curLocation.X, curLocation.Y);
                    curLocation.X += xDir;
                    curLocation.Y += yDir;
                }
                else if (left)
                {
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_VERTICAL), curLocation.X, curLocation.Y);
                    curLocation.X += xDirInwards;
                    curLocation.Y += yDirInwards;
                }
                else
                {
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_CONVEX_CORNER), curLocation.X, curLocation.Y);
                    curLocation.X += xDirInwards;
                    curLocation.Y += yDirInwards;
                }
            }

            // Last tile must be closed at top, if algorithm did it wrong (because it didn't predict that it's the last tile), override here.
            if (!top)
            {
                if (left)
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_CONCAVE_CORNER), lastLocation.X, lastLocation.Y, PlaceMode.OVERRIDE);
                else
                    PlaceAnimatedTile(buildingsLayer, lakeTileSheet, GetWaterTileWithAnimationPartner(matrix.WATER_HORIZONTAL), lastLocation.X, lastLocation.Y, PlaceMode.OVERRIDE);
            }
        }

        private int[] GetWaterTileWithAnimationPartner(int[] tileIndices)
        {
            return GetWaterTileWithAnimationPartner(this.random.GetRandomValue(tileIndices));
        }

        private int[] GetWaterTileWithAnimationPartner(int tileIndex)
        {
            return new int[] { tileIndex, tileIndex + 4 };
        }

        private void GenerateLichtung()
        {
            Location leftPos = GetExitLocationForLichtung(ExitDirection.LEFT);
            Location rightPos = GetExitLocationForLichtung(ExitDirection.RIGHT);
            Location topPos = GetExitLocationForLichtung(ExitDirection.TOP);
            Location bottomPos = GetExitLocationForLichtung(ExitDirection.BOTTOM);

            GenerateLichtungCorner(leftPos + new Location(0, -1), topPos + new Location(-1, 0), 1, 0, 0, -1, DeepWoodsLichtungTileMatrix.LEFT_TO_TOP);
            GenerateLichtungCorner(topPos + new Location(1, 0), rightPos + new Location(0, -1), 0, 1, 1, 0, DeepWoodsLichtungTileMatrix.TOP_TO_RIGHT);
            GenerateLichtungCorner(rightPos + new Location(0, 1), bottomPos + new Location(1, 0), -1, 0, 0, 1, DeepWoodsLichtungTileMatrix.RIGHT_TO_BOTTOM);
            GenerateLichtungCorner(bottomPos + new Location(-1, 0), leftPos + new Location(0, 1), 0, -1, -1, 0, DeepWoodsLichtungTileMatrix.BOTTOM_TO_LEFT);

            GenerateLichtungEntranceTilesIfNecessary(ExitDirection.LEFT, leftPos, 1, 0, 0, -1, DeepWoodsLichtungTileMatrix.LEFT_TO_TOP);
            GenerateLichtungEntranceTilesIfNecessary(ExitDirection.TOP, topPos, 0, 1, 1, 0, DeepWoodsLichtungTileMatrix.TOP_TO_RIGHT);
            GenerateLichtungEntranceTilesIfNecessary(ExitDirection.RIGHT, rightPos, -1, 0, 0, 1, DeepWoodsLichtungTileMatrix.RIGHT_TO_BOTTOM);
            GenerateLichtungEntranceTilesIfNecessary(ExitDirection.BOTTOM, bottomPos, 0, -1, -1, 0, DeepWoodsLichtungTileMatrix.BOTTOM_TO_LEFT);

            int minX = Settings.Map.ExitLength;
            int minY = Settings.Map.ExitLength;
            int maxX = this.spaceManager.GetMapWidth() - Settings.Map.ExitLength;
            int maxY = this.spaceManager.GetMapHeight() - Settings.Map.ExitLength;

            deepWoods.lightSources.Add(new Vector2(this.spaceManager.GetMapWidth() / 2, this.spaceManager.GetMapHeight() / 2));
            int numAdditionalLights = ((maxX - minX) * (maxY - minY)) / NUM_TILES_PER_LIGHTSOURCE;
            for (int i = 0; i < numAdditionalLights; i++)
            {
                int x = this.random.GetRandomValue(minX, maxX);
                int y = this.random.GetRandomValue(minY, maxY);
                deepWoods.lightSources.Add(new Vector2(x, y));
            }

            Location lichtungCenter = (leftPos + rightPos + topPos + bottomPos) / 4;
            deepWoods.lichtungCenter.Value = new Point(lichtungCenter.X, lichtungCenter.Y);
        }

        private void GenerateLichtungEntranceTilesIfNecessary(ExitDirection exitDir, Location pos, int xDir, int yDir, int xDirInwards, int yDirInwards, DeepWoodsLichtungTileMatrix matrix)
        {
            // Lichtungen are generated as "growing" out from entrances from each map side.
            // However, when a side doesn't actually have an entrance,
            // the algorithm still behaves the same way from that side, assuming an entrance in the center of that side.
            // Here we simply generate 3 tiles that "close" the gap that exists, because there are no entrance tiles.

            if (this.exitLocations.ContainsKey(exitDir))
                return;

            pos.X += xDir * Settings.Map.ExitLength;
            pos.Y += yDir * Settings.Map.ExitLength;

            PlaceTile(backLayer, HasGroundTile(pos.X + 2 * xDirInwards, pos.Y + 2 * yDirInwards) ? matrix.VERTICAL : matrix.CONVEX_CORNER, pos.X + xDirInwards, pos.Y + yDirInwards, PlaceMode.OVERRIDE);
            PlaceTile(backLayer, matrix.VERTICAL, pos.X, pos.Y, PlaceMode.OVERRIDE);
            PlaceTile(backLayer, HasGroundTile(pos.X - 2 * xDirInwards, pos.Y - 2 * yDirInwards) ? matrix.VERTICAL : matrix.INVERSE_CONVEX_CORNER, pos.X - xDirInwards, pos.Y - yDirInwards, PlaceMode.OVERRIDE);
        }

        private bool HasGroundTile(int x, int y)
        {
            return this.backLayer.Tiles[x, y] != null;
        }

        private void GenerateLichtungCorner(Location startLocation, Location stopLocation, int xDir, int yDir, int xDirInwards, int yDirInwards, DeepWoodsLichtungTileMatrix matrix)
        {
            startLocation.X += xDir * Settings.Map.ExitLength;
            startLocation.Y += yDir * Settings.Map.ExitLength;
            stopLocation.X -= xDirInwards * Settings.Map.ExitLength;
            stopLocation.Y -= yDirInwards * Settings.Map.ExitLength;

            if (startLocation.X == stopLocation.X || startLocation.Y == stopLocation.Y)
                throw new ArgumentException($"Invalid arguments: startLocation and stopLocation must not be equal, nor lie in the same column or row! ({startLocation}, {stopLocation}, {xDir}, {yDir}, {xDirInwards}, {yDirInwards})");

            if (stopLocation.X > startLocation.X == (xDir < 0 || xDirInwards < 0))
                throw new ArgumentException($"Invalid arguments: stopLocation lies before startLocation in X direction! ({startLocation}, {stopLocation}, {xDir}, {yDir}, {xDirInwards}, {yDirInwards})");

            if (stopLocation.Y > startLocation.Y == (yDir < 0 || yDirInwards < 0))
                throw new ArgumentException($"Invalid arguments: stopLocation lies before startLocation in Y direction! ({startLocation}, {stopLocation}, {xDir}, {yDir}, {xDirInwards}, {yDirInwards})");

            FillLichtungRow(startLocation - new Location(xDirInwards, yDirInwards), stopLocation - new Location(xDirInwards, yDirInwards), xDir, yDir);

            Location curLocation = new Location(startLocation.X, startLocation.Y);
            bool isSteep = false;
            bool isHorizontal = true;

            while (Math.Abs(curLocation.X - stopLocation.X) > 1 && Math.Abs(curLocation.Y - stopLocation.Y) > 1)
            {
                bool isVerticalBlocked = backLayer.Tiles[curLocation.X + xDirInwards, curLocation.Y + yDirInwards] != null;
                bool goHorizontal = isSteep || isVerticalBlocked || this.random.CheckChance(GetChanceForHorizontal(curLocation, stopLocation, xDir == 0));
                bool goSteep = !isSteep && !isHorizontal && !goHorizontal && this.random.CheckChance(Chance.FIFTY_FIFTY);

                PlaceTile(backLayer, ChoseLichtungCornerTile(matrix, isHorizontal, goHorizontal, isSteep, goSteep), curLocation.X, curLocation.Y, PlaceMode.OVERRIDE);
                FillLichtungRow(curLocation, stopLocation, xDir, yDir);

                if (goSteep)
                {
                    curLocation.X += xDir + xDirInwards;
                    curLocation.Y += yDir + yDirInwards;
                }
                else if (goHorizontal)
                {
                    curLocation.X += xDir;
                    curLocation.Y += yDir;
                }
                else
                {
                    curLocation.X += xDirInwards;
                    curLocation.Y += yDirInwards;
                }

                isHorizontal = goHorizontal;
                isSteep = goSteep;
            }

            // Finish last tiles from curLocation to stop in x direction:
            while (Math.Abs((curLocation.X - stopLocation.X) * xDir) > 0 || Math.Abs((curLocation.Y - stopLocation.Y) * yDir) > 0)
            {
                PlaceTile(backLayer, ChoseLichtungCornerTile(matrix, isHorizontal, true, isSteep, false), curLocation.X, curLocation.Y, PlaceMode.OVERRIDE);
                FillLichtungRow(curLocation, stopLocation, xDir, yDir);

                curLocation.X += xDir;
                curLocation.Y += yDir;

                isHorizontal = true;
                isSteep = false;
            }

            // Finish last tiles from curLocation to stop in y direction:
            while (Math.Abs((curLocation.X - stopLocation.X) * xDirInwards) > 0 || Math.Abs((curLocation.Y - stopLocation.Y) * yDirInwards) > 0)
            {
                PlaceTile(backLayer, ChoseLichtungCornerTile(matrix, isHorizontal, false, isSteep, false), curLocation.X, curLocation.Y, PlaceMode.OVERRIDE);
                FillLichtungRow(curLocation, stopLocation, xDir, yDir);

                curLocation.X += xDirInwards;
                curLocation.Y += yDirInwards;

                isHorizontal = false;
                isSteep = false;
            }

            // Finish last tile at stop:
            PlaceTile(backLayer, ChoseLichtungCornerTile(matrix, isHorizontal, false, isSteep, false), curLocation.X, curLocation.Y, PlaceMode.OVERRIDE);
            FillLichtungRow(curLocation, stopLocation, xDir, yDir);
        }

        private void FillLichtungRow(Location curLocation, Location stopLocation, int xDir, int yDir)
        {
            int numTilesToFillIn = 1 + Math.Max(Math.Abs((stopLocation.X - curLocation.X) * xDir), Math.Abs((stopLocation.Y - curLocation.Y) * yDir));
            for (int i = 0; i <= numTilesToFillIn; i++)
            {
                PlaceTile(backLayer, GetRandomGrassTileIndex(GrassType.BRIGHT), curLocation.X + i * xDir, curLocation.Y + i * yDir);
            }
        }

        private int ChoseLichtungCornerTile(DeepWoodsLichtungTileMatrix matrix, bool isHorizontal, bool goHorizontal, bool isSteep, bool goSteep)
        {
            if (isSteep)
            {
                if (goHorizontal)
                    return matrix.STEEP_TO_HORIZONTAL;

                throw new ArgumentException("ChoseLichtungCornerTile: Invalid parameters, can't go from steep tile to anything but horizontal! (isHorizontal=" + isHorizontal + ", goHorizontal=" + goHorizontal + ", isSteep=" + isSteep + ", goSteep=" + goSteep + ")");
            }

            if (isHorizontal)
            {
                if (goHorizontal)
                    return matrix.HORIZONTAL;
                else if (goSteep)
                    throw new ArgumentException("ChoseLichtungCornerTile: Invalid parameters, can't go to steep tile from anything but vertical! (isHorizontal=" + isHorizontal + ", goHorizontal=" + goHorizontal + ", isSteep=" + isSteep + ", goSteep=" + goSteep + ")");
                else
                    return matrix.CONCAVE_CORNER;
            }
            else
            {
                if (goHorizontal)
                    return matrix.CONVEX_CORNER;
                else if (goSteep)
                    return matrix.VERTICAL_TO_STEEP;
                else
                    return matrix.VERTICAL;
            }
        }

        private Chance GetChanceForHorizontal(Location location1, Location location2, bool flipped)
        {
            // TODO: Is this inverted?
            int xDiff = Math.Abs(location1.X - location2.X);
            int yDiff = Math.Abs(location1.Y - location2.Y);
            int total = xDiff + yDiff;
            return new Chance(flipped ? yDiff : xDiff, total);
        }

        private bool PlaceTile(Layer layer, int[] tileIndices, Placing placing, int steps, int stepsInward, PlaceMode placeMode = PlaceMode.DONT_OVERRIDE)
        {
            return PlaceTile(layer, this.random.GetRandomValue(tileIndices), placing, steps, stepsInward, placeMode);
        }

        private bool PlaceTile(Layer layer, int tileIndex, Placing placing, int steps, int stepsInward, PlaceMode placeMode = PlaceMode.DONT_OVERRIDE)
        {
            int x = placing.location.X + (steps * placing.XDir) + (stepsInward * placing.XDirInward);
            int y = placing.location.Y + (steps * placing.YDir) + (stepsInward * placing.YDirInward);
            return PlaceTile(layer, tileIndex, x, y, placeMode);
        }

        private bool PlaceTile(Layer layer, int[] tileIndices, int x, int y, PlaceMode placeMode = PlaceMode.DONT_OVERRIDE)
        {
            return PlaceTile(layer, this.random.GetRandomValue(tileIndices), x, y, placeMode);
        }

        private bool PlaceTile(Layer layer, int tileIndex, int x, int y, PlaceMode placeMode = PlaceMode.DONT_OVERRIDE)
        {
            if (x < 0 || y < 0 || x >= this.spaceManager.GetMapWidth() || y >= this.spaceManager.GetMapHeight())
            {
                return false;
            }

            if (placeMode == PlaceMode.OVERRIDE || layer.Tiles[x, y] == null)
            {
                layer.Tiles[x, y] = new StaticTile(layer, defaultOutdoorTileSheet, BlendMode.Alpha, tileIndex);
                return true;
            }

            return false;
        }

        private bool PlaceAnimatedTile(Layer layer, TileSheet tileSheet, int[][] tileIndicesFrames, int x, int y, PlaceMode placeMode = PlaceMode.DONT_OVERRIDE, int frameInterval = 1000)
        {
            return PlaceAnimatedTile(layer, tileSheet, tileIndicesFrames[this.random.GetRandomValue(0, tileIndicesFrames.Length)], x, y, placeMode, frameInterval);
        }

        private bool PlaceAnimatedTile(Layer layer, TileSheet tileSheet, int[] tileIndexFrames, int x, int y, PlaceMode placeMode = PlaceMode.DONT_OVERRIDE, int frameInterval = 1000)
        {
            if (x < 0 || y < 0 || x >= this.spaceManager.GetMapWidth() || y >= this.spaceManager.GetMapHeight())
            {
                return false;
            }

            if (placeMode == PlaceMode.OVERRIDE || layer.Tiles[x, y] == null)
            {
                StaticTile[] frames = new List<int>(tileIndexFrames).ConvertAll<StaticTile>(tileIndex => new StaticTile(layer, tileSheet, BlendMode.Alpha, tileIndex)).ToArray();
                layer.Tiles[x, y] = new AnimatedTile(layer, frames, frameInterval);
                return true;
            }

            return false;
        }

        private bool ClearTile(Layer layer, int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.spaceManager.GetMapWidth() || y >= this.spaceManager.GetMapHeight())
            {
                return false;
            }

            layer.Tiles[x, y] = null;
            return true;
        }

        private void FillForestTile(int x, int y)
        {
            PlaceTile(buildingsLayer, PLAIN_FOREST_BACKGROUND, x, y);
            PlaceTile(alwaysFrontLayer, GetRandomForestFillerTileIndex(), x, y);
        }

        private int GetRandomForestFillerTileIndex()
        {
            return this.random.GetRandomValue(FOREST_BACKGROUND);
        }

        private void GenerateForestPatch(xTile.Dimensions.Rectangle rectangle)
        {
            int offset = Settings.Map.MaxBumpSizeForForestBorder + 1;

            GenerateForestRow(
                new Placing(new Location(rectangle.X + offset + 1, rectangle.Y + offset), PlacingDirection.DOWNWARDS, PlacingDirection.LEFTWARDS),
                rectangle.Height - offset * 2,
                DeepWoodsRowTileMatrix.RIGHT,
                0);

            GenerateForestRow(
                new Placing(new Location(rectangle.X + rectangle.Width - offset - 1, rectangle.Y + offset), PlacingDirection.DOWNWARDS, PlacingDirection.RIGHTWARDS),
                rectangle.Height - offset * 2,
                DeepWoodsRowTileMatrix.LEFT,
                0);

            GenerateForestRow(
                new Placing(new Location(rectangle.X + offset, rectangle.Y + offset + 1), PlacingDirection.RIGHTWARDS, PlacingDirection.UPWARDS),
                rectangle.Width - offset * 2,
                DeepWoodsRowTileMatrix.BOTTOM,
                0);

            GenerateForestRow(
                new Placing(new Location(rectangle.X + offset, rectangle.Y + rectangle.Height - offset - 1), PlacingDirection.RIGHTWARDS, PlacingDirection.DOWNWARDS),
                rectangle.Width - offset * 2,
                DeepWoodsRowTileMatrix.TOP,
                0,
                true);

            PlaceTile(backLayer, DeepWoodsRowTileMatrix.BOTTOM.DARK_GRASS_RIGHT_CONVEX_CORNER, rectangle.X + 2, rectangle.Y + 2);
            PlaceTile(backLayer, DeepWoodsRowTileMatrix.TOP.DARK_GRASS_RIGHT_CONVEX_CORNER, rectangle.X + 2, rectangle.Y + rectangle.Height - 3);
            PlaceTile(backLayer, DeepWoodsRowTileMatrix.BOTTOM.DARK_GRASS_LEFT_CONVEX_CORNER, rectangle.X + rectangle.Width - 3, rectangle.Y + 2);
            PlaceTile(backLayer, DeepWoodsRowTileMatrix.TOP.DARK_GRASS_LEFT_CONVEX_CORNER, rectangle.X + rectangle.Width - 3, rectangle.Y + rectangle.Height - 3);

            ClearTile(buildingsLayer, rectangle.X + 3, rectangle.Y + 3);
            ClearTile(buildingsLayer, rectangle.X + 3, rectangle.Y + rectangle.Height - 4);
            ClearTile(buildingsLayer, rectangle.X + rectangle.Width - 4, rectangle.Y + 3);
            ClearTile(buildingsLayer, rectangle.X + rectangle.Width - 4, rectangle.Y + rectangle.Height - 4);

            int minFillX = rectangle.X + offset + 2;
            int maxFillX = rectangle.X + rectangle.Width - offset - 1;
            int minFillY = rectangle.Y + offset + 2;
            int maxFillY = rectangle.Y + rectangle.Height - offset - 1;

            int numFillTiles = 0;
            for (int x = minFillX; x < maxFillX; x++)
            {
                for (int y = minFillY; y < maxFillY; y++)
                {
                    FillForestTile(x, y);
                    numFillTiles++;
                }
            }

            int maxLightSources = Math.Max(1, numFillTiles / NUM_TILES_PER_LIGHTSOURCE);
            int numLightSources = 1 + this.random.GetRandomValue(0, maxLightSources);
            for (int i = 0; i < numLightSources; i++)
            {
                deepWoods.lightSources.Add(new Vector2(this.random.GetRandomValue(minFillX, maxFillX + 1), this.random.GetRandomValue(minFillY, maxFillY + 1)));
            }
        }

        private void TryGenerateForestPatch(Location location)
        {
            int wishWidth = this.random.GetRandomValue(Settings.Map.MinSizeForForestPatch, Settings.Map.MaxSizeForForestPatch);
            int wishHeight = this.random.GetRandomValue(Settings.Map.MinSizeForForestPatch, Settings.Map.MaxSizeForForestPatch);

            xTile.Dimensions.Rectangle rectangle;
            if (this.spaceManager.TryGetFreeRectangleForForestPatch(location, wishWidth, wishHeight, out rectangle))
            {
                GenerateForestPatch(rectangle);
            }
        }

        private void GenerateForestPatches()
        {
            int mapWidth = this.spaceManager.GetMapWidth();
            int mapHeight = this.spaceManager.GetMapHeight();

            int numForestPatches;

            if (mapWidth > Settings.Map.ForestPatchCenterMinDistanceToMapBorder * 2 && mapHeight > Settings.Map.ForestPatchCenterMinDistanceToMapBorder * 2)
            {
                // Calculate maximum theoretical amount of forest patches for the current map.
                int maxForestPatches = (mapWidth * mapHeight) / Settings.Map.MinimumTilesForForestPatch;

                // Get a random value from 0 to maxForestPatches, using a "two dice" method,
                // where the center numbers are more likely than the edges.
                numForestPatches =
                    this.random.GetRandomValue(0, maxForestPatches / 2)
                    + this.random.GetRandomValue(0, maxForestPatches / 2);
            }
            else
            {
                numForestPatches = this.random.GetRandomValue(0, 1);
            }

            // Try to generate forest patches at random positions.
            // Some of these may not generate anything due to overlaps, that's by design.
            for (int i = 0; i < numForestPatches; i++)
            {
                int x = this.random.GetRandomValue(Settings.Map.ForestPatchCenterMinDistanceToMapBorder, mapWidth - Settings.Map.ForestPatchCenterMinDistanceToMapBorder);
                int y = this.random.GetRandomValue(Settings.Map.ForestPatchCenterMinDistanceToMapBorder, mapHeight - Settings.Map.ForestPatchCenterMinDistanceToMapBorder);
                TryGenerateForestPatch(new Location(x, y));
            }
        }

    }
}
