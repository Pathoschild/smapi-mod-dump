/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using xTile.Tiles;

namespace MTN2.MapData {
    public abstract class Resource {
        protected GameLocation Map;
        public List<Spawn> ResourceList { get; set; }
        public string MapName { get; set; } = "Farm";
        
        public Resource(string MapName) {
            this.MapName = MapName;
            ResourceList = new List<Spawn>();
        }

        public virtual void Initalize() {
            Map = Game1.getLocationFromName(MapName);
            for (int i = 0; i < ResourceList.Count; i++) {
                ResourceList[i].Initialize();
            }
        }

        public abstract void SpawnAll(int Attempts);
        public abstract void SpawnItem(int Attempts, int index);
        public abstract void AddAtPoint(Vector2 Point, Spawn SItem);

        protected virtual void TileBoundLogic(Spawn SItem, int Attempts, int Width, int Height) {
            Vector2 Tile = new Vector2();

            int MapWidth = (SItem.MapWide) ? Map.map.Layers[0].LayerWidth : SItem.AreaBinding.Width;
            int MapHeight = (SItem.MapWide) ? Map.map.Layers[0].LayerHeight : SItem.AreaBinding.Height;
            int X = (SItem.MapWide) ? 0 : SItem.AreaBinding.X-1;
            int Y = (SItem.MapWide) ? 0 : SItem.AreaBinding.Y-1;

            for (Tile.X = X; Tile.X < MapWidth; Tile.X++) {
                for (Tile.Y = Y; Tile.Y < MapHeight; Tile.Y++) {
                    if (TileBoundLogicCheck(Tile, Width, Height, SItem.TileBinding)) {
                        AddAtPoint(Tile, SItem);
                        return;
                    }
                }
            }
            return;
        }

        protected virtual void AreaBoundLogic(Spawn SItem, int Attempts, int Width, int Height) {
            Vector2 Tile;
            for (int i = 0; i < Attempts; i++) {
                Tile = (SItem.MapWide) ? GenerateTile() : GenerateTile(SItem.AreaBinding);
                if (AreaBoundLogicCheck(Tile, Width, Height)) {
                    AddAtPoint(Tile, SItem);
                    return;
                }
            }
            return;
        }

        protected virtual Vector2 GenerateTile() {
            return new Vector2(Game1.random.Next(-1, Map.map.Layers[0].LayerWidth), Game1.random.Next(-1, Map.map.Layers[0].LayerHeight));
        }

        protected virtual Vector2 GenerateTile(Area area) {
            return new Vector2(Game1.random.Next(area.X - 1, area.Width), Game1.random.Next(area.Y - 1, area.Height));
        }

        private bool AreaBoundLogicCheck(Vector2 tile, int Width, int Height) {
            return AreaCheck(tile, Width, Height);
        }

        private bool TileBoundLogicCheck(Vector2 tile, int Width, int Height, int TileId) {
            Tile Target = Map.map.GetLayer("Paths").Tiles[(int)tile.X, (int)tile.Y];
            if (Target != null && Target.TileIndex == TileId) {
                return AreaCheck(tile, Width, Height);
            }
            return false;
        }

        private bool AreaCheck(Vector2 tile, int Width, int Height) {
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    if (!Map.isTileLocationTotallyClearAndPlaceable(new Vector2(tile.X + i, tile.Y + j))) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
