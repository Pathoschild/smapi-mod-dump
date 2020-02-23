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
        private ModConfig _config;
        private string _luckForecast;
        private string _weatherForecast;
        private TV _television;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            _config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            if (_config.EnableShortcutKeys)
                helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private async void OnDayStarted(object sender, DayStartedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            
            _television = new TV();
            _luckForecast = null;
            _weatherForecast = null;

            if (_config.ShowWeatherOnWakeUp) {
                await Task.Delay(_config.InitialDelay * 1000);
                ShowWeather();
            }

            if (_config.ShowLuckForecastOnWakeUp) {
                await Task.Delay(_config.OffsetDelay * 1000);
                ShowLuckForecast();
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (_config.TipKey == e.Button) {
                ShowLuckForecast();
            } else if (_config.WeatherKey == e.Button) {
                ShowWeather();
            }
        }

        private void ShowLuckForecast() {
            if (_luckForecast == null) {
                _luckForecast = Helper.Reflection
                                .GetMethod(_television, "getFortuneForecast")
                                .Invoke<string>();
            }

            Game1.addHUDMessage(new HUDMessage(_luckForecast, 2));
        }

        private void ShowWeather() {
            if (_weatherForecast == null) {
                _weatherForecast = Helper.Reflection
                                .GetMethod(_television, "getWeatherForecast")
                                .Invoke<string>();
            }

            Game1.addHUDMessage(new HUDMessage(_weatherForecast, 2));
        }
  
    }
}