/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BetterMixedSeeds.Models;
using BetterMixedSeeds.Models.Config;
using BetterMixedSeeds.Patches;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Crop = BetterMixedSeeds.Models.Config.Crop;

namespace BetterMixedSeeds
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The list of all base game seed objects.</summary>
        /// <remarks>This is determined at runtime so the mod doesn't need patching in later game versions that add new seeds.</remarks>
        private List<Seed> GameSeeds = new List<Seed>();

        /// <summary>The list of all the loaded Json Assets seed objects.</summary>
        private List<Seed> JASeeds = new List<Seed>();

        /// <summary>The Json Assets crop mods with seed objects.</summary>
        private Dictionary<IManifest, List<Seed>> JACropMods = new Dictionary<IManifest, List<Seed>>();

        /// <summary>The year requirements of base game crops.</summary>
        /// <remarks>This isn't stored in any data file, rather they're hard coded.<br/>Key: seed id, Value: year requirement.</remarks>
        private Dictionary<int, int> GameCropYearRequirements = new Dictionary<int, int>
        {
            { 476, 2 },
            { 273, 2 },
            { 485, 2 },
            { 489, 2 }
        };


        /*********
        ** Accessors 
        *********/
        /// <summary>The list of seeds that mixed seeds can plant.</summary>
        public List<Seed> Seeds { get; private set; }

        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; private set; }

        /// <summary>The singleton instance for <see cref="BetterMixedSeeds.ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            // load the config
            Config = this.Helper.ReadConfig<ModConfig>();

            // add event handlers
            this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when the player launches the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>Used for getting the base game crops (before Json Assets intercepts the file).</remarks>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // load the required game data
            var cropData = this.Helper.Content.Load<Dictionary<int, string>>(Path.Combine("Data", "Crops"), ContentSource.GameContent);
            var objectData = this.Helper.Content.Load<Dictionary<int, string>>(Path.Combine("Data", "ObjectInformation"), ContentSource.GameContent);

            // cache all crops (this won't include JA crop mods, which is wanted, so we know which crops to put under 'StardewValley' in the config file)
            foreach (var cropInfo in cropData)
            {
                // get crop name from objectData using crop id from cropInfo
                var cropName = objectData.FirstOrDefault(kvp => kvp.Key.ToString() == cropInfo.Value.Split('/')[3]).Value?.Split('/')[0] ?? "";

                // add a separate seed for each season (so dropChance can be different between seasons)
                var seasons = cropInfo.Value.Split('/')[1].Split(' ');
                foreach (var season in seasons)
                {
                    // get the existing crop config (for the drop chance)
                    var cropConfig = Config.StardewValley?.GetSeasonByName(season)?.Crops.FirstOrDefault(crop => crop.Name.ToLower() == cropName.ToLower());

                    // get whether the crop is a trellis crop
                    bool.TryParse(cropInfo.Value.Split('/')[7], out var isTrellis);

                    GameSeeds.Add(new Seed(
                        id: cropInfo.Key,
                        cropName: cropName,
                        dropChance: cropConfig?.DropChance ?? 1,
                        isTrellis: isTrellis,
                        yearRequirement: (GameCropYearRequirements.ContainsKey(cropInfo.Key)
                            ? GameCropYearRequirements[cropInfo.Key]
                            : 0),
                        season: season
                    ));
                }
            }
        }

        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>Used for generating the config and loading the valid seeds.</remarks>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // get all Json Assets packs that add crops (this needs to be done through the SMAPI mod registry and reflection as the JA API doesn't expose this in it's API)
            var jaModData = this.Helper.ModRegistry.Get("SpaceChase0.JsonAssets");
            if (jaModData != null)
            {
                // get all required Json Assets data
                this.Monitor.Log("Json Assets instance found, retrieving crop mods...");
                var jaInstance = (Mod)jaModData.GetType().GetProperty("Mod", BindingFlags.Public | BindingFlags.Instance).GetValue(jaModData);
                JASeeds = GetJASeeds(jaInstance);
            }

            // clean config (remove old seeds, add new ones)
            CleanConfig();

            // save config
            this.Helper.WriteConfig(Config);

            // get the enabled seeds
            Seeds = GetAllEnabledSeeds();

            // log current seeds
            foreach (var seed in Seeds)
                this.Monitor.Log($"Added seed: Id: {seed.Id}, CropName: {seed.CropName}, DropChance: {seed.DropChance}, Season: {seed.Season}, IsTrellis: {seed.IsTrellis}, YearRequirement: {seed.YearRequirement}");

            // add harmony patches
            ApplyHarmonyPatches();
        }

        /// <summary>Gets all the loaded crops from a specified Json Assets instance.</summary>
        /// <param name="jaInstance">The Json Assets instance to get the loaded crops from.</param>
        /// <returns>The loaded crops from the specifed Json Assets instance.</returns>
        private List<Seed> GetJASeeds(Mod jaInstance)
        {
            var jaSeeds = new List<Seed>();

            // get crop data directly from Json Assets
            var jaCropMods = (Dictionary<IManifest, List<string>>)jaInstance.GetType().GetField("cropsByContentPack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(jaInstance);
            var jaCrops = jaInstance.GetType().GetField("crops", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(jaInstance);

            // convert to local type then convert to seeds
            JACropMods = ConvertJACropMods(jaCropMods, jaCrops);

            // combine all crop mods to single crop collection
            foreach (var jaCropMod in JACropMods)
                jaSeeds.AddRange(jaCropMod.Value);

            return jaSeeds;
        }

        /// <summary>Converts a Json Assets crop data collection into a usable local type collection.</summary>
        /// <param name="jaCropMods">The json assets crops by content packs.</param>
        /// <param name="jaCrops">The collection of Json Assets crop models to convert.</param>
        /// <returns>The Json Assets crop data in a local type.</returns>
        private Dictionary<IManifest, List<Seed>> ConvertJACropMods(Dictionary<IManifest, List<string>> jaCropMods, object jaCrops)
        {
            var jaSeedsByContentPack = new Dictionary<IManifest, List<Seed>>();

            // convert jaCrops to usable object
            var jaCropsCollection = ((IEnumerable)jaCrops).Cast<object>();
            var jaSeeds = new List<Seed>();

            foreach (var jaCrop in jaCropsCollection)
            {
                // get all required seed / crop properties
                var seedId = (int)jaCrop.GetType().GetMethod("GetSeedId", BindingFlags.Public | BindingFlags.Instance).Invoke(jaCrop, null);
                var cropName = (string)jaCrop.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance).GetValue(jaCrop);
                var isTrellis = (bool)jaCrop.GetType().GetProperty("TrellisCrop", BindingFlags.Public | BindingFlags.Instance).GetValue(jaCrop);
                var seasons = (List<string>)jaCrop.GetType().GetProperty("Seasons", BindingFlags.Public | BindingFlags.Instance).GetValue(jaCrop);
                var yearRequirement = 0;

                // get year requirement
                var purchaseRequirements = (List<string>)jaCrop.GetType().GetProperty("SeedPurchaseRequirements", BindingFlags.Public | BindingFlags.Instance).GetValue(jaCrop);
                if (purchaseRequirements != null)
                {
                    var stringYearRequirement = purchaseRequirements
                        .FirstOrDefault(purchaseRequirement => purchaseRequirement.StartsWith("y "));

                    if (!string.IsNullOrEmpty(stringYearRequirement))
                        int.TryParse(stringYearRequirement.Split(' ')[1], out yearRequirement);
                }

                // add the seed for each season
                foreach (var season in seasons)
                {
                    // get the drop chance of the seed if the seed is already in the config file
                    var dropChance = 1f;
                    var jaCropMod = jaCropMods.Where(cropMod => cropMod.Value.Contains(cropName)).Select(cropMod => cropMod.Key).FirstOrDefault();
                    if (jaCropMod != null)
                        if (Config.CropModSettings.TryGetValue(jaCropMod.UniqueID, out var cropMod))
                            dropChance = cropMod.GetSeasonByName(season)?.Crops?.FirstOrDefault(crop => crop.Name.ToLower() == cropName.ToLower())?.DropChance ?? 1;

                    jaSeeds.Add(new Seed(seedId, cropName, dropChance, isTrellis, yearRequirement, season));
                }
            }

            // loop through each JA crop content pack and recreate with seed objects
            foreach (var jaCropMod in jaCropMods)
            {
                // get all the seeds for the crop mod
                var cropModSeeds = new List<Seed>();

                // get all the seeds from crop names
                foreach (var cropName in jaCropMod.Value)
                {
                    var seeds = jaSeeds.Where(jaSeed => jaSeed.CropName.ToLower() == cropName.ToLower());
                    if (seeds.Count() == 0)
                    {
                        Monitor.Log($"Couldn't find seed with cropName: {cropName}", LogLevel.Error);
                        continue;
                    }

                    cropModSeeds.AddRange(seeds);
                }

                jaSeedsByContentPack.Add(jaCropMod.Key, cropModSeeds);
            }

            return jaSeedsByContentPack;
        }

        /// <summary>Cleans the <see cref="Config"/>.</summary>
        /// <remarks>Cleaning consists of adding any missing crops or crop mods and removing any crops or crop mods that haven't been loaded.</remarks>
        private void CleanConfig()
        {
            this.Monitor.Log("Cleaning config...");

            // clean stardew valley seeds (this is used for if a game update adds new seeds)
            Config.StardewValley = CreateCropModFromSeeds(GameSeeds, Config.StardewValley);

            // clean json assets crop mods seeds
            {
                // add the crop mod to config if it doesn't already exist
                foreach (var jaCropMod in JACropMods)
                    if (!Config.CropModSettings.ContainsKey(jaCropMod.Key.UniqueID))
                        Config.CropModSettings.Add(jaCropMod.Key.UniqueID, CreateCropModFromSeeds(jaCropMod.Value));
                    else
                        Config.CropModSettings[jaCropMod.Key.UniqueID] = CreateCropModFromSeeds(jaCropMod.Value, Config.CropModSettings[jaCropMod.Key.UniqueID]);

                // remove crop mod from config if it hasn't been loaded by JA
                for (int i = 0; i < Config.CropModSettings.Count; i++)
                {
                    var cropMod = Config.CropModSettings.ElementAt(i);
                    if (!JACropMods.Keys.Any(cropModManifest => cropModManifest.UniqueID.ToLower() == cropMod.Key.ToLower()))
                    {
                        Config.CropModSettings.Remove(cropMod.Key);
                        i--; // decrement i to account for the removed item
                    }
                }
            }
        }

        /// <summary>Creates a <see cref="BetterMixedSeeds.Models.Config.CropMod"/> from a collection of <see cref="BetterMixedSeeds.Models.Seed"/>s.</summary>
        /// <param name="seeds">The seeds to convert.</param>
        /// <param name="oldCropMod">The old <see cref="BetterMixedSeeds.Models.Config.CropMod"/> object to copy config options from.</param>
        /// <returns>The created crop mod object.</returns>
        private CropMod CreateCropModFromSeeds(IEnumerable<Seed> seeds, CropMod oldCropMod = null)
        {
            // sort all crop ids into seasons
            var springCrops = new List<Crop>();
            var summerCrops = new List<Crop>();
            var fallCrops = new List<Crop>();
            var winterCrops = new List<Crop>();

            foreach (var seed in seeds)
            {
                if (seed.Season.ToLower() == "spring")
                {
                    GetExistingCropConfigValues(oldCropMod, "spring", seed.CropName, out var enabled, out var dropChance);
                    springCrops.Add(new Crop(seed.CropName, enabled, dropChance));
                }
                else if (seed.Season.ToLower() == "summer")
                {
                    GetExistingCropConfigValues(oldCropMod, "summer", seed.CropName, out var enabled, out var dropChance);
                    summerCrops.Add(new Crop(seed.CropName, enabled, dropChance));
                }
                else if (seed.Season.ToLower() == "fall")
                {
                    GetExistingCropConfigValues(oldCropMod, "fall", seed.CropName, out var enabled, out var dropChance);
                    fallCrops.Add(new Crop(seed.CropName, enabled, dropChance));
                }
                else if (seed.Season.ToLower() == "winter")
                {
                    GetExistingCropConfigValues(oldCropMod, "winter", seed.CropName, out var enabled, out var dropChance);
                    winterCrops.Add(new Crop(seed.CropName, enabled, dropChance));
                }
            }

            // return converted object
            return new CropMod(new Season(springCrops), new Season(summerCrops), new Season(fallCrops), new Season(winterCrops), oldCropMod?.Enabled ?? true); ;
        }

        /// <summary>Gets the existing crop config values from a specified <see cref="BetterMixedSeeds.Models.Config.CropMod"/>, <paramref name="season"/>, and <paramref name="cropName"/>.</summary>
        /// <param name="cropMod">The crop mod to get the config values from.</param>
        /// <param name="season">The season to get the values from.</param>
        /// <param name="cropName">The name of the crop.</param>
        /// <param name="enabled">The enabled config value of the crop.</param>
        /// <param name="dropChance">The drop chance config value of the crop.</param>
        private void GetExistingCropConfigValues(CropMod cropMod, string season, string cropName, out bool enabled, out float dropChance)
        {
            // setup default values
            enabled = true;
            dropChance = 1;

            // validate
            if (cropMod == null)
            {
                // set the default enabled value for cactus fruit to be false, this is because they die when planted outside (it'll be just disabled instead of removed because some mods such as Seeds Are Rare may rely on this mod being able to drop cactus fruit, even if it requires users to change their configuration)
                if (cropName.ToLower() == "cactus fruit")
                    enabled = false;

                // set the default enabled value for qu fruit to be false, this is a quest item so it's rather unbalanced to have this enabled by default
                if (cropName.ToLower() == "qi fruit")
                    enabled = false;

                return;
            }

            // get Crop object
            var crop = cropMod.GetSeasonByName(season)?.Crops?.FirstOrDefault(c => c.Name.ToLower() == cropName.ToLower());
            if (crop == null)
                return;

            enabled = crop.Enabled;
            dropChance = crop.DropChance;
        }

        /// <summary>Gets all the enabled seeds from the <see cref="Config"/>.</summary>
        /// <returns>All the enabled seeds from the <see cref="Config"/>.</returns>
        private List<Seed> GetAllEnabledSeeds()
        {
            var enabledSeeds = new List<Seed>();

            // cache required data to convert crop name to seed id
            var objectInfoData = this.Helper.Content.Load<Dictionary<int, string>>(Path.Combine("Data", "ObjectInformation"), ContentSource.GameContent);
            var cropData = this.Helper.Content.Load<Dictionary<int, string>>(Path.Combine("Data", "Crops"), ContentSource.GameContent);

            // loop through each crop mod in config
            var cropMods = Config.CropModSettings.Values.Add(Config.StardewValley);
            foreach (var cropMod in cropMods)
            {
                // ensure crop mod is enabled
                if (!cropMod.Enabled)
                    continue;

                // loop through each season and look for enabled crops
                var seasonNames = new[] { "spring", "summer", "fall", "winter" };
                foreach (var seasonName in seasonNames)
                {
                    var season = cropMod.GetSeasonByName(seasonName);
                    var crops = season?.Crops.Where(crop => crop.Enabled && crop.DropChance > 0);
                    if (crops == null)
                        continue;

                    foreach (var crop in crops)
                    {
                        // get crop id from name
                        var cropId = objectInfoData
                            .Where(objectInfo => objectInfo.Value.Split('/')[0].ToLower() == crop.Name.ToLower())
                            .Select(objectInfo => objectInfo.Key)
                            .FirstOrDefault();

                        // get seed id from crop id
                        var seedId = cropData
                            .Where(cropInfo => cropInfo.Value.Split('/')[3] == cropId.ToString())
                            .Select(cropInfo => cropInfo.Key)
                            .FirstOrDefault();

                        // ensure the crop and it's seed could be found
                        if (seedId == 0)
                        {
                            this.Monitor.Log($"Crop: {crop.Name} (CropId: {cropId}) couldn't be found", LogLevel.Warn);
                            continue;
                        }

                        // get corresponding seed
                        var seed = GameSeeds.FirstOrDefault(seedInfo => seedInfo.Id == seedId && seedInfo.Season.ToLower() == seasonName.ToLower())
                            ?? JASeeds.FirstOrDefault(seedInfo => seedInfo.Id == seedId && seedInfo.Season.ToLower() == seasonName.ToLower());

                        if (seed == null)
                        {
                            this.Monitor.Log($"Seed: {seedId} couldn't be found with season: {seasonName.ToLower()}", LogLevel.Warn);
                            continue;
                        }

                        // ensure seed meets trellis and year config requirements
                        if ((!Config.EnableTrellisCrops && seed.IsTrellis)
                            || (Config.UseSeedYearRequirement && Game1.year < seed.YearRequirement))
                            continue;

                        enabledSeeds.Add(seed);
                    }
                }
            }

            return enabledSeeds;
        }

        /// <summary>Applies harmony patches for patching source code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create harmony instance for patching game code
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Crop), new Type[] { typeof(int), typeof(int), typeof(int) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(CropPatch), nameof(CropPatch.ConstructorTranspiler)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Crop), nameof(StardewValley.Crop.getRandomLowGradeCropForThisSeason)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CropPatch), nameof(CropPatch.GetRandomLowGradeCropForThisSeasonPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), "cutWeed"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.CutWeedTranspiler)))
            );
        }
    }
}