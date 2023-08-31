/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static StardewValley.WaterTiles;

namespace Unlockable_Bundles.Lib
{
    //This class is used to cache building maps that contain bundles
    //Buildings all share the same map, so when one gets updated the other does as well
    public class UnsafeMap : Map
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }

        private WaterTileData[,] WaterTiles = null;

        public UnsafeMap(GameLocation location)
        {
            var map = location.Map;
            CopyFromTileSheets(Helper.Reflection.GetField<List<TileSheet>>(map, "m_tileSheets").GetValue());

            foreach (var layer in map.Layers) {
                var newLayer = new Layer(layer.Id, this, layer.LayerSize, layer.TileSize);
                AddLayer(newLayer);

                for (int x = 0; x < layer.Tiles.Array.GetLength(0); x++)
                    for (int y = 0; y < layer.Tiles.Array.GetLength(1); y++)
                        if (layer.Tiles[x, y] is not null) {
                            newLayer.Tiles[x, y] = layer.Tiles[x, y].Clone(newLayer);
                            newLayer.Tiles[x, y].TileSheet.TileIndexProperties[newLayer.Tiles[x, y].TileIndex].CopyFrom(layer.Tiles[x, y].TileSheet.TileIndexProperties[layer.Tiles[x, y].TileIndex]);
                            newLayer.Tiles[x, y].Properties.CopyFrom(CloneProperties(layer.Tiles[x, y].Properties));
                        }
            }

            try {
                if (location.waterTiles is not null) {
                    WaterTiles = new WaterTileData[location.waterTiles.waterTiles.GetLength(0), location.waterTiles.waterTiles.GetLength(1)];

                    for (int x = 0; x < WaterTiles.GetLength(0); x++)
                        for (int y = 0; y < WaterTiles.GetLength(1); y++)
                            WaterTiles[x, y] = new WaterTileData(location.waterTiles.waterTiles[x, y].isWater, location.waterTiles.waterTiles[x, y].isVisible);
                }
            } catch (Exception) {
            }
        }

        //If we don't create new objects the properties will get lost when return to title
        //We don't reload the cached maps either as SF only ever loads their maps once even when calling a forced reload on the map
        public static IPropertyCollection CloneProperties(IPropertyCollection collection)
        {
            var ret = new PropertyCollection();
            foreach (var entry in collection)
                ret.Add(entry.Key, entry.Value);
            return ret;
        }

        public void CopyFromTileSheets(List<TileSheet> tileSheets)
        {
            var m_tileSheets = ModEntry._Helper.Reflection.GetField<List<TileSheet>>(this, "m_tileSheets").GetValue();
            m_tileSheets.AddRange(tileSheets);
        }

        public void CopyLayer(Layer layer)
        {
            var m_layers = ModEntry._Helper.Reflection.GetField<List<Layer>>(this, "m_tileSheets").GetValue();
            var m_layersById = ModEntry._Helper.Reflection.GetField<Dictionary<string, Layer>>(this, "m_tileSheets").GetValue();

            m_layers.Add(layer);
            m_layersById.Add(layer.Id, layer);
        }

        public void PasteData(GameLocation location)
        {
            var map = location.Map;

            foreach (var layer in map.Layers) {
                var replaceLayer = GetLayer(layer.Id);
                if (replaceLayer is null) {
                    map.RemoveLayer(layer);
                    continue;
                }

                for (int x = 0; x < layer.Tiles.Array.GetLength(0); x++)
                    for (int y = 0; y < layer.Tiles.Array.GetLength(1); y++)
                        if (replaceLayer.Tiles[x, y] is null)
                            layer.Tiles[x, y] = null;
                        else {
                            layer.Tiles[x, y] = CloneTile(replaceLayer.Tiles[x, y], layer);
                            layer.Tiles[x, y].TileSheet.TileIndexProperties[layer.Tiles[x, y].TileIndex].CopyFrom(replaceLayer.Tiles[x, y].TileSheet.TileIndexProperties[replaceLayer.Tiles[x, y].TileIndex]);
                            layer.Tiles[x, y].Properties.CopyFrom(replaceLayer.Tiles[x, y].Properties);
                        }
            }

            if (WaterTiles is null)
                location.waterTiles = null;
            else
                for (int x = 0; x < WaterTiles.GetLength(0); x++)
                    for (int y = 0; y < WaterTiles.GetLength(1); y++)
                        WaterTiles[x, y] = new WaterTileData(location.waterTiles.waterTiles[x, y].isWater, location.waterTiles.waterTiles[x, y].isVisible);
        }

        private static Tile CloneTile(Tile tile, Layer layer)
        {
            Tile clone = tile is StaticTile
                ? cloneStaticTile(tile as StaticTile, layer)
                : cloneAnimatedTile(tile as AnimatedTile, layer);

            return clone;
        }

        private static AnimatedTile cloneAnimatedTile(AnimatedTile copyFrom, Layer layer)
        {
            StaticTile[] tileFrames = new StaticTile[copyFrom.TileFrames.Length];
            for (int i = 0; i < copyFrom.TileFrames.Length; i++)
                tileFrames[i] = cloneStaticTile(copyFrom.TileFrames[i], layer);

            return new AnimatedTile(layer, tileFrames, copyFrom.FrameInterval);
        }

        private static StaticTile cloneStaticTile(StaticTile copyFrom, Layer layer)
        {
            var key = copyFrom.TileSheet.Id;
            //I can't cache the tilesheet this time as that'll cause issues in splitscreen when multiple users enter the same building type
            TileSheet tilesheet = layer.Map.GetTileSheet(key);
            return new StaticTile(layer, tilesheet, copyFrom.BlendMode, copyFrom.TileIndex);

        }
    }
}
