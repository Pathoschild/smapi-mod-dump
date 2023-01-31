/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Layers;

namespace AeroCore.Utils
{
    [ModInit]
    public static class Maps
    {
        private static readonly PerScreen<bool> skipFade = new();

        public static GameLocation EventVoid => evoid.Value;
        private static readonly Lazy<GameLocation> evoid = new(() => CreateTempLocation("Maps/EventVoid"));

        internal static void Init()
        {
            Patches.FadeHooks.AfterFadeOut += checkSkipFadeIn;
        }

        private static void checkSkipFadeIn(int screen)
        {
            if (skipFade.Value)
            {
                var fade = Patches.FadeHooks.gameFade.Value;
                if (fade is not null)
                    fade.fadeToBlackAlpha = -1f;
			}
            skipFade.Value = false;
        }

        public static string[] MapPropertyArray(this GameLocation loc, string prop) => 
            loc.getMapProperty(prop).Split(' ', StringSplitOptions.RemoveEmptyEntries);

        public static bool TryGetMapProperty(this GameLocation loc, string name, out string prop)
        {
            prop = string.Empty;
            if (loc is null || loc.Map is null)
                return false;
            if (!loc.Map.Properties.TryGetValue(name, out var p))
                return false;
            prop = p.ToString();
            return true;
        }
        public static bool TryGetMapPropertyArray(this GameLocation loc, string name, out string[] prop)
        {
            if(TryGetMapProperty(loc, name, out var p))
            {
                prop = p.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return true;
            }
            prop = Array.Empty<string>();
            return false;
        }

        public static IEnumerable<(xTile.Tiles.Tile, int, int)> TilesInLayer(Layer layer)
        {
            if (layer == null)
                yield break;

            for (int x = 0; x < layer.LayerWidth; x++)
            {
                for (int y = 0; y < layer.LayerHeight; y++)
                {
                    var tile = layer.Tiles[x, y];
                    if (tile != null)
                    {
                        yield return (tile, x, y);
                    }
                }
            }
        }
        public static IEnumerable<(xTile.Tiles.Tile, int, int)> TilesInLayer(this xTile.Map map, string layerName)
        {
            foreach (var item in TilesInLayer(map.GetLayer(layerName)))
                yield return item;
        }
        public static bool TileHasProperty(this xTile.Tiles.Tile tile, string name, out string prop)
        {
            bool ret = tile.Properties.TryGetValue(name, out var val) || tile.TileIndexProperties.TryGetValue(name, out val);
            prop = val?.ToString();
            return ret;
        }
        public static void WarpToTempMap(string path, Farmer who)
        {
            GameLocation temp = new(PathUtilities.NormalizeAssetName("Maps/" + path), "Temp");
            temp.map.LoadTileSheets(Game1.mapDisplayDevice);
            Event e = Game1.currentLocation.currentEvent;
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation.currentEvent = null;
            Game1.currentLightSources.Clear();
            Game1.currentLocation = temp;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.currentLocation.currentEvent = e;
            Game1.player.currentLocation = Game1.currentLocation;
            who.currentLocation = Game1.currentLocation;
            Game1.panScreen(0, 0);
        }
        public static LocationRequest RequestLocationOf(GameLocation location)
            => new(location.NameOrUniqueName, false, location);
        public static GameLocation CreateTempLocation(string path)
        {
            GameLocation loc = new(PathUtilities.NormalizeAssetName(path), "Temp");
            loc.map.LoadTileSheets(Game1.mapDisplayDevice);
            return loc;
        }
        public static LocationRequest GetTempLocation(string path)
            => RequestLocationOf(CreateTempLocation(path));
        public static void QuickWarp(LocationRequest request, int x, int y, int facing)
        {
            skipFade.Value = true;
            Game1.warpFarmer(request, x, y, facing);
            Patches.FadeHooks.gameFade.Value.fadeToBlackAlpha = 2f;
        }
        public static void QuickWarp(string name, int x, int y, bool flip)
            => QuickWarp(Game1.getLocationRequest(name), x, y, flip ? ((Game1.player.FacingDirection + 2) % 4) : Game1.player.FacingDirection);
        public static void QuickWarp(string name, int x, int y, int facing)
            => QuickWarp(Game1.getLocationRequest(name), x, y, facing);
        public static void QuickWarp(string name, int x, int y, int facing, bool structure)
            => QuickWarp(Game1.getLocationRequest(name, structure), x, y, facing);
        public static void ReloadCurrentLocation(bool quick = false)
        {
            if (Game1.player is null)
                return;

            var pos = Game1.player.getTileLocationPoint();
            var loc = Game1.player.currentLocation;
            var facing = Game1.player.FacingDirection;
            var screen = Context.ScreenId;

            ModEntry.api.AfterThisFadeIn(() =>
            {
                Misc.NextGameTick += () =>
                {
                    if (quick)
                        skipFade.Value = true;
                    Game1.warpFarmer(RequestLocationOf(loc), pos.X, pos.Y, facing);
                    Patches.FadeHooks.gameFade.Value.fadeToBlackAlpha = 2f;
                };
            });

            skipFade.Value = true;
            Game1.warpFarmer(RequestLocationOf(EventVoid), 0, 0, 0);
            if (quick)
                Patches.FadeHooks.gameFade.Value.fadeToBlackAlpha = 2f;
        }
        public static Point ToPoint(this xTile.Dimensions.Location loc) => new(loc.X, loc.Y);
        public static Rectangle ToRect(this xTile.Dimensions.Rectangle rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
        public static Point ToPoint(this xTile.Dimensions.Size size) => new(size.Width, size.Height);
        public static Vector2 ToVector2(this xTile.Dimensions.Location loc) => new(loc.X,loc.Y);
        public static Vector2 ToVector2(this xTile.Dimensions.Size size) => new(size.Width, size.Height);

        public static int GetSeasonIndexForLocation(this GameLocation loc)
            => Utility.getSeasonNumber(loc.seasonOverride is null ? loc.GetSeasonForLocation() : loc.seasonOverride);

        public static ResourceClump ResourceClumpAt(this GameLocation loc, Vector2 position)
        {
            if (loc is null)
                return null;
            foreach(var clump in loc.resourceClumps)
                if (clump.currentTileLocation == position)
                    return clump;
            return null;
        }
        public static ResourceClump ResourceClumpIntersecting(this GameLocation loc, int x, int y)
        {
            if (loc is null)
                return null;
            foreach (var clump in loc.resourceClumps)
                if (clump.occupiesTile(x, y))
                    return clump;
            return null;
        }
    }
}
