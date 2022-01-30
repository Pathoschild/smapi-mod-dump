/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/GreenhouseSprinklers
**
*************************************************/

using Bpendragon.GreenhouseSprinklers.Data;

using GreenhouseSprinklers.APIs;

using HarmonyLib;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

using System;
using System.Collections.Generic;
using System.Linq;
using Bpendragon.GreenhouseSprinklers.Patches;

namespace Bpendragon.GreenhouseSprinklers
{
    partial class ModEntry : Mod, IAssetLoader
    {
        private ModConfig Config;
        public Dictionary<int, int> BuildMaterials1 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials2 { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> BuildMaterials3 { get; set; } = new Dictionary<int, int>();
        public Difficulty difficulty;
        const string ModDataKey = "Bpendragon.GreenhouseSprinklers.GHLevel";

        public static int GetUpgradeLevel(GreenhouseBuilding greenhouse)
        {
            return greenhouse.modData.TryGetValue(ModDataKey, out string level)
               ? int.Parse(level)
               : 0;
        }

        public override void Entry(IModHelper helper)
        {
            //set up for translations
            I18n.Init(helper.Translation);
            //read settings
            Config = Helper.ReadConfig<ModConfig>();
            SetBuildMaterials();

            //Set up harmony to patch the Building.upgrade function
            BuildingPatches.Initialize(Monitor, Helper, Config);
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Building), nameof(Building.dayUpdate)),
                prefix: new HarmonyMethod(typeof(BuildingPatches), nameof(BuildingPatches.Upgrade_Prefix))
            );

            helper.ConsoleCommands.Add("ghs_setlevel", "Sets the level for the greenhouse.\n\nUsage: ghs_setlevel <value>\n- value: integer between 0 and 3 inclusive", SetGHLevel);
            helper.ConsoleCommands.Add("ghs_getlevel", "Returns the level for the greenhouse.\n\nUsage: ghs_setlevel", GetGHLevel);
            helper.ConsoleCommands.Add("ghs_waternow", "Debug command to force watering the greenhouse (and farm if level 3 unlocked).\n\nUsage: ghs_waternow", WaterNow);

            //Register Event Listeners
            helper.Events.GameLoop.DayStarted += OnDayStart;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.SaveLoaded += OnLoad;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.Saved += OnSaveCompleted;
        }

        private void WaterNow(string command, string[] args)
        {
            int level = GetUpgradeLevel(Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault());
            Monitor.Log($"Greenhouse is level {level}", LogLevel.Info);
            if(level == 0) Monitor.Log($"Watering nothing, no upgrades purchased", LogLevel.Info);
            if (level >= 1)
            {
                Monitor.Log($"Watering Greenhouse", LogLevel.Info);
                WaterGreenHouse();
            }
            if (level >= 3)
            {
                Monitor.Log($"Watering Farm", LogLevel.Info);
                WaterFarm();
            }
        }

        private void SetGHLevel(string command, string[] args)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            gh.modData[ModDataKey] = args[0];

            if(Config.ShowVisualUpgrades) Helper.Content.InvalidateCache("Buildings/Greenhouse");

            Monitor.Log($"Set Greenhouse to level {args[0]} and refreshed texture cache if required.", LogLevel.Info);

        }

        private void GetGHLevel(string command, string[] args)
        {
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            Monitor.Log($"Greenhouse is level {GetUpgradeLevel(gh)}.", LogLevel.Info);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Helper.Content.AssetEditors.Add(new MyModMail());


            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            if (api != null)
            {
                api.RegisterToken(ModManifest, "GreenHouseLevel", () =>
                {

                    if (Context.IsWorldReady)
                    {
                        var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
                        return new[] { GetUpgradeLevel(gh).ToString() };
                    }
                    else return new[] { "0" };
                });
            }
        }

        private void SetBuildMaterials()
        {
            var diff = Config.DifficultySettings.First(x => x.Difficulty == Config.SelectedDifficulty);
            difficulty = Config.SelectedDifficulty;
            if (null == diff)
            {
                Monitor.Log("Difficulty Settings not found or set, mod will not work properly", LogLevel.Error);
            }

            BuildMaterials1[(int)diff.FirstUpgrade.Sprinkler] = diff.FirstUpgrade.SprinklerCount;
            BuildMaterials1[787] = diff.FirstUpgrade.Batteries;

            BuildMaterials2[(int)diff.SecondUpgrade.Sprinkler] = diff.SecondUpgrade.SprinklerCount;
            BuildMaterials2[787] = diff.SecondUpgrade.Batteries;

            BuildMaterials3[(int)diff.FinalUpgrade.Sprinkler] = diff.FinalUpgrade.SprinklerCount;
            BuildMaterials3[787] = diff.FinalUpgrade.Batteries;

        }


        private void WaterGreenHouse()
        {
            var gh = Game1.getLocationFromName("Greenhouse");
            if (gh != null)
            {
                Monitor.Log("Greenhouse Located");
            }
            else
            {
                Monitor.Log("No Greenhouse found", LogLevel.Warn);
            }
            int i = 0;
            var terrainfeatures = gh.terrainFeatures.Values;
            Monitor.Log($"Found {terrainfeatures.Count()} terrainfeatures in Greenhouse");

            foreach (var tf in terrainfeatures)
            {
                if (tf is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    i++;
                }
            }
            Monitor.Log($"{i} tiles watered.");

            int j = 0;
            Monitor.Log("Watering pots in greenhouse");
            foreach (IndoorPot pot in gh.objects.Values.OfType<IndoorPot>())
            {
                if (pot.hoeDirt.Value is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    pot.showNextIndex.Value = true;
                    j++;
                }
            }

            Monitor.Log($"{j} Pots Watered");
        }

        private void WaterFarm()
        {
            var farm = Game1.getFarm();
            int i = 0;
            var terrainFeatures = farm.terrainFeatures.Values;
            Monitor.Log($"Found {terrainFeatures.Count()} terrainfeatures in Farm");

            foreach (var tf in terrainFeatures)
            {
                bool shouldWaterTile = true;
                if (Game1.whichFarm == Farm.beach_layout && !Config.WaterSandOnBeachFarm)
                {
                    //If Property "NoSprinklers" is Set to true, don't water
                    shouldWaterTile = farm.doesTileHaveProperty((int)tf.currentTileLocation.X, (int)tf.currentTileLocation.Y, "NoSprinklers", "Back") != "T";
                }

                if (tf is HoeDirt dirt && shouldWaterTile)
                {
                    dirt.state.Value = HoeDirt.watered;
                    i++;
                }
            }
            Monitor.Log($"{i} tiles watered.");

            int j = 0;
            Monitor.Log("Watering pots on farm");
            foreach (IndoorPot pot in farm.objects.Values.OfType<IndoorPot>())
            {
                if (pot.hoeDirt.Value is HoeDirt dirt)
                {
                    dirt.state.Value = HoeDirt.watered;
                    pot.showNextIndex.Value = true;
                    j++;
                }
            }

            Monitor.Log($"{j} Pots Watered");
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (!Context.IsWorldReady) return false;
            var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
            if (asset.AssetNameEquals("Buildings/Greenhouse") && GetUpgradeLevel(gh) > 0 && Config.ShowVisualUpgrades)
            {
                return true;
            }

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings/Greenhouse"))
            {
                var gh = Game1.getFarm().buildings.OfType<GreenhouseBuilding>().FirstOrDefault();
                
                return Helper.Content.Load<T>($"assets/Greenhouse{GetUpgradeLevel(gh)}.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }


    }
}
