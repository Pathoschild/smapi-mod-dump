/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FletcherGoss/FQTweaks
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace FQFestivalTweaks
{
    public class ModEntry : Mod
    {
        private ModConfig _config;

        private IDictionary<string, int[]> _festivalTimes;

        private int _attendedTime;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLoopOnGameLaunched;
            helper.Events.GameLoop.TimeChanged += GameLoopOnTimeChanged;
            helper.Events.Player.Warped += PlayerOnWarped;
        }

        private void GameLoopOnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this._config = this.Helper.ReadConfig<ModConfig>();
            this._festivalTimes = new Dictionary<string, int[]>();
            this._attendedTime = -1;

            IDictionary<string, string> festivalDates =
                this.Helper.Content.Load<IDictionary<string, string>>("Data/Festivals/FestivalDates",
                    ContentSource.GameContent);

            foreach (string festivalDate in festivalDates.Keys)
            {
                IDictionary<string, string> festivalInfo =
                    this.Helper.Content.Load<IDictionary<string, string>>($"Data/Festivals/{festivalDate}",
                        ContentSource.GameContent);

                string[] times = festivalInfo["conditions"].Split('/')[1].Split(' ');

                this._festivalTimes.Add(festivalDate, Array.ConvertAll(times, int.Parse));
                this.Monitor.Log($"Added festival: {festivalDate}:\t{times[0].PadLeft(4)} - {times[1].PadLeft(4)}",
                    LogLevel.Trace);
            }
        }

        private void GameLoopOnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            {
                return;
            }

            if (this._attendedTime < 0)
            {
                return;
            }

            int[] times = this._festivalTimes[$"{Game1.currentSeason}{Game1.dayOfMonth}"];

            int startTime = times[0];
            int endTime = times[1];

            this.Monitor.Log($"Default end time: {endTime.ToString().PadLeft(4)}", LogLevel.Trace);

            if (this._config.MinMinutesAtFestival > 0)
            {
                int minMinutesAtFestival = this._config.MinMinutesAtFestival;
                int festivalLengthMinutes = Utility.CalculateMinutesBetweenTimes(startTime, endTime);

                // If the festival length is less than MinMinutesAtFestival, it shouldn't take longer.
                if (festivalLengthMinutes < minMinutesAtFestival)
                {
                    minMinutesAtFestival = festivalLengthMinutes;
                }

                int attendedMinutes = Utility.CalculateMinutesBetweenTimes(this._attendedTime, endTime);

                if (attendedMinutes < minMinutesAtFestival)
                {
                    this.Monitor.Log($"Spent less than {minMinutesAtFestival} minutes at festival", LogLevel.Trace);
                    endTime = this._attendedTime + Utility.ConvertMinutesToTime(minMinutesAtFestival);
                    this.Monitor.Log($"Changed end time: {endTime.ToString().PadLeft(4)}", LogLevel.Trace);
                }

                if (endTime > this._config.MaxEndTime)
                {
                    this.Monitor.Log($"Cannot end later than {this._config.MaxEndTime}", LogLevel.Trace);
                    endTime = this._config.MaxEndTime;
                    this.Monitor.Log($"Changed end time: {endTime.ToString().PadLeft(4)}", LogLevel.Trace);
                }
            }

            // Source: https://github.com/CJBok/SDV-Mods/blob/master/CJBCheatsMenu/Framework/Cheats/Time/SetTimeCheat.cs
            int intervals = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, endTime) / 10;

            if (intervals > 0)
            {
                for (int i = 0; i < intervals; i++)
                {
                    Game1.performTenMinuteClockUpdate();
                }
            }
            else if (intervals < 0)
            {
                for (int i = 0; i > intervals; i--)
                {
                    Game1.timeOfDay = Utility.ModifyTime(Game1.timeOfDay, -20);
                    Game1.performTenMinuteClockUpdate();
                }
            }

            Game1.outdoorLight = Color.White;
            Game1.ambientLight = Color.White;

            Game1.gameTimeInterval = 0;
            Game1.UpdateGameClock(Game1.currentGameTime);

            this._attendedTime = -1;
            this.Monitor.Log($"Festival ended: {Game1.timeOfDay.ToString().PadLeft(4)}", LogLevel.Trace);
        }

        private void PlayerOnWarped(object sender, WarpedEventArgs e)
        {
            if (Game1.isFestival())
            {
                this._attendedTime = Game1.timeOfDay;
                this.Monitor.Log($"Festival started: {Game1.timeOfDay.ToString().PadLeft(4)}", LogLevel.Trace);
            }
        }
    }
}