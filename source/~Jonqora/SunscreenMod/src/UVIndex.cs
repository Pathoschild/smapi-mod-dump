using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;

namespace SunscreenMod
{
    class UVIndex : IAssetEditor
    {

        /*********
        ** Accessors
        *********/
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;
        protected static ModConfig Config => ModConfig.Instance;


        /*********
        ** Fields
        *********/
        protected static ITranslationHelper i18n = Helper.Translation;


        /*********
        ** Properties
        *********/
        /// <summary>Forecasted UV Index high for the current game day.</summary>
        public int TodayMaxUVIndex => UVIndexForecastForToday();

        /// <summary>Forecasted UV Index high for the next game day.</summary>
        public int TomorrowMaxUVIndex => UVIndexForecastForTomorrow();


        /*********
        ** Constants
        *********/
        /// <summary>Daily solar noon in integer time.</summary>
        private const int SOLAR_NOON = 1300; //1:00 pm
        /// <summary>Average daily UV index high on sunny days in midsummer.</summary>
        private const int AVG_MAX_UV_HIGH = 10;
        /// <summary>Average daily UV index high on sunny days in midwinter.</summary>
        private const int AVG_MAX_UV_LOW = 2;
        /// <summary>Multiplication factor for converting between UV intensity and UV index.</summary>
        private const int UV_INDEX_FACTOR = 25;
        /// <summary>Standard deviation from seasonal sunny average for randomized max daily UV intensity.</summary>
        private const int MAX_UV_STD_DEV = UV_INDEX_FACTOR; //Equal to +/- 1 on the UV index scale

        /// <summary>List of game data keys in Strings/StringsFromCSFiles that correspond to weather forcast text.</summary>
        private readonly string[] WeatherKeys = new string[]
        {
                        "TV.cs.13164", //Um... that's odd. My information sheet just says 'null'. This is embarrassing...
                        "TV.cs.13175", //It's going to be clear and sunny tomorrow... perfect weather for the {0}! The event will take place {1}, starting between {2} and {3}. Don't be late!
                        "TV.cs.13180", //Bundle up, folks. It's going to snow tomorrow!
                        "TV.cs.13181", //Expect a few inches of snow tomorrow.
                        "TV.cs.13182", //It's going to be a beautiful, sunny day tomorrow!
                        "TV.cs.13183", //It's going to be clear and sunny all day.
                        "TV.cs.13184", //It's going to rain all day tomorrow.
                        "TV.cs.13185", //Looks like a storm is approaching. Thunder and lightning is expected.
                        "TV.cs.13187", //Partially cloudy with a light breeze. Expect lots of pollen!
                        "TV.cs.13189", //It's going to be cloudy, with gusts of wind throughout the day.
                        "TV.cs.13190" //It's going to snow all day. Make sure you bundle up, folks!
        };


        /*********
        ** Methods
        *********/
        /// <summary>Sine function to calculate seasonal average UV intensity high for a given day.</summary>
        /// <param name="daysSinceStart">Date represented as integer days since start</param>
        /// <returns>UV intensity (daily high in sunny weather)</returns>
        private static int SeasonalAvgMaxUV(int daysSinceStart)
        {
            double MEAN = (AVG_MAX_UV_HIGH + AVG_MAX_UV_LOW) / 2; //6
            double AMP = (AVG_MAX_UV_HIGH - AVG_MAX_UV_LOW) / 2; //4
            double avg = AMP * Math.Sin(2 * Math.PI / 112 * (daysSinceStart - 14)) + MEAN; //Period 112d, phase shift 14d right, mean 6, range 2-10
            return Convert.ToInt32(avg * UV_INDEX_FACTOR); //Larger values before correcting to a UV Index value
        }

        /// <summary>Calculates UV intensity high for a given day, adjusting for random variance and weather.</summary>
        /// <param name="daysSinceStart">Date represented as integer days since start</param>
        /// <param name="weather">Integer weather from game constants e.g. Game1.weather_rain</param>
        /// <returns>UV intensity (daily high)</returns>
        public static int DailyMaxUV(int daysSinceStart, int weather = Game1.weather_sunny)
        {
            if (!Context.IsWorldReady)
            {
                throw new Exception("Couldn't generate random variance as no save is loaded."); //Ignore before a save is loaded.
            }
            int avg = SeasonalAvgMaxUV(daysSinceStart);

            //Fixed random variance from seasonal average
            Random fixedRandom = new Random((int)Game1.uniqueIDForThisGame / 2 + 6676 + daysSinceStart);
            double u1 = 1.0 - fixedRandom.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - fixedRandom.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

            //Certain weather causes lower UV
            double weatherModifier = 1.0; //Game1.weather_sunny, Game1.weather_festival, or Game1.weather_wedding
            if (weather == Game1.weather_debris) weatherModifier = 0.8;
            if (weather == Game1.weather_rain || weather == Game1.weather_lightning || weather == Game1.weather_snow) weatherModifier = 0.3;

            int randDailyUV = Convert.ToInt32(weatherModifier * (avg + MAX_UV_STD_DEV * randStdNormal)); //mean + stdDev * randStdNormal
            randDailyUV = Math.Max(0, randDailyUV); //Cap at 0 on lower end
            return randDailyUV;
        }

