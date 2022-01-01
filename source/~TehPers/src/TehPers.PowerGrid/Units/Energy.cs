/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.PowerGrid.Units
{
    /// <summary>
    /// A measurement of energy.
    /// </summary>
    /// <inheritdoc cref="IEquatable{T}"/>
    /// <inheritdoc cref="IComparable{T}"/>
    public readonly struct Energy : IEquatable<Energy>, IComparable<Energy>
    {
        private const long UnitsPerMilliwatt = Energy.UnitsPerWatt / 1000;
        private const long UnitsPerWatt = 81000;
        private const long UnitsPerKilowatt = Energy.UnitsPerWatt * 1000;
        private const long UnitsPerMegawatt = Energy.UnitsPerKilowatt * 1000;
        private const long UnitsPerGigawatt = Energy.UnitsPerMegawatt * 1000;
        private const long UnitsPerTerawatt = Energy.UnitsPerGigawatt * 1000;

        public static Energy FromMilliwatts(long milliwatts) =>
            new(milliwatts * Energy.UnitsPerMilliwatt);
        public static Energy FromWatts(long watts) => new(watts * Energy.UnitsPerWatt);
        public static Energy FromKilowatts(long kilowatts) => new(kilowatts * Energy.UnitsPerKilowatt);
        public static Energy FromMegawatts(long megawatts) => new(megawatts * Energy.UnitsPerKilowatt);
        public static Energy FromGigawatts(long gigawatts) => new(gigawatts * Energy.UnitsPerKilowatt);
        public static Energy FromTerawatts(long terawatts) => new(terawatts * Energy.UnitsPerKilowatt);

        public long Units { get; }

        public long TotalMilliwatts => this.Units / Energy.UnitsPerMilliwatt;
        public long TotalWatts => this.Units / Energy.UnitsPerWatt;
        public long TotalKilowatts => this.Units / Energy.UnitsPerKilowatt;
        public long TotalMegawatts => this.Units / Energy.UnitsPerMegawatt;
        public long TotalGigawatts => this.Units / Energy.UnitsPerGigawatt;
        public long TotalTerawatts => this.Units / Energy.UnitsPerTerawatt;

        public Energy(long units)
        {
            this.Units = units;
        }

        public override string ToString()
        {
            var actualJoules = this.Units / 81000d;
            return $"{actualJoules:G}J";
        }

        public bool Equals(Energy other)
        {
            return this.Units == other.Units;
        }

        public override bool Equals(object? obj)
        {
            return obj is Energy other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Units.GetHashCode();
        }

        public int CompareTo(Energy other)
        {
            return this.Units.CompareTo(other.Units);
        }

        public static bool operator ==(Energy left, Energy right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Energy left, Energy right)
        {
            return !(left == right);
        }

        public static bool operator <(Energy left, Energy right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Energy left, Energy right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Energy left, Energy right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Energy left, Energy right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}