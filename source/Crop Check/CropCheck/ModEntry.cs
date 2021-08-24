/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FairfieldBW/CropCheck
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace CropCheck
{

    public class ModEntry : Mod
    {

        private ModConfig Config;

        private List<string> Locations;

        private string TileName;

        private ICursorPosition cursorPos;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Locations = new List<string>()
            {
                "Farm",
                "IslandWest",
                "Greenhouse"
            };

            // console commands
            helper.ConsoleCommands.Add("cropcheck_list", "Lists all tiles that the player has designated to check.\n\nUsage: cropcheck_list <location>\n- location: farm, greenhouse, islandwest, or all", this.ListSavedTiles);
            helper.ConsoleCommands.Add("cropcheck_clear", "Unsaves all tiles that the player has designated to check.\n\nUsage: cropcheck_clear <location>\n- location: farm, greenhouse, islandwest, or all", this.ClearSavedTiles);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /// <summary>Raised when the player finishes naming the tile.</summary>
        /// <param name="name">The tile name.</param>
        public void ReturnTileName(string name)
        {
            this.TileName = name;
            Game1.exitActiveMenu();
            this.SaveTile(TileName);
        }

        /// <summary>Raised after player types the cropcheck_list command.</summary>
        /// <param name="command">The event command.</param>
        /// <param name="args">The event arguments.</param>
        private void ListSavedTiles(string command, string[] args)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("A save must be loaded to use this command.", LogLevel.Error);
                return;
            }
            else if (args.GetLength(0) < 1)
            {
                this.Monitor.Log("Missing an argument. Type 'help cropcheck_list' for more info.", LogLevel.Error);
                return;
            }

            // lists all saved tiles in each location
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            string locationToList = args[0].ToLower();
            string output = "";

            if (locationToList == "all")
            {
                foreach (string location in Locations)
                {
                    output += $"\n{location}:";
                    foreach (KeyValuePair<string, string> entry in model.TileChecks[location])
                    {
                        output += $"\n{entry.Key} : {entry.Value}";
                    }
                }
            }
            else if (locationToList == "farm")
            {
                foreach (KeyValuePair<string, string> entry in model.TileChecks["Farm"])
                {
                    output += $"\n{entry.Key} : {entry.Value}";
                }
            }
            else if (locationToList == "greenhouse")
            {
                foreach (KeyValuePair<string, string> entry in model.TileChecks["Greenhouse"])
                {
                    output += $"\n{entry.Key} : {entry.Value}";
                }
            }
            else if (locationToList == "islandwest")
            {
                foreach (KeyValuePair<string, string> entry in model.TileChecks["IslandWest"])
                {
                    output += $"\n{entry.Key} : {entry.Value}";
                }
            }
            else
            {
                this.Monitor.Log("Not a valid Location", LogLevel.Error);
                return;
            }
            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Raised after player types the cropcheck_clear command.</summary>
        /// <param name="command">The event command.</param>
        /// <param name="args">The event arguments.</param>
        private void ClearSavedTiles(string command, string[] args)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("A save must be loaded to use this command.", LogLevel.Error);
                return;
            }
            else if (args.GetLength(0) < 1)
            {
                this.Monitor.Log("Missing an argument. Type 'help cropcheck_clear' for more info.", LogLevel.Error);
                return;
            }

            // unsaves all saved tiles in each location
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            string locationToList = args[0].ToLower();

            if (locationToList == "all")
            {
                foreach (string location in Locations)
                {
                    model.TileChecks[location].Clear();
                }
                this.Monitor.Log("Cleared all tiles from all locations.", LogLevel.Info);
            }
            else if (locationToList == "farm")
            {
                model.TileChecks["Farm"].Clear();
                this.Monitor.Log($"Cleared all tiles from {locationToList}.", LogLevel.Info);
            }
            else if (locationToList == "greenhouse")
            {
                model.TileChecks["Greenhouse"].Clear();
                this.Monitor.Log($"Cleared all tiles from {locationToList}.", LogLevel.Info);
            }
            else if (locationToList == "islandwest")
            {
                model.TileChecks["IslandWest"].Clear();
                this.Monitor.Log($"Cleared all tiles from {locationToList}.", LogLevel.Info);
            }
            else
            {
                this.Monitor.Log("Not a valid Location", LogLevel.Error);
                return;
            }
            this.Helper.Data.WriteSaveData("TileChecksData", model);
        }

        /// <summary>Raised after the game is launched.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // add config to Generic Mog Config Menu
            var api = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;

            api.RegisterModConfig(
                mod: this.ModManifest,
                revertToDefault: () => this.Config = new ModConfig(),
                saveToFile: () => this.Helper.WriteConfig(this.Config)
            );

            api.SetDefaultIngameOptinValue(this.ModManifest, true);

            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "CropCheck Hotkey",
                optionDesc: "The hotkey used to save and unsave tiles to be checked by CropCheck.",
                optionGet: () => this.Config.ToggleKey,
                optionSet: value => this.Config.ToggleKey = value
            );
            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "Fruit on Trees for Alert",
                optionDesc: "The number of fruits on a tree needed for CropCheck to alert the player.",
                optionGet: () => this.Config.FruitTreeAlertNumber,
                optionSet: value => this.Config.FruitTreeAlertNumber = value
            );
        }

        /// <summary>Raised after the day starts in game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();

            // goes through each tile in farmable locations and checks for harvestable crops that are in tiles that are saved
            foreach (string location in Locations)
            {
                foreach (TerrainFeature t in Game1.getLocationFromName(location).terrainFeatures.Values)
                {
                    // checks for harvestable crop on tile
                    if (t is HoeDirt && (t as HoeDirt).readyForHarvest())
                    {
                        foreach (KeyValuePair<string, string> entry in model.TileChecks[location])
                        {
                            if (entry.Value == t.currentTileLocation.ToString())
                            {
                                Game1.addHUDMessage(new HUDMessage($"{entry.Key} is ready to be harvested in {location}.", ""));
                            }
                        }
                    }
                    // checks for tree with harvestable fruits on tile
                    else if (t is FruitTree && (t as FruitTree).fruitsOnTree >= this.Config.FruitTreeAlertNumber)
                    {
                        foreach (KeyValuePair<string, string> entry in model.TileChecks[location])
                        {
                            if (entry.Value == t.currentTileLocation.ToString())
                            {
                                Game1.addHUDMessage(new HUDMessage($"{entry.Key} has {(t as FruitTree).fruitsOnTree} fruit(s) ready to be harvested in {location}.", ""));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Raised after the player presses a button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // checks if the key pressed matches the hotkey in the config and if the player is able to move
            if (this.Config.ToggleKey.JustPressed() && (Context.IsPlayerFree))
            {
                // checks if the current location is a valid location
                if (Locations.Contains(Game1.currentLocation.Name))
                {
                    string playerLocation = Game1.currentLocation.Name;

                    this.cursorPos = this.Helper.Input.GetCursorPosition();

                    var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();

                    this.Monitor.Log($"{model.TileChecks}", LogLevel.Info);

                    bool tileAlreadyExists = false;
                    string tileToBeRemoved = "";

                    // checks if tile is already saved
                    foreach (KeyValuePair<string, string> entry in model.TileChecks[playerLocation])
                    {
                        if (entry.Value == this.cursorPos.Tile.ToString())
                        {
                            tileAlreadyExists = true;
                            tileToBeRemoved = entry.Key;
                        }
                    }

                    // if tile is saved, unsave it
                    if (tileAlreadyExists)
                    {
                        model.TileChecks[playerLocation].Remove(tileToBeRemoved);
                        this.Helper.Data.WriteSaveData("TileChecksData", model);
                        Game1.addHUDMessage(new HUDMessage($"{tileToBeRemoved} has been removed from the tile checks for {playerLocation}.", ""));
                    }
                    // open naming menu
                    else
                    {
                        Game1.activeClickableMenu = new NamingMenu(ReturnTileName, "Enter Tile Name:", "");
                    }
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage("You can't grow crops here!", ""));
                }
            }
        }

        /// <summary>Raised when the naming menu closes.</summary>
        /// <param name="TileName">The tile name to be saved.</param>
        private void SaveTile(string TileName)
        {
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            string playerLocation = Game1.currentLocation.Name;

            // checks if there is already a tile saved with that name in the current location
            if (!model.TileChecks[playerLocation].ContainsKey($"{TileName}"))
            {
                model.TileChecks[playerLocation].Add($"{TileName}", this.cursorPos.Tile.ToString());
                Game1.addHUDMessage(new HUDMessage($"{TileName} has been added to tile checks for {playerLocation}.", ""));
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("There is already a tile saved with that name.", ""));
            }
            this.Helper.Data.WriteSaveData("TileChecksData", model);
        }
    }
}