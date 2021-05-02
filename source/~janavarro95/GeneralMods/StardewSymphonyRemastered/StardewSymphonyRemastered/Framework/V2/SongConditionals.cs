/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardewSymphonyRemastered.Framework.V2
{
    public class SongConditionals
    {
        public string season;
        public string weather;
        public string time;
        public string location;
        public string dayOfWeek;
        public string eventKey;
        public string festival;
        public string menu;

        private readonly string[] seasons;
        private readonly string[] weathers;
        private readonly string[] daysOfWeek;
        private readonly string[] timesOfDay;
        public static char seperator = '_';

        public SongConditionals()
        {
            this.seasons = new[]
{
                "spring",
                "summer",
                "fall",
                "winter"
            };

            this.weathers = new[]
            {
                "sunny",
                "rain",
                "debris",
                "lightning",
                "snow",
                "festival",
                "wedding"
            };
            this.daysOfWeek = new[]
            {
                "sunday",
                "monday",
                "tuesday",
                "wednesday",
                "thursday",
                "friday",
                "saturday"
            };
            this.timesOfDay = new[]
            {
                "day",
                "night",
                "12A.M.",
                "1A.M.",
                "2A.M.",
                "3A.M.",
                "4A.M.",
                "5A.M.",
                "6A.M.",
                "7A.M.",
                "8A.M.",
                "9A.M.",
                "10A.M.",
                "11A.M.",
                "12P.M.",
                "1P.M.",
                "2P.M.",
                "3P.M.",
                "4P.M.",
                "5P.M.",
                "6P.M.",
                "7P.M.",
                "8P.M.",
                "9P.M.",
                "10P.M.",
                "11P.M.",
            };
        }

        public SongConditionals(string key) : this()
        {
            this.setConditionalsFromKey(key);
        }

        /// <summary>
        /// Parse a given key and figure out when a song can play.
        /// </summary>
        /// <param name="key"></param>
        public void setConditionalsFromKey(string key)
        {
            string[] splits = key.Split(seperator);
            foreach (string split in splits)
            {
                //Parse key into conditionals.
                if (this.seasons.Contains(split))
                {
                    this.season = split;
                }
                else if (this.weathers.Contains(split))
                {
                    this.weather = split;
                }
                else if (this.daysOfWeek.Contains(split))
                {
                    this.dayOfWeek = split;
                }
                else if (this.timesOfDay.Contains(split))
                {
                    this.time = split;
                }
                else if (SongSpecificsV2.menus.Contains(split))
                {
                    this.menu = split;
                }
                else if (SongSpecificsV2.locations.Contains(split))
                {
                    this.location = split;
                }
                else if (SongSpecificsV2.events.Contains(split))
                {
                    this.eventKey = split;
                }
                else if (SongSpecificsV2.festivals.Contains(split))
                {
                    this.festival = split;
                }
            }
        }

        /// <summary>
        /// Checks if a song can be played provided a given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool canBePlayed(SongConditionals other)
        {
            if (this.isLocationSpecific())
            {
                if (!string.IsNullOrEmpty(other.location))
                {
                    if (this.location.Equals(other.location)==false) return false; //If there is a check but not the right location return false
                }
                else
                {
                    return false; //If there is no check against this then return false;
                }
            }
            if (this.isTimeSpecific())
            {
                if (!string.IsNullOrEmpty(other.time))
                {
                    if (this.time.Equals(other.time)==false) return false; //If the two times don't match return false
                }
                else
                {
                    return false; //If there is no check against this and this is time specific don't allow it.
                }
            }


            if (this.isDaySpecific())
            {
                //condition specific check
                if (!string.IsNullOrEmpty(other.dayOfWeek))
                {
                    if (this.dayOfWeek.Equals(other.dayOfWeek)==false) return false;
                }
                else
                {
                    return false; //There is no check against this day of the week. Don't allow it.
                }
            }

            //Check for season.
            if (this.isSeasonSpecific())
            {
                if (!string.IsNullOrEmpty(other.season))
                {
                    if (this.season.Equals(other.season)==false) return false;
                }
                else
                {
                    return false;
                }
            }

            //Check for weather.
            if (this.isWeatherSpecific())
            {
                if (!string.IsNullOrEmpty(other.weather))
                {
                    if (this.weather.Equals(other.weather)==false) return false;
                }
                else
                {
                    return false;
                }
            }


            if (!string.IsNullOrEmpty(this.menu))
            {
                if (!string.IsNullOrEmpty(other.menu))
                {
                    if (this.menu.Equals(other.menu)==false) return false;
                }
            }

            if (!string.IsNullOrEmpty(this.festival))
            {
                if (!string.IsNullOrEmpty(other.festival))
                {
                    if (this.festival.Equals(other.festival)==false) return false;
                }
            }

            if (!string.IsNullOrEmpty(this.eventKey))
            {
                if (!string.IsNullOrEmpty(other.eventKey))
                {
                    if (this.eventKey.Equals(other.eventKey)==false) return false;
                }
            }
            return true;
        }

        public bool isLocationSpecific()
        {
            return !string.IsNullOrEmpty(this.location);
        }

        public bool isTimeSpecific()
        {
            return !string.IsNullOrEmpty(this.time);
        }

        public bool isSeasonSpecific()
        {
            return !string.IsNullOrEmpty(this.season);
        }
        public bool isWeatherSpecific()
        {
            return !string.IsNullOrEmpty(this.weather);
        }
        public bool isDaySpecific()
        {
            return !string.IsNullOrEmpty(this.dayOfWeek);
        }

        /// <summary>
        /// Checks if the given conditonal is generic or not.
        /// </summary>
        /// <returns></returns>
        public bool isKeyGeneric()
        {
            if (this.isTimeSpecific() == false && this.isLocationSpecific() == false) return true;
            else return false;
        }
        /*
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SongConditionals)) return false;
            SongConditionals other = (SongConditionals)obj;
            if (this.season != other.season) return false;
            if (this.weather != other.weather) return false;
            if (this.dayOfWeek != other.dayOfWeek) return false;
            if (this.location != other.location) return false;
            if (this.time != other.time) return false;
            if (this.menu != other.menu) return false;
            if (this.eventKey != other.eventKey) return false;
            if (this.festival != other.festival) return false;

            return true;
        }
        */
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("Season: " + this.season+Environment.NewLine);
            b.Append("Weather: " + this.weather + Environment.NewLine);
            b.Append("Time: " + this.time + Environment.NewLine);
            b.Append("Location: " + this.location + Environment.NewLine);
            b.Append("Day: " + this.dayOfWeek + Environment.NewLine);
            b.Append("Festival: " + this.festival + Environment.NewLine);
            b.Append("Event: " + this.eventKey + Environment.NewLine);
            b.Append("Menu: " + this.menu + Environment.NewLine);
            return b.ToString();
        }
        
    }
}
