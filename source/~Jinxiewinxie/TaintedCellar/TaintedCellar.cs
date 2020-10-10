/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TaintedCellar
{
 
    public class TaintedCellar : Mod
    {

        private CellarConfig Config;

        private XmlSerializer locationSerializer = new XmlSerializer(typeof(GameLocation));
        private GameLocation taintedCellar;
        private Map map;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<CellarConfig>();
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            SaveEvents.AfterReturnToTitle += this.SaveEvents_AfterReturnToTitle;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            if (this.Config.OnlyUnlockAfterFinalHouseUpgrade && Game1.player.HouseUpgradeLevel < 3)
            {
                return;
            }

            try
            {
                //map = this.Helper.Content.Load<Map>("FarmExpansion.xnb", ContentSource.ModFolder);
                map = this.Helper.Content.Load<Map>(@"assets\TaintedCellarMap.tbin");
                //map.LoadTileSheets(Game1.mapDisplayDevice);
            }
            catch(Exception ex)
            {
                this.UnloadMod();
                this.Monitor.Log(ex.Message, LogLevel.Error);
                this.Monitor.Log($"Unable to load map file 'assets{Path.DirectorySeparatorChar.ToString()}TaintedCellarMap.tbin', unloading mod. Please try re-installing the mod.", LogLevel.Alert);
                return;
            }

            if (!File.Exists(Path.Combine(this.Helper.DirectoryPath, "pslocationdata", $"{Constants.SaveFolderName}.xml")))
            {
                taintedCellar = new GameLocation(map, "TaintedCellarMap")
                {
                    isOutdoors = false,
                    isFarm = true
                };
            }
            else
            {
                Load();
            }

            int entranceX = (this.Config.FlipCellarEntrance ? 69 : 57) + this.Config.XPositionOffset;
            int entranceY = 12 + this.Config.YPositionOffset;
            taintedCellar.setTileProperty(3, 3, "Buildings", "Action", $"Warp {entranceX} {entranceY} Farm");

            Game1.locations.Add(taintedCellar);
            this.PatchMap(Game1.getFarm());
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            Save();
            Game1.locations.Remove(taintedCellar);
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            Game1.locations.Add(taintedCellar);
        }

        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            taintedCellar = null;
            map = null;
        }

        private void Save()
        {
            string path = Path.Combine(this.Helper.DirectoryPath, "pslocationdata", $"{Constants.SaveFolderName}.xml");

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var writer = XmlWriter.Create(path))
            {
                locationSerializer.Serialize(writer, taintedCellar);
            }
            //monitor.Log($"Object serialized to {path}");
        }

        private void Load()
        {
            taintedCellar = new GameLocation(map, "TaintedCellarMap")
            {
                isOutdoors = false,
                isFarm = true
            };

            string path = Path.Combine(this.Helper.DirectoryPath, "pslocationdata", $"{Constants.SaveFolderName}.xml");

            GameLocation loaded;
            using (var reader = XmlReader.Create(path))
            {
                loaded = (GameLocation)locationSerializer.Deserialize(reader);
            }
            //monitor.Log($"Object deserialized from {path}");
                
            for (int i = loaded.characters.Count - 1; i >= 0; i--)
            {
                if (!loaded.characters[i].DefaultPosition.Equals(Vector2.Zero))
                {
                    loaded.characters[i].position = loaded.characters[i].DefaultPosition;
                }
                loaded.characters[i].currentLocation = taintedCellar;
                if (i < loaded.characters.Count)
                {
                    loaded.characters[i].reloadSprite();
                }
            }
            foreach (TerrainFeature current in loaded.terrainFeatures.Values)
            {
                current.loadSprite();
            }
            foreach (KeyValuePair<Vector2, StardewValley.Object> current in loaded.objects)
            {
                current.Value.initializeLightSource(current.Key);
                current.Value.reloadSprite();
            }

            taintedCellar.characters = loaded.characters;
            taintedCellar.objects = loaded.objects;
            taintedCellar.numberOfSpawnedObjectsOnMap = loaded.numberOfSpawnedObjectsOnMap;
            taintedCellar.terrainFeatures = loaded.terrainFeatures;
            taintedCellar.largeTerrainFeatures = loaded.largeTerrainFeatures;
        }

        /// Patch the farm map to add the cellar entrance.
        private void PatchMap(Farm farm)
        {
            this.Helper.Content.Load<Texture2D>(@"assets\Zpaths_objects_cellar.png");
            farm.map.AddTileSheet(new TileSheet("Zpaths_objects_cellar", farm.map, this.Helper.Content.GetActualAssetKey(@"assets\Zpaths_objects_cellar.png"), new Size(32, 68), new Size(16, 16)));
            farm.map.LoadTileSheets(Game1.mapDisplayDevice);
            if (this.Config.FlipCellarEntrance)
            {
                this.PatchMap(farm, this.GetCellarRightSideEdits());
                int entranceX = 68 + this.Config.XPositionOffset;
                int entranceY1 = 11 + this.Config.YPositionOffset;
                int entranceY2 = 12 + this.Config.YPositionOffset;
                farm.setTileProperty(entranceX, entranceY1, "Buildings", "Action", "Warp 3 4 TaintedCellarMap");
                farm.setTileProperty(entranceX, entranceY2, "Buildings", "Action", "Warp 3 4 TaintedCellarMap");
            }
            else
            {
                this.PatchMap(farm, this.GetCellarLeftSideEdits());
                int entranceX = 58 + this.Config.XPositionOffset;
                int entranceY1 = 11 + this.Config.YPositionOffset;
                int entranceY2 = 12 + this.Config.YPositionOffset;
                farm.setTileProperty(entranceX, entranceY1, "Buildings", "Action", "Warp 3 4 TaintedCellarMap");
                farm.setTileProperty(entranceX, entranceY2, "Buildings", "Action", "Warp 3 4 TaintedCellarMap");
            }
            farm.setTileProperty(68, 11, "Buildings", "Action", "Warp 3 4 TaintedCellarMap");
            farm.setTileProperty(68, 12, "Buildings", "Action", "Warp 3 4 TaintedCellarMap");

            var properties = farm.map.GetTileSheet("Zpaths_objects_cellar").Properties;
            foreach (int tileID in new[] { 1865, 1897, 1866, 1898 })
                properties.Add($"@TileIndex@{tileID}@Passable", new PropertyValue(true));
        }

        /// Get the tiles to change for the right-side cellar entrance.
        private Tile[] GetCellarRightSideEdits()
        {
            string tilesheet = "Zpaths_objects_cellar";
            int x1 = 68 + this.Config.XPositionOffset;
            int x2 = 69 + this.Config.XPositionOffset;
            int y1 = 11 + this.Config.YPositionOffset;
            int y2 = 12 + this.Config.YPositionOffset;
            return new[]
            {
                new Tile(1, x1, y1, 1864, tilesheet),
                new Tile(1, x2, y1, 1865, tilesheet),
                new Tile(1, x1, y2, 1896, tilesheet),
                new Tile(1, x2, y2, 1897, tilesheet)
            };
        }

        /// Get the tiles to change for the right-side cellar entrance.
        private Tile[] GetCellarLeftSideEdits()
        {
            string tilesheet = "Zpaths_objects_cellar";
            int x1 = 57 + this.Config.XPositionOffset;
            int x2 = 58 + this.Config.XPositionOffset;
            int y1 = 11 + this.Config.YPositionOffset;
            int y2 = 12 + this.Config.YPositionOffset;
            return new[]
            {
                new Tile(1, x1, y1, 1866, tilesheet),
                new Tile(1, x2, y1, 1867, tilesheet),
                new Tile(1, x1, y2, 1898, tilesheet),
                new Tile(1, x2, y2, 1899, tilesheet)
            };
        }

        /// Apply a set of map overrides to the farm map.
        private void PatchMap(Farm farm, Tile[] tiles)
        {
            foreach (Tile tile in tiles)
            {
                if (tile.TileIndex < 0)
                {
                    farm.removeTile(tile.X, tile.Y, farm.map.Layers[tile.LayerIndex].Id);
                    farm.waterTiles[tile.X, tile.Y] = false;

                    foreach (LargeTerrainFeature feature in farm.largeTerrainFeatures)
                    {
                        if (feature.tilePosition.X == tile.X && feature.tilePosition.Y == tile.Y)
                            farm.largeTerrainFeatures.Remove(feature);
                    }
                }
                else
                {
                    Layer layer = farm.map.Layers[tile.LayerIndex];
                    xTile.Tiles.Tile mapTile = layer.Tiles[tile.X, tile.Y];
                    if (mapTile == null || mapTile.TileSheet.Id != tile.Tilesheet)
                        layer.Tiles[tile.X, tile.Y] = new StaticTile(layer, farm.map.GetTileSheet(tile.Tilesheet), 0, tile.TileIndex);
                    else
                        farm.setMapTileIndex(tile.X, tile.Y, tile.TileIndex, layer.Id);
                }
            }
        }

        private void UnloadMod()
        {
            SaveEvents.AfterLoad -= this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave -= this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave -= this.SaveEvents_AfterSave;
            SaveEvents.AfterReturnToTitle -= this.SaveEvents_AfterReturnToTitle;
        }
    }
}