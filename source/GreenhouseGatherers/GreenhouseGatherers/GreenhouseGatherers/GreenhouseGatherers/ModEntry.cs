/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using GreenhouseGatherers.GreenhouseGatherers.API;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using System;
using System.Reflection;
using HarmonyLib;
using StardewValley;
using System.Linq;
using GreenhouseGatherers.GreenhouseGatherers.Objects;
using GreenhouseGatherers.GreenhouseGatherers.Models;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Buildings;
using System.IO;
using GreenhouseGatherers.GreenhouseGatherers.Patches;
using GreenhouseGatherers.GreenhouseGatherers.Patches.Objects;

namespace GreenhouseGatherers.GreenhouseGatherers
{
    public class ModEntry : Mod
    {
        // Legacy save related
        private SaveData saveData;
        private string saveDataCachePath;

        // Save related
        internal static readonly string oldHarvestStatueFlag = "PeacefulEnd.GreenhouseGatherers/is-harvest-statue";
        internal static readonly string hasConvertedOldStatues = "PeacefulEnd.GreenhouseGatherers/HasConvertedToNewStatues";

        // Harvest Statue ModData
        internal static readonly string harvestStatueFlag = "PeacefulEnd.GreenhouseGatherers/HarvestStatue";
        internal static readonly string ateCropsFlag = "PeacefulEnd.GreenhouseGatherers/AteCrops";
        internal static readonly string spawnedJunimosFlag = "PeacefulEnd.GreenhouseGatherers/HasSpawnedJunimos";

        // Config related
        private ModConfig config;

        // Asset related
        internal static readonly string harvestStatuePath = Path.Combine("assets", "HarvestStatue");

        public override void Entry(IModHelper helper)
        {
            // Load the monitor
            ModResources.LoadMonitor(this.Monitor);

            // Load assets
            ModResources.LoadAssets(helper, harvestStatuePath);

            // Load our Harmony patches
            try
            {
                harmonyPatch();
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
                return;
            }

            // Load the config
            this.config = helper.ReadConfig<ModConfig>();

            // Hook into the game launch
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            // Hook into Player.Warped event so we can spawn some Junimos if the area was recently harvested
            helper.Events.Player.Warped += this.OnWarped;

            // Hook into save related events
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.OldLocation.objects.Values.Any(o => o.modData.ContainsKey(ModEntry.harvestStatueFlag)) && e.OldLocation.NameOrUniqueName != "CommunityCenter")
            {
                for (int i = e.OldLocation.characters.Count - 1; i >= 0; i--)
                {
                    if (e.OldLocation.characters[i] is Junimo)
                    {
                        e.OldLocation.characters.RemoveAt(i);
                    }
                }
            }

            if (!config.DoJunimosAppearAfterHarvest || !e.NewLocation.objects.Values.Any(o => o.modData.ContainsKey(ModEntry.harvestStatueFlag)))
            {
                return;
            }

            // Location contains a Harvest Statue, see if we need to spawn Junimos
            if (e.NewLocation.objects.Values.FirstOrDefault(o => o.modData.ContainsKey(ModEntry.harvestStatueFlag)) is Chest chest && chest != null)
            {
                if (bool.Parse(chest.modData[ModEntry.spawnedJunimosFlag]))
                {
                    return;
                }

                // Harvest Statue hasn't spawned some Junimos yet, so spawn a few temp ones for fluff
                if (e.NewLocation.NameOrUniqueName != "CommunityCenter")
                {
                    new HarvestStatue(chest, e.NewLocation).SpawnJunimos(config.MaxAmountOfJunimosToAppearAfterHarvest);
                }
            }
        }

