/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/WinterRain
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System;

namespace WinterRain
{
    public class ModEntry : Mod
    {
        private ModConfig config;
        private int[] snowDuration;
        private bool isSnowDuration;
        private double randomDouble;

        public override void Entry(IModHelper helper)
        {
            // read config
            config = helper.ReadConfig<ModConfig>();
            snowDuration = config.SnowDuration;

            if (!Verification())
                return;

            // event methods
            helper.Events.GameLoop.Saving += GameLoop_Saving;
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            randomDouble = Game1.random.NextDouble();
            isSnowDuration = IsSnowDuration(Game1.season, Game1.dayOfMonth);
            SetWeather(isSnowDuration);
        }

        private bool Verification()
        {
            // snowDuration
            Array.Sort(snowDuration);
            if ((int)snowDuration.GetValue(0) < 1 || (int)snowDuration.GetValue(1) > 28)
            {
                Monitor.Log("\"snowDuration\" have invalid value! Use natural number 1~28 please.", LogLevel.Error);
                return false;
            }

            // chance (for duration)
            if (config.ChanceToRain_duration < 0.0 || config.ChanceToRain_duration > 1.0)
            {
                Monitor.Log("\"chanceToRain_duration\" have invalid value! Use real number between 0.0 and 1.0 please.", LogLevel.Error);
                return false;
            }
            else if (config.ChanceToSnow_duration < 0.0 || config.ChanceToSnow_duration > 1.0)
            {
                Monitor.Log("\"chanceToSnow_duration\" have invalid value! Use real number between 0.0 and 1.0 please.", LogLevel.Error);
                return false;
            }

            // chance (for not duration)
            if (config.ChanceToRain_notDuration + config.ChanceToSnow_notDuration > 1.0)
            {
                Monitor.Log("The sum of \"ChanceToRain_notDuration\" and \"ChanceToSnow_notDuration\" exceeds 1.0!", LogLevel.Error);
                return false;
            }
            return true;
        }

        private bool IsSnowDuration(Season season, int day)
        {

            // fall 28 + winter
            if (season == StardewValley.Season.Winter)
            {
                if ((int)snowDuration.GetValue(0) > (day + 1) || (day + 1) > (int)snowDuration.GetValue(1))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (season == StardewValley.Season.Fall && day == 28)
            {
                if ((int)snowDuration.GetValue(0) != 1) // in fall 28, if tomorrow is not snowDuration
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else // other seasons
            {
                return false;
            }
        }

        private void SetWeather(bool isSnowDuration)
        {
            if (Game1.season != Season.Winter)
                return;

            if (config.UseSnowDuration)
            {
                if (isSnowDuration)
                {
                    if (randomDouble < config.ChanceToSnow_duration)
                    {
                        Game1.weatherForTomorrow = "Snow";
                    }
                    else
                    {
                        Game1.weatherForTomorrow = "Sun";
                    }
                }
                else
                {
                    if (randomDouble < config.ChanceToRain_duration)
                    {
                        Game1.weatherForTomorrow = "Rain";
                    }
                    else
                    {
                        Game1.weatherForTomorrow = "Sun";
                    }
                }
            }
            // !useSnowDuration
            else
            {
                // 0 ~ rain chance
                if (randomDouble < config.ChanceToRain_notDuration)
                {
                    Game1.weatherForTomorrow = "Rain";
                }
                // rain chance ~ (rain chance + snow chance) -> Section as much as snow chance
                else if (randomDouble > config.ChanceToRain_notDuration && randomDouble < config.ChanceToRain_notDuration + config.ChanceToSnow_notDuration)
                {
                    Game1.weatherForTomorrow = "Snow";
                }
                else
                {
                    Game1.weatherForTomorrow = "Sun";
                }
            }
        }

    }
}
