/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using System;
using System.IO;
using GenericModConfigMenu;
using JsonAssets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfitCalculator.menus;
using ProfitCalculator.ui;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using ProfitCalculator.main;
using Crop = ProfitCalculator.main.Crop;

#nullable enable

namespace ProfitCalculator
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig? Config;
        private ProfitCalculatorMainMenu? mainMenu;

        /// <summary>The mod's calculator functions.</summary>
        public static Calculator? Calculator;

        private IModHelper? helper;
        private IGenericModConfigMenuApi? configMenu;

        /// <summary>The mod's API.</summary>
        public static ModApi API { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Calculator = new();
            API = new ModApi(this);
            Monitor.Log($"Helpers initialized", LogLevel.Debug);
            this.helper = helper;

            //read config
            Config = Helper.ReadConfig<ModConfig>();
            if (Config is null || Helper is null)
            {
                return;
            }

            //hook events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunchedAPIs;
            helper.Events.GameLoop.GameLaunched += OnGameLaunchedAddGenericModConfigMenu;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoadedParseCrops;
            helper.Events.GameLoop.SaveLoaded += OnSaveGameLoaded;
            helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
        }

        /// <inheritdoc/>
        public override object GetApi()
        {
            return API;
        }

        /*********
        ** Private methods
        *********/

        private void OnGameLaunchedAPIs(object? sender, GameLaunchedEventArgs? e)
        {
            configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
            {
                Monitor.Log($"Generic Mod Config Menu not found", LogLevel.Debug);
            }
            Utils.Initialize(helper, Monitor);
        }

        private void OnGameLaunchedAddGenericModConfigMenu(object? sender, GameLaunchedEventArgs? e)
        {
            //register config menu if generic mod config menu is installed

            if (configMenu is null)
                return;
            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add keybinding setting
            configMenu.AddKeybind(
                mod: this.ModManifest,
                getValue: () => this.Config.HotKey,
                setValue: value => this.Config.HotKey = value,
                name: () => (this.Helper.Translation.Get("open") + " " + this.Helper.Translation.Get("app-name")).ToString(),
                tooltip: () => this.Helper.Translation.Get("hot-key-tooltip")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("tooltip-delay"),
                tooltip: () => this.Helper.Translation.Get("tooltip-delay-desc"),
                getValue: () => this.Config.ToolTipDelay,
                setValue: value => this.Config.ToolTipDelay = value,
                min: 0,
                max: 1000
            );
        }

        [EventPriority(EventPriority.Low - 9999)]
        private void OnSaveLoadedParseCrops(object? sender, SaveLoadedEventArgs? e)
        {
            ICropParser cropParser = new VanillaCropParser();
            foreach (var crop in cropParser.BuildCrops())
            {
                AddCrop(crop.Key, crop.Value);
            }
        }

        private void OnSaveGameLoaded(object? sender, SaveLoadedEventArgs? e)
        {
            if (Context.IsWorldReady)
                mainMenu = new ProfitCalculatorMainMenu();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs? e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            //check if button pressed is button in config
            if (e.Button == (Config?.HotKey ?? SButton.None))
            {
                //open menu if not already open else close
                if (mainMenu?.IsProfitCalculatorOpen != null && !mainMenu.IsProfitCalculatorOpen)
                {
                    mainMenu.IsProfitCalculatorOpen = true;
                    mainMenu.UpdateMenu();
                    Game1.activeClickableMenu = mainMenu;
                    Game1.playSound("bigSelect");
                }
                else if (mainMenu?.IsProfitCalculatorOpen != null)
                {
                    mainMenu.IsProfitCalculatorOpen = false;
                    mainMenu.UpdateMenu();
                    DropdownOption.ActiveDropdown = null;
                    Game1.activeClickableMenu = null;
                    Game1.playSound("bigDeSelect");
                }
            }
        }

        private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs? e)
        {
            if (e != null)
                DropdownOption.ActiveDropdown?.ReceiveScrollWheelAction(e.Delta);
        }

        /// <summary>
        /// Adds a crop to the Profit Calculator.
        /// </summary>
        /// <param name="id"> The id of the crop. Must be unique.</param>
        /// <param name="crop"> The crop to add. <see cref="Crop"/> </param>
        public void AddCrop(string id, Crop crop)
        {
            Calculator?.AddCrop(id, crop);
        }
    }
}