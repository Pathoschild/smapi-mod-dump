using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weatherGod.Helpers;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace weatherGod
{
    public class ModEntry : Mod
    {
        private ModConfig _config;
        int Counter;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameStart;
            helper.Events.Input.ButtonPressed += ButtonPressed;

        }


        /*********
        ** Private methods
        *********/
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
        }

        private void GameStart(object sender, GameLaunchedEventArgs e)
        {
            this._config = this.Helper.ReadConfig<ModConfig>();
            try
            {
                UseGMCM();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Hey, you need to install GenericModConfigMenuAPI", LogLevel.Debug);
            }
        }

        private void UseGMCM()
        {
           GenericModConfigMenuAPI configMenuApi =
                Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            configMenuApi.RegisterModConfig(ModManifest,
                () => _config = new ModConfig(),
                () => Helper.WriteConfig(_config));

            configMenuApi.RegisterSimpleOption(ModManifest, "Weather Change Key",
                "Button used to change weather",
                () => _config.Keybind,
                (button) => _config.Keybind = button);
                
        }

        private void ButtonPressed(object o, ButtonPressedEventArgs button)
        {
            SButton b = button.Button;
           string[] Weathers = { "rain", "storm", "snow", "debris", "sun" };
            if (b == _config.Keybind)
            {
               Counter += 1;
                for (int i = 0; i < Weathers.Length; i++)
                {
                    if (Counter == Weathers.Length)
                        Counter = 0;
                }
                
                Game1.isRaining = false;
                Game1.isLightning = false;
                Game1.isSnowing = false;
                Game1.isDebrisWeather = false;

                switch (Counter)
                {
                    case 0:
                        Monitor.Log($"ok counter is {Counter}, case 0", LogLevel.Debug);
                        Game1.weatherIcon = 2;
                        break;
                    case 1:
                        Monitor.Log($"ok counter is {Counter}, case 1 - rain", LogLevel.Debug);
                        Game1.isRaining = true;
                        Game1.weatherIcon = 4;
                        break;
                    case 2:
                        Monitor.Log($"ok counter is {Counter}, case 2 - storm", LogLevel.Debug);
                        Game1.isRaining = true;
                        Game1.isLightning = true;
                        Game1.weatherIcon = 5;
                        break;
                    case 3:
                        Monitor.Log($"ok counter is {Counter}, case 3- snow", LogLevel.Debug);
                        Game1.isSnowing = true;
                        Game1.weatherIcon = 7;
                        break;
                    case 4:
                        Monitor.Log($"ok counter is {Counter}, case 4 - debris", LogLevel.Debug);
                        Game1.isDebrisWeather = true;
                        Game1.weatherIcon = 3;
                        break;

                }

            }
        }

    }
}
