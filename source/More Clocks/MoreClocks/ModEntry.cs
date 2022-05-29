/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/p-holodynski/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using MoreClocks.IridiumClock;
using MoreClocks.RadioactiveClock;
using Netcode;
using StardewValley.TerrainFeatures;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using StardewValley.Objects;
using GenericModConfigMenu;

namespace MoreClocks
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        /*********
        ** Fields
        *********/
        /// <summary>The in-game event detected on the last update tick.</summary>
        private Texture2D iridiumClockTexture;
        private Texture2D radioactiveClockTexture;
        private bool isGoldClockBuilt = false;
        private bool isIridiumClockBuilt = false;
        private bool isIridiumClockTriggered = false;
        private bool isRadioactiveClockBuilt = false;
        private bool isRadioactiveClockTriggered = false;
        private Random randomvalue;
        private float originalDifficultyModifier;
        private readonly float EPSILON = 0.01f;
        private List<string> machineNames;
        private uint machineUpdateInterval = 10;
        private float machineTime = 100f;
        private ModConfig Config;

        /*********
        ** Accessors
        *********/
        public static Mod Instance;
        

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.World.BuildingListChanged += this.OnBuildingListChanged;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.SaveCreated += this.OnSaveCreated;

            try
            {
                this.iridiumClockTexture = this.Helper.Content.Load<Texture2D>($"assets/{this.Config.IridiumClockCustomTexture}.png");
            }
            catch
            {
                this.iridiumClockTexture = this.Helper.Content.Load<Texture2D>("assets/IridiumClock.png");
            }
            try
            {
                this.radioactiveClockTexture = this.Helper.Content.Load<Texture2D>($"assets/{this.Config.RadioactiveClockCustomTexture}.png");
            }
            catch
            {
                this.radioactiveClockTexture = this.Helper.Content.Load<Texture2D>("assets/RadioactiveClock.png");
            }
            this.randomvalue = new Random();
            this.machineNames = new List<string> {
                "Bee House", "Cask", "Charcoal Kiln", "Cheese Press", "Crystalarium",
                "Furnace", "Incubator", "Keg", "Lightning Rod", "Loom", "Mayonnaise Machine", "Oil Maker",
                "Preserves Jar", "Recycling Machine", "Seed Maker", "Slime Egg-Press", "Slime Incubator",
                "Tapper", "Worm Bin"};

            this.machineTime = this.machineTime - this.Config.MachineSpeedUpSpeedValue;
        }

        private void OnSaveCreated(object sender, SaveCreatedEventArgs e)
        {
            this.originalDifficultyModifier = Game1.player.difficultyModifier;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.originalDifficultyModifier = Game1.player.difficultyModifier;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.SetTitleScreenOnlyForNextOptions(mod: this.ModManifest, true);

            // add General Section Title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "General Options"
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Notifications.",
                tooltip: () => "Enable the notifications if any Clock effect occured overnight.",
                getValue: () => this.Config.clockNotificationsEnabled,
                setValue: value => this.Config.clockNotificationsEnabled = value
            );

            // add Gold Clock Section Title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Gold Clock Options"
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Profit Margin.",
                tooltip: () => "Enable the Profit Margin effect.",
                getValue: () => this.Config.ProfitMarginEnabled,
                setValue: value => this.Config.ProfitMarginEnabled = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Profit Margin Value.",
                tooltip: () => "Percentage of Profit Margin: 0.25 means 25% more profits, 1 means 100% more(double) profits",
                getValue: () => this.Config.ProfitMarginValue,
                setValue: value => this.Config.ProfitMarginValue = value,
                min: 0f,
                max: 1f,
                interval: 0.25f
            );
            // add Iridium Clock Section Title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Iridium Clock Options"
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Plant Any Season.",
                tooltip: () => "Enable the Plant Any Season effect.",
                getValue: () => this.Config.PlantAnySeasonEnabled,
                setValue: value => this.Config.PlantAnySeasonEnabled = value
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Machine Speed Up.",
                tooltip: () => "Enable the Machine Speed Up effect.",
                getValue: () => this.Config.MachineSpeedUpEnabled,
                setValue: value => this.Config.MachineSpeedUpEnabled = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Machine Speed Up chance.",
                tooltip: () => "Percentage that the Machine Speed Up will trigger overnight. From 0% to 100%.",
                getValue: () => this.Config.MachineSpeedUpChanceValue,
                setValue: value => this.Config.MachineSpeedUpChanceValue = value,
                min: 0,
                max: 100,
                interval: 1
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Machine Speed Up value.",
                tooltip: () => "The value of the Machine Speed Up effect. From 0% to 100%.",
                getValue: () => this.Config.MachineSpeedUpSpeedValue,
                setValue: value => this.Config.MachineSpeedUpSpeedValue = value,
                min: 0,
                max: 100,
                interval: 1
            );
            // add Radioactive Clock Section Title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Radioactive Clock Options"
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Crop Grow overnight.",
                tooltip: () => "Enable the Crop Grow effect.",
                getValue: () => this.Config.CropGrowEnabled,
                setValue: value => this.Config.CropGrowEnabled = value
            );
            // add some config options
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Crop Grow Method.",
                tooltip: () => "Set the Crop Grow method. Completely means crop will become fully grown (ready to harvest) and Sequentially means crop will enter its next growth stage instead.",
                getValue: () => this.Config.CropGrowMethod,
                setValue: value => this.Config.CropGrowMethod = value,
                allowedValues: new string[] { "Completely", "Sequentially" }
            );
            // add some config options
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Crop Grow Area.",
                tooltip: () => "Set the Crop Grow effect to be trigger for each crop individualy or for all the crops on the farm.",
                getValue: () => this.Config.CropGrowArea,
                setValue: value => this.Config.CropGrowArea = value,
                allowedValues: new string[] { "Individual", "Farm" }
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Crop Grow overnight chance.",
                tooltip: () => "Percentage that the Crop Grow will trigger overnight. From 0% to 100%.",
                getValue: () => this.Config.CropGrowChanceValue,
                setValue: value => this.Config.CropGrowChanceValue = value,
                min: 0,
                max: 100,
                interval: 1
            );
            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mutate to Giant Crop.",
                tooltip: () => "Enable the Crop Mutate to Giant Crop effect.",
                getValue: () => this.Config.CropMutateToGiantEnabled,
                setValue: value => this.Config.CropMutateToGiantEnabled = value
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Mutate to Giant Crop chance.",
                tooltip: () => "Percentage that the Crop Mutate to Giant Crop will trigger overnight. From 0% to 100%.",
                getValue: () => this.Config.CropMutateToGiantChanceValue,
                setValue: value => this.Config.CropMutateToGiantChanceValue = value,
                min: 0,
                max: 100,
                interval: 1
            );
        }

        /*********
        ** Private methods
        *********/

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            NetCollection<Building> buildings = ((BuildableGameLocation)Game1.getFarm()).buildings;
            foreach (Building building in buildings)
            {
                if (building.buildingType.ToString() == "Gold Clock")
                {
                    this.isGoldClockBuilt = true;
                    if (this.Config.ProfitMarginEnabled == true)
                    {
                        Game1.player.difficultyModifier = 1f + this.Config.ProfitMarginValue;
                    }
                }
                if (building.buildingType.ToString() == "Iridium Clock")
                {
                    if (this.Config.PlantAnySeasonEnabled == true)
                    {
                        Game1.getFarm().IsGreenhouse = true;
                    }
                    this.isIridiumClockBuilt = true;
                }
                if (building.buildingType.ToString() == "Radioactive Clock")
                {
                    this.isRadioactiveClockBuilt = true;
                }
            }
            if (this.isIridiumClockTriggered == true)
            {
                if (this.Config.clockNotificationsEnabled == true)
                {
                    Game1.hudMessages.Add(new HUDMessage("Iridium Clock has speed up the machines overnight", 2));
                }
                this.isIridiumClockTriggered = false;
            }
            if (this.isRadioactiveClockTriggered == true)
            {
                if (this.Config.clockNotificationsEnabled == true)
                {
                    Game1.hudMessages.Add(new HUDMessage("Radioactive Clock has mutated some crops overnight", 2));
                }
                this.isRadioactiveClockTriggered = false;
            }
        }

        // Triggers when a building is added/removed on the farm
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            foreach (Building building in e.Added)
            {
                if (building.buildingType.ToString() == "Gold Clock")
                {
                    this.isGoldClockBuilt = true;
                    if (this.Config.ProfitMarginEnabled == true)
                    {
                        Game1.player.difficultyModifier = 1f + this.Config.ProfitMarginValue;
                    }
                }
                if (building.buildingType.ToString() == "Iridium Clock")
                {
                    if (this.Config.PlantAnySeasonEnabled == true)
                    {
                        Game1.getFarm().IsGreenhouse = true;
                    }
                    this.isIridiumClockBuilt = true;
                }
                if (building.buildingType.ToString() == "Radioactive Clock")
                {
                    this.isRadioactiveClockBuilt = true;
                }
            }
            foreach (Building building in e.Removed)
            {
                if (building.buildingType.ToString() == "Gold Clock")
                {
                    this.isGoldClockBuilt = false;
                    Game1.player.difficultyModifier = this.originalDifficultyModifier;
                }
                if (building.buildingType.ToString() == "Iridium Clock")
                {
                    Game1.getFarm().IsGreenhouse = false;
                    this.isIridiumClockBuilt = false;
                }
                if (building.buildingType.ToString() == "Radioactive Clock")
                {
                    this.isRadioactiveClockBuilt = false;
                }
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
        /// <summary>Raised before the game ends the current day. This happens before it starts setting up the next day and before <see cref="IGameLoopEvents.Saving"/>.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (this.isIridiumClockBuilt == true)
            {
                if (this.Config.MachineSpeedUpEnabled == true)
                {
                    var chanceToSpeedUpMachines = this.randomvalue.Next(0, 100);
                    if (chanceToSpeedUpMachines < this.Config.MachineSpeedUpChanceValue) // % chance to trigger
                    {
                        SpeedUpAllMachines();
                        this.isIridiumClockTriggered = true;
                    }
                } 
            }
            if (this.isRadioactiveClockBuilt == true) {
                GameLocation location = Game1.getFarm();
                if (this.Config.CropGrowEnabled == true)
                {
                    // Triggers for each crop individualy
                    if (this.Config.CropGrowArea == "Individual")
                    {
                        foreach (var pair in location.terrainFeatures.Pairs)
                        {
                            if (pair.Value is HoeDirt)
                            {
                                var dirt = pair.Value as HoeDirt;
                                // A state of 1 for dirt means it's watered.
                                if (dirt.crop != null && !dirt.crop.dead.Value && dirt.state.Value == HoeDirt.watered)
                                {
                                    var randomChance = this.randomvalue.Next(0, 100);
                                    if (randomChance < this.Config.CropGrowChanceValue) // % chance to trigger
                                    {
                                        if (dirt.crop.currentPhase.Value != dirt.crop.phaseDays.Count - 1)
                                        {
                                            if (this.Config.CropGrowMethod == "Completely")
                                            {
                                                dirt.crop.growCompletely();
                                            }
                                            if (this.Config.CropGrowMethod == "Sequentially")
                                            {
                                                //this.Monitor.Log($"crop phase: {dirt.crop.currentPhase.Get()} before", LogLevel.Debug);
                                                //increase crop's current phase by 1 and update draw math to display current crop phase
                                                dirt.crop.currentPhase.Set(dirt.crop.currentPhase.Get() + 1);
                                            }
                                            this.isRadioactiveClockTriggered = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Triggers for all crops on the farm
                    if (this.Config.CropGrowArea == "Farm")
                    {
                        var randomChance = this.randomvalue.Next(0, 100);
                        if (randomChance < this.Config.CropGrowChanceValue) // % chance to trigger
                        {
                            foreach (var pair in location.terrainFeatures.Pairs)
                            {
                                if (pair.Value is HoeDirt)
                                {
                                    var dirt = pair.Value as HoeDirt;
                                    // A state of 1 for dirt means it's watered.
                                    if (dirt.crop != null && !dirt.crop.dead.Value && dirt.state.Value == HoeDirt.watered)
                                    {
                                        if (dirt.crop.currentPhase.Value != dirt.crop.phaseDays.Count - 1)
                                        {
                                            if (this.Config.CropGrowMethod == "Completely")
                                            {
                                                dirt.crop.growCompletely();
                                            }
                                            if (this.Config.CropGrowMethod == "Sequentially")
                                            {
                                                //increase crop's current phase by 1 and update draw math to display current crop phase
                                                dirt.crop.currentPhase.Set(dirt.crop.currentPhase.Get() + 1);
                                            }
                                            this.isRadioactiveClockTriggered = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.Config.CropMutateToGiantEnabled == true)
                {
                    var chanceToTurnIntoGiantCrop = this.randomvalue.Next(0, 100);
                    if (chanceToTurnIntoGiantCrop < this.Config.CropMutateToGiantChanceValue) // % chance to trigger
                    {
                        MutateCrops(this.Config.CropMutateToGiantChanceValue, location);
                        this.isRadioactiveClockTriggered = true;
                    }
                }
            }
        }

        private void OnSaving(object sender, SavingEventArgs args)
        {
            Game1.player.difficultyModifier = this.originalDifficultyModifier;
        }

        // Checks if the crops are fully grown and can become a Giant Crop
        private void MutateCrops(int chance, GameLocation environment)
        {
            foreach (Tuple<Vector2, Crop> tuple in this.GetValidCrops(environment))
            {
                int xTile = (int)tuple.Item1.X;
                int yTile = (int)tuple.Item1.Y;

                Crop crop = tuple.Item2;

                double rand = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + xTile * 2000 +
                                      yTile).NextDouble();

                bool okCrop = true;
                if (crop.currentPhase.Value == crop.phaseDays.Count - 1 &&
                    (crop.indexOfHarvest.Value == 276 || crop.indexOfHarvest.Value == 190 || crop.indexOfHarvest.Value == 254) &&
                    rand < chance)
                {
                    for (int index1 = xTile - 1; index1 <= xTile + 1; ++index1)
                    {
                        for (int index2 = yTile - 1; index2 <= yTile + 1; ++index2)
                        {
                            Vector2 key = new Vector2(index1, index2);
                            if (!environment.terrainFeatures.ContainsKey(key) ||
                                !(environment.terrainFeatures[key] is HoeDirt) ||
                                (environment.terrainFeatures[key] as HoeDirt).crop == null ||
                                (environment.terrainFeatures[key] as HoeDirt).crop.indexOfHarvest !=
                                crop.indexOfHarvest)
                            {
                                okCrop = false;

                                break;
                            }
                        }

                        if (!okCrop)
                            break;
                    }

                    if (!okCrop)
                        continue;

                    for (int index1 = xTile - 1; index1 <= xTile + 1; ++index1)
                        for (int index2 = yTile - 1; index2 <= yTile + 1; ++index2)
                        {
                            var index3 = new Vector2(index1, index2);
                            (environment.terrainFeatures[index3] as HoeDirt).crop = null;
                        }
                    (environment as Farm).resourceClumps.Add(new GiantCrop(crop.indexOfHarvest.Value,
                        new Vector2(xTile - 1, yTile - 1)));
                }
            }
        }

        // Gets the list of valid crops on the farm
        private List<Tuple<Vector2, Crop>> GetValidCrops(GameLocation environment)
        {
            List<Tuple<Vector2, Crop>> validCrops = new List<Tuple<Vector2, Crop>>();
            foreach (var pair in environment.terrainFeatures.Pairs)
            {
                if (pair.Value is HoeDirt)
                {
                    var tile = pair.Value.currentTileLocation;
                    var dirt = pair.Value as HoeDirt;
                    var crop = dirt?.crop;
                    if (crop != null && !crop.dead.Value && dirt.state.Value == HoeDirt.watered)
                    {
                        validCrops.Add(Tuple.Create(tile, dirt.crop));
                    }
                }
            }
            return validCrops;
        }

        // Triggers when the in game Menu is opened
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu carp)
            {
                var blueprints = this.Helper.Reflection.GetField<List<BluePrint>>(carp, "blueprints").GetValue();
                blueprints.Add(new BluePrint("Iridium Clock"));
                blueprints.Add(new BluePrint("Radioactive Clock"));
            }
        }

        // Sweep through all the machines in the world and speeds them up.
        private void SpeedUpAllMachines()
        {
            IEnumerable<GameLocation> locations = GetLocations();
            foreach (string name in this.machineNames)
            {
                foreach (GameLocation loc in locations)
                {
                    Func<KeyValuePair<Vector2, StardewValley.Object>, bool> matchingName = p => p.Value.name == name;
                    foreach (KeyValuePair<Vector2, StardewValley.Object> pair in loc.objects.Pairs)
                    {
                        if (matchingName(pair))
                        {
                            var obj = pair.Value;
                            if (obj.MinutesUntilReady is <= 0 or 999999 || obj.Name == "Stone")
                                continue;
                            SpeedUpMachine(obj);
                        }
                    }
                }
            }
        }

        // Speeds Ups the machine.
        private void SpeedUpMachine(StardewValley.Object obj)
        {
            string text = (obj.MinutesUntilReady / 10).ToString();
            
            // If machine hasn't been configured yet.   
            if (obj is Cask c && obj.heldObject.Value != null)
            {
                float agingRate = 1f;
                switch (c.heldObject.Value.ParentSheetIndex)
                {
                    case 426:
                        agingRate = 4f;
                        break;
                    case 424:
                        agingRate = 4f;
                        break;
                    case 459:
                        agingRate = 2f;
                        break;
                    case 303:
                        agingRate = 1.66f;
                        break;
                    case 346:
                        agingRate = 2f;
                        break;
                }
                // Configure casks
                if (Math.Abs(this.machineTime - 100f) > EPSILON && (int)Math.Round(c.agingRate.Value * 1000) % 10 != 1)
                {
                    // By percentage
                    c.agingRate.Value = agingRate * 100 / this.machineTime;
                    c.agingRate.Value = (float)Math.Round(c.agingRate.Value, 2);
                    c.agingRate.Value += 0.001f;
                }
            }
            else if (obj.MinutesUntilReady > 0)
            {
                // Configure all machines other than casks
                if (Math.Abs(this.machineTime - 100f) > EPSILON)
                {
                    // By percentage
                    obj.MinutesUntilReady = Math.Max(((int)(obj.MinutesUntilReady * this.machineTime / 100 / 10)) * 10 - 2, 8);
                }
            }
        }

        /// Get all game locations.
        /// Copied with permission from Pathoschild
        public IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data\\Blueprints"))
            {
                return true;
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            asset.AsDictionary<string, string>().Data.Add("Iridium Clock", "337 100/3/2/-1/-1/-2/-1/null/Iridium Clock/All seeds are plantable regardless of the season. Chance to Speed Up machines overnight./Buildings/none/48/80/-1/null/Farm/10000000/true");
            asset.AsDictionary<string, string>().Data.Add("Radioactive Clock", "910 100/3/2/-1/-1/-2/-1/null/Radioactive Clock/Crops have a chance to fully grow overnight. Giant Crops spawn more often./Buildings/none/48/80/-1/null/Farm/10000000/true");
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings\\Iridium Clock"))
            {
                return true;
            }
            if (asset.AssetNameEquals("Buildings\\Radioactive Clock"))
            {
                return true;
            }
            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings\\Iridium Clock"))
            {
                return (T)(object)this.iridiumClockTexture;
            }
            if (asset.AssetNameEquals("Buildings\\Radioactive Clock"))
            {
                return (T)(object)this.radioactiveClockTexture;
            }
            return (T)(object)null;
        }
    }
}