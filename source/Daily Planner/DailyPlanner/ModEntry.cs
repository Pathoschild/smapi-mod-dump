/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DailyPlanner.Framework;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using DailyPlanner.Framework.API;
using System.IO;

namespace DailyPlanner
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/

        /// <summary>The mod settings.</summary>
        private ModConfig Config { get; set; }

        public Planner Planner;

        private PlannerOverlay Overlay;

        public CheckList CheckList;

        public bool ModEnabled = true;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Display.RenderingHud += this.OnRenderingHud;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet or the mod is disabled
            if (!Context.IsPlayerFree || !this.ModEnabled)
                return;

            // Open window if button is tab button
            if ((e.Button == this.Config.OpenMenuKey || e.Button == this.Config.SecondaryControllerKey) & (Context.IsWorldReady))
            {
                this.OpenMenu();
            }
                
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Overlay = new PlannerOverlay(this, this.Config);
            this.CheckList = new();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // This mod probably doesn't work in multiplayer for non-clients, because it relies on saving data to work.
            // For now, disable the mod for farmhands and send a warning to them.
            this.ModEnabled = Game1.player.IsMainPlayer;
            if (this.ModEnabled) {
                this.Planner = new Planner(Game1.year, this.Helper, this.Monitor);
                this.Planner.CreateDailyPlan();
                this.CheckList = new();
            } else
            {
                this.Monitor.Log("Daily Planner does not currently support multiplayer and is disabled while playing a farmhand.", LogLevel.Warn);
            }
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (this.ModEnabled
                &&this.Config.ShowOverlay 
                && (this.Config.ShowPlannerTasks || this.Config.ShowChecklistTasks)
                && Game1.currentLocation?.currentEvent == null 
                && Game1.farmEvent == null 
                && !Game1.game1.takingMapScreenshot
                ) this.Overlay?.Draw(e.SpriteBatch);
        }

        private void OpenMenu()
        {
            Game1.activeClickableMenu = new PlannerMenu(this.Config.DefaultTab, this.Config, this.Planner, this.CheckList, this.Helper.Translation, this.Monitor);
            Game1.soundBank.PlayCue("bigSelect");
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
                // register mod
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config));

                // add keybind
                configMenu.AddKeybind(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.keybind.name"),
                    tooltip: () => this.Helper.Translation.Get("config.keybind.tooltip"),
                    getValue: () => this.Config.OpenMenuKey,
                    setValue: (SButton val) => this.Config.OpenMenuKey = val);

                // secondary controller keybind

                configMenu.AddKeybind(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.controller_button.name"),
                    tooltip: () => this.Helper.Translation.Get("config.controller_button.tooltip"),
                    getValue: () => this.Config.SecondaryControllerKey,
                    setValue: (SButton val) => this.Config.SecondaryControllerKey = val);

                // show overlay toggle
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.overlay_toggle.name"),
                    tooltip: () => this.Helper.Translation.Get("config.overlay_toggle.tooltip"),
                    getValue: () => this.Config.ShowOverlay,
                    setValue: (bool val) => this.Config.ShowOverlay = val);

                // show planner tasks toggle
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.show_planner_tasks.name"),
                    tooltip: () => this.Helper.Translation.Get("config.show_planner_tasks.tooltip"),
                    getValue: () => this.Config.ShowPlannerTasks,
                    setValue: (bool val) => this.Config.ShowPlannerTasks = val);

                // show checklist tasks toggle
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.show_checklist_tasks.name"),
                    tooltip: () => this.Helper.Translation.Get("config.show_checklist_tasks.tooltip"),
                    getValue: () => this.Config.ShowChecklistTasks,
                    setValue: (bool val) => this.Config.ShowChecklistTasks = val);

                // overlay background opacity
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.background_opacity.name"),
                    tooltip: () => this.Helper.Translation.Get("config.background_opacity.tooltip"),
                    getValue: () => this.Config.OverlayBackgroundOpacity,
                    setValue: (float val) => this.Config.OverlayBackgroundOpacity = val,
                    min: 0.0F,
                    max: 1.0F,
                    interval: 0.1F);

                // overlay text opacity
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.text_opacity.name"),
                    tooltip: () => this.Helper.Translation.Get("config.text_opacity.tooltip"),
                    getValue: () => this.Config.OverlayTextOpacity,
                    setValue: (float val) => this.Config.OverlayTextOpacity = val,
                    min: 0.0F,
                    max: 1.0F,
                    interval: 0.1F);

                // overlay max line count
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.max_lines.name"),
                    tooltip: () => this.Helper.Translation.Get("config.max_lines.tooltip"),
                    getValue: () => this.Config.OverlayMaxLines,
                    setValue: (int val) => this.Config.OverlayMaxLines = val,
                    min: 1,
                    max: 25,
                    interval: 1);

                // overlay max line length
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.max_line_length.name"),
                    tooltip: () => this.Helper.Translation.Get("config.max_line_length.tooltip"),
                    getValue: () => this.Config.OverlayMaxLength,
                    setValue: (int val) => this.Config.OverlayMaxLength = val,
                    min: 10,
                    max: 40,
                    interval: 1);

                // overlay x position
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.x_position.name"),
                    tooltip: () => this.Helper.Translation.Get("config.x_position.tooltip"),
                    getValue: () => this.Config.OverlayXBufferPercent,
                    setValue: (int val) => this.Config.OverlayXBufferPercent = val,
                    min: 0,
                    max: 100,
                    interval: 1);

                // overlay y position
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => this.Helper.Translation.Get("config.y_position.name"),
                    tooltip: () => this.Helper.Translation.Get("config.y_position.tooltip"),
                    getValue: () => this.Config.OverlayYBufferPercent,
                    setValue: (int val) => this.Config.OverlayYBufferPercent = val,
                    min: 0,
                    max: 100,
                    interval: 1);
            }

            // integrate with MobilePhone, if installed
            var phoneApi = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (phoneApi != null)
            {
                Texture2D appIcon = this.Helper.ModContent.Load<Texture2D>(Path.Combine(this.Helper.DirectoryPath, "assets", "app-icon.png"));

                phoneApi.AddApp(Helper.ModRegistry.ModID, "Daily Planner", () => this.OpenMenu(), appIcon);
            }
        }
    }
}
