/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using GenericModConfigMenu;


namespace TemplateMod
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        internal IModHelper MyHelper;

        public String I18nGet(String str)
        {
            return MyHelper.Translation.Get(str);
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MyHelper = helper;

            MyHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            MyHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            MyHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            //Monitor.Log($"MinGameVersion={Constants.MinimumGameVersion}, MaxGameVersion={Constants.MaximumGameVersion}", LogLevel.Info);
        }

        /// <summary>Raised after the game has loaded and all Mods are loaded. Here we load the config.json file and setup GMCM </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = MyHelper.ReadConfig<ModConfig>();

            // use GMCM in an optional manner.

            IGenericModConfigMenuApi gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm != null)
            {
                gmcm.Register(ModManifest,
                              reset: () => Config = new ModConfig(),
                              save: () => Helper.WriteConfig(Config));

                gmcm.AddBoolOption(ModManifest,
                                   () => Config.xxx,
                                   (bool value) => Config.xxx = value,
                                   () => I18nGet("config.Label"),
                                   () => I18nGet("config.Tooltip"));
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.xxx,
                                     (float value) => Config.xxx = value,
                                     () => I18nGet("config.Label"),
                                     () => I18nGet("config.tooltip"),
                                     min: 0.0f,
                                     max: 1.0f,
                                     interval: 0.1f);
            }
            else
            {
                Monitor.LogOnce("Generic Mod Config Menu not available.", LogLevel.Info);
            };
        }

        /// <summary>Raised after a game save is loaded. Here we hook into necessary events for gameplay.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased += Input_ButtonReleased;
            MyHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            MyHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            MyHelper.Events.Player.InventoryChanged += Player_InventoryChanged;
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            MyHelper.Events.Input.ButtonReleased -= Input_ButtonReleased;
            MyHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
            MyHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            MyHelper.Events.Player.InventoryChanged -= Player_InventoryChanged;
        }

        /// <summary>Raised after the player releases a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (SButtonExtensions.IsUseToolButton(e.Button))
            {
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.
        /// This method implements...
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Farmer who = Game1.player;
            bool useToolButtonPressed = SButtonExtensions.IsUseToolButton(e.Button);
            bool actionButtonPressed = SButtonExtensions.IsActionButton(e.Button);

            if (
                (useToolButtonPressed || actionButtonPressed) &&
                (who.CurrentTool != null) &&
                Context.IsPlayerFree
               )
            {
            }
        }

        /// <summary>Called when the player inventory has changed
        /// This method implements...
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
		private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
		{
        }

        /// <summary>Raised when the game state is about to be updated (≈60 times per second).
        /// This method implements... </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_UpdateTicking(object sender, EventArgs e)
        {
        }

        /// <summary>Raised just after the game state is updated (≈60 times per second).
        /// This method implements facing direction change correction.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLoop_UpdateTicked(object sender, EventArgs e)
        {
        }
    }
}

