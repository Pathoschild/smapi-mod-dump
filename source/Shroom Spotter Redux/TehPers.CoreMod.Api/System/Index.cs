/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System {
    /// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
    /// <remarks>
    /// <see cref="Index"/> is used by the C# compiler to support the new index syntax
    /// <code>
    /// int[] someArray = new int[5] { 1, 2, 3, 4, 5 } ;
    /// int lastElement = someArray[^1]; // lastElement = 5
    /// </code>
    /// 
    /// Source: https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Index.cs (Slightly modified)
    /// License: https://github.com/dotnet/corefx/blob/master/LICENSE.TXT (MIT)
    /// </remarks>
    public readonly struct Index : IEquatable<Index> {
        private readonly int _value;

        /// <summary>The offset from either the start or end.</summary>
        public int Value => this._value < 0 ? ~this._value : this._value;

        /// <summary>Whether to offset from the end.</summary>
        public bool IsFromEnd => this._value < 0;

        /// <summary>Construct an <see cref="Index"/> using a value and indicating if the index is from the start or from the end.</summary>
        /// <param name="value">The index value. it has to be zero or positive number.</param>
        /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
        /// <remarks>
        /// If the <see cref="Index"/> constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond the last element.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index(int value, bool fromEnd = false) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Index must not be less than zero.");
            }

            this._value = fromEnd ? ~value : value;
        }

        private Index(int value) {
            this._value = value;
        }

        /// <summary>Calculate the offset from the start using the giving collection length.</summary>
        /// <param name="length">The length of the collection that the Index will be used with. length has to be a positive value</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values.
        /// we don't validate either the returned offset is greater than the input length.
        /// It is expected Index will be used with collections which always have non negative length/count. If the returned offset is negative and
        /// then used to index a collection will get out of range exception which will be same affect as the validation.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int length) {
            int offset;

            if (this.IsFromEnd)
                offset = length - (~this._value);
            else
                offset = this._value;

            return offset;
        }

        /// <inheritdoc />
        public bool Equals(Index other) {
            return this._value == other._value;
        }

        /// <inheritdoc />
        public override bool Equals(object value) {
            return value is Index valueIndex && valueIndex.Value == this._value;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this._value;
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.IsFromEnd ? $"^{this.Value}" : this.Value.ToString();
        }

        /// <summary>Converts integer number to an Index.</summary>
        public static implicit operator Index(int value) {
            return FromStart(value);
        }

        /// <summary>Create an Index from the start at the position indicated by the value.</summary>
        /// <param name="value">The index value from the start.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromStart(int value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be nonnegative");
            }

            return new Index(value);
        }

        /// <summary>Create an Index from the end at the position indicated by the value.</summary>
        /// <param name="value">The index value from the end.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromEnd(int value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be nonnegative");
            }

            return new Index(~value);
        }

        /// <summary>Create an <see cref="Index"/> pointing at the first element.</summary>
        public static Index Start => new Index(0);

        /// <summary>Create an <see cref="Index"/> pointing right after the last element.</summary>
        public static Index End => new Index(~0);
    }
}