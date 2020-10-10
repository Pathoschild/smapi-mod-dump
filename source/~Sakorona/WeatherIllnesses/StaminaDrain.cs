/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using TwilightShards.Stardew.Common;
using TwilightShards.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using TwilightShards.WeatherIllnesses.Integrations;

namespace TwilightShards.WeatherIllnesses
{
    internal enum IllCauses
    {
        None,
        TooColdOutside,
        TooHotOutside,
        Darude,
        TooColdInside,
        InclementWeather,
        BlizzardsOutside,
        TheWampaWillGetYou,
        NonspecificSevereWeather
    }

    internal class StaminaDrain
    {
        private readonly int UniqueStaID = 48712560;

        private const int SicknessBuffID = 24;
        private const int FogBuffID = 26;

        private readonly IllnessConfig IllOptions;
        private readonly ITranslationHelper Helper;

        private bool FogDebuffValid;
        private bool FarmerSick;
        private bool turnTheHeatOn;

        private IllCauses ReasonSick ;
        public bool FarmerHasBeenSick;
        public bool IssuedInHouseWarning;
        private readonly IMonitor Monitor;

        private int timeInBathHouse;

        public const int MedicineClear = 1;
        public const int BathHouseClear = 2;

        public StaminaDrain(IllnessConfig Options, ITranslationHelper SHelper, IMonitor mon)
        {
            IllOptions = Options;
            Helper = SHelper;
            Monitor = mon;
            IssuedInHouseWarning = false;
        }

        public bool IsSick()
        {
            return this.FarmerSick;
        }

        public void MakeSick()
        {
            FarmerSick = true;
            FarmerHasBeenSick = true;
            if (IllOptions.Verbose)
                Monitor.Log("Adding buff icon!!");

            switch (ReasonSick)
            {
                case IllCauses.InclementWeather:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_inclement"),4);
                    break;
                case IllCauses.BlizzardsOutside:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_blizzard"),4);
                    break;
                case IllCauses.NonspecificSevereWeather:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_flu"),4);
                    break;
                case IllCauses.TheWampaWillGetYou:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_wampa"),4);
                    break;
                case IllCauses.Darude:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_darude"),4);
                    break;
                case IllCauses.TooColdInside:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_turntheheaton"), 4);
                    break;
                case IllCauses.TooColdOutside:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_cold"), 4);
                    break;
                case IllCauses.TooHotOutside:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_hot"), 4);
                    break;
                default:
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_sick"),4);
                    break;
            }
        }

        public void OnNewDay()
        {
            FarmerSick = false;
            FogDebuffValid = false;
            ReasonSick = IllCauses.None;
            IssuedInHouseWarning = false;
            turnTheHeatOn = false;
        }

        public void ClearFog()
        {
            FogDebuffValid = false;
            if (Game1.buffsDisplay.otherBuffs.Any())
            {
                int buffId = UniqueStaID + FogBuffID;
                int i = Game1.buffsDisplay.otherBuffs.FindIndex(p => p.which == buffId);
                //of course a not found is a negative index.
                if (i > 0)
                    Game1.buffsDisplay.otherBuffs.RemoveAt(i);
            }

            if (IllOptions.Verbose)
                Monitor.Log("Removing buff icon!!");

            SDVUtilities.ShowMessage("The evil is defeated!", 4);
        }

        public void ClearDrain(int reason = 1)
        {
            FarmerSick = false;
            if (Game1.buffsDisplay.otherBuffs.Any())
            {
                int buffId = UniqueStaID + SicknessBuffID;
                int i = Game1.buffsDisplay.otherBuffs.FindIndex(p => p.which == buffId);
                //of course a not found is a negative index.
                if (i > 0)
                    Game1.buffsDisplay.otherBuffs.RemoveAt(i);
            }

            if (IllOptions.Verbose)
                Monitor.Log("Removing buff icon!!");

            if (reason == StaminaDrain.MedicineClear)
            {
                SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_cold_removed"), 4);
            }
            else if (reason == StaminaDrain.BathHouseClear)
            {
                SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_bathHouse"),4);
            }
        }

        public void Reset()
        {
            FarmerSick = false;
            IssuedInHouseWarning = false;
            turnTheHeatOn = false;
        }

        public bool FarmerCanGetSick()
        {
            if (FarmerSick && !IllOptions.SickMoreThanOnce)
                return false;

            if (!IllOptions.SickMoreThanOnce && FarmerHasBeenSick)
                return false;

            return true;
        }

