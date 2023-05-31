/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace RealTunnels
{
    /// <summary>
    ///     Currently working: Iterating through tiles to detect the ones with the appropriate tile properties for purposes of
    ///     enabling/disabling collisions.
    ///     *	Todo regarding above: Make this work on a bridge-by-bridge basis, instead of globally for all of them.
    ///     *	This will also be a huge problem in multiplayer. I have NO idea how I would make the collision toggles and
    ///     drawing work in multiplayer.
    ///     *	Perhaps considering changing the draw order of the player sprite, instead of relying on disabling drawing of
    ///     specific layers?
    /// </summary>
    public class RealTunnelsEntry : Mod
    {
        private static IMonitor _monitor;
        private static bool _insideTunnel;
        private List<GameLocation> _locations = new();
        private readonly Dictionary<GameLocation, List<Tile>> _mapBridgeEndPair = new();

        // TODO: Implement logic to move tiles that are clicked to the above layer so the player can walk under them.
        public override void Entry(IModHelper helper)
        {
            _monitor = this.Monitor;

            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(Layer), nameof(Layer.Draw)),
                new HarmonyMethod(typeof(RealTunnelsEntry), nameof(Layer_DrawNormal_Prefix))
            );

            //harmony.Patch(
            //	original: AccessTools.Method(typeof(GameLocation),
            //	nameof(GameLocation.isCollidingPosition),
            //	new Type[] { typeof(Microsoft.Xna.Framework.Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) }),
            //postfix: new HarmonyMethod(typeof(RealTunnelsEntry),
            //	nameof(RealTunnelsEntry.IsCollidingPosition_Postfix))
            //);

            helper.Events.Input.ButtonPressed += this.InputOnButtonPressed;
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            int timeBefore = DateTime.Now.Millisecond;

            foreach (var location in Game1.locations)
            {
                // We create our tile list.
                var bridgeEndTiles = new List<Tile>();

                foreach (var layer in location.Map.Layers)
                    if (layer.Id.Equals("Buildings"))
                        foreach (var tile in layer.Tiles.Array)
                            if (tile != null)
                            {
                                foreach (var property in tile.Properties)
                                    if (property.Key.Equals("BridgeEnd"))
                                        bridgeEndTiles.Add(tile);

                                foreach (var property in tile.TileIndexProperties)
                                    if (property.Key.Equals("BridgeEnd"))
                                        bridgeEndTiles.Add(tile);
                            }

                this._mapBridgeEndPair.Clear();

                if (bridgeEndTiles.Count > 0)
                    this._mapBridgeEndPair.Add(location, bridgeEndTiles);
                else
                    _monitor.Log($"No BridgeEnd tiles found in location {location.Name}.");
            }

            int timeAfter = DateTime.Now.Millisecond;

            _monitor.Log($"Took {timeBefore - timeAfter}ms to load tiles.");
        }

        private void InputOnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.OemSemicolon)
            {
                _insideTunnel = !_insideTunnel;
                _monitor.Log("Moving from/into tunnel.");
            }

            if (e.Button == SButton.P)
            {
                var location = Game1.currentLocation;

                _monitor.Log("Scanning current location.");

                if (location.Name.Equals("Custom_RTTestMap"))
                {
                    _monitor.Log("In Untiled Map.");

                    foreach (var l in location.Map.Layers)
                        if (l.Id.Equals("Buildings"))
                        {
                            _monitor.Log("Found Buildings layer.");

                            foreach (var t in l.Tiles.Array)
                                if (t != null)
                                {
                                    _monitor.Log($"Tile {t.TileIndex} exists.");

                                    foreach (var p in t.TileIndexProperties)
                                        if (p.Value.Equals("BridgeEnd"))
                                        {
                                            _monitor.Log($"Key: {p.Key}");
                                            _monitor.Log($"Value: {p.Value}");
                                        }

                                    foreach (var property in t.Properties)
                                        if (property.Value.Equals("BridgeEnd"))
                                        {
                                            _monitor.Log($"Key: {property.Key}");
                                            _monitor.Log($"Value: {property.Value}");
                                        }
                                }
                        }
                }
            }

            if (e.Button == SButton.L)
            {
                foreach (var l in Game1.currentLocation.Map.Layers) _monitor.Log($"Layer: {l.Id}");

                var tile = this.GetTile(Game1.currentLocation.Map, "Buildings", (int)Game1.currentCursorTile.X,
                    (int)Game1.currentCursorTile.Y);

                if (tile != null)
                {
                    foreach (var property in tile.Properties)
                    {
                        _monitor.Log($"Cursor tile Property Key: {property.Key}");
                        _monitor.Log($"Cursor tile Property Value: {property.Value}");
                    }

                    foreach (var property in tile.TileIndexProperties)
                    {
                        _monitor.Log($"Cursor tile TileIndexProperty Key: {property.Key}");
                        _monitor.Log($"Cursor tile TileIndexProperty Value: {property.Value}");
                    }

                    //	// get a property value
                    //	string property = tile.TileIndexProperties.TryGetValue("BridgeEnd", out PropertyValue rawProperty) || tile.Properties.TryGetValue("BridgeEnd", out rawProperty)
                    //	? rawProperty?.ToString()
                    //	: null;

                    //	tile.TileIndexProperties.

                    //	if (property != null)
                    //		_monitor.Log($"Cursor tile property BridgeEnd: {property}");
                    //	if (rawProperty != null)
                    //		_monitor.Log($"Cursor tile rawProperty BridgeEnd: {rawProperty}");
                }
            }

            if (e.Button == SButton.OemOpenBrackets)
            {
                var stopwatch = Stopwatch.StartNew();

                foreach (var pair in this._mapBridgeEndPair)
                foreach (var tile in pair.Value)
                    if (tile.Properties.ContainsKey("Passable"))
                        tile.Properties.Remove("Passable");
                    else
                        tile.Properties["Passable"] = "T";

                stopwatch.Stop();

                _monitor.Log($"Took {stopwatch.ElapsedTicks} ticks to flip properties.");
                _monitor.Log($"Took {stopwatch.ElapsedMilliseconds}ms to flip properties.");
            }
        }

        private Tile GetTile(Map map, string layerName, int tileX, int tileY)
        {
            var layer = map.GetLayer(layerName);
            var pixelPosition = new Location(tileX * Game1.tileSize, tileY * Game1.tileSize);

            return layer.PickTile(pixelPosition, Game1.viewport.Size);
        }

        public static void IsCollidingPosition_Postfix(Rectangle position, xTile.Dimensions.Rectangle viewport,
            bool isFarmer, int damagesFarmer, bool glider,
            Character character, bool pathfinding, bool projectile,
            bool ignoreCharacterRequirement, ref bool __result)
        {
            // First, we look for a relevant building tile.
            var tile = Game1.currentLocation.map.GetLayer("Buildings")
                .PickTile(new Location(position.X, position.Y), viewport.Size);

            if (tile != null)
            {
                // Then, if the tile exists, we check to see if it has the tile property we're interested in.
                bool foundProperty = false;

                foreach (var property in tile.Properties)
                    if (property.Key.Equals("BridgeEnd"))
                    {
                        _monitor.Log($"Tile at pos.X: {position.X}, and pos.Y: {position.Y} has property.");

                        foundProperty = true;
                    }

                foreach (var property in tile.TileIndexProperties)
                    if (property.Key.Equals("BridgeEnd"))
                    {
                        _monitor.Log($"Tile at pos.X: {position.X}, and pos.Y: {position.Y} has property.");

                        foundProperty = true;
                    }

                if (foundProperty)
                {
                    _monitor.Log($"BridgeEnd property found on tile at {position.X},{position.Y}.");
                    __result = false;
                }

                //	// get a property value
                //	string property = tile.TileIndexProperties.TryGetValue("BridgeEnd", out PropertyValue rawProperty) || tile.Properties.TryGetValue("BridgeEnd", out rawProperty)
                //	? rawProperty?.ToString()
                //	: null;

                //	tile.TileIndexProperties.

                //	if (property != null)
                //		_monitor.Log($"Cursor tile property BridgeEnd: {property}");
                //	if (rawProperty != null)
                //		_monitor.Log($"Cursor tile rawProperty BridgeEnd: {rawProperty}");
            }
        }

        public static bool Layer_DrawNormal_Prefix(Layer __instance, IDisplayDevice displayDevice,
            xTile.Dimensions.Rectangle mapViewport, Location displayOffset,
            int pixelZoom)
        {
            // _monitor.Log($"__instance.Id: {__instance.Id}");
            // _monitor.Log($"__instance.Map.Id: {__instance.Map.Id}");

            if (__instance.Map.Id.Equals("Untiled map"))
                if (__instance.Id.Equals("AlwaysFront99"))
                    // _monitor.Log("Inside AlwaysFront99.");
                    return !_insideTunnel;

            // if (Game1.currentLocation is Farm)
            // {
            //     if (__instance.Id.Equals("Back"))
            //     {
            //         return true;
            //     }
            // }

            return true;
        }
    }
}
