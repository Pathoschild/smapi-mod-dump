/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FishInfo
**
*************************************************/

using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FishInfo
{
    internal class FishData
    {
        internal List<string> CaughtIn;
        internal List<TimePair> CatchingTimes;
        internal Weather weather;
        internal bool IsCrabPot;
        internal Season season;
        internal string InfoLocation;
        internal string InfoSeason;
        internal string InfoWeather;
        internal string InfoTime;
        internal string FishName;

        public FishData()
        {
            CaughtIn = new List<string>();
            CatchingTimes = new List<TimePair>();
            weather = Weather.None;
            IsCrabPot = false;
            season = Season.None;
            FishName = "";
        }

        internal void AddLocation(string location)
        {
            //string ToAdd = Regex.Replace(char.ToUpper(location[0]) + location.Substring(1), "([A-Z0-9]+)", " $1").Trim();
            if (!CaughtIn.Contains(location))
            {
                CaughtIn.Add(location);
                CreateLocationString();
            }
        }
        internal void AddTimes(int StartTime, int EndTime)
        {
            TimePair times = new TimePair(StartTime, EndTime);
            if (!CatchingTimes.Contains(times))
            {
                CatchingTimes.Add(times);
                CreateTimeString();
            }
        }
        internal void AddWeather(Weather weather)
        {
            if (!this.weather.HasFlag(weather))
            {
                this.weather |= weather;
                CreateWeatherString();
            }
        }
        internal void AddSeason(Season season)
        {
            if (!this.season.HasFlag(season))
            {
                this.season |= season;
                CreateSeasonString();
            }
        }
        internal void SetCrabPot(bool IsCrabPot)
        {
            this.IsCrabPot = IsCrabPot;
        }

        private string CalcEachTimeString(int time)
        {
            if (time == 1200)
            {
                return Translation.GetString("time.midday");
            }
            else if (time == 2400)
            {
                return Translation.GetString("time.midnight");
            }
            else if (time < 1200)
            {
                return FormatTime(time) + Translation.BaseGameTranslation("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370");
            }
            else if (time < 2400)
            {
                return FormatTime(time - 1200) + Translation.BaseGameTranslation("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371");
            }
            else
            {
                return FormatTime(time - 2400) + Translation.BaseGameTranslation("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370");
            }
        }

        private string FormatTime(int time)
        {
            string sTime = time.ToString();
            
            if (sTime.Length == 3)
            {
                sTime = sTime.Insert(1, ":");
            }else if(sTime.Length == 1)
            {
                sTime = $"0{sTime}:00";
            }
            else
            {
                sTime = sTime.Insert(2, ":");
            }

            return sTime;
        }

        private string CalcTimeString()
        {
            if (CatchingTimes.Count == 1 && CatchingTimes[0].StartTime == 600 && CatchingTimes[0].EndTime == 2600)
            {
                return Translation.GetString("time.allday");
            }
            List<string> strings = new List<string>();
            foreach (TimePair times in CatchingTimes)
            {
                strings.Add($"{CalcEachTimeString(times.StartTime)} - {CalcEachTimeString(times.EndTime)}");
            }
            return strings.Join();
        }

        internal void CreateLocationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();

            if (IsCrabPot)
            {
                sb.AppendLine(
                    Translation.GetString(
                        "location.crabpot",
                        new
                        {
                            location = CaughtIn.Join(new Func<string, string>(Translation.GetLocationName))
                        }
                    )
                );
            }
            else
            {
                sb.AppendLine($"{Translation.GetString("location.prefix")}:");
                sb.Append("  ");

                if (CaughtIn.Count == 0)
                {
                    sb.AppendLine(Translation.GetString("location.none"));
                }
                else
                {
                    sb.AppendLine(Game1.parseText(CaughtIn.Join(new Func<string, string>(Translation.GetLocationName)), Game1.smallFont, 256));
                }

            }

            InfoLocation = sb.ToString();
        }

        internal void CreateWeatherString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(Game1.parseText($"{Translation.GetString("weather.prefix")}:", Game1.smallFont, 256));
            sb.AppendLine(Game1.parseText($"  {Translation.GetString(weather)}", Game1.smallFont, 256));

            InfoWeather = sb.ToString();
        }

        internal void CreateTimeString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(Game1.parseText($"{Translation.GetString("time.prefix")}:", Game1.smallFont, 256));
            sb.AppendLine(Game1.parseText($"  {CalcTimeString()}", Game1.smallFont, 256));

            InfoTime = sb.ToString();
        }

        internal void CreateSeasonString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(Game1.parseText($"{Translation.GetString("season.prefix")}:", Game1.smallFont, 256));

            sb.AppendLine(Game1.parseText($"  {Translation.GetString(season)}", Game1.smallFont, 256));

            InfoSeason = sb.ToString();
        }
        
        public override string ToString()
        {

            if (InfoLocation is null || InfoLocation == string.Empty)
            {
                CreateLocationString();
            }
            if((InfoSeason is null || InfoSeason == string.Empty) && !IsCrabPot)
            {
                CreateSeasonString();
            }
            if((InfoTime is null || InfoTime == string.Empty) && !IsCrabPot)
            {
                CreateTimeString();
            }
            if((InfoWeather is null || InfoWeather == string.Empty) && !IsCrabPot)
            {
                CreateWeatherString();
            }

            return FishName + Environment.NewLine + InfoLocation + InfoSeason + InfoTime + InfoWeather;
        }
    }
}