        public int TenMinuteTick(int? hatID, double? temp, string conditions,int ticksInHouse, int ticksOutside, int ticksTotal, MersenneTwister Dice)
        {
            double amtOutside = ticksOutside / (double)ticksTotal, totalMulti = 0;
            double amtInHouse = ticksInHouse / (double)ticksTotal;
            int staminaAffect = 0;
            var condList = new List<string>();

            if (IllOptions.Verbose)
            {
                Monitor.Log($"Ticks: {ticksOutside}/{ticksTotal} with percentage {amtOutside:N3} against target {IllOptions.PercentageOutside}");
                Monitor.Log($"Ticks in house is {amtInHouse:N3} against target {IllOptions.PercentageOutside}");
                Monitor.Log($"Current Condition: {conditions}");
            }
            
            //First, update the sick status
            double sickOdds = IllOptions.ChanceOfGettingSick - Game1.player.DailyLuck;

            //weee.
            if (hatID == 28 && (conditions.Contains("lightning") || conditions.Contains("stormy") || conditions.Contains("thundersnow")))
                sickOdds -= (Dice.NextDoublePositive() / 5.0) - .1;

            if (hatID == 25 && conditions.Contains("blizzard") || conditions.Contains("whiteout"))
                sickOdds -= .22;

            if (hatID == 4 && conditions.Contains("heatwave") && !SDVTime.IsNight)
                sickOdds -= .11;

            bool farmerCaughtCold = (Dice.NextDoublePositive() <= sickOdds) && (IllOptions.StaminaDrain > 0);

            FarmHouse fh = Game1.getLocationFromName("FarmHouse") as FarmHouse;
            bool isHeaterHere = false;
            foreach (var v in fh.objects.Pairs)
            {
                if (v.Value.Name.Contains("Heater"))
                {
                    if (IllOptions.Verbose) Monitor.Log("Heater detected");
                    isHeaterHere = true;
                }
            }

            foreach (var v in fh.furniture)
            {
                if (v.furniture_type.Value == Furniture.fireplace && v.IsOn)
                {
                    if (IllOptions.Verbose) Monitor.Log("fireplace detected");
                    isHeaterHere = true;
                }
            }

            if (!(temp is null)) { 
            
                turnTheHeatOn = (turnTheHeatOn || (amtInHouse >= IllOptions.PercentageOutside && farmerCaughtCold &&
                                                   temp < IllOptions.TooColdInside && !isHeaterHere && IssuedInHouseWarning && 
                                                   (Game1.timeOfDay < 1000 || Game1.timeOfDay > 1650)));
            
                              
                if (!IssuedInHouseWarning && amtInHouse >= IllOptions.PercentageOutside && temp < IllOptions.TooColdInside
                    && (Game1.timeOfDay < 1000 || Game1.timeOfDay > 1650) && !Game1.currentLocation.IsOutdoors && !isHeaterHere)
                {
                    SDVUtilities.ShowMessage(Helper.Get("hud-text.desc_HeatOn"),4);
                    IssuedInHouseWarning = true;
                }
            }

            if (amtOutside >= IllOptions.PercentageOutside && farmerCaughtCold || this.FarmerSick || turnTheHeatOn)
            {
                //check if it's a valid condition
                if (FarmerCanGetSick())
                {
                    //rewrite time..
                    if (conditions.Contains("blizzard") || conditions.Contains("sandstorm") || conditions.Contains("whiteout") || (conditions.Contains("lightning") || conditions.Contains("stormy") || conditions.Contains("thundersnow")) && !(Game1.currentLocation is Desert) || (conditions.Contains("frost") && SDVTime.IsNight) || (conditions.Contains("heatwave") && !SDVTime.IsNight) || turnTheHeatOn)
                    {
                        if (turnTheHeatOn && !Game1.currentLocation.IsOutdoors)
                            ReasonSick = IllCauses.TooColdInside;
                        else if ((conditions.Contains("heatwave") && !SDVTime.IsNight))
                            ReasonSick = IllCauses.TooHotOutside;
                        else if (conditions.Contains("frost") && SDVTime.IsNight)
                            ReasonSick = IllCauses.TooColdOutside;
                        else if (condList.Contains("blizzard"))
                            ReasonSick = IllCauses.BlizzardsOutside;
                        else if (condList.Contains("whiteout"))
                            ReasonSick = IllCauses.TheWampaWillGetYou;
                        else if (condList.Contains("sandstorm"))
                            ReasonSick = IllCauses.Darude;
                        else if (conditions.Contains("lightning") || conditions.Contains("stormy"))
                            ReasonSick = IllCauses.InclementWeather;
                        else
                            ReasonSick = IllCauses.NonspecificSevereWeather;    

                        this.MakeSick();
                    }
                }

                //now that we've done that, go through the various conditions
                if (this.FarmerSick && (conditions.Contains("lightning") || conditions.Contains("stormy") || conditions.Contains("thundersnow")))
                {
                    totalMulti += 1;
                    condList.Add("Lightning or Thundersnow");
                }

                if (this.FarmerSick && conditions.Contains("fog"))
                {
                    totalMulti += .5;
                    condList.Add("Fog");
                }

                if (this.FarmerSick && conditions.Contains("fog") && SDVTime.IsNight)
                {
                    totalMulti += .25;
                    condList.Add("Night Fog");
                }

                if (this.FarmerSick && conditions.Contains("blizzard") && !conditions.Contains("whiteout"))
                {
                    totalMulti += 1.25;
                    condList.Add("Blizzard");
                }

                if (this.FarmerSick && conditions.Contains("sandstorm"))
                {
                    totalMulti += 1.25;
                    condList.Add("Sandstorm");
                }

                if (this.FarmerSick && conditions.Contains("blizzard") && conditions.Contains("whiteout"))
                {
                    totalMulti += 2.45;
                    condList.Add("White Out");
                }

                if (this.FarmerSick && conditions.Contains("thunderfrenzy"))
                {
                    totalMulti += 1.85;
                    condList.Add("Thunder Frenzy");
                }

                if (this.FarmerSick && conditions.Contains("frost") && SDVTime.IsNight)
                {
                    totalMulti += 1.25;
                    condList.Add("Night Frost");
                }

                if (this.FarmerSick && turnTheHeatOn)
                {
                    totalMulti += 1;
                    condList.Add("Cold House");
                }

                if (this.FarmerSick && conditions.Contains("thundersnow") && SDVTime.IsNight)
                {
                    totalMulti += .5;
                    condList.Add("Night Thundersnow");
                }

                if (this.FarmerSick && conditions.Contains("blizzard") && SDVTime.IsNight)
                {
                    totalMulti += .5;
                    condList.Add("Night Blizzard");
                }

                if (this.FarmerSick && conditions.Contains("heatwave") && !SDVTime.IsNight)
                {
                    totalMulti += 1.25;
                    condList.Add("Day Heatwave");
                }
            }

            staminaAffect -= (int)Math.Floor(IllOptions.StaminaDrain * totalMulti);

            if (IllOptions.Verbose && this.FarmerSick)
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
                
                Monitor.Log($"[{Game1.timeOfDay}] Conditions for the drain are {condString} for a total multiplier of {totalMulti} for a total drain of {staminaAffect}"); 
            }

