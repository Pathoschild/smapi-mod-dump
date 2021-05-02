/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using Harmony;
using IslandGatherers.Framework;
using IslandGatherers.Framework.Objects;
using IslandGatherers.Framework.Patches;
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
        public static int parrotStorageID;

        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static ModConfig config;
        internal static readonly string parrotPotFlag = "PeacefulEnd.IslandGatherers_IsParrotStorage";

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
                var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

                // Apply our patches
                new ObjectPatch(monitor).Apply(harmony);
                new CropPatch(monitor).Apply(harmony);
                new IslandNorthPatch(monitor).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Hook into GameLoop events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }

        private void OnIdsAssigned(object sender, EventArgs e)
        {
            // Get the Harvest Statue item ID
            parrotStorageID = ApiManager.GetJsonAssetsApi().GetBigCraftableId("Parrot Pot");
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

            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets") && ApiManager.HookIntoJsonAssets(Helper))
            {
                var jsonAssetsApi = ApiManager.GetJsonAssetsApi();

                // Hook into Json Asset's IdsAssigned event
                jsonAssetsApi.IdsAssigned += this.OnIdsAssigned;

                // Check if furyx639's Expanded Storage is in the current mod list
                if (Helper.ModRegistry.IsLoaded("furyx639.ExpandedStorage") && ApiManager.HookIntoExpandedStorage(Helper))
                {
                    var expandedStorageApi = ApiManager.GetExpandedStorageApi();

                    // Add the Harvest Statue via Expanded Storage, so we can make use of their expanded chest options
                    expandedStorageApi.LoadContentPack(Path.Combine(Helper.DirectoryPath, "assets", "[JA] Island Gatherers Pack"));
                }
                else
                {
                    // Load in our assets
                    jsonAssetsApi.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", "[JA] Island Gatherers Pack"));
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Load all Parrot Pots
            Monitor.Log("Loading...", LogLevel.Trace);
            foreach (GameLocation location in Game1.locations)
            {
                ConvertChestToParrotStorage(location);

                if (location is BuildableGameLocation)
                {
                    foreach (Building building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        ConvertChestToParrotStorage(indoorLocation);
                    }
                }
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            // Find all Parrot Pots
            Monitor.Log("Saving...", LogLevel.Trace);
            foreach (GameLocation location in Game1.locations)
            {
                ConvertParrotStorageToChest(location);

                if (location is BuildableGameLocation)
                {
                    foreach (Building building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        ConvertParrotStorageToChest(indoorLocation);
                    }
                }
            }
        }

        private void ConvertChestToParrotStorage(GameLocation location)
        {
            foreach (Chest chest in location.Objects.Pairs.Where(p => p.Value is Chest).Select(p => p.Value).ToList())
            {
                if (chest is ParrotPot || !chest.modData.ContainsKey(parrotPotFlag))
                {
                    continue;
                }

                // Add the items from the temp Chest to the ParrotPot
                ParrotPot parrotPot = new ParrotPot(chest.TileLocation, parrotStorageID, config.EnableHarvestMessage, config.DoParrotsEatExcessCrops, config.DoParrotsHarvestFromPots, config.DoParrotsHarvestFromFruitTrees, config.DoParrotsHarvestFromFlowers, config.DoParrotsSowSeedsAfterHarvest, config.MinimumFruitOnTreeBeforeHarvest);
                parrotPot.AddItems(chest.items);

                // Move the modData over in case the Chests Anywhere mod is used (so we can retain name / category data)
                parrotPot.modData = chest.modData;

                // Remove the temp Chest by placing ParrotPot
                location.setObject(chest.TileLocation, parrotPot);

                // Gather any crops nearby
                parrotPot.HarvestCrops(location);
            }
        }

        private void ConvertParrotStorageToChest(GameLocation location)
        {
            foreach (ParrotPot parrotPot in location.Objects.Pairs.Where(p => p.Value is ParrotPot).Select(p => p.Value).ToList())
            {
                // Add the items from ParrotPot to temp Chest, so the player will still have their items if mod is uninstalled
                Chest chest = new Chest(true, parrotPot.TileLocation);
                foreach (var item in parrotPot.items.Where(i => i != null))
                {
                    chest.addItem(item);
                }

                // Retain any previous modData in case the Chests Anywhere mod is used (so we can retain name / category data)
                chest.modData = parrotPot.modData;

                // Ensure the proper flag is within the modData
                chest.modData[parrotPotFlag] = String.Empty;

                // Remove the ParrotPot by placing the Chest
                location.setObject(parrotPot.TileLocation, chest);
            }
        }
    }
}
