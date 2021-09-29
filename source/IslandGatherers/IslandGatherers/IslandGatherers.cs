/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using HarmonyLib;
using IslandGatherers.Framework;
using IslandGatherers.Framework.Objects;
using IslandGatherers.Framework.Patches;
using IslandGatherers.Framework.Patches.GameLocations;
using IslandGatherers.Framework.Patches.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandGatherers
{
    public class IslandGatherers : Mod
    {
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static ModConfig config;
        internal static readonly string oldParrotPotFlag = "PeacefulEnd.IslandGatherers_IsParrotStorage";
        internal static readonly string hasConvertedOldStatues = "PeacefulEnd.IslandGatherers/HasConvertedToNewStatues";

        // Parrot Pot ModData
        internal static readonly string parrotPotFlag = "PeacefulEnd.IslandGatherers/ParrotPot";
        internal static readonly string ateCropsFlag = "PeacefulEnd.IslandGatherers/AteCrops";
        internal static readonly string spawnedJunimosFlag = "PeacefulEnd.IslandGatherers/HasSpawnedJunimos";

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor and helper
            monitor = Monitor;
            modHelper = helper;

            // Set up our asset manager
            AssetManager.SetUpAssets(helper);

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                // Apply our patches
                new ObjectPatch(Monitor, Helper).Apply(harmony);
                new CropPatch(Monitor, Helper).Apply(harmony);
                new ChestPatch(Monitor, Helper).Apply(harmony);
                new IslandNorthPatch(Monitor, Helper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Hook into GameLoop events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Set our default config
            config = Helper.ReadConfig<ModConfig>();

            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && ApiManager.HookIntoGMCM(Helper))
            {
                // Register our config options
                var configAPI = ApiManager.GetGMCMInterface();
                configAPI.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                configAPI.RegisterSimpleOption(ModManifest, "Enable Harvest Message", "If true, the mod will notify you of a harvest at the start of the day.", () => config.EnableHarvestMessage, (bool val) => config.EnableHarvestMessage = val);
                configAPI.RegisterSimpleOption(ModManifest, "Parrots Eat Excess", "If true, parrots will eat any excess crops they gather if the storage is full.", () => config.DoParrotsEatExcessCrops, (bool val) => config.DoParrotsEatExcessCrops = val);
                configAPI.RegisterSimpleOption(ModManifest, "Parrots Sow Seeds", "If true, parrots will plant seeds of the same type of the crop they harvset.", () => config.DoParrotsSowSeedsAfterHarvest, (bool val) => config.DoParrotsSowSeedsAfterHarvest = val);

                configAPI.RegisterSimpleOption(ModManifest, "Parrots Harvest From Fruit Trees", "If true, parrots will harvest from fruit trees.", () => config.DoParrotsHarvestFromFruitTrees, (bool val) => config.DoParrotsHarvestFromFruitTrees = val);
                configAPI.RegisterClampedOption(ModManifest, "Minimum Fruit On Tree Before Harvest", "The minimum amount of fruit on a tree required before harvesting.", () => config.MinimumFruitOnTreeBeforeHarvest, (int val) => config.MinimumFruitOnTreeBeforeHarvest = val, 1, 3);

                configAPI.RegisterSimpleOption(ModManifest, "Parrots Harvest From Pots", "If true, parrots will harvest from garden pots.", () => config.DoParrotsHarvestFromPots, (bool val) => config.DoParrotsHarvestFromPots = val);
                configAPI.RegisterSimpleOption(ModManifest, "Parrots Harvest From Flowers", "If true, parrots will harvest from flowers.", () => config.DoParrotsHarvestFromFlowers, (bool val) => config.DoParrotsHarvestFromFlowers = val);

                configAPI.RegisterSimpleOption(ModManifest, "Parrots Appear After Harvest", "If true, parrots will appear after harvesting.", () => config.DoParrotsAppearAfterHarvest, (bool val) => config.DoParrotsAppearAfterHarvest = val);
            }

            // Check if spacechase0's DynamicGameAssets is in the current mod list
            if (Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                Monitor.Log("Attempting to hook into spacechase0.DynamicGameAssets.", LogLevel.Debug);
                ApiManager.HookIntoDynamicGameAssets(Helper);

                var contentPack = Helper.ContentPacks.CreateTemporary(
                    Path.Combine(Helper.DirectoryPath, AssetManager.assetFolderPath),
                    "PeacefulEnd.IslandGatherers.ParrotPot",
                    "PeacefulEnd.IslandGatherers.ParrotPot",
                    "Adds the required assets for Island Gatherers.",
                    "PeacefulEnd",
                    new SemanticVersion("1.0.0"));

                // Check if furyx639's Expanded Storage is in the current mod list
                if (Helper.ModRegistry.IsLoaded("furyx639.ExpandedStorage"))
                {
                    Monitor.Log("Attempting to hook into furyx639.ExpandedStorage.", LogLevel.Debug);
                    ApiManager.HookIntoExpandedStorage(Helper);

                    // Add the Harvest Statue via Expanded Storage, so we can make use of their expanded chest options
                    ApiManager.GetExpandedStorageApi().LoadContentPack(contentPack.Manifest, Path.Combine(Helper.DirectoryPath, AssetManager.assetFolderPath));
                }
                else
                {
                    // Add the Harvest Statue purely via Json Assets
                    ApiManager.GetDynamicGameAssetsInterface().AddEmbeddedPack(contentPack.Manifest, Path.Combine(Helper.DirectoryPath, AssetManager.assetFolderPath));
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Load all Parrot Pots
            Monitor.Log("Loading...", LogLevel.Trace);
            if (!Game1.MasterPlayer.modData.ContainsKey(IslandGatherers.hasConvertedOldStatues))
            {
                ConvertOldParrotStorage();
                Game1.MasterPlayer.modData[IslandGatherers.hasConvertedOldStatues] = true.ToString();
            }

            foreach (GameLocation location in Game1.locations)
            {
                DoMorningHarvest(Game1.getLocationFromName("IslandWest"));
            }
        }

        private void DoMorningHarvest(GameLocation location)
        {
            foreach (Chest chest in location.Objects.Values.Where(o => o.modData.ContainsKey(IslandGatherers.parrotPotFlag)))
            {
                // Reset daily modData flags
                chest.modData[IslandGatherers.spawnedJunimosFlag] = false.ToString();

                // Gather any crops nearby
                var harvestStatue = new ParrotPot(chest, location);
                harvestStatue.HarvestCrops(location, config.EnableHarvestMessage, config.DoParrotsEatExcessCrops, config.DoParrotsHarvestFromPots, config.DoParrotsHarvestFromFruitTrees, config.DoParrotsHarvestFromFlowers, config.DoParrotsSowSeedsAfterHarvest, config.MinimumFruitOnTreeBeforeHarvest);
            }
        }

        private void ConvertOldParrotStorage()
        {
            // Find all chests with the "is-harvest-statue" == "true"
            Monitor.Log("Loading...", LogLevel.Trace);
            ConvertFlaggedChestsToHarvestStatues(Game1.getLocationFromName("IslandWest"));
        }

        private void ConvertFlaggedChestsToHarvestStatues(GameLocation location)
        {
            foreach (Chest chest in location.Objects.Pairs.Where(p => p.Value is Chest).Select(p => p.Value).ToList())
            {
                if (!chest.modData.ContainsKey(oldParrotPotFlag))
                {
                    continue;
                }

                // Add the items from the temp Chest to the HarvestStatue
                var items = chest.items;
                var modData = chest.modData.Pairs;
                var tileLocation = chest.TileLocation;
                location.removeObject(chest.TileLocation, false);

                var statueItem = ApiManager.GetDynamicGameAssetsInterface().SpawnDGAItem("PeacefulEnd.IslandGatherers.ParrotPot/ParrotPot") as Item;
                var wasReplaced = (statueItem as StardewValley.Object).placementAction(location, (int)tileLocation.X * 64, (int)tileLocation.Y * 64, Game1.player);
                Monitor.Log($"Attempting to replace old Parrot Pot at {tileLocation} | Was Replaced: {wasReplaced}", LogLevel.Debug);

                // Move the modData over in case the Chests Anywhere mod is used (so we can retain name / category data)
                if (wasReplaced && location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is Chest statueObj)
                {
                    foreach (var pair in modData.Where(p => p.Key != oldParrotPotFlag))
                    {
                        statueObj.modData[pair.Key] = pair.Value;
                    }

                    foreach (var item in items.Where(i => i != null))
                    {
                        statueObj.addItem(item);
                    }
                }
            }
        }
    }
}
