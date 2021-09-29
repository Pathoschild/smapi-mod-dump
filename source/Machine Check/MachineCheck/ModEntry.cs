/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FairfieldBW/MachineCheck
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using GenericModConfigMenu;

namespace MachineCheck
{
    /// <summary>The mod entry point.</summary>
    class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod settings.</summary>
        private ModConfig Config;

        /// <summary>String for tile name that neds to be saved.</summary>
        private string TileName;

        /// <summary>The current cursor position.</summary>
        private ICursorPosition cursorPos;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // console commands
            helper.ConsoleCommands.Add("machinecheck_list", $"Lists all tiles that the player has designated to check.\n\nUsage: machinecheck_list_list <location>\n- location: do 'machinecheck_list all' for a list of all locations where you have saved a tile.", this.ListSavedTiles);
            helper.ConsoleCommands.Add("machinecheck_clear", $"Unsaves all tiles that the player has designated to check.\n\nUsage: machinecheck_list_clear <location>\n- location: do 'machinecheck_list all' for a list of all locations where you have saved a tile or use 'all' to clear all locations.", this.ClearSavedTiles);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
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
                optionName: "MachineCheck Hotkey",
                optionDesc: "The hotkey used to save and unsave tiles to be checked by MachineCheck.",
                optionGet: () => this.Config.ToggleKey,
                optionSet: value => this.Config.ToggleKey = value
            );
            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "Auto Unsave Tile",
                optionDesc: "When this is set to true tiles will automatically be unsaved after they produce an item.",
                optionGet: () => this.Config.AutoUnsave,
                optionSet: value => this.Config.AutoUnsave = value
            );
        }

        /// <summary>Raised after a save is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // add display names for all legacy locations
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.MachineChecks)
            {
                if (!model.MachineChecks[entry.Key].ContainsKey("DisplayName"))
                {
                    model.MachineChecks[entry.Key].Add("DisplayName", GenerateDisplayName(Game1.getLocationFromName(entry.Key)));
                    this.Helper.Data.WriteSaveData("MachineChecksData", model);
                }
            }
        }

        /// <summary>Raised after the day starts in game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            List<string> locationsToUnsave = new List<string>();
            List<string> tilesToUnsave = new List<string>();

            // goes through each tile in all saved locations and sees if there are machines ready to be harvested
            foreach (string location in model.MachineChecks.Keys)
            {
                // unsaves location if there are no tiles saved
                if (model.MachineChecks[location].Count == 0)
                {
                    locationsToUnsave.Add(location);
                }
                else
                {
                    foreach (StardewValley.Object o in Game1.getLocationFromName(location).Objects.Values)
                    {
                        if (o.IsConsideredReadyMachineForComputer())
                        {
                            foreach (KeyValuePair<string, string> entry in model.MachineChecks[location])
                            {
                                if (entry.Value == o.tileLocation.ToString())
                                {
                                    Game1.addHUDMessage(new HUDMessage($"{entry.Key} has finished producing in {GetDisplayName(location)}.", ""));
                                    if (this.Config.AutoUnsave)
                                    {
                                        tilesToUnsave.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (string entry in tilesToUnsave)
                {
                    model.MachineChecks[location].Remove(entry);
                    this.Helper.Data.WriteSaveData("MachineChecksData", model);
                }
            }
            foreach (string location in locationsToUnsave)
            {
                model.MachineChecks.Remove(location);
                this.Helper.Data.WriteSaveData("MachineChecksData", model);
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
                var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();

                // adds a new location to save data if it doesn't already exist
                if (!model.MachineChecks.ContainsKey(Game1.currentLocation.NameOrUniqueName))
                {
                    model.MachineChecks.Add(Game1.currentLocation.NameOrUniqueName, new Dictionary<string, string>());
                    model.MachineChecks[Game1.currentLocation.NameOrUniqueName].Add("DisplayName", GenerateDisplayName(Game1.currentLocation));
                    this.Helper.Data.WriteSaveData("MachineChecksData", model);
                }

                string playerLocation = Game1.currentLocation.NameOrUniqueName;

                this.cursorPos = this.Helper.Input.GetCursorPosition();

                bool tileAlreadyExists = false;
                string tileToBeRemoved = "";

                // checks if tile is already saved
                foreach (KeyValuePair<string, string> entry in model.MachineChecks[playerLocation])
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
                    model.MachineChecks[playerLocation].Remove(tileToBeRemoved);
                    this.Helper.Data.WriteSaveData("MachineChecksData", model);
                    Game1.addHUDMessage(new HUDMessage($"{tileToBeRemoved} has been removed from the machine checks for {GetDisplayName(playerLocation)}.", ""));
                }
                // open naming menu
                else
                {
                    Game1.activeClickableMenu = new NamingMenu(ReturnTileName, "Enter Tile Name:", "");
                }
            }
        }

        /****
        ** Helper methods
        ****/
        /// <summary>Raised when the player finishes naming the tile.</summary>
        /// <param name="name">The tile name.</param>
        private void ReturnTileName(string name)
        {
            this.TileName = name;
            Game1.exitActiveMenu();
            this.SaveTile(TileName);
        }

        /// <summary>Raised when the naming menu closes.</summary>
        /// <param name="TileName">The tile name to be saved.</param>
        private void SaveTile(string TileName)
        {
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            string playerLocation = Game1.currentLocation.NameOrUniqueName;

            // checks if there is already a tile saved with that name in the current location
            if (!model.MachineChecks[playerLocation].ContainsKey($"{TileName}"))
            {
                model.MachineChecks[playerLocation].Add($"{TileName}", this.cursorPos.Tile.ToString());
                Game1.addHUDMessage(new HUDMessage($"{TileName} has been added to machine checks for {GetDisplayName(playerLocation)}.", ""));
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("There is already a tile saved with that name.", ""));
            }
            this.Helper.Data.WriteSaveData("MachineChecksData", model);
        }

        /// <summary>Generates a unique display name for locations.</summary>
        /// <param name="location">The location to generate the display name for.</param>
        private string GenerateDisplayName(GameLocation location)
        {
            if (location.uniqueName.Value is null) return location.Name;

            string name = location.Name;
            int index = 0;
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            if (model.MachineChecks.Count == 0)
            {
                return name + index.ToString();
            }

            while (true)
            {
                bool uniqueNameBool = true;

                foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.MachineChecks)
                {
                    if (model.MachineChecks[entry.Key].ContainsKey("DisplayName"))
                    {
                        if (model.MachineChecks[entry.Key]["DisplayName"] == name + index.ToString())
                        {
                            uniqueNameBool = false;
                        }
                    }
                }
                if (uniqueNameBool)
                {
                    return name + index.ToString();
                }

                index++;
            }
        }

        /// <summary>Returns real name of location when given display name</summary>
        /// <param name="DisplayName">The display name of the location.</param>
        private string GetRealName(string DisplayName)
        {
            if (DisplayName == "all") return DisplayName;

            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.MachineChecks)
            {
                if (GetDisplayName(entry.Key) == DisplayName)
                {
                    return entry.Key;
                }
            }
            return null;
        }

        /// <summary>Returns display name of location when given real name</summary>
        /// <param name="RealName">The real name of the location.</param>
        private string GetDisplayName(string RealName)
        {
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.MachineChecks)
            {
                if (entry.Key == RealName)
                {
                    return model.MachineChecks[entry.Key]["DisplayName"];
                }
            }
            return null;
        }

        /****
        ** Console commands
        ****/
        /// <summary>Raised after player types the machinecheck_list command.</summary>
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
                this.Monitor.Log("Missing an argument. Type 'help machinecheck_list' for more info.", LogLevel.Error);
                return;
            }

            // lists all saved tiles in each location
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            string locationName = GetRealName(args[0]);
            string output = "";

            if (locationName is null)
            {
                this.Monitor.Log("Not a valid Location or there are no tiles saved in that location. (Location names are case senstive)", LogLevel.Error);
                return;
            }
            else if (locationName == "all")
            {
                foreach (string location in model.MachineChecks.Keys)
                {
                    output += $"\n{model.MachineChecks[location]["DisplayName"]}:";
                    foreach (KeyValuePair<string, string> entry in model.MachineChecks[location])
                    {
                        if (entry.Key != "DisplayName")
                        {
                            output += $"\n{entry.Key} : {entry.Value}";
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> entry in model.MachineChecks[locationName])
                {
                    if (entry.Key != "DisplayName")
                    {
                        output += $"\n{entry.Key} : {entry.Value}";
                    }
                }
            }
            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Raised after player types the machinecheck_clear command.</summary>
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
                this.Monitor.Log("Missing an argument. Type 'help machinecheck_clear' for more info.", LogLevel.Error);
                return;
            }

            // unsaves all saved tiles in each location
            var model = this.Helper.Data.ReadSaveData<ModData>("MachineChecksData") ?? new ModData();
            string locationName = GetRealName(args[0]);

            if (locationName is null)
            {
                this.Monitor.Log("Not a valid Location or there are no tiles saved in that location. (Location names are case senstive)", LogLevel.Error);
            }
            else if (locationName == "all")
            {
                foreach (string location in model.MachineChecks.Keys)
                {
                    string DisplayName = model.MachineChecks[location]["DisplayName"];
                    model.MachineChecks[location].Clear();
                    model.MachineChecks[location].Add("DisplayName", DisplayName);
                }
                this.Helper.Data.WriteSaveData("MachineChecksData", model);
                this.Monitor.Log("Cleared all tiles from all locations.", LogLevel.Info);
            }
            else
            {
                string DisplayName = model.MachineChecks[locationName]["DisplayName"];
                model.MachineChecks[locationName].Clear();
                model.MachineChecks[locationName].Add("DisplayName", DisplayName);
                this.Helper.Data.WriteSaveData("MachineChecksData", model);
                this.Monitor.Log($"Cleared all tiles from {args[0]}.", LogLevel.Info);
            }
        }
    }
}