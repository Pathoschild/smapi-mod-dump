using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley;
using System.Threading.Tasks;
using System.Text;

namespace Forecaster
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private string luckForecast;
        private string weatherForecast;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            if (this.Config.enableShortcutKeys) {
                InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private async void TimeEvents_AfterDayStarted(object sender, EventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            luckForecast = this.Helper.Reflection
                            .GetMethod(new TV(), "getFortuneForecast")
                            .Invoke<string>();
            weatherForecast = this.Helper.Reflection
                               .GetMethod(new TV(), "getWeatherForecast")
                               .Invoke<string>();

            await Task.Delay(this.Config.initialDelay * 1000);
            this.showWeather();

            await Task.Delay(this.Config.offsetDelay * 1000);
            this.showTip();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.Config.tipKey == e.Button) {
                showTip();
            } else if (this.Config.weatherKey == e.Button) {
                showWeather();
            }
        }

        private void showTip() {
            Game1.addHUDMessage(new HUDMessage(luckForecast, 2));
        }

        private void showWeather() {
            Game1.addHUDMessage(new HUDMessage(weatherForecast, 2));
        }
  
    }
}