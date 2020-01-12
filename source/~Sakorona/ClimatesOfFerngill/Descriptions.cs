using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    class Descriptions
    {
        private readonly WeatherConfig Opt;
        private readonly ITranslationHelper Translator;

        public Descriptions(WeatherConfig O, ITranslationHelper T)
        {
            Opt = O;
            Translator = T;
        }
               
        public string DescRainfallAmt(int rainFall)
        {
            return WeatherUtilities.GetRainfallAmt(rainFall).ToString("N2");
        }
		
        internal string GetDescOfDay(SDate date)
        {
            return Translator.Get("date" + GeneralFunctions.FirstLetterToUpper(date.Season) + date.Day);
        }

        internal string DescribeInGameWeather(int weather)
        {
            if (weather == Game1.weather_debris)
                return Translator.Get("weather_wind");
            if (weather == Game1.weather_festival)
                return Translator.Get("weather_festival");
            if (weather == Game1.weather_lightning)
                return Translator.Get("weather_lightning");
            if (weather == Game1.weather_rain)
                return Translator.Get("weather_rainy");
            if (weather == Game1.weather_snow)
                return Translator.Get("weather_snow");
            if (weather == Game1.weather_sunny)
                return Translator.Get("weather_sunny");
            if (weather == Game1.weather_wedding)
                return Translator.Get("weather_wedding");

            return "ERROR";
        }

        private string GetTemperatureString(double temp)
        {
			//rewrote to simplify.
			if (Opt.ShowBothScales)
			{
				return Translator.Get("temp-bothScales", new { tempOne=ConvertTempFromCtoTarget(temp,Opt.FirstTempScale).ToString("N1"), tempOneN = GetTempNotation(Opt.FirstTempScale), tempTwo=ConvertTempFromCtoTarget(temp,Opt.SecondTempScale).ToString("N1"),tempTwoN = GetTempNotation(Opt.SecondTempScale)});
			}
			else
			{
				return Translator.Get("temp-oneScale", new { tempOne=ConvertTempFromCtoTarget(temp,Opt.FirstTempScale).ToString("N1"), tempOneN = GetTempNotation(Opt.FirstTempScale)});
			}			
        }
		
		private string GetTempNotation(string target)
		{
			switch (target)
			{
				case "Farenheit":
					return Translator.Get("temp-FareNotation");
				case "Kelvin":
					return Translator.Get("temp-KelvinNotation");
				case "Rankine":
					return Translator.Get("temp-RankineNotation");
				case "Delisle":
					return Translator.Get("temp-DelisleNotation");
				case "Romer":
					return Translator.Get("temp-RomerNotation");
				case "Reaumur":
					return Translator.Get("temp-ReaumurNotation");
				case "Celsius":
					return Translator.Get("temp-CelsNotation");
				case "Kraggs":
					return Translator.Get("temp-KraggsNotation");
				default:
					return Translator.Get("temp-errorNotation");
			}
		}
		
		private double ConvertTempFromCtoTarget(double temp, string target)
		{
			//supported values need to be listed.
			switch (target)
			{
				case "Farenheit":
					return GeneralFunctions.ConvCtF(temp);
				case "Kelvin":
					return GeneralFunctions.ConvCtK(temp);
				case "Rankine":
					return GeneralFunctions.ConvCtRa(temp);
				case "Delisle":
					return GeneralFunctions.ConvCtD(temp);
				case "Romer":
					return GeneralFunctions.ConvCtRo(temp);
				case "Reaumur":
					return GeneralFunctions.ConvCtRe(temp);
				case "Celsius":
				case "Kraggs":
				default:
					return temp;
			}
		}

        internal string UpperSeason(string season)
        {
            if (season == "spring") return "Spring";
            if (season == "winter") return "Winter";
            if (season == "fall") return "Fall";
            if (season == "summer") return "Summer";

            return "error";
        }

        private string GetRainDesc(int rainAmt)
        {
            switch (WeatherUtilities.GetRainCategory(rainAmt))
            {
                case RainLevels.Sunshower:
                    return Translator.Get("weather-tv.rain.sunshowers");
                case RainLevels.Light:
                    return Translator.Get("weather-tv.rain.light");
                case RainLevels.Normal:
                    return Translator.Get("weather-tv.rain.normal");
                case RainLevels.Moderate:
                    return Translator.Get("weather-tv.rain.moderate");
                case RainLevels.Heavy:
                    return Translator.Get("weather-tv.rain.heavy");
                case RainLevels.Severe:
                    return Translator.Get("weather-tv.rain.severe");
                case RainLevels.Torrential:
                    return Translator.Get("weather-tv.rain.torrential");
                case RainLevels.Typhoon:
                    return Translator.Get("weather-tv.rain.typhoon");
                case RainLevels.NoahsFlood:
                    return Translator.Get("weather-tv.rain.noahsflood");
                default:
                    return Translator.Get("weather-tv.rain.unknown");

            }
        }

        internal string GenerateMenuPopup(WeatherConditions Current, string MoonPhase = "", string NightTime = "")
        {
            string text;
            if (SDate.Now().Season == "spring" && SDate.Now().Day == 1)
                text = Translator.Get("weather-menu.openingS1D1", new { descDay = Translator.Get($"date{UpperSeason(SDate.Now().Season)}{SDate.Now().Day}") }) + Environment.NewLine + Environment.NewLine;
            else if (SDate.Now().Season == "winter" && SDate.Now().Day == 28)
                text = Translator.Get("weather-menu.openingS4D28", new { descDay = Translator.Get($"date{UpperSeason(SDate.Now().Season)}{SDate.Now().Day}") }) + Environment.NewLine + Environment.NewLine;
            else
                text = Translator.Get("weather-menu.opening", new { descDay = Translator.Get($"date{UpperSeason(SDate.Now().Season)}{SDate.Now().Day}") }) + Environment.NewLine + Environment.NewLine;

            if (Current.ContainsCondition(CurrentWeather.Sandstorm))
                text += Translator.Get("weather-menu.condition.sandstorm") + Environment.NewLine;

            if (Current.ContainsCondition(CurrentWeather.Heatwave))
            {
                text += Translator.Get("weather-menu.condition.heatwave") + Environment.NewLine;
            }

            if (Current.ContainsCondition(CurrentWeather.Frost))
            {
                text += Translator.Get("weather-menu.condition.frost") + Environment.NewLine;
            }

            if (Current.ContainsCondition(CurrentWeather.WhiteOut))
            {
                text += Translator.Get("weather-menu.condition.whiteOut") + Environment.NewLine;
            }

            if (Current.ContainsCondition(CurrentWeather.ThunderFrenzy))
            {
                text += Translator.Get("weather-menu.condition.thunderFrenzy") + Environment.NewLine;
            }
			
            if (Current.IsVariableRain)
            {
                switch (WeatherUtilities.GetRainCategory(Current.AmtOfRainDrops))
                {
                    case RainLevels.Severe:
                        text += Translator.Get("weather-condition.vrain.severe_sw") + Environment.NewLine;
                        break;
                    case RainLevels.Torrential:
                        text += Translator.Get("weather-condition.vrain.torrential_sw") + Environment.NewLine;
                        break;
                    case RainLevels.Typhoon:
                        text += Translator.Get("weather-condition.vrain.typhoon_sw") + Environment.NewLine;
                        break;
                    case RainLevels.NoahsFlood:
                        text += Translator.Get("weather-condition.vrain.godswrath_sw") + Environment.NewLine;
                        break;
                    default:
                        break;
                }
            }

            if (MoonPhase == "Blood Moon")
            {
                text += Translator.Get("weather-menu.condition.bloodmoon") + Environment.NewLine;
            }

            if (ClimatesOfFerngill.UseLunarDisturbancesApi && ClimatesOfFerngill.MoonAPI.IsSolarEclipse())
            {
                text += Translator.Get("weather-menu.condition.solareclipse") + Environment.NewLine;
            }

            ISDVWeather CurrentFog = Current.GetWeatherMatchingType("Fog").First();
            string fogString = "";

            //  If the fog is visible, we don't need to display fog information. However, if it's in the morning, 
            //    and we know evening fog is likely, we should display the message it's expected
            // That said, if it's not, we need to pull the fog information down, assuming it's been reset. This checks that the fog end
            //    time is *before* now. To avoid nested trinary statements..
            if (SDVTime.CurrentTime < CurrentFog.WeatherExpirationTime && Current.GenerateEveningFog && CurrentFog.WeatherBeginTime < new SDVTime(1200))
                fogString = Translator.Get("weather-menu.expectedFog");
            if (CurrentFog.WeatherBeginTime > SDVTime.CurrentTime && Current.GenerateEveningFog)
                fogString = Translator.Get("weather-menu.fogFuture",
                    new
                    {
                        fogTime = CurrentFog.WeatherBeginTime.ToString(),
                        endFog = CurrentFog.WeatherExpirationTime.ToString()
                    });

            //Current Conditions.
			string currentMenu;
			if (Current.HasWeather(CurrentWeather.Rain) && Current.IsVariableRain)
				currentMenu = "weather-menu.currentRainfall";
			else
				currentMenu = "weather-menu.current";
			
            text += Translator.Get(currentMenu, new
            {
                todayCondition = Current.HasWeather(CurrentWeather.Fog) 
					? Translator.Get("weather-menu.fog", new { condition = GetBasicWeather(Current), fogTime = CurrentFog.IsWeatherVisible 
						? CurrentFog.WeatherExpirationTime.ToString() 
						: "" }) 
					: GetBasicWeather(Current),
				tempString = GetTemperatureString(Current.GetCurrentTemperature(Game1.timeOfDay)),
                todayHigh = GetTemperatureString(Current.TodayHigh),
                todayLow = GetTemperatureString(Current.TodayLow),
				currentRainfall = WeatherUtilities.GetRainfallAmt(Current.AmtOfRainDrops).ToString("N2"),
                fogString               
            }) + Environment.NewLine;

            //Tomorrow weather
            text += Translator.Get("weather-menu.tomorrow", 
                        new {
                            tomorrowCondition = GetBasicWeather(Game1.weatherForTomorrow),
                            tomorrowLow = GetTemperatureString(Current.TomorrowLow),
                            tomorrowHigh = GetTemperatureString(Current.TomorrowHigh)
                        }) + Environment.NewLine;

            //now, night time
            if (!String.IsNullOrEmpty(NightTime))
            {                
                text += NightTime + Environment.NewLine;
            }

            if (ClimatesOfFerngill.UseLunarDisturbancesApi)
            {
                if (ClimatesOfFerngill.MoonAPI.IsMoonUp(Game1.timeOfDay))
					text += Translator.Get("weather-menu.desc-moonNotUp", new { moonPhase = ClimatesOfFerngill.MoonAPI.GetCurrentMoonPhase(), moonRise = ClimatesOfFerngill.MoonAPI.GetMoonRise(), moonSet = ClimatesOfFerngill.MoonAPI.GetMoonSet()});
                else
                    text += Translator.Get("weather-menu.desc-moonUp", new { moonPhase = ClimatesOfFerngill.MoonAPI.GetCurrentMoonPhase(), moonSet = ClimatesOfFerngill.MoonAPI.GetMoonSet() });
            }

            return text;
        }

        internal string GenerateTVForecast(WeatherConditions Current, MersenneTwister Dice, double fogOdds, string MoonPhase = "", bool IsMoonUp = false)
        {
            string ret = "";

            //opening string
            ret += Translator.Get("weather-tv.opening", new
            {
                location = Translator.Get("fern-loc." + Dice.Next(12)),
                playerLocation = Translator.Get("weather-location.player." + Dice.Next(4))
            });

            //hazard warning
            if (MoonPhase == "Blood Moon" && IsMoonUp)
            {
                ret += Translator.Get("weather-tv.bloodmoon");
            }

            if (Current.WeatherIsHazardous())
            {
                string hazard = "", rainfallW = "", hazardT = "", hazardF;

                if (Current.HasWeather(CurrentWeather.Heatwave)) {
                    hazardT = Translator.Get("weather-tv.hazard.hw");
                }
                if (Current.HasWeather(CurrentWeather.Frost)) {
                    hazardT = Translator.Get("weather-tv.hazard.frost");
                }

                if (Current.HasWeather(CurrentWeather.Blizzard))
                    hazard = Translator.Get("weather-tv.hazard.blizzard");
                if (Current.HasWeather(CurrentWeather.WhiteOut))
                    hazard = Translator.Get("weather-tv.hazard.whiteout");
                if (Current.HasWeather(CurrentWeather.ThunderFrenzy))
                    hazard = Translator.Get("weather-tv.hazard.thunderfrenzy");
                if (Current.HasWeather(CurrentWeather.Sandstorm))
                    hazard = Translator.Get("weather-tv.hazard.sandstorm");
                if (WeatherUtilities.IsSevereRainFall(Current.AmtOfRainDrops)) {
                    switch (WeatherUtilities.GetRainCategory(Current.AmtOfRainDrops))
                    {
                        case RainLevels.Severe:
                            hazard = Translator.Get("weather-tv.hazard.rainfall.severe");
                            break;
                        case RainLevels.Torrential:
                            hazard = Translator.Get("weather-tv.hazard.rainfall.torrential");
                            break;
                        case RainLevels.Typhoon:
                            hazard = Translator.Get("weather-tv.hazard.rainfall.typhoon");
                            break;
                        case RainLevels.NoahsFlood:
                            hazard = Translator.Get("weather-tv.hazard.rainfall.noahsflood");
                            break;
                        default:
                            hazard = Translator.Get("weather-tv.hazard.rainfall.severe");
                            break;
                    }

                    rainfallW = Translator.Get("weather-tv.hazard.rainfallW");
                }

                if (!String.IsNullOrEmpty(hazard) && !String.IsNullOrEmpty(hazardT))
                    hazardF = Translator.Get("weather-tv.twoHazardString", new { hazardOne = hazard, hazardTwo = hazardT });
                else if (!String.IsNullOrEmpty(hazard))
                    hazardF = hazard;
                else
                    hazardF = hazardT;

                ret += Translator.Get("weather-tv.hazard", new {
                    hazard = hazardF,
                    repLocation = Translator.Get("weather-location.reporter."+Dice.Next(4)),
                    rainfallWarning = rainfallW
                });
            }

            //current conditions
            string time;
            switch (SDVTime.CurrentTimePeriod)
            {
                case SDVTimePeriods.Morning:
                    time = Translator.Get("weather-tv.time.morning");
                    break;
                case SDVTimePeriods.Noon:
                case SDVTimePeriods.Afternoon:
                    time = Translator.Get("weather-tv.time.afternoon");
                    break;
                case SDVTimePeriods.Evening:
                    time = Translator.Get("weather-tv.time.evening");
                    break;
                case SDVTimePeriods.Night:
                    time = Translator.Get("weather-tv.time.night");
                    break;
                case SDVTimePeriods.Midnight:
                case SDVTimePeriods.LateNight:
                    time = Translator.Get("weather-tv.time.latenight");
                    break;
                default:
                    time = Translator.Get("weather-tv.time.generic");
                    break;
            }

            string abText = "";
            if (Current.IsAbnormalHeat)
                abText = Translator.Get("weather-tv.abnormalHeat");

            if (Current.IsAbnormalChill)
                abText = Translator.Get("weather-tv.abnormalChill");

            string sysText = "";
            if (Current.trackerModel.WeatherSystemDays > 0)
            {
                if (Current.trackerModel.IsWeatherSystem)
                    sysText = Translator.Get("weather-tv.systemMaint");
            }

            //the first monster, describing the weather.
            string dWeather = "";
            if (!String.IsNullOrEmpty(SDVUtilities.GetFestivalName(SDate.Now())))
                dWeather = Translator.Get("weather-tv.weat.festival", new { festivalName = SDVUtilities.GetFestivalName(SDate.Now()) });

            if (Current.HasWeather(CurrentWeather.Wedding))
                dWeather = Translator.Get("weather-tv.weat.wedding");

            if (Current.HasWeather(CurrentWeather.Sunny))
                dWeather = Translator.Get("weather-tv.weat.sunny");

            if (Current.HasWeather(CurrentWeather.Rain) && !Current.HasWeather(CurrentWeather.Lightning))
            {
                dWeather = (Current.IsVariableRain ? Translator.Get("weather-tv.weat.rain.variable", new { rainDesc = GetRainDesc(Current.AmtOfRainDrops).Trim() }) 
                                                   : Translator.Get("weather-tv.weat.rain", new { rainDesc = GetRainDesc(Current.AmtOfRainDrops).Trim() }));
            }

            if (Current.HasWeather(CurrentWeather.Rain) && Current.HasWeather(CurrentWeather.Lightning) && !Current.HasWeather(CurrentWeather.ThunderFrenzy))
                dWeather = Translator.Get("weather-tv.weat.thunderstorm");

            if (Current.HasWeather(CurrentWeather.ThunderFrenzy))
                dWeather = Translator.Get("weather-tv.weat.thunderfrenzy");

            if (!Current.HasWeather(CurrentWeather.Rain) && Current.HasWeather(CurrentWeather.Lightning))
                dWeather = Translator.Get("weather-tv.weat.drylightning");

            if (Current.HasWeather(CurrentWeather.Blizzard) && !Current.HasWeather(CurrentWeather.WhiteOut))
                dWeather = Translator.Get("weather-tv.weat.blizzard");

            if (Current.HasWeather(CurrentWeather.WhiteOut))
                dWeather = Translator.Get("weather-tv.weat.whiteout");

            if (Current.HasWeather(CurrentWeather.Snow) && !Current.HasWeather(CurrentWeather.Blizzard) && !Current.HasWeather(CurrentWeather.WhiteOut) && !Current.HasWeather(CurrentWeather.Lightning))
                dWeather = Translator.Get("weather-tv.weat.snowy");

            if (Current.HasWeather(CurrentWeather.Snow) && !Current.HasWeather(CurrentWeather.Blizzard) && !Current.HasWeather(CurrentWeather.WhiteOut) && Current.HasWeather(CurrentWeather.Lightning))
                dWeather = Translator.Get("weather-tv.weat.thundersnow");

            if (Current.HasWeather(CurrentWeather.Sandstorm))
                dWeather = Translator.Get("weather-tv.weat.sandstorms");

            if (Current.HasWeather(CurrentWeather.Wind))
            {
                if (Game1.currentSeason == "winter")
                    dWeather = Translator.Get("weather-tv.weat.windy.winter");
                else
                    dWeather = Translator.Get("weather-tv.weat.windy");
            }

            string fog = "";
            if (Current.HasWeather(CurrentWeather.Fog))
            {
                if (!Current.HasWeather(CurrentWeather.Wedding))
                    fog = Translator.Get("weather-tv.weat.fog", new { time = Current.GetFogTime() });
                else
                    fog = Translator.Get("weather-tv.weat.fogWed", new { time = Current.GetFogTime() });
            }

            //ending up the current conditions.
            string tHazard = "";
            if (Current.HasWeather(CurrentWeather.Heatwave)) 
            {
                tHazard = Translator.Get("weather-tv.tempHazard.heat");
            }
            
            if (Current.HasWeather(CurrentWeather.Frost))
            {
                tHazard = Translator.Get("weather-tv.tempHazard.cold",
                    new { time = (Game1.currentSeason == "spring" 
                          ? Translator.Get("weather-tv.tempHazard.cold.spring") : 
                          Translator.Get("weather-tv.tempHazard.cold.fall"))
                   });
            }

            string rRate = "";
            if (Current.HasWeather(CurrentWeather.Rain))
            {
                rRate = Translator.Get("weather-tv.rainfallRate", new { rate = WeatherUtilities.GetRainfallAmt(Current.AmtOfRainDrops).ToString("N2") });
            }

            var transParams = new Dictionary<string, string>{
                { "time",time },
                { "currentTemp",GetTemperatureString(Current.GetCurrentTemperature(Game1.timeOfDay)) },
				{ "abnormalText", abText },
				{ "systemText", sysText },
                { "descWeather", dWeather },
                { "fogText", fog },
                { "tempHazard", tHazard },
                { "rainfallRate", rRate }
			};

            ret += Translator.Get("weather-tv.currentCond", transParams);
            transParams.Clear();

            //now, tomorrow!! :D

            sysText = "";
            if (Current.trackerModel.WeatherSystemDays > 0)
            {
                if (Current.trackerModel.IsWeatherSystem)
                    sysText = Translator.Get("weather-tv.tmrw.systemMaint", 
                        new {desc = Translator.Get("weather-location.system.desc."+Dice.Next(6)) });
                if (!Current.trackerModel.IsWeatherSystem && (Game1.weatherForTomorrow != WeatherUtilities.GetWeatherCode()))
                    sysText = Translator.Get("weather-tv.tmrw.systemEnding",
                        new { desc = Translator.Get("weather-location.system.desc." + Dice.Next(6)), direction = Translator.Get("weather-location.system.move." + Dice.Next(4))});
            }
            else
            {
                sysText = Translator.Get("weather-tv.tmrw.systemNone");
            }

            //now weather
            switch (Game1.weatherForTomorrow)
            {
                case Game1.weather_rain:
                    dWeather = Translator.Get("weather-tv.rain." + Dice.Next(2));
                    break;
                case Game1.weather_festival:
                    dWeather = Translator.Get("weather-tv.festival", new { festName = SDVUtilities.GetFestivalName(SDate.Now().AddDays(1)) });
                    break;
                case Game1.weather_debris:
                    dWeather = Translator.Get("weather-tv.debris." + Dice.Next(2));
                    break;
                case Game1.weather_snow:
                    dWeather = Translator.Get("weather-tv.snowy." + Dice.Next(2));
                    break;
                case Game1.weather_wedding:
                    dWeather = Translator.Get("weather-tv.wedding");
                    break;
                case Game1.weather_lightning:
                    dWeather = Translator.Get("weather-tv.tstorm." + Dice.Next(2));
                    break;
                case Game1.weather_sunny:
                default:
                    dWeather = Translator.Get("weather-tv.sunny." + Dice.Next(2));
                    break;
            }

            //overrides for festival and wedding (just in case, my paranoia is spiking. :|)
            if (!String.IsNullOrEmpty(SDVUtilities.GetFestivalName(SDate.Now().AddDays(1))))
            {
                dWeather = Translator.Get("weather-tv.festival", new { festName = SDVUtilities.GetFestivalName(SDate.Now().AddDays(1)) });
            }
            if (Game1.player.spouse != null && Game1.player.isEngaged() && Game1.player.friendshipData[Game1.player.spouse].CountdownToWedding == 1)
            {
                dWeather = Translator.Get("weather-tv.wedding");
            }

            fog = "";
            //now, the fog string
            if (fogOdds >= .6)
            {
                fog = Translator.Get("weather-tv.fogChance");
            }

            //tHazard string

            if (Current.TomorrowHigh > (Current.TodayHigh + 1.5))
            {
                tHazard = Translator.Get("weather-tv.tmrw.warmer", new
                {
                    high = GetTemperatureString(Current.TomorrowHigh),
                    low = GetTemperatureString(Current.TomorrowLow),
                });
            }
            else if ((Current.TomorrowHigh < (Current.TodayHigh - 1.5)))
            {
                tHazard = Translator.Get("weather-tv.tmrw.cooler", new
                {
                    high = GetTemperatureString(Current.TomorrowHigh),
                    low = GetTemperatureString(Current.TomorrowLow),
                });
            }
            else
            {
                tHazard = Translator.Get("weather-tv.tmrw.effsame", new
                {
                    high = GetTemperatureString(Current.TomorrowHigh),
                    low = GetTemperatureString(Current.TomorrowLow),
                });
            }


            transParams.Add("sysText", sysText);
            transParams.Add("weather", dWeather);
            transParams.Add("fogChance", fog);
            transParams.Add("tempString", tHazard);

            ret += Translator.Get("weather-tv.tomorrowForecast", transParams);
			return ret;
        }


        private string GetBasicWeather(int weather)
        {
            if (weather == Game1.weather_debris)
                return Translator.Get($"weather_wind");
            else if (weather == Game1.weather_festival || weather == Game1.weather_wedding)
                return Translator.Get($"weather_sunny");
            else if (weather == Game1.weather_lightning)
                return Translator.Get($"weather_lightning");
            else if (weather == Game1.weather_rain)
                return Translator.Get($"weather_rainy");
            else if (weather == Game1.weather_snow)
                return Translator.Get($"weather_snow");
            else if (weather == Game1.weather_sunny)
                return Translator.Get($"weather_sunny");

            return "ERROR";
        }

        private string GetBasicWeather(WeatherConditions Weather)
        {
            if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconBlizzard || Weather.CurrentWeatherIconBasic == WeatherIcon.IconWhiteOut)
                return Translator.Get($"weather_blizzard");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconSandstorm)
                return Translator.Get($"weather_sandstorm");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconSpringDebris || Weather.CurrentWeatherIconBasic == WeatherIcon.IconDebris)
                return Translator.Get($"weather_wind");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconDryLightning)
                return Translator.Get($"weather_drythunder");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconSunny && SDVTime.CurrentIntTime < Game1.getModeratelyDarkTime())
                return Translator.Get($"weather_sunny_daytime");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconSunny && SDVTime.CurrentIntTime >= Game1.getModeratelyDarkTime())
                return Translator.Get($"weather_sunny_nighttime");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconStorm)
                return Translator.Get($"weather_lightning");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconSnow)
                return Translator.Get($"weather_snow");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconRain)
                return Translator.Get($"weather_rainy");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconThunderSnow)
                return Translator.Get($"weather_thundersnow");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconWedding)
                return Translator.Get($"weather_wedding");
            else if (Weather.CurrentWeatherIconBasic == WeatherIcon.IconFestival)
                return Translator.Get($"weather_festival");
            return "ERROR";
        }

        internal TemporaryAnimatedSprite GetWeatherOverlay(WeatherConditions Current, TV tv)
        {
            Rectangle placement = new Rectangle(413, 333, 13, 13);

            switch (Current.CurrentWeatherIconBasic)
            {
                case WeatherIcon.IconSunny:
                case WeatherIcon.IconWedding:
                case WeatherIcon.IconDryLightning:
                    placement = new Rectangle(413, 333, 13, 13);
                    break;
                case WeatherIcon.IconRain:
                    placement = new Rectangle(465, 333, 13, 13);
                    break;
                case WeatherIcon.IconDebris:
                    placement = (Game1.currentSeason.Equals("fall") ? new Rectangle(413, 359, 13, 13) : new Rectangle(465, 346, 13, 13));
                    break;
                case WeatherIcon.IconSpringDebris:
                    placement = new Rectangle(465, 359, 13, 13);
                    break;
                case WeatherIcon.IconStorm:
                    placement = new Rectangle(413, 346, 13, 13);
                    break;
                case WeatherIcon.IconFestival:
                    placement = new Rectangle(413, 372, 13, 13);
                    break;
                case WeatherIcon.IconSnow:
                case WeatherIcon.IconBlizzard:
                case WeatherIcon.IconWhiteOut:
                    placement = new Rectangle(465, 346, 13, 13);
                    break;
            }

            return new TemporaryAnimatedSprite("LooseSprites\\Cursors", placement, 100f, 4, 999999, tv.getScreenPosition() + new Vector2(3f, 3f) * tv.getScreenSizeModifier(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
        }
    }
}
