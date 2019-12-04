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
		internal static Dictionary<RainLevels,StaticRange> RainCategories{
			{(RainLevel.None,new StaticRange(0,0,0))},
			{(RainLevel.Sunshower, new StaticRange(0,9,17.5))},
			{(RainLevel.Light, new StaticRange(17.5,35,52.5))},
			{(RainLevel.Normal, new StaticRange(52.5,70,105))},
			{(RainLevel.Moderate, new StaticRange(105,140,210))},
			{(RainLevel.Heavy, new StaticRange(210,280,420))},
			{(RainLevel.Severe, new StaticRange(420,560,840))},
		        {(RainLevel.Torrential, new StaticRange(840,1120,1680))},
			{(RainLevel.Typhoon, new StaticRange(1680,2240,3360))},
			{(RainLevel.NoahsFlood, new StaticRange(3360,3680,6720))},
		};
		
        internal static int MaxRain = 4000;

        internal static int ReturnMidPoint(RainLevels level)
        {
            return RainCategories[level].MidPoint;
        }

        internal static RainLevels GetCategory(int rain)
        {
           foreach (var rl in RainCategories)
		   {
				if (rl.Value.IsWithinFullRange(rain))
					return  rl.Key;
		   }

            throw new System.Exception($"Rain is {rain}, reached point in execution it shouldn't reach.");
        }

        internal static bool IsSevereRainFall(int rainAmt)
        {
			if (GetCategory(rainAmt) == RainLevels.Torrential || GetCategory(rainAmt) == RainLevels.Typhoon || 
				GetCategory(rainAmt) == RainLevels.NoahsFlood || GetCategory(rainAmt) == RainLevels.Severe)
                return true;

            return false;
        }

        internal static int ReturnRndRainAmtInLevel(MersenneTwister dice, RainLevels rl)
        {
			return RainCategories[rl].RandomInFullRange(dice);
        }


        internal static string DescCategory(int rain)
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
                        multiplier /= 1.4;
                    else if (RainCategories[RainLevels.Severe].IsWithinUpperRange(rain)))
                        multiplier /= 1.95;
                    else if (RainCategories[RainLevels.Torrential].IsWithinFullRange(rain))
                        multiplier /= 2.25;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinLowerRange(rain))
                        multiplier /= 3.15;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinUpperRange(rain))
                        multiplier /= 3.5;
		    else if (RainCategories[RainLevels.NoahsFlood].IsWithinLowerRange(rain)))
                        multiplier /= 5.4;
                    else if (RainCategories[RainLevels.NoahsFlood].IsWithinUpperRange(rain)))
                        multiplier /= 7.8;
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
                    else if (RainCategories[RainLevels.Heavy].IsWithinUpperRange(rain) || RainCategories[RainLevels.Severe].IsWithinLowerRange(rain))
                        multiplier *= 1.65;
                    else if (RainCategories[RainLevels.Severe].IsWithinUpperRange(rain)))
                        multiplier *= 1.95;
                    else if (RainCategories[RainLevels.Torrential].IsWithinFullRange(rain))
                        multiplier *= 2.25;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinLowerRange(rain))
                        multiplier *= 3.15;
                    else if (RainCategories[RainLevels.Typhoon].IsWithinUpperRange(rain))
                        multiplier *= 3.5;
		    else if (RainCategories[RainLevels.NoahsFlood].IsWithinLowerRange(rain)))
                        multiplier *= 5.4;
                    else if (RainCategories[RainLevels.NoahsFlood].IsWithinUpperRange(rain)))
                        multiplier *= 7.8;
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
