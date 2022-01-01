/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace TehPers.PowerGrid.Units
{
    /// <summary>
    /// A measurement of time.
    /// </summary>
    public readonly struct Duration
    {
        private const long UnitsPerSecond = 1;
        private const long UnitsPerMinute = Duration.UnitsPerSecond * 60;
        private const long UnitsPerHour = Duration.UnitsPerMinute * 60;
        private const long UnitsPerDay = Duration.UnitsPerHour * 24;

        /// <summary>
        /// Units of time. This represents the exact amount of time contained in this
        /// <see cref="Duration"/> in arbitrary units. This value should not be treated as an exact
        /// duration.
        /// </summary>
        public long Units { get; }

        /// <summary>
        /// Total number of whole minutes contained in this <see cref="Duration"/>.
        /// </summary>
        public long TotalSeconds => this.Units / Duration.UnitsPerSecond;

        /// <summary>
        /// Total number of whole minutes contained in this <see cref="Duration"/>.
        /// </summary>
        public long TotalMinutes => this.Units / Duration.UnitsPerMinute;

        /// <summary>
        /// Total number of whole hours contained in this <see cref="Duration"/>.
        /// </summary>
        public long TotalHours => this.Units / Duration.UnitsPerHour;

        /// <summary>
        /// Total number of whole days contained in this <see cref="Duration"/>.
        /// </summary>
        public long TotalDays => this.Units / Duration.UnitsPerDay;

        public long Seconds => this.TotalSeconds % 60;
        public long Minutes => this.TotalMinutes % 60;
        public long Hours => this.TotalMinutes % 24;
        public long Days => this.TotalDays;

        public Duration(long value)
        {
            this.Units = value;
        }

        /// <summary>
        /// Creates a new <see cref="Duration"/> instance from the number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds in the <see cref="Duration"/>.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of seconds.</returns>
        public static Duration FromSeconds(long seconds) => new(seconds * Duration.UnitsPerSecond);

        /// <summary>
        /// Creates a new <see cref="Duration"/> instance from the number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes in the <see cref="Duration"/>.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of minutes.</returns>
        public static Duration FromMinutes(long minutes) => new(minutes * Duration.UnitsPerMinute);

        /// <summary>
        /// Creates a new <see cref="Duration"/> instance from the number of hours.
        /// </summary>
        /// <param name="hours">The number of hours in the <see cref="Duration"/>.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of hours.</returns>
        public static Duration FromHours(long hours) => new(hours * Duration.UnitsPerHour);

        /// <summary>
        /// Creates a new <see cref="Duration"/> instance from the number of days.
        /// </summary>
        /// <param name="days">The number of days in the <see cref="Duration"/>.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of days.</returns>
        public static Duration FromDays(long days) => new(days * Duration.UnitsPerDay);

        public static Duration operator +(Duration lhs, Duration rhs) => new(lhs.Units + rhs.Units);
        public static Duration operator -(Duration lhs, Duration rhs) => new(lhs.Units - rhs.Units);
        public static Duration operator -(Duration time) => new(-time.Units);
    }
}