            return staminaAffect;
        }

        public int GetBathHouseDuration()
        {
            if (timeInBathHouse == 0)
                return 0;
            Monitor.Log($"Time Of Day: {Game1.timeOfDay}, Time In Bath House: {timeInBathHouse}");
            return SDVTime.MinutesBetweenTwoIntTimes(this.timeInBathHouse, Game1.timeOfDay);
        }

        public void OnUpdateTicked()
        {
            if (this.FogDebuffValid)
            {
                if (WeatherIllnesses.UseClimates && !(WeatherIllnesses.climatesAPI is null) && WeatherIllnesses.climatesAPI.GetCurrentFogType() != "Blinding")
                {
                    Monitor.Log("Clearing the fog",LogLevel.Info);
                    ClearFog();
                }
            }

            if (WeatherIllnesses.UseClimates && !(WeatherIllnesses.climatesAPI is null) && 
                WeatherIllnesses.climatesAPI.GetCurrentFogType() == "Blinding")
            {
                FogDebuffValid = true;
            }

            if (this.IsSick())
            {
                int buffId = UniqueStaID + SicknessBuffID;
                Buff weatherBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
                if (weatherBuff == null)
                {
                    Game1.buffsDisplay.addOtherBuff(
                        weatherBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0,
                            "Weather Illnesses", "Weather Illnesses"));
                    weatherBuff.which = buffId;
                    weatherBuff.sheetIndex = SicknessBuffID;
                    weatherBuff.millisecondsDuration = 0;
                    weatherBuff.description = "You are sick due to the inclement weather!";
                }
            }

            if (this.FogDebuffValid)
            {
                int buffId = UniqueStaID + FogBuffID;
                Buff weatherBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
                if (weatherBuff == null)
                {
                    Game1.buffsDisplay.addOtherBuff(
                        weatherBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            "Weather Illnesses", "Weather Illnesses"));
                    weatherBuff.which = buffId;
                    weatherBuff.sheetIndex = FogBuffID;
                    weatherBuff.millisecondsDuration = 0;
                    weatherBuff.description = "The inclement weather is severely limiting visibility.";
                }

                if (!WeatherIllnesses.UseClimates)
                {
                    //a rather brute force method;
                    Game1.outdoorLight = Color.Black;
                }
            }
        }

        public void PoolhouseFlip()
        {
            if (timeInBathHouse == 0)
                timeInBathHouse = Game1.timeOfDay;
            else
                timeInBathHouse = 0;
        }
    }
}
