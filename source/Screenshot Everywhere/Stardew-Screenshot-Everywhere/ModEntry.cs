/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaiadin/Stardew-Screenshot-Everywhere
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Stardew_Screenshot_Everywhere
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private List<string> l_visited = new List<string>();
        private List<string> locations = new List<string>();
        private int tick = 35;
        private ModConfig Config;
        public DirectoryInfo DefaultSSdirectory { get; private set; }
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            updateConfig();
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.ToString() == this.Config.HotKey.ToString())
                this.takeScreenshot();
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            int num11 = (Environment.OSVersion.Platform != PlatformID.Unix ? 26 : 28);
            var path = Environment.GetFolderPath((Environment.SpecialFolder)num11);
            DefaultSSdirectory = new DirectoryInfo(Path.Combine(path, "StardewValley", "Screenshots"));

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
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Debug",
                getValue: () => this.Config.Debug,
                setValue: value => this.Config.Debug = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Screenshot Hotkey",
                getValue: () => this.Config.HotKey,
                setValue: value => this.Config.HotKey = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Capture Notification",
                getValue: () => this.Config.NotifyCapture,
                setValue: value => this.Config.NotifyCapture = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Camera Flash",
                getValue: () => this.Config.CameraFlash,
                setValue: value => this.Config.CameraFlash = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Shutter Sound",
                getValue: () => this.Config.ShutterSound,
                setValue: value => this.Config.ShutterSound = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Farm",
                getValue: () => this.Config.Farm,
                setValue: value => this.Config.Farm = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "FarmHouse",
                getValue: () => this.Config.FarmHouse,
                setValue: value => this.Config.FarmHouse = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Greenhouse",
                getValue: () => this.Config.Greenhouse,
                setValue: value => this.Config.Greenhouse = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Cellar",
                getValue: () => this.Config.Cellar,
                setValue: value => this.Config.Cellar = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Town",
                getValue: () => this.Config.Town,
                setValue: value => this.Config.Town = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Beach",
                getValue: () => this.Config.Beach,
                setValue: value => this.Config.Beach = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Sheds",
                getValue: () => this.Config.Shed,
                setValue: value => this.Config.Shed = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Coops",
                getValue: () => this.Config.Coop,
                setValue: value => this.Config.Coop = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Barns",
                getValue: () => this.Config.Barn,
                setValue: value => this.Config.Barn = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "IslandFarm",
                getValue: () => this.Config.IslandFarm,
                setValue: value => this.Config.IslandFarm = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Desert",
                getValue: () => this.Config.Desert,
                setValue: value => this.Config.Desert = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Forest",
                getValue: () => this.Config.Forest,
                setValue: value => this.Config.Forest = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Screenshot Locations",
                getValue: () => this.Config.ScreenShotLocations,
                setValue: value => this.Config.ScreenShotLocations = value
            );
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            updateConfig();
            if (canScreenshot())
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
            if (Config.Debug)
            {
                this.Monitor.Log($"{Game1.player.name} entered {Game1.player.currentLocation.Name}", LogLevel.Debug);
                this.Monitor.Log($"UniqueName: {Game1.player.currentLocation.uniqueName}", LogLevel.Debug);
                this.Monitor.Log($"Locations list: {readableList(this.locations)}", LogLevel.Debug);
                this.Monitor.Log($"Locations config: {this.Config.ScreenShotLocations}", LogLevel.Debug);
                this.Monitor.Log($"Visited Today: {readableList(this.l_visited)}", LogLevel.Debug);

            }
        }

        private bool canScreenshot()
        {
            string currentLocation = Game1.player.currentLocation.Name.ToLower();
            string cl_unique = Game1.player.currentLocation.uniqueName.ToString().ToLower();
            if (this.Config.Shed && currentLocation.Contains("shed"))
                if (!this.locations.Contains(cl_unique))
                    this.locations.Add(cl_unique);
            if (this.Config.Coop && currentLocation.Contains("coop"))
                if (!this.locations.Contains(cl_unique))
                    this.locations.Add(cl_unique);
            if (this.Config.Barn && currentLocation.Contains("barn"))
                if (!this.locations.Contains(cl_unique))
                    this.locations.Add(cl_unique);
            if (this.locations.Count > 1)
                if (this.locations.Contains(currentLocation) || this.locations.Contains(cl_unique))
                    if (!this.l_visited.Contains(currentLocation) && !this.l_visited.Contains(cl_unique))
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                if (!this.l_visited.Contains(currentLocation) && !this.l_visited.Contains(cl_unique))
                    return true;
                else
                    return false;
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            l_visited.Clear();
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this.tick <= 0)
            {

                string loc = Game1.player.currentLocation.Name.ToLower();
                if (loc.Contains("barn") || loc.Contains("shed") || loc.Contains("coop"))
                    l_visited.Add(Game1.currentLocation.uniqueName.ToString().ToLower());
                else
                    l_visited.Add(loc);
                this.takeScreenshot();
                Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
                this.tick = 35;
            }
            else
                this.tick -= 1;
        }

        private void takeScreenshot()
        {
            //Take Screenshot
            if (this.Config.CameraFlash)
                Game1.flashAlpha = 1f;
            if (this.Config.ShutterSound)
                Game1.playSound("cameraNoise");
            if (this.Config.NotifyCapture)
                Game1.addHUDMessage(
                    new HUDMessage(" Screenshot captured", HUDMessage.screenshot_type)
                );
            Game1.game1.takeMapScreenshot((float)1.0, getFileName(), () => { });
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            l_visited.Clear();
        }

            private string getFileName()
        {
            char sep = Path.DirectorySeparatorChar;

            string segment1 = $"{Game1.player.farmName}-{Game1.uniqueIDForThisGame}";
            string loc = Game1.player.currentLocation.Name;
            string uLoc = Game1.player.currentLocation.uniqueName;
            string segment2 = "";
            if (loc.ToLower().Contains("coop"))
                segment2 = $"Coops{sep}{uLoc}";
            else if (loc.ToLower().Contains("barn"))
                segment2 = $"Barns{sep}{uLoc}";
            else if (loc.ToLower().Contains("shed"))
                segment2 = $"Sheds{sep}{uLoc}";
            else
                segment2 = loc;

            Directory.CreateDirectory(
                Path.Combine(
                    DefaultSSdirectory.FullName,
                    segment1,
                    segment2
                    )
                );
            string gameDate = string.Format("{0:D2}-{1:D2}-{2:D2}", Game1.Date.Year, Game1.Date.SeasonIndex + 1, Game1.Date.DayOfMonth);
            return $".{sep}{segment1}{sep}{segment2}{sep}{gameDate}";
        }

        private string readableList(List<string> e)
        {
            string val = "";
            foreach (string x in e)
            {
                val += $" {x},";
            }
            return val;
        }

        private void updateConfig()
        {
            string locs = "";
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if (this.Config.Farm)
                locs += "farm,";
            if (this.Config.FarmHouse)
                locs += "farmhouse,";
            if (this.Config.Town)
                locs += "town,";
            if (this.Config.Cellar)
                locs += "cellar,";
            if (this.Config.Beach)
                locs += "beach,";
            if (this.Config.Greenhouse)
                locs += "greenhouse,";
            if (this.Config.Coop)
                locs += "coop,big coop,deluxe coop,";
            if (this.Config.Barn)
                locs += "barn,big barn,deluxe barn,";
            if (this.Config.Shed)
                locs += "shed,big shed,";
            if (this.Config.IslandFarm)
                locs += "islandwest,";
            if (this.Config.Desert)
                locs += "desert,";
            if (this.Config.Forest)
                locs += "forest,";

            if (locs.Length > 0)
                locs = locs.Substring(0, locs.Length - 1);

            this.locations = $"{locs}{(locs.Length > 0 ? "," : "")}{Config.ScreenShotLocations}".ToLower().Split(',').ToList<string>();
        }
    }
}