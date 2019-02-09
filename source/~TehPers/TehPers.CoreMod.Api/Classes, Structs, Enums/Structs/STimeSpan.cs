using System;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct STimeSpan : IEquatable<STimeSpan>, IComparable<STimeSpan> {
        /// <summary>The total number of elapsed years.</summary>
        public float TotalYears => (float) this.TotalMinutes / (4f * 28f * 2400f);

        /// <summary>The total number of elapsed seasons.</summary>
        public float TotalSeasons => (float) this.TotalMinutes / (28f * 2400f);

        /// <summary>The total number of elapsed days.</summary>
        public float TotalDays => (float) this.TotalMinutes / 2400f;

        /// <summary>The total number of elapsed minutes.</summary>
        public int TotalMinutes { get; }

        public STimeSpan(int minutes = 0, int days = 0, int seasons = 0, int years = 0) {
            this.TotalMinutes = minutes + days * 2400 + seasons * 28 * 2400 + years * 4 * 28 * 2400;
        }

        /// <inheritdoc />
        public int CompareTo(STimeSpan other) {
            return this.TotalMinutes.CompareTo(other.TotalMinutes);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is STimeSpan other && this.Equals(other);
        }

        public bool Equals(STimeSpan other) {
            return this.TotalMinutes == other.TotalMinutes;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.TotalMinutes;
        }

        public static STimeSpan operator +(in STimeSpan first, in STimeSpan second) => new STimeSpan(first.TotalMinutes + second.TotalMinutes);
        public static STimeSpan operator -(in STimeSpan first, in STimeSpan second) => new STimeSpan(first.TotalMinutes + second.TotalMinutes);
        public static bool operator >(in STimeSpan first, in STimeSpan second) => first.CompareTo(second) > 0;
        public static bool operator >=(in STimeSpan first, in STimeSpan second) => first.CompareTo(second) >= 0;
        public static bool operator <(in STimeSpan first, in STimeSpan second) => first.CompareTo(second) < 0;
        public static bool operator <=(in STimeSpan first, in STimeSpan second) => first.CompareTo(second) <= 0;
        public static bool operator ==(in STimeSpan first, in STimeSpan second) => first.Equals(second);
        public static bool operator !=(in STimeSpan first, in STimeSpan second) => !first.Equals(second);

        /// <summary>Creates a new <see cref="STimeSpan"/> from a specified number of minutes.</summary>
        /// <param name="minutes">The number of minutes that should be represented in the new instance.</param>
        /// <returns>A new <see cref="STimeSpan"/> representing the specified number of minutes.</returns>
        public static STimeSpan FromMinutes(int minutes) => new STimeSpan(minutes);

        /// <summary>Creates a new <see cref="STimeSpan"/> from a specified number of days.</summary>
        /// <param name="days">The number of days that should be represented in the new instance.</param>
        /// <returns>A new <see cref="STimeSpan"/> representing the specified number of days.</returns>
        public static STimeSpan FromDays(int days) => new STimeSpan(days: days);

        /// <summary>Creates a new <see cref="STimeSpan"/> from a specified number of seasons.</summary>
        /// <param name="seasons">The number of seasons that should be represented in the new instance.</param>
        /// <returns>A new <see cref="STimeSpan"/> representing the specified number of seasons.</returns>
        public static STimeSpan FromSeasons(int seasons) => new STimeSpan(seasons: seasons);

        /// <summary>Creates a new <see cref="STimeSpan"/> from a specified number of years.</summary>
        /// <param name="years">The number of years that should be represented in the new instance.</param>
        /// <returns>A new <see cref="STimeSpan"/> representing the specified number of years.</returns>
        public static STimeSpan FromYears(int years) => new STimeSpan(years: years);
    }
}