/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System {
    public readonly struct Range : IEnumerable<int>, IEquatable<Range> {
        /// <summary>The start of this range.</summary>
        public Index Start { get; }

        /// <summary>The end of this range.</summary>
        public Index End { get; }

        public Range(Index start, Index end) {
            this.Start = start;
            this.End = end;
        }

        public void Deconstruct(out Index start, out Index end) {
            start = this.Start;
            end = this.End;
        }

        /// <summary>Creates a new range.</summary>
        /// <param name="start">The starting index.</param>
        /// <param name="end">The ending index.</param>
        /// <returns>A new range with the specified start and end indexes.</returns>
        public static Range Create(Index start, Index end) {
            return new Range(start, end);
        }

        /// <summary>Creates a new range starting from an index and extending to the end.</summary>
        /// <param name="start">The starting index.</param>
        /// <returns>A range which spans from the specified start to the end.</returns>
        public static Range StartAt(Index start) {
            return new Range(start, new Index(0, true));
        }

        /// <summary>Creates a new range starting from the beginning and extending to the specified index.</summary>
        /// <param name="end">The end index of the range.</param>
        /// <returns>A range which spans from the start to the specified index.</returns>
        public static Range EndAt(Index end) {
            return new Range(0, end);
        }

        /// <summary>Creates a new range starting from the beginning and extending to the end.</summary>
        /// <returns>A range which covers all possible indexes.</returns>
        public static Range All => new Range(Index.Start, Index.End);
        
        /// <inheritdoc />
        public IEnumerator<int> GetEnumerator() {
            int start = this.Start.Value * (this.Start.IsFromEnd ? -1 : 1);
            int end = this.End.Value * (this.End.IsFromEnd ? -1 : 1);
            return Enumerable.Range(start, end - start).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public bool Equals(Range other) {
            return this.Start.Equals(other.Start) && this.End.Equals(other.End);
        }

        public override int GetHashCode() {
            return unchecked(this.Start.GetHashCode() ^ 397 + this.End.GetHashCode());
        }

        public override string ToString() {
            return $"{this.Start}..{this.End}";
        }

        public readonly struct OffsetAndLength {
            public int Offset { get; }
            public int Length { get; }

            public OffsetAndLength(int offset, int length) {
                Offset = offset;
                Length = length;
            }

            public void Deconstruct(out int offset, out int length) {
                offset = Offset;
                length = Length;
            }
        }
    }
}