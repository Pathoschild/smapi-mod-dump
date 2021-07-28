/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MapTK.MapExtras
{
    internal class IntegratedMapEditsAssetEditor : IAssetEditor
    {
        private readonly IPlatoHelper Plato;
        internal const string UseOrderProperty = "@As_Order";
        public IntegratedMapEditsAssetEditor(IModHelper helper)
        {
            Plato = helper.GetPlatoHelper();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.DataType == typeof(Map);
        }

        public void Edit<T>(IAssetData asset)
        {
            var mapAsset = asset.AsMap();
            var map = mapAsset.Data;

            Func<Layer, bool> layerPredicate = (l) => l.Properties.TryGetValue(MapExtrasHandler.UseProperty, out PropertyValue value)
                                                     && value.ToString().Split(' ') is string[] p
                                                     && p.Length >= 2
                                                     && (p[0] == "Merge" || p[0] == "Replace")
                                                     && (!l.Properties.TryGetValue(MapExtrasHandler.UseConditionProperty, out PropertyValue conditions) || Plato.CheckConditions(conditions.ToString(), l));


            if (!map.Layers.Any(layerPredicate))
                return;

            List<OrderedAction> actions = new List<OrderedAction>();

            map.Layers.Where(layerPredicate).ToList().ForEach((layer) =>
           {
               string[] p = layer.Properties[MapExtrasHandler.UseProperty].ToString().Split(' ');

               int order = layer.Properties.TryGetValue(UseOrderProperty, out PropertyValue value) && int.TryParse(value.ToString(), out int o) ? o : (p[0] == "Replace" && p.Length < 6) ? 0 : 1;

               Layer original = map.GetLayer(p[1]);

               if (p[0] == "Replace" && p.Length < 6)
                   actions.Add(new OrderedAction(order, () => ReplaceLayer(map, layer, original)));
               else
                   actions.Add(new OrderedAction(order, () => MergeLayer(map, layer, original,
                        p.Length > 5
                        && int.TryParse(p[2], out int x)
                        && int.TryParse(p[3], out int y)
                        && int.TryParse(p[4], out int w)
                        && int.TryParse(p[5], out int h)
                        ? new Rectangle?(new Rectangle(x, y, w, h)) : null, p[0] == "Merge")));
           });

            actions.OrderBy(a => a.Order).ToList().ForEach(a => a.Call());

            mapAsset.ReplaceWith(map);
        }

        private Map MergeLayer(Map map, Layer layer, Layer original, Rectangle? sourceArea, bool removeEmpty)
        {
            var newLayer = new Layer(original.Id, map, layer.LayerSize, layer.TileSize);

            foreach (var tile in GetTiles(layer))
                if ((removeEmpty || tile.Tile != null) && (!sourceArea.HasValue || sourceArea.Value.Contains(new Point(tile.X, tile.Y))))
                    newLayer.Tiles[tile.X, tile.Y] = tile.Tile;
                else
                   newLayer.Tiles[tile.X, tile.Y] = original.Tiles[tile.X, tile.Y];

            map.RemoveLayer(original);
            map.AddLayer(newLayer);

            return map;
        }

        private Map ReplaceLayer(Map map, Layer layer, Layer original)
        {
            string id = original.Id;
            map.RemoveLayer(original);
            layer.Id = id;
            return map;
        }

        private IEnumerable<TileInfo> GetTiles(Layer layer)
        {
            for (int y = 0; y < layer.LayerHeight / layer.TileHeight; y++)
                for (int x = 0; x < layer.LayerWidth / layer.TileWidth; x++)
                    yield return new TileInfo(x,y,layer.Tiles[x, y]);

        }

    }

    internal class OrderedAction
    {
        public int Order { get; }

        public Action Call { get; }

        public OrderedAction(int order, Action action)
        {
            Order = order;
            Call = action;
        }
    }

    internal class TileInfo
    {
        public int X { get;}

        public int Y { get; }

        public Tile Tile { get; }

        public TileInfo(int x, int y, Tile tile)
        {
            X = x;
            Y = y;
            Tile = tile;
        }
    }
}
