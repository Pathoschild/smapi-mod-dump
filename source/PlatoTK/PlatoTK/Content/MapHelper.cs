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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace PlatoTK.Content
{
    internal class MapHelper : InnerHelper, IMapHelper
    {
        public MapHelper(IPlatoHelper helper)
            : base(helper)
        {

        }

        public Map PatchMapArea(Map map, Map patch, Point position, Rectangle? sourceArea = null, bool patchProperties = true, bool removeEmpty = false)
        {
                Rectangle sourceRectangle = sourceArea.HasValue ? sourceArea.Value : new Microsoft.Xna.Framework.Rectangle(0, 0, patch.DisplayWidth / Game1.tileSize, patch.DisplayHeight / Game1.tileSize);

                //tilesheet normalizing taken with permission from Content Patcher 
                // https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/Framework/Patches/EditMapPatch.cs
                foreach (TileSheet tilesheet in patch.TileSheets)
                {
                    TileSheet mapSheet = map.GetTileSheet(tilesheet.Id);

                    if (mapSheet == null || mapSheet.ImageSource != tilesheet.ImageSource)
                    {
                        // change ID if needed so new tilesheets are added after vanilla ones (to avoid errors in hardcoded game logic)
                        string id = tilesheet.Id;
                        if (!id.StartsWith("z_", StringComparison.InvariantCultureIgnoreCase))
                            id = $"z_{id}";

                        // change ID if it conflicts with an existing tilesheet
                        if (map.GetTileSheet(id) != null)
                        {
                            int disambiguator = Enumerable.Range(2, int.MaxValue - 1).First(p => map.GetTileSheet($"{id}_{p}") == null);
                            id = $"{id}_{disambiguator}";
                        }

                        //add tilesheet
                        if (!map.TileSheets.ToList().Exists(ts => ts.Id == tilesheet.Id))
                            map.AddTileSheet(new TileSheet(tilesheet.Id, map, tilesheet.ImageSource, tilesheet.SheetSize, tilesheet.TileSize));
                    }
                }

                if (patchProperties)
                    foreach (KeyValuePair<string, PropertyValue> p in patch.Properties)
                        if (map.Properties.ContainsKey(p.Key))
                            /*if (p.Key == "EntryAction")
                                map.Properties[p.Key] = map.Properties[p.Key] + ";" + p.Value;
                            else*/
                                map.Properties[p.Key] = p.Value;
                        else
                            map.Properties.Add(p);

                
                for (int x = 0; x < sourceRectangle.Width; x++)
                    for (int y = 0; y < sourceRectangle.Height; y++)
                        foreach (Layer layer in patch.Layers)
                        {
                            int px = (int)position.X + x;
                            int py = (int)position.Y + y;

                            int sx = (int)sourceRectangle.X + x;
                            int sy = (int)sourceRectangle.Y + y;

                            Tile sourceTile = layer.Tiles[(int)sx, (int)sy];
                            Layer mapLayer = map.GetLayer(layer.Id);

                            if (mapLayer == null)
                            {
                                map.InsertLayer(new Layer(layer.Id, map, map.Layers[0].LayerSize, map.Layers[0].TileSize), map.Layers.Count);
                                mapLayer = map.GetLayer(layer.Id);
                            }

                            /*if (mapLayer.IsImageLayer())
                                mapLayer.SetupImageLayer();*/

                            if (patchProperties)
                                foreach (var prop in layer.Properties)
                                    if (!mapLayer.Properties.ContainsKey(prop.Key))
                                        mapLayer.Properties.Add(prop);
                                    else
                                        mapLayer.Properties[prop.Key] = prop.Value;

                            if (sourceTile == null)
                            {
                                if (removeEmpty)
                                {
                                    try
                                    {
                                        mapLayer.Tiles[(int)px, (int)py] = null;
                                    }
                                    catch { }
                                }
                                continue;
                            }

                            TileSheet tilesheet = map.GetTileSheet(sourceTile.TileSheet.Id);
                            Tile newTile = new StaticTile(mapLayer, tilesheet, BlendMode.Additive, sourceTile.TileIndex);

                            try
                            {
                                if (sourceTile.Properties.ContainsKey("NoTileMerge"))
                                    newTile = mapLayer.Tiles[(int)px, (int)py];
                            }
                            catch
                            {

                            }

                            if (sourceTile is AnimatedTile aniTile)
                            {
                                List<StaticTile> staticTiles = new List<StaticTile>();

                                foreach (StaticTile frame in aniTile.TileFrames)
                                    staticTiles.Add(new StaticTile(mapLayer, tilesheet, BlendMode.Additive, frame.TileIndex));

                                newTile = new AnimatedTile(mapLayer, staticTiles.ToArray(), aniTile.FrameInterval);
                            }

                            if (patchProperties)
                            {
                                foreach (var prop in sourceTile.Properties)
                                    if (newTile.Properties.ContainsKey(prop.Key))
                                        newTile.Properties[prop.Key] = prop.Value;
                                    else
                                        newTile.Properties.Add(prop);

                                foreach (var prop in sourceTile.TileIndexProperties)
                                    if (newTile.TileIndexProperties.ContainsKey(prop.Key))
                                        newTile.TileIndexProperties[prop.Key] = prop.Value;
                                    else
                                        newTile.TileIndexProperties.Add(prop);
                            }
                            try
                            {
                                mapLayer.Tiles[(int)px, (int)py] = newTile;
                            }
                            catch
                            {
                            }
                        }

                return map;
        }
    }
}
