using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Core.Collections {
    public readonly struct ImmutableArray<T> : IList<T>, IReadOnlyList<T>, IEquatable<ImmutableArray<T>> {
        private readonly T[] _source;

        /// <inheritdoc cref="IReadOnlyList{T}.Count" />
        public int Count => this._source.Length;

        /// <inheritdoc />
        public bool IsReadOnly => true;

        public ImmutableArray(IEnumerable<T> source) {
            this._source = source.ToArray();
        }

        public ImmutableArray(T[] source) {
            this._source = source;
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>) this._source).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(T item) {
            throw new InvalidOperationException();
        }

        public void Clear() {
            throw new InvalidOperationException();
        }

        public bool Contains(T item) {
            throw new InvalidOperationException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this._source.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            throw new InvalidOperationException();
        }

        public int IndexOf(T item) {
            return Array.IndexOf(this._source, item);
        }

        public void Insert(int index, T item) {
            throw new InvalidOperationException();
        }

        public void RemoveAt(int index) {
            throw new InvalidOperationException();
        }

        public T this[int index] {
            get => this._source[index];
            set => throw new InvalidOperationException();
        }

        public bool Equals(ImmutableArray<T> other) {
            if (this.Count != other.Count) {
                return false;
            }

            for (int i = 0; i < this.Count; i++) {
                if (!object.Equals(this._source[i], other[i])) {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj) {
            return obj is ImmutableArray<T> other && this.Equals(other);
        }

        public override int GetHashCode() {
            return this._source != null ? this._source.GetHashCode() : 0;
        }
    }
}
