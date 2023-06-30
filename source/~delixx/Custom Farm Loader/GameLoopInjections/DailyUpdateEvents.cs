/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using System.Xml;
using StardewModdingAPI.Events;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using Custom_Farm_Loader.Lib.Enums;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class DailyUpdateEvents
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        public static void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;

            if (!CustomFarm.IsCFLMapSelected())
                return;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            foreach (DailyUpdate dailyUpdate in customFarm.DailyUpdates) {
                if (!dailyUpdate.Filter.isValid(who: Game1.player))
                    continue;

                dailyUpdate.Location = Game1.getLocationFromName(dailyUpdate.Area.LocationName);

                if (dailyUpdate.Location == null) {
                    Monitor.LogOnce($"Unknown Location for DailyUpdate: {dailyUpdate.Area.LocationName}", LogLevel.Error);
                    continue;
                }

                if (dailyUpdate.Type == DailyUpdateType.TransformWeeds)
                    updateTransformWeeds(dailyUpdate);
                else
                    updateArea(dailyUpdate);
            }

        }

        private static void updateArea(DailyUpdate dailyUpdate)
        {
            var validTiles = dailyUpdate.validTiles();

            if (dailyUpdate.Attempts < validTiles.Count)
                validTiles = validTiles.OrderBy(x => Game1.random.Next()).Take(dailyUpdate.Attempts).ToList();


            foreach (Vector2 position in validTiles) {
                dailyUpdate.Position = position;

                if (Game1.random.NextDouble() > dailyUpdate.Chance)
                    continue;

                switchDailyUpdate(dailyUpdate);
            }
        }

        private static void switchDailyUpdate(DailyUpdate dailyUpdate)
        {
            switch (dailyUpdate.Type) {
                case DailyUpdateType.SpawnResourceClumps:
                    updateSpawnResourceClumps(dailyUpdate);
                    break;
                case DailyUpdateType.SpawnQuarryRocks:
                    updateSpawnQuerryRocks(dailyUpdate);
                    break;
                case DailyUpdateType.SpawnBeachDrops:
                    updateSpawnBeachDrops(dailyUpdate);
                    break;
                case DailyUpdateType.SpawnForestFarmDrops:
                    updateSpawnForestFarmDrops(dailyUpdate);
                    break;
                case DailyUpdateType.SpawnForagingDrops:
                    updateSpawnForagingDrops(dailyUpdate);
                    break;
                case DailyUpdateType.SpawnItemDrops:
                    updateSpawnItemDrops(dailyUpdate);
                    break;
                case DailyUpdateType.SpawnWildCrops:
                    updateSpawnWildCrops(dailyUpdate);
                    break;
            }
        }
        private static void updateSpawnForestFarmDrops(DailyUpdate dailyUpdate)
        {
            if (Game1.IsWinter)
                return;

            dailyUpdate.Location.dropObject(new StardewValley.Object(dailyUpdate.Position, getRandomForestFarmDrop(), null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);

        }

        private static void updateSpawnForagingDrops(DailyUpdate dailyUpdate)
        {
            dailyUpdate.Location.dropObject(new StardewValley.Object(dailyUpdate.Position, getRandomForagingDrop(), null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);

        }

        private static int getRandomForestFarmDrop()
        {
            List<int> possibleItems = new List<int>();
            if (Game1.currentSeason == "spring")
                possibleItems.AddRange(new int[] { 16, 20, 22, 257 });

            else if (Game1.currentSeason == "summer")
                possibleItems.AddRange(new int[] { 396, 398, 402, 404 });

            else if (Game1.currentSeason == "fall")
                possibleItems.AddRange(new int[] { 281, 404, 420, 422 });

            else
                possibleItems.AddRange(new int[] { 792 });

            return possibleItems.ElementAt(Game1.random.Next(possibleItems.Count));
        }

        private static int getRandomForagingDrop()
        {
            List<int> possibleItems = new List<int>();
            if (Game1.currentSeason == "spring")
                possibleItems.AddRange(new int[] { 16, 18, 20, 22 });

            else if (Game1.currentSeason == "summer")
                possibleItems.AddRange(new int[] { 396, 398, 402 });

            else if (Game1.currentSeason == "fall")
                possibleItems.AddRange(new int[] { 404, 406, 408, 410 });

            else
                possibleItems.AddRange(new int[] { 412, 414, 416, 418 });

            return possibleItems.ElementAt(Game1.random.Next(possibleItems.Count));
        }

        private static void updateSpawnBeachDrops(DailyUpdate dailyUpdate)
        {
            int itemID = getRandomBeachDrop(dailyUpdate.Position);
            if (itemID >= 922 & itemID <= 924)
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, itemID, 1) {
                    Fragility = 2,
                    MinutesUntilReady = 3
                });

            else if (itemID != -1)
                dailyUpdate.Location.dropObject(new StardewValley.Object(dailyUpdate.Position, itemID, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
        }

        private static int getRandomBeachDrop(Vector2 v)
        {
            Game1.stats.incrementStat("beachFarmSpawns", 1);

            if (Game1.random.NextDouble() < 0.15 || Game1.stats.getStat("beachFarmSpawns") % 4u == 0)
                return Game1.random.Next(922, 925);

            return Game1.random.NextDouble() switch {
                < 0.02 => 394,
                < 0.05 => 392,
                < 0.1 => 397,

                _ => Game1.random.Next(6) switch {
                    0 => 393,
                    1 => 719,
                    2 => 718,
                    3 => 723,
                    4 => 372,
                    5 => 152,
                    _ => -1
                }
            };
        }

        private static void updateSpawnQuerryRocks(DailyUpdate dailyUpdate)
        {
            dailyUpdate.Location.objects.Add(dailyUpdate.Position, getRandomQuerryRock(dailyUpdate.Position));
        }

        private static StardewValley.Object getRandomQuerryRock(Vector2 v)
        {
            if (Game1.random.NextDouble() < 0.15)
                return new StardewValley.Object(v, 590, 1);

            int itemID = 668;
            int health = 2;

            if (Game1.random.NextDouble() < 0.5)
                itemID = 670;

            if (Game1.random.NextDouble() < 0.1) {
                if (Game1.player.MiningLevel >= 8 && Game1.random.NextDouble() < 0.33) {
                    itemID = 77;
                    health = 7;
                } else if (Game1.player.MiningLevel >= 5 && Game1.random.NextDouble() < 0.5) {
                    itemID = 76;
                    health = 5;
                } else {
                    itemID = 75;
                    health = 3;
                }
            }
            if (Game1.random.NextDouble() < 0.21) {
                itemID = 751;
                health = 3;
            }
            if (Game1.player.MiningLevel >= 4 && Game1.random.NextDouble() < 0.15) {
                itemID = 290;
                health = 4;
            }
            if (Game1.player.MiningLevel >= 7 && Game1.random.NextDouble() < 0.1) {
                itemID = 764;
                health = 8;
            }
            if (Game1.player.MiningLevel >= 10 && Game1.random.NextDouble() < 0.01) {
                itemID = 765;
                health = 16;
            }

            return new StardewValley.Object(v, itemID, 10) {
                MinutesUntilReady = health
            };
        }


        private static void updateTransformWeeds(DailyUpdate dailyUpdate)
        {
            var viableObjects = dailyUpdate.Location.objects.Pairs.Where(
                obj => dailyUpdate.Area.isTileIncluded(new Vector2(obj.Value.TileLocation.X, obj.Value.TileLocation.Y))
                && obj.Value.name.Equals("Weeds")
                );

            viableObjects = UtilityMisc.PickSomeInRandomOrder(viableObjects, dailyUpdate.Attempts);

            int attempts = 0;
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in viableObjects) {
                if (attempts++ >= dailyUpdate.Attempts)
                    break;

                if (Game1.random.NextDouble() > dailyUpdate.Chance)
                    continue;

                obj.Value.ParentSheetIndex = 792 + Utility.getSeasonNumber(Game1.currentSeason);
            }
        }

        private static void updateSpawnResourceClumps(DailyUpdate dailyUpdate)
        {
            int width = 1;
            int height = 1;

            if (SpringObject.LargeResources.Exists(x => x == dailyUpdate.ResourceClumpID)) {
                width = 2;
                height = 2;
            }

            for (int x = (int)dailyUpdate.Position.X; x < dailyUpdate.Position.X + width; x++)
                for (int y = (int)dailyUpdate.Position.Y; y < dailyUpdate.Position.Y + width; y++)
                    if (!dailyUpdate.Location.isTileLocationTotallyClearAndPlaceable(x, y))
                        return;

            if (SpringObject.LargeResources.Exists(x => x == dailyUpdate.ResourceClumpID))
                dailyUpdate.Location.resourceClumps.Add(new ResourceClump((int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID), width, height, dailyUpdate.Position));

            else if (SpringObject.SmallResources.ContainsKey(dailyUpdate.ResourceClumpID))
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, (int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID), 10) { MinutesUntilReady = SpringObject.SmallResources[dailyUpdate.ResourceClumpID], Flipped = Game1.random.Next(0, 2) == 0 });

            else if (SpringObject.GemNodes.ContainsKey(dailyUpdate.ResourceClumpID))
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, (int)dailyUpdate.ResourceClumpID, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false) { MinutesUntilReady = SpringObject.GemNodes[dailyUpdate.ResourceClumpID] });

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Weed)
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, SpringObject.getRandomWeedForSeason(dailyUpdate.Location.GetSeasonForLocation()), 1));

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Transformed_Weed)
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, SpringObject.getTransformedWeedForSeason(dailyUpdate.Location.GetSeasonForLocation()), 1));

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Crystal)
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, (int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID), "Weeds", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false) {
                    Fragility = 2,
                    CanBeGrabbed = true
                });

            else
                dailyUpdate.Location.objects.Add(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, (int)dailyUpdate.ResourceClumpID, 1));

        }

        private static void updateSpawnItemDrops(DailyUpdate dailyUpdate)
        {
            if (dailyUpdate.Items.Count == 0)
                return;

            if (!dailyUpdate.Location.isTileLocationTotallyClearAndPlaceable((int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y))
                return;

            string itemID = UtilityMisc.PickSomeInRandomOrder(dailyUpdate.Items, 1).First();

            if (int.TryParse(itemID, out int parentSheetIndex))
                dailyUpdate.Location.dropObject(new StardewValley.Object(dailyUpdate.Position, parentSheetIndex, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
            else
                Monitor.LogOnce($"Tried to spawn non integer item '{itemID}' at DU SpawnItemDrops", LogLevel.Warn);
        }

        private static void updateSpawnWildCrops(DailyUpdate dailyUpdate)
        {
            var whichForageCrop = dailyUpdate.WildCropID switch {
                WildCropType.Spring_Onion => 1,
                WildCropType.Ginger => 2,
                _ => 0
            };

            Crop crop = null;

            if (whichForageCrop == 0) {
                crop = new Crop((int)dailyUpdate.WildCropID, (int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y);
                crop.growCompletely();
            } else
                crop = new Crop(true, whichForageCrop, (int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y);


            if (dailyUpdate.Location.terrainFeatures.ContainsKey(dailyUpdate.Position))
                (dailyUpdate.Location.terrainFeatures[dailyUpdate.Position] as HoeDirt).crop = crop;

            else
                //We add the hoedirt this way, because the constructor which includes the crop doesn't call HoeDirt.initialize for some reason
                dailyUpdate.Location.terrainFeatures.Add(dailyUpdate.Position, new HoeDirt(0, dailyUpdate.Location) { crop = crop });
        }

    }
}