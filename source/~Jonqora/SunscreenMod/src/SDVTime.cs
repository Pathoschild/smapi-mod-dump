using StardewValley;
using System;

//THIS CODE BASE KINDLY PROVIDED BY SAKORONA :)
//https://github.com/Sakorona/SDVMods/blob/develop/TwilightCoreShared/Stardew%20Valley/SDVTime.cs

namespace SunscreenMod
{
    /// <summary>Valid time periods: Morning, Afternoon, Evening, Night, LateNight, Noon, Midnight.</summary>
    public enum SDVTimePeriods
    {
        /// <summary>Before 1200</summary>
        Morning,
        /// <summary>After 1200 and before it starts to get dark</summary>
        Afternoon,
        /// <summary>When it starts to get dark until 30m after sunset</summary>
        Evening,
        /// <summary>From 30m after sunset until 2300</summary>
        Night,
        /// <summary>After 2300 but not including 2400 exactly</summary>
        LateNight,
        /// <summary>Exactly 1200</summary>
        Noon,
        /// <summary>Exactly 2400</summary>
        Midnight
    }

    ///<summary>Represents a time value in Stardew Valley</summary>
    public class SDVTime
    {
        /// <summary>Is it currently nighttime</summary>
        public static bool IsNight => Game1.isDarkOut();

        ///<summary>Get current time of day period (Morning, Afternoon, etc)</summary>
        public static SDVTimePeriods CurrentTimePeriod => CurrentTime.TimePeriod;

        ///<summary>Get the corresponding time of day period for this SDVTime instance</summary>
        public SDVTimePeriods TimePeriod
        {
            get
            {
                if (this.ReturnIntTime() < 1200)
                {
                    return SDVTimePeriods.Morning;
                }
                if (this.ReturnIntTime() == 1200)
                {
                    return SDVTimePeriods.Noon;
                }
                if (this.ReturnIntTime() > 1200 && this.ReturnIntTime() < Game1.getStartingToGetDarkTime())
                {
                    return SDVTimePeriods.Afternoon;
                }
                if (this.ReturnIntTime() >= Game1.getStartingToGetDarkTime() &&
                    this.ReturnIntTime() < (Game1.getTrulyDarkTime() + 30))
                {
                    return SDVTimePeriods.Evening;
                }
                if (this.ReturnIntTime() >= (Game1.getTrulyDarkTime() + 30) &&
                    this.ReturnIntTime() < 2300)
                {
                    return SDVTimePeriods.Night;
                }
                if (this.ReturnIntTime() == 2400)
                {
                    return SDVTimePeriods.Midnight;
                }
                else
                {
                    return SDVTimePeriods.LateNight;
                }
            }
        }

        ///<summary>Get the current time of day</summary>
        public static SDVTime CurrentTime => new SDVTime(Game1.timeOfDay);

        ///<summary>Get the current time of day as an integer</summary>
        public static int CurrentIntTime => new SDVTime(Game1.timeOfDay).ReturnIntTime();

        ///<summary>Maximum limit for hours in a valid SDVTime</summary>
        public const int MAXHOUR = 28; //Should I change this?
        ///<summary>Minutes per hour when calculating time</summary>
        public const int MINPERHR = 60;

        int hour;
        int minute;

        ///<summary>Constructs an instance using integer time</summary>
        public SDVTime(int t)
        {
            hour = t / 100;

            if (hour < 0)
                throw new ArgumentException("You cannot have a time value of less than 0 seconds");

            if (hour > MAXHOUR)
            {
                hour = MAXHOUR;
                minute = 0;
            }
            else
            {

                t -= (hour * 100);

                if (t < MINPERHR)
                    minute = t;
                else
                {
                    hour++;
                    if (hour > MAXHOUR)
                    {
                        hour = MAXHOUR;
                        minute = 0;
                    }
                    else
                    {
                        minute = t - MINPERHR;
                    }
                }
            }
        }

