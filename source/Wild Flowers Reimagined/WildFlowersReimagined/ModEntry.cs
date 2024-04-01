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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;

namespace WildFlowersReimagined
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        private const string modDataKey = "jpp.WildFlowersReimagined.flower";
        private const string saveDataKey = "jpp.WildFlowersReimagined.flower";

        
        /// <summary>
        /// The mod configuration from the player.
        /// </summary>
        private ModConfig Config;
        /// <summary>
        /// Patches made to the original terrainFeatures
        /// </summary>
        private readonly Dictionary<string,Dictionary<Vector2, (FlowerGrass flowerGrass, Grass originalGrass)>> patchMap = new();
        /// <summary>
        /// Helper class to get all the seeds from the object data
        /// </summary>
        private readonly SeedMap seedMap = new();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            // if we cannot find the config or the mod is disable exit early
            if (this.Config == null || ! this.Config.ModEnabled) {
                this.Config ??= new ModConfig
                    {
                        ModEnabled = false
                    };
                return;
            }

            I18n.Init(helper.Translation);

            helper.Events.GameLoop.DayStarted += this.OnDayStart;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;

            this.Monitor.LogOnce("Mod enabled and ready", LogLevel.Debug);

        }
        /*********
        ** Private methods
        *********/

        /// <summary>
        /// On Game Launch event: Adds the Generic Mod Config menu entry
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        private void OnGameLaunch(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                this.Monitor.Log("Generic Config Menu not found");
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
        }

        /// <summary>
        /// On SaveLoaded event: Reconstruct the patchMap from the saved data
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            var savedData = this.Helper.Data.ReadSaveData<SaveData>(saveDataKey);
            if (savedData == null)
            {
                this.Monitor.Log("Saved data not found, continuing with a blank state", LogLevel.Warn);
                return;
            }

            var locations = GetValidLocations();
            var locationDict = GetValidLocations().ToDictionary(x => x.NameOrUniqueName);

            foreach ( var (location, dataList) in savedData.PatchMapData ) {
                var localPatchMap = GetLocationPatchMap(location);
                if (!locationDict.TryGetValue(location, out var gameLocation))
                {
                    this.Monitor.Log($"Location {location} not found in the game valid locations, skipping it", LogLevel.Warn);
                } 
                else
                {
                    foreach ( var entry in dataList )
                    {
                        var key = new Vector2(entry.Vector2X, entry.Vector2Y);
                        if (!gameLocation.terrainFeatures.TryGetValue(key, out var terrainFeature) && terrainFeature is not Grass && terrainFeature.modData.ContainsKey(modDataKey))
                        {
                            this.Monitor.Log($"{location}:{key} is not valid, skipping", LogLevel.Warn);
                        }
                        else
                        {
                            var originalGrass = terrainFeature as Grass;
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
        private static List<GameLocation> GetValidLocations()
        {
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
            // Always restore the save data
            foreach (var location in GetValidLocations())
            {
                // replace back the grass
                var localPatchMap = GetLocationPatchMap(location.NameOrUniqueName);
                foreach (var (key, grassTuple) in localPatchMap)
                {
                    // check that the tile is a valid FlowerGrass still
                    if (!location.terrainFeatures.TryGetValue(key, out TerrainFeature terrainFeature) && terrainFeature is not FlowerGrass)
                    {
                        // either deleted or no longer a Flower grass
                        localPatchMap.Remove(key);
                    }
                    else
                    {
                        location.terrainFeatures[key] = grassTuple.originalGrass;

                        // prune tiles without flowers
                        if (grassTuple.flowerGrass.Crop == null)
                        {
                            localPatchMap.Remove(key);
                        }
                    }
                }
            }

            // todo?: remove locations not on valid?

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
            seedMap.Init();

            // Get all locations where flowers may spawn
            var validLocations = GetValidLocations();
            // If there is not a valid location exit
            if (validLocations.Count < 1)
            {
                this.Monitor.LogOnce("Unable to find a valid location, unable to proceed", LogLevel.Error);
                return;
            }

            foreach (var location in validLocations)
            {
                var locationSeason = location.GetSeason();
                var localPatchMap = GetLocationPatchMap(location.NameOrUniqueName);
                var localFlowers = Game1.objectData.Where(p => p.Value.Category == StardewValley.Object.flowersCategory).SelectMany(p => seedMap.GetSeedsForLocation(p.Key, location)).ToList();
                if (localFlowers.Count == 0)
                {
                    this.Monitor.LogOnce($"{location.Name}: {locationSeason} has no flowers available, skipping", LogLevel.Warn);
                    return;
                }
                else
                {
                    this.Monitor.LogOnce($"{location.Name}: {locationSeason} has {localFlowers.Count} flowers");
                }
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
                        if (localFlowers.Contains(flowerGrass.Crop.netSeedIndex.Value))
                        {
                            location.terrainFeatures[key] = flowerGrass;
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
                        var chance = Game1.random.NextDouble();
                        // this.Monitor.Log($"{key} {chance}", LogLevel.Info);
                        if (chance <= this.Config.WildflowerGrowChance)
                        // if (chance <= this.Config.WildflowerGrowChance && !value.modData.ContainsKey(modDataKey))
                        {

                            var choice = Game1.random.ChooseFrom(localFlowers);
                            
                            var seed = Crop.getRandomFlowerSeedForThisSeason(locationSeason);

                            // string dm = "ok";
                            // var okPlant = location.CanPlantSeedsHere(seed, (int)key.X, (int)key.Y, false, out dm);

                            // var crop = new Crop(seed, (int)key.X, (int)key.Y, location);
                            var crop = new Crop(choice, (int)key.X, (int)key.Y, location);

                            crop.growCompletely();
                            crop.newDay(0);

                            var flowerGrass = new FlowerGrass(grassValue.grassType.Value, grassValue.numberOfWeeds.Value, crop, this.Config.FlowerGrassConfig);

                            location.terrainFeatures[key] = flowerGrass;
                            grassValue.modData[modDataKey] = "FG replaced";

                            localPatchMap[key] = (flowerGrass, grassValue);
                        }
                    }
                }
            }
        }        
    }
}