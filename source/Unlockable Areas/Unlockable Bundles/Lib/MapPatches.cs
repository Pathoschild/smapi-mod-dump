/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unlockable_Bundles.Lib.Enums;
using Unlockable_Bundles.Lib.ShopTypes;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib
{
    public sealed class MapPatches
    {
        public static List<UnlockableModel> AppliedUnlockables = new();
        public static Dictionary<string, TileSheet> CachedTilesheets = new();
        private static Dictionary<string, UnsafeMap> CachedMaps = new();

        public static void Initialize()
        {
            Helper.Events.Player.Warped += warped;
        }

        public static void clearCache()
        {
            AppliedUnlockables.Clear();
        }

        private static void warped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name == e.NewLocation.NameOrUniqueName && !isExceptionLocation(e.NewLocation))
                return;

            if (e.NewLocation.mapPath.Value is null) {
                Monitor.Log($"Player {Game1.player.Name} warped to location {e.NewLocation.Name}:{e.NewLocation.NameOrUniqueName} with invalid mapPath");
                return;
            }

            if (!CachedMaps.ContainsKey(e.NewLocation.Map.assetPath) && !AppliedUnlockables.Any(el => el.Location == e.NewLocation.Name))
                return;

            //Buildings, Cellars and FarmHouses share the same map, so when entering a building we hard reload it before applying our map patches
            if (CachedMaps.ContainsKey(e.NewLocation.Map.assetPath))
                CachedMaps[e.NewLocation.Map.assetPath].PasteData(e.NewLocation);
            else
                e.NewLocation.loadMap(e.NewLocation.mapPath.Value, true);

            foreach (var unlockable in AppliedUnlockables.Where(el => el.LocationUnique == e.NewLocation.NameOrUniqueName))
                applyUnlockable(new Unlockable(unlockable), false);
        }

        public static void applyUnlockable(Unlockable unlockable, bool isNew = true)
        {
            if (isNew)
                AppliedUnlockables.Add((UnlockableModel)unlockable);

            if (unlockable.EditMap.ToLower() == "none")
                return;

            var map = Helper.GameContent.Load<Map>(unlockable.EditMap);
            var location = unlockable.EditMapLocation == "" ? unlockable.getGameLocation() : Game1.getLocationFromName(unlockable.EditMapLocation);

            if (location is null) {
                Monitor.Log($"Skipped applying overlay for '{unlockable.ID}'. Location '{unlockable.LocationUnique}' was null. This can happen for SF buildings in multiplayer");
                return;
            }

            cacheMapIfNecessary(location, isNew);

            if ((location.Name != location.NameOrUniqueName || isExceptionLocation(location)) && Game1.currentLocation.NameOrUniqueName != location.NameOrUniqueName)
                return;

            //Alternative to applyOverlay: applySmapiOverlay(map, unlockable, location)
            //I found my own solution to be faster in testing and more reliable with water and seasons

            applyOverlay(location, unlockable, map);
            Monitor.Log($"Applied map patch for {Multiplayer.getDebugName()}: {unlockable.ID} at {unlockable.EditMapPosition} {location.NameOrUniqueName}", DebugLogLevel);

            if (location.Name == Game1.currentLocation?.Name)
                location.Map.LoadTileSheets(Game1.mapDisplayDevice);
        }

        private static void applySmapiOverlay(Map map, Unlockable unlockable, GameLocation location)
        {
            var sourceArea = new Rectangle(0, 0, map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
            var targetArea = new Rectangle((int)unlockable.EditMapPosition.X, (int)unlockable.EditMapPosition.Y, map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
            var patchHelper = Helper.ModContent.GetPatchHelper(location.Map).AsMap();

            patchHelper.ExtendMap(targetArea.Right, targetArea.Bottom);
            patchHelper.PatchMap(map, sourceArea, targetArea, unlockable.EditMapMode);
            location.updateSeasonalTileSheets();
        }

        private static void cacheMapIfNecessary(GameLocation location, bool isNew)
        {
            if (!isNew
                || (location.Name == location.NameOrUniqueName && !isExceptionLocation(location))
                || CachedMaps.ContainsKey(location.Map.assetPath))
                return;

            var map = new UnsafeMap(location);
            CachedMaps.Add(location.Map.assetPath, map);
        }


        //Cellar, Cellar2, Cellar3 etc.
        //FarmHouse, 
        public static bool isExceptionLocation(GameLocation location) => location is Cellar or FarmHouse or Cabin;

        public static void applyOverlay(GameLocation location, Unlockable unlockable, Map overlayMap)
        {
            addTilesheetsAndLayers(location, unlockable, overlayMap);
            bool isReplaceOverlay = unlockable.EditMapMode is PatchMapMode.Replace or PatchMapMode.ReplaceByLayer;

            foreach (var overlayLayer in overlayMap.Layers) {
                int locationX = (int)unlockable.EditMapPosition.X;

                var locationLayer = location.map.GetLayer(overlayLayer.Id);
                bool isBackLayer = overlayLayer.Id.ToLower().Equals("back");

                for (int overlayX = 0; overlayX < overlayLayer.LayerSize.Width && locationX < locationLayer.LayerSize.Width; overlayX++, locationX++) {
                    int locationY = (int)unlockable.EditMapPosition.Y;

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

                        //Should be fine as is, but maybe I should move the TileIndexProperties to addTilesheetsAndLayers?
                        copy.TileSheet.TileIndexProperties[copy.TileIndex].CopyFrom(copyFrom.TileSheet.TileIndexProperties[copyFrom.TileIndex]);
                        copy.Properties.CopyFrom(copyFrom.Properties);

                        locationLayer.Tiles[locationX, locationY] = copy;

                        if (isBackLayer)
                            addWaterTiles(location, locationX, locationY);
                    }
                }
            }

            if (unlockable.EditMapMode == PatchMapMode.Replace)
                clearNonOverlappingLayers(location, unlockable, overlayMap);

            CachedTilesheets.Clear();
        }

        private static void clearNonOverlappingLayers(GameLocation location, Unlockable unlockable, Map overlayMap)
        {
            var nonOverlapping = location.Map.Layers.Where(e => overlayMap.GetLayer(e.Id) == null);

            int width = overlayMap.DisplayWidth / 64;
            int height = overlayMap.DisplayHeight / 64;

            foreach (var locationLayer in nonOverlapping) {
                bool isBackLayer = locationLayer.Id.ToLower().Equals("back");
                int locationX = (int)unlockable.EditMapPosition.X;

                for (int overlayX = 0; overlayX < width && locationX < locationLayer.LayerSize.Width; overlayX++, locationX++) {
                    int locationY = (int)unlockable.EditMapPosition.Y;

                    for (int overlayY = 0; overlayY < height && locationY < locationLayer.LayerSize.Height; overlayY++, locationY++) {

                        if (isBackLayer)
                            clearWaterTiles(location, locationX, locationY);
                        locationLayer.Tiles[locationX, locationY] = null;
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
            var key = $"zz_{unlockable.ID}_{copyFrom.TileSheet.Id}";
            TileSheet tilesheet;

            if (CachedTilesheets.TryGetValue(key, out var cachedTilesheet)) {
                tilesheet = cachedTilesheet;
            } else {
                tilesheet = location.map.GetTileSheet(key);
                CachedTilesheets.Add(key, tilesheet);
            }

            return new StaticTile(layer, tilesheet, copyFrom.BlendMode, copyFrom.TileIndex);
        }

        public static void addTilesheetsAndLayers(GameLocation location, Unlockable unlockable, Map overlayMap)
        {
            if (overlayMap == null)
                location.reloadMap();

            foreach (var tileSheet in overlayMap.TileSheets) {
                if (location.Map.TileSheets.Any(el => el.Id == $"zz_{unlockable.ID}_{tileSheet.Id}"))
                    continue;

                var newTileSheet = new TileSheet($"zz_{unlockable.ID}_{tileSheet.Id}", location.map, tileSheet.ImageSource, tileSheet.SheetSize, tileSheet.TileSize);
                newTileSheet.Properties.CopyFrom(tileSheet.Properties);
                location.Map.AddTileSheet(newTileSheet);
                CachedTilesheets.Add(newTileSheet.Id, newTileSheet);
            }

            foreach (var layer in overlayMap.Layers)
                if (!location.map.Layers.Any(el => el.Id == layer.Id)) {
                    var newLayer = new Layer(layer.Id, location.Map, location.map.Layers.First().LayerSize, layer.TileSize);
                    location.map.AddLayer(newLayer);
                }

            location.updateSeasonalTileSheets();
        }

        public static void checkIfMapPatchesNeedToBeReapplied(GameLocation location, string source = "")
        {
            if (!Context.IsWorldReady)
                return;

            if (!ShopPlacement.HasDayStarted)
                return;

            //We already check for structures at MapPatches.warped
            if (location.Name != location.NameOrUniqueName || isExceptionLocation(location))
                return;

            var needToBeApplied = AppliedUnlockables.Where(el => el.LocationUnique == location.NameOrUniqueName);

            if (!needToBeApplied.Any())
                return;

            Monitor.Log($"{source} called for {location.Name} by {Multiplayer.getDebugName()} after HasDayStarted. Possibly reapplying map patches", DebugLogLevel);

            if (Multiplayer.IsScreenReady.Value && !ShopPlacement.UnappliedMapPatches.Value.Any()) {
                foreach (var unlockable in needToBeApplied)
                    applyUnlockable(new Unlockable(unlockable), false);
            }

            if (Context.ScreenId == 1)
                Helper.Multiplayer.SendMessage(location.NameOrUniqueName, "ReapplyLocation", modIDs: new[] { ModManifest.UniqueID }, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID });

        }
    }
}