        ///<summary>Constructs an instance from hours and minutes</summary>
        public SDVTime(int h, int m) //Did some rearranging so that SDVTime(0,120) is possible
        {
            hour = h;
            minute = m;

            //Correct minutes range to 0-59
            while (minute > (MINPERHR - 1))
            {
                this.hour++;
                this.minute -= MINPERHR;
            }
            while (minute < 0)
            {
                hour--;
                minute += MINPERHR;
            }

            if (hour > MAXHOUR ||
                (hour == MAXHOUR && minute != 0))
                throw new ArgumentOutOfRangeException($"Invalid Time passed to the constructor. Time value more than {MAXHOUR}");
            if (hour < 0)
                throw new ArgumentOutOfRangeException("Invalid Time passed to the constructor. Time value less than 0.");

            /*if (m >= MINPERHR)
                throw new ArgumentOutOfRangeException("There are only 60 minutes in an hour.");*/
        }

        ///<summary>Constructs an integer time value to minutes</summary>
        public static int ConvertTimeToMinutes(int intTime)
        {
            return (intTime / 100) * MINPERHR + (intTime % 100);
        }
        ///<summary>Constructs an SDVTime value to minutes</summary>
        public static int ConvertTimeToMinutes(SDVTime sTime)
        {
            return sTime.hour * MINPERHR + sTime.minute;
        }

        /// <summary>
        /// This function takes two integer times and returns minutes between. Note: this returns an absolute value.
        /// </summary>
        /// <param name="t1">The first int time.</param>
        /// <param name="t2">The second int time</param>
        /// <returns>Amount of minutes between the two times</returns>
        public static int MinutesBetweenTwoIntTimes(int t1, int t2)
        {
            return Math.Abs(SDVTime.ConvertIntTimeToMinutes(t1) - SDVTime.ConvertIntTimeToMinutes(t2));
        }

        ///<summary>Constructs an instance by copying another SDVTime instance</summary>
        public SDVTime(SDVTime c)
        {
            hour = c.hour;
            minute = c.minute;
        }

        ///<summary>Round this SDVTime instance to the nearest 10m</summary>
        public void ClampToTenMinutes()
        {
            if (minute % 10 >= 5)
            {
                minute = ((minute / 10) + 1) * 10;

                if (minute >= MINPERHR)
                {
                    hour++;
                    minute -= MINPERHR;
                }

                /*if (hour > MAXHOUR)
                    hour -= MAXHOUR;*/ //No rollover
            }
            if (minute % 10 < 5 && (minute % 10 != 0))
            {
                minute = ((minute / 10) - 1) * 10;

                if (minute < 0)
                {
                    hour--;
                    minute += MINPERHR;
                }
            }
        }

        ///<summary>Adds time (in hours and minutes) to this SDVTime instance</summary>
        public void AddTime(int hour, int minute)
        {
            this.hour += hour;
            this.minute += minute;
            while (minute > (MINPERHR - 1))
            {
                this.hour++;
                this.minute -= MINPERHR;
            }

            while (minute < 0)
            {
                hour--;
                minute += MINPERHR;
            }

            /*if (hour > MAXHOUR)
                this.hour -= MAXHOUR;*/ //No rollover //Does this roll over into a new day, then? Does that work with MAXHOUR 28?
        }

        ///<summary>Adds an integer number of minutes to this SDVTime instance</summary>
        public void AddMinutes(int minutes) //Changed name to AddMinutes (was AddTime)
        {
            int addhr = minutes / MINPERHR;
            int addmin = minutes - (MINPERHR * addhr);

            hour += addhr;

            minute += addmin;
            while (minute > (MINPERHR - 1))
            {
                hour++;
                minute -= MINPERHR;
            }

            while (minute < 0)
            {
                hour--;
                minute += MINPERHR;
            }

            /*if (hour > MAXHOUR)
                hour -= MAXHOUR;*/ //No rollover
        }

        ///<summary>Adds a SDVTime to this SDVTime instance</summary>
        public void AddTime(SDVTime sTime)
        {
            hour += sTime.hour;
            minute += sTime.minute;

            while (minute > (MINPERHR - 1))
            {
                hour++;
                minute -= MINPERHR;
            }

            while (minute < 0)
            {
                hour--;
                minute += MAXHOUR;
            }

            /*if (hour >= MAXHOUR)
            {
                hour -= MAXHOUR;
            }*/ //No rollover
        }

