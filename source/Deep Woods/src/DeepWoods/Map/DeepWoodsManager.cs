/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using static DeepWoodsMod.DeepWoodsEnterExit;
using static DeepWoodsMod.DeepWoodsGlobals;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    public class DeepWoodsManager
    {
        public static DeepWoods currentDeepWoods = null;
        public static string currentWarpRequestName = null;
        public static Vector2? currentWarpRequestLocation = null;


        private static DeepWoodsMaxHouse maxHut = null;

        public static void AddMaxHut()
        {
            if (maxHut == null)
            {
                maxHut = new DeepWoodsMaxHouse();
            }
            if (Game1.getLocationFromName(maxHut.Name) == null)
            {
                if (!Game1.locations.Contains(maxHut))
                {
                    Game1.locations.Add(maxHut);
                }
                Game1._locationLookup[maxHut.Name] = maxHut;
            }
        }

        private static void RemoveMaxHut()
        {
            RemoveGameLocation(maxHut);
        }


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

        private static void RemoveGameLocation(GameLocation gameLocation)
        {
            // Player might be in this level, teleport them out
            if (Game1.player.currentLocation == gameLocation)
            {
                Game1.warpFarmer(Game1.getLocationRequest("Woods", false), DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.X, DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.Y, 0);
                // Take away all health and energy to avoid cheaters using Save Anywhere to escape getting lost
                if (gameLocation is DeepWoods deepWoods && deepWoods.level.Value > 1 && deepWoods.IsLost)
                {
                    Game1.player.health = 1;
                    Game1.player.Stamina = 0;
                }
                Game1.player.currentLocation = Game1.getLocationFromName("Woods");
                Game1.player.Position = new Vector2(DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.X * 64, DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.Y * 64);
            }

            Game1.locations.Remove(gameLocation);
            Game1.removeLocationFromLocationLookup(gameLocation);
        }

        public static void RemoveDeepWoodsFromGameLocations(DeepWoods deepWoods)
        {
            RemoveGameLocation(deepWoods);

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
            {
                RemoveDeepWoodsFromGameLocations(deepWoods);
            }
            else if (Game1.getLocationFromName(name) is DeepWoodsMaxHouse maxHouse)
            {
                RemoveGameLocation(maxHouse);
            }
        }

        public static void Remove()
        {
            RemoveMaxHut();

            if (!Game1.IsMasterGame)
                return;

            if (Game1.getLocationFromName("DeepWoods") is DeepWoods rootDeepWoods)
            {
                DeepWoodsManager.rootDeepWoodsBackup = rootDeepWoods;
            }

            Game1.locations.Where(l => l is DeepWoods).Select(l => l as DeepWoods).ToList().ForEach(RemoveDeepWoodsFromGameLocations);
        }

        private static void CheckValid()
        {
            if (Game1.IsMasterGame)
            {
                if (!IsValidForThisGame())
                {
                    Remove();
                    AddMaxHut();
                    DeepWoodsManager.rootDeepWoodsBackup = null;
                    DeepWoodsManager.AddDeepWoodsToGameLocations(new DeepWoods(null, 1, EnterDirection.FROM_TOP));
                }
            }

            AddMaxHut();
        }

        public static void Restore()
        {
            if (Game1.IsMasterGame)
            {
                if (DeepWoodsManager.rootDeepWoodsBackup != null)
                {
                    DeepWoodsManager.AddDeepWoodsToGameLocations(DeepWoodsManager.rootDeepWoodsBackup);
                    DeepWoodsManager.rootDeepWoodsBackup = null;
                }
            }

            CheckValid();
        }

        public static void Add()
        {
            CheckValid();
        }

        public static void AddAll(string[] deepWoodsLevelNames)
        {
            DeepWoodsManager.Remove();
            foreach (string name in deepWoodsLevelNames)
                AddBlankDeepWoodsToGameLocations(name);
            DeepWoodsManager.AddMaxHut();
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
                        nextRandomizeTime += 40;
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
            if (rawFrom is DeepWoodsMaxHouse && rawTo is DeepWoods deepWoods && deepWoods.Level == 1)
            {
                DeepWoodsManager.currentWarpRequestName = "DeepWoods";
                DeepWoodsManager.currentWarpRequestLocation = who.Position;
            }

            DeepWoods from = rawFrom as DeepWoods;
            DeepWoods to = rawTo as DeepWoods;

            if (from != null && to != null && from.Name == to.Name)
                return;

            from?.RemovePlayer(who);
            to?.AddPlayer(who);

            // everything that follows should only be done if this is the local player
            if (who != Game1.player)
            {
                ModEntry.Log("Player (" + who.UniqueMultiplayerID + ") warped from: " + rawFrom?.Name + ", to: " + rawTo?.Name, LogLevel.Trace);
                return;
            }

            ModEntry.Log("Local Player (" + who.UniqueMultiplayerID + ") warped from: " + rawFrom?.Name + ", to: " + rawTo?.Name, LogLevel.Trace);

            if (rawTo is Woods woods)
            {
                OpenPassageInSecretWoods(woods);
            }

            if (from != null && to == null)
            {
                // We left the deepwoods, fix lighting
                DeepWoodsManager.FixLighting();

                // Stop music
                Game1.changeMusicTrack("none");
                Game1.updateMusic();

                // Workaround for bug where players are warped to [0,0] for some reason
                if (rawTo is Woods)
                {
                    who.Position = new Vector2(DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.X * 64, DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.Y * 64);
                }
            }

            if (from != null
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

            if (to != null
                && to.level.Value >= Settings.Level.MinLevelForWoodsObelisk
                && !Game1.player.hasOrWillReceiveMail(WOODS_OBELISK_WIZARD_MAIL_ID)
                && (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk))
            {
                Game1.addMailForTomorrow(WOODS_OBELISK_WIZARD_MAIL_ID);
            }
        }


        private static void OpenPassageInSecretWoods(Woods woods)
        {
            // Game isn't running
            if (!ModEntry.IsDeepWoodsGameRunning)
            {
                ModEntry.Log("OpenPassageInSecretWoods: Cancelled, mod not initialized.", LogLevel.Trace);
                return;
            }

            // If warps exist, the map might be patched already, but we can't be 100% sure
            // So we just use this as an indicator not to print any warnings about stuff already being patched we might encounter further down.
            var existingWarpLocations = woods.warps.Where(warp => "DeepWoods".Equals(warp.TargetName)).Select(warp => new SimpleCoord(warp.X, warp.Y));
            bool possiblyPatchedAlready = existingWarpLocations.Any();

            Layer buildingsLayer = woods.map.GetLayer("Buildings");

            // Just to be sure
            if (buildingsLayer == null)
            {
                ModEntry.Log("OpenPassageInSecretWoods: Cancelled, invalid map (buildingsLayer is null).", LogLevel.Trace);
                return;
            }

            ModEntry.Log("OpenPassageInSecretWoods:", LogLevel.Trace);

            TileSheet borderTileSheet = woods.map.TileSheets.First();
            int borderTileIndex = 0;

            int removed = 0;
            int added = 0;
            int warpsAdded = 0;

            foreach (var location in Settings.WoodsPassage.DeleteBuildingTiles)
            {
                if (buildingsLayer.Tiles[location.X, location.Y] == null)
                {
                    if (!possiblyPatchedAlready)
                        ModEntry.Log($"    Can't remove tile from building layer at {location.X}, {location.Y}, there is no tile here! (Custom Woods map? Please modify WoodsPassage settings in the DeepWoods config file for custom Woods maps.)", LogLevel.Trace);
                }
                else
                {
                    ModEntry.Log($"    Removing tile from building layer at {location.X}, {location.Y}.", LogLevel.Trace);
                    buildingsLayer.Tiles[location.X, location.Y] = null;
                    removed++;
                }
            }

            foreach (var location in Settings.WoodsPassage.AddBuildingTiles)
            {
                if (buildingsLayer.Tiles[location.X, location.Y] == null)
                {
                    ModEntry.Log($"    Adding tile to building layer at {location.X}, {location.Y}.", LogLevel.Trace);
                    buildingsLayer.Tiles[location.X, location.Y] = new StaticTile(buildingsLayer, borderTileSheet, BlendMode.Alpha, borderTileIndex);
                    added++;
                }
                else
                {
                    if (!possiblyPatchedAlready)
                        ModEntry.Log($"    Can't add tile to building layer at {location.X}, {location.Y}, already have a tile there! (Custom Woods map? Please modify WoodsPassage settings in the DeepWoods config file for custom Woods maps.)", LogLevel.Trace);
                }
            }

            foreach (var location in Settings.WoodsPassage.WarpLocations)
            {
                if (!existingWarpLocations.Contains(location))
                {
                    ModEntry.Log($"    Adding warp to DeepWoods at {location.X}, {location.Y}.", LogLevel.Trace);
                    woods.warps.Add(new Warp(location.X, location.Y, "DeepWoods", Settings.Map.RootLevelEnterLocation.X, Settings.Map.RootLevelEnterLocation.Y + 1, false));
                    warpsAdded++;
                }
            }

            if (possiblyPatchedAlready && added == 0 && removed == 0 && warpsAdded == 0)
            {
                ModEntry.Log($"OpenPassageInSecretWoods skipped. Map was already patched.", LogLevel.Trace);
            }
            else
            {
                ModEntry.Log($"OpenPassageInSecretWoods done. (Added {added}/{Settings.WoodsPassage.AddBuildingTiles.Length} tiles, removed {removed}/{Settings.WoodsPassage.DeleteBuildingTiles.Length} tiles, added {warpsAdded}/{Settings.WoodsPassage.WarpLocations.Length} warps.)", LogLevel.Trace);
            }
        }
    }
}
