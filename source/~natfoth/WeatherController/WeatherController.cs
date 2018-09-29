using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace WeatherController
{
    public class WeatherControllerMod : Mod
    {
        public override string Name
        {
            get { return "Weather Controller"; }
        }

        public override string Authour
        {
            get { return "Natfoth"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string Description
        {
            get { return "Registers several commands to use for controlling the weather."; }
        }

        public override void Entry()
        {
            RegisterCommands();
            Events.UpdateTick += Events_UpdateTick;
        }

        void Events_UpdateTick()
        {
            if (Game1.player == null)
                return;
        }

        public static void RegisterCommands()
        {
            Command.RegisterCommand("weather_setweather", "Sets the current Weather. \"rain,storm,snow,debris,sun\"").CommandFired += weather_setWeather;
            Command.RegisterCommand("weather_setweathertom", "Sets the Weather for Tomorrow. \"rain,storm,snow,debris,festival,wedding,sun\"").CommandFired += weather_setWeatherTomorrow;
        }

        static void weather_setWeather(Command cmd)
        {
            if (cmd.CalledArgs.Length != 1)
            {
                Program.LogObjectInvalid();
            }
            else
            {
                string obj = cmd.CalledArgs[0];
                string[] objs = "rain,storm,snow,debris,sun".Split(new[] {','});
                if (objs.Contains(obj))
                {
                    Game1.isSnowing = false;
                    Game1.isRaining = false;
                    Game1.isLightning = false;
                    Game1.isDebrisWeather = false;

                    switch (obj)
                    {
                        case "rain":
                            Game1.isRaining = true;
                            break;
                        case "storm":
                            Game1.isRaining = true;
                            Game1.isLightning = true;
                            break;
                        case "snow":
                            Game1.isSnowing = true;
                            break;
                        case "debris":
                            Game1.isDebrisWeather = true;
                            break;
                    }

                    Game1.updateWeatherIcon();
                }
                else
                {
                    Program.LogObjectInvalid();
                }

            }
        }

        static void weather_setWeatherTomorrow(Command cmd)
        {
            if (cmd.CalledArgs.Length != 1)
            {
                Program.LogObjectInvalid();
            }
            else
            {
                string obj = cmd.CalledArgs[0];
                string[] objs = "rain,storm,snow,debris,festival,wedding,sun".Split(new[] { ',' });
                if (objs.Contains(obj))
                {
                    switch (obj)
                    {
                        case "rain":
                            Game1.weatherForTomorrow = Game1.weather_rain;
                            break;
                        case "storm":
                            Game1.weatherForTomorrow = Game1.weather_lightning;
                            break;
                        case "snow":
                            Game1.weatherForTomorrow = Game1.weather_snow;
                            break;
                        case "debris":
                            Game1.weatherForTomorrow = Game1.weather_debris;
                            break;
                        case "festival":
                            Game1.weatherForTomorrow = Game1.weather_festival;
                            break;
                        case "sun":
                            Game1.weatherForTomorrow = Game1.weather_sunny;
                            break;
                        case "wedding":
                            Game1.weatherForTomorrow = Game1.weather_wedding;
                            break;
                    }

                    Game1.updateWeatherIcon();
                }
                else
                {
                    Program.LogObjectInvalid();
                }

            }
        }
    }
}
