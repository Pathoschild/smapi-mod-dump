/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Unlockable_Areas.Menus;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace Unlockable_Areas.Lib
{
    //This class handles applying purchased unlockable areas
    public class UpdateHandler
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static List<UnlockableModel> AppliedUnlockables = new List<UnlockableModel>();

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.Multiplayer.ModMessageReceived += modMessageReceived;
            Helper.Events.Multiplayer.PeerConnected += peerConnected;
            Helper.Events.Player.Warped += warped;
            Helper.Events.GameLoop.ReturnedToTitle += returnedToTitle;
        }

        private static void returnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            AppliedUnlockables = new List<UnlockableModel>();
            ModData.Instance = null;
        }

        private static void warped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name == e.NewLocation.NameOrUniqueName)
                return;

            if (e.NewLocation.mapPath.Value != null && AppliedUnlockables.Any(el => el.Location == e.NewLocation.Name)) {
                //Buildings share the same map, so when entering a building we hard reload it before applying our unlockables
                e.NewLocation.loadMap(e.NewLocation.mapPath.Value, true);

                foreach (var unlockable in AppliedUnlockables.Where(el => el.LocationUnique == e.NewLocation.NameOrUniqueName))
                    applyUnlockable(new Unlockable(unlockable), false);
            }
        }

        private static void peerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            var saveData = ModData.Instance.UnlockablePurchased;
            var unlockables = Helper.GameContent.Load<Dictionary<string, UnlockableModel>>("UnlockableAreas/Unlockables");

            foreach (var keyDicPairs in saveData)
                foreach (var locationValue in keyDicPairs.Value)
                    if (unlockables.TryGetValue(keyDicPairs.Key, out UnlockableModel unlockable)) {
                        unlockable.ID = keyDicPairs.Key;
                        unlockable.LocationUnique = locationValue.Key;

                        if (locationValue.Value == true)
                            ModEntry._Helper.Multiplayer.SendMessage(unlockable, "ApplyUnlockable", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });
                    }

            ModEntry._Helper.Multiplayer.SendMessage(true, "UnlockablesReady", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID }, playerIDs: new[] { e.Peer.PlayerID });
        }

        public static void applyUnlockable(Unlockable unlockable, bool isNew = true)
        {
            if (isNew)
                AppliedUnlockables.Add((UnlockableModel)unlockable);

            if (unlockable.UpdateMap.ToLower() == "none")
                return;

            var map = Helper.GameContent.Load<Map>(unlockable.UpdateMap);
            var location = unlockable.getGameLocation();

            applyOverlay(location, unlockable, map);

            if (location.Name == Game1.player.currentLocation.Name)
                location.Map.LoadTileSheets(Game1.mapDisplayDevice);
        }

        private static void modMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.Type == "IsReady") {
                API.UnlockableAreasAPI.clearCache();
                Helper.GameContent.InvalidateCache(asset => asset.NameWithoutLocale.IsEquivalentTo("UnlockableAreas/Unlockables"));
                ModEntry._API.raiseIsReady(new API.IsReadyEventArgs(Game1.player));
            } 

            if (e.FromModID == Mod.ModManifest.UniqueID && e.Type.StartsWith("ApplyUnlockable")) {
                var unlockable = new Unlockable(e.ReadAs<UnlockableModel>());
                ModData.setUnlockablePurchased(unlockable.ID, unlockable.LocationUnique);

                if (e.Type == "ApplyUnlockable/Purchased")
                    ModEntry._API.raiseShopPurchased(new API.ShopPurchasedEventArgs(Game1.player, unlockable.Location, unlockable.LocationUnique, unlockable.ID, false));

                applyUnlockable(unlockable);

                if (Game1.activeClickableMenu != null
                    && Game1.activeClickableMenu.GetType() == typeof(ShopObjectMenu)
                    && (Game1.activeClickableMenu as ShopObjectMenu).Unlockable.ID == unlockable.ID)
                    Game1.activeClickableMenu.exitThisMenu();
            }

        }

        public static void applyOverlay(GameLocation location, Unlockable unlockable, Map overlayMap)
        {
            addTilesheetsAndLayers(location, unlockable, overlayMap);
            bool isReplaceOverlay = unlockable.UpdateType.ToLower().Equals("replace");

            foreach (var overlayLayer in overlayMap.Layers) {
                int locationX = (int)unlockable.vUpdatePosition.X;

                var locationLayer = location.map.GetLayer(overlayLayer.Id);
                bool isBackLayer = overlayLayer.Id.ToLower().Equals("back");

                for (int overlayX = 0; overlayX < overlayLayer.LayerSize.Width && locationX < locationLayer.LayerSize.Width; overlayX++, locationX++) {
                    int locationY = (int)unlockable.vUpdatePosition.Y;

                    for (int overlayY = 0; overlayY < overlayLayer.LayerSize.Height && locationY < locationLayer.LayerSize.Height; overlayY++, locationY++) {
                        var copyFrom = overlayLayer.Tiles[overlayX, overlayY];

                        if (isReplaceOverlay)
                            locationLayer.Tiles[locationX, locationY] = null;

                        if ((isBackLayer && isReplaceOverlay) || (isBackLayer && copyFrom != null))
                            clearWaterTiles(location, locationX, locationY);

                        if (copyFrom is null)
                            continue;

                        Tile copy = copyFrom is StaticTile
                            ? copyStaticTile(copyFrom as StaticTile, locationLayer, location, unlockable)
                            : copyAnimatedTile(copyFrom as AnimatedTile, locationLayer, location, unlockable);

                        copy.TileIndexProperties.CopyFrom(copyFrom.TileIndexProperties);
                        copy.Properties.CopyFrom(copyFrom.Properties);

                        locationLayer.Tiles[locationX, locationY] = copy;

                        if (isBackLayer)
                            addWaterTiles(location, locationX, locationY);
                    }
                }
            }
        }

        private static void clearWaterTiles(GameLocation location, int x, int y)
        {
            if (location.waterTiles == null)
                return;

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
            StaticTile[] tileFrames = new StaticTile[copyFrom.TileFrames.Length];
            for (int i = 0; i < copyFrom.TileFrames.Length; i++)
                tileFrames[i] = copyStaticTile(copyFrom.TileFrames[i], layer, location, unlockable);

            return new AnimatedTile(layer, tileFrames, copyFrom.FrameInterval);
        }

        private static StaticTile copyStaticTile(StaticTile copyFrom, Layer layer, GameLocation location, Unlockable unlockable)
        {
            return new StaticTile(layer, location.map.GetTileSheet($"zz_{unlockable.ID}_{copyFrom.TileSheet.Id}"), copyFrom.BlendMode, copyFrom.TileIndex);
        }

        public static void addTilesheetsAndLayers(GameLocation location, Unlockable unlockable, Map map)
        {
            if (map == null)
                location.reloadMap();

            foreach (var tileSheet in map.TileSheets) {
                if (location.Map.TileSheets.Any(el => el.Id == $"zz_{unlockable.ID}_{tileSheet.Id}"))
                    continue;

                var newTileSheet = new TileSheet($"zz_{unlockable.ID}_{tileSheet.Id}", location.map, tileSheet.ImageSource, tileSheet.SheetSize, tileSheet.TileSize);
                newTileSheet.Properties.CopyFrom(tileSheet.Properties);
                location.Map.AddTileSheet(newTileSheet);
            }

            foreach (var layer in map.Layers)
                if (!location.map.Layers.Any(el => el.Id == layer.Id)) {
                    var newLayer = new Layer(layer.Id, location.Map, location.map.Layers.First().LayerSize, layer.TileSize);
                    location.map.AddLayer(newLayer);
                }

            location.updateSeasonalTileSheets();
        }
    }
}
