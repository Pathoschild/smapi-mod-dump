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
using System.Collections;
using System.Collections.Generic;
using TehPers.CoreMod.Api.Conflux.Extensions;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public class ListX<T> : ISliceableList<T> {
        private readonly IList<T> _source;

        public ListX() : this(new List<T>()) { }
        public ListX(int capacity) : this(new List<T>(capacity)) { }
        public ListX(IEnumerable<T> source) : this(new List<T>(source)) { }
        private ListX(IList<T> source) {
            this._source = source;
        }

        #region IList<T>
        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() {
            return this._source.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) this._source).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(T item) {
            this._source.Add(item);
        }

        /// <inheritdoc />
        public void Clear() {
            this._source.Clear();
        }

        /// <inheritdoc />
        public bool Contains(T item) {
            return this._source.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) {
            this._source.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T item) {
            return this._source.Remove(item);
        }

        /// <inheritdoc />
        public int Count => this._source.Count;

        /// <inheritdoc />
        public bool IsReadOnly => this._source.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(T item) {
            return this._source.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T item) {
            this._source.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index) {
            this._source.RemoveAt(index);
        }

        /// <inheritdoc />
        public T this[int index] {
            get => this._source[index];
            set => this._source[index] = value;
        }
        #endregion

        #region ISliceable<T>
        /// <inheritdoc />
        public int Length => this._source.Count;

        /// <inheritdoc cref="IArrayLike{T}" />
        public T this[Index index] {
            get => this._source[index.Resolve((IList<T>) this)];
            set => this._source[index.Resolve((IList<T>) this)] = value;
        }

        /// <inheritdoc />
        public Slice<T> this[Range range] => new Slice<T>(this, range);
        #endregion

        /// <inheritdoc />
        ReadonlySlice<T> IReadonlySliceable<T>.this[Range range] => this[range];

        /// <summary>Creates a new <see cref="ListX{T}"/> that is linked to an existing list. Both lists will use the same memory.</summary>
        /// <param name="source">The source list.</param>
        /// <returns>A <see cref="ListX{T}"/> that shares the same memory as the source list.</returns>
        public static ListX<T> FromLinked(IList<T> source) {
            return new ListX<T>(source);
        }
    }
}
