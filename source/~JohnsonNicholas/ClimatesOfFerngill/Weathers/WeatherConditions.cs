using EnumsNET;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
using static ClimatesOfFerngillRebuild.Sprites;
using System;
using ClimatesOfFerngillRebuild.Weathers;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>
    /// This class tracks the current the current weather of the game
    /// </summary>
    public class WeatherConditions
    {
        /// <summary>This Dictionary tracks elements associated with weathers </summary>
        public Dictionary<int, WeatherData> Weathers;

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

        /// <summary>The list of custom weathers </summary>
        internal List<ISDVWeather> CurrentWeathers { get; set; }

        //evening fog details
        private bool HasSetEveningFog {get; set;}
        public bool GenerateEveningFog { get; set; }

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
            Monitor = monitor;
            ModConfig = Config;
            this.Dice = Dice;
            this.Translation = Translation;
            Weathers = PopulateWeathers();

            CurrentConditionsN = CurrentWeather.Unset;
            CurrentWeathers = new List<ISDVWeather>
            {
                new FerngillFog(Sheets, Config.Verbose, monitor, Dice, Config, SDVTimePeriods.Morning),
                new FerngillWhiteOut(Dice, Config),
                new FerngillBlizzard(Dice, Config),
                new FerngillThunderFrenzy(monitor, Dice, Config),
                //new FerngillCustomRain(monitor, Dice, Config, 300)
            };

            foreach (ISDVWeather weather in CurrentWeathers)
                weather.OnUpdateStatus += ProcessWeatherChanges;
        }

        public WeatherIcon CurrentWeatherIcon 
        {
            get
            {
                if (ClimatesOfFerngill.MoonAPI != null)
                {
                    if (ClimatesOfFerngill.MoonAPI.GetCurrentMoonPhase() == "Blood Moon")
                        return WeatherIcon.IconBloodMoon;
                }

                if (Weathers.ContainsKey((int)CurrentConditionsN))
                    return Weathers[(int)CurrentConditionsN].Icon;
                else
                    return WeatherIcon.IconError;
            }
        }

        public WeatherIcon CurrentWeatherIconBasic
        {
            get
            {
                if (ClimatesOfFerngill.MoonAPI != null)
                {
                    if (ClimatesOfFerngill.MoonAPI.GetCurrentMoonPhase() == "Blood Moon")
                        return WeatherIcon.IconBloodMoon;
                }

                if (Weathers.ContainsKey((int)CurrentConditionsN))
                    return Weathers[(int)CurrentConditionsN].IconBasic;
                else
                    return WeatherIcon.IconError;
            }
        }

        private Dictionary<int, WeatherData> PopulateWeathers()
        {
            return new Dictionary<int, WeatherData>{ 
                { (int)CurrentWeather.Sunny, new WeatherData(WeatherIcon.IconSunny, WeatherIcon.IconSunny, "sunny", Translation.Get("weather_sunny_daytime"), CondDescNight: Translation.Get("weather_sunny_nighttime"))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightning", Translation.Get("weather_drylightning"))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSunny, WeatherIcon.IconSunny, CondName: "sunnyfrost", CondDesc: Translation.Get("weather_frost", new { condition = Translation.Get("weather_sunny") }), CondDescNight: Translation.Get("weather_frost_night")) },

                { (int)(CurrentWeather.Sunny | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSunny, WeatherIcon.IconSunny, CondName: "sunnyheatwave", CondDesc: Translation.Get("weather_heatwave")) },

                { (int)(CurrentWeather.Sunny | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "fog", CondDesc: Translation.Get("weather_fog", new { condition = Translation.Get("weather_sunny") }), CondDescNight: Translation.Get("weather_frost_night")) },

                { (int)(CurrentWeather.Sunny | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "sunnyfrostfog", CondDesc: Translation.Get("weather_frost", new { condition = Translation.Get("weather_fog_basic") }))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Frost | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningfrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_drylightning") }))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Fog | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightningFog, WeatherIcon.IconSunny, CondName: "drylightningfog", CondDesc: Translation.Get("weather_fog", new { condition = Translation.Get("weather_drylightning") }))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Heatwave | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningheatwave", Translation.Get("weather_drylightningheatwave"))},

                { (int)(CurrentWeather.Frost | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningfrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_drylightning") }))},

                { (int)(CurrentWeather.Heatwave | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningheatwave", Translation.Get("weather_drylightningheatwave"))},

                { (int)(CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightning", Translation.Get("weather_drylightning"))},

                { (int)(CurrentWeather.Unset), new WeatherData(WeatherIcon.IconError, WeatherIcon.IconError, "unset", Translation.Get("weather_unset"))},

                { (int)(CurrentWeather.Rain), new WeatherData(WeatherIcon.IconRain, WeatherIcon.IconRain, "rainy", Translation.Get("weather_rainy")) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormy", Translation.Get("weather_stormy")) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyfrost", Translation.Get("weather_frost", new { conditions = Translation.Get("weather_stormy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyheatwave", Translation.Get("weather_heatwaveCond", new { conditions = Translation.Get("weather_stormy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconRainFog, WeatherIcon.IconRain, "rainyfog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_rainy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyfog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_stormy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfog", Translation.Get("weather_frenzy")) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Frost | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_frenzy")})) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyheatwave", Translation.Get("weather_heatwave", new { condition = Translation.Get("weather_frenzy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_frenzy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Frost | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfogfrost", Translation.Get("weather_frostTwo", new { condition = Translation.Get("weather_frenzy"), condtitionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Heatwave | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfogheatwave", Translation.Get("weather_heatwaveTwo", new { condition = Translation.Get("weather_frenzy"), condtitionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconRainFog, WeatherIcon.IconRain, "rainyfrostfog", Translation.Get("weather_frostTwo", new { condition = Translation.Get("weather_rainy"), condtitionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyfrostfog", Translation.Get("weather_frost", new { condition = Translation.Get("weather_stormy"), conditionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconRainFog, WeatherIcon.IconRain, "rainyheatwavefog", Translation.Get("weather_heatwaveTwo", new { condition = Translation.Get("weather_rainy"), conditionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconStormFog, WeatherIcon.IconStorm, "stormyheatwavefog", Translation.Get("weather_heatwaveTwo", new { condition = Translation.Get("weather_stormy"), conditionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "fogheatwave", CondDesc: Translation.Get("weather_fog", new { condition = Translation.Get("weather_heatwave") })) },

                { (int)(CurrentWeather.Fog | CurrentWeather.Frost),  new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "fogfrost", CondDesc: Translation.Get("weather_frost", new { condition = Translation.Get("weather_fog_basic") }))  },

                { (int)(CurrentWeather.Rain | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconRain, WeatherIcon.IconRain, "rainHeatwave", Translation.Get("weather_heatwaveCond", new { condition = Translation.Get("weather_rainy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconRain, WeatherIcon.IconRain, "rainFrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_rainy") })) },

                { (int)(CurrentWeather.Snow), new WeatherData(WeatherIcon.IconSnow, WeatherIcon.IconSnow, "snowy", Translation.Get("weather_snow")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSnow, WeatherIcon.IconSnow, "snowyFrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_snow") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconSnowFog, WeatherIcon.IconSnow, "snowyFog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_snow") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSnowFog, WeatherIcon.IconSnow, "snowyFrostFog", Translation.Get("weather_frostTwo", new { condition = Translation.Get("weather_snow"), conditionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconThunderSnowFog, WeatherIcon.IconSnow, "thunderSnowFog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_thundersnow") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconThunderSnowFog, WeatherIcon.IconSnow, "thunderSnowFrostFog", Translation.Get("weather_frostTwo", new { condition = Translation.Get("weather_thundersnow"), conditionB = Translation.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconThunderSnowFog, WeatherIcon.IconSnow, "thunderSnowFrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_thundersnow") })) },

                { (int)(CurrentWeather.Festival), new WeatherData(WeatherIcon.IconFestival, WeatherIcon.IconFestival, "festival", Translation.Get("weather_festival")) },

                { (int)(CurrentWeather.Wedding), new WeatherData(WeatherIcon.IconWedding, WeatherIcon.IconWedding, "wedding", Translation.Get("weather_wedding")) }, 

                { (int)(CurrentWeather.Wind), new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debris", Translation.Get("weather_wind")) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisfrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_wind") })) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisheatwave", Translation.Get("weather_heatwaveCond", new { condition = Translation.Get("weather_wind") })) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightningWind, WeatherIcon.IconSpringDebris, "drylightningwindy", Translation.Get("weather_drylightningwindy")) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Heatwave | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightningWind, WeatherIcon.IconSpringDebris, "drylightningheatwave", Translation.Get("weather_drylightningheatwavewindy")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard), new WeatherData(WeatherIcon.IconBlizzard, WeatherIcon.IconBlizzard, "blizzard", Translation.Get("weather_blizzard")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut), new WeatherData(WeatherIcon.IconWhiteOut, WeatherIcon.IconWhiteOut, "whiteout", Translation.Get("weather_whiteOut")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBlizzardFog, WeatherIcon.IconBlizzard, "blizzardFog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_blizzard") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconWhiteOutFog, WeatherIcon.IconWhiteOut, "whiteoutFog", Translation.Get("weather_fog", new { condition = Translation.Get("weather_whiteOut") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconBlizzard, WeatherIcon.IconBlizzard, "blizzardFog", Translation.Get("weather_frost", new { condition = Translation.Get("weather_blizzard") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconWhiteOut, WeatherIcon.IconWhiteOut, "whiteoutFog", Translation.Get("weather_frost", new { condition = Translation.Get("weather_whiteOut") })) },

                { (int)(CurrentWeather.BloodMoon | CurrentWeather.Frost | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonFrostFog", Translation.Get("weather_fog", new {condition = Translation.Get("weather_bloodmoon")}) )},

                { (int)(CurrentWeather.BloodMoon |CurrentWeather.Heatwave | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonHWFog", Translation.Get("weather_fog", new {condition = Translation.Get("weather_bloodmoon")}) )},

                { (int)(CurrentWeather.BloodMoon | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonFog", Translation.Get("weather_fog", new {condition = Translation.Get("weather_bloodmoon")}) )},
                { (int)(CurrentWeather.BloodMoon), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoon", Translation.Get("weather_bloodmoon"))},

                { (int)(CurrentWeather.BloodMoon| CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonHeatwave", Translation.Get("weather_bloodmoon"))},

                { (int)(CurrentWeather.BloodMoon| CurrentWeather.Frost), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonFrost", Translation.Get("weather_bloodmoon"))}

            };
        }

        /// *************************************************************************
        /// ACCESS METHODS
        /// *************************************************************************
        public CurrentWeather GetCurrentConditions()
        {
            return CurrentConditionsN;
        }

        public void ClearAllSpecialWeather()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.EndWeather();
        }

        public void DrawWeathers()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.DrawWeather();

            //if it's a blood moon out..
            if (ClimatesOfFerngill.MoonAPI != null && ClimatesOfFerngill.MoonAPI.GetCurrentMoonPhase() == "Blood Moon")
            {
                if (this.GetWeatherMatchingType("Fog").First().IsWeatherVisible)
                {
                    //Get fog instance
                    FerngillFog ourFog = (FerngillFog)this.GetWeatherMatchingType("Fog").First();
                    ourFog.BloodMoon = true;
                }

                if (this.GetWeatherMatchingType("Blizzard").First().IsWeatherVisible)
                {
                    //Get Blizzard instance
                    FerngillBlizzard blizzard = (FerngillBlizzard)this.GetWeatherMatchingType("Blizzard").First();
                    blizzard.IsBloodMoon = true;
                }
            }
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

        public void CreateWeather(string Type)
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather.WeatherType == Type)
                    weather.CreateWeather();
            }
        }

        public void TenMinuteUpdate()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                weather.UpdateWeather();
            }

            //update fog for the evening
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

        public void SecondUpdate()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                weather.SecondUpdate();
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

            //Some flags are contradictory. Fix that here.
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

        public bool HasPrecip()
        {
            if (CurrentConditionsN.HasAnyFlags(CurrentWeather.Snow | CurrentWeather.Rain | CurrentWeather.Blizzard))
                return true;

            return false;
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

            if (e.Weather == "ThunderFrenzy")
            {
                if (e.Present)
                {
                    CurrentConditionsN = CurrentConditionsN | CurrentWeather.ThunderFrenzy;
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.ThunderFrenzy);
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

            CurrentConditionsN = CurrentWeather.Unset;
            TodayTemps = TomorrowTemps; //If Tomorrow is null, should just allow it to be null.
            TomorrowTemps = null;

            if (Game1.currentSeason == "fall")
            {
                Weathers[(int)CurrentWeather.Wind] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "debris", Translation.Get("weather_wind"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Frost)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "debrisfrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "debrisheatwave", Translation.Get("weather_heatwaveCond", new { condition = Translation.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "drylightningwindy", Translation.Get("weather_drylightningwindy"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "drylightningheatwave", Translation.Get("weather_drylightningheatwavewindy"));
            }
            else
            {
                //reset back to spring. 
                Weathers[(int)CurrentWeather.Wind] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debris", Translation.Get("weather_wind"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Frost)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisfrost", Translation.Get("weather_frost", new { condition = Translation.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisheatwave", Translation.Get("weather_heatwaveCond", new { condition = Translation.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "drylightningwindy", Translation.Get("weather_drylightningwindy"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "drylightningheatwave", Translation.Get("weather_drylightningheatwavewindy"));
            }
        }

        /// <summary> This function resets the weather object to basic. </summary>
        public void Reset()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.Reset();

            TodayTemps = null;
            TomorrowTemps = null;
            CurrentConditionsN = CurrentWeather.Unset;
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
            {
                AddWeather(CurrentWeather.Sunny);
            }

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

        /// <summary>
        /// This function returns a description of the object. A very important note that this is meant for debugging, and as such does not do localization.
        /// </summary>
        /// <returns>A string describing the object.</returns>
        public override string ToString()
        {
            string ret = "";
            ret += $"Low for today is {TodayTemps?.LowerBound:N3} with the high being {TodayTemps?.HigherBound:N3}. The current conditions are {Weathers[(int)CurrentConditionsN].ConditionName}.";

            foreach (ISDVWeather weather in CurrentWeathers)
                ret += weather.ToString() + Environment.NewLine;
            
            ret += $"Weather set for tommorow is {Weathers[(int)(WeatherConditions.ConvertToCurrentWeather(Game1.weatherForTomorrow))].ConditionName} with high {TomorrowTemps?.HigherBound:N3} and low {TomorrowTemps?.LowerBound:N3}. Evening fog generated {GenerateEveningFog} ";

            return ret;
        }
        
        internal bool TestForSpecialWeather(FerngillClimateTimeSpan ClimateForDay)
        {
            bool specialWeatherTriggered = false;
            // Conditions: Blizzard - occurs in weather_snow in "winter"
            //             Dry Lightning - occurs if it's sunny in any season if temps exceed 25C.
            //             Frost and Heatwave check against the configuration.
            //             Thundersnow  - as Blizzard, but really rare.
            //             Fog - per climate, although night fog in winter is double normal chance
            GenerateEveningFog = (Dice.NextDouble() < ClimateForDay.EveningFogChance * ClimateForDay.RetrieveOdds(Dice,"fog",Game1.dayOfMonth)) && !this.GetCurrentConditions().HasFlag(CurrentWeather.Wind);

            bool blockFog = ClimatesOfFerngill.MoonAPI != null && ClimatesOfFerngill.MoonAPI.IsSolarEclipse();

            if (blockFog)
                GenerateEveningFog = false;
            
            double fogRoll = Dice.NextDoublePositive();
           
            if (fogRoll < ClimateForDay.RetrieveOdds(Dice, "fog", Game1.dayOfMonth) && !this.GetCurrentConditions().HasFlag(CurrentWeather.Wind) && !blockFog)
            {
                this.CreateWeather("Fog");

                if (ModConfig.Verbose)
                    Monitor.Log($"{FogDescription(fogRoll, ClimateForDay.RetrieveOdds(Dice, "fog", Game1.dayOfMonth))}");

                specialWeatherTriggered = true;
            }

            if (this.HasWeather(CurrentWeather.Snow))
            {
                double blizRoll = Dice.NextDoublePositive();
                if (blizRoll <= ModConfig.BlizzardOdds)
                {
                    this.CreateWeather("Blizzard");
                    if (ModConfig.Verbose)
                        Monitor.Log($"With roll {blizRoll:N3} against {ModConfig.BlizzardOdds}, there will be blizzards today");
                    if (Dice.NextDoublePositive() < .05 && ModConfig.HazardousWeather)
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
                        Monitor.Log($"With roll {oddsRoll:N3} against {ModConfig.ThundersnowOdds}, there will be thundersnow today");

                    specialWeatherTriggered = true;
                }
            }

            if (!(this.HasPrecip()))
            {
                double oddsRoll = Dice.NextDoublePositive();

                if (oddsRoll <= ModConfig.DryLightning && this.TodayHigh >= ModConfig.DryLightningMinTemp &&
                    !this.CurrentConditionsN.HasFlag(CurrentWeather.Frost))
                {
                    this.AddWeather(CurrentWeather.Lightning);
                    if (ModConfig.Verbose)
                        Monitor.Log($"With roll {oddsRoll:N3} against {ModConfig.DryLightning}, there will be dry lightning today.");

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

            //and finally, test for thunder frenzy
            if (this.HasWeather(CurrentWeather.Lightning) && this.HasWeather(CurrentWeather.Rain) && ModConfig.HazardousWeather)
            {
                double oddsRoll = Dice.NextDouble();
                if (oddsRoll < ModConfig.ThunderFrenzyOdds)
                {
                    this.AddWeather(CurrentWeather.ThunderFrenzy);
                    specialWeatherTriggered = true;
                    if (ModConfig.Verbose)
                        Monitor.Log($"With roll {oddsRoll:N3} against {ModConfig.ThunderFrenzyOdds}, there will be a thunder frenzy today");
                    this.CreateWeather("ThunderFrenzy");
                }
            }

            return specialWeatherTriggered;
        }
    }
}