        public void harmonyPatch()
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            new ObjectPatch(Monitor, Helper).Apply(harmony);
            new ChestPatch(Monitor, Helper).Apply(harmony);
            new CropPatch(Monitor, Helper).Apply(harmony);
            new JunimoPatch(Monitor, Helper).Apply(harmony);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Check if spacechase0's DynamicGameAssets is in the current mod list
            if (Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                Monitor.Log("Attempting to hook into spacechase0.DynamicGameAssets.", LogLevel.Debug);
                ApiManager.HookIntoDynamicGameAssets(Helper);

                var contentPack = Helper.ContentPacks.CreateTemporary(
                    Path.Combine(Helper.DirectoryPath, harvestStatuePath),
                    "PeacefulEnd.GreenhouseGatherers.HarvestStatue",
                    "PeacefulEnd.GreenhouseGatherers.HarvestStatue",
                    "Adds craftable Junimo Harvest Statues.",
                    "PeacefulEnd",
                    new SemanticVersion("1.0.0"));

                // Check if furyx639's Expanded Storage is in the current mod list
                if (Helper.ModRegistry.IsLoaded("furyx639.ExpandedStorage"))
                {
                    Monitor.Log("Attempting to hook into furyx639.ExpandedStorage.", LogLevel.Debug);
                    ApiManager.HookIntoExpandedStorage(Helper);

                    // Add the Harvest Statue via Expanded Storage, so we can make use of their expanded chest options
                    ApiManager.GetExpandedStorageInterface().LoadContentPack(contentPack.Manifest, Path.Combine(Helper.DirectoryPath, harvestStatuePath));
                }
                else
                {
                    // Add the Harvest Statue purely via Json Assets
                    ApiManager.GetDynamicGameAssetsInterface().AddEmbeddedPack(contentPack.Manifest, Path.Combine(Helper.DirectoryPath, harvestStatuePath));
                }
            }
        }

        [EventPriority(EventPriority.Low)]
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.MasterPlayer.mailReceived.Contains("WizardHarvestStatueRecipe") && (config.ForceRecipeUnlock || Game1.MasterPlayer.mailReceived.Contains("hasPickedUpMagicInk")))
            {
                Helper.Content.AssetEditors.Add(new RecipeMail());
                Game1.MasterPlayer.mailbox.Add("WizardHarvestStatueRecipe");
            }
            if (Game1.MasterPlayer.mailReceived.Contains("WizardHarvestStatueRecipe") && !Game1.MasterPlayer.knowsRecipe("HarvestStatueRecipe"))
            {
                Game1.MasterPlayer.craftingRecipes.Add("HarvestStatueRecipe", 0);
            }

            if (!Game1.MasterPlayer.modData.ContainsKey(ModEntry.hasConvertedOldStatues))
            {
                ConvertOldHarvestStatues();
            }

            // Do morning harvest
            foreach (GameLocation location in Game1.locations)
            {
                DoMorningHarvest(location);

                if (location is BuildableGameLocation)
                {
                    foreach (Building building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        DoMorningHarvest(indoorLocation);
                    }
                }
            }
        }

        private void DoMorningHarvest(GameLocation location)
        {
            foreach (Chest chest in location.Objects.Values.Where(o => o.modData.ContainsKey(ModEntry.harvestStatueFlag)))
            {
                // Reset daily modData flags
                chest.modData[ModEntry.spawnedJunimosFlag] = false.ToString();

                // Gather any crops nearby
                var harvestStatue = new HarvestStatue(chest, location);
                harvestStatue.HarvestCrops(location, config.EnableHarvestMessage, config.DoJunimosEatExcessCrops, config.DoJunimosHarvestFromPots, config.DoJunimosHarvestFromFruitTrees, config.DoJunimosHarvestFromFlowers, config.DoJunimosSowSeedsAfterHarvest, config.MinimumFruitOnTreeBeforeHarvest);
            }
        }

        private void ConvertOldHarvestStatues()
        {
            // Find all chests with the "is-harvest-statue" == "true"
            Monitor.Log("Loading...", LogLevel.Trace);
            foreach (GameLocation location in Game1.locations)
            {
                ConvertFlaggedChestsToHarvestStatues(location);

                if (location is BuildableGameLocation)
                {
                    foreach (Building building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        ConvertFlaggedChestsToHarvestStatues(indoorLocation);
                    }
                }
            }
        }

        private void ConvertFlaggedChestsToHarvestStatues(GameLocation location)
        {
            foreach (Chest chest in location.Objects.Pairs.Where(p => p.Value is Chest).Select(p => p.Value).ToList())
            {
                if (!chest.modData.ContainsKey(oldHarvestStatueFlag))
                {
                    continue;
                }

                // Add the items from the temp Chest to the HarvestStatue
                var items = chest.items;
                var modData = chest.modData.Pairs;
                var tileLocation = chest.TileLocation;
                location.removeObject(chest.TileLocation, false);

                var statueItem = ApiManager.GetDynamicGameAssetsInterface().SpawnDGAItem("PeacefulEnd.GreenhouseGatherers.HarvestStatue/HarvestStatue") as Item;
                var wasReplaced = (statueItem as StardewValley.Object).placementAction(location, (int)tileLocation.X * 64, (int)tileLocation.Y * 64, Game1.player);
                Monitor.Log($"Attempting to replace old Harvest Statue at {tileLocation} | Was Replaced: {wasReplaced}", LogLevel.Debug);

                // Move the modData over in case the Chests Anywhere mod is used (so we can retain name / category data)
                if (wasReplaced && location.objects.ContainsKey(tileLocation) && location.objects[tileLocation] is Chest statueObj)
                {
                    foreach (var pair in modData.Where(p => p.Key != oldHarvestStatueFlag))
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
