using System.Collections;
using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public readonly struct Range : IEnumerable<int> {
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
        public static Range From(Index start) {
            return new Range(start, new Index(0, true));
        }

        /// <summary>Creates a new range starting from the beginning and extending to the specified index.</summary>
        /// <param name="end">The end index of the range.</param>
        /// <returns>A range which spans from the start to the specified index.</returns>
        public static Range To(Index end) {
            return new Range(0, end);
        }

        /// <summary>Creates a new range starting from the beginning and extending to the end.</summary>
        /// <returns>A range which covers all possible indexes.</returns>
        public static Range All() {
            return new Range(0, new Index(0, true));
        }

        public static implicit operator Range((int Start, int End) range) {
            return new Range(range.Start, range.End);
        }

        /// <inheritdoc />
        public IEnumerator<int> GetEnumerator() {
            int start = this.Start.Value * (this.Start.FromEnd ? -1 : 1);
            int end = this.End.Value * (this.End.FromEnd ? -1 : 1);
            for (int n = start; n <= end; n++) {
                yield return n;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}