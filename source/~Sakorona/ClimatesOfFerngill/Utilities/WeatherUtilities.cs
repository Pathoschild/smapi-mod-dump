using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    internal static class WeatherUtilities
    {
        internal static int MaxRain = 4000;

        internal static bool IsSevereRainFall(int rainAmt)
        {
            if (GetCategory(rainAmt) == RainLevels.Torrential || GetCategory(rainAmt) == RainLevels.Typhoon || 
				GetCategory(rainAmt) == RainLevels.NoahsFlood || GetCategory(rainAmt) == RainLevels.Severe)
                return true;

            return false;
        }

        internal static int ReturnMidPoint(RainLevels level)
        {
            switch (level)
            {
                case RainLevels.None:
                    return 0;
                case RainLevels.Sunshower:
                    return 9;
                case RainLevels.Light:
                    return 35;
                case RainLevels.Normal:
                    return 70;
                case RainLevels.Moderate:
                    return 140;
                case RainLevels.Heavy:
                    return 280;
                case RainLevels.Severe:
                    return 560;
                case RainLevels.Torrential:
                    return 1120;
                case RainLevels.Typhoon:
                    return 2240;
                case RainLevels.NoahsFlood:
                    return 3680;
                default:
                    return 0;
            }
        }

        internal static int ReturnRndRainAmtInLevel(MersenneTwister dice, RainLevels rl)
        {
            if (rl == RainLevels.Sunshower)
                return dice.Next(0, (ReturnMidPoint(RainLevels.Light) / 2));
            else if (rl == RainLevels.Light)
                return dice.Next((ReturnMidPoint(RainLevels.Light) / 2), (ReturnMidPoint(RainLevels.Normal) / 2));
            else if (rl == RainLevels.Normal)
                return dice.Next((ReturnMidPoint(RainLevels.Normal) / 2), (ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)));
            else if (rl == RainLevels.Moderate)
                return dice.Next((ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)), (ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Normal)));
            else if (rl == RainLevels.Heavy)
                return dice.Next((ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Normal)), (ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Heavy)));
            else if (rl == RainLevels.Severe)
                return dice.Next((ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Heavy)), (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)));
            else if (rl == RainLevels.Torrential)
                return dice.Next((ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)), (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Torrential)));
            else if (rl == RainLevels.Typhoon)
                return dice.Next((ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Torrential)), (ReturnMidPoint(RainLevels.Typhoon) + ReturnMidPoint(RainLevels.Torrential)));
            else if (rl == RainLevels.NoahsFlood)
                return dice.Next((ReturnMidPoint(RainLevels.Typhoon) + ReturnMidPoint(RainLevels.Torrential)), MaxRain);


            return dice.Next((ReturnMidPoint(RainLevels.Normal) / 2), (ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)));
        }

        internal static double GetStepChance(int rain, bool increase = true)
        {
            double multiplier = ClimatesOfFerngill.WeatherOpt.VRMassiveStepChance;
            //directions - 0 down 1 up
            {
                if (increase)
                {
                    if (rain > 0 && rain <= ReturnMidPoint(RainLevels.Sunshower))
                        multiplier *= 5.4;
                    if (rain > ReturnMidPoint(RainLevels.Sunshower) && rain <= ReturnMidPoint(RainLevels.Light) / 2)
                        multiplier *= 3.2;
                    else if (rain > (ReturnMidPoint(RainLevels.Light) / 2) && rain <= (ReturnMidPoint(RainLevels.Light)))
                        multiplier *= 1.8;
                    else if (rain > (ReturnMidPoint(RainLevels.Light)) && rain <= (ReturnMidPoint(RainLevels.Normal) / 2))
                        multiplier *= 1.5;
                    else if (rain > (ReturnMidPoint(RainLevels.Normal) / 2) && rain <= (ReturnMidPoint(RainLevels.Normal)))
                        multiplier *= 1.4;
                    else if (rain > (ReturnMidPoint(RainLevels.Normal)) && rain <= (ReturnMidPoint(RainLevels.Heavy)))
                        multiplier /= 1.1;
                    else if (rain > (ReturnMidPoint(RainLevels.Heavy)) && rain <= (ReturnMidPoint(RainLevels.Severe)))
                        multiplier /= 1.65;
                    else if (rain > (ReturnMidPoint(RainLevels.Severe)) && rain <= (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)))
                        multiplier /= 1.95;
                    else if (rain > (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)) && rain <= (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)))
                        multiplier /= 2.25;
                    else if (rain > (ReturnMidPoint(RainLevels.Typhoon)) && rain <= (ReturnMidPoint(RainLevels.Typhoon) + ReturnMidPoint(RainLevels.Torrential)))
                        multiplier /= 3.15;
                    else if (rain > (ReturnMidPoint(RainLevels.Typhoon) + ReturnMidPoint(RainLevels.Torrential)) && rain <= (ReturnMidPoint(RainLevels.NoahsFlood)))
                        multiplier /= 3.5;
                    else if (rain > (ReturnMidPoint(RainLevels.NoahsFlood)) && rain <= MaxRain)
                        multiplier /= 5.4;
                }
                if (!increase)
                {
                    if (rain > 0 && rain <= ReturnMidPoint(RainLevels.Sunshower))
                        multiplier /= 5.4;
                    if (rain > ReturnMidPoint(RainLevels.Sunshower) && rain <= ReturnMidPoint(RainLevels.Light) / 2)
                        multiplier /= 3.2;
                    else if (rain > (ReturnMidPoint(RainLevels.Light) / 2) && rain <= (ReturnMidPoint(RainLevels.Light)))
                        multiplier /= 1.8;
                    else if (rain > (ReturnMidPoint(RainLevels.Light)) && rain <= (ReturnMidPoint(RainLevels.Normal) / 2))
                        multiplier /= 1.5;
                    else if (rain > (ReturnMidPoint(RainLevels.Normal) / 2) && rain <= (ReturnMidPoint(RainLevels.Normal)))
                        multiplier /= 1.4;
                    else if (rain > (ReturnMidPoint(RainLevels.Normal)) && rain <= (ReturnMidPoint(RainLevels.Heavy)))
                        multiplier *= 1.1;
                    else if (rain > (ReturnMidPoint(RainLevels.Heavy)) && rain <= (ReturnMidPoint(RainLevels.Severe)))
                        multiplier *= 1.65;
                    else if (rain > (ReturnMidPoint(RainLevels.Severe)) && rain <= (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)))
                        multiplier *= 1.95;
                    else if (rain > (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)) && rain <= (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)))
                        multiplier *= 2.15;
                    else if (rain > (ReturnMidPoint(RainLevels.Typhoon)) && rain <= (ReturnMidPoint(RainLevels.Typhoon) + ReturnMidPoint(RainLevels.Torrential)))
                        multiplier *= 3.15;
                    else if (rain > (ReturnMidPoint(RainLevels.Typhoon) + ReturnMidPoint(RainLevels.Torrential)) && rain <= (ReturnMidPoint(RainLevels.NoahsFlood)))
                        multiplier *= 3.5;
                    else if (rain > (ReturnMidPoint(RainLevels.NoahsFlood)) && rain <= MaxRain)
                        multiplier *= 5.4;
                }
            }
            multiplier *= 1.18;

            return multiplier;
        }


        internal static RainLevels GetCategory(int rain)
        {
            if (rain == 0)
                return RainLevels.None;
            if (rain < ReturnMidPoint(RainLevels.Light) / 2)
                return RainLevels.Sunshower;
            else if (rain >= ReturnMidPoint(RainLevels.Light) / 2 && rain < (ReturnMidPoint(RainLevels.Normal) / 2))
                return RainLevels.Light;
            else if (rain >= (ReturnMidPoint(RainLevels.Normal) / 2) && rain < (ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)))
                return RainLevels.Normal;
            else if (rain >= (ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)) &&
                rain < (ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Normal)))
                return RainLevels.Moderate;
            else if (rain >= (ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Normal)) &&
                rain < (ReturnMidPoint(RainLevels.Heavy) + ReturnMidPoint(RainLevels.Moderate)))
                return RainLevels.Heavy;
            else if (rain >= (ReturnMidPoint(RainLevels.Heavy) + ReturnMidPoint(RainLevels.Moderate)) &&
                rain < (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)))
                return RainLevels.Severe;
            else if (rain >= (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)) &&
                rain < (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)))
                return RainLevels.Torrential;
            else if (rain >= (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Torrential)) &&
                rain < (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Typhoon)))
                return RainLevels.Typhoon;
            else if (rain >= (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)))
                return RainLevels.NoahsFlood;

            throw new System.Exception($"Rain is {rain}, reached point in execution it shouldn't reach.");
        }

        internal static string DescCategory(int rain)
        {
            if (rain == 0)
                return ClimatesOfFerngill.Translator.Get("category-none");
            if (rain < ReturnMidPoint(RainLevels.Light) / 2)
                return ClimatesOfFerngill.Translator.Get("category-sunshower");
            else if (rain >= ReturnMidPoint(RainLevels.Light) / 2 && rain < (ReturnMidPoint(RainLevels.Normal) / 2))
                return ClimatesOfFerngill.Translator.Get("category-light");
            else if (rain >= (ReturnMidPoint(RainLevels.Normal) / 2) && rain < (ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)))
                return ClimatesOfFerngill.Translator.Get("category-normal");
            else if (rain >= (ReturnMidPoint(RainLevels.Normal) + ReturnMidPoint(RainLevels.Light)) &&
                rain < (ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Normal)))
                return ClimatesOfFerngill.Translator.Get("category-moderate");
            else if (rain >= (ReturnMidPoint(RainLevels.Moderate) + ReturnMidPoint(RainLevels.Normal)) &&
                rain < (ReturnMidPoint(RainLevels.Heavy) + ReturnMidPoint(RainLevels.Moderate)))
                return ClimatesOfFerngill.Translator.Get("category-heavy");
            else if (rain >= (ReturnMidPoint(RainLevels.Heavy) + ReturnMidPoint(RainLevels.Moderate)) &&
                rain < (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)))
                return ClimatesOfFerngill.Translator.Get("category-severe");
            else if (rain >= (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Heavy)) &&
                rain < (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)))
                return ClimatesOfFerngill.Translator.Get("category-torrential");
            else if (rain >= (ReturnMidPoint(RainLevels.Severe) + ReturnMidPoint(RainLevels.Torrential)) &&
                rain < (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Typhoon)))
                return ClimatesOfFerngill.Translator.Get("category-typhoon");
            else if (rain >= (ReturnMidPoint(RainLevels.Torrential) + ReturnMidPoint(RainLevels.Severe)))
                return ClimatesOfFerngill.Translator.Get("category-noahsflood");

            throw new System.Exception($"Rain is {rain}, reached point in execution it shouldn't reach.");
        }

        private static readonly Dictionary<SDate, int> _forceDays = new Dictionary<SDate, int>
        {
            { new SDate(1,"spring"), Game1.weather_sunny },
            { new SDate(2, "spring"), Game1.weather_sunny },
            { new SDate(3, "spring"), Game1.weather_rain },
            { new SDate(4, "spring"), Game1.weather_sunny },
            { new SDate(13, "spring"), Game1.weather_festival },
            { new SDate(24, "spring"), Game1.weather_festival },
            { new SDate(1, "summer"), Game1.weather_sunny },
            { new SDate(11, "summer"), Game1.weather_festival },
            { new SDate(13, "summer"), Game1.weather_lightning},
            { new SDate(26, "summer"), Game1.weather_lightning },
            { new SDate(28, "summer"), Game1.weather_festival },
            { new SDate(1,"fall"), Game1.weather_sunny },
            { new SDate(16,"fall"), Game1.weather_festival },
            { new SDate(27,"fall"), Game1.weather_festival },
            { new SDate(1,"winter"), Game1.weather_sunny },
            { new SDate(8, "winter"), Game1.weather_festival },
            { new SDate(14, "winter"), Game1.weather_festival },
            { new SDate(15, "winter"), Game1.weather_festival },
            { new SDate(16, "winter"), Game1.weather_festival },
            { new SDate(25, "winter"), Game1.weather_festival }
        };

        public static bool CheckForForceDay(Descriptions Desc, SDate Target, IMonitor mon, bool verbose)
        {
            foreach (KeyValuePair<SDate, int> entry in _forceDays)
            {
                if (entry.Key.Day == Target.Day && entry.Key.Season == Target.Season)
                {
                    if (verbose) mon.Log($"Setting a forced value for tommorow: {Desc.DescribeInGameWeather(entry.Value)} for {entry.Key.Season} {entry.Key.Day}");
                    Game1.weatherForTomorrow = entry.Value;
                    Game1.netWorldState.Value.WeatherForTomorrow = entry.Value;
                    return true;
                }
            }
            return false;
        }

        internal static void UpdateFurniture(DecoratableLocation l)
        {
            foreach (Furniture f in l.furniture)
                f.minutesElapsed(0, l);
        }

        internal static void SetWeatherRain()
        {
            Game1.isSnowing = Game1.isLightning = Game1.isDebrisWeather = false;
            Game1.isRaining = true;
            ClimatesOfFerngill.Conditions.RemoveWeather(CurrentWeather.Overcast);
            Game1.debrisWeather.Clear();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().EndWeather();
            ClimatesOfFerngill.Conditions.SetVariableRain(false);
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static void SetWeatherOvercast(bool OnNewDay = false)
        {
            Game1.isLightning = Game1.isSnowing = Game1.isDebrisWeather = false;
            Game1.isRaining = false;
            if (!OnNewDay)
            {
                ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
                ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
                ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
                Game1.debrisWeather.Clear();
            }

            ClimatesOfFerngill.Logger.Log("Resizing rain array - settting weather to overcast");
            Array.Resize(ref Game1.rainDrops, 0);
            ClimatesOfFerngill.Conditions.AddWeather(CurrentWeather.Overcast);
            ClimatesOfFerngill.Conditions.RemoveWeather(CurrentWeather.Rain);
            ClimatesOfFerngill.Conditions.SetRainAmt(0);
            ClimatesOfFerngill.Conditions.SetVariableRain(false);
            ClimatesOfFerngill.Conditions.IsOvercast = true;
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }


        internal static void SetWeatherStorm()
        {
            Game1.isSnowing = Game1.isDebrisWeather = false;
            Game1.isLightning = Game1.isRaining = true;
            Game1.debrisWeather.Clear();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static void SetWeatherSnow()
        {
            Game1.isRaining = Game1.isLightning = Game1.isDebrisWeather = false;
            Game1.isSnowing = true;
            Game1.debrisWeather.Clear();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().EndWeather();
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static void SetWeatherDebris()
        {
            Game1.isSnowing = Game1.isLightning = Game1.isRaining = false;
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
            Game1.isDebrisWeather = true;
            Game1.populateDebrisWeatherArray();
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static void SetWeatherSunny()
        {
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().EndWeather();
            Game1.debrisWeather.Clear();
            Game1.isSnowing = Game1.isLightning = Game1.isRaining = Game1.isDebrisWeather = false;
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static int GetWeatherCode()
        {
            if (!Game1.isRaining)
            {
                if (Game1.isDebrisWeather) return Game1.weather_debris;
                else if (Game1.isSnowing) return Game1.weather_snow;
                else return Game1.weather_sunny;
            }
            else
            {
                if (Game1.isLightning) return Game1.weather_lightning;
                else
                    return Game1.weather_rain;
            }
        }
    }
}
