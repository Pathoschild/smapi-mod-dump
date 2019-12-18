using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>
    /// This class tracks the current the current weather of the game
    /// </summary>
    public static class WeatherProcessing
	{
        private static int ExpireTime;
        private static List<Vector2> CropList;
        internal static int GetNewRainAmount(int prevRain, ITranslationHelper Translation, bool showRain = true)
        {
            int currRain = prevRain;
            double ChanceOfRainChange = ClimatesOfFerngill.WeatherOpt.VRChangeChance;
            bool massiveChange = false;

            //do some rain chance pre pumping
            if (currRain == 0)
                ChanceOfRainChange += .20;
            if (currRain > WeatherUtilities.GetRainCategoryMidPoint(RainLevels.Torrential))
                ChanceOfRainChange += .10;
	        if (WeatherUtilities.GetRainCategory(currRain) == RainLevels.NoahsFlood)
		        ChanceOfRainChange += .15;

            double FlipChance = .5;

            //so, lower: decrease, higher: increase
            if (WeatherUtilities.IsSevereRainFall(currRain))
                FlipChance += .2;
            if (WeatherUtilities.GetRainCategory(currRain) == RainLevels.Torrential || WeatherUtilities.GetRainCategory(currRain) == RainLevels.Typhoon || WeatherUtilities.GetRainCategory(currRain) == RainLevels.NoahsFlood)
                FlipChance += .15; //15% chance remaining of increasing. 
            if (WeatherUtilities.GetRainCategory(currRain) == RainLevels.Typhoon || WeatherUtilities.GetRainCategory(currRain) == RainLevels.NoahsFlood)
                FlipChance += .1456; //.44% chance remaning of increasing.
            if (WeatherUtilities.GetRainCategory(currRain) == RainLevels.NoahsFlood)
                FlipChance += .0018; //.26% chance remaning of increasing.
	        if (currRain == ClimatesOfFerngill.WeatherOpt.MaxRainFall)
		        FlipChance = 1; //you must go ddown.

            if (currRain <= WeatherUtilities.GetRainCategoryMidPoint(RainLevels.Light))
                FlipChance -= .2; //70% chance of increasing
            if (currRain <= WeatherUtilities.GetRainCategoryMidPoint(RainLevels.Sunshower))
                FlipChance -= .1; //80% chance of increasing
	        if (currRain == 0)
		        FlipChance -= .15; //95% chance of increasing
		
            if (ClimatesOfFerngill.WeatherOpt.Verbose)
            {
                ClimatesOfFerngill.Logger.Log($"Rain is {prevRain}, current chance for rain change is {ChanceOfRainChange}.");
                ClimatesOfFerngill.Logger.Log($"Calculated chance for rain decreasing: {WeatherUtilities.GetStepChance(currRain, increase: false).ToString("N3")}, rain increasing: {WeatherUtilities.GetStepChance(currRain, increase: true).ToString("N3")}. Flip chance is {FlipChance.ToString("N3")} ");
            }

            // I just spent nine minutes typing out an invalid explanation for this. :|
            // So, i'm renaming the variables. 

            // And redoing this entirely. It's a lot of duplicated effort.
            //Greater than flip chance is increase, lesser is decrease

            if (ClimatesOfFerngill.Dice.NextDouble() < ChanceOfRainChange)
            {
                // Handle rain changes.
                // first check for a massive change 145% to 245%
                double stepRoll = ClimatesOfFerngill.Dice.NextDouble();
                if (ClimatesOfFerngill.Dice.NextDouble() > FlipChance)
                {
                    if (ClimatesOfFerngill.WeatherOpt.Verbose)
                        ClimatesOfFerngill.Logger.Log("Increasing rain!");

                    if (stepRoll <= WeatherUtilities.GetStepChance(currRain, increase: true))
                    {
                        //get the multiplier.  This should be between 145% and 245%

                        int mult = Math.Max(1, (int)(Math.Floor(ClimatesOfFerngill.Dice.NextDouble() + .68)));

                        if (currRain == 0)
                            currRain = 15 * mult;
                        else
                            currRain = (int)(Math.Floor(currRain * mult * 1.0));

                        massiveChange = true;

                    }
                }
                else
                {
                    if (stepRoll <= WeatherUtilities.GetStepChance(currRain, increase: false))
                    {
                        ClimatesOfFerngill.Logger.Log("Increasing rain!");
                        currRain = (int)(Math.Floor(currRain / (ClimatesOfFerngill.Dice.NextDouble() + 1.45)));
                        massiveChange = true;
                    }
                }

                if (!massiveChange)
                {
                    double mult = (1 + (ClimatesOfFerngill.Dice.NextDouble() * 1.3) - .55);
                    if (currRain == 0)
                        currRain = 4;

                    if (currRain < WeatherUtilities.GetRainCategoryMidPoint(RainLevels.Light))
                        mult += 4.5;

                    currRain = (int)(Math.Floor(currRain * mult));
                }
            }



            if (!ClimatesOfFerngill.WeatherOpt.HazardousWeather)
            {
                if (ClimatesOfFerngill.WeatherOpt.Verbose)
                    ClimatesOfFerngill.Logger.Log("Triggering Hazardous Weather Failsafe");

                currRain = WeatherUtilities.GetRainCategoryMidPoint(RainLevels.Severe);
            }

            if (WeatherConditions.PreventGoingOutside(currRain))
            {
                if (Game1.timeOfDay >= 2300)
                {
                    //drop the rain to at least allow the person to return home if they aren't.
                    currRain = WeatherUtilities.GetRainCategoryMidPoint(RainLevels.Severe);
                }                    
            }

            if (currRain > ClimatesOfFerngill.WeatherOpt.MaxRainFall)
                currRain = ClimatesOfFerngill.WeatherOpt.MaxRainFall;

            if (currRain < 0) currRain = 0;

            if (currRain != prevRain && Game1.timeOfDay != 600 && showRain && !(Game1.currentLocation is Desert))
            {
                if (WeatherUtilities.GetRainCategory(currRain) == RainLevels.None)
                {
                    Game1.addHUDMessage(new HUDMessage(Translation.Get("hud-text.NearlyNoRain")));
                    return currRain;
                }

                else if (currRain > prevRain)
                {
                    if (WeatherUtilities.GetRainCategory(currRain) != WeatherUtilities.GetRainCategory(prevRain))
                    {
                        Game1.addHUDMessage(new HUDMessage(Translation.Get("hud-text.CateIncrease", new { currRain, category = WeatherUtilities.DescRainCategory(currRain) })));
                        return currRain;
                    }
                    else if (Math.Abs(currRain / prevRain) >= ClimatesOfFerngill.WeatherOpt.VRainNotifThreshold || ClimatesOfFerngill.WeatherOpt.Verbose)
                    {
                        Game1.addHUDMessage(new HUDMessage(Translation.Get("hud-text.Increase", new { currRain })));
                        return currRain;
                    }
                }
                else
                {
                    if (WeatherUtilities.GetRainCategory(currRain) != WeatherUtilities.GetRainCategory(prevRain))
                    {
                        Game1.addHUDMessage(new HUDMessage(Translation.Get("hud-text.CateDecrease", new { currRain, category = WeatherUtilities.DescRainCategory(currRain) })));
                        return currRain;
                    }
                    else if (Math.Abs(currRain / prevRain) >= ClimatesOfFerngill.WeatherOpt.VRainNotifThreshold || ClimatesOfFerngill.WeatherOpt.Verbose)
                    {
                        Game1.addHUDMessage(new HUDMessage(Translation.Get("hud-text.Decrease", new { currRain })));
                        return currRain;
                    }
                }
            }

            return currRain;
        }

        internal static void SetWeatherTomorrow(string Result, MersenneTwister Dice, FerngillClimate GameClimate, double stormOdds, RangePair TmrwTemps)
        {
            //now parse the result.
            if (Result == "rain")
            {
                SDVUtilities.SetWeather(Game1.weather_rain);

				if (Game1.currentSeason == "winter")
				{
					SDVUtilities.SetWeather(Game1.weather_snow);
					
					if (GameClimate.AllowRainInWinter && ClimatesOfFerngill.Conditions.TomorrowHigh >= -2.5)
					{
                        SDVUtilities.SetWeather(Game1.weather_rain);
					}
				}

                //Moved from the *wrong function and logic gate*
                //snow applies first
                if (WeatherConditions.IsValidWeatherForSnow(TmrwTemps) && Game1.currentSeason != "spring")
                {
                    if (ClimatesOfFerngill.WeatherOpt.Verbose)
                        ClimatesOfFerngill.Logger.Log($"Snow is enabled, with the High for the day being: {TmrwTemps.HigherBound}" +
                                    $" and the calculated midpoint temperature being {TmrwTemps.GetMidPoint()}");

                    SDVUtilities.SetWeather(Game1.weather_snow);
                }

                //apply lightning logic.
                if (Dice.NextDoublePositive() >= stormOdds && Game1.weatherForTomorrow == Game1.weather_rain)
                {
                    SDVUtilities.SetWeather(Game1.weather_lightning);
                    if (SDate.Now().Year == 1 && SDate.Now().Season == "spring" && !ClimatesOfFerngill.WeatherOpt.AllowStormsSpringYear1)
                        SDVUtilities.SetWeather(Game1.weather_rain);
                }

                //tracking time! - Snow fall on Fall 28, if the flag is set.
                if (Game1.dayOfMonth == 28 && Game1.currentSeason == "fall" && ClimatesOfFerngill.WeatherOpt.SnowOnFall28)
                {
                    ClimatesOfFerngill.Conditions.ForceTodayTemps(2, -1);
                    SDVUtilities.SetWeather(Game1.weather_snow);
                }
            }

            if (Result == "debris")
            {
                SDVUtilities.SetWeather(Game1.weather_debris);
            }

            if (Result == "sunny")
            {
                SDVUtilities.SetWeather(Game1.weather_sunny);
            }
        }

        internal static void SetWeatherNonSystemForTomorrow(MersenneTwister Dice, FerngillClimate GameClimate, double rainDays, double stormDays, double windyDays, RangePair TmrwTemps)
        {
            ProbabilityDistribution<string> WeatherDist = new ProbabilityDistribution<string>("sunny");

            WeatherDist.AddNewEndPoint(rainDays, "rain");
            if (ClimatesOfFerngill.WeatherOpt.DisableHighRainWind)
                WeatherDist.AddNewCappedEndPoint(windyDays, "debris");

            double distOdd = Dice.NextDoublePositive();

            if (ClimatesOfFerngill.WeatherOpt.Verbose)
            {
                ClimatesOfFerngill.Logger.Log(WeatherDist.ToString());
                ClimatesOfFerngill.Logger.Log($"Distribution odds is {distOdd}");
            }

            if (!(WeatherDist.GetEntryFromProb(distOdd, out string Result)))
            {
                Result = "sunny";
                ClimatesOfFerngill.Logger.Log("The weather has failed to process in some manner. Falling back to [sunny]", LogLevel.Info);
            }

            if (ClimatesOfFerngill.WeatherOpt.Verbose)
                ClimatesOfFerngill.Logger.Log($"Weather result is {Result}");

            SetWeatherTomorrow(Result, Dice, GameClimate, stormDays, TmrwTemps);
        }

        internal static void Init()
        {
            CropList = new List<Vector2>();
        }

        internal static void DynamicRainOnNewDay(WeatherConditions curr, MersenneTwister Dice)
        {
            if (!curr.HasWeather(CurrentWeather.Rain))
                return;

            curr.SetRainAmt(70);
            //Rain chances. This will also affect storms (in that storms still can have variable rain) .
            //only roll this if it's actually raining. :|
            double roll = Dice.NextDouble();

            if (roll <= ClimatesOfFerngill.WeatherOpt.VariableRainChance && Game1.isRaining)
            {
                ClimatesOfFerngill.Logger.Log($"With {roll}, we are setting for variable rain against {ClimatesOfFerngill.WeatherOpt.VariableRainChance}");
                curr.SetVariableRain(true);

                //check for overcast chance first.
                if (Dice.NextDouble() < ClimatesOfFerngill.WeatherOpt.OvercastChance && !Game1.isLightning)
                {
                    WeatherUtilities.SetWeatherOvercast(true);
                    SDVUtilities.AlterWaterStatusOfCrops(water: false);
                    SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_overcast"), 0);
                }

                //now that we've done that base check, we need to change the rainfall... 
                //... after we calc rainfall to see if it's watered. Essentially, every time this is called, 
                //... it'll check to see if it should flip the tiles.

                //Variable Rain may set the starting rain at 0300 to be something else than normal, and as such, StartingRain should 
                // only be checked if IsVariableRain is true.

                // Odds are fixed: Normal (default) is 72%, Light is 16%, Heavy is 6%, Sunshower is 5%, Severe is 1%
                double startingRainRoll = Dice.NextDouble();
                if (startingRainRoll <= .72)
                {
                    curr.StartingRain = RainLevels.Normal;
                }
                else if (startingRainRoll > .72 && startingRainRoll <= .88)
                {
                    curr.StartingRain = RainLevels.Light;
                }
                else if (startingRainRoll > .88 && startingRainRoll <= .94)
                {
                    curr.StartingRain = RainLevels.Heavy;
                }
                else if (startingRainRoll > .94 && startingRainRoll <= .99)
                {
                    curr.StartingRain = RainLevels.Sunshower;
                }
                else if (startingRainRoll > .99)
                {
                    curr.StartingRain = RainLevels.Severe;
                }

                if (Game1.isLightning && !(curr.StartingRain == RainLevels.Light || curr.StartingRain == RainLevels.Sunshower || curr.StartingRain == RainLevels.Normal))
                    curr.StartingRain = RainLevels.Heavy;

                curr.SetRainAmt(WeatherUtilities.GetRainCategoryMidPoint(curr.StartingRain));
                curr.TodayRain = curr.AmtOfRainDrops;

                //now run this for 0300 to 0600.
                for (int i = 0; i < 17; i++)
                {
                    curr.SetRainAmt(GetNewRainAmount(curr.AmtOfRainDrops, ClimatesOfFerngill.Translator));
                    curr.TodayRain += curr.AmtOfRainDrops;
                }

                //set starting amount at 6am
                curr.RefreshRainAmt();
            }
        }

        internal static void CheckForStaticRainChanges(WeatherConditions curr, MersenneTwister Dice, double ChanceForNonNormalRain)
        {
            if (Game1.isLightning && Game1.isRaining)
            {
                curr.SetRainAmt(130);
            }
            double rainOdds, newRainOdds;
            rainOdds = Dice.NextDouble();
            newRainOdds = Dice.NextDouble();

            //random chance to just set rain at a differnt number
            if (rainOdds < ChanceForNonNormalRain)
            {
                //yay use of probablity distribution
                ProbabilityDistribution<RainLevels> RainSpread = new ProbabilityDistribution<RainLevels>(RainLevels.Normal);
                RainSpread.AddNewCappedEndPoint(.12, RainLevels.Sunshower);
                RainSpread.AddNewCappedEndPoint(.28, RainLevels.Light);
                RainSpread.AddNewCappedEndPoint(.351, RainLevels.Normal);
                RainSpread.AddNewCappedEndPoint(.1362, RainLevels.Moderate);
                RainSpread.AddNewCappedEndPoint(.09563, RainLevels.Heavy);
                RainSpread.AddNewCappedEndPoint(.0382, RainLevels.Severe);
                RainSpread.AddNewCappedEndPoint(.0094, RainLevels.Torrential);
                RainSpread.AddNewCappedEndPoint(.0049, RainLevels.Typhoon);
                RainSpread.AddNewCappedEndPoint(.00287, RainLevels.NoahsFlood);

                if (!(RainSpread.GetEntryFromProb(newRainOdds, out RainLevels Result)))
                {
                    Result = RainLevels.Normal;
                    ClimatesOfFerngill.Logger.Log("The rain probablity spread has failed to find an rain level. Falling back to normal rain", LogLevel.Error);
                }
                
                curr.SetRainAmt(WeatherUtilities.ReturnRndRainAmtInLevel(Dice, Result));

                if (ClimatesOfFerngill.WeatherOpt.Verbose)
                {
                    ClimatesOfFerngill.Logger.Log($"We've set the rain to a non normal value - with roll {rainOdds} for setting non normal, and {newRainOdds} for category {Result}, resulting in new rain target {curr.AmtOfRainDrops} in category {WeatherUtilities.GetRainCategory(curr.AmtOfRainDrops)}");
                }
                

            }

            curr.TodayRain += curr.AmtOfRainDrops;
            curr.RefreshRainAmt();

        }

        internal static bool TestForSpecialWeather(WeatherConditions curr, FerngillClimateTimeSpan ClimateForDay)
        {
            bool specialWeatherTriggered = false;
            // Conditions: Blizzard - occurs in weather_snow in "winter"
            //             Dry Lightning - occurs if it's sunny in any season if temps exceed 25C.
            //             Frost and Heatwave check against the configuration.
            //             Thundersnow  - as Blizzard, but really rare. Will not happen in fog, may happen in Blizzard/WhiteOut
            //             Sandstorm - windy, with no precip for several days. Spring-Fall only, highest chance in summer.
            //             Fog - per climate, although night fog in winter is double normal chance
            curr.GenerateEveningFog = (ClimatesOfFerngill.Dice.NextDouble() < ClimateForDay.EveningFogChance * ClimateForDay.RetrieveOdds(ClimatesOfFerngill.Dice, "fog", Game1.dayOfMonth)) && !curr.GetCurrentConditions().HasFlag(CurrentWeather.Wind);

            bool blockFog = ClimatesOfFerngill.MoonAPI != null && ClimatesOfFerngill.MoonAPI.IsSolarEclipse();

            if (blockFog || ClimatesOfFerngill.WeatherOpt.DisableAllFog)
                curr.GenerateEveningFog = false;

            double fogRoll = (ClimatesOfFerngill.WeatherOpt.DisableAllFog ? 1.1 : ClimatesOfFerngill.Dice.NextDoublePositive());

            if (fogRoll < ClimateForDay.RetrieveOdds(ClimatesOfFerngill.Dice, "fog", Game1.dayOfMonth) && !curr.GetCurrentConditions().HasFlag(CurrentWeather.Wind) && !blockFog)
            {
                curr.CreateWeather("Fog");

                if (ClimatesOfFerngill.WeatherOpt.Verbose)
                    ClimatesOfFerngill.Logger.Log($"{curr.FogDescription(fogRoll, ClimateForDay.RetrieveOdds(ClimatesOfFerngill.Dice, "fog", Game1.dayOfMonth))}");

                specialWeatherTriggered = true;
            }

            //set special temps before we check for the results of them
            if (ClimatesOfFerngill.Dice.NextDouble() < ClimateForDay.HeatwaveChance)
            {
                SetHotwave();
            }
            else if (ClimatesOfFerngill.Dice.NextDouble() < ClimateForDay.ChillwaveChance)
            {
                SetChillWave();
            }

            //do these here
            //20190626 - Thanks to Crops Anywhere, I have to add this 
            if ((curr.TodayLow < ClimatesOfFerngill.WeatherOpt.TooColdOutside && !Game1.IsWinter) || (curr.TodayLow < ClimatesOfFerngill.WeatherOpt.TooColdOutside && Game1.IsWinter &&
                ClimatesOfFerngill.WeatherOpt.ApplyFrostsInWinter))
            {
                if (ClimatesOfFerngill.WeatherOpt.HazardousWeather)
                {
                    curr.AddWeather(CurrentWeather.Frost);
                    specialWeatherTriggered = true;
                }
            }

            //test for spring conversion
            if (curr.HasWeather(CurrentWeather.Rain) && curr.HasWeather(CurrentWeather.Frost) && (Game1.currentSeason == "spring" || Game1.currentSeason == "fall")
                && ClimatesOfFerngill.Dice.NextDoublePositive() <= ClimatesOfFerngill.WeatherOpt.RainToSnowConversion)
            {
                curr.RemoveWeather(CurrentWeather.Rain);
                curr.AddWeather(CurrentWeather.Snow);
                Game1.isRaining = false;
                Game1.isSnowing = true;
                specialWeatherTriggered = true;
            }


            if (curr.HasWeather(CurrentWeather.Snow))
            {
                double blizRoll = ClimatesOfFerngill.Dice.NextDoublePositive();
                double blizOdds = ClimateForDay.RetrieveOdds(ClimatesOfFerngill.Dice, "blizzard", Game1.dayOfMonth);
                if (blizRoll <= blizOdds)
                {
                    curr.CreateWeather("Blizzard");
                    if (ClimatesOfFerngill.WeatherOpt.Verbose)
                        ClimatesOfFerngill.Logger.Log($"With roll {blizRoll:N3} against {blizOdds}, there will be blizzards today");
                    if (ClimatesOfFerngill.Dice.NextDoublePositive() < ClimatesOfFerngill.WeatherOpt.WhiteOutChances && ClimatesOfFerngill.WeatherOpt.HazardousWeather)
                    {
                        curr.CreateWeather("WhiteOut");
                    }
                }

                specialWeatherTriggered = true;
            }

            //Dry Lightning is also here for such like the dry and arid climates 
            //  which have so low rain chances they may never storm.
            if (curr.HasWeather(CurrentWeather.Snow))
            {
                double oddsRoll = ClimatesOfFerngill.Dice.NextDoublePositive();
				double thunderSnowOdds = ClimateForDay.RetrieveOdds(ClimatesOfFerngill.Dice,"storm",Game1.dayOfMonth);
				
                if (oddsRoll <= thunderSnowOdds && !curr.HasWeather(CurrentWeather.Fog))
                {
                    curr.AddWeather(CurrentWeather.Lightning);
                    if (ClimatesOfFerngill.WeatherOpt.Verbose)
                        ClimatesOfFerngill.Logger.Log($"With roll {oddsRoll:N3} against {thunderSnowOdds}, there will be thundersnow today");

                    specialWeatherTriggered = true;
                }
            }

            if (!(curr.HasPrecip()))
            {
                double oddsRoll = ClimatesOfFerngill.Dice.NextDoublePositive();

                if (oddsRoll <= ClimatesOfFerngill.WeatherOpt.DryLightning && curr.TodayHigh >= ClimatesOfFerngill.WeatherOpt.DryLightningMinTemp &&
                    !curr.HasWeather(CurrentWeather.Frost))
                {
                    curr.AddWeather(CurrentWeather.Lightning);
                    if (ClimatesOfFerngill.WeatherOpt.Verbose)
                        ClimatesOfFerngill.Logger.Log($"With roll {oddsRoll:N3} against {ClimatesOfFerngill.WeatherOpt.DryLightning}, there will be dry lightning today.");

                    specialWeatherTriggered = true;
                }

                if (curr.TodayHigh > ClimatesOfFerngill.WeatherOpt.TooHotOutside && ClimatesOfFerngill.WeatherOpt.HazardousWeather)
                {
                    curr.AddWeather(CurrentWeather.Heatwave);
                    specialWeatherTriggered = true;
                }

                double sandstormOdds = .18;
                if (Game1.currentSeason == "summer")
                    sandstormOdds *= 1.2;

                if (oddsRoll < sandstormOdds && ClimatesOfFerngill.WeatherOpt.HazardousWeather && Game1.isDebrisWeather)
                {
                    curr.AddWeather(CurrentWeather.Sandstorm);
                    specialWeatherTriggered = true;
                    curr.CreateWeather("Sandstorm");
                }
            }

            //and finally, test for thunder frenzy
            if (curr.HasWeather(CurrentWeather.Lightning) && curr.HasWeather(CurrentWeather.Rain) && ClimatesOfFerngill.WeatherOpt.HazardousWeather)
            {
                double oddsRoll = ClimatesOfFerngill.Dice.NextDouble();
                if (oddsRoll < ClimatesOfFerngill.WeatherOpt.ThunderFrenzyOdds)
                {
                    curr.AddWeather(CurrentWeather.ThunderFrenzy);
                    specialWeatherTriggered = true;
                    if (ClimatesOfFerngill.WeatherOpt.Verbose)
                        ClimatesOfFerngill.Logger.Log($"With roll {oddsRoll:N3} against {ClimatesOfFerngill.WeatherOpt.ThunderFrenzyOdds}, there will be a thunder frenzy today");
                    curr.CreateWeather("ThunderFrenzy");
                }
            }

            return specialWeatherTriggered;
        }

        internal static void Reset()
        {
            ExpireTime = 0;
            CropList.Clear();
        }

        internal static void OnNewDay()
        {
            ExpireTime = 0;
            CropList.Clear();
        }

        internal static void SetHotwave()
        {
            double low, high;
            low = ClimatesOfFerngill.GetClimateForDay(SDate.Now()).RetrieveMaxTemp("low", Game1.dayOfMonth) + 1;
            high = ClimatesOfFerngill.GetClimateForDay(SDate.Now()).RetrieveMaxTemp("high",Game1.dayOfMonth) + 2;
            ClimatesOfFerngill.Conditions.SetTodayTemps(new RangePair(low, high, true));
            ClimatesOfFerngill.Conditions.IsAbnormalHeat = true;
        }

        internal static void SetChillWave()
        {
            double low, high;
            low = ClimatesOfFerngill.GetClimateForDay(SDate.Now()).RetrieveMinTemp("low",Game1.dayOfMonth) - 2;
            high = ClimatesOfFerngill.GetClimateForDay(SDate.Now()).RetrieveMinTemp("high", Game1.dayOfMonth) - 1;
            ClimatesOfFerngill.Conditions.SetTodayTemps(new RangePair(low, high, true));
            ClimatesOfFerngill.Conditions.IsAbnormalChill = true;
        }

        internal static void HandleStormTotemInterception(DialogueBox box, double odds, double stormOdds)
        {
            bool stormDialogue = false;
            List<string> lines = ClimatesOfFerngill.Reflection.GetField<List<string>>(box, "dialogues").GetValue();
            if (lines.FirstOrDefault() == Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"))
            {
                if (ClimatesOfFerngill.WeatherOpt.Verbose)
                    ClimatesOfFerngill.Logger.Log($"Rain totem interception firing with roll {odds:N3} vs. odds {stormOdds:N3}");

                // rain totem used, do your thing
                if (ClimatesOfFerngill.WeatherOpt.StormTotemChange)
                {
                    if (odds <= stormOdds)
                    {
                        if (ClimatesOfFerngill.WeatherOpt.Verbose)
                            ClimatesOfFerngill.Logger.Log("Replacing rain with storm..");

                        SDVUtilities.SetWeather(Game1.weather_lightning);
                        stormDialogue = true;
                    }
                }

                // change dialogue text
                lines.Clear();
                lines.Add(stormDialogue
                    ? ClimatesOfFerngill.Translator.Get("hud-text.desc_stormtotem")
                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"));
            }
        }

        internal static HUDMessage HandleOnSaving(WeatherConditions Conditions, MersenneTwister Dice)
        {
            if (Conditions.HasWeather(CurrentWeather.Frost) && ClimatesOfFerngill.WeatherOpt.AllowCropDeath)
            {
                Farm f = Game1.getFarm();
                int count = 0,
                    maxCrops = (int)Math.Floor(SDVUtilities.CropCountInFarm(f) * ClimatesOfFerngill.WeatherOpt.DeadCropPercentage);

                foreach (KeyValuePair<Vector2, TerrainFeature> tf in f.terrainFeatures.Pairs)
                {
                    if (count >= maxCrops)
                        break;

                    if (tf.Value is HoeDirt curr && curr.crop != null)
                    {
                        Crop test = new Crop(curr.crop.indexOfHarvest.Value, 0, 0);

                        if (Dice.NextDouble() > ClimatesOfFerngill.WeatherOpt.CropResistance && (!test.seasonsToGrowIn.Contains("winter") || !SDVUtilities.IsWinterForageable(test.indexOfHarvest.Value)))
                        {
                            CropList.Add(tf.Key);
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    foreach (Vector2 v in CropList)
                    {
                        HoeDirt hd = (HoeDirt)f.terrainFeatures[v];
                        hd.crop.dead.Value = true;
                    }

                    return (new HUDMessage(
                        ClimatesOfFerngill.Translator.Get("hud-text.desc_frost_killed", new { deadCrops = count }),
                        Color.SeaGreen, 5250f, true)
                    {
                        whatType = 2
                    });
                }

            }

            return null;
        }


        internal static void ProcessHazardousCropWeather(WeatherConditions curr, int timeOfDay, MersenneTwister Dice)
        {
            //frost works at night, heatwave works during the day
            if (timeOfDay == 1700)
            {
                if (curr.HasWeather(CurrentWeather.Heatwave) || curr.HasWeather(CurrentWeather.Sandstorm))
                {
                    ClimatesOfFerngill.Logger.Log("Beginning Heatwave code");
                    ExpireTime = 2000;
                    Farm f = Game1.getFarm();
                    int count = 0, maxCrops = (int)Math.Floor(SDVUtilities.CropCountInFarm(f) * ClimatesOfFerngill.WeatherOpt.DeadCropPercentage);

                    foreach (KeyValuePair<Vector2, TerrainFeature> tf in f.terrainFeatures.Pairs)
                    {
                        if (count >= maxCrops)
                            break;

                        if (tf.Value is HoeDirt dirt && dirt.crop != null)
                        {
                            if (Dice.NextDouble() <= ClimatesOfFerngill.WeatherOpt.CropResistance)
                            {
                                if (ClimatesOfFerngill.WeatherOpt.Verbose)
                                    ClimatesOfFerngill.Logger.Log($"Dewatering crop at {tf.Key}. Crop is {dirt.crop.indexOfHarvest}");

                                CropList.Add(tf.Key);
                                dirt.state.Value = HoeDirt.dry;
                                count++;
                            }
                        }
                    }

                    if (CropList.Count > 0)
                    {
                        if (ClimatesOfFerngill.WeatherOpt.AllowCropDeath)
                        {
                            if (curr.HasWeather(CurrentWeather.Heatwave) && !curr.HasWeather(CurrentWeather.Sandstorm))
                                SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_heatwave_kill", new { crops = count }), 3);
                            if (curr.HasWeather(CurrentWeather.Sandstorm))
                                SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_sandstorm_kill", new { crops = count }), 3);
                        }
                        else
                        {
                            if (curr.HasWeather(CurrentWeather.Heatwave) && !curr.HasWeather(CurrentWeather.Sandstorm))
                                SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_heatwave_dry", new { crops = count }), 3);
                            if (curr.HasWeather(CurrentWeather.Sandstorm))
                                SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_sandstorm_dry", new { crops = count }), 3);
                        }
                    }
                    else
                    {
                        SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_heatwave"), 3);
                    }
                }
            }

            if (Game1.timeOfDay == ExpireTime && ClimatesOfFerngill.WeatherOpt.AllowCropDeath)
            {
                ClimatesOfFerngill.Logger.Log("Beginning Crop Death code");
                //if it's still de watered - kill it.
                Farm f = Game1.getFarm();
                bool cDead = false;

                foreach (Vector2 v in CropList)
                {
                    HoeDirt hd = (HoeDirt)f.terrainFeatures[v];
                    if (hd.state.Value == HoeDirt.dry)
                    {
                        if (ClimatesOfFerngill.WeatherOpt.Verbose)
                            ClimatesOfFerngill.Logger.Log($"Killing crop at {v}. Crop is {hd.crop.indexOfHarvest}");

                        hd.crop.dead.Value = true;
                        cDead = true;
                    }
                }

                CropList.Clear(); //clear the list
                if (cDead)
                {
                    SDVUtilities.ShowMessage(ClimatesOfFerngill.Translator.Get("hud-text.desc_heatwave_cropdeath"), 3);
                }
            }
        }

        internal static double GetWeatherSystemOddsForNewDay(WeatherConditions conditions)
        {
            int nextDay = conditions.trackerModel.WeatherSystemDays + 1;
            double defaultOdds = -.08333333 * nextDay + .14; //+14% not +1400% self. :|
            //Console.WriteLine($"Odds generated for day {nextDay} of system are {defaultOdds} before curving the results");

            //this slope is added to give a small curve to the slope.
            if (nextDay >= 6)
                defaultOdds += -.083333333 * nextDay + .5;

            //Console.WriteLine($"Odds generated for day {nextDay} of system are {defaultOdds} after curving the results");
            if (defaultOdds < 0)
                return 0;
            
            return defaultOdds;
        }
    }
	
}
