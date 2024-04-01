/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IncreaseAnimalHouseMaxPopulation.Framework;
using IncreaseAnimalHouseMaxPopulation.Framework.Configs;
using Microsoft.Xna.Framework;
using Mizzion.Stardew.Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

using xTile;

namespace IncreaseAnimalHouseMaxPopulation
{
    public class ModEntry : Mod
    {
        private ModConfig _config;
        
        private IGenericModConfigMenuApi _cfgMenu;

        private PlayerData _data;

        public SButton RefreshConfig;

        public ITranslationHelper I18N;

        public Building CurrentHoveredBuilding;

        public Building CurrentHoveredBuildingDummy;

        public List<string> AnimalHouseBuildings = new List<string>
        {
            "Deluxe Barn",
            "Deluxe Coop"
        };

        public int Cost;

        public bool IsTestBuild = true;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            I18N = helper.Translation;
            
            
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.Display.RenderingHud += RenderingHud;
            helper.Events.Content.AssetRequested += AssetRequested;
            
            
            helper.ConsoleCommands.Add("pop_reset", "Deletes the save data for Increased Animal House Population.", ResetSave);
            DoSanityCheck();
        }

        //Event Methods
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {


            _cfgMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (_cfgMenu is null) return;

            //Register mod
            _cfgMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config)
            );

            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18N.Get("mod_name_text"),
                tooltip: null
            );

            //Main Settings

            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18N.Get("mod_main_settings_text"),
                tooltip: null
            );
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.MainSettings.EnableDebugMode,
                setValue: value => _config.MainSettings.EnableDebugMode = value,
                name: () => I18N.Get("setting_debug_text"),
                tooltip: () => I18N.Get("setting_debug_description")
                );
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.MainSettings.EnableHoverTip,
                setValue: value => _config.MainSettings.EnableHoverTip = value,
                name: () => I18N.Get("setting_hover_text"),
                tooltip: () => I18N.Get("setting_hover_description")
            );
            _cfgMenu.AddKeybind(
                mod: ModManifest,
                getValue: () => _config.MainSettings.RefreshConfigButton,
                setValue: value => _config.MainSettings.RefreshConfigButton = value,
                name: () => I18N.Get("setting_reload_config_button_text"),
                tooltip: () => I18N.Get("setting_reload_config_button_description")
            );

            //Building Settings
            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18N.Get("mod_building_settings_text"),
                tooltip: null
            );
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.MainSettings.EnableBuildingMapReplacements,
                setValue: value => _config.MainSettings.EnableBuildingMapReplacements = value,
                name: () => I18N.Get("setting_mapedit_text"),
                tooltip: () => I18N.Get("setting_mapedit_description")
            );
            
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.BuildingSettings.MaxBarnPopulation,
                setValue: value => _config.BuildingSettings.MaxBarnPopulation = value,
                name: () => I18N.Get("setting_max_barn_population_text"),
                tooltip: () => I18N.Get("setting_max_barn_population_description")
                );
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.BuildingSettings.MaxCoopPopulation,
                setValue: value => _config.BuildingSettings.MaxCoopPopulation = value,
                name: () => I18N.Get("setting_max_coop_population_text"),
                tooltip: () => I18N.Get("setting_max_coop_population_description")
                );
            _cfgMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.BuildingSettings.CostPerPopulationIncrease,
                setValue: value => _config.BuildingSettings.CostPerPopulationIncrease = value,
                name: () => I18N.Get("setting_cost_per_population_increase_text"),
                tooltip: () => I18N.Get("setting_cost_per_population_increase_description")
                );
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.BuildingSettings.AutoFeedExtraAnimals,
                setValue: value => _config.BuildingSettings.AutoFeedExtraAnimals = value,
                name: () => I18N.Get("setting_auto_feed_extra_animals_text"),
                tooltip: () => I18N.Get("setting_auto_feed_extra_animals_description")
                );

            //Cheat Settings
            _cfgMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18N.Get("mod_cheats_text"),
                tooltip: null
            );
            _cfgMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.Cheats.EnableFree,
                setValue: value => _config.Cheats.EnableFree = value,
                name: () => I18N.Get("setting_enable_free_food_text"),
                tooltip: () => I18N.Get("setting_enable_free_food_description")
                );

        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
                //CurrentHoveredBuilding = GetHoveredBuilding(_config.MainSettings.EnableDebugMode);
            }
            
            if (e.IsDown(RefreshConfig))
            {
                _config = Helper.ReadConfig<ModConfig>();
                DoSanityCheck();
                Monitor.Log("Reloaded the configuration file.", LogLevel.Debug);
            }
            

            if (e.IsDown(SButton.MouseLeft) && CurrentHoveredBuilding != null &&
                AnimalHouseBuildings.Contains(CurrentHoveredBuilding.buildingType.Value) &&
                !_data.Buildings.ContainsKey(CurrentHoveredBuilding.indoors.Value.uniqueName.Value) && _data.Buildings != null &&
                Game1.activeClickableMenu == null && Game1.player.CurrentItem == null)
            {
                int freeOrNot = ((!_config.Cheats.EnableFree) ? _config.BuildingSettings.CostPerPopulationIncrease : 0);
                //Lets calculate the difference between max and current max population
                int currentMaxOccupants = ((AnimalHouse)CurrentHoveredBuilding.indoors.Value).animalLimit.Value; //(AnimalHouse)this.CurrentHoveredBuilding.indoors
                Cost = (CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn")
                    ? ((_config.BuildingSettings.MaxBarnPopulation - currentMaxOccupants) * freeOrNot)
                    : ((_config.BuildingSettings.MaxCoopPopulation - currentMaxOccupants) * freeOrNot));

                CurrentHoveredBuildingDummy = CurrentHoveredBuilding;
                string question = I18N.Get("upgrade_question", new
                {
                    current_building = CurrentHoveredBuilding.buildingType.Value,
                    next_cost = Cost,
                    current_max_occupants = currentMaxOccupants,
                    config_max_occupants = CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn") ? _config.BuildingSettings.MaxBarnPopulation : _config.BuildingSettings.MaxCoopPopulation
                });
                Game1.currentLocation.createQuestionDialogue(question, Game1.currentLocation.createYesNoResponses(),
                    delegate (Farmer _, string answer)
                    {
                        if (answer == "Yes")
                        {
                            if (Game1.player.Money >= Cost)
                            {
                                Game1.player.Money -= Cost;
                                DoPopulationChange(CurrentHoveredBuildingDummy, _config.MainSettings.EnableDebugMode);
                            }
                            else
                            {
                                Game1.showRedMessage($"You don't have {Cost} gold.");
                            }
                        }
                    });
            }
        }


        private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (!_config.MainSettings.EnableBuildingMapReplacements)
                return;
            
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Coop3"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                   Map map = Helper.ModContent.Load<Map>("assets/Coop3.tmx");
                   editor.ExtendMap(minHeight: 10, minWidth: 46);
                   editor.PatchMap(map, patchMode: PatchMapMode.Replace);
                });
                
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Barn3"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    Map map = Helper.ModContent.Load<Map>("assets/Barn3.tmx");
                    editor.ExtendMap(minHeight: 14, minWidth: 50);
                    editor.PatchMap(map, patchMode: PatchMapMode.Replace);
                });

            }
        }


        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _data = Helper.Data.ReadSaveData<PlayerData>(Helper.ModRegistry.ModID) ?? new PlayerData();

            var dataFound = _data is null ? "_data couldn't be found" : "_data was either found or created";

            if(_config.MainSettings.EnableDebugMode)
                Log($"{dataFound}");
        }
        

        private void Saving(object sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData(Helper.ModRegistry.ModID, _data);
        }

        private void RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (CurrentHoveredBuilding != null && Game1.activeClickableMenu == null &&
                (_config.MainSettings.EnableDebugMode || _config.MainSettings.EnableHoverTip) && AnimalHouseBuildings.Any(
                    ab => CurrentHoveredBuilding.buildingType.Contains(ab) &&
                            CurrentHoveredBuilding.indoors.Value != null))
            {
                Translation tipText = I18N.Get("upgrade_tooltip_text", new
                {
                    current_building = CurrentHoveredBuilding.buildingType.Value,
                    max_animals = ((AnimalHouse) CurrentHoveredBuilding.indoors.Value).animalLimit
                });
                IClickableMenu.drawHoverText(Game1.spriteBatch, tipText, Game1.smallFont);
            }

            if (CurrentHoveredBuilding != null)
            {
                int p = (CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn")
                    ? _config.BuildingSettings.MaxBarnPopulation
                    : _config.BuildingSettings.MaxCoopPopulation);
                
                AnimalHouse obj = CurrentHoveredBuilding.indoors.Value as AnimalHouse;
                
                if ((obj == null || obj.animalLimit.Value != p) &&
                    CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe") &&
                    Game1.activeClickableMenu == null)
                {
                    Game1.mouseCursor = 4;
                }
            }
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && e.IsMultipleOf(4u))
            {
                CurrentHoveredBuilding = GetHoveredBuilding(_config.MainSettings.EnableDebugMode);
            }
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            if(_data != null)
                ModifyBuildings(_config.MainSettings.EnableDebugMode, doRestore: true);
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            _data = Helper.Data.ReadSaveData<PlayerData>(Helper.ModRegistry.ModID) ?? new PlayerData();
            var dataFound = _data is null ? "_data couldn't be found" : "_data was found or created";
            if (_data is null || !_data.Buildings.Any())
            {
                return;
            }

            if (_config.MainSettings.EnableDebugMode)
                Log(dataFound);
           
            ModifyBuildings(_config.MainSettings.EnableDebugMode);
        }

       
        //Custom Methods

        private bool ProperAnimalBuilding(Building b)
        {
            return b != null && Game1.activeClickableMenu == null && AnimalHouseBuildings.Any((string ab) =>
                CurrentHoveredBuilding.buildingType.Contains(ab) && CurrentHoveredBuilding.indoors.Value != null);
        }

        private Building GetHoveredBuilding(bool debugging = false)
        {
            Vector2 currentBuilding = debugging ? Helper.Input.GetCursorPosition().Tile : Game1.player.Tile +
                new Vector2(0f, -1f);

            return Game1.currentLocation != null ? Game1.currentLocation.getBuildingAt(currentBuilding) : null;
        }

        private void DoSanityCheck()
        {
            if (_config.BuildingSettings.MaxBarnPopulation <= 0)
            {
                _config.BuildingSettings.MaxBarnPopulation = 1;
                Helper.WriteConfig(_config);
                Monitor.Log("The configured MaxBarnPopulation wasn't higher than Zero, it has been set to 1",
                    LogLevel.Debug);
            }

            if (_config.BuildingSettings.MaxCoopPopulation <= 0)
            {
                _config.BuildingSettings.MaxCoopPopulation = 1;
                Helper.WriteConfig(_config);
                Monitor.Log("The configured MaxCoopPopulation wasn't higher than Zero, it has been set to 1",
                    LogLevel.Debug);
            }

            if (!Enum.TryParse(_config.MainSettings.RefreshConfigButton.ToString(), ignoreCase: true,
                out RefreshConfig))
            {
                RefreshConfig = SButton.F5;
                Monitor.Log("There was an error parsing the RefreshConfigButton. It was reset to F5");
            }
        }

        private void ResetSave(string command, string[] args)
        {
            if (_data != null)
            {
                _data?.Buildings.Clear();
                Monitor.Log("Save data was reset.", LogLevel.Debug);
            }
        }

        private void Log(string message, bool useTrace = false)
        {
            if(!useTrace)
                Monitor.Log(message, LogLevel.Debug);
            else
                Monitor.Log(message);
            
        }

        private void ModifyBuildings(bool showLog, bool doRestore = false)
        {
            var locations = DataLoader.Locations(Game1.content);
            var loadedFarm = false;

            if (_data is null || !_data.Buildings.Any() || locations is null)
                return;
            
            //Now we modify the building

            foreach (var location in locations)
            {
                GameLocation loc;
                if (location.Key.Contains("Farm_") && !loadedFarm)
                {
                    loc = Game1.getLocationFromName("Farm");
                    loadedFarm = true;
                }
                else
                {
                    loc = Game1.getLocationFromName(location.Key);
                }

                if (loc is null || !loc.IsBuildableLocation())
                    continue;
                
                if(_config.MainSettings.EnableDebugMode)
                    Log($"{loc.DisplayName} was a buildable location.");

                foreach (var building in loc.buildings.Where(building => building?.indoors.Value != null))
                {
                    if(_config.MainSettings.EnableDebugMode)
                        Log($"Scanning for {building.indoors.Value.uniqueName.Value}");

                    GameLocation l = building.indoors.Value;
                    AnimalHouse animalHouse = (AnimalHouse)(l is AnimalHouse ? l : null);
                    
                    if(animalHouse is null || !_data.Buildings.ContainsKey(building.indoors.Value.uniqueName.Value))
                        continue;
                    
                    if(_config.MainSettings.EnableDebugMode)
                        Log($"_data found for {building.indoors.Value.uniqueName.Value}.");

                    if (doRestore)
                    {
                        if(_config.MainSettings.EnableDebugMode)
                            Log("Running Restore");
                        if (_config.BuildingSettings.AutoFeedExtraAnimals)
                            FeedExtraAnimals(animalHouse);
                        
                        ResetPopulationChange(building, _config.MainSettings.EnableDebugMode);
                    }
                    else
                    {
                        if(_config.MainSettings.EnableDebugMode)
                            Log("Running Change Population");

                        DoPopulationChange(building, _config.MainSettings.EnableDebugMode);
                    }
                }
            }
        }

        private void DoPopulationChange(Building building, bool showLog = false)
        {
            if (building is null)
            {
                Log("Building was null");
                return;
            }
            
            var pop = building.buildingType.Value.Contains("Deluxe Barn")
                ? _config.BuildingSettings.MaxBarnPopulation
                : _config.BuildingSettings.MaxCoopPopulation;

            var curPop = ((AnimalHouse)building.indoors.Value).animalLimit.Value;

            if (((AnimalHouse)building.indoors.Value).animalLimit.Value != pop && !_data.Buildings.ContainsKey(building.indoors.Value.uniqueName.Value))
            {
                ((AnimalHouse)building.indoors.Value).animalLimit.Value = pop;
                building.maxOccupants.Value = pop;
                _data.Buildings.TryAdd(building.indoors.Value.uniqueName.Value, value: true);
            }
            else if (((AnimalHouse)building.indoors.Value).animalLimit.Value != pop)
            {
                ((AnimalHouse)building.indoors.Value).animalLimit.Value = pop;
                building.maxOccupants.Value = pop;
            }
            
            if(_config.MainSettings.EnableDebugMode)
                Game1.showGlobalMessage($"Set {building.buildingType.Value}'s max occupants to {building.maxOccupants.Value}");
            
            Log($"Set {building.buildingType.Value}'s max occupants to {building.maxOccupants.Value}", showLog);
        }

        private void ResetPopulationChange(Building building, bool showLog = false)
        {
            var pop = ((building.buildingType).Value.Contains("Deluxe Barn") ? _config.BuildingSettings.MaxBarnPopulation : _config.BuildingSettings.MaxCoopPopulation);
            if (((AnimalHouse)building.indoors.Value).animalLimit.Value == pop)
            {
                ((AnimalHouse)building.indoors.Value).animalLimit.Value = 12;
                building.maxOccupants.Value = 12;
            }
            if (_config.MainSettings.EnableDebugMode)
            {
                Game1.showGlobalMessage($"Reset {building.buildingType.Value}'s max occupants to {building.maxOccupants.Value}.");
            }
            Log($"Reset {building.buildingType.Value}'s max occupants to {building.maxOccupants.Value}.", showLog);
        }

        private static int GetNumberTroughs(GameLocation location)
        {
            var numTroughs = 0;

            for (int x = 0; x < location.map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < location.map.Layers[0].LayerHeight; y++)
                {
                    if (location.doesTileHaveProperty(x, y, "Trough", "Back", false) != null)
                    {
                        numTroughs++;
                    }
                }
            }

            return numTroughs;
        }

        private void FeedExtraAnimals(AnimalHouse ah)
        {
            if (ah is null)
                return;

            var rootLocation = ((GameLocation)ah).GetRootLocation();
            var numTroughs = GetNumberTroughs(ah);
            var numAnimals = ah.animalsThatLiveHere.Count;
            var numExtraAnimals = numAnimals - numTroughs > 0 ? numAnimals - numTroughs : 0;
            var animalsFed = 0;
            
            if(_config.MainSettings.EnableDebugMode)
                Log($"Troughs: {numTroughs}, NumAnimals: {numAnimals}, Extra Animals: {numExtraAnimals}");

            if (numExtraAnimals == 0 || !ah.Animals.Any())
                return;

            var animals = ah.Animals.Pairs.ToArray();
            foreach (var animal in animals)
            {
                var numHay = rootLocation.piecesOfHay.Value;
                if (animalsFed < numExtraAnimals && numHay >= 1)
                {
                    animal.Value.fullness.Value = 255;
                    rootLocation.piecesOfHay.Value--;
                    animalsFed++;
                }
            }
        }
    }
}