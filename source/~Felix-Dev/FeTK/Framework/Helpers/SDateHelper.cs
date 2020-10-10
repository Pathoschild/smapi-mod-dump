/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Helpers
{
    /// <summary>
    /// Provides an API for common tasks involving instances of the <see cref="SDate"/> class.
    /// </summary>
    public static class SDateHelper
    {
        /// <summary>
        /// Create a equivalent <see cref="SDate"/> instance from a given game day.
        /// </summary>
        /// <param name="gameDay">The game day to create an <see cref="SDate"/> reprensentation for. First game day has index "1".</param>
        /// <returns>An equivalent <see cref="SDate"/> representation for the specified <paramref name="gameDay"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="gameDay"/> is less than or equal to zero.</exception>
        public static SDate GetDateFromDay(int gameDay)
        {
            if (gameDay <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gameDay), $"The specified day \"{gameDay}\" has to be greater than zero!");
            }

            return new SDate(1, "spring", 1).AddDays(gameDay - 1);
        }

        /// <summary>
        /// Get the day offset to the current in-game day based on the given date.
        /// </summary>
        /// <param name="date">The date to compute the day offset for.</param>
        /// <returns>The computed day offset.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="date"/> is <c>null</c>.</exception>
        public static int GetCurrentDayOffsetFromDate(SDate date)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date));
            }

            return date.DaysSinceStart - SDate.Now().DaysSinceStart;
        }
    }
}
