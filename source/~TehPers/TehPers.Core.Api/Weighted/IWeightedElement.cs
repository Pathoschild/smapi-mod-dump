namespace TehPers.Core.Api.Weighted {
    /// <summary>A wrapper for objects that assigns a weighted chance to them.</summary>
    /// <typeparam name="T">The type of object being wrapped.</typeparam>
    public interface IWeightedElement<out T> : IWeighted {
        /// <summary>The value wrapped by this element.</summary>
        T Value { get; }
    }
}