using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley;
using System.Threading.Tasks;

namespace Forecaster
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
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

            string luckForecast = this.Helper.Reflection
                               .GetMethod(new TV(), "getFortuneForecast")
                               .Invoke<string>();
            string weatherForecast = this.Helper.Reflection
                               .GetMethod(new TV(), "getWeatherForecast")
                               .Invoke<string>();

            await Task.Delay(this.Config.initialDelay * 1000);
            Game1.addHUDMessage(new HUDMessage(weatherForecast, 2));

            await Task.Delay(this.Config.offsetDelay * 1000);
            Game1.addHUDMessage(new HUDMessage(luckForecast, 2));

        }
    }
}