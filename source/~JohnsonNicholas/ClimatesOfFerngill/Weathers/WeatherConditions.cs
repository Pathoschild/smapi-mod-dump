using EnumsNET;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
using static ClimatesOfFerngillRebuild.Sprites;
using System;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>
    /// This class tracks the current the current weather of the game
    /// </summary>
    public class WeatherConditions
    {
        ///<summary>Game configuration options</summary>
        private WeatherConfig ModConfig;

        ///<summary>pRNG object</summary>
        private MersenneTwister Dice;

        ///<summary>SMAPI logger</summary>
        private IMonitor Monitor;

        /// <summary> The translation interface </summary>
        private ITranslationHelper Translation;

        /// <summary>Track today's temperature</summary>
        private RangePair TodayTemps;

        /// <summary>Track tomorrow's temperature</summary>
        private RangePair TomorrowTemps;

        /// <summary>Track current conditions</summary>
        private CurrentWeather CurrentConditionsN { get; set; }

        internal List<ISDVWeather> CurrentWeathers { get; set; }

        private bool HasSetEveningFog {get; set;}

        public bool GenerateEveningFog { get; set; }

        private bool FogCreationProhibited { get; set; }

        private SDVTime MorningFogExpir { get; set; }

        /// *************************************************************************
        /// ACCESS METHODS
        /// *************************************************************************
        public CurrentWeather GetCurrentConditions()
        {
            return CurrentConditionsN;
        }

        public void DrawWeathers()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.DrawWeather();
        }

        public void MoveWeathers()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.MoveWeather();
        }
        
        public string FogDescription(double fogRoll, double fogChance)
        {
            string desc = "";
            foreach (ISDVWeather weather in CurrentWeathers)
            {
               if (weather.WeatherType == "Fog")
                {
                    FerngillFog fog = (FerngillFog)weather;
                    desc += fog.FogDescription(fogRoll, fogChance);
                }
            }

            return desc;
        }

        public void CreateWeather(string Type, bool IsMorningFog = false)
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather.WeatherType == Type)
                    weather.CreateWeather();

                if (Type == "Fog" && IsMorningFog)
                {
                    MorningFogExpir = weather.WeatherExpirationTime;
                }
            }
        }

        public void TenMinuteUpdate()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                weather.UpdateWeather();
            }

            if (SDVTime.CurrentTimePeriod == SDVTimePeriods.Afternoon && GenerateEveningFog && !HasSetEveningFog && (!IsFestivalToday && !IsWeddingToday))
            {
                //Get fog instance
                FerngillFog ourFog = (FerngillFog)this.GetWeatherMatchingType("Fog").First();
                if (!ourFog.WeatherInProgress)
                {
                    ourFog.SetEveningFog();
                    HasSetEveningFog = true;
                }
            }
        }

        internal List<ISDVWeather> GetWeatherMatchingType(string type)
        {
            List<ISDVWeather> Weathers = new List<ISDVWeather>();
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather.WeatherType == type)
                    Weathers.Add(weather);
            }

            return Weathers;
        }

        /// <summary>Rather than track the weather seprately, always get it from the game.</summary>
        public CurrentWeather TommorowForecast => ConvertToCurrentWeather(Game1.weatherForTomorrow);

        public bool IsTodayTempSet => TodayTemps != null;
        public bool IsTomorrowTempSet => TomorrowTemps != null;
        public bool IsFestivalToday => CurrentConditionsN.HasFlag(CurrentWeather.Festival);
        public bool IsWeddingToday => CurrentConditionsN.HasFlag(CurrentWeather.Wedding);

        /// <summary> This returns the high for today </summary>
        public double TodayHigh => TodayTemps.HigherBound;

        /// <summary> This returns the high for tomorrow </summary>
        public double TomorrowHigh => TomorrowTemps.HigherBound;

        /// <summary> This returns the low for today </summary>
        public double TodayLow => TodayTemps.LowerBound;

        /// <summary> This returns the low for tomorrow </summary>
        public double TomorrowLow => TomorrowTemps.LowerBound;

        public void AddWeather(CurrentWeather newWeather)
        {
            //sanity remove these once weather is set.
            CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Unset);

            //Some flags are contradictoary. Fix that here.
            if (newWeather == CurrentWeather.Rain)
            {
                //unset debris, sunny, snow and blizzard, if it's raining.
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sunny);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Snow);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Blizzard);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Wind);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Sunny)
            {
                //unset debris, rain, snow and blizzard, if it's sunny.
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Rain);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Snow);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Blizzard);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Wind);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Wind)
            {
                //unset sunny, rain, snow and blizzard, if it's debris.
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Rain);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Snow);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Blizzard);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sunny);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Snow)
            {
                //unset debris, sunny, snow and blizzard, if it's raining.
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sunny);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Rain);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Wind);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Frost)
            {
                CurrentConditionsN.RemoveFlags(CurrentWeather.Heatwave);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Heatwave)
            {
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Frost);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Wedding || newWeather == CurrentWeather.Festival)
            {
                CurrentConditionsN = newWeather; //Clear *everything else* if it's a wedding or festival.
            }

            else
                CurrentConditionsN |= newWeather;
        }

        internal void ForceEveningFog()
        {
            //Get fog instance
            List<ISDVWeather> fogWeather = this.GetWeatherMatchingType("Fog");
            foreach (ISDVWeather weat in fogWeather)
            {
                SDVTime BeginTime, ExpirTime;
                BeginTime = new SDVTime(Game1.getStartingToGetDarkTime());
                BeginTime.AddTime(Dice.Next(-15, 90));

                ExpirTime = new SDVTime(BeginTime);
                ExpirTime.AddTime(Dice.Next(120, 310));

                BeginTime.ClampToTenMinutes();
                ExpirTime.ClampToTenMinutes();
                weat.SetWeatherTime(BeginTime, ExpirTime);
            }
        }

        /// <summary> Syntatic Sugar for Enum.HasFlag(). Done so if I choose to rewrite how it's accessed, less rewriting of invoking functions is needed. </summary>
        /// <param name="checkWeather">The weather being checked.</param>
        /// <returns>If the weather is present</returns>
        public bool HasWeather(CurrentWeather checkWeather)
        {
            return CurrentConditionsN.HasFlag(checkWeather);
        }

        public void ClearFog() => CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Fog);

        public bool HasPrecip()
        {
            if (CurrentConditionsN.HasAnyFlags(CurrentWeather.Snow | CurrentWeather.Rain | CurrentWeather.Blizzard))
                return true;

            return false;
        }

        public WeatherIcon CurrentWeatherIcon
        {
            get
            {
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Rain))
                    return WeatherIcon.IconRain;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Lightning)))
                    return WeatherIcon.IconStorm;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Sunny)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Wind)))
                    return WeatherIcon.IconDryLightningWind;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Snow)))
                    return WeatherIcon.IconThunderSnow;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Snow))
                    return WeatherIcon.IconSnow;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Blizzard)))
                    return WeatherIcon.IconBlizzard;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut)))
                    return WeatherIcon.IconWhiteOut;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Festival))
                    return WeatherIcon.IconFestival;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Wedding))
                    return WeatherIcon.IconWedding;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Sunny))
                    return WeatherIcon.IconSunny;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Unset))
                    return WeatherIcon.IconSunny;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Lightning))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Wind)) {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Lightning)))
                    return WeatherIcon.IconThunderSnow;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Lightning)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Sunny)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Heatwave)))
                    return WeatherIcon.IconRain;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Heatwave | CurrentWeather.Fog)))
                    return WeatherIcon.IconRainFog;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave)))
                    return WeatherIcon.IconStorm;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Fog)))
                    return WeatherIcon.IconStormFog;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Heatwave | CurrentWeather.Sunny)))
                    return WeatherIcon.IconSunny;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Wind)))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Heatwave | CurrentWeather.Wind)))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }

                //The more complex ones.
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Sunny | CurrentWeather.Frost)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Rain | CurrentWeather.Frost)))
                    return WeatherIcon.IconStorm;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut)))
                    return WeatherIcon.IconWhiteOut;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Frost)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Wind | CurrentWeather.Frost)))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Frost)))
                    return WeatherIcon.IconRain;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Frost)))
                    return WeatherIcon.IconSnow;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Frost)))
                    return WeatherIcon.IconBlizzard;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Frost | CurrentWeather.WhiteOut)))
                    return WeatherIcon.IconBlizzard;

                //And now for fog.
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Sunny)))
                    return WeatherIcon.IconSunnyFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Rain | CurrentWeather.Fog)))
                    return WeatherIcon.IconStormFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Fog)))
                    return WeatherIcon.IconSunnyFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Fog)))
                    return WeatherIcon.IconRainFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Fog)))
                    return WeatherIcon.IconSnowFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Fog)))
                    return WeatherIcon.IconBlizzardFog;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Frost | CurrentWeather.Fog)))
                    return WeatherIcon.IconSunnyFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Frost | CurrentWeather.Fog)))
                    return WeatherIcon.IconRainFog;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Fog)))
                    return WeatherIcon.IconWhiteOutFog;

                Console.WriteLine($"Error. Current conditions are: {CurrentConditionsN}");

                return WeatherIcon.IconError;
            }
        }

        public bool BlockFog { get; set; }

        public WeatherIcon CurrentWeatherIconBasic
        {
            get
            {
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Rain))
                    return WeatherIcon.IconRain;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Lightning)))
                    return WeatherIcon.IconStorm;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Sunny)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Wind)))
                    return WeatherIcon.IconDryLightningWind;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Snow)))
                    return WeatherIcon.IconThunderSnow;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Snow))
                    return WeatherIcon.IconSnow;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Blizzard)))
                    return WeatherIcon.IconBlizzard;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.WhiteOut)))
                    return WeatherIcon.IconWhiteOut;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Festival))
                    return WeatherIcon.IconFestival;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Wedding))
                    return WeatherIcon.IconWedding;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Sunny))
                    return WeatherIcon.IconSunny;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Unset))
                    return WeatherIcon.IconSunny;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Lightning))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)CurrentWeather.Wind))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Lightning)))
                    return WeatherIcon.IconThunderSnow;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Lightning)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Sunny)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Heatwave)))
                    return WeatherIcon.IconRain;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Heatwave | CurrentWeather.Fog)))
                    return WeatherIcon.IconRain;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave)))
                    return WeatherIcon.IconStorm;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Fog)))
                    return WeatherIcon.IconStorm;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Heatwave | CurrentWeather.Sunny)))
                    return WeatherIcon.IconSunny;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave)))
                    return WeatherIcon.IconDryLightning;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Wind)))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Heatwave | CurrentWeather.Wind)))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }

                //The more complex ones.
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Sunny | CurrentWeather.Frost)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Rain | CurrentWeather.Frost)))
                    return WeatherIcon.IconStorm;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Frost)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Wind | CurrentWeather.Frost)))
                {
                    if (SDate.Now().Season == "spring") return WeatherIcon.IconSpringDebris;
                    else return WeatherIcon.IconDebris;
                }
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Frost)))
                    return WeatherIcon.IconRain;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Frost)))
                    return WeatherIcon.IconSnow;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Frost)))
                    return WeatherIcon.IconBlizzard;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Frost | CurrentWeather.WhiteOut)))
                    return WeatherIcon.IconWhiteOut;

                //And now for fog.
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Sunny)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Lightning | CurrentWeather.Rain | CurrentWeather.Fog)))
                    return WeatherIcon.IconStorm;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Fog)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Fog)))
                    return WeatherIcon.IconRain;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Snow | CurrentWeather.Fog)))
                    return WeatherIcon.IconSnow;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Fog)))
                    return WeatherIcon.IconBlizzard;

                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Sunny | CurrentWeather.Frost | CurrentWeather.Fog)))
                    return WeatherIcon.IconSunny;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Rain | CurrentWeather.Frost | CurrentWeather.Fog)))
                    return WeatherIcon.IconRain;
                if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditionsN, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Fog | CurrentWeather.WhiteOut)))
                    return WeatherIcon.IconWhiteOut;

                Console.WriteLine($"Error. Current conditions are: {CurrentConditionsN}");

                return WeatherIcon.IconError;
            }
        }

        /// ******************************************************************************
        /// CONSTRUCTORS
        /// ******************************************************************************

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="Dice">pRNG</param>
        /// <param name="monitor">SMAPI log object</param>
        /// <param name="Config">Game configuration</param>
        public WeatherConditions(Icons Sheets, MersenneTwister Dice, ITranslationHelper Translation, IMonitor monitor, WeatherConfig Config)
        {
            this.Monitor = monitor;
            this.ModConfig = Config;
            this.Dice = Dice;
            this.Translation = Translation;
            CurrentConditionsN = CurrentWeather.Unset;
            CurrentWeathers = new List<ISDVWeather>
            {
                new FerngillFog(Sheets, Config.Verbose, monitor, Dice, Config, SDVTimePeriods.Morning),
                new FerngillWhiteOut(Dice, Config),
                new FerngillBlizzard(Dice, Config)
            };

            foreach (ISDVWeather weather in CurrentWeathers)
                weather.OnUpdateStatus += ProcessWeatherChanges;
        }
  
        private void ProcessWeatherChanges(object sender, WeatherNotificationArgs e)
        {
            if (e.Weather == "WhiteOut")
            {
                if (e.Present)
                {
                    CurrentConditionsN = CurrentConditionsN | CurrentWeather.WhiteOut;
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.WhiteOut);
                }
            }

            if (e.Weather == "Fog")
            {
                if (e.Present)
                {
                    CurrentConditionsN = CurrentConditionsN | CurrentWeather.Fog;
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Fog);
                }
            }

            if (e.Weather == "Blizzard")
            {
  
                if (e.Present)
                    CurrentConditionsN = CurrentConditionsN | CurrentWeather.Blizzard;
                else
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Blizzard);
            }
        }

        public bool ContainsCondition(CurrentWeather cond)
        {
            if (CurrentConditionsN.HasFlag(cond))
            {
                return true;
            }

            return false;
        }

        /// ******************************************************************************
        /// PROCESSING
        /// ******************************************************************************
        internal void ForceTodayTemps(double high, double low)
        {
            if (TodayTemps is null)
                TodayTemps = new RangePair();

            TodayTemps.HigherBound = high;
            TodayTemps.LowerBound = low;
        }

        /// <summary>This function resets the weather for a new day.</summary>
        public void OnNewDay()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.OnNewDay();

            FogCreationProhibited = false;
            CurrentConditionsN = CurrentWeather.Unset;
            TodayTemps = TomorrowTemps; //If Tomorrow is null, should just allow it to be null.
            TomorrowTemps = null;
            MorningFogExpir = new SDVTime(600);
        }

        /// <summary> This function resets the weather object to basic. </summary>
        public void Reset()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.Reset();

            FogCreationProhibited = false;
            TodayTemps = null;
            TomorrowTemps = null;
            CurrentConditionsN = CurrentWeather.Unset;
            MorningFogExpir = new SDVTime(600);
        }

        public SDVTime GetFogTime()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather is FerngillFog f)
                {
                    if (f.IsWeatherVisible)
                        return f.WeatherExpirationTime;
                    else
                        return new SDVTime(600);
                }
            }

            return new SDVTime(600);
        }

        /// <summary> This sets the temperatures from outside for today </summary>
        /// <param name="a">The RangePair that contains the generated temperatures</param>
        public void SetTodayTemps(RangePair a) => TodayTemps = new RangePair(a, EnforceHigherOverLower: true);

        /// <summary> This sets the temperatures from outside for tomorrow</summary>
        /// <param name="a">The RangePair that contains the generated temperatures</param>
        public void SetTomorrowTemps(RangePair a) => TomorrowTemps = new RangePair(a, EnforceHigherOverLower: true);

        /// ***************************************************************************
        /// Utility functions
        /// ***************************************************************************
        
        ///<summary> This function converts from the game weather back to the CurrentWeather enum. Intended primarily for use with tommorow's forecasted weather.</summary>
        internal static CurrentWeather ConvertToCurrentWeather(int weather)
        { 
            if (weather == Game1.weather_rain)
                return CurrentWeather.Rain;
            else if (weather == Game1.weather_festival)
                return CurrentWeather.Festival;
            else if (weather == Game1.weather_wedding)
                return CurrentWeather.Wedding;
            else if (weather == Game1.weather_debris)
                return CurrentWeather.Wind;
            else if (weather == Game1.weather_snow)
                return CurrentWeather.Snow;
            else if (weather == Game1.weather_lightning)
                return CurrentWeather.Rain | CurrentWeather.Lightning;

            //default return.
            return CurrentWeather.Sunny;
        }

        internal void SetTodayWeather()
        {      
            CurrentConditionsN = CurrentWeather.Unset; //reset the flag.

            if (!Game1.isDebrisWeather && !Game1.isRaining && !Game1.isSnowing)
                AddWeather(CurrentWeather.Sunny);

            if (Game1.isRaining)
                AddWeather(CurrentWeather.Rain);
            if (Game1.isDebrisWeather)
                AddWeather(CurrentWeather.Wind);
            if (Game1.isLightning)
                AddWeather(CurrentWeather.Lightning);
            if (Game1.isSnowing)
                AddWeather(CurrentWeather.Snow);

            if (Utility.isFestivalDay(SDate.Now().Day, SDate.Now().Season))
                AddWeather(CurrentWeather.Festival);

            if (Game1.weddingToday)
                AddWeather(CurrentWeather.Wedding);

            //check current weathers.
            foreach (ISDVWeather weat in CurrentWeathers)
            {
                if (weat.WeatherType == "Fog" && weat.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.Fog;
                if (weat.WeatherType == "Blizzard" && weat.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.Blizzard;
                if (weat.WeatherType == "WhiteOut" && weat.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.WhiteOut;
            }
        }

        /// <summary> Force for wedding only to match vanilla behavior. </summary>
        internal void ForceWeddingOnly() => CurrentConditionsN = CurrentWeather.Wedding;

        /// <summary> Force for festival only to match vanilla behavior. </summary>
        internal void ForceFestivalOnly() => CurrentConditionsN = CurrentWeather.Festival;

        /// <summary>Gets a quick string describing the current weather.</summary>
        /// <returns>A quick ID of the weather</returns>
        private string GetWeatherType()
        {
            return WeatherConditions.GetWeatherType(CurrentConditionsN);
        }

        /// <summary>Gets a quick string describing the weather. Meant primarily for use within the class. </summary>
        /// <returns>A quick ID of the weather</returns>
        private static string GetWeatherType(CurrentWeather CurrentConditions)
        {
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Rain))
                return "rainy";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Rain | CurrentWeather.Lightning)))
                return "stormy";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Lightning))
                return "drylightning";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Snow))
                return "snowy";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Snow | CurrentWeather.Blizzard)))
                return "blizzard";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Festival))
                return "festival";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Wedding))
                return "wedding";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Sunny))
                return "sunny";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Unset))
                return "unset";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)CurrentWeather.Wind))
                return "windy";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Snow)))
                return "thundersnow";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Sunny)))
                return "drylightningheatwavesunny";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.Wind)))
                return "drylightningheatwavewindy";

            //The more complex ones.
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Frost | CurrentWeather.Sunny)))
                return "drylightningwithfrost";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Rain | CurrentWeather.Frost)))
                return "stormswithfrost";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Sunny | CurrentWeather.Frost)))
                return "sunnyfrost";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Sunny | CurrentWeather.Frost | CurrentWeather.Fog)))
                return "sunnyfrostfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Rain | CurrentWeather.Frost | CurrentWeather.Fog)))
                return "rainyfrostfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Wind | CurrentWeather.Frost)))
                return "windyfrost";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Rain | CurrentWeather.Frost)))
                return "rainyfrost";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Snow | CurrentWeather.Frost)))
                return "snowyfrost";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Frost)))
                return "blizzardfrost";

            //And now for fog.
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Sunny)))
                return "drylightningwithfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Lightning | CurrentWeather.Rain | CurrentWeather.Fog)))
                return "stormswithfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Sunny | CurrentWeather.Fog)))
                return "sunnyfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Rain | CurrentWeather.Fog)))
                return "rainyfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Snow | CurrentWeather.Fog)))
                return "snowyfog";
            if (GeneralFunctions.ContainsOnlyMatchingFlags(CurrentConditions, (int)(CurrentWeather.Blizzard | CurrentWeather.Snow | CurrentWeather.Fog)))
                return "blizzardfog";

            return $"ERROR. Dumping raw data: {CurrentConditions}";
        }

        ///<summary>This function returns the current condition (pulled from the translation helper.)</summary>
        internal string DescribeConditions()
        {
            switch (GetWeatherType())
            {
                case "rainy":
                    return Translation.Get("weather_rainy");
                case "stormy":
                    return Translation.Get("weather_lightning");
                case "drylightning":
                    return Translation.Get("weather_drylightning");
                case "snowy":
                    return Translation.Get("weather_snow");
                case "blizzard":
                    return Translation.Get("weather_blizzard");
                case "wedding":
                    return Translation.Get("weather_wedding");
                case "festival":
                    return Translation.Get("weather_festival");
                case "unset":
                    return Translation.Get("weather_unset");
                case "windy":
                    return Translation.Get("weather_wind");
                case "thundersnow":
                    return Translation.Get("weather_thundersnow");
                case "heatwave":
                    return Translation.Get("weather_heatwave");
                case "drylightningheatwave":
                    return Translation.Get("weather_drylightningheatwave");
                case "drylightningheatwavesunny":
                    return Translation.Get("weather_drylightningheatwavesunny");
                case "drylightningheatwavewindy":
                    return Translation.Get("weather_drylightningheatwavewindy");
                case "drylightningwithfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_drylightning") });
                case "stormswithfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_lightning") });
                case "sunnyfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_sunny") });
                case "windyfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_wind") });
                case "rainyfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_rainy") });
                case "snowyfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_snow") });
                case "blizzardfrost":
                    return Translation.Get("weather_frost", new { condition = Translation.Get("weather_blizzard") });
                case "drylightningwithfog":
                    return Translation.Get("weather_fog", new { condition = Translation.Get("weather_drylightning") + " " + Translation.Get("weather_sunny") });
                case "stormswithfog":
                    return Translation.Get("weather_fog", new { condition = Translation.Get("weather_lightning") });
                case "sunnyfog":
                    return Translation.Get("weather_fog", new { condition = Translation.Get("weather_sunny") });
                case "rainyfog":
                    return Translation.Get("weather_fog", new { condition = Translation.Get("weather_rainy") });
                case "snowyfog":
                    return Translation.Get("weather_fog", new { condition = Translation.Get("weather_snow") });
                case "blizzardfog":
                    return Translation.Get("weather_fog", new { condition = Translation.Get("weather_blizzard") });
                default:
                    return "ERROR";
            }
        }

        /// <summary>
        /// This function returns a description of the object. A very important note that this is meant for debugging, and as such does not do localization.
        /// </summary>
        /// <returns>A string describing the object.</returns>
        public override string ToString()
        {
            string ret = "";
            ret += $"Low for today is {TodayTemps?.LowerBound.ToString("N3")} with the high being {TodayTemps?.HigherBound.ToString("N3")}. The current conditions are {GetWeatherType()}.";

            foreach (ISDVWeather weather in CurrentWeathers)
                ret += weather.ToString() + Environment.NewLine;
            
            ret += $"Weather set for tommorow is {WeatherConditions.GetWeatherType(WeatherConditions.ConvertToCurrentWeather(Game1.weatherForTomorrow))} with high {TomorrowTemps?.HigherBound.ToString("N3")} and low {TomorrowTemps?.LowerBound.ToString("N3")}. Evening fog generated {GenerateEveningFog} ";

            return ret;
        }
        
        internal bool TestForSpecialWeather(double fogChance)
        {
            bool specialWeatherTriggered = false;
            // Conditions: Blizzard - occurs in weather_snow in "winter"
            //             Dry Lightning - occurs if it's sunny in any season if temps exceed 25C.
            //             Frost and Heatwave check against the configuration.
            //             Thundersnow  - as Blizzard, but really rare.
            //             Fog - per climate, although night fog in winter is double normal chance

            //GenerateEveningFog = (Dice.NextDouble() < (Game1.currentSeason == "winter" ? fogChance * 2 : fogChance)) && !this.GetCurrentConditions().HasFlag(CurrentWeather.Wind);
            GenerateEveningFog = true;

            if (BlockFog)
                GenerateEveningFog = false;
            
            double fogRoll = Dice.NextDoublePositive();

            if (fogRoll < fogChance && !this.GetCurrentConditions().HasFlag(CurrentWeather.Wind) && !BlockFog)
            {
                this.CreateWeather("Fog", true);

                if (ModConfig.Verbose)
                    Monitor.Log($"{FogDescription(fogRoll, fogChance)}");

                specialWeatherTriggered = true;
            }

            if (this.HasWeather(CurrentWeather.Snow))
            {
                double blizRoll = Dice.NextDoublePositive();
                if (blizRoll <= ModConfig.BlizzardOdds)
                {
                    this.CreateWeather("Blizzard");
                    if (ModConfig.Verbose)
                        Monitor.Log($"With roll {blizRoll.ToString("N3")} against {ModConfig.BlizzardOdds}, there will be blizzards today");
                    if (Dice.NextDoublePositive() < .05)
                    {
                        this.CreateWeather("WhiteOut");
                    }
                }

                specialWeatherTriggered = true;
            }

            //Dry Lightning is also here for such like the dry and arid climates 
            //  which have so low rain chances they may never storm.
            if (this.HasWeather(CurrentWeather.Snow))
            {
                double oddsRoll = Dice.NextDoublePositive();

                if (oddsRoll <= ModConfig.ThundersnowOdds)
                {
                    this.AddWeather(CurrentWeather.Lightning);
                    if (ModConfig.Verbose)
                        Monitor.Log($"With roll {oddsRoll.ToString("N3")} against {ModConfig.ThundersnowOdds}, there will be thundersnow today");

                    specialWeatherTriggered = true;
                }
            }

            if (!(this.HasPrecip()))
            {
                double oddsRoll = Dice.NextDoublePositive();

                if (oddsRoll <= ModConfig.DryLightning && this.TodayHigh >= ModConfig.DryLightningMinTemp)
                {
                    this.AddWeather(CurrentWeather.Lightning);
                    if (ModConfig.Verbose)
                        Monitor.Log($"With roll {oddsRoll.ToString("N3")} against {ModConfig.DryLightning}, there will be dry lightning today.");

                    specialWeatherTriggered = true;
                }

                if (this.TodayHigh > ModConfig.TooHotOutside && ModConfig.HazardousWeather)
                {
                    this.AddWeather(CurrentWeather.Heatwave);
                    specialWeatherTriggered = true;
                }
            }

            if (this.TodayLow < ModConfig.TooColdOutside && !Game1.IsWinter)
            {
                if (ModConfig.HazardousWeather)
                {
                    this.AddWeather(CurrentWeather.Frost);
                    specialWeatherTriggered = true;
                }
            }

            //test for spring conversion.- 50% chance
            if (this.HasWeather(CurrentWeather.Rain) && this.HasWeather(CurrentWeather.Frost) && Game1.currentSeason == "spring" && Dice.NextDoublePositive() <= .5)
            {
                CurrentConditionsN.RemoveFlags(CurrentWeather.Rain);
                CurrentConditionsN |= CurrentWeather.Snow;
                Game1.isRaining = false;
                Game1.isSnowing = true;
                specialWeatherTriggered = true;
            }


            return specialWeatherTriggered;
        }
    }
}
