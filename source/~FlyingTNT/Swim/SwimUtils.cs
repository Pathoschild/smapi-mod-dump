/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Swim
{
    internal class SwimUtils
    {
        private static IMonitor SMonitor;
        private static ModConfig Config;
        private static IModHelper SHelper;
        public static Dictionary<string, string> seaMonsterSounds = new Dictionary<string, string>() {
            {"A","dialogueCharacter"},
            {"B","grunt"},
            {"C","throwDownITem"},
            {"D","stoneStep"},
            {"E","thudStep"},
            {"F","toolSwap"},
            {"G","bob"},
            {"H","dwoop"},
            {"I","ow"},
            {"J","breathin"},
            {"K","boop"},
            {"L","flute"},
            {"M","backpackIN"},
            {"N","croak"},
            {"O","flybuzzing"},
            {"P","skeletonStep"},
            {"Q","dustMeep"},
            {"R","throw"},
            {"S","shadowHit"},
            {"T","slingshot"},
            {"U","dwop"},
            {"V","fishingRodBend"},
            {"W","Cowboy_Footstep"},
            {"X","junimoMeep1"},
            {"Y","fallDown"},
            {"Z","harvest"},
        };

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            SMonitor = monitor;
            Config = config;
            SHelper = helper;
        }

        public static Point GetEdgeWarpDestination(int idxPos, EdgeWarp edge)
        {
            try
            {
                int idx = 1 + idxPos - edge.FirstTile;
                int length = 1 + edge.LastTile - edge.FirstTile;
                int otherLength = 1 + edge.OtherMapLastTile - edge.OtherMapFirstTile;
                int otherIdx = (int)Math.Round((idx / (float)length) * otherLength);
                int tileIdx = edge.OtherMapFirstTile - 1 + otherIdx;
                if (edge.DestinationHorizontal == true)
                {
                    SMonitor.Log($"idx {idx} length {length} otherIdx {otherIdx} tileIdx {tileIdx} warp point: {tileIdx},{edge.OtherMapIndex}");
                    return new Point(tileIdx, edge.OtherMapIndex);
                }
                else
                {
                    SMonitor.Log($"warp point: {edge.OtherMapIndex},{tileIdx}");
                    return new Point(edge.OtherMapIndex, tileIdx);
                }
            }
            catch
            {

            }
            return Point.Zero;
        }

        public static void DiveTo(DiveLocation diveLocation)
        {
            DivePosition dp = diveLocation.OtherMapPos;
            if (dp == null)
            {
                SMonitor.Log($"Diving to existing tile position");
                Point pos = Game1.player.TilePoint;
                dp = new DivePosition()
                {
                    X = pos.X,
                    Y = pos.Y
                };
            }

            if (Game1.getLocationFromName(diveLocation.OtherMapName) is not GameLocation location || !IsValidDiveLocation(location, new Vector2(dp.X, dp.Y)))
            {
                SMonitor.Log($"Invalid dive location: {diveLocation.OtherMapName} ({dp.X}, {dp.Y})");
                return;
            }

            if (!IsMapUnderwater(Game1.player.currentLocation.Name))
            {
                ModEntry.bubbles.Value.Clear();
            }
            else
            {
                Game1.changeMusicTrack("none", false, StardewValley.GameData.MusicContext.Default);
            }

            Game1.playSound("pullItemFromWater");
            Game1.warpFarmer(diveLocation.OtherMapName, dp.X, dp.Y, false);
        }

        public static bool IsValidDiveLocation(GameLocation map, Vector2 location)
        {
            return map.isTileOnMap(location) && (!map.IsTileBlockedBy(location, CollisionMask.Buildings, CollisionMask.Buildings) || IsWaterTile(location, map)) && map.getTileIndexAt((int)location.X, (int)location.Y, "Back") != -1;
        }

        public static int MaxOxygen()
        {
            return Game1.player.MaxStamina * Math.Max(1, Config.OxygenMult);
        }

        public static bool IsMapUnderwater(string name)
        {
            return ModEntry.diveMaps.ContainsKey(name) && ModEntry.diveMaps[name].Features.Contains("Underwater");
        }

        // Replaces myButtonDown because this is what that variable really did
        public static bool isSafeToTryJump()
        {
            // Null checks
            if (Game1.player == null || Game1.player.currentLocation == null || Game1.player.currentLocation.waterTiles == null)
            {
                return false;
            }

            // Player state checks
            if (!Context.IsPlayerFree || !Context.CanPlayerMove || Game1.player.isRidingHorse())
            {
                return false;
            }

            // Modded player state checks
            if (Game1.player.millisecondsPlayed - SwimHelperEvents.lastJump.Value < 250 || IsMapUnderwater(Game1.player.currentLocation.Name))
            {
                return false;
            }

            // Player input checks
            if (!((Game1.player.isMoving() && Config.ReadyToSwim) || (Config.ManualJumpButton.IsDown() && Config.EnableClickToSwim)) || Config.PreventJumpButton.IsDown())
            {
                return false;
            }

            // Don't let them jump into water in the night market (still let them leave the water tho)
            if(Game1.player.currentLocation is BeachNightMarket && !Game1.player.swimming.Value)
            {
                return false;
            }

            return true;
        }

        private static readonly PerScreen<bool> surfacing = new PerScreen<bool>();
        public static void updateOxygenValue()
        {
            if (ModEntry.isUnderwater.Value)
            {
                if (ModEntry.oxygen.Value >= 0)
                {
                    if (!IsWearingScubaGear())
                        ModEntry.oxygen.Value--;
                    else
                    {
                        if (ModEntry.oxygen.Value < MaxOxygen())
                            ModEntry.oxygen.Value++;
                        if (ModEntry.oxygen.Value < MaxOxygen())
                            ModEntry.oxygen.Value++;
                    }
                }
                if (ModEntry.oxygen.Value < 0 && !surfacing.Value)
                {
                    surfacing.Value = true;
                    Game1.playSound("pullItemFromWater");
                    DiveLocation diveLocation = ModEntry.diveMaps[Game1.player.currentLocation.Name].DiveLocations.Last();
                    DiveTo(diveLocation);
                }
            }
            else
            {
                surfacing.Value = false;
                if (ModEntry.oxygen.Value < MaxOxygen())
                    ModEntry.oxygen.Value++;
                if (ModEntry.oxygen.Value < MaxOxygen())
                    ModEntry.oxygen.Value++;
            }
        }

        public static int CheckForBuriedItem(Farmer who)
        {
            int objectIndex = 330;
            if (Game1.random.NextDouble() < 0.1)
            {
                if (Game1.random.NextDouble() < 0.75)
                {
                    switch (Game1.random.Next(5))
                    {
                        case 0:
                            objectIndex = 96;
                            break;
                        case 1:
                            objectIndex = (who.hasOrWillReceiveMail("lostBookFound") ? ((Game1.netWorldState.Value.LostBooksFound < 21) ? 102 : 770) : 770);
                            break;
                        case 2:
                            objectIndex = 110;
                            break;
                        case 3:
                            objectIndex = 112;
                            break;
                        case 4:
                            objectIndex = 585;
                            break;
                    }
                }
                else if (Game1.random.NextDouble() < 0.75)
                {
                    var r = Game1.random.NextDouble();

                    if (r < 0.75)
                    {
                        objectIndex = ((Game1.random.NextDouble() < 0.5) ? 121 : 97);
                    }
                    else if (r < 0.80)
                    {
                        objectIndex = 99;
                    }
                    else
                    {
                        objectIndex = ((Game1.random.NextDouble() < 0.5) ? 122 : 336);
                    }
                }
                else
                {
                    objectIndex = ((Game1.random.NextDouble() < 0.5) ? 126 : 127);
                }
            }
            else
            {
                if (Game1.random.NextDouble() < 0.5)
                {
                    objectIndex = 330;
                }
                else
                {
                    if (Game1.random.NextDouble() < 0.25)
                    {
                        objectIndex = 749;
                    }
                    else if (Game1.random.NextDouble() < 0.5)
                    {
                        var r = Game1.random.NextDouble();
                        if (r < 0.7)
                        {
                            objectIndex = 535;
                        }
                        else if (r < 8.5)
                        {
                            objectIndex = 537;
                        }
                        else
                        {
                            objectIndex = 536;
                        }
                    }
                }
            }
            return objectIndex;
        }

        public static bool IsWearingScubaGear()
        {
            bool tank = Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.ItemId == ModEntry.scubaTankID.Value;
            bool mask = Game1.player.hat.Value != null && Game1.player.hat.Value.ItemId == ModEntry.scubaMaskID.Value;

            return tank && mask;
        }

        public static bool IsInWater()
        {
            WaterTiles tiles = Game1.player.currentLocation.waterTiles;
            Point p = Game1.player.TilePoint;

            if (!Game1.player.swimming.Value && Game1.player.currentLocation.map.GetLayer("Buildings")?.PickTile(new Location(p.X, p.Y) * Game1.tileSize, Game1.viewport.Size) != null)
            {
                //Monitor.Log("Not in water");
                return false;
            }

            bool output = IsMapUnderwater(Game1.player.currentLocation.Name)
                ||
                (
                    tiles != null
                    &&
                    (
                        (p.X >= 0 && p.Y >= 0 && tiles.waterTiles.GetLength(0) > p.X && tiles.waterTiles.GetLength(1) > p.Y && tiles[p.X, p.Y])
                        ||
                        (
                            Game1.player.swimming.Value
                            &&
                            (p.X < 0 || p.Y < 0 || tiles.waterTiles.GetLength(0) <= p.X || tiles.waterTiles.GetLength(1) <= p.Y)
                        )
                    )
                );

            //Monitor.Log(output ? "In water" : "Not in water");
            return output;
        }

        public static List<Vector2> GetTilesInDirection(int count, int direction)
        {
            List<Vector2> tiles = new List<Vector2>();
            if (direction == 1)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(Game1.player.TilePoint.ToVector2() + new Vector2(i, 0));
                }

            }

            if (direction == 2)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(Game1.player.TilePoint.ToVector2() + new Vector2(0, i));
                }

            }

            if (direction == 3)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(Game1.player.TilePoint.ToVector2() - new Vector2(i, 0));
                }

            }

            if (direction == 0)
            {

                for (int i = count; i > 0; i--)
                {
                    tiles.Add(Game1.player.TilePoint.ToVector2() - new Vector2(0, i));
                }

            }

            return tiles;

        }

        public static Vector2 GetNextTile()
        {
            int dir = Game1.player.FacingDirection;
            if (dir == 1)
            {

                return Game1.player.Tile + new Vector2(1, 0);

            }

            if (dir == 2)
            {

                return Game1.player.Tile + new Vector2(0, 1);

            }

            if (dir == 3)
            {

                return Game1.player.Tile - new Vector2(1, 0);

            }

            if (dir == 0)
            {

                return Game1.player.Tile - new Vector2(0, 1);
            }
            return Vector2.Zero;
        }

        public static void MakeOxygenBar(int current, int max)
        {
            ModEntry.OxygenBarTexture.Value = new Texture2D(Game1.graphics.GraphicsDevice, (int)Math.Round(Game1.viewport.Width * 0.74f), 30);
            Color[] data = new Color[ModEntry.OxygenBarTexture.Value.Width * ModEntry.OxygenBarTexture.Value.Height];
            ModEntry.OxygenBarTexture.Value.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                if (i <= ModEntry.OxygenBarTexture.Value.Width || i % ModEntry.OxygenBarTexture.Value.Width == ModEntry.OxygenBarTexture.Value.Width - 1)
                {
                    data[i] = new Color(0.5f, 1f, 0.5f);
                }
                else if (data.Length - i < ModEntry.OxygenBarTexture.Value.Width || i % ModEntry.OxygenBarTexture.Value.Width == 0)
                {
                    data[i] = new Color(0, 0.5f, 0);
                }
                else if ((i % ModEntry.OxygenBarTexture.Value.Width) / (float)ModEntry.OxygenBarTexture.Value.Width < (float)current / (float)max)
                {
                    data[i] = Color.GhostWhite;
                }
                else
                {
                    data[i] = Color.Black;
                }
            }
            ModEntry.OxygenBarTexture.Value.SetData(data);
        }

        public static string doesTileHaveProperty(Map map, int xTile, int yTile, string propertyName, string layerName)
        {
            PropertyValue property = null;
            if (map != null && map.GetLayer(layerName) != null)
            {
                Tile tmp = map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size);
                if (tmp != null)
                {
                    tmp.TileIndexProperties.TryGetValue(propertyName, out property);
                }
                if (property == null && tmp != null)
                {
                    map.GetLayer(layerName).PickTile(new Location(xTile * 64, yTile * 64), Game1.viewport.Size).Properties.TryGetValue(propertyName, out property);
                }
            }
            if (property != null)
            {
                //Monitor.Log("Tile has property: " + property.ToString());
                return property.ToString();
            }
            return null;
        }

        public static void ReadDiveMapData(DiveMapData data)
        {
            foreach (DiveMap map in data.Maps)
            {
                if (!ModEntry.diveMaps.ContainsKey(map.Name))
                {
                    ModEntry.diveMaps.Add(map.Name, map);
                    SMonitor.Log($"added dive map info for {map.Name}", LogLevel.Debug);
                }
                else
                {
                    SMonitor.Log($"dive map info already exists for {map.Name}", LogLevel.Trace);
                }
            }
        }
        public static async void SeaMonsterSay(string speech)
        {
            foreach (char c in speech)
            {
                string s = c.ToString().ToUpper();
                if (seaMonsterSounds.ContainsKey(s))
                {
                    Game1.playSound("junimoMeep1", (seaMonsterSounds.Keys.ToList().IndexOf(s) / 26) * 2 - 1);
                }
                await Task.Delay(100);
            }
        }

        public static bool IsWaterTile(Vector2 tilePos)
        {
            return IsWaterTile(tilePos, Game1.player.currentLocation);
        }

        public static bool IsWaterTile(Vector2 tilePos, GameLocation location)
        {
            if (location != null && location.waterTiles != null && tilePos.X >= 0 && tilePos.Y >= 0 && location.waterTiles.waterTiles.GetLength(0) > tilePos.X && location.waterTiles.waterTiles.GetLength(1) > tilePos.Y)
            {
                return location.waterTiles[(int)tilePos.X, (int)tilePos.Y];
            }
            return false;
        }

        public static bool IsTilePassable(GameLocation location, Location tileLocation, xTile.Dimensions.Rectangle viewport)
        {
            PropertyValue passable = null;
            Microsoft.Xna.Framework.Rectangle tileLocationRect = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
            Tile tmp = location.map.GetLayer("Back").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tmp != null)
            {
                tmp.TileIndexProperties.TryGetValue("Passable", out passable);
            }
            Tile tile = location.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (location.largeTerrainFeatures is not null)
            {
                using (List<LargeTerrainFeature>.Enumerator enumerator = location.largeTerrainFeatures.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.getBoundingBox().Intersects(tileLocationRect))
                        {
                            return false;
                        }
                    }
                }
            }
            Vector2 vLocation = new Vector2(tileLocation.X, tileLocation.Y);
            if (location.terrainFeatures.TryGetValue(vLocation, out TerrainFeature feature) && feature != null && tileLocationRect.Intersects(feature.getBoundingBox()) && (!feature.isPassable(null) || (feature is HoeDirt && ((HoeDirt)feature).crop != null)))
            {
                return false;
            }
            bool result = passable == null && tile == null && tmp != null;
            return result;
        }

        public static bool isMouseButtonDown(KeybindList keybindList)
        {
            if(keybindList.GetKeybindCurrentlyDown() is not Keybind keybind)
                return false;

            if (keybind.Buttons.Length == 0)
                return false;

            return keybind.Buttons[0] == SButton.MouseLeft || keybind.Buttons[0] == SButton.MouseRight || keybind.Buttons[0] == SButton.MouseMiddle || keybind.Buttons[0] == SButton.MouseX1 || keybind.Buttons[0] == SButton.MouseX2;
        }
        public static bool DebrisIsAnItem(Debris debris)
        {
            return debris.debrisType.Value == Debris.DebrisType.OBJECT || debris.debrisType.Value == Debris.DebrisType.ARCHAEOLOGY || debris.debrisType.Value == Debris.DebrisType.RESOURCE || debris.item != null;
        }

        internal static bool CanSwimHere()
        {
            GameLocation location = Game1.player.currentLocation;
            bool result = (!Config.SwimIndoors || location.IsOutdoors) && location is not VolcanoDungeon && location is not BoatTunnel && !ModEntry.locationIsPool.Value;
            if (!result)
                return false;

            Point playerPosition = Game1.player.TilePoint;

            string property = doesTileHaveProperty(location.Map, playerPosition.X, playerPosition.Y, "TouchAction", "Back");

            if (property == "PoolEntrance" || property == "ChangeIntoSwimsuit")
            {
                SMonitor.Log("The current tile is a pool entrance! Disabling swimming in this location.");
                ModEntry.locationIsPool.Value = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the direction of one point relative to another.
        /// 
        /// Point 1 is the starting point, and point 2 the endpoint.
        /// Returns a cardinal direction using Stardew Valley's direction system (0 is up, 1 right, 2 down, and 3 left)
        /// </summary>
        /// <param name="x1">The x coordinate of the first point.</param>
        /// <param name="y1">The y coordinate of the first point.</param>
        /// <param name="x2">The x coordinate of the second point.</param>
        /// <param name="y2">The y coordinate of the second point.</param>
        /// <returns>A cardinal direction using Stardew Valley's direction system (0 is up, 1 right, 2 down, and 3 left)</returns>
        public static int GetDirection(float x1, float y1, float x2, float y2)
        {
            if (Math.Abs(x1 - x2) > Math.Abs(y1 - y2))
            {
                if (x2 - x1 > 0)
                {
                    return 1; // Right
                }
                else
                {
                    return 3; // Left
                }
            }
            else
            {
                if (y2 - y1 > 0)
                {
                    return 2; // Down
                }
                else
                {
                    return 0; // Up
                }
            }
        }
    }
}