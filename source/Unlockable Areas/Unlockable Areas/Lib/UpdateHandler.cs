/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using xTile;
using xTile.Tiles;
using xTile.Layers;
using Unlockable_Areas.NetLib;
using Unlockable_Areas.Menus;
using Netcode;
using StardewModdingAPI.Events;

namespace Unlockable_Areas.Lib
{
    //This class handles applying purchased unlockable areas
    public class UpdateHandler
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.Multiplayer.ModMessageReceived += modMessageReceived;
            Helper.Events.Multiplayer.PeerConnected += peerConnected;
        }

        private static void peerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            var saveData = SaveDataEvents.Data.UnlockablePurchased;
            var unlockables = Unlockable.convertModelDicToEntity(Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableAreas/Unlockables"));

            foreach (KeyValuePair<string, Unlockable> entry in unlockables) {
                if (!saveData.ContainsKey(entry.Key))
                    saveData[entry.Key] = false;

                if (saveData[entry.Key])
                    ModEntry._Helper.Multiplayer.SendMessage((UnlockableModel)entry.Value, "ApplyUnlockable", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });
            }
        }

        public static void applyUnlockable(Unlockable unlockable)
        {
            var map = Helper.GameContent.Load<Map>(unlockable.UpdateMap);
            var location = Game1.getLocationFromName(unlockable.Location);

            applyOverlay(location, unlockable, map);

            if (location.Name == Game1.player.currentLocation.Name)
                location.reloadMap();
        }

        private static void modMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == Mod.ModManifest.UniqueID && e.Type == "ApplyUnlockable") {
                var unlockable = new Unlockable(e.ReadAs<UnlockableModel>());
                applyUnlockable(unlockable);

                if (SaveDataEvents.Data == null)
                    SaveDataEvents.Data = new ModData();

                SaveDataEvents.Data.UnlockablePurchased[unlockable.ID] = true;

                if (Game1.activeClickableMenu != null
                    && Game1.activeClickableMenu.GetType() == typeof(ShopObjectMenu)
                    && (Game1.activeClickableMenu as ShopObjectMenu).Unlockable.ID == unlockable.ID)
                    Game1.activeClickableMenu.exitThisMenu();
            }

        }

        public static void applyOverlay(GameLocation location, Unlockable unlockable, Map map)
        {
            addTilesheetsAndLayers(location, unlockable, map);

            foreach (var layer in map.Layers) {
                var locationLayer = location.map.GetLayer(layer.Id);
                int locX = (int)unlockable.vUpdatePosition.X;

                for (int x = 0; x < layer.LayerSize.Width && locX < locationLayer.LayerSize.Width; x++, locX++) {
                    int locY = (int)unlockable.vUpdatePosition.Y;

                    for (int y = 0; y < layer.LayerSize.Height && locY < locationLayer.LayerSize.Height; y++, locY++) {
                        var copyFrom = layer.Tiles[x, y];

                        if (unlockable.UpdateType.ToLower() == "replace")
                            locationLayer.Tiles[locX, locY] = null;

                        if ((layer.Id.ToLower() == "back" && unlockable.UpdateType.ToLower() == "replace") || (layer.Id.ToLower() == "back" && copyFrom != null))
                            clearWaterTiles(location, locX, locY);

                        if (copyFrom != null) {
                            Tile copy = null;
                            if (copyFrom.GetType() == typeof(StaticTile))
                                copy = copyStaticTile((copyFrom as StaticTile), locationLayer, location, unlockable);
                            else if (copyFrom.GetType() == typeof(AnimatedTile))
                                copy = copyAnimatedTile((copyFrom as AnimatedTile), locationLayer, location, unlockable);

                            copy.TileIndexProperties.CopyFrom(copyFrom.TileIndexProperties);
                            copy.Properties.CopyFrom(copyFrom.Properties);

                            locationLayer.Tiles[locX, locY] = copy;

                            if (layer.Id.ToLower() == "back")
                                addWaterTiles(location, locX, locY);
                        }
                    }
                }
            }
        }

        private static void clearWaterTiles(GameLocation location, int x, int y)
        {
            //WaterTiles doesn't have the greatest means of accessing its inner arrays for a clean execution, so I'll just catch it if it fails
            try {
                location.waterTiles[x, y] = false;
            } catch { }
            try {
                location.waterTiles.waterTiles[x, y].isWater = false;
            } catch { }

        }

        private static void addWaterTiles(GameLocation location, int x, int y)
        {
            //Replicates how the game handles water
            string water_property = location.doesTileHaveProperty(x, y, "Water", "Back");
            if (water_property != null)
                if (water_property == "I")
                    location.waterTiles.waterTiles[x, y] = new WaterTiles.WaterTileData(is_water: true, is_visible: false);
                else
                    location.waterTiles[x, y] = true;
        }

        private static AnimatedTile copyAnimatedTile(AnimatedTile copyFrom, Layer layer, GameLocation location, Unlockable unlockable)
        {
            var tileFrames = new List<StaticTile>();
            foreach (var tile in copyFrom.TileFrames)
                tileFrames.Add(copyStaticTile(tile, layer, location, unlockable));

            return new AnimatedTile(layer, tileFrames.ToArray(), copyFrom.FrameInterval);
        }

        private static StaticTile copyStaticTile(StaticTile copyFrom, Layer layer, GameLocation location, Unlockable unlockable)
        {
            return new StaticTile(layer, location.map.GetTileSheet($"zz_{unlockable.ID}_{copyFrom.TileSheet.Id}"), copyFrom.BlendMode, copyFrom.TileIndex);
        }

        public static void addTilesheetsAndLayers(GameLocation location, Unlockable unlockable, Map map)
        {
            foreach (var tileSheet in map.TileSheets) {
                var newTileSheet = new TileSheet($"zz_{unlockable.ID}_{tileSheet.Id}", location.map, getSeasonalImageSource(tileSheet.ImageSource), tileSheet.SheetSize, tileSheet.TileSize);
                newTileSheet.Properties.CopyFrom(tileSheet.Properties);
                location.Map.AddTileSheet(newTileSheet);
            }

            foreach (var layer in map.Layers)
                if (!location.map.Layers.Any(el => el.Id == layer.Id)) {
                    var newLayer = new Layer(layer.Id, location.Map, location.map.Layers.First().LayerSize, layer.TileSize);
                    location.map.AddLayer(newLayer);
                }
            return;
        }

        private static string getSeasonalImageSource(string imageSource)
        {
            string newSource = imageSource;

            if (imageSource.ToLower().StartsWith("spring"))
                newSource = Game1.currentSeason + imageSource.Remove(0, 6);
            else if (imageSource.ToLower().StartsWith("summer"))
                newSource = Game1.currentSeason + imageSource.Remove(0, 6);
            else if (imageSource.ToLower().StartsWith("fall"))
                newSource = Game1.currentSeason + imageSource.Remove(0, 4);
            else if (imageSource.ToLower().StartsWith("winter"))
                newSource = Game1.currentSeason + imageSource.Remove(0, 6);

            if (newSource != imageSource) {
                try {
                    var test = Helper.GameContent.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(newSource);

                    if (test == null)
                        throw new Exception();
                } catch {
                    //Assumed seasonal asset does not exist, so we skip this.
                    Monitor.LogOnce($"Attempted to load assumed seasonal Tileset '{newSource}', but failed. Loading '{imageSource}' instead", LogLevel.Trace);
                    newSource = imageSource;
                }
            }

            return newSource;
        }
    }
}
