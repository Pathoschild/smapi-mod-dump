/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace DailyScreenshot
{

    /// <summary>
    /// User specified Triggers.  Use with caution, data is 
    /// not validated during construction.  Call 
    /// ValidateUserInput before using values
    /// </summary>
    public class ModTrigger
    {
        /// <summary>
        /// Trace messages to the console
        /// </summary>
        /// <param name="message">Text to display</param>
        void MTrace(string message) => ModEntry.g_dailySS.MTrace(message);

        /// <summary>
        /// Helper function for sending trace messages
        /// </summary>
        /// <param name="message">text to send</param>
        internal void MDebug(string message) => ModEntry.g_dailySS.MDebug(message);

        /// <summary>
        /// Warning messages to the console
        /// </summary>
        /// <param name="message">Text to display</param>
        void MWarn(string message) => ModEntry.g_dailySS.MWarn(message);

        /// <summary>
        /// Convert the current date to DateFlags
        /// </summary>
        /// <returns>The current date as DateFlags</returns>
        private static DateFlags GetDate() =>
            (DateFlags)((1 << (Game1.Date.SeasonIndex + 28)) | (1 << (Game1.Date.DayOfMonth - 1)));

        /// <summary>
        /// Minium time span the game allows (moves in 10 minute increments)
        /// </summary>
        private const int MIN_TIME_INTERVAL = 10;
        private const int MINUTES_PER_HOUR = 60;
        private const int MINUTES_TO_ADD_FOR_NEXT_HOUR = 40;

        /// <summary>
        /// True if this rule has triggered automatically
        /// </summary>
        private bool m_triggered = false;

        #region Dates 
        /// <summary>
        /// Days of the season specifiers and season specifers
        /// </summary>
        [Flags]
        public enum DateFlags
        {
            Day_None = 0,
            Day_01 = 1 << 0,
            Day_1 = Day_01,
            Day_02 = 1 << 1,
            Day_2 = Day_02,
            Day_03 = 1 << 2,
            Day_3 = Day_03,
            Day_04 = 1 << 3,
            Day_4 = Day_04,
            Day_05 = 1 << 4,
            Day_5 = Day_05,
            Day_06 = 1 << 5,
            Day_6 = Day_06,
            Day_07 = 1 << 6,
            Day_7 = Day_07,
            Day_08 = 1 << 7,
            Day_8 = Day_08,
            Day_09 = 1 << 8,
            Day_9 = Day_09,
            Day_10 = 1 << 9,
            Day_11 = 1 << 10,
            Day_12 = 1 << 11,
            Day_13 = 1 << 12,
            Day_14 = 1 << 13,
            Day_15 = 1 << 14,
            Day_16 = 1 << 15,
            Day_17 = 1 << 16,
            Day_18 = 1 << 17,
            Day_19 = 1 << 18,
            Day_20 = 1 << 19,
            Day_21 = 1 << 20,
            Day_22 = 1 << 21,
            Day_23 = 1 << 22,
            Day_24 = 1 << 23,
            Day_25 = 1 << 24,
            Day_26 = 1 << 25,
            Day_27 = 1 << 26,
            Day_28 = 1 << 27,
            Spring = 1 << 28,
            Summer = 1 << 29,
            Fall = 1 << 30,
            Winter = 1 << 31,
            AnyDay = Mondays | Tuesdays | Wednesdays | Thursdays | Fridays | Saturdays | Sundays,
            AnySeason = Spring | Summer | Fall | Winter,
            Mondays = Day_01 | Day_08 | Day_15 | Day_22,
            Tuesdays = Mondays << 1,
            Wednesdays = Mondays << 2,
            Thursdays = Mondays << 3,
            Fridays = Mondays << 4,
            Saturdays = Mondays << 5,
            Sundays = Mondays << 6,
            FirstDayOfTheMonth = Day_01,
            LastDayOfTheMonth = Day_28,
            Daily = AnyDay | AnySeason
        }

        /// <summary>
        /// Days to take screenshots
        /// Note: Enum value, no need to validate
        /// </summary>
        /// <value>User specified days to take screenshots</value>
        public DateFlags Days { get; set; } = DateFlags.Daily;
        #endregion

        #region Weather
        /// <summary>
        /// Weather conditions
        /// </summary>
        [Flags]
        public enum WeatherFlags
        {
            Weather_None = 0,
            Sunny = 1 << 0,
            Rainy = 1 << 1,
            Windy = 1 << 2,
            Stormy = 1 << 3,
            Snowy = 1 << 5,
            Any = Sunny | Rainy | Windy | Stormy | Snowy,
        }

        /// <summary>
        /// Weather for screenshot.
        /// Note: Enum value, no need to validate
        /// </summary>
        /// <value>User specified weather for screenshot</value>
        public WeatherFlags Weather { get; set; } = WeatherFlags.Any;

        /// <summary>
        /// Finds the current weather
        /// </summary>
        /// <returns>WeatherFlags of the current weather</returns>
        public WeatherFlags GetWeather()
        {
            if (Game1.isSnowing)
                return WeatherFlags.Snowy;
            if (Game1.isLightning)
                return WeatherFlags.Stormy;
            if (Game1.isRaining)
                return WeatherFlags.Rainy;
            if (Game1.isDebrisWeather)
                return WeatherFlags.Windy;
            return WeatherFlags.Sunny;
        }
        #endregion

        #region Locations
        /// <summary>
        /// Location for screenshot
        /// Any means take anywhere
        /// </summary>
        [Flags]
        public enum LocationFlags
        {
            Location_None = 0,
            Farm = 1 << 0,
            GreenHouse = 1 << 2,
            Farmhouse = 1 << 3,
            Beach = 1 << 4,
            FarmCave = 1 << 5,
            Cellar = 1 << 6,
            Desert = 1 << 7,
            Museum = 1 << 8,
            CommunityCenter = 1 << 9,
            Town = 1 << 10,
            Mountain = 1 << 11,
            Mine = 1 << 12,
            MineShaft = 1 << 13,
            IslandWest = 1 << 14,
            IslandFarmhouse = 1 << 15,
            IslandFieldOffice = 1 << 16,
            Unknown = 1 << 24,
            Any = Farm | GreenHouse | Farmhouse | Beach | Unknown |
                Mountain | CommunityCenter | Town | Museum | FarmCave | Cellar | Desert | 
                Mine | MineShaft | 
                IslandWest | IslandFarmhouse | IslandFieldOffice,
        }

        /// <summary>
        /// Checks if a rule can trigger
        /// </summary>
        /// <param name="ruleName">Name of the rule for warnings</param>
        /// <returns>True if the rule can trigger</returns>
        internal bool CanTrigger(string ruleName)
        {
            bool ret = true;
            if (DateFlags.Day_None == (Days & DateFlags.AnySeason))
            {
                MWarn($"Rule {ruleName} will not trigger as no season is set.  Days=\"{Days}\"");
                ret = false;
            }
            if (DateFlags.Day_None == (Days & DateFlags.AnyDay))
            {
                MWarn($"Rule {ruleName} will not trigger as no day is set.  Days=\"{Days}\"");
                ret = false;
            }
            if (WeatherFlags.Weather_None == (Weather & WeatherFlags.Any))
            {
                MWarn($"Rule {ruleName} will not trigger as no weather is set.  Weather=\"{Weather}\"");
                ret = false;
            }
            if (LocationFlags.Location_None == (Location & LocationFlags.Any))
            {
                MWarn($"Rule {ruleName} will not trigger as no location is set.  Location=\"{Location}\"");
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Checks if the specified times are valid and fixes any user mistakes
        /// </summary>
        /// <param name="ruleName">Name of the rule for warnings</param>
        /// <returns>True if the times were modified</returns>
        internal bool ValidateUserInput(string ruleName)
        {
            bool modified = false;
            int startTime = SetLimits(StartTime);
            int endTime = SetLimits(EndTime);
            if (StartTime != startTime || EndTime != endTime || startTime > endTime)
            {
                modified = true;

                if (StartTime != startTime || EndTime != endTime)
                    MWarn($"Setting limits on StartTime and EndTime for rule \"{ruleName}\"");

                // if the times are impossible then swap them
                if (startTime > endTime)
                {
                    MWarn($"Swapping StartTime and EndTime for rule \"{ruleName}\"");
                    StartTime = endTime;
                    EndTime = startTime;
                }
                else
                {
                    StartTime = startTime;
                    EndTime = endTime;
                }
                MWarn($"StartTime is now \"{StartTime}\" amd EmdTime is now \"{EndTime}\" for rule \"{ruleName}\"");
            }

            return modified;
        }

        /// <summary>
        /// Makes sure the time is set to be a multiple of 10 and
        /// between ModConfig.DEFAULT_START_TIME and 
        /// ModConfig.DEFAULT_END_TIME
        /// </summary>
        /// <param name="time">User specified time</param>
        /// <returns>Fixed up time rounded to the nearest 10 minutes</returns>
        private int SetLimits(int time)
        {
            // round to the nearest 10 minutes
            int val = Math.Max(time, ModConfig.DEFAULT_START_TIME); ;
            if (MINUTES_PER_HOUR <= (val % 100))
            {
                val += MINUTES_TO_ADD_FOR_NEXT_HOUR;
            }
            val = Math.Min(val + (MIN_TIME_INTERVAL / 2), ModConfig.DEFAULT_END_TIME);
            // Round to the nearest 10 mintues
            return val - (val % MIN_TIME_INTERVAL);
        }

        /// <summary>
        /// Location for screenshot.
        /// Note: Enum value, no need to validate
        /// </summary>
        /// <value>User specified location for screenshot</value>
        public LocationFlags Location { get; set; } = LocationFlags.Farm;

        /// <summary>
        /// Finds the user's current location
        /// </summary>
        /// <returns>LocationFlags enum of the location</returns>
        public static LocationFlags GetLocation()
        {
            GameLocation location = Game1.currentLocation;
            return location switch
            {
                Farm => LocationFlags.Farm,
                FarmHouse => LocationFlags.Farmhouse,
                Beach => LocationFlags.Beach,
                FarmCave => LocationFlags.FarmCave,
                Cellar => LocationFlags.Cellar,
                Desert => LocationFlags.Desert,
                LibraryMuseum => LocationFlags.Museum,
                CommunityCenter => LocationFlags.CommunityCenter,
                Town => LocationFlags.Town,
                Mountain => LocationFlags.Mountain,
                Mine => LocationFlags.Mine,
                MineShaft => LocationFlags.MineShaft,
                IslandWest => LocationFlags.IslandWest,
                IslandFarmHouse => LocationFlags.IslandFarmhouse,
                IslandFieldOffice => LocationFlags.IslandFieldOffice,
                _ => location.IsGreenhouse ? LocationFlags.GreenHouse : LocationFlags.Unknown,
            };
        }
        #endregion

        #region Keyboard
        /// <summary>
        /// Key for screenshot
        /// Note: Enum value, no need to validate
        /// </summary>
        /// <value>User specified key for a screenshot</value>
        public SButton Key { get; set; } = SButton.None;
        #endregion

        /// <summary>
        /// Start of the time frame to take screenshot
        /// Note: must validate
        /// </summary>
        /// <value>Value between ModConfig.DEFAULT_START_TIME and ModConfig.DEFAULT_END_TIME divisible by 10</value>
        public int StartTime { get; set; } = ModConfig.DEFAULT_START_TIME;

        /// <summary>
        /// End of the time frame to take screenshot
        /// Note: must validate
        /// </summary>
        /// <value>Value between ModConfig.DEFAULT_START_TIME and ModConfig.DEFAULT_END_TIME divisible by 10</value>
        public int EndTime { get; set; } = ModConfig.DEFAULT_END_TIME;

        /// <summary>
        /// Helper function to see if a time is contained by StartTime and EndTime
        /// </summary>
        /// <param name="time">Game time of day</param>
        /// <returns>true if the time is contained</returns>
        internal bool CheckTime(int time)
        {
            //MTrace($"Is {time} <= {EndTime} and {time} >= {StartTime} ");
            return time >= StartTime && time <= EndTime;
        }

        /// <summary>
        /// Check if this trigger is still valid
        /// </summary>
        /// <returns>True if the trigger can still fire today</returns>
        internal bool CanTriggerToday()
        {
            return m_triggered == false &&
                DateFlags.Day_None != (GetDate() & Days) &&
                Game1.timeOfDay <= EndTime;
        }

        /// <summary>
        /// Is the trigger waiting on a warp?
        /// </summary>
        /// <returns>True if caller should be waiting on the warp event</returns>
        internal bool IsWaitingOnWarp()
        {
            return LocationFlags.Location_None == (GetLocation() & Location) &&
                CanTriggerToday();
        }

        /// <summary>
        /// Is the trigger waiting on a time?
        /// </summary>
        /// <returns>True if caller should be waiting on the time event</returns>
        internal bool IsWaitingOnTime()
        {
            return Game1.timeOfDay < StartTime &&
                CanTriggerToday();
        }

        /// <summary>
        /// Is the trigger waiting on a keypress?
        /// </summary>
        /// <returns>True if the caller should be waiting on the keypress event</returns>
        internal bool IsWaitingOnKeypress()
        {
            return SButton.None != Key && CanTriggerToday();
        }

        /// <summary>
        /// Resets the triggered flag, should be called at the start of the day
        /// </summary>
        public void ResetTrigger()
        {
#if DEBUG
            MTrace("Trigger reset");
#endif
            m_triggered = false;
        }

        /// <summary>
        /// Function for cheking if the trigger should cause a screenshot
        /// </summary>
        /// <param name="key">Optional, used for keypress</param>
        /// <returns>True if the rule should trigger</returns>
        public bool CheckTrigger(SButton key = SButton.None)
        {
#if DEBUG
            MTrace($"m_triggered = {m_triggered}");
#endif
            if (m_triggered)
                return false;
            DateFlags current_date = GetDate();
            WeatherFlags current_weather = GetWeather();
            LocationFlags current_location = GetLocation();
            if (current_weather != (current_weather & Weather))
                return false;
            if (current_location != (current_location & Location))
                return false;
            if (current_date != (current_date & Days))
            {
                // Trigger will never be valid for this day,
                // wait for the next to reset.
                // Some mods can mess with weather
                m_triggered = true;
                return false;
            }

            // Some mods can mess with time so don't set triggered flag
            if (!CheckTime(Game1.timeOfDay))
                return false;

            // Keys is not a flags enum, only one can be set at a time
            if (Key != key)
                return false;


            // If it is button based, allow another screenshot after this one for this day
            if (SButton.None == key)
                m_triggered = true;

            return true;
        }

    }
}
