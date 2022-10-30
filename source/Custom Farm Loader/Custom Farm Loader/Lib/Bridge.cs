/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using xTile.Tiles;
using xTile.Layers;
using xTile.ObjectModel;

namespace Custom_Farm_Loader.Lib
{
    public class Bridge
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public const string BridgesTilesheet = "CFL_Bridges";
        public static List<Rectangle> PassableBridgeAreas = new List<Rectangle>();

        public string Id = "";
        public BridgeType Type;
        public Rotation Rotation;
        public Point Position = new Point(0, 0);
        public int Length;
        public int Price;
        public string Item;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public static List<Bridge> parseBridgeJObject(JProperty jObj)
        {
            List<Bridge> bridges = new List<Bridge>();

            foreach (JProperty jProperty in jObj.First()) {
                if (jProperty.Value.Type == JTokenType.Null)
                    continue;

                Bridge bridge = new Bridge();
                bridge.Id = jProperty.Name;

                foreach (JProperty jProperty2 in jProperty.Value) {
                    if (jProperty.Value.Type == JTokenType.Null)
                        continue;

                    string name = jProperty2.Name;
                    string value = jProperty2.Value.ToString();


                    switch (name.ToLower()) {
                        case "type":
                            bridge.Type = UtilityMisc.parseEnum<BridgeType>(value); break;
                        case "rotation":
                            bridge.Rotation = UtilityMisc.parseEnum<Rotation>(value); break;
                        case "position":
                            bridge.Position = new Point(int.Parse(value.Split(",")[0]), int.Parse(value.Split(",")[1])); break;
                        case "length":
                            bridge.Length = (int)uint.Parse(value); break;
                        case "price":
                            bridge.Price = int.Parse(value); break;
                        case "item":
                            bridge.Item = ItemObject.MapNameToParentsheetindex(value); break;
                        default:
                            Monitor.Log($"Unknown Bridges Attribute at '{bridge.Id}'", LogLevel.Error);
                            throw new ArgumentException($"Unknown Bridges Attribute at '{bridge.Id}'", name);
                    }
                }

                bridges.Add(bridge);
            }

            return bridges;
        }


        public static void addBridgesTilesheet(GameLocation location)
        {
            if (location.map.TileSheets.ToList().Exists(el => el.ImageSource == BridgesTilesheet))
                return;

            TileSheet tileSheet = new TileSheet(location.map, BridgesTilesheet, new xTile.Dimensions.Size(20, 4), new xTile.Dimensions.Size(16));
            location.map.AddTileSheet(tileSheet);
            return;
        }


        private Point pos;
        private TileSheet tilesheet;
        private Layer buildingsLayer;
        private Layer bridgeLayer;
        private Layer backLayer;
        public void setTiles(GameLocation location)
        {
            pos = Position;
            tilesheet = location.map.TileSheets.ToList().Find(el => el.ImageSource == BridgesTilesheet);
            backLayer = location.map.GetLayer("Back");
            buildingsLayer = location.map.GetLayer("Buildings");
            if (Helper.ModRegistry.IsLoaded("aedenthorn.ExtraMapLayers")) {
                if (location.map.GetLayer("Buildings9") == null)
                    location.map.AddLayer(new Layer("Buildings9", location.map, buildingsLayer.LayerSize, buildingsLayer.TileSize));
                bridgeLayer = location.map.GetLayer("Buildings9");
            } else {
                Monitor.LogOnce("ExtraMapLayers not detected. Please install Aedenthorns ExtraMapLayers Mod to fix bridge sprites", LogLevel.Info);
                bridgeLayer = buildingsLayer;
            }


            switch (Type) {
                case BridgeType.Planks:
                    setTilesPlanks(location, new[] { 0, 20, 40, 60 });
                    setImpassableTiles(1);
                    break;
                case BridgeType.Wide_Planks:
                    setTilesPlanks(location, new[] { 0, 20, 40, 60 }, new[] { 1, 21, 41, 61 });
                    setImpassableTiles(2);
                    break;
            }
        }

        private void setImpassableTiles(int width)
        {
            //From all 4 corners of the bridge go one tile outwards and from the first impassable tile towards the middle
            //start setting all back tiles as impassable
            Point[] corners = new Point[3];
            if (Rotation == Rotation.Vertical)
                corners = new Point[] { new(pos.X - 1, pos.Y), new(pos.X + width, pos.Y), new(pos.X - 1, pos.Y + Length + 1), new(pos.X + width, pos.Y + Length + 1) };
            else if (Rotation == Rotation.Horizontal)
                corners = new Point[] { new(pos.X, pos.Y - 1), new(pos.X, pos.Y + width), new(pos.X + Length + 1, pos.Y - 1), new(pos.X + Length + 1, pos.Y + width) };

            setImpassableTiles(corners[0], true);
            setImpassableTiles(corners[1], true);
            setImpassableTiles(corners[2], false);
            setImpassableTiles(corners[3], false);
        }

