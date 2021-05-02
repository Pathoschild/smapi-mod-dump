/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame
{
    /// <summary>
    /// TODO:
    /// Force maps to all have the same layers as SDV maps so we can draw different things on a map.
    /// Add player respawn timer in to shooting gallery mode.
    /// Reduce player Max HP to 10?
    /// </summary>
    /*
     * Back:	Terrain, water, and basic features (like permanent paths).
Buildings:	Placeholders for buildings (like the farmhouse). Any tiles placed on this layer will act like a wall unless the tile property has a "Passable" "T".
Paths:	Flooring, paths, grass, and debris (like stones, weeds, and stumps from the 'paths' tilesheet) which can be removed by the player.
Front:	Objects that are drawn on top of things behind them, like most trees. These objects will be drawn on top of the player if the player is North of them but behind the player if the player is south of them.
AlwaysFront:	Objects that are always drawn on top of other layers as well as the player. This is typically used for foreground effects like foliage cover.
    */
    public class SeasideScrambleMap
    {
        public xTile.Map map;

        public const int TileSize = 64;

        public SeasideScrambleMap()
        {

        }

        public SeasideScrambleMap(Map Map)
        {
            this.map = Map;
            //this.map.LoadTileSheets(Game1.mapDisplayDevice);
        }

        public Microsoft.Xna.Framework.Rectangle MapBounds
        {
            get
            {
                Vector2 size = this.getPixelSize();
                return new Microsoft.Xna.Framework.Rectangle(0,0,(int)size.X,(int)size.Y);
            }
        }

        public Vector2 Center
        {
            get
            {
                Microsoft.Xna.Framework.Rectangle bounds = this.MapBounds;
                return new Vector2(bounds.Width / 2, bounds.Height / 2);
            }
        }

        public virtual void update(GameTime time)
        {
            this.map.Update(time.ElapsedGameTime.Milliseconds);
        }

        public virtual void draw(SpriteBatch b)
        {
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            Game1.mapDisplayDevice.BeginScene(b);

            for (int i = 0; i < this.map.Layers.Count; i++)
            {
                this.map.Layers[i].Draw(Game1.mapDisplayDevice, SeasideScramble.self.camera.viewport, Location.Origin, false, 4);
            }
            Game1.mapDisplayDevice.EndScene();
            b.End();
        }

        public virtual void drawBackLayer()
        {

        }
        public virtual void drawBuildingsLayer()
        {

        }
        public virtual void drawPathsLayer()
        {

        }
        public virtual void drawFrontLayer()
        {

        }
        public virtual void drawAlwaysFrontLayer()
        {

        }

        public virtual void spawnPlayersAtPositions()
        {

        }

        /// <summary>
        /// Loads a map from a tbin file from the mod's asset folder.
        /// </summary>
        /// <param name="MapName"></param>
        /// <returns></returns>
        public static KeyValuePair<string, xTile.Map> LoadMap(string MapName)
        {
            // load the map file from your mod folder
            string pathToMaps = Path.Combine("Content", "Minigames", "SeasideScramble", "Maps", MapName);
            xTile.Map map = ModCore.ModHelper.Content.Load<Map>(pathToMaps, ContentSource.ModFolder);
            for (int index = 0; index < map.TileSheets.Count; ++index)
            {
                string imageSource = map.TileSheets[index].ImageSource;
                string fileName = Path.GetFileName(imageSource);
                string path1 = Path.GetDirectoryName(imageSource);
                map.TileSheets[index].ImageSource = Path.Combine(path1, fileName);
            }
            map.LoadTileSheets(Game1.mapDisplayDevice);

            // get the internal asset key for the map file
            string mapAssetKey = ModCore.ModHelper.Content.GetActualAssetKey(MapName, ContentSource.ModFolder);
            return new KeyValuePair<string, Map>(mapAssetKey, map);
        }

        public virtual Vector2 getPixelSize()
        {
            Layer layer = this.map.GetLayer("Back");
            return new Vector2(layer.TileSize.Width * (layer.LayerWidth/4f) * 4f, layer.TileSize.Height * (layer.LayerHeight/4f) * 4f);
        }
        public virtual Vector2 getPixelSize(float mapZoomScale)
        {
            Layer layer = this.map.GetLayer("Back");
            return new Vector2(layer.TileSize.Width * layer.LayerWidth * mapZoomScale, layer.TileSize.Height * layer.LayerWidth * mapZoomScale);
        }

        /// <summary>
        /// Checks to see if a given position is inside the map.
        /// </summary>
        /// <param name="Position"></param>
        /// <returns>A boolean representing if the point is inside the map.</returns>
        public bool insideMap(Vector2 Position)
        {
            return this.MapBounds.Contains((int)Position.X, (int)Position.Y);
        }

        public bool isObstacleAt(Vector2 position)
        {
            int tileSize = 1;//16 * 4;
            int x = (int)position.X / tileSize;
            int y = (int)position.Y / tileSize;

            PropertyValue propertyValue = (PropertyValue)null;
            Tile t=this.map.GetLayer("Front").PickTile(new Location(x, y), SeasideScramble.self.camera.viewport.Size);
            t?.TileIndexProperties.TryGetValue("Obstacle", out propertyValue);
            if (propertyValue == null) return false;
            else
            {
                return true;
            }
        }

        public void debugInfo()
        {
            Layer layer = this.map.GetLayer("Back");
            ModCore.log("layer Tile Size: " + new Vector2(layer.TileSize.Width,layer.TileSize.Height));
            ModCore.log("Layer Width/Height: " + new Vector2(layer.LayerWidth, layer.LayerHeight));
            ModCore.log("Size multiplier: 4");
        }

    }
}
