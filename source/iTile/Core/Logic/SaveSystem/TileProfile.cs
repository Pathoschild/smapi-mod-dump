/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace iTile.Core.Logic.SaveSystem
{
    public class TileProfile
    {
        public bool Deleted { get; set; }

        public bool Animated { get; set; }

        public TileProfile[] TileFrames { get; set; }

        public long FrameInterval { get; set; }

        public BlendMode BlendMode { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        [JsonIgnore]
        public Vector2 Position
            => new Vector2(X, Y);

        public string LayerId { get; set; }

        public List<PropertyProfile> Properties { get; set; }

        public string TileSheetImageSource { get; set; }

        public string TileSheetId { get; set; }

        public Size TileSheetSize { get; set; }

        public Size TileSheetTileSize { get; set; }

        public int TileIndex { get; set; }

        [JsonConstructor]
        public TileProfile()
        {
        }

        public TileProfile(Tile gameTile, Vector2 location, string layerId, bool deleted = false)
        {
            if (deleted)
            {
                Deleted = true;
                X = location.X;
                Y = location.Y;
                LayerId = layerId;
                return;
            }

            TileSheet ts = gameTile.TileSheet;
            Animated = gameTile is AnimatedTile;
            BlendMode = gameTile.BlendMode;
            X = location.X;
            Y = location.Y;
            LayerId = layerId;
            CopyProperties(gameTile);
            TileSheetImageSource = ts.ImageSource;
            TileSheetId = ts.Id;
            TileSheetSize = ts.SheetSize;
            TileSheetTileSize = ts.TileSize;
            TileIndex = gameTile.TileIndex;

            if (Animated)
            {
                FrameInterval = GetFrameIntervalForAnimatedTile(gameTile);
                StaticTile[] frames = GetTileFramesForAnimatedTile(gameTile);
                if (frames != null && frames.Length != 0)
                {
                    TileFrames = new TileProfile[frames.Length];
                    for (int i = 0; i < frames.Length; i++)
                    {
                        TileProfile profile = new TileProfile(frames[0], Position, LayerId);
                        TileFrames[i] = profile;
                    }
                }
            }
        }

        private void CopyProperties(Tile gameTile)
        {
            Properties = new List<PropertyProfile>();
            foreach (KeyValuePair<string, PropertyValue> pair in gameTile.Properties)
            {
                Properties.Add(new PropertyProfile(pair.Key, pair.Value));
            }
        }

        private void PasteProperties(Tile gameTile)
        {
            foreach (PropertyProfile profile in Properties)
            {
                gameTile.Properties.Add(profile.Key, profile.ToPropertyValue());
            }
        }

        public static StaticTile[] GetTileFramesForAnimatedTile(Tile gameTile)
        {
            FieldInfo field = gameTile.GetType().GetField("m_tileFrames", BindingFlags.Instance | BindingFlags.NonPublic);
            StaticTile[] frames = (StaticTile[])field.GetValue(gameTile);
            return frames;
        }

        private long GetFrameIntervalForAnimatedTile(Tile gameTile)
        {
            FieldInfo field = gameTile.GetType().GetField("m_frameInterval", BindingFlags.Instance | BindingFlags.NonPublic);
            long animInt = (long)field.GetValue(gameTile);
            return animInt;
        }

        private StaticTile[] TileFramesToStaticTiles(Map map)
        {
            StaticTile[] frames = new StaticTile[TileFrames.Length];
            for (int i = 0; i < TileFrames.Length; i++)
            {
                frames[i] = new StaticTile(
                    map.GetLayer(TileFrames[i].LayerId),
                    map.GetTileSheet(TileFrames[i].TileSheetId),
                    TileFrames[i].BlendMode,
                    TileFrames[i].TileIndex);
            }
            return frames;
        }

        public Tile ToGameTile(Map map)
        {
            if (Deleted)
                return null;

            Tile tile;
            Layer layer = map.GetLayer(LayerId);
            TileSheet tileSheet = map.GetTileSheet(TileSheetId);
            if (Animated)
            {
                tile = new AnimatedTile(layer, TileFramesToStaticTiles(map), FrameInterval);
            }
            else
            {
                tile = new StaticTile(layer, tileSheet, BlendMode, TileIndex);
            }
            PasteProperties(tile);
            return tile;
        }
    }
}