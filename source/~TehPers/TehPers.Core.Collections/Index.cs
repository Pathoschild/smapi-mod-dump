using System;
using System.Collections.Generic;

namespace TehPers.Core.Collections {
    public readonly struct Index {
        private readonly int _value;

        /// <summary>The offset from either the start or end.</summary>
        public int Value => this._value < 0 ? ~this._value : this._value;

        /// <summary>Whether to offset from the end.</summary>
        public bool FromEnd => this._value < 0;

        public Index(int index) : this(index, false) { }
        public Index(int index, bool fromEnd) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must not be less than zero.");
            }

            this._value = fromEnd ? ~index : index;
        }

        public int Resolve<T>(IReadonlyArrayLike<T> source) => this.Resolve(source.Length);
        public int Resolve<T>(IList<T> source) => this.Resolve(source.Count);
        public int Resolve(int length) {
            if (this.Value >= length) {
                throw new IndexOutOfRangeException("Index must be within the bounds of the source.");
            }

            return this.FromEnd ? length + this._value : this._value;
        }

        public static implicit operator Index(int value) {
            return value < 0 ? new Index(~value, true) : new Index(value, false);
        }
    }
}