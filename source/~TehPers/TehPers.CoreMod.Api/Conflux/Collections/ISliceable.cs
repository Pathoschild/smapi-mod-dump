using System;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public interface ISliceable<T> : IReadonlySliceable<T>, IArrayLike<T> {
        /// <summary>Selects a range of elements.</summary>
        /// <param name="range">The range to select. Either use a <see cref="Range"/> object, or use the syntax <c>(start, end)</c>. <see cref="ValueTuple"/> is required for the second syntax.</param>
        /// <returns>A reference to a slice of elements.</returns>
        new Slice<T> this[Range range] { get; }
    }
}