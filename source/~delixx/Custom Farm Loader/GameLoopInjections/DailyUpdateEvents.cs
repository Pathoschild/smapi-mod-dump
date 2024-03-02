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

        private static bool ReachedMaxForage = false;
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
            update(customFarm.DailyUpdates);
        }

        public static void update(List<DailyUpdate> dailyUpdates)
        {
            ReachedMaxForage = false;

            foreach (DailyUpdate dailyUpdate in dailyUpdates) {
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
            validTiles = validTiles.OrderBy(x => Game1.random.Next()).Take(dailyUpdate.Attempts).ToList();

            var spawnedCount = countSpawned(dailyUpdate);
            foreach (Vector2 position in validTiles) {
                if (dailyUpdate.MaxSpawned is not null && spawnedCount >= dailyUpdate.MaxSpawned)
                    break;

                dailyUpdate.Position = position;

                if (Game1.random.NextDouble() > dailyUpdate.Chance)
                    continue;

                if (switchDailyUpdate(dailyUpdate))
                    spawnedCount++;
            }
        }

        private static bool switchDailyUpdate(DailyUpdate dailyUpdate)
        {
            switch (dailyUpdate.Type) {
                case DailyUpdateType.SpawnResourceClumps:
                    return updateSpawnResourceClumps(dailyUpdate);

                case DailyUpdateType.SpawnQuarryRocks:
                    return updateSpawnQuerryRocks(dailyUpdate);

                case DailyUpdateType.SpawnBeachDrops:
                    return updateSpawnBeachDrops(dailyUpdate);

                case DailyUpdateType.SpawnForestFarmDrops:
                    return updateSpawnForestFarmDrops(dailyUpdate);

                case DailyUpdateType.SpawnForagingDrops:
                    return updateSpawnForagingDrops(dailyUpdate);

                case DailyUpdateType.SpawnItemDrops:
                    return updateSpawnItemDrops(dailyUpdate);

                case DailyUpdateType.SpawnWildCrops:
                    return updateSpawnWildCrops(dailyUpdate);
            }

            return false;
        }
        private static bool updateSpawnForestFarmDrops(DailyUpdate dailyUpdate)
        {
            if (Game1.IsWinter)
                return false;


            var obj = ItemRegistry.Create<StardewValley.Object>(getRandomForestFarmDrop());
            return dailyUpdate.Location.dropObject(obj, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
        }

        private static bool updateSpawnForagingDrops(DailyUpdate dailyUpdate)
        {
            var obj = ItemRegistry.Create<StardewValley.Object>(getRandomForagingDrop());
            return dailyUpdate.Location.dropObject(obj, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
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

        private static bool updateSpawnBeachDrops(DailyUpdate dailyUpdate)
        {
            string itemID = getRandomBeachDrop(dailyUpdate.Position);
            if (itemID == "922" || itemID == "923" || itemID == "924")
                return dailyUpdate.Location.objects.TryAdd(dailyUpdate.Position, new StardewValley.Object(itemID, 1) {
                    Fragility = 2,
                    MinutesUntilReady = 3
                });

            else if (itemID != "-1") {
                var obj2 = ItemRegistry.Create<StardewValley.Object>("(O)" + itemID);
                obj2.CanBeSetDown = false;
                obj2.IsSpawnedObject = true;
                return dailyUpdate.Location.dropObject(obj2, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
            }

            return false;
        }

        private static string getRandomBeachDrop(Vector2 v)
        {
            Game1.stats.Increment("beachFarmSpawns", 1);

            if (Game1.random.NextDouble() < 0.15 || Game1.stats.Get("beachFarmSpawns") % 4u == 0)
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

        private static bool updateSpawnQuerryRocks(DailyUpdate dailyUpdate)
        {
            var obj = ItemRegistry.Create<StardewValley.Object>(getRandomQuerryRock());
            return dailyUpdate.Location.Objects.TryAdd(dailyUpdate.Position, obj);
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

            var spawnedCount = countSpawned(dailyUpdate);
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in viableObjects) {
                if (dailyUpdate.MaxSpawned is not null && spawnedCount++ >= dailyUpdate.MaxSpawned)
                    break;

                if (Game1.random.NextDouble() > dailyUpdate.Chance)
                    continue;

                //We create a new object because simply changing the parentsheetindex like vanilla does it doesn't seem to work
                var newObj = ItemRegistry.Create<StardewValley.Object>(((int)SpringObject.getTransformedWeedForSeason(dailyUpdate.Location.GetSeason())).ToString());
                dailyUpdate.Location.Objects.Remove(obj.Value.TileLocation);
                dailyUpdate.Location.Objects.TryAdd(obj.Value.TileLocation, newObj);
            }
        }

        private static bool updateSpawnResourceClumps(DailyUpdate dailyUpdate)
        {

            if (SpringObject.LargeResources.Exists(x => x == dailyUpdate.ResourceClumpID)) {
                if (!dailyUpdate.isValidTile(dailyUpdate.Position + new Vector2(1, 0))
                    || !dailyUpdate.isValidTile(dailyUpdate.Position + new Vector2(0, 1))
                    || !dailyUpdate.isValidTile(dailyUpdate.Position + new Vector2(1, 1)))
                    return false;

                dailyUpdate.Location.resourceClumps.Add(new ResourceClump((int)SpringObject.randomizeResourceIDs(dailyUpdate.ResourceClumpID), 2, 2, dailyUpdate.Position));
                return true;
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
            return dailyUpdate.Location.Objects.TryAdd(dailyUpdate.Position, obj);
        }

        private static bool updateSpawnItemDrops(DailyUpdate dailyUpdate)
        {
            if (dailyUpdate.Items.Count == 0)
                return false;

            if (dailyUpdate.Location.IsTileOccupiedBy(new Vector2((int)dailyUpdate.Position.X, (int)dailyUpdate.Position.Y)))
                return false;

            string itemId = UtilityMisc.PickSomeInRandomOrder(dailyUpdate.Items, 1).First();

            var obj = ItemRegistry.Create<StardewValley.Object>(itemId);
            return dailyUpdate.Location.dropObject(obj, dailyUpdate.Position * 64f, Game1.viewport, initialPlacement: true);
        }

        private static bool updateSpawnWildCrops(DailyUpdate dailyUpdate)
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
                && hoeDirt.crop is null) {
                hoeDirt.crop = crop;
                return true;
            } else
                //We add the hoedirt this way, because the constructor which includes the crop doesn't call HoeDirt.initialize for some reason
                return dailyUpdate.Location.terrainFeatures.TryAdd(dailyUpdate.Position, new HoeDirt(0, dailyUpdate.Location) { crop = crop });
        }

        private static int countSpawned(DailyUpdate dailyUpdate)
        {
            switch (dailyUpdate.Type) {
                case DailyUpdateType.SpawnResourceClumps:
                    return countSpawnedResourceClumps(dailyUpdate);

                case DailyUpdateType.SpawnQuarryRocks:
                    return countSpawnedObjects(dailyUpdate, new() { "590", "668", "670", "77", "76", "75", "751", "290", "764", "765" });

                case DailyUpdateType.SpawnBeachDrops:
                    return countSpawnedObjects(dailyUpdate, new() { "922", "923", "924", "394", "392", "397", "719", "718", "723", "372", "152" });

                case DailyUpdateType.SpawnForestFarmDrops:
                    return countSpawnedObjects(dailyUpdate, new() { "16", "20", "22", "257", "396", "398", "402", "404", "281", "420", "422", "792" });

                case DailyUpdateType.SpawnForagingDrops:
                    return countSpawnedObjects(dailyUpdate, new() { "16", "18", "20", "22", "396", "398", "402", "404", "406", "408", "410", "412", "414", "416", "418" });

                case DailyUpdateType.SpawnItemDrops:
                    return countSpawnedObjects(dailyUpdate, dailyUpdate.Items);

                case DailyUpdateType.SpawnWildCrops:
                    return countSpawnedCrops(dailyUpdate, new() { ((int)dailyUpdate.WildCropID).ToString() });

                case DailyUpdateType.TransformWeeds:
                    return countSpawnedObjects(dailyUpdate, new() { "792", "793", "794" });
            }

            return 0;
        }
        private static int countSpawnedResourceClumps(DailyUpdate dailyUpdate)
        {
            List<int> itemIds = new();

            if (SpringObject.SmallResources.ContainsKey(dailyUpdate.ResourceClumpID)
                || SpringObject.LargeResources.Exists(x => x == dailyUpdate.ResourceClumpID)
                || dailyUpdate.ResourceClumpID == SpringObjectID.Crystal)
                SpringObject.getVariants(dailyUpdate.ResourceClumpID).ForEach(el => itemIds.Add((int)el));

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Weed)
                itemIds = new() { 674, 675, 784, 676, 677, 785, 678, 679, 786 };

            else if (dailyUpdate.ResourceClumpID == SpringObjectID.Transformed_Weed)
                itemIds = new() {
                    (int)SpringObjectID.Transformed_Weed,
                    (int)SpringObjectID.Transformed_Weed_Summer,
                    (int)SpringObjectID.Transformed_Weed_Fall };

            else
                itemIds = new() { (int)dailyUpdate.ResourceClumpID };


            if (SpringObject.LargeResources.Exists(x => x == dailyUpdate.ResourceClumpID))
                return dailyUpdate.Location.resourceClumps.Count(el => el is ResourceClump rc && itemIds.Contains(rc.parentSheetIndex.Value) && dailyUpdate.Area.isTileIncluded(el.Tile));

            return countSpawnedObjects(dailyUpdate, itemIds.ConvertAll(el => el.ToString()));
        }

        //Not checking for IsSpawnedObject because I think that's irrelevant and probably inconsistent with different object types
        private static int countSpawnedObjects(DailyUpdate dailyUpdate, List<string> itemIds)
            => dailyUpdate.Location.Objects.Pairs.Count(el => itemIds.Contains(el.Value.ItemId) && dailyUpdate.Area.isTileIncluded(el.Value.TileLocation));

        private static int countSpawnedCrops(DailyUpdate dailyUpdate, List<string> itemIds)
        {
            var whichForageCrop = dailyUpdate.WildCropID switch {
                WildCropType.Spring_Onion => "1",
                WildCropType.Ginger => "2",
                _ => ""
            };

            return dailyUpdate.Location.terrainFeatures.Pairs.Count(el => el.Value is HoeDirt hd
                                                                   && hd.crop is not null
                                                                   && (whichForageCrop == "" ? itemIds.Contains(hd.crop.netSeedIndex.Value) : hd.crop.whichForageCrop.Value == whichForageCrop)
                                                                   && dailyUpdate.Area.isTileIncluded(el.Value.Tile));
        }
    }
}