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
    public class ArrayX<T> : ISliceable<T> {
        private readonly T[] _source;

        /// <inheritdoc />
        public int Length => this._source.Length;

        public ArrayX(int length) : this(new T[length]) { }
        private ArrayX(T[] source) {
            this._source = source;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>) this._source).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this._source.GetEnumerator();
        }

        /// <inheritdoc cref="IArrayLike{T}" />
        public T this[Index index] {
            get => this._source[index.Resolve(this)];
            set => this._source[index.Resolve(this)] = value;
        }

        /// <inheritdoc />
        public Slice<T> this[Range range] => new Slice<T>(this, range);

        /// <inheritdoc />
        ReadonlySlice<T> IReadonlySliceable<T>.this[Range range] => this[range];

        /// <summary>Creates a new <see cref="ListX{T}"/> that is linked to an existing list. Both lists will use the same memory.</summary>
        /// <param name="source">The source list.</param>
        /// <returns>A <see cref="ListX{T}"/> that shares the same memory as the source list.</returns>
        public static ArrayX<T> FromLinked(T[] source) {
            return new ArrayX<T>(source);
        }
    }
}