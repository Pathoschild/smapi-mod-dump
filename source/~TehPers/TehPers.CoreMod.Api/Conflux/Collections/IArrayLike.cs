namespace TehPers.CoreMod.Api.Conflux.Collections {
    public interface IArrayLike<T> : IReadonlyArrayLike<T> {
        /// <summary>Selects an element.</summary>
        /// <param name="index">The index of the element to select.</param>
        /// <returns>The selected element.</returns>
        new T this[Index index] { get; set; }
    }
}