        /*public static int AddTimeToIntTime(int s1, int s2) //Is this one an int time (e.g. 1620) and an int minutes?
        {
            SDVTime sTime = new SDVTime(s1);
            sTime.AddTime(s2);
            return sTime.ReturnIntTime();
        }*/


        //operator functions

        ///<summary>Adds two instances of SDVTime</summary>
        public static SDVTime operator +(SDVTime s1, SDVTime s2)
        {
            SDVTime ret = new SDVTime(s1);
            ret.AddTime(s2);
            return ret;
        }

        ///<summary>Returns the difference of two instances of SDVTime</summary>
        public static SDVTime operator -(SDVTime s1, SDVTime s2)
        {
            SDVTime ret = new SDVTime(s1);
            ret.hour -= s2.hour;
            ret.minute -= s2.minute;

            while (ret.minute > (MINPERHR - 1))
            {
                ret.hour++;
                ret.minute -= MINPERHR;
            }

            while (ret.minute < 0)
            {
                ret.hour--;
                ret.minute += MINPERHR;
            }

            /*if (ret.hour >= MAXHOUR)
            {
                ret.hour -= MAXHOUR;
            }*/ //No rollover
            return ret;
        }

        /*public static SDVTime operator -(SDVTime s1, int time) //Not using these ones
        {
            SDVTime ret = new SDVTime(s1);
            ret.AddTime(time * -1);
            return ret;
        }

        public static SDVTime operator +(SDVTime s1, int time)
        {
            SDVTime ret = new SDVTime(s1);
            ret.AddTime(time);
            return ret;
        }*/

        ///<summary>Tests two instances of SDVTime for equality</summary>
        public static bool operator ==(SDVTime s1, SDVTime s2)
        {
            if (s1 is null && s2 is null)
            {
                return true;
            }
            else if (s1 is null || s2 is null)
            {
                return false;
            }
            return (s1.hour == s2.hour) && (s1.minute == s2.minute);
        }

        ///<summary>Tests two instances of SDVTime for inequality</summary>
        public static bool operator !=(SDVTime s1, SDVTime s2)
        {
            return !(s1 == s2);
        }

        ///<summary>Tests an instance of SDVTime and an instance of integer time for equality. Gives unpredictable behavior.</summary>
        public static bool operator ==(SDVTime s1, int s2) //A bit odd. SDVTime(5,20) == 520 returns true, but SDVTime(5,20) + 520 doesn't return SDVTime(10, 40)
        {
            if ((s1.hour == (s2 / 100)) && (s1.minute == (s2 % 100)))
                return true;
            else
                return false;
        }

        ///<summary>Tests an instance of SDVTime and an instance of integer time for inequality. Gives unpredictable behavior.</summary>
        public static bool operator !=(SDVTime s1, int s2)
        {
            return !(s1 == s2);
        }

        ///<summary>Tests if an instance of SDVTime is greater than (chronologically after) another instance.</summary>
        public static bool operator >(SDVTime s1, SDVTime s2)
        {
            if (s1.hour > s2.hour)
                return true;
            else if (s1.hour == s2.hour && s1.minute > s2.minute)
                return true;

            return false;
        }

        ///<summary>Tests if an instance of SDVTime is less than (chronologically before) another instance.</summary>
        public static bool operator <(SDVTime s1, SDVTime s2)
        {
            if (s1.hour < s2.hour)
                return true;
            else if (s1.hour == s2.hour && s1.minute < s2.minute)
                return true;

            return false;
        }

        ///<summary>Tests if an instance of SDVTime is greater than (chronologically after) or equal to another instance.</summary>
        public static bool operator >=(SDVTime s1, SDVTime s2)
        {
            if (s1 == s2)
                return true;
            if (s1.hour > s2.hour)
                return true;
            else if (s1.hour == s2.hour && s1.minute > s2.minute)
                return true;

            return false;
        }

        ///<summary>Tests if an instance of SDVTime is less than (chronologically before) or equal to another instance.</summary>
        public static bool operator <=(SDVTime s1, SDVTime s2)
        {
            if (s1 == s2)
                return true;
            if (s1.hour < s2.hour)
                return true;
            else if (s1.hour == s2.hour && s1.minute < s2.minute)
                return true;

            return false;
        }

