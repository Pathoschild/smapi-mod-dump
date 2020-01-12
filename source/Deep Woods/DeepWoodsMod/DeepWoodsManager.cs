using System;
using System.Collections.Generic;
using DeepWoodsMod.Framework.Messages;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsEnterExit;
using static DeepWoodsMod.DeepWoodsGlobals;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    class DeepWoodsManager
    {
        public static DeepWoods currentDeepWoods = null;
        public static string currentWarpRequestName = null;
        public static Vector2? currentWarpRequestLocation = null;


        private static DeepWoods rootDeepWoodsBackup = null;
        private static bool lostMessageDisplayedToday = false;
        private static int nextRandomizeTime = 0;


        public static void AddExitLocation(DeepWoods deepWoods, Location tile, DeepWoodsExit exit)
        {
            if (!Game1.IsMasterGame)
                return;

            if (deepWoods == null)
                return;

            deepWoods.AddExitLocation(tile, exit);
        }

        public static void RemoveExitLocation(DeepWoods deepWoods, Location tile)
        {
            if (!Game1.IsMasterGame)
                return;

            if (deepWoods == null)
                return;

            deepWoods.RemoveExitLocation(tile);
        }



        public static void WarpFarmerIntoDeepWoods(int level)
        {
            // Warp into root level if appropriate.
            if (level <= 1)
            {
                Game1.warpFarmer("DeepWoods", Settings.Map.RootLevelEnterLocation.X, Settings.Map.RootLevelEnterLocation.Y, false);
            }
            else if (!Game1.IsMasterGame)
            {
                ModEntry.SendMessage(level, MessageId.RequestWarp, Game1.MasterPlayer.UniqueMultiplayerID);
            }
            else
            {
                WarpFarmerIntoDeepWoods(AddDeepWoodsFromObelisk(level));
            }
        }

        public static DeepWoods AddDeepWoodsFromObelisk(int level)
        {
            if (!Game1.IsMasterGame)
                throw new ApplicationException("Illegal call to DeepWoodsManager.AddDeepWoodsFromObelisk in client.");

            // First check if a level already exists and use that.
            foreach (GameLocation gameLocation in Game1.locations)
            {
                if (gameLocation is DeepWoods && (gameLocation as DeepWoods).level.Value == level)
                {
                    return gameLocation as DeepWoods;
                }
            }

            // Otherwise create a new level.
            DeepWoods deepWoods = new DeepWoods(level);
            DeepWoodsManager.AddDeepWoodsToGameLocations(deepWoods);
            return deepWoods;
        }

        public static void WarpFarmerIntoDeepWoodsFromServerObelisk(string name, Vector2 enterLocation)
        {
            Game1.player.FacingDirection = DeepWoodsEnterExit.EnterDirToFacingDirection(EnterDirection.FROM_TOP);
            Game1.warpFarmer(name, (int)enterLocation.X, (int)enterLocation.Y + 1, false);
        }

        public static void WarpFarmerIntoDeepWoods(DeepWoods deepWoods)
        {
            if (deepWoods == null)
                return;

            Game1.player.FacingDirection = DeepWoodsEnterExit.EnterDirToFacingDirection(deepWoods.EnterDir);
            if (deepWoods.EnterDir == EnterDirection.FROM_TOP)
            {
                Game1.warpFarmer(deepWoods.Name, deepWoods.enterLocation.X, deepWoods.enterLocation.Y + 1, false);
            }
            else if (deepWoods.EnterDir == EnterDirection.FROM_RIGHT)
            {
                Game1.warpFarmer(deepWoods.Name, deepWoods.enterLocation.X + 1, deepWoods.enterLocation.Y, false);
            }
            else
            {
                Game1.warpFarmer(deepWoods.Name, deepWoods.enterLocation.X, deepWoods.enterLocation.Y, false);
            }
        }

        public static void AddDeepWoodsToGameLocations(DeepWoods deepWoods)
        {
            if (deepWoods == null)
                return;

            Game1.locations.Add(deepWoods);

            if (Game1.IsMasterGame)
            {
                foreach (Farmer who in Game1.otherFarmers.Values)
                    if (who != Game1.player)
                        ModEntry.SendMessage(deepWoods.Name, MessageId.AddLocation, who.UniqueMultiplayerID);
            }
        }

        public static void RemoveDeepWoodsFromGameLocations(DeepWoods deepWoods)
        {
            // Player might be in this level, teleport them out
            if (Game1.player.currentLocation == deepWoods)
            {
                Game1.warpFarmer(Game1.getLocationRequest("Woods", false), WOODS_WARP_LOCATION.X, WOODS_WARP_LOCATION.Y, 0);
                // Take away all health and energy to avoid cheaters using Save Anywhere to escape getting lost
                if (deepWoods.level.Value > 1 && deepWoods.IsLost)
                {
                    Game1.player.health = 1;
                    Game1.player.Stamina = 0;
                }
                Game1.player.currentLocation = Game1.getLocationFromName("Woods");
                Game1.player.Position = new Vector2(WOODS_WARP_LOCATION.X * 64, WOODS_WARP_LOCATION.Y * 64);
            }

            Game1.locations.Remove(deepWoods);
            Game1.removeLocationFromLocationLookup(deepWoods);

            if (Game1.IsMasterGame)
            {
                foreach (Farmer who in Game1.otherFarmers.Values)
                    if (who != Game1.player)
                        ModEntry.SendMessage(deepWoods.Name, MessageId.RemoveLocation, who.UniqueMultiplayerID);
            }
        }

        public static void AddBlankDeepWoodsToGameLocations(string name)
        {
            if (Game1.IsMasterGame)
                return;

            if (Game1.getLocationFromName(name) == null)
                AddDeepWoodsToGameLocations(new DeepWoods(name));
        }

        public static void RemoveDeepWoodsFromGameLocations(string name)
        {
            if (Game1.getLocationFromName(name) is DeepWoods deepWoods)
                RemoveDeepWoodsFromGameLocations(deepWoods);
        }

        public static void Remove()
        {
            if (!Game1.IsMasterGame)
                return;

            if (Game1.getLocationFromName("DeepWoods") is DeepWoods rootDeepWoods)
                DeepWoodsManager.rootDeepWoodsBackup = rootDeepWoods;

            List<DeepWoods> toBeRemoved = new List<DeepWoods>();
            foreach (var location in Game1.locations)
                if (location is DeepWoods deepWoods)
                    toBeRemoved.Add(deepWoods);

            foreach (var deepWoods in toBeRemoved)
                DeepWoodsManager.RemoveDeepWoodsFromGameLocations(deepWoods);
        }

        private static void CheckValid()
        {
            if (!Game1.IsMasterGame)
                return;

            if (!IsValidForThisGame())
            {
                Remove();
                DeepWoodsManager.AddDeepWoodsToGameLocations(new DeepWoods(null, 1, EnterDirection.FROM_TOP));
            }
        }

        public static void Restore()
        {
            if (!Game1.IsMasterGame)
                return;

            if (DeepWoodsManager.rootDeepWoodsBackup != null)
                DeepWoodsManager.AddDeepWoodsToGameLocations(DeepWoodsManager.rootDeepWoodsBackup);
            DeepWoodsManager.rootDeepWoodsBackup = null;

            CheckValid();
        }

        public static void Add()
        {
            if (!Game1.IsMasterGame)
                return;

            ModEntry.Log("DeepWoodsManager.Add()", StardewModdingAPI.LogLevel.Trace);

            CheckValid();
        }

        public static void AddAll(string[] deepWoodsLevelNames)
        {
            DeepWoodsManager.Remove();
            foreach (string name in deepWoodsLevelNames)
                AddBlankDeepWoodsToGameLocations(name);
        }

        public static bool IsValidForThisGame()
        {
            return (Game1.getLocationFromName("DeepWoods") is DeepWoods deepWoods
                && deepWoods.uniqueMultiplayerID.Value == Game1.MasterPlayer.UniqueMultiplayerID);
        }

        // This is called by every client at the start of a new day
        public static void LocalDayUpdate(int dayOfMonth)
        {
            DeepWoodsManager.currentDeepWoods = null;
            DeepWoodsManager.currentWarpRequestName = null;
            DeepWoodsManager.currentWarpRequestLocation = null;

            lostMessageDisplayedToday = false;

            if (Game1.IsMasterGame)
            {
                nextRandomizeTime = 0;
                Remove();
                Restore();
            }
        }

        // This is called by every client everytime the time of day changes (10 ingame minute intervals)
        public static void LocalTimeUpdate(int timeOfDay)
        {
            if (Game1.IsMasterGame)
            {
                CheckValid();

                // Check if it's a new hour
                if (timeOfDay >= nextRandomizeTime)
                {
                    // Loop over copies, because inside loops we remove and/or add DeepWoods levels from/to Game1.locations
                    foreach (var location in new List<GameLocation>(Game1.locations))
                    {
                        // Check which DeepWoods can be removed
                        if (location is DeepWoods deepWoods)
                            deepWoods.TryRemove();
                    }

                    foreach (var location in new List<GameLocation>(Game1.locations))
                    {
                        // Randomize exits
                        if (location is DeepWoods deepWoods)
                            deepWoods.RandomizeExits();
                    }

                    foreach (var location in new List<GameLocation>(Game1.locations))
                    {
                        // Validate exits
                        if (location is DeepWoods deepWoods)
                            deepWoods.ValidateAndIfNecessaryCreateExitChildren();
                    }

                    int timeTillNextRandomization = Game1.random.Next(3, 10) * 10;  // one of: 30, 40, 50, 60, 70, 80, 90
                    if (timeTillNextRandomization >= 60)
                        timeTillNextRandomization += 40;  // one of: 30, 40, 50, 100, 110, 120, 130

                    nextRandomizeTime = timeOfDay + timeTillNextRandomization;

                    // e.g. 1250 + 30 gives 1280, but we want 1320
                    if (nextRandomizeTime % 100 >= 60)
                        timeTillNextRandomization += 40;
                }
            }
        }

        // This is called by every client every frame
        public static void LocalTick()
        {
            DeepWoodsManager.currentDeepWoods?.CheckWarp();
        }

        private static bool isModifiedLighting = false;
        public static void FixLighting()
        {
            if (Game1.currentLocation is DeepWoods || Game1.currentLocation is Woods)
            {
                int darkOutDelta = Game1.timeOfDay - Game1.getTrulyDarkTime();
                if (darkOutDelta > 0)
                {
                    double delta = darkOutDelta / 100 + (darkOutDelta % 100 / 60.0) + ((Game1.gameTimeInterval / (double)Game1.realMilliSecondsPerGameTenMinutes) / 6.0);
                    double maxDelta = (2400 - Game1.getTrulyDarkTime()) / 100.0;

                    double ratio = Math.Min(1.0, delta / maxDelta);

                    if (ratio <= 0.0)
                    {
                        Game1.ambientLight = DAY_LIGHT;
                    }
                    else if (ratio >= 1.0)
                    {
                        Game1.ambientLight = NIGHT_LIGHT;
                    }
                    else
                    {
                        Color dayLightFactorized = DAY_LIGHT * (float)(1.0 - ratio);
                        Color nightLightFactorized = NIGHT_LIGHT * (float)ratio;
                        Game1.ambientLight.R = (byte)Math.Min(255, dayLightFactorized.R + nightLightFactorized.R);
                        Game1.ambientLight.G = (byte)Math.Min(255, dayLightFactorized.G + nightLightFactorized.G);
                        Game1.ambientLight.B = (byte)Math.Min(255, dayLightFactorized.B + nightLightFactorized.B);
                        Game1.ambientLight.A = 255;
                    }
                }
                else
                {
                    Game1.ambientLight = DAY_LIGHT;
                }

                Game1.outdoorLight = Game1.ambientLight;
                DeepWoodsManager.isModifiedLighting = true;
            }
            else
            {
                if (DeepWoodsManager.isModifiedLighting
                    && Game1.timeOfDay < Game1.getStartingToGetDarkTime()
                    && !Game1.isRaining
                    && !ModEntry.GetHelper().ModRegistry.IsLoaded("knakamura.dynamicnighttime"))
                {
                    Game1.outdoorLight = Color.White;
                }
                DeepWoodsManager.isModifiedLighting = false;
            }
        }


        // Called whenever a player warps, both from and to may be null
        public static void PlayerWarped(Farmer who, GameLocation rawFrom, GameLocation rawTo)
        {
            DeepWoods from = rawFrom as DeepWoods;
            DeepWoods to = rawTo as DeepWoods;

            if (from != null && to != null && from.Name == to.Name)
                return;

            ModEntry.Log("PlayerWarped from: " + rawFrom?.Name + ", to: " + rawTo?.Name, LogLevel.Trace);

            from?.RemovePlayer(who);
            to?.AddPlayer(who);

            if (from != null && to == null)
            {
                // We left the deepwoods, fix lighting
                DeepWoodsManager.FixLighting();

                // Stop music
                Game1.changeMusicTrack("none");
                Game1.updateMusic();

                // Workaround for bug where players are warped to [0,0] for some reason
                if (rawTo is Woods && who == Game1.player)
                {
                    who.Position = new Vector2(WOODS_WARP_LOCATION.X * 64, WOODS_WARP_LOCATION.Y * 64);
                }
            }

            if (who == Game1.player
                && from != null
                && to != null
                && from.Parent == null
                && to.Parent == from
                && !lostMessageDisplayedToday
                && !to.spawnedFromObelisk.Value
                && ExitDirToEnterDir(CastEnterDirToExitDir(from.EnterDir)) == to.EnterDir)
            {
                Game1.addHUDMessage(new HUDMessage(I18N.LostMessage) { noIcon = true });
                lostMessageDisplayedToday = true;
            }

            if (who == Game1.player
                && to != null
                && to.level.Value >= Settings.Level.MinLevelForWoodsObelisk
                && !Game1.player.hasOrWillReceiveMail(WOODS_OBELISK_WIZARD_MAIL_ID)
                && (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk))
            {
                Game1.addMailForTomorrow(WOODS_OBELISK_WIZARD_MAIL_ID);
            }
        }
    }
}
