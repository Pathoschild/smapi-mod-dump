using BetterMixedSeeds.Config;
using BetterMixedSeeds.Data;
using BetterMixedSeeds.Patches;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Crop = BetterMixedSeeds.Config.Crop;

namespace BetterMixedSeeds
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors 
        *********/
        /// <summary>The list of seeds that mixed seeds can plant.</summary>
        public static List<Seed> Seeds { get; private set; }

        /// <summary>Provides methods for interacting with the mod directory.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor { get; private set; }

        /// <summary>The mod configuration.</summary>
        public static ModConfig ModConfig { get; private set; }


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            ModHelper = this.Helper;
            ModConfig = this.Helper.ReadConfig<ModConfig>();

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }


        /*********
        ** Private Methods 
        *********/
        /****
        ** Events
        ****/
        /// <summary>Invoked when the player has loaded a save. Used for calculating the possible seed based from config and installed mods.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            PopulateSeeds();
        }

        /****
        ** Methods
        ****/
        /// <summary>Apply the harmony patches for replacing game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Crop), new Type[] { typeof(int), typeof(int), typeof(int) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(CropPatch), nameof(CropPatch.ConstructorTranspile)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Crop), nameof(StardewValley.Crop.getRandomLowGradeCropForThisSeason)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CropPatch), nameof(CropPatch.RandomCropPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), "cutWeed"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.CutWeedPrefix)))
            );
        }

        /// <summary>Calculates what seeds should dropped from mixed seeds. This is dependant on the config and installed mods.</summary>
        private void PopulateSeeds()
        {
            List<Seed> enabledSeeds = new List<Seed>();

            // add seeds from Stardew Valley
            enabledSeeds = enabledSeeds
                .Union(CheckModForEnabledCrops("StardewValley"))
                .ToList();

            // add seeds from integrated mods
            List<string> integratedModsInstalled = CheckIntegratedMods();

            object jaApi = null;
            if (integratedModsInstalled.Count() != 0)
            {
                jaApi = this.Helper.ModRegistry.GetApi("spacechase0.JsonAssets");
                if (jaApi != null)
                {
                    foreach (string modName in integratedModsInstalled)
                    {
                        ModMonitor.Log($"{modName} loaded");

                        enabledSeeds = enabledSeeds
                            .Union(CheckModForEnabledCrops(modName))
                            .ToList();
                    }
                }
                else
                {
                    ModMonitor.Log("Failed to retrieve Json Assets API", LogLevel.Error);
                }
            }

            List<Seed> updatedEnabledSeeds = new List<Seed>();

            foreach (var enabledSeed in enabledSeeds)
            {
                Seed newEnabledSeed = enabledSeed;
                int seedId;

                // check if the current seedName is a number, meaning it's a Stardew seed
                if (int.TryParse(enabledSeed.Name, out seedId))
                {
                    newEnabledSeed.Id = seedId;
                }
                else
                {
                    // use JA API to get the seed if from the name
                    seedId = this.Helper.Reflection.GetMethod(jaApi, "GetObjectId").Invoke<int>(enabledSeed.Name);

                    if (seedId == -1)
                    {
                        ModMonitor.Log($"Could find seed index for item: {enabledSeed.Name}", LogLevel.Error);
                        continue;
                    }

                    newEnabledSeed.Id = seedId;
                }

                updatedEnabledSeeds.Add(newEnabledSeed);
            }

            Seeds = updatedEnabledSeeds;
        }

        /// <summary>The method that detects which other crop mods the player has installed to add these crops to the possible mixed seed output.</summary>
        /// <returns>List of internal mod names for use with the config and seed index.</returns>
        private List<string> CheckIntegratedMods()
        {
            List<string> integratedModsInstalled = new List<string>();

            // the uniqueId (key) with the internal name (value) for each integrated mod
            Dictionary<string, string> integratedMods = new Dictionary<string, string> {
                { "ParadigmNomad.FantasyCrops", "FantasyCrops" },
                { "paradigmnomad.freshmeat", "FreshMeat" },
                { "ppja.fruitsandveggies", "FruitAndVeggies" },
                { "mizu.flowers", "MizusFlowers" },
                { "PPJA.cannabiskit", "CannabisKit" },
                { "Popobug.SPCFW", "SixPlantableCrops" },
                { "BFV.FruitVeggie", "BonsterCrops" },
                { "RevenantCrops", "RevenantCrops" },
                { "kildarien.farmertoflorist", "FarmerToFlorist" },
                { "Fish.LuckyClover", "LuckyClover" },
                { "Fish.FishsFlowers", "FishsFlowers" },
                { "Fish.FishsFlowersCompatibilityVersion", "FishsFlowersCompatibilityVersion" },
                { "StephansLotsOfCrops", "StephansLotsOfCrops" },
                { "minervamaga.JA.EemieCrops", "EemiesCrops" },
                { "jfujii.TeaTime", "TeaTime" },
                { "Mae.foragetofarm", "ForageToFarm" },
                { "rearda88.GemandMineralCrops", "GemAndMineralCrops" },
                { "6480.crops.arabidopsis", "MouseEarCress" },
                { "ppja.ancientcrops", "AncientCrops" },
                { "PokeCropsJson", "PokeCrops" },
                { "jawsawn.StarboundValley", "StarboundValley" },
                { "key.cropspack", "IKeychainsWinterLycheePlant" },
                { "hung2563hn.GreenPear", "GreenPear" },
                { "BlatantDecoy.SodaVine", "SodaVine" },
                { "amburr.spoopyvalley", "SpoopyValley" },
                { "yaramy.svbakery", "StardewBakery"},
                { "Hesper.JA.Succulents", "Succulents" },
                { "SSaturn.TropicalFarm", "TropicalFarm" }
            };

            foreach (var integratedMod in integratedMods)
            {
                if (this.Helper.ModRegistry.IsLoaded(integratedMod.Key))
                {
                    integratedModsInstalled.Add(integratedMod.Value);
                }
            }

            return integratedModsInstalled;
        }

        /// <summary>The method for finding which seeds are currently enabled in the specified mod.</summary>
        /// <param name="modName">The internal mod name that will be used for getting the list of seeds.</param>
        /// <returns>A list of enabled seeds.</returns>
        private List<Seed> CheckModForEnabledCrops(string modName)
        {
            List<Seed> seedNames = new List<Seed>();

            // get the CropMod object for the current mod from config
            PropertyInfo configInfo = typeof(ModConfig).GetProperty(modName);
            CropMod mod = (CropMod)configInfo.GetValue(ModConfig);

            // get the 4 season properties for the current CropMod
            PropertyInfo[] modSeasonsInfo = typeof(CropMod).GetProperties();

            // get the seed index for finding seed names for the crops
            PropertyInfo seedIndexInfo = typeof(CropModData).GetProperty(modName);
            List<SeedData> seedIndex = (List<SeedData>)seedIndexInfo.GetValue(null);

            foreach (var modSeasonInfo in modSeasonsInfo)
            {
                Season season = (Season)modSeasonInfo.GetValue(mod);
                string seasonName = modSeasonInfo.Name.ToLower();

                if (season == null)
                {
                    continue;
                }

                foreach (Crop crop in season.Crops)
                {
                    if (!crop.Enabled || crop.Chance == 0)
                    {
                        continue;
                    }

                    SeedData seedData = seedIndex
                        .Where(seed => seed.CropName == crop.Name)
                        .FirstOrDefault();

                    // check both name and id as a seed could have either populated, depending if its a SDV crop, or JA crop
                    if (string.IsNullOrEmpty(seedData.SeedName) && seedData.SeedId == -1)
                    {
                        ModMonitor.Log($"Seed name for {crop.Name} couldn't be found", LogLevel.Error);
                        continue;
                    }

                    if (ModEntry.ModConfig.UseCropYearRequirement && Game1.year < seedData.YearRequirement)
                    {
                        ModMonitor.Log($"Skipped {crop.Name} as year requirement was not met");
                        continue;
                    }

                    // if the seed is already in the dictionary, add the season to the array
                    if (seedNames.Where(seed => seed.Name == seedData.SeedName).Any())
                    {
                        seedNames
                            .Where(seed => seed.Name == seedData.SeedName)
                            .Select(seed => seed.Seasons.Add(seasonName));
                    }
                    else
                    {
                        // add the to the list [Chance] number of times, so it has an effect on the final result
                        for (int j = 0; j < crop.Chance; j++)
                        {
                            // if the seed name is null, load the seed id instead. this is only the case for base game seeds. this is so the code that handles converting ja names to ids doesn't run the base game crops
                            string seedName = seedData.SeedName;
                            if (seedData.SeedName == null)
                            {
                                seedName = seedData.SeedId.ToString();
                            }

                            seedNames.Add(new Seed(0, seedName, new string[1] { seasonName }));
                        }

                        ModMonitor.Log($"{seedData.CropName} has been added to the seed list");
                    }
                }
            }

            return seedNames;
        }
    }
}