        ///<summary>Return the absolute distance from midnight in minutes for this instance of SDVTime.</summary>
        public int GetNumberOfMinutesFromMidnight()
        {
            if (hour > 24)
            {
                return (((hour - 24) * MINPERHR) + minute);
            }
            else
            {
                return (hour * MINPERHR) + minute;
            }
        }

        //description and return functions

        ///<summary>Returns the value of this SDVTime instance converted to integer time.</summary>
        public int ReturnIntTime()
        {
            return (hour * 100) + minute;
        }

        ///<summary>Tests if an integer time value represents a valid game time.</summary>
        public static bool IsValidGameTime(int time)
        {
            //between 6am and 2am
            if (time < new SDVTime(6, 0).ReturnIntTime() || time > new SDVTime(26, 0).ReturnIntTime())
                return false;
            //check formatting
            if ((time % 100) > 59)
                return false;

            return true;
        }

        ///<summary>Tests if a SDVTime instance represents a valid game time.</summary>
        public static bool IsValidGameTime(SDVTime sTime)
        {
            //between 6am and 2am
            if (sTime < new SDVTime(6, 0) || sTime > new SDVTime(26, 0))
                return false;

            return true;
        }

        ///<summary>Tests if an integer time value represents a sensible time value.</summary>
        public static bool IsValidTime(int time)
        {
            //not negative or more than max
            if (time < new SDVTime(0, 0).ReturnIntTime() || time > new SDVTime(MAXHOUR, 0).ReturnIntTime())
                return false;
            //check formatting
            if ((time % 100) > 59)
                return false;

            return true;
        }

        ///<summary>Tests if an SDVTime instance represents a sensible time value.</summary>
        public static bool IsValidTime(SDVTime sTime)
        {
            //not negative or more than max
            if (sTime < new SDVTime(0, 0) || sTime > new SDVTime(MAXHOUR, 0))
                return false;

            return true;
        }

        ///<summary>Return the value of this SDVTime instance represented in 12-hour time, e.g. 03:40 pm</summary>
        public string Get12HourTime()
        {
            if (hour < 12)
                return $"{hour}:{minute.ToString().PadLeft(2, '0')} am";
            else if (hour == 12)
                return $"{hour}:{minute.ToString().PadLeft(2, '0')} pm";
            else if (hour > 12 && hour < 24)
                return $"{(hour - 12)}:{minute.ToString().PadLeft(2, '0')} pm";
            else if (hour == 24)
                return $"{hour}:{minute.ToString().PadLeft(2, '0')} am";
            else if (hour > 24)
                return $"{(hour - 24)}:{minute.ToString().PadLeft(2, '0')} am";

            return "99:99 99";
        }

        ///<summary>Return the value of this SDVTime instance represented in 24-hour time, e.g. "1710"</summary>
        public override string ToString()
        {
            if (hour < 24)
                return $"{hour.ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')}";
            else
                return $"{(hour - 24).ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')}";
        }

        ///<summary>Test this SDVTime instance for equality with another object</summary>
        public override bool Equals(object obj)
        {
            SDVTime time = (SDVTime)obj;

            return time != null &&
                   hour == time.hour &&
                   minute == time.minute;
        }

        ///<summary>Test this SDVTime instance for equality with another instance of SDVTime</summary>
        public bool Equals(SDVTime other)
        {
            return other != null &&
                   hour == other.hour &&
                   minute == other.minute;
        }

        ///<summary>Convert an integer time value to total minutes</summary>
        public static int ConvertIntTimeToMinutes(int time) //Redundant? ConvertTimeToMinutes
        {
            int hour = time / 100;
            int min = time % 100;

            return ((hour * MINPERHR) + min);
        }

        ///<summary>Get hashcode for this SDVTime instance</summary>
        public override int GetHashCode()
        {
            var hashCode = -1190848304;
            hashCode = hashCode * -1521134295 + hour.GetHashCode();
            hashCode = hashCode * -1521134295 + minute.GetHashCode();
            return hashCode;
        }
    }
}