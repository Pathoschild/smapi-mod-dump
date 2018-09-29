using StardewValley;
using StardewModdingAPI;
using TwilightShards.Stardew.Common;
using TwilightShards.Common;
using System;
using EnumsNET;
using System.Collections.Generic;

namespace ClimatesOfFerngillRebuild
{
    internal class StaminaDrain
    {
        private WeatherConfig Config;
        private ITranslationHelper Helper;
        private bool FarmerSick;
        public bool FarmerHasBeenSick;
        private IMonitor Monitor;

        private readonly int FROST = 1;
        private readonly int HEATWAVE = 2;

        public StaminaDrain(WeatherConfig Options, ITranslationHelper SHelper, IMonitor mon)
        {
            Config = Options;
            Helper = SHelper;
            Monitor = mon;
        }

        public bool IsSick()
        {
            return this.FarmerSick;
        }

       public void MakeSick(int reason = 0)
        {
            FarmerSick = true;
            FarmerHasBeenSick = true;
            if (reason == FROST)
            {
                SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_freeze"));
            }
            else if (reason == HEATWAVE)
            {
                SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_exhaust"));
            }
            else          
                SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_sick"));
        }

        public void OnNewDay()
        {
            FarmerSick = false;
        }

        public void ClearDrain()
        {
            FarmerSick = false;
            SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_cold_removed"));
        }

        public void Reset()
        {
            FarmerSick = false;
        }

        public bool FarmerCanGetSick()
        {
            if (FarmerSick && !Config.SickMoreThanOnce)
                return false;

            if (!Config.SickMoreThanOnce && FarmerHasBeenSick)
                return false;

            return true;
        }

        public int TenMinuteTick(WeatherConditions conditions, int ticksOutside, int ticksTotal, MersenneTwister Dice)
        {
            double amtOutside = ticksOutside / (double)ticksTotal, totalMulti = 0;
            int staminaAffect = 0;
            int sickReason = 0;
            var condList = new List<string>();

           /* if (Config.Verbose)
                Monitor.Log($"Ticks: {ticksOutside}/{ticksTotal} with percentage {amtOutside.ToString("N3")} against" +
                    $" target {Config.AffectedOutside}"); */

            //Logic: At all times, if the today danger is not null, we should consider processing.
            //However: If it's frost, only at night. If it's a heatwave, only during the day.
            //So this means: if it's storming, you can get sick. If it's a blizzard or thundersnow.. you can get sick
            //If it's frost or heatwave during the appropriate time.. you can get sick

            //First, update the sick status
            bool farmerCaughtCold = false;
            double sickOdds = Config.ChanceOfGettingSick - Game1.dailyLuck;

            //weee.
            if (Game1.player.hat?.which == 28 && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Lightning))
                sickOdds -= (Dice.NextDoublePositive() / 5.0) - .1;

            if (Game1.player.hat?.which == 25 && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Blizzard))
                sickOdds -= .22;

            if (Game1.player.hat?.which == 4 && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Heatwave) && !SDVTime.IsNight)
                sickOdds -= .11;

            farmerCaughtCold = (Dice.NextDoublePositive() <= sickOdds);

            if (amtOutside >= Config.AffectedOutside && farmerCaughtCold || this.FarmerSick)
            {
                //check if it's a valid condition
                if (FarmerCanGetSick())
                {
                    if (conditions.GetCurrentConditions().HasAnyFlags(CurrentWeather.Blizzard | CurrentWeather.Lightning) || (conditions.GetCurrentConditions().HasFlag(CurrentWeather.Frost) && SDVTime.IsNight) | (conditions.GetCurrentConditions().HasFlag(CurrentWeather.Heatwave) && !SDVTime.IsNight))
                    {
                        if ((conditions.GetCurrentConditions().HasFlag(CurrentWeather.Heatwave) && !SDVTime.IsNight))
                            sickReason = HEATWAVE;
                        else if (conditions.GetCurrentConditions().HasFlag(CurrentWeather.Frost) && SDVTime.IsNight)
                            sickReason = FROST;

                        this.MakeSick(sickReason);
                    }
                }

                //test status
                /*if (Config.Verbose)              
                    Monitor.Log($"Status update. Farmer Sick: {FarmerSick} and Valid Conditions: {conditions.GetCurrentConditions().HasAnyFlags(CurrentWeather.Blizzard | CurrentWeather.Lightning) || (conditions.GetCurrentConditions().HasFlag(CurrentWeather.Frost) && SDVTime.IsNight) | (conditions.GetCurrentConditions().HasFlag(CurrentWeather.Heatwave) && !SDVTime.IsNight)}"); */

                //now that we've done that, go through the various conditions
                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Lightning))
                {
                    totalMulti += 1;
                    condList.Add("Lightning or Thundersnow");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Fog))
                {
                    totalMulti += .5;
                    condList.Add("Fog");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Fog) && SDVTime.IsNight)
                {
                    totalMulti += .25;
                    condList.Add("Night Fog");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Blizzard) && !conditions.GetCurrentConditions().HasFlag(CurrentWeather.WhiteOut))
                {
                    totalMulti += 1.25;
                    condList.Add("Blizzard");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Blizzard) && conditions.GetCurrentConditions().HasFlag(CurrentWeather.WhiteOut))
                {
                    totalMulti += 2.25;
                    condList.Add("White Out");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Frost) && SDVTime.IsNight)
                {
                    totalMulti += 1.25;
                    condList.Add("Night Frost");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasAllFlags(CurrentWeather.Lightning | CurrentWeather.Snow) && SDVTime.IsNight)
                {
                    totalMulti += .5;
                    condList.Add("Night Thundersnow");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Blizzard) && SDVTime.IsNight)
                {
                    totalMulti += .5;
                    condList.Add("Night Blizzard");
                }

                if (this.FarmerSick && conditions.GetCurrentConditions().HasFlag(CurrentWeather.Heatwave) && !SDVTime.IsNight)
                {
                    totalMulti += 1.25;
                    condList.Add("Day Heatwave");
                }
            }


            staminaAffect -= (int)Math.Floor(Config.StaminaDrain * totalMulti);
            
            if (Config.Verbose && this.FarmerSick)
            {
                string condString = "[ ";
                for (int i = 0; i < condList.Count; i++)
                {
                    if (i != condList.Count - 1)
                    {
                        condString += condList[i] + ", ";
                    }
                    else
                    {
                        condString += condList[i];
                    }
                }
                condString += " ]";
                /*
                Monitor.Log($"[{Game1.timeOfDay}] Conditions for the drain are {condString} for a total multipler of {totalMulti} for a total drain of {staminaAffect}"); */
            }
            
            return staminaAffect;
        }
    }
}
