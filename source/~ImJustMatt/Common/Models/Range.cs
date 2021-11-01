/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Models
{
    using System;

    /// <summary>
    ///     Represents a range of values.
    /// </summary>
    /// <typeparam name="T">The value type for the range.</typeparam>
    internal class Range<T>
        where T : IComparable<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Range{T}" /> class.
        /// </summary>
        public Range()
        {
            this.Minimum = default!;
            this.Maximum = default!;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Range{T}" /> class.
        /// </summary>
        /// <param name="minimum">The minimum value of the range.</param>
        /// <param name="maximum">The maximum value of the range.</param>
        public Range(T minimum, T maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        ///     Gets or sets the minimum value of the range.
        /// </summary>
        public T Minimum { get; set; }

        /// <summary>
        ///     Gets or sets the maximum value of the range.
        /// </summary>
        public T Maximum { get; set; }

        /// <summary>
        ///     Clamps a value based on the range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>Returns the clamped value.</returns>
        /// <exception cref="InvalidOperationException">Range is not valid.</exception>
        public T Clamp(T value)
        {
            if (!this.IsValid() || this.Minimum is null || this.Maximum is null)
            {
                throw new InvalidOperationException();
            }

            if (value.CompareTo(this.Minimum) <= 0)
            {
                return this.Minimum;
            }

            return this.Maximum.CompareTo(value) <= 0 ? this.Maximum : value;
        }

        private bool IsValid()
        {
            return this.Minimum is not null && this.Maximum is not null && this.Minimum.CompareTo(this.Maximum) <= 0;
        }
    }
}