        private void setImpassableTiles(Point start, bool forward)
        {
            Tile tile;
            Tile backTile;
            var foundImpassable = false;
            for (int i = 0; i < ((Length + 2) / 2) + 1; i++) {

                if (forward) {
                    tile = Rotation == Rotation.Vertical ? buildingsLayer.Tiles[start.X, start.Y + i] : buildingsLayer.Tiles[start.X + i, start.Y];
                    backTile = Rotation == Rotation.Vertical ? backLayer.Tiles[start.X, start.Y + i] : backLayer.Tiles[start.X + i, start.Y];
                } else {
                    tile = Rotation == Rotation.Vertical ? buildingsLayer.Tiles[start.X, start.Y - i] : buildingsLayer.Tiles[start.X - i, start.Y];
                    backTile = Rotation == Rotation.Vertical ? backLayer.Tiles[start.X, start.Y - i] : backLayer.Tiles[start.X - i, start.Y];
                }

                if (!foundImpassable)
                    if (containsPassable(backTile) || (tile != null && !containsPassable(tile)))
                        foundImpassable = true;


                if (foundImpassable && backTile != null && !backTile.Properties.ContainsKey("Passable"))
                    backTile.Properties.Add(new KeyValuePair<string, PropertyValue>("Passable", "T"));
            }
        }

        private bool containsPassable(Tile tile)
        {
            if (tile == null)
                return false;

            return tile.Properties.ContainsKey("Passable") || tile.TileIndexProperties.ContainsKey("Passable");
        }


        private void setTilesPlanks(GameLocation location, int[] left, int[] right = null)
        {
            if (Rotation == Rotation.Vertical)
                PassableBridgeAreas.Add(new Rectangle(pos.X * 64, pos.Y * 64, 2 * 64, ((int)Length + 2) * 64));
            else if (Rotation == Rotation.Horizontal)
                PassableBridgeAreas.Add(new Rectangle(pos.X * 64, pos.Y * 64, ((int)Length + 2) * 64, 2 * 64));

            //For bridges that are 2 tiles wide. Sets the second tile depending on rotation
            var width = Rotation == Rotation.Vertical ? 1 : 0;
            var height = Rotation == Rotation.Horizontal ? 1 : 0;

            for (int i = 0; i < Length + 2; i++) {
                //Center tile increment
                var xIncr = Rotation == Rotation.Vertical ? 0 : i;
                var yIncr = Rotation == Rotation.Horizontal ? 0 : i;

                if (i == 0) {
                    createTile(pos.X, pos.Y, left[0]);
                    if (right != null) createTile(pos.X + width, pos.Y + height, right[0]);

                } else if (i == Length + 1) {
                    createTile(pos.X + xIncr, pos.Y + yIncr, left[3]);
                    if (right != null) createTile(pos.X + xIncr + width, pos.Y + yIncr + height, right[3]);

                } else {
                    if (i % 2 == 0) {
                        createTile(pos.X + xIncr, pos.Y + yIncr, left[1]);
                        if (right != null) createTile(pos.X + xIncr + width, pos.Y + yIncr + height, right[1]);

                    } else {
                        createTile(pos.X + xIncr, pos.Y + yIncr, left[2]);
                        if (right != null) createTile(pos.X + xIncr + width, pos.Y + yIncr + height, right[2]);

                    }

                }

            }
        }

        private void createTile(int x, int y, int index)
        {
            var buildingsTile = buildingsLayer.Tiles[x, y] ?? new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, 99999);

            if (!buildingsTile.Properties.ContainsKey("Passable"))
                buildingsTile.Properties.Add(new KeyValuePair<string, PropertyValue>("Passable", "T"));
            buildingsLayer.Tiles[x, y] = buildingsTile;


            var tile = new StaticTile(bridgeLayer, tilesheet, BlendMode.Alpha, index);
            tile.Properties.Add(new KeyValuePair<string, PropertyValue>("Passable", "T"));

            if (Rotation == Rotation.Horizontal) {
                tile.Properties.Add(new KeyValuePair<string, PropertyValue>("@Rotation", "90"));
                tile.Properties.Add(new KeyValuePair<string, PropertyValue>("@Flip", ((int)SpriteEffects.FlipVertically).ToString()));
            }
            bridgeLayer.Tiles[x, y] = tile;


            if (backLayer.Tiles[x, y] != null && !backLayer.Tiles[x, y].Properties.ContainsKey("NoSpawn"))
                buildingsLayer.Tiles[x, y].Properties.Add(new KeyValuePair<string, PropertyValue>("NoSpawn", "All"));
        }
    }
}
