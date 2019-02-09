using System.Collections;
using System.Collections.Generic;

namespace TehPers.Core.Collections {
    public readonly struct ReadonlySlice<T> : IReadonlySliceable<T> {
        private readonly ISliceable<T> _source;
        private readonly int _start;
        public int Length { get; }

        public ReadonlySlice(ISliceable<T> source, Range range) : this(source, range.Start, range.End) { }
        public ReadonlySlice(ISliceable<T> source, Index start, Index end) {
            this._source = source;
            this._start = start.Resolve(source);
            this.Length = end.Resolve(source) - this._start;
        }

        public ReadonlySlice(ISliceable<T> source) : this(source, 0, source.Length) { }
        public ReadonlySlice(ISliceable<T> source, int start, int length) {
            this._source = source;
            this._start = start;
            this.Length = length;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < this.Length; i++) {
                yield return this._source[this._start + i];
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <inheritdoc cref="IArrayLike{T}"/> />
        public T this[Index index] => this._source[index.Resolve(this)];

        /// <inheritdoc />
        public ReadonlySlice<T> this[Range range] {
            get {
                int rangeStart = range.Start.Resolve(this);
                int rangeLength = range.End.Resolve(this) - rangeStart;
                return new ReadonlySlice<T>(this._source, this._start + rangeStart, rangeLength);
            }
        }
    }
}