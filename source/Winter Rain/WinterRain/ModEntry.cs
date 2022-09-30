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
            snowDuration = config.snowDuration;

            if (!verification())
                return;

            // event methods
            helper.Events.GameLoop.Saving += GameLoop_Saving;
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            randomDouble = Game1.random.NextDouble();

            // fall 28 + winter
            if (Game1.currentSeason == "winter")
            {
                if ((int)snowDuration.GetValue(0) > (Game1.dayOfMonth + 1) || (Game1.dayOfMonth + 1) > (int)snowDuration.GetValue(1))
                {
                    isSnowDuration = false;
                }
                else
                {
                    isSnowDuration = true;
                }
            }
            else if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 28)
            {
                if ((int)snowDuration.GetValue(0) != 1) // in fall 28, if tomorrow is not snowDuration
                {
                    isSnowDuration = false;
                }
                else
                {
                    isSnowDuration = true;
                }
            }
            else
            {
                isSnowDuration = false;
                return;
            }

            if (config.useSnowDuration)
            {
                // weather re-set, because 63% the default value is too high.
                if (isSnowDuration)
                {
                    if (randomDouble < config.chanceToSnow)
                    {
                        Game1.weatherForTomorrow = 5;
                    }
                    else
                    {
                        Game1.weatherForTomorrow = 0;
                    }
                }
                else
                {
                    if (randomDouble < config.chanceToRain)
                    {
                        Game1.weatherForTomorrow = 1;
                    }
                    else
                    {
                        Game1.weatherForTomorrow = 0;
                    }
                }
            }
            // !useSnowDuration
            else
            {
                if (config.chanceToRain < config.chanceToSnow)
                {
                    // rain -> snow
                    if (randomDouble < config.chanceToRain)
                    {
                        Game1.weatherForTomorrow = 1;
                    }
                    else if (randomDouble < config.chanceToSnow)
                    {
                        Game1.weatherForTomorrow = 5;
                    }
                    else
                    {
                        Game1.weatherForTomorrow = 0;
                    }
                }
                else // chanceToSnow <= chanceToRain
                {
                    // snow -> rain
                    if (randomDouble < config.chanceToSnow)
                    {
                        Game1.weatherForTomorrow = 5;
                    }
                    else if (randomDouble < config.chanceToRain)
                    {
                        Game1.weatherForTomorrow = 1;
                    }
                    else
                    {
                        Game1.weatherForTomorrow = 0;
                    }
                }
            }
        }

        private bool verification()
        {
            
            // snowDuration sort
            if (snowDuration[0] > snowDuration[1])
            {
                (snowDuration[1], snowDuration[0]) = (snowDuration[0], snowDuration[1]);
            }

            Array.Sort(snowDuration);
            if ((int)snowDuration.GetValue(0) < 1 || (int)snowDuration.GetValue(1) > 28)
            {
                Monitor.Log("\"snowDuration\" have invalid value! Use natural number 1~28 please.", LogLevel.Error);
                return false;
            }
            else if (0.0 > config.chanceToRain || 1.0 < config.chanceToRain)
            {
                Monitor.Log("\"chanceToRain\" have invalid value! Use real number between 0.0 and 1.0 please.\"", LogLevel.Error);
                return false;
            }
            else if (0.0 > config.chanceToSnow || 1.0 < config.chanceToSnow)
            {
                Monitor.Log("\"chanceToSnow\" have invalid value! Use real number between 0.0 and 1.0 please.\"", LogLevel.Error);
                return false;
            }
            else
                return true;
        }
    }
}
