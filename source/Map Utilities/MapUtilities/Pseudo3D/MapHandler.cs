using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using xTile;
using xTile.Layers;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Pseudo3D
{
    public static class MapHandler
    {

        public static bool isPseudo3DLocation(GameLocation location)
        {
            Map locationMap = location.map;
            foreach(Layer layer in locationMap.Layers)
            {
                string[] nameSplit = layer.Id.Split('_');
                if (nameSplit.Length == 2 && (nameSplit[0].Equals("Back") || nameSplit[0].Equals("Buildings") || nameSplit[0].Equals("Front")))
                {
                    //Logger.log("Found " + layer.Id + ".");
                    return true;
                }
            }
            return false;
        }

        public static bool hasLevel(GameLocation location, string level)
        {
            return hasLevel(location.map, level);
        }

        public static bool hasLevel(Map map, string level)
        {
            foreach (Layer layer in map.Layers)
            {
                string[] nameSplit = layer.Id.Split('_');
                if (nameSplit.Length == 2 && nameSplit[1].Equals(level) && (nameSplit[0].Equals("Back") || nameSplit[0].Equals("Buildings") || nameSplit[0].Equals("Front")))
                {
                    return true;
                }
            }
            return false;
        }

        public static void drawOverlays(SpriteBatch b, GameLocation location)
        {
            Map map = location.map;
            foreach(Layer layer in map.Layers)
            {
                string[] nameSplit = layer.Id.Split('_');
                if (nameSplit.Length == 2 && (nameSplit[0].Equals("Back") || nameSplit[0].Equals("Buildings") || nameSplit[0].Equals("Front")))
                {
                    if (nameSplit[0].Equals("Front"))
                    {
                        b.End();
                        b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                        foreach (Character c in location.characters)
                        {
                            if (nameSplit[1].Equals(LevelHandler.getLevelForCharacter(c)))
                            {
                                c.draw(b);
                            }
                        }
                        foreach (Farmer f in location.farmers)
                        {
                            if (nameSplit[1].Equals(LevelHandler.getLevelForCharacter(f)))
                            {
                                f.draw(b);
                            }
                        }
                        Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                        layer.Draw(Game1.mapDisplayDevice, Game1.viewport, xTile.Dimensions.Location.Origin, false, Game1.pixelZoom);
                        Game1.mapDisplayDevice.EndScene();
                        b.End();
                        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
                    }
                    else
                    {
                        Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                        layer.Draw(Game1.mapDisplayDevice, Game1.viewport, xTile.Dimensions.Location.Origin, false, Game1.pixelZoom);
                        Game1.mapDisplayDevice.EndScene();
                    }
                }
            }
        }

        public static string getEquivalentLayerIfPresent(Map map, Character c, string layerName)
        {
            foreach (Layer layer in map.Layers)
            {
                string[] ID = layer.Id.Split('_');
                if (ID.Length == 2 && ID[1].Equals(LevelHandler.getLevelForCharacter(c)) && ID[0].Equals(layerName))
                {
                    //Logger.log("Found equivalent layer for " + layerName + ": " + layer.Id);
                    return layer.Id;
                }
            }
            Logger.log("Could not find equivalent layer for " + layerName + "!");
            return null;
        }

        public static void setPassableTiles(GameLocation location, string layerID)
        {
            Map map = location.map;

            bool hadLayerID = false;
            foreach (Layer layer in map.Layers)
            {
                if (layer.Id.Split('_').Length == 2 && layer.Id.Split('_')[1].Equals(layerID))
                {
                    hadLayerID = true;
                    break;
                }
            }
            if (!hadLayerID)
            {
                Logger.log("Could not find a pseudo-3D layer by the ID '" + layerID + "'!", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            Dictionary<Layer, Layer> levelLayers = new Dictionary<Layer, Layer>();

            foreach (Layer layer in map.Layers)
            {
                if (layer.Id.Split('_').Length == 2 && layer.Id.Split('_')[1].Equals(layerID) && map.GetLayer(layer.Id.Split('_')[0]) != null)
                {
                    levelLayers[layer] = map.GetLayer(layer.Id.Split('_')[0]);
                }
            }

            for (int x = 0; x < map.GetLayer("Back").LayerSize.Width; x++)
            {
                for (int y = 0; y < map.GetLayer("Back").LayerSize.Height; y++)
                {
                    foreach(Layer layer in levelLayers.Keys)
                    {
                        //bool currentlyPassable = location.isPointPassable(new xTile.Dimensions.Location(x * 64, y * 64), Game1.viewport);
                        string layerName = layer.Id.Split('_')[0];
                        if (layerName.Equals("Back"))
                        {

                        }
                    }
                }
            }


        }

        public static void applyLayer(GameLocation location, string layerID)
        {
            
            Map map = location.map;

            bool hadLayerID = false;
            foreach (Layer layer in map.Layers)
            {
                if(layer.Id.Split('_').Length == 2 && layer.Id.Split('_')[1].Equals(layerID))
                {
                    hadLayerID = true;
                    break;
                }
            }
            if (!hadLayerID)
            {
                Logger.log("Could not find a pseudo-3D layer by the ID '" + layerID + "'!", StardewModdingAPI.LogLevel.Warn);
                return;
            }
            Layer back_placeHolder = map.GetLayer("Back_placeHolder");
            Layer buildings_placeHolder = map.GetLayer("Buildings_placeHolder");
            Layer front_placeHolder = map.GetLayer("Front_placeHolder");
            if(back_placeHolder == null)
            {
                map.AddLayer(new Layer("Back_placeHolder", map, map.GetLayer("Back").LayerSize, map.GetLayer("Back").TileSize));
                back_placeHolder = map.GetLayer("Back_placeHolder");
            }
            if (buildings_placeHolder == null)
            {
                map.AddLayer(new Layer("Buildings_placeHolder", map, map.GetLayer("Buildings").LayerSize, map.GetLayer("Buildings").TileSize));
                buildings_placeHolder = map.GetLayer("Buildings_placeHolder");
            }
            if (front_placeHolder == null)
            {
                map.AddLayer(new Layer("Front_placeHolder", map, map.GetLayer("Front").LayerSize, map.GetLayer("Front").TileSize));
                front_placeHolder = map.GetLayer("Front_placeHolder");
            }

            List<Layer> layersOnLevel = new List<Layer>();

            foreach(Layer layer in map.Layers)
            {
                if (layer.Id.Split('_').Length == 2 && layer.Id.Split('_')[1].Equals(layerID))
                {
                    layersOnLevel.Add(layer);
                }
            }
            
            for(int x = 0; x < map.GetLayer("Back").LayerSize.Width; x++)
            {
                for(int y = 0; y < map.GetLayer("Back").LayerSize.Height; y++)
                {
                    applyFullLayersetAtTile(map, layersOnLevel, x, y);
                }
            }
        }

        public static void applyFullLayersetAtTile(Map map, List<Layer> layersOnLevel, int x, int y)
        {
            foreach (Layer layer in layersOnLevel)
            {
                if (layer.Tiles[x, y] != null)
                {
                    Layer activeLayer = null;
                    Layer tempLayer = null;

                    string layerPrefix = layer.Id.Split('_')[0];
                    if (layerPrefix.Equals("Back"))
                    {
                        activeLayer = map.GetLayer("Back");
                        tempLayer = map.GetLayer("Back_placeHolder");
                    }
                    else if (layerPrefix.Equals("Buildings"))
                    {
                        activeLayer = map.GetLayer("Buildings");
                        tempLayer = map.GetLayer("Buildings_placeHolder");
                    }
                    else if (layerPrefix.Equals("Front"))
                    {
                        activeLayer = map.GetLayer("Front");
                        tempLayer = map.GetLayer("Front_placeHolder");
                    }

                    if (activeLayer != null && activeLayer.Tiles[x, y] != null)
                    {
                        tempLayer.Tiles[x, y] = activeLayer.Tiles[x, y];
                    }
                    activeLayer.Tiles[x, y] = layer.Tiles[x, y];
                    layer.Tiles[x, y] = null;
                }
            }
        }
    }
}
