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
using System;

namespace TwilightShards.Stardew.Common
{

    public enum SDVTimePeriods
    {
        Morning,
        Afternoon,
        Evening,
        Night,
        LateNight,
        Noon,
        Midnight
    }

    public class SDVTime
    {
        public static bool IsNight => Game1.isDarkOut();
        
        public static SDVTimePeriods CurrentTimePeriod => CurrentTime.TimePeriod;

        public SDVTimePeriods TimePeriod
        {
            get
            {
                if (this.ReturnIntTime() < 1200)
                {
                    return SDVTimePeriods.Morning;
                }
                if (this.ReturnIntTime() == 1200) { 
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

        public static SDVTime CurrentTime => new SDVTime(Game1.timeOfDay);
        public static int CurrentIntTime => new SDVTime(Game1.timeOfDay).ReturnIntTime();

        private const int MAXHOUR = 28;
        private const int MINPERHR = 60;

        int hour;
        int minute;

        public SDVTime(int t)
        {
            hour = t / 100;

            if (hour < 0)
                throw new ArgumentException("You cannot have a time value of less than 0 seconds");

            if (hour > MAXHOUR) { 
                hour = MAXHOUR;
                minute = 0;
            }
            else { 

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

        public SDVTime(int h, int m)
        {
            hour = h;
            if (hour > MAXHOUR)
                throw new ArgumentOutOfRangeException($"Invalid Time passed to the constructor. Hour value more than {MAXHOUR}");
            if (hour < 0)
                throw new ArgumentOutOfRangeException("Invalid Time passed to the constructor. Hour value less than 0.");

            minute = m;

            if (m >= MINPERHR)
                throw new ArgumentOutOfRangeException("There are only 60 minutes in an hour.");
        }

        public static int ConvertTimeToMinutes(int intTime)
        {
            return (intTime / 100) * 60 + (intTime % 100);
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

        public SDVTime(SDVTime c)
        {
            hour = c.hour;
            minute = c.minute;
        }

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

                    if (hour > MAXHOUR)
                        hour -= MAXHOUR;
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

        public void AddTime(int hour, int minute)
        {
            this.hour += hour;
            this.minute += minute;
            while (minute > (MINPERHR -1))
            {
                this.hour++;
                this.minute -= MINPERHR;
            }

            while (minute < 0)
            {
                hour--;
                minute += MINPERHR;
            }

            if (hour > MAXHOUR)
                this.hour -= MAXHOUR;
        }

        public void AddTime(int time)
        {
            int addhr = time / MINPERHR;
            int addmin = time - (MINPERHR * addhr);

            hour += addhr;

            minute += addmin;
            while (minute > (MINPERHR-1))
            {
                hour++;
                minute -= MINPERHR;
            }

            while (minute < 0)
            {
                hour--;
                minute += MINPERHR;
            }

            if (hour > MAXHOUR)
                hour -= MAXHOUR;
        }

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

            if (hour >= MAXHOUR)
            {
                hour -= MAXHOUR;
            }
        }

        public static int AddTimeToIntTime(int s1, int s2)
        {
            SDVTime sTime = new SDVTime(s1);
            sTime.AddTime(s2);
            return sTime.ReturnIntTime();
        }

        //operator functions
        public static SDVTime operator +(SDVTime s1, SDVTime s2)
        {
            SDVTime ret = new SDVTime(s1);
            ret.AddTime(s2);
            return ret;
        }

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

            if (ret.hour >= MAXHOUR)
            {
                ret.hour -= MAXHOUR;
            }
            return ret;
        }

        public static SDVTime operator -(SDVTime s1, int time)
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
        }

        public static bool operator ==(SDVTime s1, SDVTime s2)
        {
            if ((s1.hour == s2.hour) && (s1.minute == s2.minute))
                return true;
            else
                return false;
        }

        public static bool operator !=(SDVTime s1, SDVTime s2)
        {
            if ((s1.hour == s2.hour) && (s1.minute == s2.minute))
                return false;
            else
                return true;
        }

        public static bool operator ==(SDVTime s1, int s2)
        {
            if ((s1.hour == (s2 / 100)) && (s1.minute == (s2 % 100)))
                return true;
            else
                return false;
        }
        
        public static bool operator !=(SDVTime s1, int s2)
        {
            return !(s1 == s2);
        }

        public static bool operator >(SDVTime s1, SDVTime s2)
        {
            if (s1.hour > s2.hour)
                return true;
            else if (s1.hour == s2.hour && s1.minute > s2.minute)
                return true;

            return false;
        }

        public static bool operator <(SDVTime s1, SDVTime s2)
        {
            if (s1.hour < s2.hour)
                return true;
            else if (s1.hour == s2.hour && s1.minute < s2.minute)
                return true;

            return false;
        }

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
        public int ReturnIntTime()
        {
            return (hour * 100) + minute;
        }

        public static bool VerifyValidIntTime(int time)
        {
            //basic bounds first
            if (time < 0600 || time > 2800)
                return false;
            if ((time % 100) > 59)
                return false;

            return true;
        }

        public string Get12HourTime()
        {
            if (hour < 12)
                return $"{hour.ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')} am";
            else if (hour == 12)
                return $"{hour}:{minute.ToString().PadLeft(2, '0')} pm";
            else if (hour > 12 && hour < 24)
                return $"{(hour - 12).ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')} pm";
            else if (hour == 24)
                return $"{hour}:{minute.ToString().PadLeft(2, '0')} am";
            else if (hour > 24)
                return $"{(hour - 24).ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')} am";

            return "99:99 99";
        }

        public override string ToString()
        {
            if (hour < 24)
                return $"{hour.ToString().PadLeft(2,'0')}:{minute.ToString().PadLeft(2, '0')}";
            else
                return $"{(hour - 24).ToString().PadLeft(2,'0')}:{minute.ToString().PadLeft(2, '0')}";
        }

        public override bool Equals(object obj)
        {
            SDVTime time = (SDVTime)obj;

            return time != null &&
                   hour == time.hour &&
                   minute == time.minute;
        }

        public bool Equals(SDVTime other)
        {
            return other != null &&
                   hour == other.hour &&
                   minute == other.minute;
        }

        public static int ConvertIntTimeToMinutes(int time)
        {
            int hour = time / 100;
            int min = time % 100;

            return ((hour * MINPERHR) + min);
        }

        public override int GetHashCode()
        {
            var hashCode = -1190848304;
            hashCode = hashCode * -1521134295 + hour.GetHashCode();
            hashCode = hashCode * -1521134295 + minute.GetHashCode();
            return hashCode;
        }
    }
}
