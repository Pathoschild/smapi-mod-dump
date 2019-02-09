using System;

namespace TehPers.Core.Collections {
    public interface IReadonlySliceable<T> : IReadonlyArrayLike<T> {
        /// <summary>Selects a range of elements.</summary>
        /// <param name="range">The range to select. Either use a <see cref="Range"/> object, or use the syntax <c>(start, end)</c>. <see cref="ValueTuple"/> is required for the second syntax.</param>
        /// <returns>A reference to a slice of elements.</returns>
        ReadonlySlice<T> this[Range range] { get; }
    }
}