using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile.Tiles;

namespace PondWithBridge
{
    public class PondWithBridge : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            Farm farm = Game1.getFarm();
            farm.map.AddTileSheet(new TileSheet("Z", farm.map, Helper.Content.GetActualAssetKey("spring_town", ContentSource.GameContent), new xTile.Dimensions.Size(32, 62), new xTile.Dimensions.Size(16, 16)));
            farm.map.LoadTileSheets(Game1.mapDisplayDevice);

            PatchMap(farm, BridgeEdits);
        }

        private readonly List<Tile> BridgeEdits = new List<Tile>
        {
            //---tiles to null---
            new Tile(0, 40, 48, -1), new Tile(0, 41, 48, -1), //negative 1 layer
            new Tile(0, 40, 49, -1), new Tile(0, 41, 49, -1), //1st row back layer
            new Tile(1, 39, 49, -1), new Tile(1, 40, 49, -1), new Tile(1, 41, 49, -1), new Tile(1, 42, 49, -1),//1st row buildings
            new Tile(0, 40, 50, -1), new Tile(0, 41, 50, -1), //2nd row back layer
            new Tile(1, 39, 50, -1), new Tile(1, 40, 50, -1), new Tile(1, 41, 50, -1), new Tile(1, 42, 50, -1),//2nd row buildings
            new Tile(0, 40, 51, -1), new Tile(0, 41, 51, -1), //3rd row back layer
            new Tile(0, 40, 52, -1), new Tile(0, 41, 52, -1), //4th row back layer
            new Tile(0, 40, 53, -1), new Tile(0, 41, 53, -1), //5th row back layer
            new Tile(0, 40, 54, -1), new Tile(0, 41, 54, -1), //6th row back layer
            new Tile(0, 40, 55, -1), new Tile(0, 41, 55, -1), //7th row back layer
            new Tile(0, 40, 56, -1), new Tile(0, 41, 56, -1), //8th row back layer
            new Tile(0, 40, 57, -1), new Tile(0, 41, 57, -1), //9th row back layer
            new Tile(1, 41, 57, -1), new Tile(1, 42, 57, -1), //9th buildings  layer
            new Tile(0, 40, 58, -1), new Tile(0, 41, 58, -1), //10th row back layer
            new Tile(1, 39, 58, -1), new Tile(1, 40, 58, -1), new Tile(1, 41, 58, -1), //10th buildings  layer
            new Tile(0, 39, 59, -1), new Tile(0, 40, 59, -1), new Tile(0, 41, 59, -1), new Tile(0, 42, 59, -1), //11th row back layer

            //----tiles to edit---
            //bridge
            new Tile(1, 39, 48, 905, 2), new Tile(0, 40, 48, 970, 2), new Tile(0, 41, 48, 970, 2), new Tile(1, 42, 48, 905, 2), //-1st
            new Tile(1, 39, 49, 905, 2), new Tile(0, 40, 49, 909, 2), new Tile(0, 41, 49, 909, 2), new Tile(1, 42, 49, 905, 2), //1st
            new Tile(1, 39, 50, 905, 2), new Tile(0, 40, 50, 909, 2), new Tile(0, 41, 50, 909, 2), new Tile(1, 42, 50, 905, 2), //2nd
            new Tile(1, 39, 51, 905, 2), new Tile(0, 40, 51, 909, 2), new Tile(0, 41, 51, 909, 2), new Tile(1, 42, 51, 905, 2), //3rd
            new Tile(1, 39, 52, 905, 2), new Tile(0, 40, 52, 909, 2), new Tile(0, 41, 52, 909, 2), new Tile(1, 42, 52, 905, 2), //4th
            new Tile(1, 39, 53, 905, 2), new Tile(0, 40, 53, 909, 2), new Tile(0, 41, 53, 909, 2), new Tile(1, 42, 53, 905, 2), //5th
            new Tile(1, 39, 54, 905, 2), new Tile(0, 40, 54, 909, 2), new Tile(0, 41, 54, 909, 2), new Tile(1, 42, 54, 905, 2), //6th
            new Tile(1, 39, 55, 905, 2), new Tile(0, 40, 55, 909, 2), new Tile(0, 41, 55, 909, 2), new Tile(1, 42, 55, 905, 2), //7th
            new Tile(1, 39, 56, 905, 2), new Tile(0, 40, 56, 909, 2), new Tile(0, 41, 56, 909, 2), new Tile(1, 42, 56, 905, 2), //8th
            new Tile(1, 39, 57, 905, 2), new Tile(0, 40, 57, 909, 2), new Tile(0, 41, 57, 909, 2), new Tile(1, 42, 57, 905, 2), //9th
            new Tile(1, 39, 58, 905, 2), new Tile(0, 40, 58, 909, 2), new Tile(0, 41, 58, 909, 2), new Tile(1, 42, 58, 905, 2), //9th
            new Tile(1, 39, 59, 937, 2), new Tile(0, 40, 59, 969, 2), new Tile(0, 41, 59, 969, 2), new Tile(1, 42, 59, 937, 2), //11th
            //shadows
            new Tile(4, 38, 59, 223, 2), new Tile(4, 38, 58, 191, 2), new Tile(4, 38, 57, 191, 2), new Tile(4, 38, 56, 191, 2),
            new Tile(4, 38, 55, 191, 2), new Tile(4, 38, 54, 191, 2), new Tile(4, 38, 53, 191, 2), new Tile(4, 38, 52, 191, 2),
            new Tile(4, 38, 51, 191, 2), new Tile(4, 38, 50, 191, 2), new Tile(4, 38, 49, 191, 2), new Tile(4, 38, 48, 159, 2),

        };


        private void PatchMap(GameLocation gl, List<Tile> tileArray)
        {
            foreach (Tile tile in tileArray)
            {
                if (tile.tileIndex < 0)
                {
                    gl.removeTile(tile.x, tile.y, tile.layer);
                    gl.waterTiles[tile.x, tile.y] = false;

                    continue;
                }

                if (gl.map.Layers[tile.l].Tiles[tile.x, tile.y] == null)
                {
                    gl.map.Layers[tile.l].Tiles[tile.x, tile.y] = new StaticTile(gl.map.GetLayer(tile.layer), gl.map.TileSheets[tile.tileSheet], xTile.Tiles.BlendMode.Alpha, tile.tileIndex);
                }
                else
                {
                    gl.setMapTileIndex(tile.x, tile.y, tile.tileIndex, tile.layer);
                }
            }
        }
    }

    public class Tile
    {
        public int l;
        public int x;
        public int y;
        public int tileIndex;
        public string layer;
        public int tileSheet = 1;

        public Tile(int l, int x, int y, int tileIndex)
        {
            this.l = l; this.x = x; this.y = y; this.tileIndex = tileIndex;
            setLayerName(l);
        }

        public Tile(int l, int x, int y, int tileIndex, int tileSheet)
        {
            this.l = l; this.x = x; this.y = y; this.tileIndex = tileIndex; this.tileSheet = tileSheet;
            setLayerName(l);
        }

        public void setLayerName(int l)
        {
            switch (l)
            {
                case 0:
                    this.layer = "Back";
                    break;
                case 1:
                    this.layer = "Buildings";
                    break;
                case 2:
                    this.layer = "Paths";
                    break;
                case 3:
                    this.layer = "Front";
                    break;
                case 4:
                    this.layer = "AlwaysFront";
                    break;
                default:
                    break;
            }
        }
    }
}
