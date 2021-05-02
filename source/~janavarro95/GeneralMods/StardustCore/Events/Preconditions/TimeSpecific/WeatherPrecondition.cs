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

namespace StardustCore.Events.Preconditions.TimeSpecific
{
    public class WeatherPrecondition:EventPrecondition
    {
        public enum Weather
        {
            Sunny,
            Rainy,
            Debris,
            Storm,
            Festival,
            Snow,
            Wedding
        }

        public Weather weather;

        public WeatherPrecondition()
        {
            
        }

        /// <summary>
        /// Creates the prconiditon that it must be sunny for the event to occur.
        /// </summary>
        /// <returns></returns>
        public string precondition_sunnyWeather()
        {
            StringBuilder b = new StringBuilder();
            b.Append("w sunny");
            return b.ToString();
        }

        /// <summary>
        /// Creates the precondition that it must be rainy for the event to occur.
        /// </summary>
        /// <returns></returns>
        public string precondition_rainyWeather()
        {
            StringBuilder b = new StringBuilder();
            b.Append("w rainy");
            return b.ToString();
        }

        public override string ToString()
        {
            if(this.weather== Weather.Sunny)
            {
                return this.precondition_sunnyWeather();
            }

            if(this.weather== Weather.Rainy)
            {
                return this.precondition_rainyWeather();
            }

            return "";
        }

        public override bool meetsCondition()
        {
            if (Game1.weatherIcon == (int)this.weather) return true;
            else return false;
        }

    }
}
