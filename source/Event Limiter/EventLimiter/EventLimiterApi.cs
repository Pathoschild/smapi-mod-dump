/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/EventLimiter
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLimiter
{
    // Api for Event Limiter
    public class EventLimiterApi
    {
        // Fields and properties to allow api to work, these can't be implemented by mods
        private static ModConfig config;
        private static List<int> internalexceptions;
        internal static void Hook(ModConfig config, List<int> internalexceptions)
        {
            EventLimiterApi.config = config;
            EventLimiterApi.internalexceptions = internalexceptions;
        }

        // Public api, useable by mods

        /// <summary>
        /// Get day event limit, max amount of events seen in a day, minus exceptions.
        /// </summary>
        /// <returns>The day event limit</returns>
        public int GetDayLimit()
        {
            return config.EventsPerDay;
        }

        /// <summary>
        /// Get in a row event limit, amount of events seen in a row, minus exceptions.
        /// </summary>
        /// <returns>The max amount of events in a row</returns>
        public int GetRowLimit()
        {
            return config.EventsInARow;
        }

        /// <summary>
        /// Get a list of event exceptions.
        /// </summary>
        /// <param name="includeinternal">Whether internally added events (events not shown in config) are returned</param>
        /// <returns>A list of events that are exempt from normal event limits</returns>
        public List<int> GetExceptions(bool includeinternal = true)
        {
            List<int> exceptions = new List<int>();

            if (includeinternal == true)
            {
                foreach(int modexception in internalexceptions)
                {
                    exceptions.Add(modexception);
                }
            }

            foreach (int exception in config.Exceptions)
            {
                exceptions.Add(exception);
            }

            return exceptions;
        }

        /// <summary>
        /// Add an event exception that will not be reflected in the config, these are identified as mod added exceptions.
        /// </summary>
        /// <param name="eventid">The id of the event to make an exception</param>
        /// <returns>Whether the exception was successfully added</returns>
        public bool AddInternalException(int eventid)
        {
            try
            {
                internalexceptions.Add(eventid);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

}
