/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using iTile.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using System;
using System.Linq;
using System.Collections.Generic;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using StardewModdingAPI;
using iTile.Core.Logic.Network;
using System.Reflection;

namespace iTile.Core.Logic.SaveSystem
{
    public class LocationProfile
    {
        [JsonIgnore]
        public static readonly string extraTileSheetPrefix = string.Concat("ExtraTileSheet_", iTile.ModID, "_");

        [JsonIgnore]
        public Dictionary<string, Dictionary<Vector2, Tile>> initialTiles = new Dictionary<string, Dictionary<Vector2, Tile>>();

        public List<TileProfile> tiles = new List<TileProfile>();

        [JsonConstructor]
        public LocationProfile()
        {
        }

        public LocationProfile(string name)
        {
            Name = name;
            if (Location == null)
            {
                throw new ArgumentException(string.Format("No location found with name {0} when trying to initialize LocationProfile", Name));
            }
        }

        [JsonIgnore]
        public GameLocation Location => Game1.getLocationFromName(Name);

        public string Name { get; set; }

        public void LoadInitialTiles()
        {
            if (Location == null || Location.map == null)
                return;

            foreach (string id in Layers.layerIDs)
            {
                initialTiles[id] = new Dictionary<Vector2, Tile>();
                Layer layer = Location.map.GetLayer(id);
                if (layer == null)
                    continue;
                for (int i = 0; i < layer.Tiles.Array.GetLength(0); i++)
                {
                    for (int j = 0; j < layer.Tiles.Array.GetLength(1); j++)
                    {
                        initialTiles[id].Add(new Vector2(i, j), layer.Tiles[i, j]);
                    }
                }
            }
        }

        public void LoadInExtraTileSheets()
        {
            if (tiles == null || tiles.Count == 0)
                return;

            foreach (TileProfile t in tiles)
            {
                LoadInTileSheetForTile(t);
            }
        }

        public void HandleTileReplacement(Tile tile, string layerId, Vector2 position, bool broadcast = true)
        {
            TileProfile tp = new TileProfile(tile, position, layerId, tile == null);
            HandleTileReplacement(tp, broadcast);
        }

        public void HandleTileReplacement(TileProfile tp, bool broadcast = true)
        {
            TileProfile existingTile = GetTileProfile(tp.Position, tp.LayerId, out int index);
            if (existingTile != null)
            {
                tiles.RemoveAt(index);
            }
            ReplaceTileSafe(tp, tp.LayerId, tp.Position);

            if (broadcast && Context.IsMultiplayer && Context.HasRemotePlayers)
            {
                iTile._Helper.Multiplayer.SendMessage(new Packet(tp, Name), NetworkManager.pasteActionKey, NetworkManager.toModIDs);
            }
        }

        private TileProfile GetTileProfile(Vector2 position, string layerId, out int index)
        {
            int i = 0;
            index = 0;
            TileProfile profile = tiles.FirstOrDefault(t =>
            {
                if (t.LayerId == layerId && t.Position.Equals(position))
                    return true;
                i++;
                return false;
            });
            index = i;
            return profile;
        }

        public void RestoreTile(Vector2 position, string layerId, bool broadcast = true)
        {
            TileProfile existingTile = GetTileProfile(position, layerId, out int index);
            if (existingTile != null)
            {
                tiles.RemoveAt(index);
            }

            Tile initialTile = initialTiles[layerId][position];
            if (initialTile != null)
            {
                DoubleCheckTS(initialTile);
            }
            Location.map.GetLayer(layerId).Tiles[(int)position.X, (int)position.Y] = initialTile;

            if (broadcast && Context.IsMultiplayer && Context.HasRemotePlayers)
            {
                iTile._Helper.Multiplayer.SendMessage(new Packet(position, layerId, Name), NetworkManager.restoreActionKey, NetworkManager.toModIDs);
            }
        }

        private void DoubleCheckTS(Tile tile)
        {
            TileSheet ts = tile.TileSheet;
            if (!Location.map.DependsOnTileSheet(ts))
            {
                if (Location.map.TileSheets.FirstOrDefault(t => t.Id == ts.Id && t.ImageSource == ts.ImageSource) == null)
                    return;

                if (tile is StaticTile)
                {
                    AssignTS(tile);
                }
                else if (tile is AnimatedTile)
                {
                    foreach (StaticTile sTile in TileProfile.GetTileFramesForAnimatedTile(tile))
                    {
                        AssignTS(sTile);
                    }
                }
            }
        }

        private void AssignTS(Tile tile)
        {
            FieldInfo field = tile.GetType().GetField("m_tileSheet", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(tile, Location.map.GetTileSheet(tile.TileSheet.Id));
        }

        public void ReplaceTileSafe(TileProfile tile, string layerId, Vector2 position, bool onAction = true)
        {
            if (!tile.Deleted)
                tile.TileSheetId = LoadInTileSheetForTile(tile);
            if (onAction)
                tiles.Add(tile);
            Location.map.GetLayer(layerId).Tiles[(int)position.X, (int)position.Y] = tile.ToGameTile(Location.map);
        }

        public void ApplyTile(TileProfile tile)
        {
            ReplaceTileSafe(tile, tile.LayerId, tile.Position, false);
        }

        public void ApplyAllTiles()
        {
            foreach (TileProfile tp in tiles)
            {
                ApplyTile(tp);
            }
        }

        private string LoadInTileSheetForTile(TileProfile tile)
        {
            Map map = Location.map;
            string newTileSheetId = GetCustomTileSheetIdFromTile(tile);

            if (map.GetTileSheet(tile.TileSheetId) != null)
                return tile.TileSheetId;
            if (map.GetTileSheet(newTileSheetId) != null)
                return newTileSheetId;

            TileSheet ts = new TileSheet(newTileSheetId, map, tile.TileSheetImageSource, tile.TileSheetSize, tile.TileSheetTileSize);
            map.AddTileSheet(ts);
            map.LoadTileSheets(Game1.mapDisplayDevice);
            {
                foreach (TileSheet tish in map.TileSheets)
                    iTile.LogDebug(tish.Id);
            }
            return newTileSheetId;
        }

        private static string GetCustomTileSheetIdFromTile(TileProfile tile)
        {
            string id = tile.TileSheetId;
            if (!id.StartsWith(extraTileSheetPrefix))
                id = string.Concat(extraTileSheetPrefix, id);
            return id;
        }
    }
}