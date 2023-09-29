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

                Monitor.Log($"Running daily update of type {dailyUpdate.Type} at {dailyUpdate.Area.LocationName}");
                if (dailyUpdate.Type == DailyUpdateType.TransformWeeds)
                    updateTransformWeeds(dailyUpdate);
                else
                    updateArea(dailyUpdate);
            }

        }

        public static void updateArea(DailyUpdate dailyUpdate)
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
                    //updateSpawnBeachDrops(dailyUpdate);
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


            var obj = ItemRegistry.Create<StardewValley.Object>(getRandomForestFarmDrop());
            dailyUpdate.Location.dropObject(obj, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);


        }

        private static void updateSpawnForagingDrops(DailyUpdate dailyUpdate)
        {
            var obj = ItemRegistry.Create<StardewValley.Object>(getRandomForagingDrop());
            dailyUpdate.Location.dropObject(obj, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);

        }

        private static string getRandomForestFarmDrop()
        {
            List<int> possibleItems = new List<int>();
            if (Game1.season == Season.Spring)
                possibleItems.AddRange(new int[] { 16, 20, 22, 257 });

            else if (Game1.season == Season.Summer)
                possibleItems.AddRange(new int[] { 396, 398, 402, 404 });

            else if (Game1.season == Season.Fall)
                possibleItems.AddRange(new int[] { 281, 404, 420, 422 });

            else
                possibleItems.AddRange(new int[] { 792 });

            return possibleItems.ElementAt(Game1.random.Next(possibleItems.Count)).ToString();
        }

        private static string getRandomForagingDrop()
        {
            List<int> possibleItems = new List<int>();
            if (Game1.season == Season.Spring)
                possibleItems.AddRange(new int[] { 16, 18, 20, 22 });

            else if (Game1.season == Season.Summer)
                possibleItems.AddRange(new int[] { 396, 398, 402 });

            else if (Game1.season == Season.Fall)
                possibleItems.AddRange(new int[] { 404, 406, 408, 410 });

            else
                possibleItems.AddRange(new int[] { 412, 414, 416, 418 });

            return possibleItems.ElementAt(Game1.random.Next(possibleItems.Count)).ToString();
        }

        private static void updateSpawnBeachDrops(DailyUpdate dailyUpdate)
        {
            string itemID = getRandomBeachDrop(dailyUpdate.Position);
            if (itemID == "922" || itemID == "923" || itemID == "924")
                dailyUpdate.Location.objects.TryAdd(dailyUpdate.Position, new StardewValley.Object(dailyUpdate.Position, itemID) {
                    Fragility = 2,
                    MinutesUntilReady = 3
                });

            else if (itemID != "-1")
                dailyUpdate.Location.dropObject(new StardewValley.Object(dailyUpdate.Position, itemID), dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
        }

        private static string getRandomBeachDrop(Vector2 v)
        {
            Game1.stats.incrementStat("beachFarmSpawns", 1);

            if (Game1.random.NextDouble() < 0.15 || Game1.stats.getStat("beachFarmSpawns") % 4u == 0)
                return Game1.random.Next(922, 925).ToString();

            return Game1.random.NextDouble() switch {
                < 0.02 => "394",
                < 0.05 => "392",
                < 0.1 => "397",

                _ => Game1.random.Next(6) switch {
                    0 => "393",
                    1 => "719",
                    2 => "718",
                    3 => "723",
                    4 => "372",
                    5 => "152",
                    _ => "-1"
                }
            };
        }

        private static void updateSpawnQuerryRocks(DailyUpdate dailyUpdate)
        {
            var obj = ItemRegistry.Create<StardewValley.Object>(getRandomQuerryRock());
            dailyUpdate.Location.Objects.TryAdd(dailyUpdate.Position, obj);
        }

        private static string getRandomQuerryRock()
        {
            if (Game1.random.NextDouble() < 0.15)
                return "590";

            int itemID = 668;

            if (Game1.random.NextDouble() < 0.5)
                itemID = 670;

            if (Game1.random.NextDouble() < 0.1) {
                if (Game1.player.MiningLevel >= 8 && Game1.random.NextDouble() < 0.33) {
                    itemID = 77;
                } else if (Game1.player.MiningLevel >= 5 && Game1.random.NextDouble() < 0.5) {
                    itemID = 76;
                } else {
                    itemID = 75;
                }
            }
            if (Game1.random.NextDouble() < 0.21)
                itemID = 751;

            if (Game1.player.MiningLevel >= 4 && Game1.random.NextDouble() < 0.15)
                itemID = 290;

            if (Game1.player.MiningLevel >= 7 && Game1.random.NextDouble() < 0.1)
                itemID = 764;

            if (Game1.player.MiningLevel >= 10 && Game1.random.NextDouble() < 0.01)
                itemID = 765;

            return itemID.ToString();
        }


        public static void updateTransformWeeds(DailyUpdate dailyUpdate)
        {
            var viableObjects = dailyUpdate.Location.objects.Pairs.Where(
                obj => dailyUpdate.Area.isTileIncluded(new Vector2(obj.Value.TileLocation.X, obj.Value.TileLocation.Y))
                && obj.Value.IsWeeds()
                && (obj.Value.ParentSheetIndex < 792 || obj.Value.ParentSheetIndex > 794)
                );

            viableObjects = UtilityMisc.PickSomeInRandomOrder(viableObjects, dailyUpdate.Attempts);

            int attempts = 0;
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in viableObjects) {
                if (attempts++ >= dailyUpdate.Attempts)
                    break;

                if (Game1.random.NextDouble() > dailyUpdate.Chance)
                    continue;

                //We create a new object because simply changing the parentsheetindex like vanilla does it doesn't seem to work
                var newObj = ItemRegistry.Create<StardewValley.Object>(((int)SpringObject.getTransformedWeedForSeason(dailyUpdate.Location.GetSeason())).ToString());
                dailyUpdate.Location.Objects.Remove(obj.Value.TileLocation);
                dailyUpdate.Location.Objects.Add(obj.Value.TileLocation, newObj);
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
                    if (dailyUpdate.Location.IsTileOccupiedBy(new Vector2(x, y)))
                        return;

            if (SpringObject.LargeResources.Exists(x => x == dailyUpdate.ResourceClumpID)) {
                dailyUpdate.Location.resourceClumps.Add(new ResourceClump((int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID), width, height, dailyUpdate.Position));
                return;
            }

            string itemId;
            if (SpringObject.SmallResources.ContainsKey(dailyUpdate.ResourceClumpID))
                itemId = ((int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID)).ToString();

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Weed)
                itemId = SpringObject.getRandomWeedForSeason(dailyUpdate.Location.GetSeason());

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Transformed_Weed)
                itemId = ((int)SpringObject.getTransformedWeedForSeason(dailyUpdate.Location.GetSeason())).ToString();

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Crystal)
                itemId = ((int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID)).ToString();

            else
                itemId = ((int)dailyUpdate.ResourceClumpID).ToString();

            var obj = ItemRegistry.Create<StardewValley.Object>(itemId);
            dailyUpdate.Location.Objects.TryAdd(dailyUpdate.Position, obj);
        }

        private static void updateSpawnItemDrops(DailyUpdate dailyUpdate)
        {
            if (dailyUpdate.Items.Count == 0)
                return;

            if (dailyUpdate.Location.IsTileOccupiedBy(new Vector2((int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y)))
                return;

            string itemId = UtilityMisc.PickSomeInRandomOrder(dailyUpdate.Items, 1).First();

            var obj = ItemRegistry.Create<StardewValley.Object>(itemId);
            dailyUpdate.Location.dropObject(obj, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
        }

        private static void updateSpawnWildCrops(DailyUpdate dailyUpdate)
        {
            var whichForageCrop = dailyUpdate.WildCropID switch {
                WildCropType.Spring_Onion => "1",
                WildCropType.Ginger => "2",
                _ => ""
            };

            Crop crop;
            if (whichForageCrop == "") {
                crop = new Crop(((int)dailyUpdate.WildCropID).ToString(), (int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y, dailyUpdate.Location);
                crop.growCompletely();
            } else
                crop = new Crop(true, whichForageCrop, (int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y, dailyUpdate.Location);


            if (dailyUpdate.Location.terrainFeatures.ContainsKey(dailyUpdate.Position)
                && dailyUpdate.Location.terrainFeatures[dailyUpdate.Position] is HoeDirt hoeDirt //we did already check for this in DailyUpdate.wildCropsException
                && hoeDirt.crop is null)
                hoeDirt.crop = crop;

            else
                //We add the hoedirt this way, because the constructor which includes the crop doesn't call HoeDirt.initialize for some reason
                dailyUpdate.Location.terrainFeatures.TryAdd(dailyUpdate.Position, new HoeDirt(0, dailyUpdate.Location) { crop = crop });
                //dailyUpdate.Location.terrainFeatures.TryAdd(dailyUpdate.Position, new HoeDirt(0, crop));
        }

    }
}