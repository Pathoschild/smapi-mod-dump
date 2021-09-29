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
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace CropCheck
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
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
            helper.ConsoleCommands.Add("cropcheck_list", $"Lists all tiles that the player has designated to check.\n\nUsage: cropcheck_list <location>\n- location: do 'cropcheck_list all' for a list of all locations where you have saved a tile.", this.ListSavedTiles);
            helper.ConsoleCommands.Add("cropcheck_clear", $"Unsaves all tiles that the player has designated to check.\n\nUsage: cropcheck_clear <location>\n- location: do 'cropcheck_list all' for a list of all locations where you have saved a tile or use 'all' to clear all locations.", this.ClearSavedTiles);

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
            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "Auto Unsave Tile",
                optionDesc: "When this is set to true tiles will automatically be unsaved after they are fully grown.",
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
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.TileChecks)
            {
                if (!model.TileChecks[entry.Key].ContainsKey("DisplayName"))
                {
                    model.TileChecks[entry.Key].Add("DisplayName", GenerateDisplayName(Game1.getLocationFromName(entry.Key)));
                    this.Helper.Data.WriteSaveData("TileChecksData", model);
                }
            }
        }

        /// <summary>Raised after the day starts in game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            List<string> locationsToUnsave = new List<string>();
            List<string> tilesToUnsave = new List<string>();

            // goes through each tile in farmable locations and checks for harvestable crops that are in tiles that are saved
            foreach (string location in model.TileChecks.Keys)
            {
                // unsaves location if there are no tiles saved
                if (model.TileChecks[location].Count <= 1)
                {
                    locationsToUnsave.Add(location);
                }
                else
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
                                    Game1.addHUDMessage(new HUDMessage($"{entry.Key} is ready to be harvested in {GetDisplayName(location)}.", ""));
                                    if (this.Config.AutoUnsave)
                                    {
                                        tilesToUnsave.Add(entry.Key);
                                    }
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
                                    Game1.addHUDMessage(new HUDMessage($"{entry.Key} has {(t as FruitTree).fruitsOnTree} fruit(s) ready to be harvested in {GetDisplayName(location)}.", ""));
                                    if (this.Config.AutoUnsave)
                                    {
                                        tilesToUnsave.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    foreach (Object o in Game1.getLocationFromName(location).Objects.Values)
                    {
                        if (o is IndoorPot && (o as IndoorPot).hoeDirt.Value.readyForHarvest())
                        {
                            foreach (KeyValuePair<string, string> entry in model.TileChecks[location])
                            {
                                if (entry.Value == o.tileLocation.ToString())
                                {
                                    Game1.addHUDMessage(new HUDMessage($"{entry.Key} is ready to be harvested in {GetDisplayName(location)}.", ""));
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
                    model.TileChecks[location].Remove(entry);
                    this.Helper.Data.WriteSaveData("TileChecksData", model);
                }
            }
            foreach (string location in locationsToUnsave)
            {
                model.TileChecks.Remove(location);
                this.Helper.Data.WriteSaveData("TileChecksData", model);
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
                var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();

                // adds a new location to save data if it doesn't already exist
                if (!model.TileChecks.ContainsKey(Game1.currentLocation.NameOrUniqueName))
                {
                    model.TileChecks.Add(Game1.currentLocation.NameOrUniqueName, new Dictionary<string, string>());
                    model.TileChecks[Game1.currentLocation.NameOrUniqueName].Add("DisplayName", GenerateDisplayName(Game1.currentLocation));
                    this.Helper.Data.WriteSaveData("TileChecksData", model);
                }

                string playerLocation = Game1.currentLocation.NameOrUniqueName;

                this.cursorPos = this.Helper.Input.GetCursorPosition();

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
                    Game1.addHUDMessage(new HUDMessage($"{tileToBeRemoved} has been removed from the tile checks for {GetDisplayName(playerLocation)}.", ""));
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
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            string playerLocation = Game1.currentLocation.NameOrUniqueName;

            // checks if there is already a tile saved with that name in the current location
            if (!model.TileChecks[playerLocation].ContainsKey($"{TileName}"))
            {
                model.TileChecks[playerLocation].Add($"{TileName}", this.cursorPos.Tile.ToString());
                Game1.addHUDMessage(new HUDMessage($"{TileName} has been added to tile checks for {GetDisplayName(playerLocation)}.", ""));
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("There is already a tile saved with that name.", ""));
            }
            this.Helper.Data.WriteSaveData("TileChecksData", model);
        }

        /// <summary>Generates a unique display name for locations.</summary>
        /// <param name="location">The location to generate the display name for.</param>
        private string GenerateDisplayName(GameLocation location)
        {
            if (location.uniqueName.Value is null) return location.Name;

            string name = location.Name;
            int index = 0;
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            if (model.TileChecks.Count == 0)
            {
                return name + index.ToString();
            }

            while (true)
            {
                bool uniqueNameBool = true;

                foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.TileChecks)
                {
                    if (model.TileChecks[entry.Key].ContainsKey("DisplayName"))
                    {
                        if (model.TileChecks[entry.Key]["DisplayName"] == name + index.ToString())
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

            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.TileChecks)
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
            var model = this.Helper.Data.ReadSaveData<ModData>("TileChecksData") ?? new ModData();
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in model.TileChecks)
            {
                if (entry.Key == RealName)
                {
                    return model.TileChecks[entry.Key]["DisplayName"];
                }
            }
            return null;
        }

        /****
        ** Console commands
        ****/
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
            string locationName = GetRealName(args[0]);
            string output = "";

            if (locationName is null)
            {
                this.Monitor.Log("Not a valid Location or there are no tiles saved in that location. (Location names are case senstive)", LogLevel.Error);
                return;
            }
            else if (locationName == "all")
            {
                foreach (string location in model.TileChecks.Keys)
                {
                    output += $"\n{model.TileChecks[location]["DisplayName"]}:";
                    foreach (KeyValuePair<string, string> entry in model.TileChecks[location])
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
                foreach (KeyValuePair<string, string> entry in model.TileChecks[locationName])
                {
                    if (entry.Key != "DisplayName")
                    {
                        output += $"\n{entry.Key} : {entry.Value}";
                    }
                }
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
            string locationName = GetRealName(args[0]);

            if (locationName is null)
            {
                this.Monitor.Log("Not a valid Location or there are no tiles saved in that location. (Location names are case senstive)", LogLevel.Error);
            }
            else if (locationName == "all")
            {
                foreach (string location in model.TileChecks.Keys)
                {
                    string DisplayName = model.TileChecks[location]["DisplayName"];
                    model.TileChecks[location].Clear();
                    model.TileChecks[location].Add("DisplayName", DisplayName);
                }
                this.Helper.Data.WriteSaveData("TileChecksData", model);
                this.Monitor.Log("Cleared all tiles from all locations.", LogLevel.Info);
            }
            else
            {
                string DisplayName = model.TileChecks[locationName]["DisplayName"];
                model.TileChecks[locationName].Clear();
                model.TileChecks[locationName].Add("DisplayName", DisplayName);
                this.Helper.Data.WriteSaveData("TileChecksData", model);
                this.Monitor.Log($"Cleared all tiles from {args[0]}.", LogLevel.Info);
            }
        }
    }
}