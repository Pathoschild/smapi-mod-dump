/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ProducerFrameworkMod.Controllers;
using ProducerFrameworkMod.Utils;
using StardewValley;

namespace ProducerFrameworkMod.ContentPack
{
    public class ProducerConfig
    {
        public string ModUniqueID;
        public List<string> OverrideMod = new List<string>();
        public string ProducerName;
        public bool AlternateFrameProducing;
        public bool AlternateFrameWhenReady;
        public bool DisableBouncingAnimationWhileWorking;
        public NoInputStartMode? NoInputStartMode;
        public Dictionary<StardewStats, string> IncrementStatsOnOutput;
        public bool MultipleStatsIncrement;
        public LightSourceConfig LightSource;
        public WorkingTime WorkingTime;
        public List<Weather> WorkingWeather;
        public List<string> WorkingLocation;
        public bool? WorkingOutdoors;
        public List<string> WorkingSeason;
        public Animation ProducingAnimation;
        public Animation ReadyAnimation;

        public ProducerConfig()
        {
            IncrementStatsOnOutput = new Dictionary<StardewStats, string>();
        }

        public ProducerConfig(string producerName, bool alternateFrameProducing = false, bool alternateFrameWhenReady = false) :
            this(producerName, new Dictionary<StardewStats, string>(), alternateFrameProducing, alternateFrameWhenReady) {}

        public ProducerConfig(string producerName, Dictionary<StardewStats, string> incrementStatsOnOutput, bool alternateFrameProducing = false, bool alternateFrameWhenReady = false, bool multipleStatsIncrement = false) : this()
        {
            ProducerName = producerName;
            AlternateFrameProducing = alternateFrameProducing;
            AlternateFrameWhenReady = alternateFrameWhenReady;
            IncrementStatsOnOutput = incrementStatsOnOutput ?? IncrementStatsOnOutput;
            MultipleStatsIncrement = multipleStatsIncrement;
        }

        public bool CheckWeatherCondition()
        {
            return WorkingWeather == null || WorkingWeather.Any(s => s == GameUtils.GetCurrentWeather());
        }

        public bool CheckLocationCondition(GameLocation location)
        {
            return (WorkingLocation == null || WorkingLocation.Any(l => l == location.Name))
                   && (WorkingOutdoors == null || location.IsOutdoors == WorkingOutdoors.Value);
        }
        
        public bool CheckSeasonCondition(GameLocation location)
        {
            return WorkingSeason == null || WorkingSeason.Any(s => s == location.GetSeasonForLocation());
        }

        public bool CheckCurrentTimeCondition()
        {
            if (WorkingTime != null)
            {
                if (WorkingTime.Begin <= WorkingTime.End)
                {
                    if (Game1.timeOfDay < WorkingTime.Begin || Game1.timeOfDay >= WorkingTime.End)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Game1.timeOfDay >= WorkingTime.End && Game1.timeOfDay < WorkingTime.Begin)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CheckElapsedTimeCondition(ref int minutes)
        {
            if (WorkingTime != null)
            {
                int finishTime;
                int startTime;
                if (Game1.newDay)
                {
                    //If the new day flag is set, game time was not increased before calling the method, so previous time is the time of the day and minutes were calculate until 6am.
                    startTime = Game1.timeOfDay;
                    finishTime = 600;
                }
                else
                {
                    finishTime = Game1.timeOfDay;
                    int previousMinutes = finishTime % 100 - minutes % 60 >= 0 ? minutes % 60 : minutes % 60 + 40;
                    startTime = (finishTime - (minutes / 60) * 100 - previousMinutes) % 2400;
                    startTime = startTime < 0 ? 2400 - startTime : startTime;
                }

                //Each working period is calculated separately 
                List<Tuple<int, int>> workingInterval = new List<Tuple<int, int>>();
                if (WorkingTime.End >= WorkingTime.Begin)
                {
                    workingInterval.Add(new Tuple<int, int>(WorkingTime.Begin, WorkingTime.End));
                }
                else
                {
                    workingInterval.Add(new Tuple<int, int>(0, WorkingTime.End));
                    workingInterval.Add(new Tuple<int, int>(WorkingTime.Begin, 2400));
                }

                //Each day is calculated separately 
                int daysElapsed = (Utility.ConvertTimeToMinutes(startTime) + minutes) / 1600;
                //Resetting minutes to start calculation
                minutes = 0;
                for (int i = 0; i <= daysElapsed; i++)
                {
                    //First day start from start time, other from 000.
                    int begin = i == 0 ? startTime : 000;
                    //Last day end at finish time, other at 2400
                    int end = i == daysElapsed ? finishTime : 2400;
                    foreach (var tuple in workingInterval)
                    {
                        int workTimeStart = Math.Min(Math.Max(begin, tuple.Item1), tuple.Item2);
                        int workTimeFinish = Math.Min(Math.Max(end, tuple.Item1), tuple.Item2);
                        minutes += Utility.CalculateMinutesBetweenTimes(workTimeStart, workTimeFinish);
                        // Minutes from 200 to 600 are worth 50% more, this extra minutes are added here.
                        int extraMinutes = (int)(Utility.CalculateMinutesBetweenTimes(
                                                      Math.Min(600, Math.Max(200, workTimeStart)),
                                                      Math.Min(600, Math.Max(200, workTimeFinish))) / 1.5);
                        minutes += extraMinutes;
                    }
                }
                return minutes > 0;
            }
            return true;
        }

        public void IncrementStats(Item output)
        {
            foreach (KeyValuePair<StardewStats, string> keyValuePair in this.IncrementStatsOnOutput)
            {
                if (keyValuePair.Value == null
                    || keyValuePair.Value == output.Name
                    || keyValuePair.Value == output.ParentSheetIndex.ToString()
                    || keyValuePair.Value == output.Category.ToString()
                    || output.HasContextTag(keyValuePair.Value))
                {
                    StatsController.IncrementStardewStats(keyValuePair.Key, output.Stack);
                    if (!this.MultipleStatsIncrement)
                    {
                        break;
                    }
                }
            }
        }
    }
}
