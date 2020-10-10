/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using EnumsNET;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
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

        /// <summary>Track today's temperature</summary>
        private RangePair TodayTemps;

        /// <summary>Track tomorrow's temperature</summary>
        private RangePair TomorrowTemps;

        /// <summary>Track current conditions</summary>
        private CurrentWeather CurrentConditionsN { get; set; }

        /// <summary>The list of custom weathers </summary>
        internal List<ISDVWeather> CurrentWeathers { get; set; }
        internal bool IsVariableRain { get; private set; }
        internal bool IsOvercast { get; set; }
        internal int AmtOfRainDrops { get; private set; }
        internal RainLevels StartingRain { get; set; }

        //evening fog details
        private bool HasSetEveningFog { get; set; }
        public bool GenerateEveningFog { get; set; }
        internal ClimateTracker trackerModel;
        internal int TodayRain;
        private int TenMCounter;

        public bool IsAbnormalHeat { get; set; }
        public bool IsAbnormalChill { get; set; }

        /// ******************************************************************************
        /// CONSTRUCTORS
        /// ******************************************************************************

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WeatherConditions()
        {
            TenMCounter = 0;
            Weathers = PopulateWeathers();
  	        IsOvercast = false;
            IsAbnormalHeat = false;
	        IsAbnormalChill = false;
            IsVariableRain = false;
		
	    trackerModel = new ClimateTracker();
		
            CurrentConditionsN = CurrentWeather.Unset;
            StartingRain = RainLevels.None;
            CurrentWeathers = new List<ISDVWeather>
            {
                new FerngillFog(SDVTimePeriods.Morning),
                new FerngillWhiteOut(),
                new FerngillBlizzard(),
                new FerngillThunderFrenzy(),
                new FerngillSandstorm()
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
                { (int)CurrentWeather.Sunny, new WeatherData(WeatherIcon.IconSunny, WeatherIcon.IconSunny, "sunny", ClimatesOfFerngill.Translator.Get("weather_sunny_daytime"), CondDescNight: ClimatesOfFerngill.Translator.Get("weather_sunny_nighttime"))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightning", ClimatesOfFerngill.Translator.Get("weather_drylightning"))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSunny, WeatherIcon.IconSunny, CondName: "sunnyfrost", CondDesc: ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_sunny") }), CondDescNight: ClimatesOfFerngill.Translator.Get("weather_frost_night")) },

                { (int)(CurrentWeather.Sunny | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSunny, WeatherIcon.IconSunny, CondName: "sunnyheatwave", CondDesc: ClimatesOfFerngill.Translator.Get("weather_heatwave")) },

                { (int)(CurrentWeather.Sunny | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "fog", CondDesc: ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_sunny") }), CondDescNight: ClimatesOfFerngill.Translator.Get("weather_frost_night")) },

                { (int)(CurrentWeather.Sunny | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "sunnyfrostfog", CondDesc: ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_fog_basic") }))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Frost | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_drylightning") }))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Fog | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightningFog, WeatherIcon.IconSunny, CondName: "drylightningfog", CondDesc: ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_drylightning") }))},

                { (int)(CurrentWeather.Sunny | CurrentWeather.Heatwave | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningheatwave", ClimatesOfFerngill.Translator.Get("weather_drylightningheatwave"))},

                { (int)(CurrentWeather.Frost | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_drylightning") }))},

                { (int)(CurrentWeather.Heatwave | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightningheatwave", ClimatesOfFerngill.Translator.Get("weather_drylightningheatwave"))},

                { (int)(CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightning, WeatherIcon.IconDryLightning, "drylightning", ClimatesOfFerngill.Translator.Get("weather_drylightning"))},

                { (int)(CurrentWeather.Unset), new WeatherData(WeatherIcon.IconError, WeatherIcon.IconError, "unset", ClimatesOfFerngill.Translator.Get("weather_unset"))},

                { (int)(CurrentWeather.Rain), new WeatherData(WeatherIcon.IconRain, WeatherIcon.IconRain, "rainy", ClimatesOfFerngill.Translator.Get("weather_rainy")) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormy", ClimatesOfFerngill.Translator.Get("weather_stormy")) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { conditions = ClimatesOfFerngill.Translator.Get("weather_stormy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyheatwave", ClimatesOfFerngill.Translator.Get("weather_heatwaveCond", new { conditions = ClimatesOfFerngill.Translator.Get("weather_stormy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconRainFog, WeatherIcon.IconRain, "rainyfog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_rainy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyfog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_stormy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfog", ClimatesOfFerngill.Translator.Get("weather_frenzy")) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Frost | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_frenzy")})) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Heatwave | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyheatwave", ClimatesOfFerngill.Translator.Get("weather_heatwave", new { condition = ClimatesOfFerngill.Translator.Get("weather_frenzy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_frenzy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Frost | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfogfrost", ClimatesOfFerngill.Translator.Get("weather_frostTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_frenzy"), condtitionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Heatwave | CurrentWeather.ThunderFrenzy), new WeatherData(WeatherIcon.IconThunderFrenzy, WeatherIcon.IconStorm, "lightfrenzyfogheatwave", ClimatesOfFerngill.Translator.Get("weather_heatwaveTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_frenzy"), condtitionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconRainFog, WeatherIcon.IconRain, "rainyfrostfog", ClimatesOfFerngill.Translator.Get("weather_frostTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_rainy"), condtitionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconStorm, WeatherIcon.IconStorm, "stormyfrostfog", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_stormy"), conditionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconRainFog, WeatherIcon.IconRain, "rainyheatwavefog", ClimatesOfFerngill.Translator.Get("weather_heatwaveTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_rainy"), conditionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconStormFog, WeatherIcon.IconStorm, "stormyheatwavefog", ClimatesOfFerngill.Translator.Get("weather_heatwaveTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_stormy"), conditionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "fogheatwave", CondDesc: ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_heatwave") })) },

                { (int)(CurrentWeather.Fog | CurrentWeather.Frost),  new WeatherData(WeatherIcon.IconSunnyFog, WeatherIcon.IconSunny, CondName: "fogfrost", CondDesc: ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_fog_basic") }))  },

                { (int)(CurrentWeather.Rain | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconRain, WeatherIcon.IconRain, "rainHeatwave", ClimatesOfFerngill.Translator.Get("weather_heatwaveCond", new { condition = ClimatesOfFerngill.Translator.Get("weather_rainy") })) },

                { (int)(CurrentWeather.Rain | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconRain, WeatherIcon.IconRain, "rainFrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_rainy") })) },

                { (int)(CurrentWeather.Snow), new WeatherData(WeatherIcon.IconSnow, WeatherIcon.IconSnow, "snowy", ClimatesOfFerngill.Translator.Get("weather_snow")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSnow, WeatherIcon.IconSnow, "snowyFrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_snow") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconSnowFog, WeatherIcon.IconSnow, "snowyFog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_snow") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSnowFog, WeatherIcon.IconSnow, "snowyFrostFog", ClimatesOfFerngill.Translator.Get("weather_frostTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_snow"), conditionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconThunderSnow, WeatherIcon.IconSnow, "thunderSnow",ClimatesOfFerngill.Translator.Get("weather_thundersnow")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.Lightning | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconThunderSnow, WeatherIcon.IconSnow, "blizzardThunderSnowFrost",ClimatesOfFerngill.Translator.Get("weather_thundersnow")) },
                
                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Frost | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconThunderSnow, WeatherIcon.IconSnow, "whiteOutBlizzardFrostThunderSnow",ClimatesOfFerngill.Translator.Get("weather_thundersnow")) },
                
                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconThunderSnow, WeatherIcon.IconSnow, "blizThunderSnow",ClimatesOfFerngill.Translator.Get("weather_thundersnow")) },
                
                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconThunderSnow, WeatherIcon.IconSnow, "whiteOutThunderSnow",ClimatesOfFerngill.Translator.Get("weather_thundersnow")) },
                
                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconThunderSnowFog, WeatherIcon.IconSnow, "thunderSnowFog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_thundersnow") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning | CurrentWeather.Fog | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconThunderSnowFog, WeatherIcon.IconSnow, "thunderSnowFrostFog", ClimatesOfFerngill.Translator.Get("weather_frostTwo", new { condition = ClimatesOfFerngill.Translator.Get("weather_thundersnow"), conditionB = ClimatesOfFerngill.Translator.Get("weather_fog_basic") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Lightning | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconThunderSnowFog, WeatherIcon.IconSnow, "thunderSnowFrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_thundersnow") })) },

                { (int)(CurrentWeather.Festival), new WeatherData(WeatherIcon.IconFestival, WeatherIcon.IconFestival, "festival", ClimatesOfFerngill.Translator.Get("weather_festival")) },

                { (int)(CurrentWeather.Wedding), new WeatherData(WeatherIcon.IconWedding, WeatherIcon.IconWedding, "wedding", ClimatesOfFerngill.Translator.Get("weather_wedding")) }, 

                { (int)(CurrentWeather.Wind), new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debris", ClimatesOfFerngill.Translator.Get("weather_wind")) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_wind") })) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisheatwave", ClimatesOfFerngill.Translator.Get("weather_heatwaveCond", new { condition = ClimatesOfFerngill.Translator.Get("weather_wind") })) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightningWind, WeatherIcon.IconSpringDebris, "drylightningwindy", ClimatesOfFerngill.Translator.Get("weather_drylightningwindy")) },

                { (int)(CurrentWeather.Wind | CurrentWeather.Heatwave | CurrentWeather.Lightning), new WeatherData(WeatherIcon.IconDryLightningWind, WeatherIcon.IconSpringDebris, "drylightningheatwave", ClimatesOfFerngill.Translator.Get("weather_drylightningheatwavewindy")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard), new WeatherData(WeatherIcon.IconBlizzard, WeatherIcon.IconBlizzard, "blizzard", ClimatesOfFerngill.Translator.Get("weather_blizzard")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut), new WeatherData(WeatherIcon.IconWhiteOut, WeatherIcon.IconWhiteOut, "whiteout", ClimatesOfFerngill.Translator.Get("weather_whiteOut")) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBlizzardFog, WeatherIcon.IconBlizzard, "blizzardFog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_blizzard") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconWhiteOutFog, WeatherIcon.IconWhiteOut, "whiteoutFog", ClimatesOfFerngill.Translator.Get("weather_fog", new { condition = ClimatesOfFerngill.Translator.Get("weather_whiteOut") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconBlizzard, WeatherIcon.IconBlizzard, "blizzardFog", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_blizzard") })) },

                { (int)(CurrentWeather.Snow | CurrentWeather.Blizzard | CurrentWeather.WhiteOut | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconWhiteOut, WeatherIcon.IconWhiteOut, "whiteoutFog", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_whiteOut") })) },

                { (int)(CurrentWeather.BloodMoon | CurrentWeather.Frost | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonFrostFog", ClimatesOfFerngill.Translator.Get("weather_fog", new {condition = ClimatesOfFerngill.Translator.Get("weather_bloodmoon")}) )},

                { (int)(CurrentWeather.BloodMoon |CurrentWeather.Heatwave | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonHWFog", ClimatesOfFerngill.Translator.Get("weather_fog", new {condition = ClimatesOfFerngill.Translator.Get("weather_bloodmoon")}) )},

                { (int)(CurrentWeather.BloodMoon | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonFog", ClimatesOfFerngill.Translator.Get("weather_fog", new {condition = ClimatesOfFerngill.Translator.Get("weather_bloodmoon")}) )},
                { (int)(CurrentWeather.BloodMoon), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoon", ClimatesOfFerngill.Translator.Get("weather_bloodmoon"))},

                { (int)(CurrentWeather.BloodMoon| CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonHeatwave", ClimatesOfFerngill.Translator.Get("weather_bloodmoon"))},

                { (int)(CurrentWeather.BloodMoon| CurrentWeather.Frost), new WeatherData(WeatherIcon.IconBloodMoon, WeatherIcon.IconBloodMoon, "bloodMoonFrost", ClimatesOfFerngill.Translator.Get("weather_bloodmoon"))},

                { (int)(CurrentWeather.Sandstorm), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstorm", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Wind), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstormWind", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Wind | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstormWindHW", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Wind | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstormWindFrost", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Sunny), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstorm", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Wind | CurrentWeather.Sunny), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstormWind", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Wind | CurrentWeather.Heatwave| CurrentWeather.Sunny), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstormWindHW", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Sandstorm | CurrentWeather.Wind | CurrentWeather.Frost| CurrentWeather.Sunny), new WeatherData(WeatherIcon.IconSandstorm, WeatherIcon.IconSandstorm, "sandstormWindFrost", ClimatesOfFerngill.Translator.Get("weather_sandstorm"))},
                { (int)(CurrentWeather.Overcast), new WeatherData(WeatherIcon.IconOvercast, WeatherIcon.IconOvercast, "overcast", ClimatesOfFerngill.Translator.Get("weather_overcast"))},
                { (int)(CurrentWeather.Overcast | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconOvercast, WeatherIcon.IconOvercast, "overcastHeatwave", ClimatesOfFerngill.Translator.Get("weather_overcast"))},
                { (int)(CurrentWeather.Overcast | CurrentWeather.Frost), new WeatherData(WeatherIcon.IconOvercast, WeatherIcon.IconOvercast, "overcastFrost", ClimatesOfFerngill.Translator.Get("weather_overcast"))},
                { (int)(CurrentWeather.Overcast | CurrentWeather.Fog | CurrentWeather.Heatwave), new WeatherData(WeatherIcon.IconOvercast, WeatherIcon.IconOvercast, "overcastHeatwaveFog", ClimatesOfFerngill.Translator.Get("weather_fog", new {condition = ClimatesOfFerngill.Translator.Get("weather_overcast")}))},
                { (int)(CurrentWeather.Overcast | CurrentWeather.Frost | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconOvercast, WeatherIcon.IconOvercast, "overcastFrostFog",  ClimatesOfFerngill.Translator.Get("weather_fog", new {condition = ClimatesOfFerngill.Translator.Get("weather_overcast")}))},
                { (int)(CurrentWeather.Overcast | CurrentWeather.Fog), new WeatherData(WeatherIcon.IconOvercast, WeatherIcon.IconOvercast, "overcastFog",  ClimatesOfFerngill.Translator.Get("weather_fog", new {condition = ClimatesOfFerngill.Translator.Get("weather_overcast")}))}
            };
        }

        /// *************************************************************************
        /// ACCESS METHODS
        /// *************************************************************************
        /// <summary>Rather than track the weather separately, always get it from the game.</summary>
        public CurrentWeather TomorrowForecast => ConvertToCurrentWeather(Game1.weatherForTomorrow);
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
		
        public bool IsFoggy()
        {
            if (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Fog").First().IsWeatherVisible)
                return true;

            return false;
        }

        public bool IsThunderFrenzy()
        {
            if (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("ThunderFrenzy").First().IsWeatherVisible)
                return true;

            return false;
        }

        public bool IsBlizzard()
        {
            if (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Blizzard").First().IsWeatherVisible)
                return true;

            return false;
        }
        
        public bool IsWhiteOut()
        {
            if (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("WhiteOut").First().IsWeatherVisible)
                return true;

            return false;
        }

        public bool IsSandstorm()
        {
            if (ClimatesOfFerngill.Conditions.GetWeatherMatchingType("Sandstorm").First().IsWeatherVisible)
                return true;

            return false;
        }

        public double GetCurrentTemperature(int timeOfDay)
        {
            double temp;
            //first, we should get the cyclic times.
            int highMin = SDVTime.ConvertTimeToMinutes(1530), currMin = SDVTime.ConvertTimeToMinutes(timeOfDay), lowMin, nextLowMin;
            int highTime = 1530, lowTime, nextLowTime;

            //first, get the time the high will peak. This generally will be 3-4pm. Changes are due to location.
            if (ClimatesOfFerngill.DynamicNightAPI != null)
            {
                lowTime = ClimatesOfFerngill.DynamicNightAPI.GetSunriseTime() - 100;
                nextLowTime = ClimatesOfFerngill.DynamicNightAPI.GetAnySunriseTime(SDate.Now().AddDays(1)) + 2300;
                nextLowMin = SDVTime.ConvertTimeToMinutes(nextLowTime);
            }
            else
            {
                lowTime = 0500;
                nextLowMin = 2900;
            }
            lowMin = SDVTime.ConvertTimeToMinutes(lowTime);

            if (ClimatesOfFerngill.WeatherOpt.Verbose)
            {
                ClimatesOfFerngill.Logger.Log($"Calculations: Low Time is {lowTime}, with minute count {lowMin}, highTime is {highTime}, highMin is {highMin}. The next low time is {nextLowMin}. Current time is {timeOfDay}, with min count {currMin}");
            }

            //using straight linear interpolation.
            double range = TodayHigh - TodayLow, mRange;
            if (timeOfDay < lowTime)
            {
                mRange = lowMin;
                temp = (((lowMin - currMin) / mRange) * range) + TodayLow;

                ClimatesOfFerngill.Logger.Log($"Calculations: Minute Range is {mRange}, with the calc time difference being {lowMin - currMin}, resulting in a calculated temp being {temp}");
            }
            else if (timeOfDay == lowTime)
                temp = TodayLow;
            else if (timeOfDay > lowTime && timeOfDay < highTime)
            {
                mRange = highMin - lowMin;
                temp = (((highMin - currMin) / mRange) * range) + TodayLow;
                ClimatesOfFerngill.Logger.Log($"Calculations: Minute Range is {mRange}, with the calc time difference being {lowMin - currMin}, resulting in a calculated temp being {temp}");
            }
            else if (timeOfDay == highTime)
                temp = TodayHigh;
            else
            {
                mRange = nextLowMin - highMin;
                temp = (((nextLowMin - currMin) / mRange) * range) +TodayLow;
                ClimatesOfFerngill.Logger.Log($"Calculations: Minute Range is {mRange}, with the calc time difference being {lowMin - currMin}, resulting in a calculated temp being {temp}");
            }
            return temp;
        }

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

        public void CreateWeather(string type)
        {
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather.WeatherType == type)
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
                var ourFog = (FerngillFog)this.GetWeatherMatchingType("Fog").First();
                if (!ourFog.WeatherInProgress)
                {
                    ourFog.SetEveningFog();
                    GenerateWeatherSync();
                    HasSetEveningFog = true;
                }
            }

            TodayRain += AmtOfRainDrops;
            TenMCounter++;

            if (TenMCounter == 3 && IsVariableRain)
            {
                UpdateDynamicRain();
                TenMCounter = 0;
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
            var weathers = new List<ISDVWeather>();
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather.WeatherType == type)
                    weathers.Add(weather);
            }

            return weathers;
        }

        public void RemoveWeather(CurrentWeather weather)
        {
            CurrentConditionsN.RemoveFlags(weather);
        }

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
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Snow);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Blizzard);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sunny);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Snow)
            {
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sunny);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Rain);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Wind);

                CurrentConditionsN |= newWeather;
            }

            else if (newWeather == CurrentWeather.Overcast)
            {
                //remove lightning, rain and wind
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sunny);
                CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Lightning);
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

        internal void UpdateClimateTracker()
        {
            if (trackerModel is null)
            {
                trackerModel = new ClimateTracker();
            }
            if (this.HasWeather(CurrentWeather.Rain))
            {
                trackerModel.DaysSinceRainedLast = 0;
            }
            else
                trackerModel.DaysSinceRainedLast++;

            //update the trackerModel some more
            if (NormalizedWeatherName() == trackerModel.CurrentStreak.CurrentWeather)
            {
                trackerModel.CurrentStreak.NumDaysOfWeather++;
            }
            else
            {
                trackerModel.CurrentStreak.CurrentWeather = NormalizedWeatherName();
                trackerModel.CurrentStreak.NumDaysOfWeather = 1;
            }
			
			if (trackerModel.WeatherSystemDays > 0 && !trackerModel.IsWeatherSystem)
				trackerModel.WeatherSystemDays = 0;

            if (TomorrowTemps != null)
            {
                trackerModel.TempsOnNextDay = new RangePair(TomorrowTemps);
            }
            else
            {
                var tomorrow = SDate.Now().AddDays(1);
                SetTomorrowTemps(ClimatesOfFerngill.GetClimateForDay(tomorrow).GetTemperatures(ClimatesOfFerngill.Dice, tomorrow.Day));
                trackerModel.TempsOnNextDay = new RangePair(TomorrowTemps);
            }
        }

        internal void ForceEveningFog()
        {
            //Get fog instance
            var fogWeather = this.GetWeatherMatchingType("Fog");
            foreach (ISDVWeather weather in fogWeather)
            {
                var beginTime = new SDVTime(Game1.getStartingToGetDarkTime());
                beginTime.AddTime(ClimatesOfFerngill.Dice.Next(-15, 90));

                var expirationTime = new SDVTime(beginTime);
                expirationTime.AddTime(ClimatesOfFerngill.Dice.Next(120, 310));

                beginTime.ClampToTenMinutes();
                expirationTime.ClampToTenMinutes();
                weather.SetWeatherTime(beginTime, expirationTime);
            }
        }

        /// <summary> Syntactic Sugar for Enum.HasFlag(). Done so if I choose to rewrite how it's accessed, less rewriting of invoking functions is needed. </summary>
        /// <param name="checkWeather">The weather being checked.</param>
        /// <returns>If the weather is present</returns>
        public bool HasWeather(CurrentWeather checkWeather)
        {
            return CurrentConditionsN.HasFlag(checkWeather);
        }

        public bool HasPrecip()
        {
            if (CurrentConditionsN.HasAnyFlags(CurrentWeather.Snow | CurrentWeather.Rain | CurrentWeather.Blizzard) && !CurrentConditionsN.HasFlag(CurrentWeather.Overcast))
                return true;

            return false;
        }

        internal RangePair GetTodayTemps() => TodayTemps;
        internal RangePair GetTomorrowTemps() => TomorrowTemps;
  
        private void ProcessWeatherChanges(object sender, WeatherNotificationArgs e)
        {
            if (e.Weather == "WhiteOut")
            {
                if (e.Present)
                {
                    CurrentConditionsN |= CurrentWeather.WhiteOut;
                    this.GenerateWeatherSync();
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.WhiteOut);
                    this.GenerateWeatherSync();
                }
            }

            if (e.Weather == "ThunderFrenzy")
            {
                if (e.Present)
                {
                    CurrentConditionsN |= CurrentWeather.ThunderFrenzy;
                    this.GenerateWeatherSync();
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.ThunderFrenzy);
                    this.GenerateWeatherSync();
                }
            }

            if (e.Weather == "Sandstorm")
            {
                if (e.Present)
                { 
                    CurrentConditionsN |= CurrentWeather.Sandstorm;
                    this.GenerateWeatherSync();
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Sandstorm);
                    this.GenerateWeatherSync();
                }
            }

            if (e.Weather == "Fog")
            {
                if (e.Present)
                {
                    CurrentConditionsN |= CurrentWeather.Fog;
                    this.GenerateWeatherSync();
                }
                else
                {
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Fog);
                    this.GenerateWeatherSync();
                }
            }

            if (e.Weather == "Blizzard")
            {
                if (e.Present) { 
                    CurrentConditionsN |= CurrentWeather.Blizzard;
                    this.GenerateWeatherSync();
                }

                else { 
                    CurrentConditionsN = CurrentConditionsN.RemoveFlags(CurrentWeather.Blizzard);
                    this.GenerateWeatherSync();
                }
            }
        }

        internal FogType GetCurrentFogType()
        {
            FerngillFog f = CurrentWeathers.FirstOrDefault(c => c.WeatherType == "Fog") as FerngillFog;
            
            if (f is null)
                return FogType.None;
            else
                return f.CurrentFogType;
        }

        internal string GetCurrentFogTypeDesc()
        {
            FerngillFog f = CurrentWeathers.FirstOrDefault(c => c.WeatherType == "Fog") as FerngillFog;

            if (f is null)
                return "None";
            else
                return FerngillFog.DescFogType(f.CurrentFogType);
        }

        internal bool GetWeatherStatus(string weather)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == weather)
                    return w.WeatherInProgress;
            }
            return false;
        }

        internal SDVTime GetWeatherBeginTime(string weather)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == weather)
                    return w.WeatherBeginTime;
            }
            return new SDVTime(0600);
        }

        internal SDVTime GetWeatherEndTime(string weather)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == weather)
                    return w.WeatherExpirationTime;
            }

            return new SDVTime(0600);
        }

        internal void SetWeatherBeginTime(string weather, int weatherTime)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == weather)
                    w.SetWeatherBeginTime(new SDVTime(weatherTime));
            }
        }

        internal void SetWeatherEndTime(string weather, int weatherTime)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == weather)
                    w.SetWeatherExpirationTime(new SDVTime(weatherTime));
            }
        }

        public void GenerateWeatherSync()
        {
            if (!Context.IsMainPlayer)
                return;

            WeatherSync message = GenerateWeatherSyncMessage();

            ClimatesOfFerngill.MPHandler.SendMessage<WeatherSync>(message, "WeatherSync", new [] { "KoihimeNakamura.ClimatesOfFerngill" });    
        }

        internal void SetTodayTempsFromTomorrow()
        {
            TodayTemps.LowerBound = TomorrowTemps.LowerBound;
            TodayTemps.HigherBound = TomorrowTemps.HigherBound;
        }

        public WeatherSync GenerateWeatherSyncMessage()
        {
            WeatherSync message = new WeatherSync
            {
                weatherType = WeatherUtilities.GetWeatherCode(),
                isFoggy = GetWeatherStatus("Fog"),
                isThunderFrenzy = GetWeatherStatus("ThunderFrenzy"),
                isWhiteOut = GetWeatherStatus("WhiteOut"),
                isBlizzard = GetWeatherStatus("Blizzard"),
                isSandstorm = GetWeatherStatus("Sandstorm"),
                isOvercast = ((CurrentConditionsN & CurrentWeather.Overcast) != 0),
                isVariableRain = IsVariableRain,
                fogWeatherBeginTime = GetWeatherBeginTime("Fog").ReturnIntTime(),
                thunWeatherBeginTime = GetWeatherBeginTime("ThunderFrenzy").ReturnIntTime(),
                blizzWeatherBeginTime = GetWeatherBeginTime("Blizzard").ReturnIntTime(),
                whiteWeatherBeginTime = GetWeatherBeginTime("WhiteOut").ReturnIntTime(),
                sandstormWeatherBeginTime = GetWeatherBeginTime("Sandstorm").ReturnIntTime(),
                sandstormWeatherEndTime = GetWeatherEndTime("Sandstorm").ReturnIntTime(),
                fogWeatherEndTime = GetWeatherEndTime("Fog").ReturnIntTime(),
                thunWeatherEndTime = GetWeatherEndTime("ThunderFrenzy").ReturnIntTime(),
                blizzWeatherEndTime = GetWeatherEndTime("Blizzard").ReturnIntTime(),
                whiteWeatherEndTime = GetWeatherEndTime("WhiteOut").ReturnIntTime(),
                todayHigh = TodayTemps.HigherBound,
                todayLow = TodayTemps.LowerBound,
                StartingRain = StartingRain,
                rainAmt = AmtOfRainDrops,
                tommorowHigh = TomorrowHigh,
                isAbnormalHeat = IsAbnormalHeat,
                isAbnormalChill = IsAbnormalChill,
                tommorowLow = TomorrowLow,
                currTracker = new ClimateTracker(trackerModel)
            };

            return message;
        }

        public void ForceWeatherStart(string s)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == s)
                    w.ForceWeatherStart();
            }
        }

        public void ForceWeatherEnd(string s)
        {
            foreach (ISDVWeather w in this.CurrentWeathers)
            {
                if (w.WeatherType == s)
                    w.ForceWeatherEnd();
            }
        }

        public void HaltWeatherSystem()
        {
            trackerModel.WeatherSystemType = "";
            trackerModel.IsWeatherSystem = false;
        }

        public void SetNewWeatherSystem(string type, int days)
        {
            trackerModel.WeatherSystemType = type;
            trackerModel.IsWeatherSystem = true;
            trackerModel.WeatherSystemDays = days;
        }

        public void SetSync(WeatherSync ws)
        {
            //set general weather first, then the specialized weathers
            switch (ws.weatherType)
            {
                case Game1.weather_sunny:
                    WeatherUtilities.SetWeatherSunny();
                    break;
                case Game1.weather_debris:
                    WeatherUtilities.SetWeatherDebris();
                    break;
                case Game1.weather_snow:
                    WeatherUtilities.SetWeatherSnow();
                    break;
                case Game1.weather_rain:
                    WeatherUtilities.SetWeatherRain();
                    break;
                case Game1.weather_lightning:
                    WeatherUtilities.SetWeatherStorm();
                    break;
                default:
                    WeatherUtilities.SetWeatherSunny();
                    break;
            }

            Game1.updateWeatherIcon();

            if (ws.isFoggy && ws.fogWeatherEndTime <= Game1.timeOfDay && this.IsFoggy())
                ForceWeatherEnd("Fog");

            if (ws.isBlizzard && ws.blizzWeatherEndTime <= Game1.timeOfDay && this.IsBlizzard())
                ForceWeatherEnd("Blizzard");

            if (ws.isWhiteOut && ws.whiteWeatherEndTime <= Game1.timeOfDay && this.IsWhiteOut())
                ForceWeatherEnd("WhiteOut");

            if (ws.isThunderFrenzy && ws.thunWeatherEndTime <= Game1.timeOfDay && this.IsThunderFrenzy())
                ForceWeatherEnd("ThunderFrenzy");

            if (ws.isAbnormalHeat)
            {
                this.IsAbnormalHeat = true;
                WeatherProcessing.SetHotwave();
            }

            if (ws.isAbnormalChill)
            {
                this.IsAbnormalChill = true;
                WeatherProcessing.SetChillWave();
            }

            //yay, force set weathers!
            if (ws.isFoggy && (ws.fogWeatherBeginTime <= Game1.timeOfDay && ws.fogWeatherEndTime > Game1.timeOfDay))
            {
                ForceWeatherStart("Fog");
                SetWeatherBeginTime("Fog", ws.fogWeatherBeginTime);
                SetWeatherEndTime("Fog", ws.fogWeatherEndTime);
            }

            if (ws.isBlizzard && (ws.blizzWeatherBeginTime <= Game1.timeOfDay && ws.blizzWeatherEndTime > Game1.timeOfDay))
            {
                ForceWeatherStart("Blizzard");
                SetWeatherBeginTime("Blizzard", ws.blizzWeatherBeginTime);
                SetWeatherEndTime("Blizzard", ws.blizzWeatherEndTime);
            }

            if (ws.isWhiteOut && (ws.whiteWeatherBeginTime <= Game1.timeOfDay && ws.whiteWeatherEndTime > Game1.timeOfDay))
            {
                ForceWeatherStart("WhiteOut");
                SetWeatherBeginTime("WhiteOut", ws.whiteWeatherBeginTime);
                SetWeatherEndTime("WhiteOut", ws.whiteWeatherEndTime);
            }

            if (ws.isThunderFrenzy && (ws.thunWeatherBeginTime <= Game1.timeOfDay && ws.thunWeatherEndTime > Game1.timeOfDay))
            {
                SetWeatherBeginTime("ThunderFrenzy", ws.thunWeatherBeginTime);
                SetWeatherEndTime("ThunderFrenzy", ws.thunWeatherEndTime);
            }

            if (ws.isSandstorm && (ws.sandstormWeatherBeginTime <= Game1.timeOfDay && ws.sandstormWeatherEndTime > Game1.timeOfDay))
            {
                ForceWeatherStart("Sandstorm");
                SetWeatherBeginTime("Sandstorm", ws.sandstormWeatherBeginTime);
                SetWeatherEndTime("Sandstorm", ws.sandstormWeatherEndTime);
            }

            if (ws.isOvercast)
            {
                SetRainAmt(0);
                CurrentConditionsN |= CurrentWeather.Overcast;
                WeatherUtilities.SetWeatherOvercast();
            }
            
            if (ws.isVariableRain)
            {
                AmtOfRainDrops = ws.rainAmt;
                IsVariableRain = ws.isVariableRain;
            }

            if (TodayTemps is null)
            {
                TodayTemps = new RangePair();
            }
            TodayTemps.HigherBound = ws.todayHigh;
            TodayTemps.LowerBound = ws.todayLow;
            SetTomorrowTemps(new RangePair(ws.tommorowLow,ws.tommorowHigh, true));
            StartingRain = ws.StartingRain;

            //update tracker object
            trackerModel = new ClimateTracker(ws.currTracker);
            ClimatesOfFerngill.Conditions.SetTodayWeather();
        }

        public string PrintWeather()
        {
            string s = "";
            foreach(ISDVWeather w in this.CurrentWeathers)
            {
                s += w.DebugWeatherOutput();
                s += Environment.NewLine;
            }

            return s;
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
            this.GenerateWeatherSync();
        }

        /// <summary>This function resets the weather for a new day.</summary>
        public void OnNewDay()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.OnNewDay();

            TenMCounter = 0;
            AmtOfRainDrops = 0;
            IsOvercast = false;
            IsVariableRain = false;
            StartingRain = RainLevels.None;

            CurrentConditionsN = CurrentWeather.Unset;
            //Formerly, if tomorrow was null, we'd just allow nulls. Now we don't. 
            if (TomorrowTemps is null) { 
                ClimatesOfFerngill.GetTodayTemps();
                ClimatesOfFerngill.GetTomorrowTemps();
            }
            else { 
                TodayTemps = TomorrowTemps; 
                ClimatesOfFerngill.GetTomorrowTemps();
            }

            if (TodayTemps is null)
                ClimatesOfFerngill.GetTodayTemps();

            if (Game1.currentSeason == "fall")
            {
                Weathers[(int)CurrentWeather.Wind] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "debris", ClimatesOfFerngill.Translator.Get("weather_wind"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Frost)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "debrisfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "debrisheatwave", ClimatesOfFerngill.Translator.Get("weather_heatwaveCond", new { condition = ClimatesOfFerngill.Translator.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "drylightningwindy", ClimatesOfFerngill.Translator.Get("weather_drylightningwindy"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconDebris, WeatherIcon.IconDebris, "drylightningheatwave", ClimatesOfFerngill.Translator.Get("weather_drylightningheatwavewindy"));
            }
            else
            {
                //reset back to spring. 
                Weathers[(int)CurrentWeather.Wind] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debris", ClimatesOfFerngill.Translator.Get("weather_wind"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Frost)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisfrost", ClimatesOfFerngill.Translator.Get("weather_frost", new { condition = ClimatesOfFerngill.Translator.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "debrisheatwave", ClimatesOfFerngill.Translator.Get("weather_heatwaveCond", new { condition = ClimatesOfFerngill.Translator.Get("weather_wind") }));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "drylightningwindy", ClimatesOfFerngill.Translator.Get("weather_drylightningwindy"));
                Weathers[(int)(CurrentWeather.Wind | CurrentWeather.Heatwave | CurrentWeather.Lightning)] = new WeatherData(WeatherIcon.IconSpringDebris, WeatherIcon.IconSpringDebris, "drylightningheatwave", ClimatesOfFerngill.Translator.Get("weather_drylightningheatwavewindy"));
            }

            SetTodayWeather(); //run this automatically
            UpdateClimateTracker();
            GenerateWeatherSync();
        }

        private string NormalizedWeatherName()
        {
            //basically, we need to remove certain flags. 
            var rawConditions = GetCurrentConditions();
            //clear certain flags.
            rawConditions.RemoveFlags(CurrentWeather.BloodMoon);
            rawConditions.RemoveFlags(CurrentWeather.Fog);
            rawConditions.RemoveFlags(CurrentWeather.Heatwave);
            rawConditions.RemoveFlags(CurrentWeather.Frost);
            rawConditions.RemoveFlags(CurrentWeather.Sandstorm);

            //safety check
            if (rawConditions == CurrentWeather.Unset)
                ClimatesOfFerngill.Logger.Log("Warning, we've got no weather left here!!!!", LogLevel.Info);

            return Weathers[(int)rawConditions].ConditionName;
        }

        /// <summary> This function resets the weather object to basic. </summary>
        public void Reset()
        {
            foreach (ISDVWeather weather in CurrentWeathers)
                weather.Reset();

            TenMCounter = 0;
            trackerModel = null;
            IsOvercast = false;
            AmtOfRainDrops = 0;
            IsVariableRain = false;
            StartingRain = RainLevels.None;
            TodayTemps = null;
            TomorrowTemps = null;
            CurrentConditionsN = CurrentWeather.Unset;
        }

        /// <summary>
        /// This function gets when the fog ends. If it's 600, it's not visible.
        /// </summary>
        /// <returns>The time the fog ends</returns>
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
        public void SetTodayTemps(RangePair a)
        {
            TodayTemps = new RangePair(a, EnforceHigherOverLower: true);
            SeasonalBound(TodayTemps);
        }

        private static void SeasonalBound(RangePair a)
        {

            //seasonal check
            if (Game1.currentSeason == "spring")
            {
                if (a.LowerBound < -10)
                    a.LowerBound = -10;
                if (a.HigherBound > 30)
                    a.HigherBound = 30;
            }

            if (Game1.currentSeason == "summer")
            {
                if (a.LowerBound < 10)
                    a.LowerBound = 10;
                if (a.HigherBound > 60)
                    a.HigherBound = 60;
            }

            if (Game1.currentSeason == "fall")
            {
                if (a.LowerBound < -15)
                    a.LowerBound = 15;
                if (a.HigherBound > 32)
                    a.HigherBound = 32;
            }

            
            if (Game1.currentSeason == "winter")
            {
                if (a.LowerBound < -20)
                    a.LowerBound = -20;
                if (a.HigherBound > 18)
                    a.HigherBound = 18;
            }
        }

        /// <summary> This sets the temperatures from outside for tomorrow</summary>
        /// <param name="a">The RangePair that contains the generated temperatures</param>
        public void SetTomorrowTemps(RangePair a){
			TomorrowTemps = new RangePair(a, EnforceHigherOverLower: true);
			if (trackerModel?.TempsOnNextDay is null)
				trackerModel.TempsOnNextDay = new RangePair(TomorrowTemps);
			else
				trackerModel.TempsOnNextDay.UpdateRangePair(TomorrowTemps.LowerBound, TomorrowTemps.HigherBound);

            SeasonalBound(TomorrowTemps);
		}

        /// ***************************************************************************
        /// Utility functions
        /// ***************************************************************************
        
        ///<summary> This function converts from the game weather back to the CurrentWeather enum. Intended primarily for use with tomorrow's forecasted weather.</summary>
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

        internal void RefreshRainAmt()
        {
            Array.Resize(ref Game1.rainDrops, AmtOfRainDrops);

            if (ClimatesOfFerngill.WeatherOpt.Verbose)
            {
                ClimatesOfFerngill.Logger.Log(Game1.IsMasterGame
                    ? $"Setting rain to {AmtOfRainDrops}"
                    : $"Setting from master: rain to {AmtOfRainDrops}");
            }
        }

        internal void SetRainAmt(int rainAmt)
        {
            if (AmtOfRainDrops != rainAmt)
                AmtOfRainDrops = rainAmt;

            Array.Resize(ref Game1.rainDrops, AmtOfRainDrops);

            if (Game1.IsMasterGame)
                ClimatesOfFerngill.Logger.Log($"Setting rain to {AmtOfRainDrops}");
            else
            {
                ClimatesOfFerngill.Logger.Log($"Setting from master: rain to {AmtOfRainDrops}");
            }
        }
        
        internal void Refresh()
        {
            SetTodayWeather();
        }

        internal void SetTodayWeather()
        {
           CurrentConditionsN = CurrentWeather.Unset; //reset the flag.
            
            if (!Game1.isDebrisWeather && !Game1.isRaining && !Game1.isSnowing)
            {
                AddWeather(CurrentWeather.Sunny);
            }

            if (IsOvercast)
            {
                SDVUtilities.AlterWaterStatusOfCrops(false);
		        RemoveWeather(CurrentWeather.Sunny);
                AddWeather(CurrentWeather.Overcast);
            }

            if (Game1.isRaining) { 
		        RemoveWeather(CurrentWeather.Overcast);
                AddWeather(CurrentWeather.Rain);
                if (IsVariableRain)
                {
                    SetRainAmt(AmtOfRainDrops);
                }
            }
		
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
            foreach (ISDVWeather weather in CurrentWeathers)
            {
                if (weather.WeatherType == "Fog" && weather.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.Fog;
                if (weather.WeatherType == "Blizzard" && weather.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.Blizzard;
                if (weather.WeatherType == "WhiteOut" && weather.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.WhiteOut;
                if (weather.WeatherType == "Sandstorm" && weather.IsWeatherVisible)
                    CurrentConditionsN |= CurrentWeather.Sandstorm;
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

	        ret += Environment.NewLine;
            foreach (ISDVWeather weather in CurrentWeathers)
                ret += weather.ToString() + Environment.NewLine;
            
            ret += $"Weather set for tomorrow is {Weathers[(int)(WeatherConditions.ConvertToCurrentWeather(Game1.weatherForTomorrow))].ConditionName} with high {TomorrowTemps?.HigherBound:N3} and low {TomorrowTemps?.LowerBound:N3}. Evening fog generated {GenerateEveningFog} ";

			ret += Environment.NewLine + trackerModel;
            ret += Environment.NewLine + $"Variable Rain Status: {IsVariableRain} with rain drops currently: {AmtOfRainDrops}";
            return ret;
        }

        internal void OnSaving()
        {
	    if (trackerModel is null)
		    trackerModel = new ClimateTracker();
		
        //check for rain accumulation         
        int numTotals = (int)Math.Floor(SDVTime.MinutesBetweenTwoIntTimes(2600, Game1.timeOfDay) / 10.0);
        if (IsVariableRain)
        {
            for (int i = 0; i < numTotals; i++)
            {
                AmtOfRainDrops = WeatherProcessing.GetNewRainAmount(AmtOfRainDrops, ClimatesOfFerngill.Translator, false);
                TodayRain += AmtOfRainDrops;
            }
        }
        else if (this.ContainsCondition(CurrentWeather.Rain))
        {
            TodayRain += AmtOfRainDrops * numTotals;
        }
        trackerModel.AmtOfRainSinceDay1 += TodayRain;
        trackerModel.AmtOfRainInCurrentStreak += TodayRain;
        TodayRain = 0;
        }

        internal void SetVariableRain(bool val)
        {
            IsVariableRain = val;
        }

        internal static bool IsValidWeatherForSnow(RangePair temps)
        {
            if (temps.HigherBound <= 2 && temps.GetMidPoint() <= 0)
                return true;

            return false;
        }
		
		internal bool WeatherIsHazardous()
		{
			if (HasWeather(CurrentWeather.Heatwave) || HasWeather(CurrentWeather.Frost) || HasWeather(CurrentWeather.Blizzard)
				|| HasWeather(CurrentWeather.WhiteOut) || HasWeather(CurrentWeather.ThunderFrenzy) || HasWeather(CurrentWeather.Sandstorm))
				return true;
				
			if (WeatherUtilities.IsSevereRainFall(AmtOfRainDrops))
				return true;
			
			return false;			
		}

        internal void UpdateDynamicRain()
        {
            AmtOfRainDrops = WeatherProcessing.GetNewRainAmount(AmtOfRainDrops, ClimatesOfFerngill.Translator);
            SetRainAmt(AmtOfRainDrops);
        }

        internal static bool PreventGoingOutside(int amtOfRainDrops)
        {
            if ((WeatherUtilities.GetRainCategory(amtOfRainDrops) == RainLevels.NoahsFlood) || (WeatherUtilities.GetRainCategory(amtOfRainDrops) == RainLevels.Typhoon))
                    return true;

            return false;
        }

    }
}
