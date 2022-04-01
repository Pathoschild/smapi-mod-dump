/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ProceduralDungeons
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        public static void ExtendMap(ref Map map, int w, int h)
        {
            List<Layer> layers = AccessTools.FieldRefAccess<Map, List<Layer>>(map, "m_layers");
            for (int i = 0; i < layers.Count; i++)
            {
                Tile[,] tiles = AccessTools.Field(typeof(Layer), "m_tiles").GetValue(layers[i]) as Tile[,];
                Size size = (Size)AccessTools.Field(typeof(Layer), "m_layerSize").GetValue(layers[i]);
                if (tiles.GetLength(0) >= w && tiles.GetLength(1) >= h)
                    continue;

                w = Math.Max(w, tiles.GetLength(0));
                h = Math.Max(h, tiles.GetLength(1));

                SMonitor.Log($"Extending layer {layers[i].Id} from {size.Width},{size.Height} ({tiles.GetLength(0)},{tiles.GetLength(1)}) to {w},{h}");

                size = new Size(w, h);
                AccessTools.FieldRefAccess<Layer, Size>(layers[i], "m_layerSize") = size;
                AccessTools.FieldRefAccess<Map, List<Layer>>(map, "m_layers") = layers;

                Tile[,] newTiles = new Tile[w, h];

                for (int k = 0; k < tiles.GetLength(0); k++)
                {
                    for (int l = 0; l < tiles.GetLength(1); l++)
                    {
                        newTiles[k, l] = tiles[k, l];
                    }
                }
                AccessTools.FieldRefAccess<Layer, Tile[,]>(layers[i], "m_tiles") = newTiles;
                AccessTools.FieldRefAccess<Layer, TileArray>(layers[i], "m_tileArray") = new TileArray(layers[i], newTiles);

            }
            AccessTools.FieldRefAccess<Map, List<Layer>>(map, "m_layers") = layers;
        }
        internal static Map GetRandomMap()
        {

            Map map = SHelper.Content.Load<Map>("assets/base.tmx");
            ExtendMap(ref map, Config.MapWidth, Config.MapHeight);
            TileSheet sheet = map.TileSheets[0];
            int maxRooms = 100 / Config.RoomSizeFactor;
            int rooms = Game1.random.Next(maxRooms * Config.RoomAmountFactor / 100, maxRooms);
            double coeff = Math.Sqrt(Config.MapWidth * Config.MapHeight / (float)rooms / 144f);
            SMonitor.Log($"making {rooms} rooms, max {coeff * 16}x{coeff * 9}");
            Rectangle bounds = new Rectangle((int)(coeff * 16 / 8f), (int)(coeff * 9 / 8f), mapWidth - (int)(coeff * 16 / 4f), mapHeight - (int)(coeff * 9 / 4f));

            List<Point> points = new List<Point>();

            // prefill with ocean tiles

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Point p = new Point(x, y);
                    if (bounds.Contains(p))
                        points.Add(p);
                    location.map.GetLayer("Back").Tiles[x, y] = new StaticTile(location.map.GetLayer("Back"), sheet, BlendMode.Alpha, GetRandomOceanTile());
                }
            }
            SMonitor.Log($"got {points.Count} points");

            // determine island no and sizes, positions


            List<Rectangle> isleBoxes = new List<Rectangle>();
            for (int i = 0; i < rooms; i++)
            {
                List<Point> freePoints = new List<Point>();
                int width = Game1.random.Next((int)(coeff * 16 / 2f), (int)(coeff * 16 * 3 / 4f));
                int height = Game1.random.Next((int)(coeff * 9 / 2f), (int)(coeff * 9 * 3 / 4f));

                Monitor.Log($"trying to make island of size {width}x{height}");
                Rectangle bounds2 = new Rectangle(width / 2, height / 2, mapWidth - width, mapHeight - height);
                for (int j = points.Count - 1; j >= 0; j--)
                {
                    if (bounds2.Contains(points[j]))
                    {
                        Rectangle testRect = new Rectangle(points[j].X - width / 2, points[j].Y - height / 2, width, height);
                        bool free = true;
                        foreach (Rectangle r in isleBoxes)
                        {
                            if (r.Intersects(testRect))
                            {
                                free = false;
                                break;
                            }
                        }
                        if (free)
                        {
                            freePoints.Add(points[j]);
                        }
                    }
                }
                if (!freePoints.Any())
                    continue;
                Point randPoint = freePoints[Game1.random.Next(freePoints.Count)];
                Rectangle isleBox = new Rectangle(randPoint.X - width / 2, randPoint.Y - height / 2, width, height);
                isleBoxes.Add(isleBox);
            }
            Monitor.Log($"got {isleBoxes.Count} island boxes");

            // build islands

            foreach (Rectangle isleBox in isleBoxes)
            {

                // preset each tile to land

                bool[] landTiles = new bool[isleBox.Width * isleBox.Height];
                for (int i = 0; i < isleBox.Width * isleBox.Height; i++)
                    landTiles[i] = true;

                // randomly shape island

                for (int x = 0; x < isleBox.Width; x++)
                {
                    for (int y = 0; y < isleBox.Height; y++)
                    {
                        int idx = y * isleBox.Width + x;
                        double land = 1;
                        if (x == 0 || x == isleBox.Width - 1 || y == 0 || y == isleBox.Height - 1)
                        {
                            landTiles[idx] = false;
                            continue;
                        }

                        float widthOffset = Math.Abs(isleBox.Width / 2f - x) / (isleBox.Width / 2f);
                        float heightOffset = Math.Abs(isleBox.Height / 2f - y) / (isleBox.Height / 2f);
                        double distance = Math.Sqrt(Math.Pow(widthOffset, 2) + Math.Pow(heightOffset, 2));

                        if (idx > 0 && !landTiles[idx - 1])
                            land -= 0.1;
                        if (idx > isleBox.Width - 1 && !landTiles[idx - isleBox.Width])
                            land -= 0.1;
                        if (idx < landTiles.Length - 1 && !landTiles[idx + 1])
                            land -= 0.1;
                        if (idx < landTiles.Length - isleBox.Width - 1 && !landTiles[idx + isleBox.Width])
                            land -= 0.1;

                        land -= distance;
                        landTiles[idx] = Game1.random.NextDouble() < land;
                    }
                }

                // smoothen

                bool changed = true;
                while (changed)
                {
                    changed = false;
                    for (int x = 0; x < isleBox.Width; x++)
                    {
                        for (int y = 0; y < isleBox.Height; y++)
                        {
                            int idx = y * isleBox.Width + x;
                            if (!landTiles[idx])
                            {
                                bool[] surround = GetSurroundingTiles(isleBox, landTiles, idx);
                                if ((surround[0] && surround[7]) || (surround[2] && surround[5])
                                    || (surround[1] && (surround[5] || surround[6] || surround[7]))
                                    || (surround[3] && (surround[2] || surround[4] || surround[7]))
                                    || (surround[4] && (surround[0] || surround[3] || surround[5]))
                                    || (surround[6] && (surround[0] || surround[1] || surround[2]))
                                )
                                {
                                    if (Game1.random.NextDouble() < 0.85)
                                    {
                                        landTiles[idx] = true;
                                    }
                                    else
                                    {
                                        landTiles[idx - 1 - isleBox.Width] = false;
                                        landTiles[idx - isleBox.Width] = false;
                                        landTiles[idx + 1 - isleBox.Width] = false;
                                        landTiles[idx - 1] = false;
                                        landTiles[idx + 1] = false;
                                        landTiles[idx - 1 + isleBox.Width] = false;
                                        landTiles[idx + isleBox.Width] = false;
                                        landTiles[idx + 1 + isleBox.Width] = false;
                                    }
                                    changed = true;
                                }
                            }
                        }
                    }
                }

                // add land and border tiles

                for (int x = 0; x < isleBox.Width; x++)
                {
                    for (int y = 0; y < isleBox.Height; y++)
                    {
                        int idx = y * isleBox.Width + x;
                        bool[] surround = GetSurroundingTiles(isleBox, landTiles, idx);

                        if (landTiles[idx])
                        {
                            if (surround.Where(b => b).Count() == 8)
                            {
                                location.map.GetLayer("Back").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Back"), sheet, BlendMode.Alpha, GetRandomLandTile());
                            }
                            else
                            {
                                if (!surround[1])
                                {
                                    if (!surround[3])
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 51);
                                    }
                                    else if (!surround[4])
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 54);
                                    }
                                    else if (location.map.GetLayer("Buildings").Tiles[isleBox.X + x - 1, isleBox.Y + y]?.TileIndex == 52)
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 53);
                                    }
                                    else
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 52);
                                    }
                                }
                                else if (!surround[6])
                                {
                                    if (!surround[3])
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 51);
                                    }
                                    else if (!surround[4])
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 54);
                                    }
                                    else if (location.map.GetLayer("Buildings").Tiles[isleBox.X + x - 1, isleBox.Y + y]?.TileIndex == 52)
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 53);
                                    }
                                    else
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 52);
                                    }
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties.Add("@Flip", 2);
                                }
                                else if (!surround[3])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 68);
                                }
                                else if (!surround[4])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 71);
                                }
                                else if (!surround[0])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 70);
                                }
                                else if (!surround[2])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 69);
                                }
                                else if (!surround[5])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 87);
                                }
                                else if (!surround[7])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, 86);
                                }
                            }
                        }
                        else
                        {
                            if (surround.Where(b => b).Any())
                            {
                                if (surround[1])
                                {
                                    if (surround[3])
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 226);
                                    else if (surround[4])
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 175);
                                    else
                                    {
                                        if (location.map.GetLayer("Buildings").Tiles[isleBox.X + x - 1, isleBox.Y + y]?.TileIndex == 141)
                                            location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 158);
                                        else
                                            location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 141);
                                    }
                                }
                                else if (surround[6])
                                {
                                    if (surround[3])
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 226);
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties.Add("@Flip", 2);
                                    }
                                    else if (surround[4])
                                    {
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 175);
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties.Add("@Flip", 2);
                                    }
                                    else
                                    {
                                        if (location.map.GetLayer("Buildings").Tiles[isleBox.X + x - 1, isleBox.Y + y]?.TileIndex == 141)
                                            location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 158);
                                        else
                                            location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 141);
                                        location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties.Add("@Flip", 2);
                                    }
                                }
                                else if (surround[3])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 260);
                                }
                                else if (surround[4])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 209);
                                }
                                else if (surround[0])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 243);
                                }
                                else if (surround[2])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 192);
                                }
                                else if (surround[5])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 243);
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties.Add("@Flip", 2);
                                }
                                else if (surround[7])
                                {
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y] = CreateAnimatedTile(location, sheet, 192);
                                    location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties.Add("@Flip", 2);
                                }
                                location.map.GetLayer("Buildings").Tiles[isleBox.X + x, isleBox.Y + y].Properties["Passable"] = "T";
                                //location.map.GetLayer("Back").Tiles[isleBox.X + x, isleBox.Y + y].TileIndexProperties["NPCBarrier"] = "t";
                            }
                        }
                    }
                }
            }

            // add water tiles

            location.waterTiles = new bool[location.map.Layers[0].LayerWidth, location.map.Layers[0].LayerHeight];
            bool foundAnyWater = false;
            for (int x = 0; x < location.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                {
                    if (location.doesTileHaveProperty(x, y, "Water", "Back") != null)
                    {
                        foundAnyWater = true;
                        location.waterTiles[x, y] = true;
                    }
                }
            }
            if (!foundAnyWater)
            {
                location.waterTiles = null;
            }

            // add terrain features

            foreach (Rectangle isleBox in isleBoxes)
            {
                // get free spots

                List<Vector2> freeSpots = new List<Vector2>();
                List<Vector2> freeCenters = new List<Vector2>();
                for (int x = isleBox.X; x < isleBox.Right; x++)
                {
                    for (int y = isleBox.Y; y < isleBox.Bottom; y++)
                    {
                        if (location.doesTileHaveProperty(x, y, "Water", "Back") == null)
                        {
                            freeSpots.Add(new Vector2(x, y));
                            if (
                                location.doesTileHaveProperty(x - 1, y - 1, "Water", "Back") == null
                                && location.doesTileHaveProperty(x, y - 1, "Water", "Back") == null
                                && location.doesTileHaveProperty(x + 1, y - 1, "Water", "Back") == null
                                && location.doesTileHaveProperty(x - 1, y, "Water", "Back") == null
                                && location.doesTileHaveProperty(x + 1, y, "Water", "Back") == null
                                && location.doesTileHaveProperty(x - 1, y + 1, "Water", "Back") == null
                                && location.doesTileHaveProperty(x, y + 1, "Water", "Back") == null
                                && location.doesTileHaveProperty(x + 1, y + 1, "Water", "Back") == null
                            )
                            {
                                freeCenters.Add(new Vector2(x, y));
                            }
                        }
                    }
                }
                Monitor.Log($"Island has {freeSpots.Count} free spots, {freeCenters.Count} centers");

                List<Vector2> randFreeSpots = new List<Vector2>(freeSpots);

                int n = randFreeSpots.Count;
                while (n > 1)
                {
                    n--;
                    int k = Game1.random.Next(n + 1);
                    var value = randFreeSpots[k];
                    randFreeSpots[k] = randFreeSpots[n];
                    randFreeSpots[n] = value;
                }

                List<Vector2> randFreeCenters = new List<Vector2>(freeCenters);

                n = randFreeCenters.Count;
                while (n > 1)
                {
                    n--;
                    int k = Game1.random.Next(n + 1);
                    var value = randFreeCenters[k];
                    randFreeCenters[k] = randFreeCenters[n];
                    randFreeCenters[n] = value;
                }


                int taken = 0;
                if (Game1.random.NextDouble() < Config.TreasureChance)
                {
                    Monitor.Log($"Island has treasure chest");
                    location.overlayObjects[randFreeSpots[taken]] = new Chest(0, new List<Item>() { MineShaft.getTreasureRoomItem() }, randFreeSpots[taken], false, 0);
                    taken++;
                }
                if (Game1.random.NextDouble() < Config.TreesChance)
                {
                    Monitor.Log($"Island has trees");
                    int trees = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.TreesPortion)));
                    for (int i = 0; i < trees; i++)
                    {
                        location.terrainFeatures.Add(randFreeSpots[i + taken], new Tree(6, 5));
                    }
                    taken += trees;
                    if (Game1.random.NextDouble() < Config.CoconutChance)
                    {
                        Monitor.Log($"Island has coconuts");
                        int nuts = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.CoconutPortion)));
                        for (int i = 0; i < nuts; i++)
                        {
                            Vector2 position = randFreeSpots[i + taken];
                            location.dropObject(new Object(88, 1, false, -1, 0), position * 64f, Game1.viewport, true, null);
                        }
                        taken += nuts;
                    }
                }
                if (Game1.random.NextDouble() < Config.FaunaChance)
                {
                    Monitor.Log($"Island has fauna");
                    int fauna = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.FaunaPortion)));
                    for (int i = 0; i < fauna; i++)
                    {
                        Vector2 position = randFreeSpots[i + taken];
                        int index = 393;
                        if (Game1.random.NextDouble() < 0.2)
                        {
                            index = 397;
                        }
                        else if (Game1.random.NextDouble() < 0.1)
                        {
                            index = 152;
                        }
                        location.dropObject(new Object(index, 1, false, -1, 0), position * 64f, Game1.viewport, true, null);
                    }
                    taken += fauna;
                }
                if (Game1.random.NextDouble() < Config.ArtifactChance)
                {
                    Monitor.Log($"Island has artifacts");
                    int artifacts = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.ArtifactPortion)));
                    for (int i = 0; i < artifacts; i++)
                    {
                        Vector2 position = randFreeSpots[i + taken];
                        location.objects.Add(position, new Object(position, 590, 1));
                    }
                    taken += artifacts;
                }
                if (Game1.random.NextDouble() < Config.MonsterChance)
                {
                    int monsters = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.MonsterPortion)));
                    double type = Game1.random.NextDouble();
                    if (type < 0.2)
                    {
                        Monitor.Log($"Island has skeletons");
                    }
                    else if (type < 0.3)
                    {
                        Monitor.Log($"Island has dinos");
                    }
                    else if (type < 0.5)
                    {
                        Monitor.Log($"Island has golems");
                    }
                    else
                    {
                        Monitor.Log($"Island has crabs");
                    }
                    for (int i = 0; i < monsters; i++)
                    {
                        Vector2 position = randFreeSpots[i + taken];
                        if (type < 0.2)
                        {
                            location.characters.Add(new Skeleton(position * Game1.tileSize));
                        }
                        else if (type < 0.3)
                        {
                            location.characters.Add(new DinoMonster(position * Game1.tileSize));
                        }
                        else if (type < 0.5)
                        {
                            location.characters.Add(new RockGolem(position * Game1.tileSize, 10));
                        }
                        else
                        {
                            location.characters.Add(new RockCrab(position * Game1.tileSize));
                        }
                    }
                    taken += monsters;
                }
                if (Game1.random.NextDouble() < Config.GrassChance)
                {
                    Monitor.Log($"Island has grass");
                    int grass = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.GrassPortion)));
                    for (int i = 0; i < grass; i++)
                    {
                        location.terrainFeatures.Add(randFreeSpots[i + taken], new Grass(Game1.random.Next(2, 5), Game1.random.Next(1, 3)));
                    }
                    taken += grass;
                }
                if (Game1.random.NextDouble() < Config.MineralChance)
                {
                    Monitor.Log($"Island has minerals");
                    int minerals = Math.Min(freeSpots.Count - taken, (int)(freeSpots.Count * Math.Min(1, Config.MineralPortion)));
                    for (int i = 0; i < minerals; i++)
                    {
                        Vector2 position = randFreeSpots[i + taken];
                        if (Game1.random.NextDouble() < 0.06)
                        {
                            location.terrainFeatures.Add(position, new Tree(1 + Game1.random.Next(2), 1));
                        }
                        else if (Game1.random.NextDouble() < 0.02)
                        {
                            if (Game1.random.NextDouble() < 0.1)
                            {
                                location.objects.Add(position, new Object(position, 46, "Stone", true, false, false, false)
                                {
                                    MinutesUntilReady = 12
                                });
                            }
                            else
                            {
                                location.objects.Add(position, new Object(position, (Game1.random.Next(7) + 1) * 2, "Stone", true, false, false, false)
                                {
                                    MinutesUntilReady = 5
                                });
                            }
                        }
                        else if (Game1.random.NextDouble() < 0.1)
                        {
                            if (Game1.random.NextDouble() < 0.001)
                            {
                                location.objects.Add(position, new Object(position, 765, 1)
                                {
                                    MinutesUntilReady = 16
                                });
                            }
                            else if (Game1.random.NextDouble() < 0.1)
                            {
                                location.objects.Add(position, new Object(position, 764, 1)
                                {
                                    MinutesUntilReady = 8
                                });
                            }
                            else if (Game1.random.NextDouble() < 0.33)
                            {
                                location.objects.Add(position, new Object(position, 290, 1)
                                {
                                    MinutesUntilReady = 5
                                });
                            }
                            else
                            {
                                location.objects.Add(position, new Object(position, 751, 1)
                                {
                                    MinutesUntilReady = 3
                                });
                            }
                        }
                        else
                        {
                            Object obj = new Object(position, (Game1.random.NextDouble() < 0.25) ? 32 : ((Game1.random.NextDouble() < 0.33) ? 38 : ((Game1.random.NextDouble() < 0.5) ? 40 : 42)), 1);
                            obj.minutesUntilReady.Value = 2;
                            obj.Name = "Stone";
                            location.objects.Add(position, obj);
                        }
                    }
                    taken += minerals;
                }
            }
        }

        private static AnimatedTile CreateAnimatedTile(GameLocation location, TileSheet sheet, int first)
        {
            return new AnimatedTile(location.map.GetLayer("Buildings"), new StaticTile[] {
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 1),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 2),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 3),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 3),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 4),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 5),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first + 6),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first),
                new StaticTile(location.map.GetLayer("Buildings"), sheet, BlendMode.Alpha, first),
            }, 250);
        }

        private static bool[] GetSurroundingTiles(Rectangle isleBox, bool[] landTiles, int idx)
        {
            bool[] list = new bool[8];
            if (idx >= isleBox.Width)
            {
                if (idx % isleBox.Width > 0)
                    list[0] = landTiles[idx - 1 - isleBox.Width];
                list[1] = landTiles[idx - isleBox.Width];
                if (idx % isleBox.Width < isleBox.Width - 1)
                    list[2] = landTiles[idx + 1 - isleBox.Width];
            }
            if (idx % isleBox.Width > 0)
                list[3] = landTiles[idx - 1];
            if (idx % isleBox.Width < isleBox.Width - 1)
                list[4] = landTiles[idx + 1];
            if (idx < landTiles.Length - isleBox.Width)
            {
                if (idx % isleBox.Width > 0)
                    list[5] = landTiles[idx - 1 + isleBox.Width];
                list[6] = landTiles[idx + isleBox.Width];
                if (idx % isleBox.Width < isleBox.Width - 1)
                    list[7] = landTiles[idx + 1 + isleBox.Width];
            }
            return list;
        }

        private static int GetRandomOceanTile()
        {
            double d = Game1.random.NextDouble();
            int[] tiles = new int[]
            {
                458,
                185,
                475,
                130
            };
            double chance = 0.02;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (d < chance * (i + 1))
                    return tiles[i];
            }

            return 75;
        }
        private static int GetRandomLandTile()
        {
            double d = Game1.random.NextDouble();
            int[] tiles = new int[]
            {
                18,
                168,
                25,
                43
            };
            double chance = 0.02;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (d < chance * (i + 1))
                    return tiles[i];
            }

            if (d < 0.02)
                return 18;
            if (d < 0.04)
                return 168;
            if (d < 0.06)
                return 25;
            if (d < 0.08)
                return 43;

            return 42;
        }
    }
}