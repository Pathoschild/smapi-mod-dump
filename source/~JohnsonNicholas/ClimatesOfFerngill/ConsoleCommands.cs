/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Linq;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    internal static class ConsoleCommands
    {

        public static void Init()
        {

        }

#pragma warning disable IDE0060 // Remove unused parameter
        /// <summary>
        /// This function changes the weather (Console Command)
        /// </summary>
        /// <param name="arg1">The command used</param>
        /// <param name="arg2">The console command parameters</param>
        public static void WeatherChangeFromConsole(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            string ChosenWeather = arg2[0];

            switch (ChosenWeather)
            {
                case "rain":
                    WeatherUtilities.SetWeatherRain();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_rain"), LogLevel.Info);
                    break;
                case "testing":
                    WeatherUtilities.SetWeatherTesting();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log("HERE BE KRAKENS!", LogLevel.Info);
                    break;
                case "testing2":
                    WeatherUtilities.SetWeatherTesting2();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log("HERE BE KRAKENS, MATEY! FIRE PORT!", LogLevel.Info);
                    break;
                case "testing3":
                    WeatherUtilities.SetWeatherTesting3();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log("[The Skull Moon Hates You]", LogLevel.Info);
                    break;
                case "sandstorm":
                    WeatherUtilities.SetWeatherSandstorm();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log("Doo doo doo doo doo doo doo doo", LogLevel.Info);
                    break;
                case "vrain":
                    WeatherUtilities.SetWeatherRain();
                    ClimatesOfFerngill.ForceVariableRain();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_rain"), LogLevel.Info);
                    break;
                case "storm":
                    WeatherUtilities.SetWeatherStorm();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_storm"), LogLevel.Info);
                    break;
                case "thunderfrenzy":
                    WeatherUtilities.SetWeatherStorm();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().CreateWeather();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
                case "snow":
                    WeatherUtilities.SetWeatherSnow();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
                case "debris":
                    WeatherUtilities.SetWeatherDebris();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_debris", LogLevel.Info));
                    break;
                case "sunny":
                    WeatherUtilities.SetWeatherSunny();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_sun", LogLevel.Info));
                    break;
				case "overcast":
                    WeatherUtilities.SetWeatherOvercast();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_overcast", LogLevel.Info));
                    break;
                case "blizzard":
                    WeatherUtilities.SetWeatherSnow();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().CreateWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().SetWeatherExpirationTime(new SDVTime(2800));
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
                case "fog":
                    WeatherUtilities.SetWeatherSunny();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().CreateWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(2800));
                    break;
                case "lightfog":
                    WeatherUtilities.SetWeatherSunny();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First() as FerngillFog).CreateWeather(FogType.Light, true);
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(2800));
                    break;
                case "normalfog":
                    WeatherUtilities.SetWeatherSunny();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First() as FerngillFog).CreateWeather(FogType.Normal, true);
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(2800));
                    break;
                case "blindingfog":
                    WeatherUtilities.SetWeatherSunny();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                    (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First() as FerngillFog).CreateWeather(FogType.Blinding, true);
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherBeginTime(new SDVTime(2800));
                    break;
                case "whiteout":
                    WeatherUtilities.SetWeatherSnow();
                    Game1.updateWeatherIcon();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().CreateWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().CreateWeather();
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().SetWeatherBeginTime(new SDVTime(0600));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().SetWeatherExpirationTime(new SDVTime(2800));
                    ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().SetWeatherExpirationTime(new SDVTime(2800));
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset_snow"), LogLevel.Info);
                    break;
            }

            Game1.updateWeatherIcon();
            ClimatesOfFerngill.Conditions.SetTodayWeather();
            ClimatesOfFerngill.Conditions.GenerateWeatherSync();
        }

#pragma warning disable IDE0060 // Remove unused parameter
        /// <summary>
        /// This function changes the weather for tomorrow (Console Command)
        /// </summary>
        /// <param name="arg1">The command used</param>
        /// <param name="arg2">The console command parameters</param>
        public static void TomorrowWeatherChangeFromConsole(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            string chosenWeather = arg2[0];
            switch (chosenWeather)
            {
                case "rain":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_rain;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwrain"), LogLevel.Info);
                    break;
                case "storm":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_lightning;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwstorm"), LogLevel.Info);
                    break;
                case "snow":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_snow;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwsnow"), LogLevel.Info);
                    break;
                case "debris":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_debris;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwdebris"), LogLevel.Info);
                    break;
                case "festival":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_festival;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwfestival"), LogLevel.Info);
                    break;
                case "sun":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_sunny;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwsun"), LogLevel.Info);
                    break;
                case "wedding":
                    Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = Game1.weather_wedding;
                    ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Translator.Get("console-text.weatherset-tmrwwedding"), LogLevel.Info);
                    break;
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        internal static void DisplayRainTotal(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
        {
            ClimatesOfFerngill.Logger.Log($"Rain Totals: Today: {ClimatesOfFerngill.Conditions.TodayRain}, Starting: {ClimatesOfFerngill.Conditions.StartingRain.ToString()}");
            ClimatesOfFerngill.Logger.Log($"Savefile Totals: {ClimatesOfFerngill.Conditions.trackerModel.AmtOfRainSinceDay1}");
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        internal static void ShowWind(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
        {
            ClimatesOfFerngill.Logger.Log($"Wind status: Global Wind is {WeatherDebris.globalWind}, and wind gusts are {Game1.windGust}");
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        internal static void ResetGlobalWind(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
        {
            WeatherDebris.globalWind = 0f;
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        public static void ClearSpecial(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
        {
            ClimatesOfFerngill.Conditions.ClearAllSpecialWeather();
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void SetRainAmt(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            int rainAmt = Convert.ToInt32(arg2[0]);

            Array.Resize(ref Game1.rainDrops, rainAmt);
            ClimatesOfFerngill.Conditions.SetRainAmt(rainAmt);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void SetRainDef(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            float x = Convert.ToInt32(arg2[0]);
            float y = Convert.ToInt32(arg2[1]);

            ClimatesOfFerngill.RainX = x;
            ClimatesOfFerngill.RainY = y;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void SetWindChance(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            ClimatesOfFerngill.WindChance = (float)Convert.ToDouble(arg2[0]);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void SetWindThreshold(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            ClimatesOfFerngill.WindThreshold = (float)Convert.ToDouble(arg2[0]);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void SetWindRange(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!Context.IsMainPlayer) return;

            if (arg2.Length < 1)
                return;

            float x = (float)Convert.ToDouble(arg2[0]);
            float y = (float)Convert.ToDouble(arg2[1]);

            ClimatesOfFerngill.WindMin = x;
            ClimatesOfFerngill.WindCap = y;
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        public static void OutputWeather(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var retString = $"Weather for {SDate.Now()} is {ClimatesOfFerngill.Conditions.ToString()}. {Environment.NewLine} System flags: isRaining {Game1.isRaining} isSnowing {Game1.isSnowing} isDebrisWeather: {Game1.isDebrisWeather} isLightning {Game1.isLightning}, with tommorow's set weather being {Game1.weatherForTomorrow}";
            ClimatesOfFerngill.Logger.Log(retString);
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        internal static void DisplayClimateTrackerData(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
		{
			ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Conditions.trackerModel.ToString());
		}

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        internal static void ShowSpecialWeather(string arg1, string[] arg2)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0060 // Remove unused parameter
        {
           ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Conditions.PrintWeather());
           ClimatesOfFerngill.Logger.Log(ClimatesOfFerngill.Conditions.GetCurrentConditions().ToString());
        }
    }
}
