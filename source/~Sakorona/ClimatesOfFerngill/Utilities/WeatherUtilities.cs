/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

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
        internal static readonly Dictionary<RainLevels, StaticRange> RainCategories = new Dictionary<RainLevels, StaticRange>{
                { RainLevels.None,new StaticRange(0, 0, 0)},
                { RainLevels.Sunshower, new StaticRange(0, 17.5, 9)},
                { RainLevels.Light, new StaticRange(17.5, 52.5, 35)},
                { RainLevels.Normal, new StaticRange(52.5, 105, 70)},
                { RainLevels.Moderate, new StaticRange(105, 210, 140)},
                { RainLevels.Heavy, new StaticRange(210, 420, 280)},
                { RainLevels.Severe, new StaticRange(420, 840, 560)},
                { RainLevels.Torrential, new StaticRange(840, 2520, 1680)},
                { RainLevels.Typhoon, new StaticRange(2520, 7560, 5040)},
                { RainLevels.NoahsFlood, new StaticRange(7560, 15120, 30240)},
        };    

        internal static int GetRainCategoryMidPoint(RainLevels level) => (int)Math.Round(RainCategories[level].MidPoint,0);
        internal static int GetRainCategoryUpperBound(RainLevels level) => (int) Math.Round(RainCategories[level].UpperBound,0);
        internal static int GetRainCategoryLowerBound(RainLevels level) => (int)Math.Round(RainCategories[level].LowerBound, 0);
        
        internal static RainLevels GetRainCategory(int rain)
        {
           foreach (var rl in RainCategories)
		   {
                if (rl.Value.IsWithinFullRange(rain))
                {
                    return rl.Key;
                }
		   }

           throw new System.Exception($"Rain is {rain}, reached point in execution it shouldn't reach.");
        }

        internal static bool IsSevereRainFall(int rainAmt)
        {
			if (GetRainCategory(rainAmt) == RainLevels.Torrential || GetRainCategory(rainAmt) == RainLevels.Typhoon || 
				GetRainCategory(rainAmt) == RainLevels.NoahsFlood || GetRainCategory(rainAmt) == RainLevels.Severe)
                return true;

            return false;
        }

        internal static int ReturnRndRainAmtInLevel(MersenneTwister dice, RainLevels rl)
        {
			return (int)(Math.Round(RainCategories[rl].RandomInFullRange(dice),0));
        }

        internal static string DescRainCategory(int rain)
        {
            if (rain == 0)
                return ClimatesOfFerngill.Translator.Get("category-none");
            else if (RainCategories[RainLevels.Sunshower].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-sunshower");
            else if (RainCategories[RainLevels.Light].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-light");
            else if (RainCategories[RainLevels.Normal].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-normal");
            else if (RainCategories[RainLevels.Moderate].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-moderate");
            else if (RainCategories[RainLevels.Heavy].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-heavy");
            else if (RainCategories[RainLevels.Severe].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-severe");
            else if (RainCategories[RainLevels.Torrential].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-torrential");
            else if (RainCategories[RainLevels.Typhoon].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-typhoon");
            else if (RainCategories[RainLevels.NoahsFlood].IsWithinFullRange(rain))
                return ClimatesOfFerngill.Translator.Get("category-noahsflood");

            throw new System.Exception($"Rain is {rain}, reached point in execution it shouldn't reach.");
        }

        internal static double GetStepChance(int rain, bool increase = true)
        {
            double multiplier = ClimatesOfFerngill.WeatherOpt.VRMassiveStepChance;
            //directions - 0 down 1 up
            {
                if (increase)
                {
		            if (RainCategories[RainLevels.Sunshower].IsWithinLowerRange(rain))
                        multiplier *= 5.4;
                    else if (RainCategories[RainLevels.Sunshower].IsWithinUpperRange(rain))
                        multiplier *= 3.2;
                    else if (RainCategories[RainLevels.Light].IsWithinLowerRange(rain))
                        multiplier *= 1.8;
                    else if (RainCategories[RainLevels.Light].IsWithinUpperRange(rain))
                        multiplier *= 1.5;
                    else if (RainCategories[RainLevels.Normal].IsWithinLowerRange(rain))
                        multiplier *= 1.4;
                    else if (RainCategories[RainLevels.Heavy].IsWithinUpperRange(rain) || RainCategories[RainLevels.Severe].IsWithinLowerRange(rain))
                        multiplier /= 1.95;
                    else if (RainCategories[RainLevels.Severe].IsWithinUpperRange(rain))
                        multiplier /= 2.65;
                    else if (RainCategories[RainLevels.Torrential].IsWithinFullRange(rain))
                        multiplier /= 3.45;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinLowerRange(rain))
                        multiplier /= 4.45;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinUpperRange(rain))
                        multiplier /= 5.65;
		            else if (RainCategories[RainLevels.NoahsFlood].IsWithinLowerRange(rain))
                        multiplier /= 7.4;
                    else if (RainCategories[RainLevels.NoahsFlood].IsWithinUpperRange(rain))
                        multiplier /= 8.62;
                }

                if (!increase)
                {
		            if (RainCategories[RainLevels.Sunshower].IsWithinLowerRange(rain))
                        multiplier /= 5.4;
                    else if (RainCategories[RainLevels.Sunshower].IsWithinUpperRange(rain))
                        multiplier /= 3.2;
                    else if (RainCategories[RainLevels.Light].IsWithinLowerRange(rain))
                        multiplier /= 1.8;
                    else if (RainCategories[RainLevels.Light].IsWithinUpperRange(rain))
                        multiplier /= 1.5;
                    else if (RainCategories[RainLevels.Normal].IsWithinLowerRange(rain))
                        multiplier /= 1.4;
                    else if (RainCategories[RainLevels.Severe].IsWithinUpperRange(rain))
                        multiplier *= 2.65;
                    else if (RainCategories[RainLevels.Torrential].IsWithinFullRange(rain))
                        multiplier *= 3.45;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinLowerRange(rain))
                        multiplier *= 4.45;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinUpperRange(rain))
                        multiplier *= 5.65;
		            else if (RainCategories[RainLevels.NoahsFlood].IsWithinLowerRange(rain))
                        multiplier *= 7.4;
                    else if (RainCategories[RainLevels.NoahsFlood].IsWithinUpperRange(rain))
                        multiplier *= 8.62;
                }
            }
            
            return multiplier;
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
                    if (verbose) mon.Log($"Setting a forced value for tomorrow: {Desc.DescribeInGameWeather(entry.Value)} for {entry.Key.Season} {entry.Key.Day}");
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

        public static double GetRainfallAmt(int rainFall)
        {
            double rain;

            //70, or normal rain, should be about 4mm. 280 should be 12? 
            //It gives violent as >50mm, so if we use these two, we get a slope of [12-4/280-70] or [8/210] = 0.03809 with a b intercept of 
            //4 = (70)(.03809) + b; 4 = 2.663 + b; b  = 1.3337; testing this, 7 is 1.6003 mm, and we cross the light threshold at 30-35. 
            //Expanded testing: Midpoint of the sunshower is 1.67nm, light is 2.6nm, normal is 4nm, moderate is 6.66nm, heavy is 11.99nm
            //severe is 22.66nm, torrential is 43.99nm, typhoon is 86.65nm, and noahsflood is 141.5049nm. The cap (at 4000) is 153.69nm
            rain = (rainFall) * (0.0380952381) + 1.3337;

            if (ClimatesOfFerngill.WeatherOpt.UseImperialForRainfall)
                return rain / 25.4;
            else
                return rain;
        }


        internal static string GetUnitAmt(int val)
        {
            return WeatherUtilities.GetRainfallAmt(val) + " mm/hr";

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

            Array.Resize(ref Game1.rainDrops, 70);
            ClimatesOfFerngill.Conditions.SetRainAmt(70);

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static void SetWeatherTesting()
        {
            Game1.isSnowing = Game1.isLightning = false;
            Game1.isDebrisWeather = Game1.isRaining = true;
            ClimatesOfFerngill.Conditions.RemoveWeather(CurrentWeather.Overcast);
            Game1.debrisWeather.Clear();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().EndWeather();
            ClimatesOfFerngill.Conditions.SetVariableRain(false);
            Game1.populateDebrisWeatherArray();
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();

            Array.Resize(ref Game1.rainDrops, 70);
            ClimatesOfFerngill.Conditions.SetRainAmt(70);

            if (Game1.currentLocation is DecoratableLocation)
                UpdateFurniture(Game1.currentLocation as DecoratableLocation);
        }

        internal static void SetWeatherTesting3()
        {
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherExpirationTime(new SDVTime(Game1.timeOfDay + 10));
            Game1.windGust = -18.0f;
            ClimatesOfFerngill.WindOverrideSpeed = -24.35f;
            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();
        }

        internal static void SetWeatherTesting2()
        {
            Game1.isSnowing = Game1.isLightning = false;
            Game1.isDebrisWeather = Game1.isRaining = true;
            ClimatesOfFerngill.Conditions.RemoveWeather(CurrentWeather.Overcast);
            Game1.debrisWeather.Clear();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().EndWeather();
            ClimatesOfFerngill.Conditions.SetVariableRain(false);
            Game1.populateDebrisWeatherArray();

            if (!ClimatesOfFerngill.Conditions.IsFoggy())
            {
                ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().ForceWeatherStart();
                ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().SetWeatherExpirationTime(new SDVTime(2600));
            }

            Game1.updateWeather(Game1.currentGameTime);
            Game1.currentLocation.UpdateWhenCurrentLocation(Game1.currentGameTime);
            ClimatesOfFerngill.Conditions.Refresh();
            SDVUtilities.UpdateAudio();            

            Array.Resize(ref Game1.rainDrops, 70);
            ClimatesOfFerngill.Conditions.SetRainAmt(70);

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

            ClimatesOfFerngill.Logger.Log("Resizing rain array - setting weather to overcast");
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

        internal static void SetWeatherSandstorm()
        {
            Game1.isSnowing = Game1.isDebrisWeather = false;
            Game1.isLightning = Game1.isRaining = false;
            Game1.isDebrisWeather = true;
            ClimatesOfFerngill.WindCap = -50f;
            ClimatesOfFerngill.WindMin = -30f;
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().ForceWeatherStart();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().SetWeatherExpirationTime(new SDVTime(2600));
            Game1.populateDebrisWeatherArray();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().EndWeather();
            ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().EndWeather();
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