        /// <summary>Calculates immediate UV intensity for the current date at a given time.</summary>
        /// <param name="time">Time of day to calculate for</param>
        /// <returns>UV intensity at time of day</returns>
        public static int UVIntensityAt(SDVTime time)
        {
            if (!Context.IsWorldReady)
            {
                throw new Exception("Couldn't get date and time as no save is loaded."); //Ignore before a save is loaded.
            }

            int today = SDate.Now().DaysSinceStart;
            int todaysWeather = Game1.weather_sunny;
            if (Game1.isDebrisWeather) todaysWeather = Game1.weather_debris;
            if (Game1.isRaining) todaysWeather = Game1.weather_rain;
            if (Game1.isLightning) todaysWeather = Game1.weather_lightning;
            if (Game1.isSnowing) todaysWeather = Game1.weather_snow;

            int todayMaxUV = DailyMaxUV(today, todaysWeather);

            SDVTime solarNoon = new SDVTime(SOLAR_NOON);
            SDVTime sunset = new SDVTime(Game1.getTrulyDarkTime());
            int halfCycleMinutes = SDVTime.ConvertTimeToMinutes(sunset - solarNoon);
            int solarNoonMinutes = SDVTime.ConvertTimeToMinutes(solarNoon);
            int timeMinutes = SDVTime.ConvertTimeToMinutes(time);
            SDVTime sunrise = new SDVTime(solarNoon); sunrise.AddMinutes(-1 * halfCycleMinutes);

            if (time <= sunrise || time >= sunset) return 0;
            else
            {
                double ampitude = todayMaxUV / 2;
                double UV = ampitude * Math.Cos(Math.PI / halfCycleMinutes * (timeMinutes - solarNoonMinutes)) + ampitude;
                return Convert.ToInt32(UV);
            }
        }

        /// <summary>Calculates a forecasted UV index high for a given day, random variance and weather.</summary>
        /// <param name="daysSinceStart">Date represented as integer days since start</param>
        /// <param name="weather">Integer weather from game constants e.g. Game1.weather_rain</param>
        /// <returns>UV index (daily high)</returns>
        public static int UVIndexForecast(int daysSinceStart, int weather = Game1.weather_sunny)
        {
            int MaxUV = DailyMaxUV(daysSinceStart, weather);
            int uvIndex = Convert.ToInt32((double)MaxUV / 25);
            return uvIndex;
        }

        /// <summary>Calculates tomorrow's forecasted UV index high, given random variance and weather.</summary>
        /// <returns>UV index (daily high)</returns>
        public static int UVIndexForecastForTomorrow()
        {
            if (!Context.IsWorldReady)
            {
                throw new Exception("Couldn't find current date as no save is loaded."); //Ignore before a save is loaded.
            }
            int tomorrow = SDate.Now().AddDays(1).DaysSinceStart;
            return UVIndexForecast(tomorrow, Game1.weatherForTomorrow);
        }

        /// <summary>Calculates today's forecasted UV index high, given random variance and weather.</summary>
        /// <returns>UV index (daily high)</returns>
        public static int UVIndexForecastForToday()
        {
            if (!Context.IsWorldReady)
            {
                throw new Exception("Couldn't find current date as no save is loaded."); //Ignore before a save is loaded.
            }
            int today = SDate.Now().DaysSinceStart;
            return UVIndexForecast(today, GetTodaysWeather());
        }

        /// <summary>Get's the integer weather constant for the current weather.</summary>
        /// <returns>Weather value (e.g. integer value of Game1.weather_lightning)</returns>
        public static int GetTodaysWeather()
        {
            int todaysWeather = Game1.weather_sunny;
            if (Game1.isDebrisWeather) todaysWeather = Game1.weather_debris;
            if (Game1.isRaining) todaysWeather = Game1.weather_rain;
            if (Game1.isLightning) todaysWeather = Game1.weather_lightning;
            if (Game1.isSnowing) todaysWeather = Game1.weather_snow;
            return todaysWeather;
        }

        /// <summary>Determines risk level according to the UV index scale.</summary>
        /// <param name="uvIndex">UV Index value</param>
        /// <returns>Risk level (ranging from LowRisk to ExtremeRisk)</returns>
        public string GetRiskLevel(int uvIndex)
        {
            if (uvIndex <= 2) return "LowRisk"; //Max UV index 0-2
            else if (uvIndex >= 3 && uvIndex <= 5) return "ModerateRisk"; //Max UV index 3-5
            else if (uvIndex >= 6 && uvIndex <= 7) return "HighRisk"; //Max UV index 6-7
            else if (uvIndex >= 8 && uvIndex <= 10) return "VeryHighRisk"; //Max UV index 8-10
            else return "ExtremeRisk"; //Max UV index is 11+
        }


        /*********
        ** IAssetEditor methods
        *********/
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <typeparam name="_T">The asset Type.</typeparam>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        /// <returns>true for asset Strings\StringsFromCSFiles, false otherwise</returns>
        public bool CanEdit<_T>(IAssetInfo asset)
        {
            return Config.WeatherReport && asset.AssetNameEquals($"Strings\\StringsFromCSFiles") && Context.IsWorldReady; //Config enabled weather updates
        }

        /// <summary>Edit the Strings\StringsFromCSFiles TV weather channel entries to show UV forecast index and advice.</summary>
        /// <typeparam name="_T">The asset Type</typeparam>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
        public void Edit<_T>(IAssetData asset)
        {
            SDate tomorrow = SDate.Now().AddDays(1);

            if (Config.SunburnPossible(tomorrow))
            {
                var data = asset.AsDictionary<string, string>().Data;
                int uvIndex = TomorrowMaxUVIndex;
                string uvForecast = i18n.Get("Weather.UVForecast", new
                {
                    uvIndex,
                    uvRiskLevel = i18n.Get($"Weather.{GetRiskLevel(uvIndex)}")
                });
                string uvAdvice = i18n.Get($"Advice.{GetRiskLevel(uvIndex)}");

                // Get and edit the appropriate string values
                foreach (string key in WeatherKeys)
                {
                    data[key] += " " + uvForecast + " " + uvAdvice;
                }
            }
        }

    }
}