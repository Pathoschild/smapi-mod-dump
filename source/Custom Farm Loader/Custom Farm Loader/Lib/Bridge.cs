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
        public uint Length;
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
                            bridge.Length = uint.Parse(value); break;
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

            TileSheet tileSheet = new TileSheet(location.map, BridgesTilesheet, new xTile.Dimensions.Size(2, 4), new xTile.Dimensions.Size(16));
            tileSheet.Id = BridgesTilesheet;
            location.map.AddTileSheet(tileSheet);
            return;
        }

        public void setTiles(GameLocation location)
        {
            switch (Type) {
                case BridgeType.Planks:
                    break;
                case BridgeType.Wide_Planks:
                    setTilesWidePlanks(location);
                    break;
            }
        }

        private Point pos;
        private TileSheet tilesheet;
        private Layer layer;
        private Layer backLayer;

        private void setTilesWidePlanks(GameLocation location)
        {
            pos = Position;
            tilesheet = location.map.TileSheets.ToList().Find(el => el.ImageSource == BridgesTilesheet);
            layer = location.map.GetLayer("Buildings");
            backLayer = location.map.GetLayer("Back");

            if (Rotation == Rotation.Vertical) {
                PassableBridgeAreas.Add(new Rectangle(pos.X * 64, pos.Y * 64, 2 * 64, ((int)Length + 2) * 64));

                for (int y = 0; y < Length + 2; y++) {
                    if (y == 0) {
                        createTile(pos.X, pos.Y, 0);
                        createTile(pos.X + 1, pos.Y, 1);

                    } else if (y == Length + 1) {
                        createTile(pos.X, pos.Y + y, 6);
                        createTile(pos.X + 1, pos.Y + y, 7);

                    } else {
                        if (y % 2 == 0) {
                            createTile(pos.X, pos.Y + y, 2);
                            createTile(pos.X + 1, pos.Y + y, 3);

                        } else {
                            createTile(pos.X, pos.Y + y, 4);
                            createTile(pos.X + 1, pos.Y + y, 5);

                        }

                    }
                }
            } else if (Rotation == Rotation.Horizontal) {
                PassableBridgeAreas.Add(new Rectangle(pos.X * 64, pos.Y * 64, ((int)Length + 2) * 64, 2 * 64));

                for (int x = 0; x < Length + 2; x++) {
                    if (x == 0) {
                        createTile(pos.X, pos.Y, 0);
                        createTile(pos.X, pos.Y + 1, 1);

                    } else if (x == Length + 1) {
                        createTile(pos.X + x, pos.Y, 6);
                        createTile(pos.X + x, pos.Y + 1, 7);

                    } else {
                        if (x % 2 == 0) {
                            createTile(pos.X + x, pos.Y, 2);
                            createTile(pos.X + x, pos.Y + 1, 3);

                        } else {
                            createTile(pos.X + x, pos.Y, 4);
                            createTile(pos.X + x, pos.Y + 1, 5);

                        }
                    }
                }
            }

        }

        private void createTile(int x, int y, int index)
        {
            var tile = new StaticTile(layer, tilesheet, BlendMode.Alpha, index);
            tile.Properties.Add(new KeyValuePair<string, PropertyValue>("Passable", "T"));

            if (Rotation == Rotation.Horizontal) {
                tile.Properties.Add(new KeyValuePair<string, PropertyValue>("@Rotation", "90"));
                tile.Properties.Add(new KeyValuePair<string, PropertyValue>("@Flip", ((int)SpriteEffects.FlipVertically).ToString()));
            }
                

            layer.Tiles[x, y] = tile;
        }
    }
}
