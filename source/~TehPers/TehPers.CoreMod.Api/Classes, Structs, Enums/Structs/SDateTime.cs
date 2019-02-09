using System;
using StardewValley;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Extensions;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct SDateTime : IEquatable<SDateTime>, IComparable<SDateTime> {
        /// <summary>The total number of elapsed years.</summary>
        public float TotalYears => (float) this.TotalMinutes / (4f * 28f * 2400f);

        /// <summary>The total number of elapsed seasons.</summary>
        public float TotalSeasons => (float) this.TotalMinutes / (28f * 2400f);

        /// <summary>The total number of elapsed days.</summary>
        public float TotalDays => (float) this.TotalMinutes / 2400f;

        /// <summary>The total number of elapsed minutes.</summary>
        public int TotalMinutes { get; }

        /// <summary>The number of elapsed years.</summary>
        public int Year => (int) this.TotalYears + 1;

        /// <summary>The number of elapsed seasons in the year.</summary>
        public Season Season => ((int) this.TotalSeasons % 4).Match<int, Season>()
            .When(0, Season.Spring)
            .When(1, Season.Summer)
            .When(2, Season.Fall)
            .When(3, Season.Winter)
            .ElseThrow();

        /// <summary>The number of elapsed days in the season.</summary>
        public int DayOfSeason => (int) this.TotalDays % 28 + 1;

        /// <summary>The current time of day in SDV format (hhmm).</summary>
        public int TimeOfDay => this.TotalMinutes + 40 * (this.TotalMinutes / 60);

        /// <summary>The number of elapsed minutes in the day. This is not in SDV time format, use <see cref="TimeOfDay"/> instead if that is needed.</summary>
        public int MinutesOfDay => this.TotalMinutes % 2400;

        public SDateTime(int years, Season season, int days = 0, int minutes = 0) {
            int seasons = season.Match<Season, int>()
                .When(Season.Spring, 0)
                .When(Season.Summer, 1)
                .When(Season.Fall, 2)
                .When(Season.Winter, 3)
                .ElseThrow();

            this.TotalMinutes = minutes + days * 2400 + seasons * 28 * 2400 + years * 4 * 28 * 2400;
        }

        public SDateTime(int years, int seasons = 0, int days = 0, int minutes = 0) {
            this.TotalMinutes = minutes + days * 2400 + seasons * 28 * 2400 + years * 4 * 28 * 2400;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is SDateTime other && this.Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(SDateTime other) {
            return this.TotalMinutes == other.TotalMinutes;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.TotalMinutes;
        }

        /// <inheritdoc />
        public int CompareTo(SDateTime other) {
            return this.TotalMinutes.CompareTo(other.TotalMinutes);
        }

        /// <inheritdoc />
        public override string ToString() {
            int timeOfDay = this.TimeOfDay;
            return $"{this.Season} {this.DayOfSeason}, {this.Year} {timeOfDay / 100}:{timeOfDay % 100}";
        }

        public static SDateTime operator +(in SDateTime first, in STimeSpan second) => new SDateTime(0, 0, 0, first.TotalMinutes + second.TotalMinutes);
        public static SDateTime operator -(in SDateTime first, in STimeSpan second) => new SDateTime(0, 0, 0, first.TotalMinutes - second.TotalMinutes);
        public static STimeSpan operator -(in SDateTime first, in SDateTime second) => new STimeSpan(first.TotalMinutes - second.TotalMinutes);
        public static bool operator >(in SDateTime first, in SDateTime second) => first.CompareTo(second) > 0;
        public static bool operator >=(in SDateTime first, in SDateTime second) => first.CompareTo(second) >= 0;
        public static bool operator <(in SDateTime first, in SDateTime second) => first.CompareTo(second) < 0;
        public static bool operator <=(in SDateTime first, in SDateTime second) => first.CompareTo(second) <= 0;
        public static bool operator ==(in SDateTime first, in SDateTime second) => first.Equals(second);
        public static bool operator !=(in SDateTime first, in SDateTime second) => !first.Equals(second);

        public static SDateTime FromDateAndTime(int year, Season season, int dayOfSeason, int timeOfDay) {
            return new SDateTime(year, season, dayOfSeason, 60 * (timeOfDay / 100) + timeOfDay % 100);
        }

        /// <summary>The current date and time.</summary>
        public static SDateTime Now => SDateTime.FromDateAndTime(Game1.year, Game1.currentSeason?.GetSeason() ?? Season.Spring, Game1.dayOfMonth, Game1.timeOfDay);

        /// <summary>The current date.</summary>
        public static SDateTime Today => new SDateTime(Game1.year, Game1.currentSeason?.GetSeason() ?? Season.Spring, Game1.dayOfMonth);


    }
}