/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Linq;
using StardewSurvivalProject.source.utils;
using StardewValley;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace StardewSurvivalProject.source.model
{
    public class EnvTemp
    {
        private static double DEFAULT_VALUE = ModConfig.GetInstance().EnvironmentBaseTemperature;
        public double value { get; set; }
        private Random rand = null;

        public EnvTemp()
        {
            this.value = DEFAULT_VALUE;
            this.rand = new Random();
        }

        public void updateEnvTemp(int time, string season, int weatherIconId, GameLocation location = null, int currentMineLevel = 0)
        {
            
            double BASE_VALUE = DEFAULT_VALUE;
            double value = BASE_VALUE;
            double dayNightCycleTempDiffScale = ModConfig.GetInstance().DefaultDayNightCycleTemperatureDiffScale;
            double fluctuationTempScale = ModConfig.GetInstance().DefaultTemperatureFluctuationScale;
            bool fixedTemp = false;

            //LogHelper.Debug($"season={season} time={time} weatherId={weatherIconId}");

            //start with applying adjustment based on season
            if (season.Equals("spring")) value *= ModConfig.GetInstance().SpringSeasonTemperatureMultiplier;
            else if (season.Equals("fall")) value *= ModConfig.GetInstance().FallSeasonTemperatureMultiplier ;
            else if (season.Equals("summer")) value *= ModConfig.GetInstance().SummerSeasonTemperatureMultiplier;
            else if (season.Equals("winter")) value *= ModConfig.GetInstance().WinterSeasonTemperatureMultiplier;

            //next, check for weather
            switch (weatherIconId)
            {
                case (int)weatherIconType.SUNNY:
                    value *= ModConfig.GetInstance().SunnyWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.FESTIVAL:
                    value *= ModConfig.GetInstance().FestivalWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.WEDDING:
                    value *= ModConfig.GetInstance().WeddingWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.STORM:
                    value *= ModConfig.GetInstance().StormWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.RAIN:
                    value *= ModConfig.GetInstance().RainWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.WINDY_SPRING:
                    value *= ModConfig.GetInstance().WindySpringWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.WINDY_FALL:
                    value *= ModConfig.GetInstance().WindySpringWeatherTemperatureMultiplier; break;
                case (int)weatherIconType.SNOW:
                    value *= ModConfig.GetInstance().SnowWeatherTemperatureMultiplier; break;
                default: break;
            }

            
            //check for location
            if (location != null)
            {
                if (ModConfig.GetInstance().UseCustomLocationTemperatureData)
                {
                    //LogHelper.Debug(location.Name);
                    data.LocationEnvironmentData locationData = data.CustomEnvironmentDictionary.GetEnvironmentData(location.Name);
                    if (locationData != null)
                    {
                        value += locationData.tempModifierAdditive;
                        value *= locationData.tempModifierMultiplicative;
                        if (locationData.tempModifierFixedValue > -273)
                        {
                            value = locationData.tempModifierFixedValue;
                            fixedTemp = true;
                        }
                        dayNightCycleTempDiffScale = locationData.tempModifierTimeDependentScale;
                        fluctuationTempScale = locationData.tempModifierFluctuationScale;
                    }
                }

                //if (!location.IsOutdoors && ModConfig.GetInstance().UseDefaultIndoorTemperatureModifier)
                //{
                //    //cut temperature difference by half if indoor if outside is colder
                //    value += Math.Min((DEFAULT_VALUE - value) / 2, 0);
                //}

                //special treatment for cave
                if (location.Name.Contains("UndergroundMine") && ModConfig.GetInstance().UseDefaultCaveTemperatureModifier)
                {
                    if (currentMineLevel == 77377)
                    {
                        value = DEFAULT_VALUE;
                        fixedTemp = true;
                    }
                    else if (currentMineLevel >= 0 && currentMineLevel < 40)
                    {
                        value = DEFAULT_VALUE + 0.22 * currentMineLevel;
                        fixedTemp = true;
                    }
                    else if (currentMineLevel >= 40 && currentMineLevel < 80)
                    {
                        value = -0.01 * Math.Pow(currentMineLevel - 60, 2) - 6;
                        fixedTemp = true;
                    }
                    else if (currentMineLevel >= 80 && currentMineLevel < 121)
                    {
                        value = 1.1 * Math.Pow(currentMineLevel - 60, 1.05);
                        fixedTemp = true;
                    }
                    else if (currentMineLevel >= 121)
                    {
                        value = DEFAULT_VALUE + 0.045 * currentMineLevel;
                        fixedTemp = true;
                    }
                }
            }

            //next, check for time
            //convert time to actual decimal format to run on a time-dependent function
            double decTime = ((double)(time / 100) + ((double)(time % 100) / 60.0));
            //LogHelper.Debug(decTime.ToString());
            //curve look good enough on desmos so YOLO
            double timeTempModifier = Math.Sin((decTime - 8.5) / (Math.PI * 1.2)) * dayNightCycleTempDiffScale; //TODO change number 3 to a season and location-dependent multiplier
            value += (fixedTemp) ? 0 : timeTempModifier;

            //finally, add some randomness XD
            value += rand.NextDouble() * fluctuationTempScale - 0.5 * fluctuationTempScale;

            if (!location.IsOutdoors)
            {
                if (ModConfig.GetInstance().UseDefaultIndoorTemperatureModifier)
                    //cut temperature difference by half if indoor if outside is colder (assume wall can block heat)
                    value += Math.Min((DEFAULT_VALUE - value) / 2, 0);

                // aggregate a list of weather control item to calculate indoor temperature
                List<data.TempControlObject> tempControlDevices = new List<data.TempControlObject>();

                foreach (SObject obj in location.objects.Values)
                {
                    data.TempControlObject tempControl = data.TempControlObjectDictionary.GetTempControlData(obj.name);
                    if (tempControl != null)
                    {
                        LogHelper.Debug(obj.name);
                        //if this item need to be active
                        if (tempControl.needActive)
                        {
                            if (checkIfItemIsActive(new KeyValuePair<int, SObject>(obj.GetHashCode(), obj), tempControl.activeType) == false) continue;    
                        }
                        tempControlDevices.Add(tempControl);

                    }
                }

                foreach (SObject obj in location.furniture)
                {
                    data.TempControlObject tempControl = data.TempControlObjectDictionary.GetTempControlData(obj.name);
                    if (tempControl != null)
                    {
                        LogHelper.Debug(obj.name);
                        //if this item need to be active
                        if (tempControl.needActive)
                        {
                            if (checkIfItemIsActive(new KeyValuePair<int, SObject>(obj.GetHashCode(), obj), tempControl.activeType) == false) continue;
                        }
                        tempControlDevices.Add(tempControl);

                    }
                }

                LogHelper.Debug(tempControlDevices.Count.ToString());

                int area = location.Map.GetLayer("Back").Tiles.Array.Length;
                LogHelper.Debug("temp = " + value);

                double power = area * value;

                foreach (data.TempControlObject tempControlObj in tempControlDevices)
                {
                    LogHelper.Debug(tempControlObj.name);
                    //calculate indoor heating power base on core temp and range (assume full effectiveness if object is placed indoor)
                    if (tempControlObj.deviceType.Equals("general"))
                    {
                        double perfectAmbientPower = area * DEFAULT_VALUE;
                        double maxPowerFromDevice = tempControlObj.operationalRange * (tempControlObj.effectiveRange * 2 + 1) * (tempControlObj.effectiveRange * 2 + 1) * tempControlObj.ambientCoefficient;
                        if (perfectAmbientPower > power)
                            power = Math.Min(perfectAmbientPower, power + maxPowerFromDevice);
                        else
                            power = Math.Max(perfectAmbientPower, power - maxPowerFromDevice);
                    }
                    else power += (tempControlObj.coreTemp - DEFAULT_VALUE) * (tempControlObj.effectiveRange * 2 + 1) * (tempControlObj.effectiveRange * 2 + 1) * tempControlObj.ambientCoefficient; 

                    
                }

                // modify indoor temp base on area and the heating / cooling power of all temperature control devices

                value = power / area;

                LogHelper.Debug("final temp = " + value);
            }

            this.value = value;
        }

        public void updateLocalEnvTemp(int playerTileX, int playerTileY)
        {

            //FIXME: approach can be improved
            //TODO: change to instead keeping track of a list of heating / cooling source (initialize on loading save file)

            //check in player proximity for any object (AxA tile square around player position
            //should change based on the biggest effectiveRange entry)


            int proximityCheckBound = (int)Math.Ceiling(data.TempControlObjectDictionary.maxEffectiveRange); 
            Dictionary<int, SObject> nearbyObject = new Dictionary<int, SObject>();

            for (int i = playerTileX - proximityCheckBound; i <= playerTileX + proximityCheckBound; i++)
            {
                for (int j = playerTileY - proximityCheckBound; j <= proximityCheckBound + proximityCheckBound; j++)
                {
                    SObject obj = Game1.currentLocation.getObjectAtTile(i, j);
                    if (obj != null && !nearbyObject.ContainsKey(obj.GetHashCode()))
                    {
                        //LogHelper.Debug($"there is a {obj.name} nearby");
                        nearbyObject.Add(obj.GetHashCode(), obj);
                    }
                }
            }
            //filter object as heating source and cooling source

            double oldVal = value;

            //LinkedList<double> tempModifier = new LinkedList<double>();

            foreach (KeyValuePair<int, SObject> o in nearbyObject)
            {
                data.TempControlObject tempControl = data.TempControlObjectDictionary.GetTempControlData(o.Value.name);
                if (tempControl != null)
                {
                    //if this item need to be active
                    if (tempControl.needActive)
                    {
                        if (!checkIfItemIsActive(o, tempControl.activeType))
                            continue;                        
                    }

                    //prioritize ambient temp if it exceed device's core temp
                    if ((tempControl.deviceType.Equals("heating") && tempControl.coreTemp < value) || (tempControl.deviceType.Equals("cooling") && tempControl.coreTemp > value)) continue;

                    //dealing with target temp value here?
                    double distance_sqr = distance_square(o.Value.TileLocation.X, o.Value.TileLocation.Y, playerTileX, playerTileY);
                    //LogHelper.Debug($"Distance square from player to {o.Key} is {distance_sqr}");

                    double effRange = tempControl.effectiveRange;
                    if (distance_sqr <= effRange * effRange)
                    {
                        double tempModifierEntry = (tempControl.coreTemp - this.value) * (1 / (1 + distance_sqr));
                        value += tempModifierEntry;
                    }
                }

            }
            LogHelper.Debug($"Final temperature modifier is {value - oldVal}");

        }

        private double distance_square(double aX, double aY, double bX, double bY)
        {
            return (aX - bX) * (aX - bX) + (aY - bY) * (aY - bY);
        }

        private bool checkIfItemIsActive(KeyValuePair<int, SObject> o, int checkType = 0)
        {
            //check if the object checking is a big craftable craftable
            if (checkType == 1)
            {
                //check if said big craftable is being used
                if (o.Value.MinutesUntilReady >= 0 && o.Value.heldObject.Value != null)
                {
                    //LogHelper.Debug($"there is an active {o.Value.name} nearby (machine)");
                    return true;
                }
                else
                {
                    //LogHelper.Debug($"there is an inactive {o.Value.name} nearby (machine)");
                    return false;
                }
            }
            else
            {
                //if not big craftable (assuming furniture), check if said furniture is active
                if (o.Value.IsOn)
                {
                    //LogHelper.Debug($"there is an active {o.Value.name} nearby");
                    return true;
                }
                else
                {
                    //LogHelper.Debug($"there is an inactive {o.Value.name} nearby");
                    return false;
                }
            }
        }
    }
}
