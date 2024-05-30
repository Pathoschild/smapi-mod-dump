/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jpparajeles/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace WildFlowersReimagined
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        private const string modDataKey = "jpp.WildFlowersReimagined.flower";
        private const string saveDataKey = "jpp.WildFlowersReimagined.flower";
        
        private const bool debugFlag = true;

        /// <summary>
        /// The mod configuration from the player.
        /// </summary>
        private ModConfig Config;
        /// <summary>
        /// ModConfig, to add options later down the line
        /// </summary>
        private IGenericModConfigMenuApi? configMenu;
        /// <summary>
        /// Flag to not initialize the flower config more than once
        /// </summary>
        private bool flowerConfigEnabled = false;

        /// <summary>
        /// Patches made to the original terrainFeatures
        /// </summary>
        private readonly Dictionary<string, Dictionary<Vector2, (FlowerGrass flowerGrass, Grass originalGrass)>> patchMap = new();
        /// <summary>
        /// Helper class to get all the seeds from the object data
        /// </summary>
        private readonly SeedMap seedMap = new();

        private readonly Random localRNG = new Random(Guid.NewGuid().GetHashCode());

        private static FlowerGrassConfig? configMirrorFlowerGrass = null; 





        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            // if we cannot find the config or the mod is disable exit early
            if (this.Config == null || !this.Config.ModEnabled)
            {
                this.Config ??= new ModConfig
                {
                    ModEnabled = false
                };
                return;
            }

            configMirrorFlowerGrass = Config.FlowerGrassConfig;

            I18n.Init(helper.Translation);

            helper.Events.GameLoop.DayStarted += this.OnDayStart;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;

            //dbg
            if (debugFlag)
            {
                helper.Events.Input.ButtonPressed += this.DbgButtonPressed;
            }


            this.Monitor.LogOnce("Mod enabled and ready", LogLevel.Debug);

        }

        public static FlowerGrassConfig ConfigLoadedFlowerConfig()
        {
            // This case should almost never happen
            if (configMirrorFlowerGrass == null)
            {
                return new FlowerGrassConfig();
            }
            return configMirrorFlowerGrass;
        }

        /*********
        ** Private methods
        *********/
        
        private void DbgButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.K)
            {
                var currPos = e.Cursor.Tile;
                var current = Game1.currentLocation;

                current.terrainFeatures.TryGetValue(currPos, out var terrainFeature);

                var sts = "";

                foreach (var loc in Game1.locations)
                {
                    if (loc == current)
                    {
                        sts = loc.IsActiveLocation().ToString();
                        break;
                    }
                }

                this.Monitor.Log($"{current}:{sts}:{currPos} has {terrainFeature}", LogLevel.Info);


                if (terrainFeature is FlowerGrass fgT)
                {
                    this.Monitor.Log(fgT.ToDebugString(), LogLevel.Info);
                    if (fgT.Crop != null)
                    {
                        fgT.Crop.updateDrawMath(fgT.Tile);
                    }
                }
            }
        }




        /// <summary>
        /// On Game Launch event: Adds the Generic Mod Config menu entry
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        private void OnGameLaunch(object? sender, GameLaunchedEventArgs e)
        {

            // get spacecore
            var spacecore = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            if (spacecore == null)
            {
                this.Monitor.Log("SpaceCore not found, multiplayer will not work as expected", LogLevel.Warn);
            }
            else
            {
                spacecore.RegisterSerializerType(typeof(FlowerGrass));
                this.Monitor.Log("SpaceCore Registration OK", LogLevel.Info);
            }

            // get Generic Mod Config Menu's API (if it's installed)
            configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                this.Monitor.Log("Generic Config Menu not found", LogLevel.Info);
                return;
            }

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: I18n.Config_ModEnable_Key,
                tooltip: I18n.Config_ModEnable_Tooltip,
                getValue: () => this.Config.ModEnabled,
                setValue: value => this.Config.ModEnabled = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: I18n.Config_HarvestByScythe_Key,
                tooltip: I18n.Config_HarvestByScythe_Tooltip,
                getValue: () => this.Config.FlowerGrassConfig.UseScythe,
                setValue: value => this.Config.FlowerGrassConfig.UseScythe = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: I18n.Config_GrowChance_Key,
                tooltip: I18n.Config_GrowChance_Tooltip,
                min: 0.0f,
                max: 1.0f,
                formatValue: number => number.ToString("0.000%"),
                getValue: () => this.Config.WildflowerGrowChance,
                setValue: value => this.Config.WildflowerGrowChance = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: I18n.Config_PreserveFlowersOnProbability0_Key,
                tooltip: I18n.Config_PreserveFlowersOnProbability0_Tooltip,
                getValue: () => this.Config.PreserveFlowersOnProbability0,
                setValue: value => this.Config.PreserveFlowersOnProbability0 = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: I18n.Config_AllLocations_Key,
                tooltip: I18n.Config_AllLocations_Tooltip,
                getValue: () => this.Config.CheckAllLocations,
                setValue: value => this.Config.CheckAllLocations = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: I18n.Config_KeepRegrowFlower_Key,
                tooltip: I18n.Config_KeepRegrowFlower_Tooltip,
                getValue: () => this.Config.FlowerGrassConfig.KeepRegrowFlower,
                setValue: value => this.Config.FlowerGrassConfig.KeepRegrowFlower = value
            );
        }

        /// <summary>
        /// Adds the flower config section of the config map.
        /// </summary>
        private void AddFlowerConfig()
        {
            if (!Config.ModEnabled || configMenu == null)
            {
                return;
            }

            // load the decoder
            string[] flowerProbabilityLabels =
            {
                I18n.Config_FlowerProbability_0(),
                I18n.Config_FlowerProbability_1(),
                I18n.Config_FlowerProbability_2(),
                I18n.Config_FlowerProbability_3(),
                I18n.Config_FlowerProbability_4(),
                I18n.Config_FlowerProbability_5(),

            };        

            var flowerProbabilityDecoder = flowerProbabilityLabels.Select((element, index) => (element, index)).ToDictionary(p => p.element, p => p.index);


            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "flower_config",
                text: I18n.Config_FlowerPage_Link,
                tooltip: I18n.Config_FlowerPage_Tooltip
            );
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "flower_config",
                pageTitle: I18n.Config_FlowerPage_Key
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: I18n.Config_FlowerPage_Description
            );


            // var flowers = Game1.objectData.Where(p => p.Value.Category == StardewValley.Object.flowersCategory).ToList();
            var flowers = seedMap.GetFlowerConfigMap();
            
            foreach (var (flowerInfo, SeedListData) in flowers)
            {
                var flowerData = flowerInfo.GetParsedData();
                if (SeedListData.Count < 1)
                {
                    this.Monitor.Log($"{flowerData.InternalName}:{flowerData.DisplayName} has no seed data", LogLevel.Warn);
                    continue;
                }
                var item = flowerInfo.CreateItem(0);

                configMenu.AddComplexOption(
                    mod: this.ModManifest,
                    name: () => flowerData.DisplayName,
                    tooltip: () => $"{flowerData.ItemId}:{flowerData.InternalName}",
                    draw: (sb, v) =>
                    {

                        item.drawInMenu(sb, v, 1.0f);
                    }
                );
                // we cannot gargantee that there is only one seed per flower
                foreach (var seedInfo in SeedListData)
                {
                    var seedData = seedInfo.GetParsedData();
                    configMenu.AddTextOption(
                        mod: this.ModManifest,
                        getValue: () => flowerProbabilityLabels[this.Config.FlowerProbabilityMap.GetValueOrDefault(seedData.ItemId, 3)],
                        setValue: (v) => this.Config.FlowerProbabilityMap[seedData.ItemId] = flowerProbabilityDecoder.GetValueOrDefault(v, 3),
                        name: () => seedData.DisplayName,
                        tooltip: () => $"{seedData.ItemId}:{seedData.InternalName}",
                        allowedValues: flowerProbabilityLabels
                    );
                }
            }
        }

        /// <summary>
        /// On SaveLoaded event: Reconstruct the patchMap from the saved data
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // if this is not the main player, don't try to load the data
            if (! Context.IsMainPlayer)
            {
                return;
            }

            var savedData = this.Helper.Data.ReadSaveData<SaveData>(saveDataKey);
            if (savedData == null)
            {
                this.Monitor.Log("Saved data not found, continuing with a blank state", LogLevel.Warn);
                return;
            }
            // clear the any saved data
            this.patchMap.Clear();

            var locations = GetValidLocations();
            var locationDict = GetValidLocations().ToDictionary(x => x.NameOrUniqueName);

            foreach (var (location, dataList) in savedData.PatchMapData)
            {
                var localPatchMap = GetLocationPatchMap(location);
                if (!locationDict.TryGetValue(location, out var gameLocation) || gameLocation == null)
                {
                    this.Monitor.Log($"Location {location} not found in the game valid locations, skipping it", LogLevel.Warn);
                }
                else
                {
                    foreach (var entry in dataList)
                    {
                        var key = new Vector2(entry.Vector2X, entry.Vector2Y);
                        try
                        {
                            if (!gameLocation.terrainFeatures.TryGetValue(key, out var terrainFeature) || terrainFeature == null || terrainFeature is not Grass)
                            {
                                this.Monitor.Log($"{location}:{key} is not valid, skipping", LogLevel.Warn);
                            }
                            else
                            {
                                var originalGrass = terrainFeature as Grass;
                                if (originalGrass == null)
                                {
                                    this.Monitor.Log("Unexpected null for original grass after is check");
                                    continue;
                                }
                                var crop = new Crop(entry.SeedIndex, entry.Vector2X, entry.Vector2Y, gameLocation);
                                crop.growCompletely();
                                crop.phaseToShow.Value = entry.PhaseToShow;
                                crop.currentPhase.Value = entry.CurrentPhase;
                                crop.tintColor.Value = new Color(entry.TintColorR, entry.TintColorG, entry.TintColorB, entry.TintColorA);
                                crop.dead.Value = entry.Dead;

                                var flowerGrass = new FlowerGrass(originalGrass.grassType.Value, originalGrass.numberOfWeeds.Value, crop, this.Config.FlowerGrassConfig);
                                localPatchMap[key] = (flowerGrass, originalGrass);

                            }
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"Unexpected exception {ex.Message} skipping {key}", LogLevel.Error);
                            this.Monitor.LogOnce($"{ex.StackTrace} \n {ex.Source}", LogLevel.Error);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Returns the Vector -> (FlowerGrass, Grass) entry for this location.
        /// This utility is to make sure the location is added to the patch map
        /// </summary>
        /// <param name="locationUniqueKey">Name of the location</param>
        /// <returns>Vector -> (FlowerGrass, Grass)</returns>
        private Dictionary<Vector2, (FlowerGrass flowerGrass, Grass originalGrass)> GetLocationPatchMap(string locationUniqueKey)
        {

            if (!this.patchMap.TryGetValue(locationUniqueKey, out Dictionary<Vector2, (FlowerGrass flowerGrass, Grass originalGrass)> localPatchMap))
            {
                localPatchMap = new Dictionary<Vector2, (FlowerGrass flowerGrass, Grass originalGrass)>();
                this.patchMap[locationUniqueKey] = localPatchMap;
            }
            return localPatchMap;
        }

        /// <summary>
        /// Get a list of the locations that can have grass
        /// </summary>
        /// <returns>List of locations that can have grass</returns>
        private List<GameLocation> GetValidLocations()
        {
            if (this.Config.CheckAllLocations)
            {
                return Game1.locations.ToList();
            }
            var validLocations = new List<GameLocation>();
            foreach (var gameLocation in Game1.locations)
            {
                if (gameLocation.IsFarm)
                {
                    validLocations.Add(gameLocation);
                }
                // greenhouses? maybe a config later
                if (gameLocation.IsGreenhouse)
                {
                    validLocations.Add(gameLocation);
                }
            }
            return validLocations.Distinct().ToList();
        }

        /// <summary>
        /// On Saving: Keep the save file vanilla by replacing the FlowerGrass with normal grass and save the patchMap to be used on the next on SaveLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            // Always restore the save data
            var validLocations = GetValidLocations();
            foreach (var location in validLocations)
            {
                // replace back the grass
                var localPatchMap = GetLocationPatchMap(location.NameOrUniqueName);
                foreach (var (key, grassTuple) in localPatchMap)
                {
                    // check that the tile is a valid FlowerGrass still
                    if (!location.terrainFeatures.TryGetValue(key, out TerrainFeature terrainFeature) || terrainFeature is not FlowerGrass)
                    {
                        // either deleted or no longer a Flower grass
                        localPatchMap.Remove(key);
                    }
                    else
                    {
                        // location.terrainFeatures[key] = grassTuple.originalGrass;
                        location.terrainFeatures.Remove(key);
                        location.terrainFeatures.Add(key, grassTuple.originalGrass);

                        // prune tiles without flowers
                        if (grassTuple.flowerGrass.Crop == null)
                        {
                            localPatchMap.Remove(key);
                        }
                    }
                }
            }

            // Failsafe: Remove any and all Flower grass remaining as they have been lost from the patch map 
            var countInvalidLocations = 0;
            var countInvalidTiles = 0;
            foreach (var location in Game1.locations)
            {
                var lostTiles = location.terrainFeatures.Pairs.Where(p => p.Value is FlowerGrass).Select(p => p.Key).ToList();
                if (lostTiles.Count > 0)
                {

                    countInvalidLocations++;
                    countInvalidTiles += lostTiles.Count;

                    var locationName = location.NameOrUniqueName;
                    this.Monitor.Log($"Location {locationName} had {lostTiles.Count} lost tiles. Starting Save Repair process", LogLevel.Warn);
                    if (!validLocations.Contains(location))
                    {
                        this.Monitor.Log("Grass found on invalid location");
                    }
                    if (!patchMap.ContainsKey(locationName))
                    {
                        this.Monitor.Log("Location not found on the patch map");
                    }
                    else
                    {
                        // I don't expect this to ever run, but just in case
                        this.Monitor.Log("Location found on the patch map, deleting data");
                        this.patchMap.Remove(locationName);
                    }
                    foreach (var key in lostTiles)
                    {
                        location.terrainFeatures.Remove(key);
                    }
                    this.Monitor.Log("Save data for the repair should be fixed now");
                }
            }



            if (!this.Config.ModEnabled)
            {
                this.Monitor.Log("Mod disabled", LogLevel.Debug);
                return;
            }

            // save the mod data next
            var saveData = new SaveData
            {
                PatchMapData = patchMap.ToDictionary(pm => pm.Key, pm => pm.Value.Select(pme => new SaveDataItem
                {
                    Vector2X = (int)pme.Key.X,
                    Vector2Y = (int)pme.Key.Y,
                    PhaseToShow = pme.Value.flowerGrass.Crop.phaseToShow.Value,
                    CurrentPhase = pme.Value.flowerGrass.Crop.currentPhase.Value,
                    SeedIndex = pme.Value.flowerGrass.Crop.netSeedIndex.Value,
                    TintColorR = pme.Value.flowerGrass.Crop.tintColor.Value.R,
                    TintColorG = pme.Value.flowerGrass.Crop.tintColor.Value.G,
                    TintColorB = pme.Value.flowerGrass.Crop.tintColor.Value.B,
                    TintColorA = pme.Value.flowerGrass.Crop.tintColor.Value.A,
                    Dead = pme.Value.flowerGrass.Crop.dead.Value,
                }).Where(sdi => sdi.SeedIndex != null).ToList()),
            };
            this.Helper.Data.WriteSaveData(saveDataKey, saveData);
        }


        /// <summary>
        /// On Day Start event: Main part of the mod
        /// For all valid locations, get the TerrainFeatures and on luck of the draw replace the grass with a Flower grass with a seed of this season.
        /// Grow the flower instantly and preserve the original grass
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDayStart(object? sender, DayStartedEventArgs e)
        {
            if (!this.Config.ModEnabled)
            {
                this.Monitor.Log("Mod disabled", LogLevel.Debug);
                return;
            }

            //Init the seedmap
            seedMap.Init(this.Monitor);
            if (!this.flowerConfigEnabled)
            {
                AddFlowerConfig();
                flowerConfigEnabled = true;
            }



            // Get all locations where flowers may spawn
            var validLocations = GetValidLocations();
            // If there is not a valid location exit
            if (validLocations.Count < 1)
            {
                this.Monitor.LogOnce("Unable to find a valid location, unable to proceed", LogLevel.Error);
                return;
            }

            // if this is not the main player, skip adding data to prevent a crash
            if (!Context.IsMainPlayer)
            {
                foreach (var location in validLocations)
                {
                    foreach(var (vector, terrainFeature) in location.terrainFeatures.Pairs)
                    {
                        if (terrainFeature is FlowerGrass flowerGrass)
                        {
                            if (!vector.Equals(flowerGrass.Tile))
                            {
                                flowerGrass.Tile = vector;
                                flowerGrass.Location = location;
                                flowerGrass.dayUpdate();
                            }   
                        }
                    }
                }
                return;
            }

            foreach (var location in validLocations)
            {
                var locationSeason = location.GetSeason();
                var localPatchMap = GetLocationPatchMap(location.NameOrUniqueName);
                var localFlowers = seedMap.GetSeedsForLocation(location, Config.FlowerProbabilityMap);
                var localFlowerCandidates = seedMap.GetSeedCandidatesForLocation(location);
                if (localFlowers.Count == 0)
                {
                    this.Monitor.LogOnce($"{location.Name}: {locationSeason} has no flowers available, skipping", LogLevel.Warn);
                    if (localFlowerCandidates.Count > 0)
                    {
                        this.Monitor.LogOnce("Current settings prevent flowers from appearing", LogLevel.Warn);
                    }
                    return;
                }
                else
                {
                    this.Monitor.LogOnce($"{location.Name}: {locationSeason} has {localFlowers.Count} flowers");
                }
                var replacements = new List<(Vector2 key, FlowerGrass replacement)>();
                foreach (var pair in location.terrainFeatures.Pairs.Where(p => p.Value is Grass))
                {
                    var key = pair.Key;
                    var value = pair.Value;

                    if (pair.Value is not Grass grassValue)
                    {
                        this.Monitor.Log("Unexpected code path reached, Value is Grass but as Grass is null.Skipping tile", LogLevel.Warn);
                        continue;
                    }

                    // if we have data already let's load the data
                    if (grassValue.modData.ContainsKey(modDataKey) && localPatchMap.ContainsKey(key))
                    {
                        var (flowerGrass, originalGrass) = localPatchMap[key];
                        // keep the flower if it's in the probability list or PreserveFlowersOnProbability0 and it's in the candidate list
                        if (localFlowers.Contains(flowerGrass.Crop.netSeedIndex.Value) || (Config.PreserveFlowersOnProbability0 && localFlowerCandidates.Contains(flowerGrass.Crop.netSeedIndex.Value)))
                        {
                            // location.terrainFeatures[key] = flowerGrass;
                            replacements.Add((key, flowerGrass));
                            /*
                            if (!flowerGrass.Tile.Equals(key))
                            {
                                flowerGrass.Tile = key;
                            }
                            */
                        }
                        else
                        {
                            grassValue.modData.Remove(modDataKey);
                            localPatchMap.Remove(key);
                        }
                    }
                    // if not let's do random chance
                    else
                    {
                        var chance = localRNG.NextDouble();
                        // this.Monitor.Log($"{key} {chance}", LogLevel.Info);
                        if (chance <= this.Config.WildflowerGrowChance)
                        {

                            var choice = localRNG.ChooseFrom(localFlowers);

                            var seed = Crop.getRandomFlowerSeedForThisSeason(locationSeason);

                            // string dm = "ok";
                            // var okPlant = location.CanPlantSeedsHere(seed, (int)key.X, (int)key.Y, false, out dm);

                            // var crop = new Crop(seed, (int)key.X, (int)key.Y, location);
                            var crop = new Crop(choice, (int)key.X, (int)key.Y, location);

                            crop.growCompletely();
                            crop.newDay(0);

                            var flowerGrass = new FlowerGrass(grassValue.grassType.Value, grassValue.numberOfWeeds.Value, crop, this.Config.FlowerGrassConfig);

                            // location.terrainFeatures.Remove(key);
                            // location.terrainFeatures.Add(key, flowerGrass);
                            //location.terrainFeatures[key] = flowerGrass;
                            replacements.Add((key, flowerGrass));
                            grassValue.modData[modDataKey] = "FG replaced";

                            localPatchMap[key] = (flowerGrass, grassValue);
                        }
                    }
                }
                foreach (var (key, flowerGrass) in replacements)
                {
                    location.terrainFeatures.Remove(key);
                    location.terrainFeatures.Add(key, flowerGrass);
                }
            }
        }
    }
}